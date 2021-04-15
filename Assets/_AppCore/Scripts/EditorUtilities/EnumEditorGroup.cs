#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace AppCore.EditorUtilities
{
    [Serializable]
    public class EnumEditorGroup
    {
        [SerializeField] bool _draw;
        [SerializeField] EnumInfo _enumInfo;

        public EnumInfo EnumInfo { get { return _enumInfo; } }
        public bool Draw { get { return _draw; } }

        public Action<string> OnDrawEndLineGUI;

        public EnumEditorGroup(string name, bool draw = false)
        {
            _enumInfo = new EnumInfo(name);
            _draw = draw;
        }

        public void CheckForRewrite()
        {
            if (_enumInfo.HasToBeUpdated())
            {
                Debug.Log($"Incorrect enum detected for '{_enumInfo.Name}.cs'. It has to be replaced.");
                Write();
            }
        }

        public void Write()
        {
            ClassfileGenerator.CreateEnum(_enumInfo);
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(15f);
            GUILayout.BeginVertical();
            GUILayout.Space(15f);

            GUILayout.BeginHorizontal();

            var styleEnum = new GUIStyle(EditorStyles.whiteLargeLabel);
            styleEnum.normal.textColor = Color.black;

            var styleText = new GUIStyle(EditorStyles.whiteLargeLabel);
            styleText.normal.textColor = Color.gray;

            GUILayout.Label($"{_enumInfo.Name} ", styleEnum);
            GUILayout.Label(" enum values:", styleText);
            GUILayout.EndHorizontal();

            if (GUILayout.Button(_draw ? "Hide" : "Show"))
            {
                _draw = !_draw;
            }

            if (_draw)
            {
                for (int i = 0; i < _enumInfo.Elements.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("-", GUILayout.Width(25f)))
                    {
                        _enumInfo.Elements.RemoveAt(i);
                    }

                    _enumInfo.Elements[i] = GUILayout.TextField(_enumInfo.Elements[i]);

                    GUILayout.Label(i.ToString());

                    OnDrawEndLineGUI?.Invoke(_enumInfo.Elements[i]);

                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("+"))
                {
                    _enumInfo.Elements.Add("");
                }
                if (_enumInfo.Elements.Count > 0 && GUILayout.Button("-"))
                {
                    _enumInfo.Elements.RemoveAt(_enumInfo.Elements.Count - 1);
                }
                if (GUILayout.Button("Clear"))
                {
                    _enumInfo.Elements.Clear();
                }
                GUILayout.EndHorizontal();

                if (_enumInfo.Elements.Count > 0 && GUILayout.Button($"Write to {_enumInfo.Name}.cs"))
                    Write();
            }

            GUILayout.Space(15f);
            GUILayout.EndVertical();
            GUILayout.Space(15f);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}
#endif