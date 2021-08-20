using System;

namespace PowerLinesMessaging
{
    public class BrokerUrl
    {
        private readonly string host;
        private readonly int port;
        private readonly string username;
        private readonly string password;

        public BrokerUrl(string host, int port, string username, string password)
        {
            this.host = host;
            this.port = port;
            this.username = username;
            this.password = password;
        }

        public override string ToString()
        {
            return string.Format("amqp://{0}:{1}@{2}:{3}/", username, password, host, port);            
        }
    }
}
