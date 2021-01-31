namespace PowerLinesMessaging
{
    public interface ISender
    {
        void SendMessage(object obj, string routingKey = "");
    }
}
