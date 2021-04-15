using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppCore
{
    public class MonoHelpers : MonoBehaviour
    {
        public static MonoHelpers Instance { get; private set; }

        public event Action OnUpdate;
        public event Action OnLateUpdate;
        public event Action OnFixedUpdate;
        public event Action OnDrawGizmosEvent;

        private void Awake()
        {
            if (Instance != null)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return base.StartCoroutine(enumerator);
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            base.StopCoroutine(coroutine);
        }

        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        private void OnDrawGizmos()
        {
            OnDrawGizmosEvent?.Invoke();
        }
    }
}