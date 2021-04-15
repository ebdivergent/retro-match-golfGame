using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AppCore
{
    public class UIElement : MonoBehaviour
    {
        [SerializeField] List<UIElement> _nested;
        private bool _shown = false;
        private Tween _delayCompleteTween;

        public virtual float AnimationDuration 
        { 
            get
            {
                if (_nested == null || _nested.Count == 0)
                    return 0f;

                return _nested.Max(x => x.AnimationDuration); 
            } 
        }
        public bool Shown { get { return _shown; } }

        public event Action<bool> OnShowEvent;
        public event Action<bool> OnAnimationCompleteEvent;

        private void Awake()
        {
            OnAwakeHandler();

            ShowAnimation(_shown = false, true);
        }

        private void OnDestroy()
        {
            OnDestroyHandler();
        }

        protected virtual void OnAwakeHandler()
        {

        }

        protected virtual void OnDestroyHandler()
        {

        }

        protected void InvokeAnimCompleteEvent(bool show)
        {
            OnAnimationCompleteEvent?.Invoke(show);
        }

        protected virtual void ShowAnimation(bool show, bool immediate = false)
        {
            gameObject.SetActive(show);
        }

        public void Show(bool show, bool immediate = false)
        {
            if (_shown == show)
                return;

            _shown = show;

            OnShowEvent?.Invoke(_shown);
            
            _delayCompleteTween.KillIfPlaying(false);

            ShowAnimation(_shown);

            foreach (var uiElement in _nested)
                if (uiElement != null)
                    uiElement.Show(show, immediate);
                else
                    Debug.LogError($"Please, remove null reference in nested list of UIElement on GameObject '{gameObject.name}'");

            _delayCompleteTween = DOVirtual.DelayedCall(AnimationDuration, () =>
            {
                InvokeAnimCompleteEvent(show);
            });
        }

        public UIElement GetNestedByName(string name)
        {
            return _nested.FirstOrDefault(x => x.name == name);
        }

        public bool ContainsNested(UIElement uIElement)
        {
            return _nested.Contains(uIElement);
        }

        public bool AddNested(UIElement uIElement)
        {
            if (!ContainsNested(uIElement))
            {
                _nested.Add(uIElement);

                return true;
            }

            return false;
        }

        public bool RemoveNested(UIElement uIElement)
        {
            return _nested.Remove(uIElement);
        }
    }
}