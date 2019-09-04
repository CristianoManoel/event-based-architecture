// using System.Collections.Concurrent;
// using System.Collections.Generic;

// namespace WebTest.Infrastructure
// {
//     public class Repository<T>
//     {
//         private readonly ConcurrentDictionary<object, T> items = new ConcurrentDictionary<object, T>();

//         public void Add(object key, T item)
//         {
//             items.TryAdd(key, item);
//         }

//         public IEnumerable<T> GetAll()
//         {
//             return items.Values;
//         }
//     }
// }
