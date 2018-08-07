using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AnyAssetImporter
{
    public abstract class AImporterPanel<T> : IEditorPanel where T : ABaseRule
    {
        protected GUIStyle titleLabStyle = new GUIStyle();

        private Vector2 ruleScrollPos;
        private Vector2 dirScrollPos;

        protected List<string> ruleTabs = new List<string>();
        protected string curRuleTab;

        protected Dictionary<string, T> keyRules = new Dictionary<string, T>();

        protected T curRule;

        protected FolderTreeView folderTree;
        protected TreeViewState treeviewState;
        private int defaultRuleIndex;
        protected FolderElement curSelectFolder;

        private bool isChange;
        public bool IsInited { get; set; }

        public virtual T CurrentRule
        {
            get { return curRule; }
            set { curRule = value; }
        }

        public AImporterPanel()
        {
            curRuleTab = ImportPreferences.AddRule;
        }

        public void SetCurrentRule(T rule)
        {
            curRule = rule;
            curRuleTab = rule.RuleKey;
            ruleScrollPos = Vector2.zero;
            defaultRuleIndex = 0;
        }

        public virtual void OnInit()
        {
            IsInited = true;

            titleLabStyle.alignment = TextAnchor.MiddleCenter;
            titleLabStyle.fontSize = 25;
            titleLabStyle.fontStyle = FontStyle.Bold;
            titleLabStyle.richText = true;
        }


        protected void initFolderTree(List<FolderElement> folders)
        {
            treeviewState = new TreeViewState();

            AssetImporterWindow window = AssetImporterWindow.Instance;
            Rect windowRect = window.position;
            TreeModel<FolderElement> treeModel = new TreeModel<FolderElement>(folders);

            Rect mulHeaderRect = multiColumnTreeViewRect(ref windowRect);
            MultiColumnHeaderState mulHeader = CreateDefaultMultiColumnHeaderState(mulHeaderRect.width);
            MultiColumnHeader multiColumnHeader = new MultiColumnHeader(mulHeader);
            folderTree = new FolderTreeView(treeviewState, multiColumnHeader, treeModel);
            folderTree.clickItem += OnClickFolderTreeItem;
            folderTree.ReimportFolderEvent += reimportFolder;

//            folderTree.SetExpanded(1, true);
            OnClickFolderTreeItem(treeModel.Find(1));
        }

        private void OnClickFolderTreeItem(FolderElement folderElement)
        {
            if (folderElement == null) return;

            curSelectFolder = folderElement;
            ruleTabs.Clear();

            List<T> rules = AssetImporterWindow.Instance.GetRules<T>(folderElement.RelativePath);
            if (rules == null || rules.Count <= 0)
            {
                curRuleTab = ImportPreferences.AddRule;
                defaultRuleIndex = 0;
                curRule = null;
                return;
            }

            
            keyRules.Clear();
            for (int i = 0; i < rules.Count; i++)
            {
                ruleTabs.Add(rules[i].RuleKey);
                keyRules[rules[i].RuleKey] = rules[i];
            }

            curRuleTab = ruleTabs[0];
            curRule = keyRules[curRuleTab];
        }


        public MultiColumnHeaderState CreateDefaultMultiColumnHeaderState(float treeViewWidth)
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name"),
                    headerTextAlignment = TextAlignment.Left,
                    width = 0, // adjusted below
                    minWidth = 60,
                    autoResize = true,
                    allowToggleVisibility = false,
                    canSort = false

                },
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Option"),
                    headerTextAlignment = TextAlignment.Left,
                    width = 150, // adjusted below
                    maxWidth = 150,
                    autoResize = false,
                    allowToggleVisibility = false,
                    canSort = false
                },
            };


            // Set name column width (flexible)
            int nameColumn = (int) ImportPreferences.FolderTitle.Name;
            columns[nameColumn].width = treeViewWidth - GUI.skin.verticalScrollbar.fixedWidth;
            for (int i = 0; i < columns.Length; ++i)
                if (i != nameColumn)
                    columns[nameColumn].width -= columns[i].width;

            if (columns[nameColumn].width < 60f)
                columns[nameColumn].width = 60f;

            var state = new MultiColumnHeaderState(columns);
            return state;
        }


        private Rect multiColumnTreeViewRect(ref Rect windowRect)
        {
            return new Rect(10, 45, windowRect.width*0.5f - 15, windowRect.height - 45);
        }

        /// <summary>
        /// reimport all asset in this folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="rules"></param>
        protected void reimportFolder(FolderElement folder, ABaseRule[] rules)
        {
            Dictionary<ABaseRule, List<string>> fileDic = new Dictionary<ABaseRule, List<string>>();
            searchFiles(folder.FullPath, rules, fileDic);

            string dataPath = Application.dataPath;

            float totalCount = 0;
            foreach (List<string> files in fileDic.Values)
                totalCount += files.Count;

            int index = 0;
            foreach (ABaseRule rule in fileDic.Keys)
            {
                List<string> filePaths = fileDic[rule];
                for (int i = 0; i < filePaths.Count; i++)
                {
                    string relativePath = filePaths[i].Replace(dataPath, "Assets");
                    relativePath = relativePath.Replace("\\", "/");

                    rule.ApplySettings(relativePath);

                    EditorUtility.DisplayProgressBar("Tip", "Reimporter...", index / totalCount);
                    index++;
                }
            }
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Search all asset if match rule
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="filePaths"></param>
        protected void searchFiles(string folder, ABaseRule[] rules, Dictionary<ABaseRule, List<string>> filePaths)
        {
            for (int i = 0; i < rules.Length; i++)
            {
                string[] subFilePaths = Directory.GetFiles(folder, "*" + rules[i].Suffix, SearchOption.TopDirectoryOnly);

                List<string> paths = null;
                if (!filePaths.TryGetValue(rules[i], out paths))
                {
                    paths = new List<string>();
                    filePaths[rules[i]] = paths;
                }
                paths.AddRange(subFilePaths);
            }

            string[] childFolders = Directory.GetDirectories(folder);
            AssetImporterWindow aiw = AssetImporterWindow.Instance;
            string dataPath = Application.dataPath;
            for (int i = 0; i < childFolders.Length; i++)
            {
                string relativePath = childFolders[i].Replace(dataPath, "Assets");
                if (aiw.HasRules<T>(relativePath))
                {
                    // skip this folder if it have new rules
                    continue;
                }  
                searchFiles(childFolders[i] , rules , filePaths);
            }
        } 

        public virtual void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            Rect windowRect = AssetImporterWindow.Instance.position;
            EditorGUILayout.BeginVertical(GUILayout.Width(windowRect.width*0.5f), GUILayout.ExpandHeight(true));
            GUILayoutHelper.DrawHeader(typeof(T).Name);
            //            dirScrollPos = GUILayout.BeginScrollView(dirScrollPos);
            //            this.OnFolderGUI();
            //            GUILayout.EndScrollView();
            this.folderTree.OnGUI(multiColumnTreeViewRect(ref windowRect));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            onParamsGUI();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        public virtual void OnFolderGUI()
        {
            if (curRule.Folders == null || curRule.Folders.Length <= 0) return;

            EditorGUILayout.LabelField("Folders");

            EditorGUI.indentLevel++;
            for (int i = 0; i < curRule.Folders.Length; i++)
            {
                EditorGUILayout.TextField(curRule.Folders[i]);
            }
            EditorGUI.indentLevel--;

        }

        public virtual void OnRuleGUI(T rule)
        {
            rule.RuleKey = EditorGUILayout.TextField("Rule Key", rule.RuleKey);
            rule.Suffix = EditorGUILayout.TextField("File Suffix", rule.Suffix);

//            this.OnFolderGUI();

            GUILayout.Space(10);
        }


        private void onParamsGUI()
        {
            GUILayout.Space(3);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (ruleTabs.Count > 0)
            {
                for (int i = 0; i < ruleTabs.Count; i++)
                {
                    if (GUILayout.Toggle(curRuleTab.Equals(ruleTabs[i]), ruleTabs[i], EditorStyles.toolbarButton,
                        GUILayout.Width(100)))
                    {
                        if (!ruleTabs[i].Equals(curRuleTab))
                        {
                            this.SetCurrentRule(keyRules[ruleTabs[i]]);
                        }
                    }
                }
                
            }
            
            if (GUILayout.Toggle(curRuleTab.Equals(ImportPreferences.AddRule), ImportPreferences.AddRule, EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                if (!curRuleTab.Equals(ImportPreferences.AddRule))
                {
                    defaultRuleIndex = 0;
                    curRuleTab = ImportPreferences.AddRule;
                    ruleScrollPos = Vector2.zero;
                }
            }
            
            GUILayout.Toolbar(0, new[] { "" }, EditorStyles.toolbar, GUILayout.ExpandWidth(true));

            if (GUILayout.Toggle(curRuleTab.Equals(ImportPreferences.NewRule), ImportPreferences.NewRule, EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                if (!curRuleTab.Equals(ImportPreferences.NewRule))
                {
                    curRuleTab = ImportPreferences.NewRule;
                    curRule = ScriptableObject.CreateInstance<T>();
                    curRule.ApplyDefaults();
                    curRule.RuleKey = curSelectFolder.name;
                    ruleScrollPos = Vector2.zero;
                }
            }
            
            EditorGUILayout.EndHorizontal();
        
            EditorGUILayout.Space();

            GUILayout.Space(-5);

            ruleScrollPos = GUILayout.BeginScrollView(ruleScrollPos, GUILayout.ExpandWidth(true));
            if (curRuleTab.Equals(ImportPreferences.AddRule))
            {
                this.onAddRuleGUI();
            }else if (curRuleTab.Equals(ImportPreferences.NewRule))
            {
                this.onNewRuleGUI();
            }
            else
            {
                this.onRuleInfoGUI();
            }
            GUILayout.EndScrollView();
        }

        private void onAddRuleGUI()
        {
            AssetImporterWindow aiw = AssetImporterWindow.Instance;
            string[] rules = aiw.GetRuleKeys<T>();

            defaultRuleIndex = EditorGUILayout.Popup("Rules" , defaultRuleIndex, rules);
            
            if (defaultRuleIndex != 0)
            {
                curRule = aiw.GetRule<T>(rules[defaultRuleIndex]);
                GUILayoutHelper.DrawSeparator();

                EditorGUILayout.BeginVertical("Box");
                this.OnRuleGUI(curRule);
                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();
                GUILayoutHelper.DrawSeparator();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(ImportPreferences.BtnApply, GUILayout.MaxWidth(80)))
                {
                    ruleTabs.Add(curRule.RuleKey);
                    curRuleTab = curRule.RuleKey;
                    keyRules[curRuleTab] = curRule;

                    string relativePath = this.curSelectFolder.RelativePath;
                    string[] sfr = new string[curRule.Folders.Length + 1];
                    Array.Copy(curRule.Folders , sfr , curRule.Folders.Length);
                    sfr[sfr.Length - 1] = relativePath;
                    curRule.Folders = sfr;

                    AssetImporterWindow.Instance.AddFolderRule(relativePath , curRule);

                    ApplyFolder(curRule , relativePath);

                    EditorUtility.SetDirty(curRule);
                }
                EditorGUILayout.EndHorizontal();
            }
        }


        private void onNewRuleGUI()
        {
            EditorGUILayout.BeginVertical("Box");
            this.OnRuleGUI(curRule);
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(ImportPreferences.BtnCreate, GUILayout.MaxWidth(80)))
            {
                if (curSelectFolder == null)
                {
                    EditorUtility.DisplayDialog("Warning", "please select folder!", "OK");
                    return;
                }
                
                string fullPath = Path.Combine(ImportPreferences.RuleFolderPath, curRule.RuleKey + ".asset");
                if (File.Exists(fullPath))
                {
                    if(!EditorUtility.DisplayDialog("Tip", "file is exist , do you replace!", "OK" , "cancel"))
                        return;
                }

                string relativePath = this.curSelectFolder.RelativePath;

                curRule.Folders = new[]{relativePath};
                AssetDatabase.CreateAsset(curRule , fullPath);
                curRuleTab = curRule.RuleKey;
                ruleTabs.Add(curRuleTab);
                keyRules[curRuleTab] = curRule;

                AssetImporterWindow.Instance.AddFolderRule(relativePath, curRule);

                ApplyFolder(curRule , relativePath);
            }
            EditorGUILayout.EndHorizontal();
        }


        private void onRuleInfoGUI()
        {
            
            EditorGUILayout.BeginVertical("Box");
            EditorGUI.BeginChangeCheck();
            this.OnRuleGUI(curRule);
            if (EditorGUI.EndChangeCheck())
                isChange = true;
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(ImportPreferences.BtnRemove, GUILayout.MaxWidth(80)))
            {
                string[] sfr = new string[curRule.Folders.Length - 1];
                string fileRelativePath = this.curSelectFolder.RelativePath;
                int index = 0;
                for (int i = 0; i < curRule.Folders.Length; i++)
                {
                    if(curRule.Folders[i].Equals(fileRelativePath))  continue;

                    sfr[index] = curRule.Folders[i];
                    index ++;
                }
                curRule.Folders = sfr;

                ruleTabs.Remove(curRule.RuleKey);
                if (ruleTabs.Count > 0)
                    SetCurrentRule(keyRules[ruleTabs[0]]);
                else
                {
                    defaultRuleIndex = 0;
                    curRuleTab = ImportPreferences.AddRule;
                    ruleScrollPos = Vector2.zero;
                }

                AssetImporterWindow.Instance.RemoveFolderRule(fileRelativePath , curRule);

                EditorUtility.SetDirty(curRule);
            }
            GUILayout.FlexibleSpace();

            if (isChange)
            {
                if (GUILayout.Button(ImportPreferences.BtnApply, GUILayout.MaxWidth(80)))
                {
                    Apply(curRule);
                    EditorUtility.SetDirty(curRule);
                }                
            }
            EditorGUILayout.EndHorizontal();
        }



        public void Apply(T rule)
        {
            if (rule.Folders == null || rule.Folders.Length <= 0) return;

            List<string> filePaths = new List<string>();
            for (int i = 0; i < rule.Folders.Length; i++)
            {
                searchFiles(rule.Folders[i] , rule.Suffix , filePaths);
            }

            if (filePaths.Count <= 0) return;
            
            float totalCount = filePaths.Count;
            string dataPath = Application.dataPath;

            for (int i = 0; i < filePaths.Count; i++)
            {
                string relativePath = filePaths[i].Replace(dataPath, "Assets");
                relativePath = relativePath.Replace("\\", "/");

                rule.ApplySettings(relativePath);

                EditorUtility.DisplayProgressBar("Hold On" , "Reimporter..." , i / totalCount);
            }
            EditorUtility.ClearProgressBar();
        }


        public void ApplyFolder(T rule , string folder)
        {
            List<string> filePaths = new List<string>();
            searchFiles(folder, rule.Suffix, filePaths);
            
            if (filePaths.Count <= 0) return;

            float totalCount = filePaths.Count;
            string dataPath = Application.dataPath;

            for (int i = 0; i < filePaths.Count; i++)
            {
                string relativePath = filePaths[i].Replace(dataPath, "Assets");
                relativePath = relativePath.Replace("\\", "/");

                rule.ApplySettings(relativePath);

                EditorUtility.DisplayProgressBar("Hold On", "Reimporter...", i / totalCount);
            }
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Search all asset if match rule
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="paths"></param>
        protected void searchFiles(string folder, string suffix , List<string> paths)
        {
            AssetImporterWindow aiw = AssetImporterWindow.Instance;
            string dataPath = Application.dataPath;
            
            string[] subFilePaths = Directory.GetFiles(folder, "*" + suffix, SearchOption.TopDirectoryOnly);
            paths.AddRange(subFilePaths);

            string[] childFolders = Directory.GetDirectories(folder);
                
            for (int j = 0; j < childFolders.Length; j++)
            {
                string relativePath = childFolders[j].Replace(dataPath, "Assets");
                if (aiw.HasRules<T>(relativePath))
                {
                    // skip this folder if it have new rules
                    continue;
                }
                searchFiles(childFolders[j] , suffix, paths);
            }
        }

        public virtual void OnDestroy()
        {
        }
    }
}