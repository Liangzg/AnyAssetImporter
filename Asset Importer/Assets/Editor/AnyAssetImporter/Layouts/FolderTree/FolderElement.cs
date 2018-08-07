using System.IO;
using UnityEngine;

namespace AnyAssetImporter
{
    public class FolderElement : TreeElement
    {
        
        public string FullPath { get; private set; }
        
        public string RelativePath { get; private set; }

        public SearchOption Search;

        public FolderElement(string name, string relativePath , int depth, int id) : base(name, depth, id)
        {
            this.FullPath = relativePath;
            RelativePath = relativePath.Replace(Application.dataPath, "Assets");
        }
    }
}