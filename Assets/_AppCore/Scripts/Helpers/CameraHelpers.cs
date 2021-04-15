using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AppCore
{
    public class CameraInfo
    {
        public string name;
        public Camera link;
        public string tag;
    }

    [RequireComponent(typeof(Camera))]
    public class CameraHelpers : MonoBehaviour
    {
        #region Static
        private static List<CameraInfo> s_cameraInfos;
        private static List<CameraInfo> s_activeCameras;

        private static SortedDictionary<string, CameraInfo> s_tagMap;
        private static SortedDictionary<string, CameraInfo> s_nameMap;

        private static CameraInfo _lastAdded;
        private static CameraInfo _lastActivated;
        static CameraHelpers()
        {
            s_cameraInfos = new List<CameraInfo>();
            s_activeCameras = new List<CameraInfo>();

            s_tagMap = new SortedDictionary<string, CameraInfo>(System.StringComparer.Ordinal);
            s_nameMap = new SortedDictionary<string, CameraInfo>(System.StringComparer.Ordinal);
        }
        #endregion

        #region Private members
        private CameraInfo _cameraInfo;
        private Camera _camera;
        #endregion

        #region Unity interface
        private void Awake()
        {
            _camera = GetComponent<Camera>();

            _cameraInfo = new CameraInfo()
            {
                name = name,
                link = _camera,
                tag = gameObject.tag,
            };

            s_cameraInfos.Add(_cameraInfo);

            _lastAdded = _cameraInfo;
        }

        private void OnEnable()
        {
            s_activeCameras.Add(_cameraInfo);

            _lastActivated = _cameraInfo;
        }

        private void OnDisable() => s_activeCameras.Remove(_cameraInfo);

        private void OnDestroy() => s_cameraInfos.Remove(_cameraInfo);
        #endregion

        #region Public
        public static CameraInfo LastActivated => _lastActivated;
        public static CameraInfo LastAdded => _lastAdded;

        public static CameraInfo GetActiveByName(string name) => s_activeCameras.FirstOrDefault(x => x.name == name);
        public static CameraInfo GetActiveByTag(string tag) => s_activeCameras.FirstOrDefault(x => x.tag == tag);

        public static CameraInfo GetByName(string name)
        {
            CameraInfo cameraInfo;
            if (s_nameMap.TryGetValue(name, out cameraInfo))
            {
                return cameraInfo;
            }

            return null;
        }

        public static CameraInfo GetByTag(string tag)
        {
            CameraInfo cameraInfo;
            if (s_tagMap.TryGetValue(tag, out cameraInfo))
            {
                return cameraInfo;
            }

            return null;
        }
        #endregion
    }
}