using System.IO;
using AnyAssetImporter;
using Boo.Lang;
using UnityEditor;
using UnityEngine;

namespace AnyAssetImporter
{
    [CustomEditor(typeof(TextureAssetRule))]
    public class TextureAssetRuleInspector : Editor
    {
        private static bool changed;
        private TextureAssetRule orig;

        private TextureImportPanel importPanel;
        
        void OnEnable()
        {
            changed = false;
            orig = (TextureAssetRule)target;

            importPanel = new TextureImportPanel();
            importPanel.CurrentRule = orig;
            Undo.RecordObject(target, "assetruleundo");
        }

        public override void OnInspectorGUI()
        {
            TextureAssetRule t = (TextureAssetRule) target;

            EditorGUI.BeginChangeCheck();
            
            importPanel.OnRuleGUI(t);

            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
            }

            if (changed)
            {
//                if (GUILayout.Button("Apply"))
//                    importPanel.Apply(t);
                EditorUtility.SetDirty(t);
            }
        }

    }

}


