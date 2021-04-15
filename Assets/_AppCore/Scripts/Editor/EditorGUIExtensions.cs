using UnityEditor;
using UnityEngine;

namespace AppCore
{
    public static class EditorGUIExtensions
    {
        public static bool DrawFixHelpBox(string help, string customButton = null)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.HelpBox(help, MessageType.Warning);

            bool value = GUILayout.Button(customButton ?? "Fix it");

            EditorGUILayout.EndHorizontal();

            return value;
        }

        public static void DrawHeader(string header)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
            EditorGUILayout.Space();
        }

        public static Texture2D TextureField(string name, Texture2D texture)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name);
            var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return result;
        }

        public static Sprite SpriteField(string name, Sprite sprite)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name);
            var result = (Sprite)EditorGUILayout.ObjectField(sprite, typeof(Sprite), false, GUILayout.Width(70), GUILayout.Height(70));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return result;
        }
    }
}