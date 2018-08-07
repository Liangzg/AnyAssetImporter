using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AnyAssetImporter
{
    public class AssetImporterWindow : EditorWindow {

        private const string MENU = "Tools/Asset Importer";
        private static AssetImporterWindow asw = null;
         
        private IEditorPanel curPanel;
        private string version = "0.0.1";

        private ImportPreferences.EPanel panelState = ImportPreferences.EPanel.Texture;
        private ImportPreferences.EPanel lastPanelState = ImportPreferences.EPanel.Setting;

        private TextureImportPanel texturePanel;
        private ModelImportPanel modelPanel;
        private AudioImportPanel audioPanel;
        private SettingPanel settingPanel;
        
        private Dictionary<string, List<ABaseRule>> folderRules;
        private Dictionary<string, ABaseRule> keyRules;

        [MenuItem(MENU , false)]
        private static void showWindow()
        {
            AssetImporterWindow window = EditorWindow.GetWindow<AssetImporterWindow>("Asset Importer");
            window.Show();
        }


        public static AssetImporterWindow Instance
        {
            get { return asw; }
        }

        private void OnEnable()
        {
            asw = this;
            this.initAllRules();

            panelState = ImportPreferences.EPanel.Texture;
        }


        private void initAllRules()
        {
            texturePanel = new TextureImportPanel();
            modelPanel = new ModelImportPanel();
            audioPanel = new AudioImportPanel();
            settingPanel = new SettingPanel();

            keyRules = new Dictionary<string, ABaseRule>();
            folderRules = new Dictionary<string, List<ABaseRule>>();

            string ruleFolders = ImportPreferences.RuleFolderPath;
            string[] files = Directory.GetFiles(ruleFolders, "*.asset", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string relativePath = files[i].Replace(Application.dataPath, "Assets").Replace("\\", "/");
                ABaseRule rule = AssetDatabase.LoadAssetAtPath<ABaseRule>(relativePath);

                if (rule == null)    continue;

                for (int j = 0; j < rule.Folders.Length; j++)
                {
                    List<ABaseRule> frs = null;
                    if (!folderRules.TryGetValue(rule.Folders[j], out frs))
                    {
                        frs = new List<ABaseRule>();
                        folderRules[rule.Folders[j]] = frs;
                    }
                    frs.Add(rule);
                }

                keyRules[rule.RuleKey] = rule;
            }
        }


        public void AddFolderRule(string folderPath, ABaseRule rule)
        {
            List<ABaseRule> frs = null;
            if (!folderRules.TryGetValue(folderPath, out frs))
            {
                frs = new List<ABaseRule>();
                folderRules[folderPath] = frs;
            }
            frs.Add(rule);

            if (!keyRules.ContainsKey(rule.RuleKey))
                keyRules[rule.RuleKey] = rule;
        }


        public void RemoveFolderRule(string folderPath, ABaseRule rule)
        {
            List<ABaseRule> frs = null;
            if (!folderRules.TryGetValue(folderPath, out frs))
            {
                return;
            }
            frs.Remove(rule);

            if (frs.Count <= 0) folderRules.Remove(folderPath);
        }

        /// <summary>
        /// Find rule list by folder relative path 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public List<T> GetRules<T>(string folderPath) where T : ABaseRule
        {
            List<ABaseRule> frs = null;
            if (!folderRules.TryGetValue(folderPath, out frs))
            {
                return null;
            }

            List<T> rules = new List<T>();
            for (int i = 0; i < frs.Count; i++)
            {
                if(frs[i] is T)
                    rules.Add((T)frs[i]);    
            }
            return rules;
        }


        public ABaseRule[] GetCurrentRules(string folderPath)
        {
            if (panelState == ImportPreferences.EPanel.Texture)
            {
                List<TextureAssetRule> tars = GetRules<TextureAssetRule>(folderPath);
                return tars != null ? tars.ToArray() : null;
            }
            if (panelState == ImportPreferences.EPanel.Model)
            {
                List<ModelAssetRule> mars = GetRules<ModelAssetRule>(folderPath);
                return mars != null ? mars.ToArray() : null;
            }
            if (panelState == ImportPreferences.EPanel.Audio)
            {
                List<AudioAssetRule> aar = GetRules<AudioAssetRule>(folderPath);
                return aar != null ? aar.ToArray() : null;
            }
            return null;
        } 


        public bool HasRules<T>(string folderPath) where T : ABaseRule
        {
            List<ABaseRule> frs = null;
            if (!folderRules.TryGetValue(folderPath, out frs))
            {
                return false;
            }
            
            for (int i = 0; i < frs.Count; i++)
            {
                if (frs[i] is T)
                    return true;
            }
            return false;
        }

        public T GetRule<T>(string ruleKey) where T : ABaseRule
        {
            ABaseRule rule = null;
            if (!keyRules.TryGetValue(ruleKey, out rule))
                return null;
            return (T)rule;
        }

        /// <summary>
        /// Find all rule list by rule type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string[] GetRuleKeys<T>() where T : ABaseRule
        {
            List<string> rules = new List<string>();
            rules.Add("None");
            foreach (ABaseRule rule in keyRules.Values)
            {
                if(rule is T)   rules.Add(rule.RuleKey);
            }
            return rules.ToArray();
        }


        public bool HasRuleKey(string key)
        {
            return keyRules.ContainsKey(key);
        }

        private void OnGUI()
        {
            onTopBar();
            curPanel.OnGUI();
            onBottomGUI();
        }

        public void onTopBar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            string[] enumNames = Enum.GetNames(typeof(ImportPreferences.EPanel));
            ImportPreferences.EPanel[] enumValues = (ImportPreferences.EPanel[])Enum.GetValues(typeof(ImportPreferences.EPanel));
            for (int i = 0; i < enumNames.Length - 1; i++)
            {
                if (GUILayout.Toggle(panelState == enumValues[i], enumNames[i], EditorStyles.toolbarButton, GUILayout.Width(100)))
                {
                    panelState = enumValues[i];
                    curPanel = GetCurrentPanel();
                }
            }
            GUILayout.Toolbar(0, new[] { "" }, EditorStyles.toolbar, GUILayout.ExpandWidth(true));

            if (GUILayout.Toggle(panelState == ImportPreferences.EPanel.Setting, enumNames[enumNames.Length - 1], EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                panelState = enumValues[enumNames.Length - 1];
                curPanel = GetCurrentPanel();
            }
            
            if (lastPanelState != panelState)
            {
                lastPanelState = panelState;
                curPanel.OnInit();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void onBottomGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            GUILayout.Label(version);
            EditorGUILayout.EndHorizontal();
        }


        public IEditorPanel GetCurrentPanel()
        {
            switch (panelState)
            {
                case ImportPreferences.EPanel.Texture:
                    return texturePanel;
                case ImportPreferences.EPanel.Model:
                    return modelPanel;
                case ImportPreferences.EPanel.Audio:
                    return audioPanel;
                case ImportPreferences.EPanel.Setting:
                    return settingPanel;
            }
            return null;
        }

        private void OnDestroy()
        {
            asw = null;
        }
    }

}

