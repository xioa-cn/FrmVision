using System;

namespace FrmServices.LogServices
{
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Critical = 4
    }

    public sealed class LogEntry
    {
        public LogEntry(DateTime timestamp, LogLevel level, string message, string source = null)
        {
            Timestamp = timestamp;
            Level = level;
            Message = message ?? string.Empty;
            Source = source ?? string.Empty;
        }

        public DateTime Timestamp { get; }
        public LogLevel Level { get; }
        public string Message { get; }
        public string Source { get; }
        public string DisplayTime => Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");

        public string LevelText
        {
            get
            {
                switch (Level)
                {
                    case LogLevel.Debug: return "调试";
                    case LogLevel.Info: return "信息";
                    case LogLevel.Warning: return "警告";
                    case LogLevel.Error: return "错误";
                    case LogLevel.Critical: return "严重";
                    default: return Level.ToString();
                }
            }
        }
    }

    public sealed class LogEntryEventArgs : EventArgs
    {
        public LogEntryEventArgs(LogEntry entry) { Entry = entry; }
        public LogEntry Entry { get; }
    }

    public sealed class LogStorageErrorEventArgs : EventArgs
    {
        public LogStorageErrorEventArgs(Exception exception) { Exception = exception; }
        public Exception Exception { get; }
    }
}
