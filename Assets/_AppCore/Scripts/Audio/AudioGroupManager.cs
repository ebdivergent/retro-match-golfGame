using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AppCore
{
    public interface IAudioGroupManager
    {
        IAudioGroup DefaultGroup { get; }
        IAudioGroup MusicGroup { get; }
        IAudioGroup GetGroup(string id);
        IAudioStream GetStream(string id);
        IAudioStream GetStreamByClip(AudioClip clip);
        IAudioStream GetStreamByClip(string clipName);
        IAudioStream[] GetStreams(string id);
        IAudioStream[] GetStreamsByClip(AudioClip clip);
        IAudioStream[] GetStreamsByClip(string clipName);
    }

    public class AudioGroupManager : MonoBehaviour, IAudioGroupManager
    {
        public static class Constants
        {
            public static readonly string MusicGroupID = "Music";
            public static readonly string DefaultGroupID = "Default";
        }

        #region Private members
        [SerializeField] List<AudioGroup> _groups = new List<AudioGroup>();
        #endregion

        #region Properties
        public IAudioGroup MusicGroup { get; private set; }
        public IAudioGroup DefaultGroup { get; private set; }
        public static IAudioGroupManager Instance { get; private set; }
        #endregion

        #region Unity interface
        private void Awake()
        {
            if (Instance != null)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Init();
        }
        #endregion

        #region Private methods
        private void Init()
        {
            MusicGroup = GetGroup(Constants.MusicGroupID);
            DefaultGroup = GetGroup(Constants.DefaultGroupID);

            //TimeManager.Instance.OnPauseEvent += OnPause;
        }
        #endregion

        #region Event handlers
        //private void OnPause(bool pause)
        //{
        //    DefaultGroup.SetPause(pause);
        //}
        #endregion

        #region Group management
        public IAudioGroup GetGroup(string id)
        {
            if (id == null)
                return null;

            var group = _groups.FirstOrDefault(gr => gr.ID == id);

            if (group == null)
            {
                group = new AudioGroup(id);
                _groups.Add(group);
            }
            
            return group;
        }

        public IAudioStream GetStream(string id)
        {
            if (id == null)
                return null;

            foreach (var group in _groups)
            {
                var stream = group.GetStream(id);

                if (stream != null)
                {
                    return stream;
                }
            }

            return null;
        }

        public IAudioStream GetStreamByClip(AudioClip clip)
        {
            if (clip == null)
                return null;

            foreach (var group in _groups)
            {
                var stream = group.GetStreamByClip(clip);

                if (stream != null)
                {
                    return stream;
                }
            }

            return null;
        }

        public IAudioStream GetStreamByClip(string clipName)
        {
            if (clipName == null)
                return null;

            foreach (var group in _groups)
            {
                var stream = group.GetStreamByClip(clipName);

                if (stream != null)
                {
                    return stream;
                }
            }

            return null;
        }

        public IAudioStream[] GetStreams(string id)
        {
            var streamList = new List<IAudioStream>();

            if (id == null)
                return streamList.ToArray();

            foreach (var group in _groups)
            {
                var streams = group.GetStreams(id);

                if (streams != null)
                    streamList.AddRange(streams);
            }

            return streamList.ToArray();
        }

        public IAudioStream[] GetStreamsByClip(AudioClip clip)
        {
            var streamList = new List<IAudioStream>();

            if (clip == null)
                return streamList.ToArray();

            foreach (var group in _groups)
            {
                var streams = group.GetStreamsByClip(clip);

                if (streams != null)
                    streamList.AddRange(streams);
            }

            return streamList.ToArray();
        }

        public IAudioStream[] GetStreamsByClip(string clipName)
        {
            var streamList = new List<IAudioStream>();

            if (clipName == null)
                return streamList.ToArray();

            foreach (var group in _groups)
            {
                var streams = group.GetStreamsByClip(clipName);

                if (streams != null)
                    streamList.AddRange(streams);
            }

            return streamList.ToArray();
        }
        #endregion
    }
}