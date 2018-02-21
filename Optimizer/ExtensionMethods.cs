namespace ExtensionMethods
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ComponentModel;

    /// <summary>
    /// Методы расширения для ObservableCollection
    /// </summary>
    public static class ObservableExtension
    {
        /// <summary>
        /// Удаляет все элементы в коллекции ObservableCollection&lt;T&gt;, соответствующие условиям предиката Predicate&lt;T&gt;.
        /// </summary>
        public static int RemoveAllMain<T>(this ObservableCollection<T> obsColl, Predicate<T> predicate)
            where T : class
        {
            int count = 0;

            lock (((ICollection)obsColl).SyncRoot)
            {
                for (int i = 0; i < obsColl.Count; i++)
                {
                    if (predicate(obsColl[i]))
                    {
                        obsColl.RemoveAt(i);
                        i--;
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Удаляет первое вхождение элемента в коллекции ObservableCollection&lt;T&gt;, соответствующие условиям предиката Predicate&lt;T&gt;.
        /// </summary>
        public static bool RemoveFirstMain<T>(this ObservableCollection<T> obsColl, Predicate<T> predicate)
            where T : class
        {
            lock (((ICollection)obsColl).SyncRoot)
            {
                for (int i = 0; i < obsColl.Count; i++)
                {
                    if (predicate(obsColl[i]))
                    {
                        obsColl.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Выполняет поиск элемента, удовлетворяющего условиям указанного предиката, и возвращает первое
        /// найденное вхождение в пределах всего списка.
        /// </summary>
        public static T FindMain<T>(this ObservableCollection<T> obsColl, Predicate<T> predicate) where T : class
        {
            T result = null;

            lock (((ICollection)obsColl).SyncRoot)
            {
                for (int i = 0; i < obsColl.Count; i++)
                {
                    T item = obsColl[i];

                    if (predicate(item))
                    {
                        result = item;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Определяет, содержит ли ObservableCollection&lt;T&gt; элементы, удовлетворяющие условиям указанного предиката.
        /// </summary>
        public static bool ExistsMain<T>(this ObservableCollection<T> obsColl, Predicate<T> predicate) where T : class
        {
            bool result = false;

            lock (((ICollection)obsColl).SyncRoot)
            {
                for (int i = 0; i < obsColl.Count; i++)
                {
                    T item = obsColl[i];

                    if (predicate(item))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Выполняет поиск элемента, удовлетворяющего условиям указанного предиката, и возвращает
        /// количество найденных элементов.
        /// </summary>
        public static int CountElement<T>(this ObservableCollection<T> obsColl, Predicate<T> predicate) where T : class
        {
            int result = 0;

            lock (((ICollection)obsColl).SyncRoot)
            {
                for (int i = 0; i < obsColl.Count; i++)
                {
                    T item = obsColl[i];

                    if (predicate(item))
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Извлекает все элементы, удовлетворяющие условиям указанного предиката.
        /// </summary>
        public static ObservableCollection<T> FindAllMain<T>(this ObservableCollection<T> obsColl, Predicate<T> predicate) where T : class
        {
            ObservableCollection<T> result = new ObservableCollection<T>();

            lock (((ICollection)obsColl).SyncRoot)
            {
                for (int i = 0; i < obsColl.Count; i++)
                {
                    T item = obsColl[i];

                    if (predicate(item))
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }
    }
}