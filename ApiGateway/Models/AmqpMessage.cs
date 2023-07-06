namespace ApiGateway.Models
{
    public class AmqpMessage
    {
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public string ResponseQueue { get; set; }
        public string Message { get; set; }

    }
}
