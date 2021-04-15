using System;
using UnityEngine;

namespace AppCore
{
    [Serializable]
    public class TrackedValue<T>
    {
        [SerializeField] T _value;

        public event Action<T> OnValueChanged;

        public T Value { get { return _value; } set { _value = value; OnValueChanged?.Invoke(_value); } }
    }

    [Serializable] public class TrackedBool : TrackedValue<bool> { }
    [Serializable] public class TrackedInt : TrackedValue<int> { }
    [Serializable] public class TrackedFloat : TrackedValue<float> { }
    [Serializable] public class TrackedString : TrackedValue<string> { }
    [Serializable] public class TrackedDouble : TrackedValue<double> { }

    [Serializable] public class TrackedDateTime : TrackedValue<DateTime> { }

    [Serializable] public class TrackedVector2 : TrackedValue<Vector2> { }
    [Serializable] public class TrackedVector3 : TrackedValue<Vector3> { }
    [Serializable] public class TrackedVector4 : TrackedValue<Vector4> { }

    [Serializable] public class TrackedColor : TrackedValue<Color> { }

}