using System;
using UnityEngine;

namespace AppCore
{
    public static class GizmosHelper
    {
        public static void ActionInRequiredColor(Color color, Action action)
        {
            var gizmoColor = Gizmos.color;
            Gizmos.color = color;
            action();
            Gizmos.color = gizmoColor;
        }

        public static void DrawSphere(Vector3 position, float radius, Color color)
        {
            ActionInRequiredColor(color, () =>
            {
                Gizmos.DrawSphere(position, radius);
            });
        }

        public static void DrawCube(Vector3 position, Vector3 size, Color color)
        {
            ActionInRequiredColor(color, () =>
            {
                Gizmos.DrawCube(position, size);
            });
        }

        public static void DrawCube(Vector3 position, Vector3 size, Color color, Quaternion rotation)
        {
            ActionInRequiredColor(color, () =>
            {
                var m = Gizmos.matrix;
                Matrix4x4 newM = Matrix4x4.TRS(position, rotation, Vector3.one);
                Gizmos.matrix = newM;
                Gizmos.DrawCube(Vector3.zero, size);
                Gizmos.matrix = m;
            });
        }

        public static void DrawCube(this Transform transform, Color color)
        {
            ActionInRequiredColor(color, () =>
            {
                var m = Gizmos.matrix;
                Matrix4x4 newM = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                Gizmos.matrix = newM;
                Gizmos.DrawCube(Vector3.zero, transform.lossyScale);
                Gizmos.matrix = m;
            });
        }

        public static void DrawCube(this Transform transform, Color color, Vector3 localOffset)
        {
            ActionInRequiredColor(color, () =>
            {
                var m = Gizmos.matrix;
                Matrix4x4 newM = Matrix4x4.TRS(transform.position + transform.TransformPoint(localOffset), transform.rotation, Vector3.one);
                Gizmos.matrix = newM;
                Gizmos.DrawCube(Vector3.zero, transform.lossyScale);
                Gizmos.matrix = m;
            });
        }

        public static void DrawCube(this Transform transform, Color color, Vector3 localOffset, Vector3 scaleMultiplier)
        {
            ActionInRequiredColor(color, () =>
            {
                var m = Gizmos.matrix;
                Matrix4x4 newM = Matrix4x4.TRS(transform.position + transform.TransformPoint(localOffset), transform.rotation, Vector3.one);
                Gizmos.matrix = newM;
                Gizmos.DrawCube(Vector3.zero, Vector3.Scale(transform.lossyScale, scaleMultiplier));
                Gizmos.matrix = m;
            });
        }

        public static void DrawCube(this Transform transform, Vector3 scaleMultiplier, Color color)
        {
            ActionInRequiredColor(color, () =>
            {
                var m = Gizmos.matrix;
                Matrix4x4 newM = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                Gizmos.matrix = newM;
                Gizmos.DrawCube(Vector3.zero, Vector3.Scale(transform.lossyScale, scaleMultiplier));
                Gizmos.matrix = m;
            });
        }
    }
}