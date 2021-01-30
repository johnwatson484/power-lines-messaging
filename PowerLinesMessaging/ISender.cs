namespace PowerLinesMessaging
{
    public interface ISender
    {
        void CreateConnectionToQueue(SenderOptions options);

        void CloseConnection();

        void SendMessage(object obj, string routingKey = "");
    }
}
