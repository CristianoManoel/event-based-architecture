using System;
using System.Collections.Generic;
using System.Text;

namespace Notification.Infrastructure.Helpers
{
    public static class ReflectionHelper
    {
        public static IEnumerable<T> GetProperties<T>(object instance)
        {
            var propertyInfos = instance.GetType().GetProperties();

            foreach (var p in propertyInfos)
            {
                if (p.PropertyType == typeof(T))
                    yield return (T)p.GetValue(instance);
            }
        }
    }
}
