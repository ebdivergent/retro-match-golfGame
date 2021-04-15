using UnityEngine;

namespace AppCore
{
    public abstract class SDKInitializer : MonoBehaviour, ILoadableService
    {
        [SerializeField] protected int order;

        public int Order { get { return order; } }

        public abstract void StartLoad(MonoBehaviour caller);

        public abstract bool IsLoaded();
    };

    public interface ILoadableService
    {
        void StartLoad(MonoBehaviour monoBehaviour);
        bool IsLoaded();
    }
}