using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TicTacToe.Logging;
using Xunit;

namespace TicTacToe.UnitTests
{
    public class FileLoggerTests
    {
        [Fact]
        public void ShouldCreateALogFileAndAddEntry()
        {
            var fileLogger = new FileLogger(
            "Test", (category, level) => true,
            Path.Combine(Directory.GetCurrentDirectory(), "testlog.log"));
            var isEnabled = fileLogger.IsEnabled(LogLevel.Information);
            Assert.True(isEnabled);
        }
    }
}
