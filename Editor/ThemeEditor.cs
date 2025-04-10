using UnityEditor;
using UnityEngine;

namespace ThematicUI
{
    [CustomEditor(typeof(Theme))]
    public class ThemeEditor : Editor
    {
        private Theme theme;
        private string rename;

        private GUIStyle headerStyle;
        private GUIStyle containerStyle;
        private bool stylesInitialized = false;

        private void OnEnable()
        {
            theme = (Theme)target;
            rename = target.name;
        }

        public override void OnInspectorGUI()
        {
            if (!stylesInitialized)
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 13,
                    normal = { textColor = new Color(0.8f, 0.9f, 1f) }
                };

                containerStyle = new GUIStyle(GUI.skin.box)
                {
                    margin = new RectOffset(0, 0, 10, 10),
                    padding = new RectOffset(10, 10, 5, 5)
                };

                stylesInitialized = true;
            }

            serializedObject.Update();

            DrawHeader("🎭 Theme Editor", 16);
            DrawRenameSection();

            EditorGUILayout.Space(10);
            DrawThemeKeyList();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }

        private void DrawHeader(string title, int fontSize = 14)
        {
            GUIStyle style = new(EditorStyles.boldLabel)
            {
                fontSize = fontSize,
                normal = { textColor = Color.white }
            };
            EditorGUILayout.LabelField(title, style);
            EditorGUILayout.Space(4);
        }

        private void DrawRenameSection()
        {
            EditorGUILayout.BeginVertical(containerStyle);
            EditorGUILayout.LabelField("✏️ Rename Theme", EditorStyles.miniBoldLabel);

            EditorGUILayout.BeginHorizontal();
            rename = EditorGUILayout.TextField("Theme Name", rename);

            GUI.backgroundColor = new Color(0.4f, 0.8f, 1f);
            if (GUILayout.Button("Apply", GUILayout.Width(80)))
            {
                Rename();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void Rename()
        {
            string path = AssetDatabase.GetAssetPath(theme.ThemeAsset);
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            for (int i = 0; i < assets.Length; i++)
            {
                if (AssetDatabase.IsSubAsset(assets[i]) && assets[i].name == target.name)
                {
                    assets[i].name = rename;
                    EditorUtility.SetDirty(assets[i]);
                }
            }
            EditorUtility.SetDirty(theme);
            AssetDatabase.ImportAsset(path);
        }

        private void DrawThemeKeyList()
        {
            EditorGUILayout.BeginVertical(containerStyle);
            EditorGUILayout.LabelField("🎯 Key Values", EditorStyles.miniBoldLabel);

            if (theme.Keys == null || theme.Keys.Count == 0)
            {
                EditorGUILayout.HelpBox("No theme keys defined for this theme.", MessageType.Info);
            }
            else
            {
                foreach (var key in theme.Keys)
                {
                    EditorGUILayout.Space(4);
                    key.DrawField();
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
}
