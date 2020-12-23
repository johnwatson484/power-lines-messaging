namespace PowerLinesMessaging
{
    public interface ISender
    {
        void CreateConnectionToQueue(QueueType queueType, string brokerUrl, string queue);

        void CloseConnection();

        void SendMessage(object obj, string routingKey = "");
    }
}
