namespace PowerLinesMessaging;

public class BrokerUrl(string host, int port, string username, string password)
{
    private readonly string host = host;
    private readonly int port = port;
    private readonly string username = username;
    private readonly string password = password;

    public override string ToString()
    {
        return string.Format("amqp://{0}:{1}@{2}:{3}/", username, password, host, port);
    }
}
