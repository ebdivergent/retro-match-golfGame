using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

namespace AppCore
{
    public class AudioClipButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] AudioClip _audioClip;

        public void OnPointerClick(PointerEventData eventData)
        {
            AudioGroupManager.Instance.GetStreamsByClip(_audioClip).ToList().ForEach(stream => { stream.Stop(); });
            AudioStreamManager.Instance.GetStreamBuilder().SetClip(_audioClip).Play();
        }
    }
}