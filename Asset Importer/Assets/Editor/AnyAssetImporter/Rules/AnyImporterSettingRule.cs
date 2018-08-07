using System;
using UnityEngine;

namespace AnyAssetImporter
{
    [Serializable]
    public class AnyImporterSettingRule : ScriptableObject
    {
        public string[] TextureFolders;

        public string[] ModelFolders;

        public string[] AudioFolders;
    }

}