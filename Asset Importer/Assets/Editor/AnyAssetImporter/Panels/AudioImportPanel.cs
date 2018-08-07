using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnyAssetImporter
{
    public class AudioImportPanel : AImporterPanel<AudioAssetRule>
    {

        private AudioStyles mStyles;
        private AudioStyles styles
        {
            get
            {
                if(mStyles == null)
                    mStyles = new AudioStyles();
                return mStyles;
            }
        }


        public override void OnInit()
        {
            base.OnInit();

            string[] folders = ImportPreferences.ImporterSettings.AudioFolders;
            List<FolderElement> folderEles = ImportPreferences.GetAllFolders(folders);
            this.initFolderTree(folderEles);
        }

        public override void OnRuleGUI(AudioAssetRule rule)
        {
            base.OnRuleGUI(rule);

            rule.LoadBackground = EditorGUILayout.Toggle(styles.LoadBackground, rule.LoadBackground);

            GUILayout.Space(5);
            rule.LoadType = (AudioClipLoadType) EditorGUILayout.EnumPopup(styles.LoadType, rule.LoadType);

            int index = Math.Max(Array.FindIndex(styles.CompressionEnumOpts, opt => opt.Equals(rule.CompressionFormat)) , 0);
            index = EditorGUILayout.Popup(styles.CompressionFormat, index, styles.CompressionFormatOpts);
            rule.CompressionFormat = styles.CompressionEnumOpts[index];

            rule.SampleRate =(AudioSampleRateSetting) EditorGUILayout.EnumPopup(styles.SampleRateSetting, rule.SampleRate);
        }

    }

}

