using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FrmServices.LogServices
{
    public sealed class LogService
    {
        private const string FilePrefix = "frmvision-";
        private const string FileExtension = ".log";
        private const int MaximumQueuedEntries = 20000;
        private const int MaximumWriteBatchSize = 256;
        private const int FlushEnqueueWaitMilliseconds = 25;
        private const int ProcessExitFlushMilliseconds = 500;
        private static readonly Encoding LogEncoding = new UTF8Encoding(false);
        private static readonly Lazy<LogService> DefaultInstance =
            new Lazy<LogService>(() => new LogService());

        private readonly object _settingsLock = new object();
        private readonly BlockingCollection<PendingLogEntry> _writeQueue =
            new BlockingCollection<PendingLogEntry>(
                new ConcurrentQueue<PendingLogEntry>(), MaximumQueuedEntries);
        private readonly string _settingsFilePath;
        private readonly Task _writerTask;
        private DateTime _lastCleanupDate = DateTime.MinValue;
        private string _logDirectory;
        private int _retentionDays;
        private long _droppedWriteCount;

        public LogService()
        {
            var settingsDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "FrmVision");
            _settingsFilePath = Path.Combine(settingsDirectory, "log-settings.xml");
            LoadSettings();
            _writerTask = Task.Factory.StartNew(ProcessWriteQueue,
                CancellationToken.None, TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
                Flush(TimeSpan.FromMilliseconds(ProcessExitFlushMilliseconds));
        }

        public static LogService Default => DefaultInstance.Value;
        public event EventHandler<LogEntryEventArgs> EntryWritten;
        public event EventHandler<LogStorageErrorEventArgs> StorageError;

        public string LogDirectory
        {
            get { lock (_settingsLock) return _logDirectory; }
        }

        public int RetentionDays
        {
            get { lock (_settingsLock) return _retentionDays; }
        }

        public void Debug(string message, string source = null) => Write(LogLevel.Debug, message, source);
        public void Info(string message, string source = null) => Write(LogLevel.Info, message, source);
        public void Warning(string message, string source = null) => Write(LogLevel.Warning, message, source);
        public void Error(string message, string source = null) => Write(LogLevel.Error, message, source);
        public void Critical(string message, string source = null) => Write(LogLevel.Critical, message, source);

        public void Write(LogLevel level, string message, string source = null)
        {
            if (!Enum.IsDefined(typeof(LogLevel), level))
                throw new ArgumentOutOfRangeException(nameof(level));

            var entry = new LogEntry(DateTime.Now, level, message, source);
            if (!_writeQueue.TryAdd(new PendingLogEntry(entry, LogDirectory)))
                Interlocked.Increment(ref _droppedWriteCount);
            RaiseEntryWritten(entry);
        }

        public bool Flush(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero) return false;

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var completion = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            try
            {
                int enqueueWait = Math.Min(FlushEnqueueWaitMilliseconds,
                    Math.Max(0, (int)Math.Min(int.MaxValue,
                        timeout.TotalMilliseconds)));
                if (!_writeQueue.TryAdd(
                        PendingLogEntry.CreateFlushMarker(completion), enqueueWait))
                    return false;

                TimeSpan remaining = timeout - stopwatch.Elapsed;
                return remaining > TimeSpan.Zero &&
                       completion.Task.Wait(remaining);
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public Task<bool> FlushAsync(TimeSpan timeout) =>
            Task.Run(() => Flush(timeout));

        public void UpdateSettings(string logDirectory, int retentionDays)
        {
            if (string.IsNullOrWhiteSpace(logDirectory))
                throw new ArgumentException("日志路径不能为空。", nameof(logDirectory));
            if (retentionDays < 1 || retentionDays > 3650)
                throw new ArgumentOutOfRangeException(nameof(retentionDays), "日志保留天数必须在 1 到 3650 之间。");

            var fullPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables(logDirectory.Trim()));
            Directory.CreateDirectory(fullPath);

            lock (_settingsLock)
            {
                SaveSettings(fullPath, retentionDays);
                _logDirectory = fullPath;
                _retentionDays = retentionDays;
                _lastCleanupDate = DateTime.MinValue;
            }

            CleanupExpiredFiles(fullPath, retentionDays);
        }

        public Task<IList<LogEntry>> ReadHistoryAsync(DateTime from, DateTime to)
        {
            if (to < from)
                throw new ArgumentException("结束时间不能早于开始时间。", nameof(to));

            var directory = LogDirectory;
            return Task.Run<IList<LogEntry>>(() => ReadHistory(directory, from, to));
        }

        private void LoadSettings()
        {
            var defaultDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "FrmVision", "Logs");
            _logDirectory = defaultDirectory;
            _retentionDays = 30;

            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var document = XDocument.Load(_settingsFilePath);
                    var root = document.Root;
                    var directoryValue = root?.Element("Directory")?.Value;
                    var retentionValue = root?.Element("RetentionDays")?.Value;

                    if (!string.IsNullOrWhiteSpace(directoryValue))
                        _logDirectory = Path.GetFullPath(directoryValue);
                    if (int.TryParse(retentionValue, out var days) && days >= 1 && days <= 3650)
                        _retentionDays = days;
                }

                Directory.CreateDirectory(_logDirectory);
            }
            catch (Exception ex)
            {
                _logDirectory = defaultDirectory;
                _retentionDays = 30;
                RaiseStorageError(ex);
            }
        }

        private void SaveSettings(string directory, int retention)
        {
            var settingsDirectory = Path.GetDirectoryName(_settingsFilePath);
            Directory.CreateDirectory(settingsDirectory);

            var document = new XDocument(
                new XElement("LogSettings",
                    new XElement("Directory", directory),
                    new XElement("RetentionDays", retention)));
            document.Save(_settingsFilePath);
        }

        private void ProcessWriteQueue()
        {
            var batch = new List<PendingLogEntry>(MaximumWriteBatchSize + 1);
            while (true)
            {
                PendingLogEntry pending;
                try
                {
                    pending = _writeQueue.Take();
                }
                catch (InvalidOperationException)
                {
                    return;
                }

                if (pending.FlushCompletion != null)
                {
                    pending.FlushCompletion.TrySetResult(true);
                    continue;
                }

                batch.Clear();
                batch.Add(pending);
                TaskCompletionSource<bool> flushCompletion = null;

                while (batch.Count < MaximumWriteBatchSize &&
                       _writeQueue.TryTake(out pending))
                {
                    if (pending.FlushCompletion != null)
                    {
                        flushCompletion = pending.FlushCompletion;
                        break;
                    }

                    batch.Add(pending);
                }

                long droppedEntries = Interlocked.Exchange(
                    ref _droppedWriteCount, 0);
                if (droppedEntries > 0)
                {
                    var overloadEntry = new LogEntry(DateTime.Now,
                        LogLevel.Warning,
                        "日志写入队列已满，本批次丢弃 " + droppedEntries +
                        " 条磁盘日志。", nameof(LogService));
                    batch.Add(new PendingLogEntry(overloadEntry, LogDirectory));
                    RaiseEntryWritten(overloadEntry);
                }

                WriteBatch(batch);
                flushCompletion?.TrySetResult(true);
            }
        }

        private void WriteBatch(IList<PendingLogEntry> batch)
        {
            var fileContents = new Dictionary<string, StringBuilder>(
                StringComparer.OrdinalIgnoreCase);
            var directories = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            foreach (PendingLogEntry pending in batch)
            {
                string filePath = Path.Combine(pending.Directory,
                    FilePrefix + pending.Entry.Timestamp.ToString("yyyy-MM-dd") +
                    FileExtension);
                if (!fileContents.TryGetValue(filePath, out StringBuilder content))
                {
                    content = new StringBuilder();
                    fileContents.Add(filePath, content);
                }

                content.Append(Serialize(pending.Entry));
                content.Append(Environment.NewLine);
                directories.Add(pending.Directory);
            }

            foreach (KeyValuePair<string, StringBuilder> file in fileContents)
            {
                try
                {
                    string directory = Path.GetDirectoryName(file.Key);
                    if (!string.IsNullOrEmpty(directory))
                        Directory.CreateDirectory(directory);
                    File.AppendAllText(file.Key, file.Value.ToString(), LogEncoding);
                }
                catch (Exception ex)
                {
                    RaiseStorageError(ex);
                }
            }

            foreach (string directory in directories)
                CleanupIfNeeded(directory);
        }

        private void CleanupIfNeeded(string directory)
        {
            var today = DateTime.Today;
            int retention;
            lock (_settingsLock)
            {
                if (_lastCleanupDate == today) return;
                _lastCleanupDate = today;
                retention = _retentionDays;
            }
            CleanupExpiredFiles(directory, retention);
        }

        private void CleanupExpiredFiles(string directory, int retentionDays)
        {
            try
            {
                if (!Directory.Exists(directory)) return;
                var cutoff = DateTime.Now.AddDays(-retentionDays);
                foreach (var file in Directory.EnumerateFiles(directory, FilePrefix + "*" + FileExtension))
                {
                    if (File.GetLastWriteTime(file) < cutoff) File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                RaiseStorageError(ex);
            }
        }

        private static IList<LogEntry> ReadHistory(string directory, DateTime from, DateTime to)
        {
            var entries = new List<LogEntry>();
            if (!Directory.Exists(directory)) return entries;

            foreach (var file in Directory.EnumerateFiles(directory, FilePrefix + "*" + FileExtension)
                         .Where(path => IsFileInRange(path, from, to))
                         .OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
            {
                try
                {
                    using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(stream, Encoding.UTF8, true))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var entry = Deserialize(line);
                            if (entry != null && entry.Timestamp >= from && entry.Timestamp <= to)
                                entries.Add(entry);
                        }
                    }
                }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }

            return entries.OrderByDescending(entry => entry.Timestamp).ToList();
        }

        private static bool IsFileInRange(string path, DateTime from, DateTime to)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            if (string.IsNullOrEmpty(fileName) || !fileName.StartsWith(FilePrefix, StringComparison.OrdinalIgnoreCase))
                return false;

            var datePart = fileName.Substring(FilePrefix.Length);
            return DateTime.TryParseExact(datePart, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out var fileDate) &&
                   fileDate >= from.Date && fileDate <= to.Date;
        }

        private static string Serialize(LogEntry entry)
        {
            return string.Join("\t", new[]
            {
                entry.Timestamp.ToString("O", CultureInfo.InvariantCulture),
                entry.Level.ToString(), Escape(entry.Source), Escape(entry.Message)
            });
        }

        private static LogEntry Deserialize(string line)
        {
            var values = line.Split(new[] { '\t' }, 4);
            if (values.Length != 4 ||
                !DateTime.TryParseExact(values[0], "O", CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind, out var timestamp) ||
                !Enum.TryParse(values[1], true, out LogLevel level)) return null;

            return new LogEntry(timestamp, level, Unescape(values[3]), Unescape(values[2]));
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("\\", "\\\\")
                .Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n");
        }

        private static string Unescape(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var result = new StringBuilder(value.Length);
            var escaping = false;
            foreach (var character in value)
            {
                if (!escaping)
                {
                    if (character == '\\') escaping = true;
                    else result.Append(character);
                    continue;
                }

                switch (character)
                {
                    case 't': result.Append('\t'); break;
                    case 'r': result.Append('\r'); break;
                    case 'n': result.Append('\n'); break;
                    default: result.Append(character); break;
                }
                escaping = false;
            }
            if (escaping) result.Append('\\');
            return result.ToString();
        }

        private void RaiseEntryWritten(LogEntry entry)
        {
            var handlers = EntryWritten;
            if (handlers == null) return;
            foreach (EventHandler<LogEntryEventArgs> handler in handlers.GetInvocationList())
            {
                try { handler(this, new LogEntryEventArgs(entry)); }
                catch { }
            }
        }

        private void RaiseStorageError(Exception exception)
        {
            var handlers = StorageError;
            if (handlers == null) return;
            foreach (EventHandler<LogStorageErrorEventArgs> handler in handlers.GetInvocationList())
            {
                try { handler(this, new LogStorageErrorEventArgs(exception)); }
                catch { }
            }
        }

        private sealed class PendingLogEntry
        {
            public PendingLogEntry(LogEntry entry, string directory)
            {
                Entry = entry;
                Directory = directory;
            }
            private PendingLogEntry(TaskCompletionSource<bool> flushCompletion)
            {
                FlushCompletion = flushCompletion;
            }
            public static PendingLogEntry CreateFlushMarker(TaskCompletionSource<bool> completion) =>
                new PendingLogEntry(completion);
            public LogEntry Entry { get; }
            public string Directory { get; }
            public TaskCompletionSource<bool> FlushCompletion { get; }
        }
    }

    public static class AppLog
    {
        public static void Debug(string message, string source = null) => LogService.Default.Debug(message, source);
        public static void Info(string message, string source = null) => LogService.Default.Info(message, source);
        public static void Warning(string message, string source = null) => LogService.Default.Warning(message, source);
        public static void Error(string message, string source = null) => LogService.Default.Error(message, source);
        public static void Critical(string message, string source = null) => LogService.Default.Critical(message, source);
        public static void Write(LogLevel level, string message, string source = null) =>
            LogService.Default.Write(level, message, source);
    }
}
