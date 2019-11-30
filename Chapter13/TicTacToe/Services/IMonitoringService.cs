using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.Services
{
    public interface IMonitoringService
    {
        void TrackEvent(string eventName, TimeSpan elapsed, IDictionary<string, string> properties = null);
    }
}
