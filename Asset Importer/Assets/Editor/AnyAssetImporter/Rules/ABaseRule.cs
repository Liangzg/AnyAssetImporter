using System.IO;
using UnityEngine;

namespace AnyAssetImporter
{
    [System.Serializable]
    public class ABaseRule : ScriptableObject
    {
        public string RuleKey;
        
        public string Suffix;

        public string[] Folders;

        public virtual void ApplyDefaults() { }

        public void ApplySettings(string assetPath)
        {
            ApplySettings(UnityEditor.AssetImporter.GetAtPath(assetPath));
        }

        public virtual void ApplySettings(UnityEditor.AssetImporter importer)
        {
            
        }
    }
}