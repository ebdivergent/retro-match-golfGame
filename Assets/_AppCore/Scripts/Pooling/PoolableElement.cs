using System;
using UnityEngine;

namespace AppCore.Pooling
{
    public interface IPoolable
    {
        bool Return();
        bool Remove();

        void _OnActivated();
        void _OnDeactivated();
    }

    public sealed class PoolableElement : MonoBehaviour, IPoolable
    {
        public IPool Pool { get; set; }

        public event Action OnActivatedEvent;
        public event Action OnDeactivatedEvent;

        public void _OnActivated()
        {
            OnActivatedEvent?.Invoke();
        }

        public void _OnDeactivated()
        {
            OnDeactivatedEvent?.Invoke();
        }

        public bool Remove()
        {
            return Pool.Remove(gameObject);
        }

        public bool Return()
        {
            return Pool.Return(gameObject);
        }
    }
}
