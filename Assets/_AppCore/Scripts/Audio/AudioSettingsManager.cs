using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppCore
{
    public interface IAudioSettingsManager : IAudioSettingsContainer
    {
    }

    // TODO: add external volume controls if required.
    public class AudioSettingsManager : MonoBehaviour, IAudioSettingsManager
    {
        [SerializeField] AudioSettingsLayer _settings = new AudioSettingsLayer();

        public static IAudioSettingsManager Instance { get; private set; }

        public IAudioSettingsLayer Settings { get { return _settings; } }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            //Init();
        }

        //private void Init()
        //{
        //    var musicGroup = AudioGroupManager.Instance.MusicGroup;

        //    musicGroup.SetVolume(DataManager.Instance.DataContainer.MusicVolume);

        //    _settings.Volume = DataManager.Instance.DataContainer.MasterVolume;

        //    _settings.OnVolumeChanged += OnMasterVolumeChanged;
        //    musicGroup.Settings.OnVolumeChanged += OnMusicVolumeChanged;
        //}

        //private void OnMasterVolumeChanged(float volume)
        //{
        //    DataManager.Instance.DataContainer.MasterVolume = volume;
        //}

        //private void OnMusicVolumeChanged(float volume)
        //{
        //    DataManager.Instance.DataContainer.MusicVolume = volume;
        //}
    }
}