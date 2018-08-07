using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AnyAssetImporter
{

    public class ImportPoster : AssetPostprocessor
    {
        static T FindRuleForAsset<T>(string path, string assetFilter) where T : ABaseRule
        {
//            path = Directory.GetParent(path).FullName;
//            path = path.Replace('\\', '/');
//            path = path.Remove(0, Application.dataPath.Length);
//            path = path.Insert(0, "Assets");

            string suffix = Path.GetExtension(path);

            string localRulePath = ImportPreferences.RuleFolderPath;
            T matchRule = null;
            int pathLength = 0;
            string[] ruleFiles = AssetDatabase.FindAssets(assetFilter, new[] {localRulePath});
            foreach (string findAsset in ruleFiles)
            {
                string p = AssetDatabase.GUIDToAssetPath(findAsset);
                
                T rule = AssetDatabase.LoadAssetAtPath<T>(p);

                if(rule.Folders == null)    continue;

                for (int i = 0; i < rule.Folders.Length; i++)
                {
                    string folderPath = rule.Folders[i];
                    if (string.IsNullOrEmpty(rule.Suffix) || suffix.Equals(rule.Suffix) &&
                        path.StartsWith(folderPath) && folderPath.Length > pathLength)
                    {
                        matchRule = rule;
                        pathLength = folderPath.Length;
                        break;
                    } 
                }
            }

            return matchRule;
        }



        private static void ExcuteMeshRule(UnityEditor.AssetImporter importer)
        {
            ModelAssetRule rule = FindRuleForAsset<ModelAssetRule>(importer.assetPath, "t:ModelAssetRule");
            if (rule == null)
            {
                Debug.Log("No model rules found for asset");
            }
            else
            {
                rule.ApplySettings(importer);
            }
        }

        private void OnPreprocessModel()
        {
            ExcuteMeshRule(assetImporter);
        
        }

        private static void ExcuteTextureRule(UnityEditor.AssetImporter importer)
        {
            TextureAssetRule rule = FindRuleForAsset<TextureAssetRule>(importer.assetPath, "t:TextureAssetRule");
            if (rule == null)
            {
                Debug.Log("No texture rules found for asset");
            }
            else
            {
                rule.ApplySettings(importer);
            }
        }

        private void OnPreprocessTexture()
        {
            ExcuteTextureRule(assetImporter);
        
        }

        private void OnPreprocessAnimation()
        {
           // Debug.Log("no animation rules");
        }

        public void OnPreprocessAudio()
        {
            ExcuteAudioRule(assetImporter);
        }


        private static void ExcuteAudioRule(UnityEditor.AssetImporter importer)
        {
            AudioAssetRule rule = FindRuleForAsset<AudioAssetRule>(importer.assetPath, "t:AudioAssetRule");
            if (rule == null)
            {
                Debug.Log("No audio rules found for asset");
            }
            else
            {
                rule.ApplySettings(importer);
            }
        }


        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            for (int i = 0; i < movedAssets.Length; i++)
            {
                UnityEditor.AssetImporter importer = UnityEditor.AssetImporter.GetAtPath(movedAssets[i]);

                //model:
                if(importer is ModelImporter)
                    ExcuteMeshRule(importer);

                //texture:
                else if(importer is TextureImporter)
                    ExcuteTextureRule(importer);

                //audio
                else if(importer is AudioImporter)
                    ExcuteAudioRule(importer);
            }
        }
    }

}

