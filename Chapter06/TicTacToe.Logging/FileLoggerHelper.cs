using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TicTacToe.Logging
{
    public class FileLoggerHelper
    {
        private string fileName;

        public FileLoggerHelper(string fileName)
        {
            this.fileName = fileName;
        }

        static ReaderWriterLock locker = new ReaderWriterLock();

        public void InsertLog(LogEntry logEntry)
        {
            var directory = System.IO.Path.GetDirectoryName(fileName);

            if (!System.IO.Directory.Exists(directory))
                System.IO.Directory.CreateDirectory(directory);

            try
            {
                locker.AcquireWriterLock(int.MaxValue);
                System.IO.File.AppendAllText(fileName,
                  $"{logEntry.CreatedTime} {logEntry.EventId} { logEntry.LogLevel}  { logEntry.Message}  " + 
                Environment.NewLine);
            }
            finally
            {
                locker.ReleaseWriterLock();
            }
        }
    }
}
