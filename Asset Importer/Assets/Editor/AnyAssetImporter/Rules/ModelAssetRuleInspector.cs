using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace AnyAssetImporter
{
    [CustomEditor(typeof(ModelAssetRule))]
    public class ModelAssetRuleInspector : Editor 
    {

        private static bool changed = false;

        //private bool m_SecondaryUVAdvancedOptions = false;


        private ModelAssetRule orig;

        private ModelImportPanel importPanel;

        void OnEnable()
        {
            changed = false;
            orig = (ModelAssetRule)target;

            importPanel = new ModelImportPanel();
            importPanel.CurrentRule = orig;

            Undo.RecordObject(target, "assetruleundo");
        }

        public override void OnInspectorGUI()
        {
            ModelAssetRule t = (ModelAssetRule) target;

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


