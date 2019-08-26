//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Text;

//namespace ClientService.Infrastructure.Helpers
//{
//    public class PropertyCollection<T> : IEnumerable<T>
//    {
//        private List<T> _properties;

//        private IEnumerable<T> Properties
//        {
//            get
//            {
//                if (_properties == null)
//                {
//                    _properties = new List<T>();
//                    _properties.AddRange(ReflectionHelper.GetProperties<T>(this));
//                }

//                return _properties;
//            }
//        }

//        public PropertyCollection()
//        {
            
//        }

//        public IEnumerator<T> GetEnumerator()
//        {
//            return Properties.GetEnumerator();
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return Properties.GetEnumerator();
//        }
//    }
//}
