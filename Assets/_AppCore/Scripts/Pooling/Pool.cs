using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AppCore.Pooling
{
    public interface IPool
    {
        GameObject Get();
        T Get<T>() where T : Component;

        bool Return(GameObject gameObject);
        void ReturnAll();

        bool Remove(GameObject gameObject);
        void RemoveAll();
    }

    public class Pool : MonoBehaviour, IPool
    {
        #region Private members
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _initialCount = 0;

        private List<GameObject> _allItemsList = new List<GameObject>();
        private List<GameObject> _activeItemsList = new List<GameObject>();
        private List<GameObject> _inactiveItemsList = new List<GameObject>();

        public GameObject Prefab { get { return _prefab; } }
        #endregion

        #region static methods
        private static void CallOn<T>(GameObject go, Action<T> action)
        {
            var requiredTypeInstance = go.GetComponent<T>();
            if (requiredTypeInstance != null)
            {
                action.Invoke(requiredTypeInstance);
            }
        }
        #endregion

        #region Private methods

        private void Awake()
        {
            InitializeItems();
        }

        private GameObject GetPrefabInstance(bool activeAfterAdding)
        {
            var go = Instantiate(_prefab.gameObject, transform);

            go.name = $"Pooled_{_prefab.name}_{_allItemsList.Count}";
            var poolableElement = go.AddComponent<PoolableElement>();
            poolableElement.Pool = this;

            _allItemsList.Add(go);

            (activeAfterAdding ? _activeItemsList : _inactiveItemsList).Add(go);

            if (go.gameObject.activeSelf != activeAfterAdding)
                go.SetActive(activeAfterAdding);

            return go;
        }

        private void InitializeItems()
        {
            int diff = _initialCount - _allItemsList.Count;

            diff = Mathf.Max(diff, 0);

            for (int i = 0; i < diff; i++)
            {
                GetPrefabInstance(false);
            }
        }
        #endregion 

        #region Public methods
        public static Pool Create(GameObject prefab, int initialCount = 4, string goName = "Pool")
        {
            var go = new GameObject(goName);
            var pool = go.AddComponent<Pool>();

            pool._prefab = prefab;
            pool._initialCount = initialCount;
            pool.InitializeItems();

            return pool;
        }

        public GameObject Get()
        {
            GameObject go = null;

            var lastInactive = _inactiveItemsList.LastOrDefault();

            if (lastInactive)
            {
                _inactiveItemsList.RemoveAt(_inactiveItemsList.Count - 1);
                _activeItemsList.Add(lastInactive);

                go = lastInactive;

                go.SetActive(true);
            }
            else
            {
                go = GetPrefabInstance(true);
            }

            CallOn<IPoolable>(go, poolable => poolable._OnActivated());

            return go;
        }

        public T Get<T>() where T : Component
        {
            return Get().GetComponent<T>();
        }

        public bool Return(GameObject go)
        {
            if (_activeItemsList.Remove(go))
            {
                _inactiveItemsList.Add(go);

                go.SetActive(false);

                CallOn<IPoolable>(go, poolable => poolable._OnDeactivated());

                return true;
            }

            return false;
        }

        public void ReturnAll()
        {
            var activeList = _activeItemsList.ToArray();

            foreach (var item in activeList)
            {
                Return(item);
            }
        }

        public bool Remove(GameObject go)
        {
            if (_allItemsList.Remove(go))
            {
                // Deactivate object before removal.
                Return(go);

                _inactiveItemsList.Remove(go);
            }
            else
                return false;

            Destroy(go);

            return true;
        }

        public void RemoveAll()
        {
            var allItemsList = _allItemsList.ToArray();

            foreach (var item in allItemsList)
            {
                Remove(item);
            }
        }
        #endregion
    }
}
