using UnityEngine;

namespace AppCore
{
    public interface IProgressBar
    {
        IProgressBarBehaviour ProgressBarBehaviour { get; }
    }

    public class ProgressBar : MonoBehaviour, IProgressBar
    {
        public IProgressBarBehaviour ProgressBarBehaviour { get; private set; }

        private void Awake()
        {
            ProgressBarBehaviour = GetComponent<IProgressBarBehaviour>();
        }
    }
}