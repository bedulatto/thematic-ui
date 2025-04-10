using UnityEditor;
using UnityEngine;

namespace ThematicUI
{
    [CustomEditor(typeof(Theme))]
    public class ThemeEditor : Editor
    {
        private Theme theme;
        private string rename;

        private void OnEnable()
        {
            theme = (Theme)target;
            rename = target.name;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            rename = EditorGUILayout.TextField("Theme Name", rename);
            if (GUILayout.Button("Apply"))
            {
                Rename();
            }
            EditorGUILayout.EndHorizontal();

            DrawThemeKeyList();

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets(); 
            }
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
            if (theme.Keys == null || theme.Keys.Count == 0)
            {
                EditorGUILayout.HelpBox("No theme keys defined for this theme.", MessageType.Info);
                return;
            }

            foreach (var key in theme.Keys)
            {
                EditorGUILayout.Space();
                key.DrawField();
            }
        }
    }
}
