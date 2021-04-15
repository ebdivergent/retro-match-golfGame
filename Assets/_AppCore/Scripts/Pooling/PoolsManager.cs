using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AppCore.Pooling
{
    public interface ITypedElement
    {
        string TypeAsString { get; }
    }

    public interface IPoolsManager
    {
        string[] AvailableTypeStrings { get; }

        GameObject Get(string typeAsString);
        T Get<T>(string typeAsString) where T : Component;
        T Get<T, TType>(TType type) where T : Component;
        GameObject GetPrefab(string typeAsString);
        bool Remove(GameObject go);
        void RemoveAll();
        bool Return(GameObject go);
        void ReturnAll();
    }

    public class PoolsManager : MonoBehaviour, IPoolsManager
    {
        #region Private members
        [SerializeField] GameObject[] _gameObjectsForPooling;
        [SerializeField] int _initialCountPerType = 4;

        private Dictionary<string, Pool> _pools;
        #endregion

        #region Properties
        public string[] AvailableTypeStrings { get { return _pools.Keys.ToArray(); } }
        #endregion

        #region Unity interface
        private void Awake()
        {
            if (transform.childCount > 0)
            {
                enabled = false;
                Debug.LogError($"Remove all children from this game object: {name}");
                return;
            }

            _pools = new Dictionary<string, Pool>();

            foreach (var go in _gameObjectsForPooling)
            {
                if (!go || _pools.ContainsKey(go.name))
                {
                    Debug.LogError($"GameObject duplicate detected '{go.name}'");
                }
                else
                {
                    var pool = Pool.Create(go.gameObject, _initialCountPerType);

                    try
                    {
                        _pools.Add(go.GetComponent<ITypedElement>().TypeAsString, pool);
                    }
                    catch
                    {
                        Debug.LogError($"No type found on prefab '{go.name}'");
                    }
                }
            }
        }
        #endregion

        #region Public methods
        public GameObject GetPrefab(string typeAsString)
        {
            return _pools[typeAsString].Prefab;
        }

        public GameObject Get(string typeAsString)
        {
            return _pools[typeAsString].Get();
        }

        public T Get<T>(string typeAsString) where T : Component
        {
            return _pools[typeAsString].Get<T>();
        }

        public T Get<T, TType>(TType type) where T : Component
        {
            return _pools[type.ToString()].Get<T>();
        }

        public bool Return(GameObject go)
        {
            try
            {
                return _pools[go.GetComponent<ITypedElement>().TypeAsString].Return(go);
            }
            catch
            {
                return false;
            }
        }

        public void ReturnAll()
        {
            foreach (var pool in _pools)
            {
                pool.Value.ReturnAll();
            }
        }

        public bool Remove(GameObject go)
        {
            try
            {
                return _pools[go.GetComponent<ITypedElement>().TypeAsString].Remove(go);
            }
            catch
            {
                return false;
            }
        }

        public void RemoveAll()
        {
            foreach (var pool in _pools)
            {
                pool.Value.RemoveAll();
            }
        }
        #endregion
    }
}