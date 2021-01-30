namespace PowerLinesMessaging
{
    public class Options
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public QueueType QueueType { get; set; }
        public string QueueName { get; set; }
        public string BrokerUrl
        {
            get
            {
                return new BrokerUrl(Host, Port, Username, Password).ToString();
            }
        }
    }
}
