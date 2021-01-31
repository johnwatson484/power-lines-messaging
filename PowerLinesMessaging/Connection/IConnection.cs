namespace PowerLinesMessaging
{
    public interface IConnection
    {
        Consumer CreateConsumerChannel(ConsumerOptions options);
        Sender CreateSenderChannel(SenderOptions options);
        void CloseChannel(string name);
        void CloseConnection();
    }
}
