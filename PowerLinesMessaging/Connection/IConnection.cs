namespace PowerLinesMessaging
{
    public interface IConnection
    {
        void CreateConnection(ConnectionOptions options);

        void CloseConnection();
    }
}
