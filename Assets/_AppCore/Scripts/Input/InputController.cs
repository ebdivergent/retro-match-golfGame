using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MouseDataClass = AppCore.MouseData;

namespace AppCore
{
    public interface IMouseData
    {
        bool IsTouching { get; }
        float TouchTimeScaled { get; }
        float TouchTimeUnscaled { get; }
        float SummaryDragDistance { get; }
        Vector2 SummaryDragDelta { get; }
        Vector2 FrameDragDelta { get; }
        Vector2 Position { get; }
        Vector2? PrevMousePos { get; }

        float TouchDurationScaled { get; }
        float TouchDurationUnscaled { get; }

        Vector3 GetWorldDelta(Camera camera = null, Plane? plane = null);
        Vector3? GetWorldPosition(Camera camera = null, Plane? plane = null);

        bool GetRaycastHit(out RaycastHit raycastHit, Camera camera = null);
        bool GetRaycastHits(out RaycastHit previous, out RaycastHit current, Camera camera = null);
    }

    [System.Serializable]
    public struct MouseData : IMouseData
    {
        [SerializeField] bool _isTouching;
        [SerializeField] float _touchTimeScaled;
        [SerializeField] float _touchTimeUnscaled;
        [SerializeField] Vector2? _previousFramePosition;
        [SerializeField] float _summaryDragDistance;
        [SerializeField] Vector2 _summaryDragDelta;
        [SerializeField] Vector2 _frameDragDelta;

        public bool IsTouching { get { return _isTouching; } }
        public float TouchTimeScaled { get { return _touchTimeScaled; } }
        public float TouchTimeUnscaled { get { return _touchTimeUnscaled; } }
        public float SummaryDragDistance { get { return _summaryDragDistance; } }

        public Vector2? PrevMousePos { get { return _previousFramePosition; } }

        public Vector2 FrameDragDelta { get { return _frameDragDelta; } }
        public Vector2 SummaryDragDelta { get { return _summaryDragDelta; } }

        public Vector2 Position { get { return Input.mousePosition; } }

        public float TouchDurationScaled
        {
            get { return Time.time - _touchTimeScaled; }
        }

        public float TouchDurationUnscaled
        {
            get { return Time.unscaledTime - _touchTimeUnscaled; }
        }

        public Vector3 GetWorldDelta(Camera camera = null, Plane? plane = null)
        {
            if (PrevMousePos == null)
                return Vector2.zero;

            camera = camera ?? CameraHelpers.LastActivated.link;

            if (camera == null)
            {
                Log.Error("Camera not found");
                return Vector2.zero;
            }

            plane = plane ?? new Plane(Vector3.back, Vector3.zero);

            var prevRay = camera.ScreenPointToRay(PrevMousePos.Value);
            var posRay = camera.ScreenPointToRay(Position);

            float distance;

            if (plane.Value.Raycast(prevRay, out distance))
            {
                Vector3 prevPos = prevRay.origin + prevRay.direction * distance;

                if (plane.Value.Raycast(posRay, out distance))
                {
                    return posRay.origin + posRay.direction * distance - prevPos;
                }
            }

            return Vector2.zero;
        }

        public Vector3? GetWorldPosition(Camera camera = null, Plane? plane = null)
        {
            camera = camera ?? CameraHelpers.LastActivated.link;

            if (camera == null)
            {
                Log.Error("Camera not found");
                return Vector2.zero;
            }

            plane = plane ?? new Plane(Vector3.back, Vector3.zero);

            var posRay = camera.ScreenPointToRay(Position);

            float distance;
            if (plane.Value.Raycast(posRay, out distance))
            {
                return posRay.origin + posRay.direction * distance;
            }

            return null;
        }

        public bool GetRaycastHit(out RaycastHit raycastHit, Camera camera = null)
        {
            camera = camera ?? CameraHelpers.LastActivated.link;
            raycastHit = default;

            if (camera == null)
            {
                Log.Error("Camera not found");
                return false;
            }

            var posRay = camera.ScreenPointToRay(Position);

            return Physics.Raycast(posRay, out raycastHit);
        }

        public bool GetRaycastHits(out RaycastHit previous, out RaycastHit current, Camera camera = null)
        {
            previous = default;
            current = default;

            camera = camera ?? CameraHelpers.LastActivated.link;

            if (camera == null)
            {
                Log.Error("Camera not found");
                return false;
            }

            if (!PrevMousePos.HasValue)
            {
                return false;
            }

            var prevPos = camera.ScreenPointToRay(PrevMousePos.Value);
            var currentPos = camera.ScreenPointToRay(Position);

            if (Physics.Raycast(prevPos, out previous) && Physics.Raycast(currentPos, out current))
            {
                return true;
            }

            return false;
        }

        public static MouseData GetDefaultFromUpdate()
        {
            return new MouseData()
            {
                _touchTimeScaled = Time.time,
                _touchTimeUnscaled = Time.unscaledTime,
                _summaryDragDistance = 0,
                _previousFramePosition = null,
                _summaryDragDelta = Vector2.zero,
                _isTouching = false,
                _frameDragDelta = Vector2.zero,
            };
        }

        public static MouseData GetDefault(float touchTime, float touchTimeUnscaled)
        {
            return new MouseData()
            {
                _touchTimeScaled = touchTime,
                _touchTimeUnscaled = touchTimeUnscaled,
                _summaryDragDistance = 0,
                _previousFramePosition = null,
                _summaryDragDelta = Vector2.zero,
                _isTouching = false,
                _frameDragDelta = Vector2.zero,
            };
        }

        public MouseData(MouseData mouseData, Vector2 mousePos)
        {
            _isTouching = true;

            Vector2 frameDelta;
            if (mouseData._previousFramePosition.HasValue)
                frameDelta = mousePos - mouseData._previousFramePosition.Value;
            else
                frameDelta = Vector2.zero;

            _frameDragDelta = frameDelta;
            _summaryDragDelta = mouseData._summaryDragDelta + frameDelta;
            _summaryDragDistance = mouseData._summaryDragDistance + frameDelta.magnitude;

            _touchTimeScaled = mouseData._touchTimeScaled;
            _touchTimeUnscaled = mouseData._touchTimeUnscaled;

            _previousFramePosition = mousePos;
        }
    }

    public interface IInputController
    {
        IMouseData MouseData { get; }

        event Action<IMouseData> OnMouseTouchedEvent;
        event Action<IMouseData> OnMouseHoldEvent;
        event Action<IMouseData> OnMouseReleasedEvent;
    }

    public class InputController : MonoBehaviour, IInputController
    {
        private MouseDataClass _mouseData;

        public IMouseData MouseData { get { return _mouseData; } }
        public static IInputController Instance { get; private set; }

        public event Action<IMouseData> OnMouseTouchedEvent;
        public event Action<IMouseData> OnMouseHoldEvent;
        public event Action<IMouseData> OnMouseReleasedEvent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Input.multiTouchEnabled = false;
            _mouseData = MouseDataClass.GetDefaultFromUpdate();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _mouseData = MouseDataClass.GetDefaultFromUpdate();
                OnMouseTouchedEvent?.Invoke(_mouseData);
            }
            if (Input.GetMouseButton(0))
            {
                OnMouseHoldEvent?.Invoke(_mouseData);
                _mouseData = new MouseDataClass(_mouseData, Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                OnMouseReleasedEvent?.Invoke(_mouseData);
                _mouseData = MouseDataClass.GetDefaultFromUpdate();
            }
        }
    }
}