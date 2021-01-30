
namespace PowerLinesMessaging
{
    public class ConsumerOptions : Options
    {
        public string RoutingKey { get; set; }
        public string SubscriptionQueueName { get; set; }
    }
}
