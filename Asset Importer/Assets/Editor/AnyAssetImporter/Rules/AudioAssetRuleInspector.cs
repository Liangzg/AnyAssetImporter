using UnityEditor;

namespace AnyAssetImporter
{
    [CustomEditor(typeof(AudioAssetRule))]
    public class AudioAssetRuleInspector : Editor
    {
        private static bool changed;
        private AudioAssetRule orig;

        private AudioImportPanel importPanel;

        void OnEnable()
        {
            changed = false;
            orig = (AudioAssetRule)target;

            importPanel = new AudioImportPanel();
            importPanel.CurrentRule = orig;
            Undo.RecordObject(target, "assetruleundo");
        }

        public override void OnInspectorGUI()
        {
            AudioAssetRule t = (AudioAssetRule)target;

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