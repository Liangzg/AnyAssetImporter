using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AnyAssetImporter
{

    
    public sealed class ImportPreferences
    {

        public enum FolderTitle
        {
            Name, Option
        }

        public enum EPanel
        {
            Texture, Model, Audio, Setting
        }

        public const string TextureTitle = "Texture";
        public const string ModelTitle = "Model";
        public const string AudioTitle = "Audio";
        public const string SettingTitle = "Setting";

        private const string defaultRulePath = "Assets/Scripts/Editor/AnyAssetImporter/RuleConfigs";

        public const string NewRule = "New";
        public const string AddRule = "Add";

        public const string BtnRemove = "Remove";
        public const string BtnApply = "Apply";
        public const string BtnCreate = "Create";

        public const string settingPath = defaultRulePath + "/Settings.asset";
        private static AnyImporterSettingRule importerSetting;


        private static string getPrefsKey(string key)
        {
            return typeof (ImportPreferences).FullName + key;
        }

        public static string RuleFolderPath
        {
            get { return EditorPrefs.GetString(getPrefsKey("RulePath"), defaultRulePath); }

            set { EditorPrefs.SetString(getPrefsKey("RulePath"), value);}
        }


        public static string[] AssetRootFolders
        {
            get
            {
                string folders = EditorPrefs.GetString(getPrefsKey("rootFolders"), "Assets");
                return folders.Split('|');
            }
            set
            {
                string folders = String.Join("|", value);
                EditorPrefs.SetString(getPrefsKey("rootFolders") , folders);
            }
        }


        public static AnyImporterSettingRule ImporterSettings
        {
            get
            {
                if (importerSetting == null && File.Exists(settingPath))
                    importerSetting = AssetDatabase.LoadAssetAtPath<AnyImporterSettingRule>(settingPath);
                return importerSetting;
            }
        }

        /// <summary>
        /// Get all folders in asset root path
        /// </summary>
        /// <returns></returns>
        public static List<FolderElement> GetAllFolders(string[] rootPaths)
        {
            List<FolderElement> folders = new List<FolderElement>();
            // build the directory tree 
            folders.Add(new FolderElement("Root", "", -1, 0));

            if (rootPaths != null)
            {
                int depth = 0;
                for (int i = 0; i < rootPaths.Length; i++)
                {
                    string fullPath = null;
                    string[] paths = rootPaths[i].Split('/');
                    if (paths.Length > 2)
                    {
                        for (int j = 1; j < paths.Length; j++)
                        {
                            fullPath = string.Join("/", paths, 0, j + 1);
                            fullPath = fullPath.Replace("Assets", Application.dataPath);
                            string folderName = new DirectoryInfo(fullPath).Name;
                            FolderElement fe = new FolderElement(folderName, fullPath, j - 1, folders.Count);
                            folders.Add(fe);
                        }
                        depth = paths.Length - 1;
                    }
                    else
                    {
                        fullPath = rootPaths[i].Replace("Assets", Application.dataPath);
                        string folderName = new DirectoryInfo(fullPath).Name;
                        FolderElement fe = new FolderElement(folderName, fullPath, 0, folders.Count);
                        folders.Add(fe);
                        depth = 1;
                    }
                    
                    searchChildDirectory(folders, fullPath, depth);
                }                
            }
            return folders;
        }


        private static void searchChildDirectory(List<FolderElement> elements , string dir , int depth)
        {
            string[] dirs = Directory.GetDirectories(dir , "*" , SearchOption.TopDirectoryOnly);

            if (dir.Length <= 0) return;
            
            for (int i = 0; i < dirs.Length; i++)
            {
                string rootName = new DirectoryInfo(dirs[i]).Name;
                FolderElement childFolder = new FolderElement(rootName, dirs[i].Replace("\\" , "/"), depth, elements.Count);
                elements.Add(childFolder);

                searchChildDirectory(elements , dirs[i] , depth + 1);
            }
        }
    }
}