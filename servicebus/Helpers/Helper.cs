using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus
{
    internal static class Helper
    {
        public static IEnumerable<string> ParseHosts(params string[] brokers)
        {

            if (brokers == null)
                throw new ArgumentNullException(nameof(brokers));

            foreach (var brocker in brokers.SelectMany(f => f.Split(',')))
            {
                if (string.IsNullOrWhiteSpace(brocker))
                    throw new ArgumentNullException(nameof(brocker));

                yield return brocker;
            }
        }
    }
}
