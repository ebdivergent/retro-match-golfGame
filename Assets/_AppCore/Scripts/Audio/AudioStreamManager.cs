using AppCore.Pooling;
using UnityEngine;

namespace AppCore
{
    public interface IAudioStreamManager
    {
        AudioStream.Builder GetStreamBuilder();
    }

    public class AudioStreamManager : MonoBehaviour, IAudioStreamManager
    {
        private Pool _audioStreamPool;

        public static IAudioStreamManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Init();
        }

        private void Init()
        {
            var prefab = AudioStream.CreateFromSource(new GameObject().AddComponent<AudioSource>());

            prefab.name = "AudioStream_Prefab";

            prefab.Source.spatialBlend = 0f;
            prefab.Source.maxDistance = float.MaxValue;
            prefab.Source.minDistance = float.MinValue;

            _audioStreamPool = Pool.Create(prefab.gameObject, 10, "Pool_AudioStreams");

            DontDestroyOnLoad(_audioStreamPool.gameObject);
            DontDestroyOnLoad(prefab.gameObject);
        }

        public AudioStream.Builder GetStreamBuilder()
        {
            AudioStream stream = _audioStreamPool.Get<AudioStream>();

            var builder = new AudioStream.Builder(stream);

            builder.SetOnComplete(() =>
            {
                _audioStreamPool.Return(stream.gameObject);
            });

            return builder;
        }
    }
}