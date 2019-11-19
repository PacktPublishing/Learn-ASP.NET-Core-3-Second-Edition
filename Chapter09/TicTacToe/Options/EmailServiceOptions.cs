using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicTacToe.Options
{
    public class EmailServiceOptions
    {
        public string MailType { get; set; }
        public string MailServer { get; set; }
        public string MailPort { get; set; }
        public string UseSSL { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string RemoteServerAPI { get; set; }
        public string RemoteServerKey { get; set; }

        public EmailServiceOptions()
        {

        }

        public EmailServiceOptions(string mailType,
         string mailServer, string mailPort, string useSSL,
         string userId, string password, string remoteServerAPI,
         string remoteServerKey)
        {
            MailType = mailType;
            MailServer = mailServer;
            MailPort = mailPort;
            UseSSL = useSSL;
            UserId = userId;
            Password = password;
            RemoteServerAPI = remoteServerAPI;
            RemoteServerKey = remoteServerKey;
        }
    }
}
