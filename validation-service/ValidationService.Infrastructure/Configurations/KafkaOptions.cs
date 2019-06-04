namespace ValidationService.Infrastructure.Configurations
{
    public class KafkaOptions
    {
        public ProducerOptions Producer { get; set; }
        
        public ConsumerOptions Consumer { get; set; }
    }
}