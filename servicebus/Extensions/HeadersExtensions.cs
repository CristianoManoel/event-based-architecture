using Confluent.Kafka;
using System;
using System.Text;

namespace ServiceBus.Extensions
{
    public static class HeadersExtensions
    {
        public static bool ExistsKey(this Headers headers, string key)
        {
            if (headers.TryGetLastBytes(key, out _))
                return true;

            return false;
        }

        public static bool TryGetValueString(this Headers headers, string key, out string value)
        {
            value = default;

            if (headers.TryGetLastBytes(key, out byte[] b))
            {
                value = Encoding.UTF8.GetString(b);
                return true;
            }

            return false;
        }

        public static bool TryGetValueBoolean(this Headers headers, string key, out bool value)
        {
            value = default;

            if (headers.TryGetLastBytes(key, out byte[] b))
            {
                value = BitConverter.ToBoolean(b, 0);
                return true;
            }

            return false;
        }

        public static bool TryGetValueInt32(this Headers headers, string key, out int value)
        {
            value = default;

            if (headers.TryGetLastBytes(key, out byte[] b))
            {   
                value = BitConverter.ToInt32(b, 0);
                return true;
            }

            return false;
        }

        public static void AddOrUpdate(this Headers headers, string key, int value)
        {
            headers.Remove(key);
            headers.Add(key, BitConverter.GetBytes(value));
        }

        public static void AddOrUpdate(this Headers headers, string key, long value)
        {
            headers.Remove(key);
            headers.Add(key, BitConverter.GetBytes(value));
        }

        public static void AddOrUpdate(this Headers headers, string key, bool value)
        {
            headers.Remove(key);
            headers.Add(key, BitConverter.GetBytes(value));
        }

        public static void AddOrUpdate(this Headers headers, string key, string value)
        {
            headers.Remove(key);
            headers.Add(key, System.Text.Encoding.UTF8.GetBytes(value));
        }

    }
}
