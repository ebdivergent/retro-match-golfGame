using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AppCore
{
    [Serializable] public class TrackedPageType : TrackedValue<PageType> { }

    public class UIController : MonoBehaviour
    {
        [SerializeField] Page[] _pageList;
        [SerializeField] TrackedPageType _currentPage;

        private Page _activePage;

        private bool _isTransition;

        public static UIController Instance { get; private set; }

        public PageType GetCurrentPageType()
        {
            return _currentPage.Value;
        }

        public Page GetActivePage()
        {
            return _activePage;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _currentPage.OnValueChanged += OnCurrentPageChanged;
        }

        private void Start()
        {
            var page = GetPage(_currentPage.Value);
            page.Show(true, true);

            _activePage = page;
            _isTransition = false;
        }

        public bool SetPage(PageType pageType)
        {
            if (_isTransition)
            {
                return false;
            }

            if (pageType == _currentPage.Value)
            {
                return false;
            }

            _currentPage.Value = pageType;

            return true;
        }

        private void OnCurrentPageChanged(PageType pageType)
        {
            var nextPage = GetPage(pageType);
            var currentPage = _activePage;

            if (nextPage == currentPage)
            {
                return;
            }
            else if (nextPage == null)
            {
                Action<bool> onShow = null;

                onShow = (val) =>
                {
                    _isTransition = false;
                    Debug.Log(onShow == null);
                    currentPage.OnShowEvent -= onShow;
                };

                currentPage.transform.SetAsLastSibling();

                currentPage.OnShowEvent += onShow;
                currentPage.Show(false);

                _activePage = null;
                return;
            }
            else if (currentPage == null)
            {
                Action<bool> onShow = null;

                onShow = (val) =>
                {
                    _isTransition = false;
                    nextPage.OnShowEvent -= onShow;
                };

                nextPage.transform.SetAsLastSibling();

                nextPage.OnShowEvent += onShow;
                nextPage.Show(true);

                _activePage = nextPage;
                return;
            }
            else
            {
                _isTransition = true;
                _activePage = nextPage;

                if (currentPage.Order < nextPage.Order)
                {
                    nextPage.transform.SetAsLastSibling();
                    currentPage.transform.SetAsLastSibling();

                    Action<bool> onShow = null;

                    onShow = (val) =>
                    {
                        _isTransition = false;
                        currentPage.OnShowEvent -= onShow;
                    };

                    currentPage.OnShowEvent += onShow;
                    currentPage.Show(false);

                    nextPage.Show(true, true);
                }
                else
                {
                    currentPage.transform.SetAsLastSibling();
                    nextPage.transform.SetAsLastSibling();

                    Action<bool> onShow = null;

                    onShow = (val) =>
                    {
                        _isTransition = false;
                        currentPage.Show(false, true);
                        nextPage.OnShowEvent -= onShow;
                    };

                    nextPage.OnShowEvent += onShow;
                    nextPage.Show(true);
                }
            }
        }

        private Page GetPage(PageType pageType)
        {
            return _pageList.FirstOrDefault(x => x.PageType == pageType);
        }
    }
}