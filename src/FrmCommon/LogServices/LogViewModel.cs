using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FrmCommon.Mvvm;

namespace FrmServices.LogServices
{
    public sealed class LogViewModel : ReactiveObject, IDisposable
    {
        public const int DefaultMaximumLiveEntries = 5000;
        public const int DefaultMaximumHistoryEntries = 10000;

        private readonly object _entriesLock = new object();
        private readonly Queue<LogEntry> _liveEntries = new Queue<LogEntry>();
        private readonly LogService _service;
        private int _maximumLiveEntries = DefaultMaximumLiveEntries;
        private int _maximumHistoryEntries = DefaultMaximumHistoryEntries;
        private bool _disposed;

        public LogViewModel() : this(LogService.Default) { }

        public LogViewModel(LogService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _service.EntryWritten += OnEntryWritten;
            _service.StorageError += OnStorageError;
        }

        public event EventHandler<LogEntryEventArgs> LiveEntryReceived;
        public event EventHandler<LogStorageErrorEventArgs> StorageError;
        public string LogDirectory => _service.LogDirectory;
        public int RetentionDays => _service.RetentionDays;

        public int MaximumLiveEntries
        {
            get { lock (_entriesLock) return _maximumLiveEntries; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), "实时日志最大条数必须大于 0。");

                lock (_entriesLock)
                {
                    if (_maximumLiveEntries == value) return;
                    _maximumLiveEntries = value;
                    TrimLiveEntries();
                }
                OnPropertyChanged(nameof(MaximumLiveEntries));
            }
        }

        public int MaximumHistoryEntries
        {
            get { lock (_entriesLock) return _maximumHistoryEntries; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(value), "历史日志最大查询条数必须大于 0。");

                lock (_entriesLock)
                {
                    if (_maximumHistoryEntries == value) return;
                    _maximumHistoryEntries = value;
                }
                OnPropertyChanged(nameof(MaximumHistoryEntries));
            }
        }

        public IList<LogEntry> GetLiveEntries(LogLevel? level, string keyword)
        {
            lock (_entriesLock)
            {
                return Filter(_liveEntries, level, keyword)
                    .OrderByDescending(entry => entry.Timestamp).ToList();
            }
        }

        public async Task<IList<LogEntry>> GetHistoryAsync(
            DateTime from, DateTime to, LogLevel? level, string keyword)
        {
            var history = await _service.ReadHistoryAsync(from, to).ConfigureAwait(false);
            int maximumEntries;
            lock (_entriesLock) maximumEntries = _maximumHistoryEntries;
            return Filter(history, level, keyword)
                .OrderByDescending(entry => entry.Timestamp)
                .Take(maximumEntries)
                .ToList();
        }

        public void ClearLiveEntries()
        {
            lock (_entriesLock) _liveEntries.Clear();
        }

        public void SaveSettings(string directory, int retentionDays)
        {
            _service.UpdateSettings(directory, retentionDays);
            OnPropertyChanged(nameof(LogDirectory));
            OnPropertyChanged(nameof(RetentionDays));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _service.EntryWritten -= OnEntryWritten;
            _service.StorageError -= OnStorageError;
            lock (_entriesLock)
            {
                _disposed = true;
                _liveEntries.Clear();
            }
        }

        private static IEnumerable<LogEntry> Filter(
            IEnumerable<LogEntry> entries, LogLevel? level, string keyword)
        {
            var result = entries;
            if (level.HasValue) result = result.Where(entry => entry.Level == level.Value);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var term = keyword.Trim();
                result = result.Where(entry =>
                    entry.Message.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    entry.Source.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            return result;
        }

        private void OnEntryWritten(object sender, LogEntryEventArgs e)
        {
            lock (_entriesLock)
            {
                if (_disposed) return;
                _liveEntries.Enqueue(e.Entry);
                TrimLiveEntries();
            }
            LiveEntryReceived?.Invoke(this, e);
        }

        private void TrimLiveEntries()
        {
            while (_liveEntries.Count > _maximumLiveEntries)
                _liveEntries.Dequeue();
        }

        private void OnStorageError(object sender, LogStorageErrorEventArgs e) =>
            StorageError?.Invoke(this, e);
    }
}
