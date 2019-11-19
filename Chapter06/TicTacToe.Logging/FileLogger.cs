using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace TicTacToe.Logging
{
    public sealed class FileLogger : ILogger
    {
        private string _categoryName; 
          private Func<string, LogLevel, bool> _filter; 
          private string _fileName; 
          private FileLoggerHelper _helper; 
 
          public FileLogger(string categoryName, Func<string, LogLevel,
           bool> filter, string fileName) 
          { 
            _categoryName = categoryName; 
            _filter = filter; 
            _fileName = fileName; 
            _helper = new FileLoggerHelper(fileName); 
          } 
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }


        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(_categoryName, logLevel)); 
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))  return;            

            if (formatter == null)            
                throw new ArgumentNullException(nameof(formatter));
            
            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))  return;
            
            if (exception != null)  message += "\n" + exception.ToString();
            
            var logEntry = new LogEntry
            {
                Message = message,
                EventId = eventId.Id,
                LogLevel = logLevel.ToString(),
                CreatedTime = DateTime.UtcNow
            };

            _helper.InsertLog(logEntry);
        }        
    }
}
