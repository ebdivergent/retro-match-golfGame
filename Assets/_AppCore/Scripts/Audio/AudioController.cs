using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace AppCore
{
    public interface IAudioController
    {
        AudioSource CreateSource(GameObject gameObject);
        bool RemoveSource(AudioSource audioSource);
        void RemoveSourceAt(int index);
        AudioSource FindSource(string audioClipName);
        AudioSource FindSource(AudioClip clip);
        Tween SetVolumeSource(AudioSource audioSource, float volume, float delayFade);
        float PlaySource(string audioClipName, AudioSource audioSource, float delayFade, float volume, bool loop);
        float PlaySource(AudioClip audioClip, AudioSource audioSource, float delayFade, float volume, bool loop);
        void PauseSource(AudioSource audioSource, float delayFade);
        void UnPauseSource(AudioSource audioSource, float delayFade, bool loop);
        void StopSource(AudioSource audioSource, float delayFade);

        void PlayMusic(AudioClip audioClip, float delayFade = 0f);
        void PlayMusic(string audioClipName, float delayFade = 0f);
        void PlayOrUnPauseMusic(AudioClip audioClip, float delayFade = 0f);
        void PlayOrUnPauseMusic(string audioClipName, float delayFade = 0f);
        void PlayMusicIfNotPlaying(AudioClip audioClip, float delayFade = 0f);
        float PlayMusicIfNotPlaying(string audioClipName, float delayFade = 0f);
        void PauseMusic(float delayFade = 0f);
        void UnPauseMusic(float delayFade = 0f);
        void StopMusic(float delayFade = 0f);
        AudioClip GetMusicClip();
        bool IsMusicClip(AudioClip audioClip);
        bool IsMusicClip(string audioClipName);
        bool IsMusicPaused();
        bool IsMusicPaused(AudioClip audioClip);
        bool IsMusicPaused(string audioClipName);
        bool IsMusicPlaying();
        bool IsMusicPlaying(AudioClip audioClip);
        bool IsMusicPlaying(string audioClipName);
        Tween SetMusicVolume(float volume, float delayFade = 0f);
        float GetMusicVolume();
        float GetMusicSourceVolume();

        float Play(AudioClip audioClip, float delayFade = 0f, bool loop = false);
        float Play(string audioClipName, float delayFade = 0f, bool loop = false);
        void PlayOrUnPause(AudioClip audioClip, float delayFade = 0f, bool loop = false);
        void PlayOrUnPause(string audioClipName, float delayFade = 0f, bool loop = false);
        void PlayIfNotPlaying(AudioClip audioClip, float delayFade = 0f, bool loop = false);
        void PlayIfNotPlaying(string audioClipName, float delayFade = 0f, bool loop = false);
        void Pause(float delayFade = 0f);
        bool Pause(AudioClip audioClip, float delayFade = 0f);
        bool Pause(string audioClipName, float delayFade = 0f);
        void UnPause(float delayFade = 0f, bool loop = false);
        bool UnPause(AudioClip clip, float delayFade = 0f, bool loop = false);
        bool UnPause(string audioClipName, float delayFade = 0f, bool loop = false);
        void Stop(float delayFade = 0f);
        bool Stop(AudioClip clip, float delayFade = 0f);
        bool Stop(string audioClipName, float delayFade = 0f);
        bool IsSoundPlaying(AudioClip audioClip);
        bool IsSoundPlaying(string audioClipName);
        bool IsSoundPlaying();
        bool IsSoundPaused(AudioClip audioClip);
        bool IsSoundPaused(string audioClipName);
        void SetVolume(float volume, float delayFade = 0f);
        Tween SetVolume(AudioClip audioClip, float volume, float delayFade = 0f);
        Tween SetVolume(string audioClipName, float volume, float delayFade = 0f);
        float GetSourceVolume(AudioClip audioClip);
        float GetSourceVolume(string audioClipName);
    }

    [Obsolete("Use AudioStreamManager instead.")]
    public class AudioController : MonoBehaviour, IAudioController, ISceneLoadStartedListener, ISceneLoadEndedListener
    {
        #region Private members
        private List<AudioSource> _audioSourcesList = new List<AudioSource>();
        private List<AudioSource> _pausedSourcesList = new List<AudioSource>();
        private bool _isMusicPaused = false;

        private GameObject _audioContainerGO = null;
        private GameObject _musicContainerGO = null;

        private AudioSource _musicSource = null;

        private float _musicVolume = 1.0f;
        #endregion

        #region Properties
        public static IAudioController Instance { get; private set; }
        #endregion

        #region Event handlers
        public void OnSceneLoadStartedHandler(SceneName sceneName)
        {
            Stop();
            _audioSourcesList.Clear();
            _pausedSourcesList.Clear();
        }
        public void OnSceneLoadEndedHandler(LoadSceneResult loadSceneResult, SceneName sceneName)
        {
            InitializeSoundContainer();
        }
        #endregion

        #region Unity interface
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeMusicContainer();
            InitializeSoundContainer();
        }

        private void Update()
        {
            float soundVolume = DataManager.Instance.DataContainer.VolumeSound;

            _musicVolume = DataManager.Instance.DataContainer.VolumeMusic;

            bool soundEnabled = soundVolume > Mathf.Epsilon;
            bool musicEnabled = _musicVolume > Mathf.Epsilon;

            if (_musicSource != null)
            {
                if (_musicSource.mute == musicEnabled)
                    _musicSource.mute = !musicEnabled;
            }

            int count = _audioSourcesList.Count;
            for (int i = 0; i < count; i++)
            {
                var source = _audioSourcesList[i];

                // If sound was completed and isn't paused.
                if (source == null ||
                    (source.time == 0f && !source.isPlaying // Wasn't completed.
                    && !_pausedSourcesList.Contains(source))) // Isn't paused.
                {
                    RemoveSourceAt(i);
                    count--;
                    continue;
                }
                else
                {
                    source.volume = soundVolume;
                }

                // If sound is not muted when sound setting inactive.
                if (source.mute == soundEnabled)
                    source.mute = !soundEnabled;
            }
        }
        #endregion

        #region Private methods
        private void InitializeMusicContainer()
        {
            _musicContainerGO = gameObject;

            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicVolume = DataManager.Instance.DataContainer.VolumeMusic;
            _musicSource.volume = _musicVolume;

            DontDestroyOnLoad(_musicSource);
        }

        private void InitializeSoundContainer()
        {
            _audioContainerGO = new GameObject();
            _audioContainerGO.name = "AudioController_Sounds";

            DontDestroyOnLoad(_audioContainerGO);
        }

        private AudioClip GetClip(string audioClipName)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        #region Public methods
        #region Core
        public AudioSource CreateSource(GameObject gameObject)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            _audioSourcesList.Add(audioSource);
            return audioSource;
        }
        public bool RemoveSource(AudioSource audioSource)
        {
            _pausedSourcesList.Remove(audioSource);
            bool removed = _audioSourcesList.Remove(audioSource);
            DestroyObject(audioSource);
            return removed;
        }
        public void RemoveSourceAt(int index)
        {
            var audioSource = _audioSourcesList[index];
            _pausedSourcesList.Remove(audioSource);
            _audioSourcesList.RemoveAt(index);
            DestroyObject(audioSource);
        }
        public AudioSource FindSource(string audioClipName)
        {
            int sourcesCount = _audioSourcesList.Count;
            for (int i = 0; i < sourcesCount; i++)
                if (_audioSourcesList[i].clip)
                    if (_audioSourcesList[i].clip.name == audioClipName)
                        return _audioSourcesList[i];
            return null;
        }
        public AudioSource FindSource(AudioClip clip)
        {
            int sourcesCount = _audioSourcesList.Count;
            for (int i = 0; i < sourcesCount; i++)
                if (_audioSourcesList[i].clip == clip)
                    return _audioSourcesList[i];
            return null;
        }
        public Tween SetVolumeSource(AudioSource audioSource, float volume, float delayFade)
        {
            if (!audioSource)
            {
                Debug.LogError("No clip received");
                return null;
            }
            if (delayFade == 0.0f)
                audioSource.volume = volume;
            audioSource.DOKill(true);
            return audioSource.DOFade(volume, delayFade);
        }
        public float PlaySource(string audioClipName, AudioSource audioSource, float delayFade, float volume, bool loop)
        {
            return PlaySource(GetClip(audioClipName), audioSource, delayFade, volume, loop);
        }
        public float PlaySource(AudioClip audioClip, AudioSource audioSource, float delayFade, float volume, bool loop)
        {
            if (!audioClip)
            {
                Debug.LogError("No clip received");
                return 0.0f;
            }
            audioSource.loop = loop;
            audioSource.clip = audioClip;
            audioSource.Play();
            SetVolumeSource(audioSource, volume, delayFade);
            return audioClip.length;
        }
        public void PauseSource(AudioSource audioSource, float delayFade)
        {
            _pausedSourcesList.Add(audioSource);
            SetVolumeSource(audioSource, 0.0f, delayFade).OnComplete(audioSource.Pause);
        }
        public void UnPauseSource(AudioSource audioSource, float delayFade, bool loop)
        {
            _pausedSourcesList.Remove(audioSource);
            audioSource.UnPause();
            SetVolumeSource(audioSource, 1.0f, delayFade);
        }
        public void StopSource(AudioSource audioSource, float delayFade)
        {
            SetVolumeSource(audioSource, 0.0f, delayFade).OnComplete(delegate
            {
                RemoveSource(audioSource);
            });
        }

        public void PlayMusic(AudioClip audioClip, float delayFade = 0f)
        {
            _isMusicPaused = false;
            AudioSource currentMusicSource = _musicSource;
            StopMusic(delayFade);
            DOVirtual.DelayedCall(delayFade, delegate
            {
                Destroy(currentMusicSource);
            }, false);
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.volume = 0f;
            PlaySource(audioClip, _musicSource, delayFade, _musicVolume, true);
        }
        public void PlayMusic(string audioClipName, float delayFade = 0f)
        {
            _isMusicPaused = false;
            AudioSource currentMusicSource = _musicSource;
            StopMusic(delayFade);
            DOVirtual.DelayedCall(delayFade, delegate
            {
                Destroy(currentMusicSource);
            }, false);
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.volume = 0f;
            PlaySource(GetClip(audioClipName), _musicSource, delayFade, _musicVolume, true);
        }
        public void PlayOrUnPauseMusic(AudioClip audioClip, float delayFade = 0f)
        {
            if (IsMusicClip(audioClip) && _isMusicPaused)
                UnPauseMusic(delayFade);
            else
                PlayMusic(audioClip, delayFade);
        }
        public void PlayOrUnPauseMusic(string audioClipName, float delayFade = 0f)
        {
            if (IsMusicClip(audioClipName) && _isMusicPaused)
                UnPauseMusic(delayFade);
            else
                PlayMusic(audioClipName, delayFade);
        }
        public void PlayMusicIfNotPlaying(AudioClip audioClip, float delayFade = 0f)
        {
            if (!IsMusicPlaying(audioClip))
                PlayMusic(audioClip, delayFade);
        }
        public float PlayMusicIfNotPlaying(string audioClipName, float delayFade = 0f)
        {
            if (!IsMusicPlaying(audioClipName))
                PlayMusic(audioClipName, delayFade);
            return 0;
        }
        public void PauseMusic(float delayFade = 0f)
        {
            _isMusicPaused = true;
            AudioSource currentMusicSource = _musicSource;
            SetVolumeSource(currentMusicSource, 0, delayFade).OnComplete(currentMusicSource.Pause);
        }
        public void UnPauseMusic(float delayFade = 0f)
        {
            _isMusicPaused = false;
            _musicSource.UnPause();
            SetVolumeSource(_musicSource, _musicVolume, delayFade);
        }
        public void StopMusic(float delayFade = 0f)
        {
            AudioSource currentMusicSource = _musicSource;
            SetVolumeSource(currentMusicSource, 0f, delayFade).OnComplete(currentMusicSource.Stop);
        }
        public AudioClip GetMusicClip()
        {
            return _musicSource.clip;
        }
        public bool IsMusicClip(AudioClip audioClip)
        {
            return audioClip == GetMusicClip();
        }
        public bool IsMusicClip(string audioClipName)
        {
            var currentMusicClip = GetMusicClip();
            if (currentMusicClip)
                return currentMusicClip.name == audioClipName;
            return false;
        }
        public bool IsMusicPaused()
        {
            return _isMusicPaused;
        }
        public bool IsMusicPaused(AudioClip audioClip)
        {
            return IsMusicPaused() && IsMusicClip(audioClip);
        }
        public bool IsMusicPaused(string audioClipName)
        {
            return IsMusicPaused() && IsMusicClip(audioClipName);
        }
        public bool IsMusicPlaying()
        {
            return _musicSource.isPlaying;
        }
        public bool IsMusicPlaying(AudioClip audioClip)
        {
            return _musicSource.isPlaying && IsMusicClip(audioClip);
        }
        public bool IsMusicPlaying(string audioClipName)
        {
            return _musicSource.isPlaying && IsMusicClip(audioClipName);
        }
        public Tween SetMusicVolume(float volume, float delayFade = 0f)
        {
            DataManager.Instance.DataContainer.VolumeMusic = _musicVolume = volume;
            return SetVolumeSource(_musicSource, _musicVolume, delayFade);
        }
        public float GetMusicVolume()
        {
            return _musicVolume;
        }
        public float GetMusicSourceVolume()
        {
            return _musicSource.volume;
        }

        public float Play(AudioClip audioClip, float delayFade = 0f, bool loop = false)
        {
            return PlaySource(audioClip, CreateSource(_audioContainerGO), delayFade, 1f, loop);
        }
        public float Play(string audioClipName, float delayFade = 0f, bool loop = false)
        {
            return PlaySource(GetClip(audioClipName), CreateSource(_audioContainerGO), delayFade, 1f, loop);
        }
        public void PlayOrUnPause(AudioClip audioClip, float delayFade = 0f, bool loop = false)
        {
            if (IsSoundPaused(audioClip))
                UnPause(audioClip, delayFade, loop);
            else
                Play(audioClip, delayFade, loop);
        }
        public void PlayOrUnPause(string audioClipName, float delayFade = 0f, bool loop = false)
        {
            if (IsSoundPaused(audioClipName))
                UnPause(audioClipName, delayFade, loop);
            else
                Play(audioClipName, delayFade, loop);
        }
        public void PlayIfNotPlaying(AudioClip audioClip, float delayFade = 0f, bool loop = false)
        {
            if (!IsSoundPlaying(audioClip))
                Play(audioClip, delayFade, loop);
        }
        public void PlayIfNotPlaying(string audioClipName, float delayFade = 0f, bool loop = false)
        {
            if (!IsSoundPlaying(audioClipName))
                Play(audioClipName, delayFade, loop);
        }
        public void Pause(float delayFade = 0f)
        {
            int count = _audioSourcesList.Count;
            for (int i = 0; i < count; i++)
                PauseSource(_audioSourcesList[i], delayFade);
        }
        public bool Pause(AudioClip audioClip, float delayFade = 0f)
        {
            var source = FindSource(audioClip);
            if (!source)
                return false;
            PauseSource(source, delayFade);
            return true;
        }
        public bool Pause(string audioClipName, float delayFade = 0f)
        {
            var source = FindSource(audioClipName);
            if (!source)
                return false;
            PauseSource(source, delayFade);
            return true;
        }
        public void UnPause(float delayFade = 0f, bool loop = false)
        {
            int sourcesCount = _audioSourcesList.Count;
            for (int i = 0; i < sourcesCount; i++)
                UnPauseSource(_audioSourcesList[i], delayFade, loop);
        }
        public bool UnPause(AudioClip clip, float delayFade = 0f, bool loop = false)
        {
            var source = FindSource(clip);
            if (!source)
                return false;
            UnPauseSource(source, delayFade, loop);
            return true;
        }
        public bool UnPause(string audioClipName, float delayFade = 0f, bool loop = false)
        {
            var source = FindSource(audioClipName);
            if (!source)
                return false;
            UnPauseSource(source, delayFade, loop);
            return true;
        }
        public void Stop(float delayFade = 0f)
        {
            int count = _audioSourcesList.Count;
            for (int i = 0; i < count; i++)
                StopSource(_audioSourcesList[i], delayFade);
        }
        public bool Stop(AudioClip clip, float delayFade = 0f)
        {
            var source = FindSource(clip);
            if (!source)
                return false;
            StopSource(source, delayFade);
            return true;
        }
        public bool Stop(string audioClipName, float delayFade = 0f)
        {
            var source = FindSource(audioClipName);
            if (!source)
                return false;
            StopSource(source, delayFade);
            return true;
        }
        public bool IsSoundPlaying(AudioClip audioClip)
        {
            var source = FindSource(audioClip);
            if (source)
                return source.isPlaying;
            return false;
        }
        public bool IsSoundPlaying(string audioClipName)
        {
            var source = FindSource(audioClipName);
            if (source)
                return source.isPlaying;
            return false;
        }
        public bool IsSoundPlaying()
        {
            int sourcesCount = _audioSourcesList.Count;
            for (int i = 0; i < sourcesCount; i++)
                if (_audioSourcesList[i].isPlaying)
                    return true;
            return false;
        }
        public bool IsSoundPaused(AudioClip audioClip)
        {
            var source = FindSource(audioClip);
            if (source)
                return _pausedSourcesList.Contains(source);
            return false;
        }
        public bool IsSoundPaused(string audioClipName)
        {
            var source = FindSource(audioClipName);
            if (source)
                return _pausedSourcesList.Contains(source);
            return false;
        }
        public void SetVolume(float volume, float delayFade = 0f)
        {
            int sourcesCount = _audioSourcesList.Count;
            for (int i = 0; i < sourcesCount; i++)
                SetVolumeSource(_audioSourcesList[i], volume, delayFade);
        }
        public Tween SetVolume(AudioClip audioClip, float volume, float delayFade = 0f)
        {
            return SetVolumeSource(FindSource(audioClip), volume, delayFade);
        }
        public Tween SetVolume(string audioClipName, float volume, float delayFade = 0f)
        {
            return SetVolumeSource(FindSource(audioClipName), volume, delayFade);
        }
        public float GetSourceVolume(AudioClip audioClip)
        {
            var source = FindSource(audioClip);
            if (source)
                return source.volume;
            return 0f;
        }
        public float GetSourceVolume(string audioClipName)
        {
            var source = FindSource(audioClipName);
            if (source)
                return source.volume;
            return 0f;
        }
        #endregion
        #endregion
    }
}