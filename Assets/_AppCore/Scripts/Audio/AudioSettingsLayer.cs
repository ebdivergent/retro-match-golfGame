using System;
using DG.Tweening;
using UnityEngine;

namespace AppCore
{
    public interface IAudioSettingsLayer
    {
        float Volume { get; }
        bool Mute { get; }
        bool Pause { get; }

        event Action<float> OnVolumeChanged;
        event Action<bool> OnMute;
        event Action<bool> OnPause;
    }

    [Serializable]
    public class AudioSettingsLayer : IAudioSettingsLayer
    {
        #region Private members
        [SerializeField] bool _mute = false;
        [SerializeField] float _volume = 1f;
        [SerializeField] bool _pause = false;

        private Tweener _volumeTweener;
        #endregion

        #region Properties
        public float Volume 
        {
            get { return _volume; } 
            set 
            { 
                StopTween(); 
                _volume = value;
                OnVolumeChanged?.Invoke(_volume); 
            } 
        }
        public bool Mute { get { return _mute; } set { if (_mute == value) return; _mute = value; OnMute?.Invoke(_mute); } }
        public bool Pause { get { return _pause; } set { if (_pause == value) return; _pause = value; OnPause?.Invoke(_pause); } }
        #endregion

        #region Events
        public event Action<float> OnVolumeChanged;
        public event Action<bool> OnMute;
        public event Action<bool> OnPause;
        #endregion

        #region Public methods
        public void Reset()
        {
            StopTween();

            Volume = 1f;
            Mute = false;
            Pause = false;
        }

        public DG.Tweening.Tweener TweenVolume(float endValue, float duration)
        {
            StopTween();

            return DOVirtual.Float(Volume, endValue, duration, OnVolumeTweenUpdate);
        }

        public void StopTween()
        {
            _volumeTweener.Kill();
        }
        #endregion

        #region Event handlers
        private void OnVolumeTweenUpdate(float volume)
        {
            _volume = volume; 

            OnVolumeChanged?.Invoke(_volume);
        }
        #endregion
    }
}