using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnyAssetImporter
{
    public interface IEditorPanel
    {
        bool IsInited { get; set; }

        void OnInit();
        
        void OnGUI();

        void OnDestroy();
    }

}

