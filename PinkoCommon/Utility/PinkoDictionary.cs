using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinkoCommon.Utility
{
    /// <summary>
    /// Concurrent / thread safe Pinko dictionary
    /// </summary>
    public class PinkoDictionary<TK, T>
    {
        /// <summary>
        /// Replace keyed item  when condition is true. Done this way to assure atomic operation in dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="conditionAction"></param>
        /// <param name="replacementAction"></param>
        /// <returns></returns>
        public T ReplaceCondition(TK key, Func<T, bool> conditionAction, Func<T, T> replacementAction)
        {
            lock (_dictionary)
            {
                var currentItem = this[key];
                if (conditionAction(currentItem))
                {
                    var item = replacementAction(currentItem);
                    _dictionary[key] = item;
                    return item;
                }
            }

            return default(T);
        }


        /// <summary>
        /// Constructor - PinkoDictionary 
        /// </summary>
        public PinkoDictionary(int iCapacity = 1000)
        {
            _dictionary = new Dictionary<TK, T>(iCapacity);
        }

        /// <summary>
        /// Count
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            lock (_dictionary)
                return _dictionary.Count();
        }


        /// <summary>
        /// Get enumerator copy of all values. 
        /// Expensive since it gets a copy of values enumerator
        /// </summary>
        /// <returns>IEnumerable copy</returns>
        public IEnumerable<T> GetEnumerator()
        {
            lock (_dictionary)
                return _dictionary
                            .Values
                            .Select(x => x);
        }


        /// <summary>
        /// indexer
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Null if item does not exists, else the item</returns>
        public T this[TK key]
        {
            get 
            {
                lock (_dictionary)
                    if (_dictionary.ContainsKey(key))
                        return _dictionary[key];

                return default(T);
            }

            set
            {
                lock (_dictionary)
                    _dictionary[key] = value;
                
            }
        }

        /// <summary>
        /// Remove item
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Item removed, else NULL</returns>
        public T Remove(TK key)
        {
            lock (_dictionary)
            {
                if (_dictionary.ContainsKey(key))
                {
                    var item = _dictionary[key];
                    _dictionary.Remove(key);
                    return item;
                }
            }

            return default(T);
        }


        /// <summary>
        /// Get item. Item added if it does not exists
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itemFactory">Factor item. It will be called if item does not exists</param>
        /// <returns>
        /// True: update existing item. - 
        /// False: inserted items because it did not exists
        /// </returns>
        public T Get(TK key, Func<T> itemFactory)
        {
            lock (_dictionary)
            {
                if (!_dictionary.ContainsKey(key))
                {
                    var item = itemFactory();
                    _dictionary[key] = item;
                    return item;
                }
            }

            return default(T);
        }



        /// <summary>
        /// Get item. Item added if it does not exists
        /// </summary>
        /// <param name="key"></param>
        /// <param name="itemFactory">Factor item. It will be called if item does not exists</param>
        /// <returns>
        /// False: if inserted.
        /// True: if updated
        /// False: inserted items because it did not exists
        /// </returns>
        public bool Update(TK key, Func<T> itemFactory)
        {
            lock (_dictionary)
            {
                if (_dictionary.ContainsKey(key))
                {
                    _dictionary[key] = itemFactory();
                    return true;
                }

                _dictionary[key] = itemFactory();
            }

            return false;
        }


        /// <summary>
        /// internal dictionary
        /// </summary>
        private readonly Dictionary<TK, T> _dictionary;
    }
}
