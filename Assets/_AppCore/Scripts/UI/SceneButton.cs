using UnityEngine;
using UnityEngine.EventSystems;

namespace AppCore
{
    public class SceneButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] SceneName _sceneName;

        public void OnPointerClick(PointerEventData eventData)
        {
            SceneManager.Instance.ChangeScene(_sceneName);
        }
    }
}