//using ServiceBus.Configurations;
//using System.Collections.Generic;

//namespace ServiceBus
//{
//    public class ConsumerMap : List<KeyValuePair<ConsumerSettings, IEventConsumer>>
//    {
//        public void Add(ConsumerSettings config, IEventConsumer consumer)
//        {
//            if (config != null && consumer != null)
//                Add(new KeyValuePair<ConsumerSettings, IEventConsumer>(config, consumer));
//        }

//        public void Add(ConsumerSettings config, IEnumerable<IEventConsumer> consumers)
//        {
//            foreach (var c in consumers)
//                Add(config, c);
//        }

//        public IEnumerable<IEventConsumer> GetConsumers(ConsumerSettings key)
//        {
//            return FindAll(p => p.Key.Equals(key)).ConvertAll(p => p.Value);
//        }
//    }
//}
