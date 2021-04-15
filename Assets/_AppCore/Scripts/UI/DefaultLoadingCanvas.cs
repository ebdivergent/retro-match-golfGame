using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AppCore
{
    public class DefaultLoadingCanvas : LoadingCanvas
    {
        [SerializeField] Image _loadingBar;

        protected override void OnSceneLoadStarted(SceneName sceneName)
        {
            _loadingBar.fillAmount = 0f;
        }

        protected override void OnSceneLoadPercentChanged(float percent)
        {
            _loadingBar.fillAmount = percent;
        }

        protected override void OnSceneLoadEnded(LoadSceneResult loadSceneResult, SceneName sceneName)
        {
            _loadingBar.fillAmount = 1f;
        }
    }
}