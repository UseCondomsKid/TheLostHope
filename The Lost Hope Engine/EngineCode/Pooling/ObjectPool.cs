using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheLostHopeEngine.EngineCode.Pooling
{
    public class ObjectPool<T>
    {
        private readonly Func<T> _objectCreate;
        private readonly Action<T> _objectRelease;
        private readonly Queue<T> _objects;
        private readonly int _maxSize;

        public ObjectPool(Func<T> objectCreate, Action<T> objectRelease, int maxSize)
        {
            this._objectCreate = objectCreate ?? throw new ArgumentNullException(nameof(objectCreate));
            this._objectRelease = objectRelease ?? throw new ArgumentNullException(nameof(objectRelease));
            this._maxSize = maxSize;
            this._objects = new Queue<T>(maxSize);
        }

        public int GetPoolSize() { return _objects.Count; }

        public T GetObject()
        {
            lock (_objects)
            {
                if (_objects.Count > 0)
                {
                    return _objects.Dequeue();
                }
            }

            return _objectCreate();
        }

        public void ReturnObject(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Cannot return a null object to the pool.");
            }

            _objectRelease(obj);

            lock (_objects)
            {
                if (_objects.Count < _maxSize)
                {
                    _objects.Enqueue(obj);
                }
            }
        }
    }
}
