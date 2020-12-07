using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace CodingSeb.Mvvm
{
    internal class WeakDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDisposable
        where TKey : class
        where TValue : class
    {
        private readonly object locker = new object();
        //private readonly HashSet<WeakReference> weakKeySet = new HashSet<WeakReference>(new ObjectReferenceEqualityComparer<WeakReference>());
        private ConditionalWeakTable<TKey, WeakKeyHolder> keyHolderMap = new ConditionalWeakTable<TKey, WeakKeyHolder>();
        private Dictionary<WeakReference, TValue> valueMap = new Dictionary<WeakReference, TValue>(new ObjectReferenceEqualityComparer<WeakReference>());

        private class WeakKeyHolder
        {
            private readonly WeakDictionary<TKey, TValue> outer;

            public WeakKeyHolder(WeakDictionary<TKey, TValue> outer, TKey key)
            {
                this.outer = outer;
                WeakRef = new WeakReference(key);
            }

            public WeakReference WeakRef { get; }

            ~WeakKeyHolder()
            {
                outer?.OnKeyDrop(WeakRef);  // Nullable operator used just in case this.outer gets set to null by GC before this finalizer runs. But I haven't had this happen.
            }
        }

        private void OnKeyDrop(WeakReference weakKeyRef)
        {
            lock (locker)
            {
                if (!bAlive)
                    return;

                //this.weakKeySet.Remove(weakKeyRef);
                valueMap.Remove(weakKeyRef);
            }
        }

        // The reason for this is in case (for some reason which I have never seen) the finalizer trigger doesn't work
        // There is not much performance penalty with this, since this is only called in cases when we would be enumerating the inner collections anyway.
        private void ManualShrink()
        {
            foreach (var key in valueMap.Keys.Where(k => !k.IsAlive).ToList())
                valueMap.Remove(key);
        }

        private Dictionary<TKey, TValue> CurrentDictionary
        {
            get
            {
                lock (locker)
                {
                    ManualShrink();
                    return valueMap.ToDictionary(p => (TKey)p.Key.Target, p => p.Value);
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var val))
                    return val;

                throw new KeyNotFoundException();
            }

            set
            {
                Set(key, value, isUpdateOkay: true);
            }
        }

        private bool Set(TKey key, TValue val, bool isUpdateOkay)
        {
            lock (locker)
            {
                if (keyHolderMap.TryGetValue(key, out var weakKeyHolder))
                {
                    if (!isUpdateOkay)
                        return false;

                    valueMap[weakKeyHolder.WeakRef] = val;
                    return true;
                }

                weakKeyHolder = new WeakKeyHolder(this, key);
                keyHolderMap.Add(key, weakKeyHolder);
                //this.weakKeySet.Add(weakKeyHolder.WeakRef);
                valueMap.Add(weakKeyHolder.WeakRef, val);

                return true;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                lock (locker)
                {
                    ManualShrink();
                    return valueMap.Keys.Select(k => (TKey)k.Target).ToList();
                }
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                lock (locker)
                {
                    ManualShrink();
                    return valueMap.Select(p => p.Value).ToList();
                }
            }
        }

        public int Count
        {
            get
            {
                lock (locker)
                {
                    ManualShrink();
                    return valueMap.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (!Set(key, value, isUpdateOkay: false))
                throw new ArgumentException("Key already exists");
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            lock (locker)
            {
                keyHolderMap = new ConditionalWeakTable<TKey, WeakKeyHolder>();
                valueMap.Clear();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            object curVal = null;

            lock (locker)
            {
                if (!keyHolderMap.TryGetValue(item.Key, out WeakKeyHolder weakKeyHolder))
                    return false;

                curVal = weakKeyHolder.WeakRef.Target;
            }

            return curVal?.Equals(item.Value) == true;
        }

        public bool ContainsKey(TKey key)
        {
            lock (locker)
            {
                return keyHolderMap.TryGetValue(key, out var weakKeyHolder);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>)CurrentDictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return CurrentDictionary.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            lock (locker)
            {
                if (!keyHolderMap.TryGetValue(key, out var weakKeyHolder))
                    return false;

                keyHolderMap.Remove(key);
                valueMap.Remove(weakKeyHolder.WeakRef);

                return true;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (locker)
            {
                if (!keyHolderMap.TryGetValue(item.Key, out var weakKeyHolder))
                    return false;

                if (weakKeyHolder.WeakRef.Target?.Equals(item.Value) != true)
                    return false;

                keyHolderMap.Remove(item.Key);
                valueMap.Remove(weakKeyHolder.WeakRef);

                return true;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (locker)
            {
                if (!keyHolderMap.TryGetValue(key, out var weakKeyHolder))
                {
                    value = default;
                    return false;
                }

                value = valueMap[weakKeyHolder.WeakRef];
                return true;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool bAlive = true;

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool bManual)
        {
            if (bManual)
            {
                Monitor.Enter(locker);

                if (!bAlive)
                    return;
            }

            try
            {
                keyHolderMap = null;
                valueMap = null;
                bAlive = false;
            }
            finally
            {
                if (bManual)
                    Monitor.Exit(locker);
            }
        }

        ~WeakDictionary()
        {
            Dispose(false);
        }
    }

    internal class ObjectReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        public static ObjectReferenceEqualityComparer<T> Default = new ObjectReferenceEqualityComparer<T>();

        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }

    internal class ObjectReferenceEqualityComparer : ObjectReferenceEqualityComparer<object>
    {
    }
}