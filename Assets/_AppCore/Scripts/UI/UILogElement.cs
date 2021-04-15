using UnityEngine;
using UnityEngine.UI;

namespace AppCore
{
    public class UILogElement : MonoBehaviour
    {
        [SerializeField] Text _logText;
        [SerializeField] Text _timestampText;

        public void Setup(string message, Color messageColor, string timestamp, Color timestampColor)
        {
            _logText.text = message;
            _logText.color = messageColor;

            _timestampText.text = timestamp;
            _timestampText.color = timestampColor;
        }
    }
}