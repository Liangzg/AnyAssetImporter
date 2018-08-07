using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AnyAssetImporter
{
    public class SettingPanel : IEditorPanel
    {
        public bool IsInited { get; set; }

        private Vector2 scrollPos;

        private AnyImporterSettingRule settings;
        
        public void OnInit()
        {
            IsInited = true;

            if (File.Exists(ImportPreferences.settingPath))
                settings = AssetDatabase.LoadAssetAtPath<AnyImporterSettingRule>(ImportPreferences.settingPath);
            else
                settings = ScriptableObject.CreateInstance<AnyImporterSettingRule>();
        }

        public void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            GUILayoutHelper.DrawHeader("Texture Folders");
            settings.TextureFolders = drawFoldersGUI(settings.TextureFolders);

            GUILayoutHelper.DrawHeader("Model Folders");
            settings.ModelFolders = drawFoldersGUI(settings.ModelFolders);

            GUILayoutHelper.DrawHeader("Audio Folders");
            settings.AudioFolders = drawFoldersGUI(settings.AudioFolders);
            
            EditorGUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save"))
            {
                if(File.Exists(ImportPreferences.settingPath))
                    EditorUtility.SetDirty(settings);
                else
                    AssetDatabase.CreateAsset(settings , ImportPreferences.settingPath);
            }
            
        }


        private string[] drawFoldersGUI(string[] folders)
        {
            if (folders != null)
            {
                EditorGUILayout.LabelField("Folders");
                EditorGUI.indentLevel++;
                for (int i = 0; i < folders.Length; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.TextField(folders[i]);

                    if (GUILayout.Button("X", GUILayout.MaxWidth(30)))
                    {
                        List<string> list = new List<string>(folders);
                        list.RemoveAt(i);
                        folders = list.ToArray();
                        break;
                    }

                    GUILayout.Space(10);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }


            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(50);
            if (GUILayout.Button("Add New Folder"))
            {
                string folderPath = EditorUtility.OpenFolderPanel("Add", Application.dataPath, null);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    string[] newFolders = null;
                    if (folders != null)
                    {
                        newFolders = new string[folders.Length + 1];
                        Array.Copy(folders, newFolders, folders.Length);
                    }else
                        newFolders = new string[1];

                    newFolders[newFolders.Length - 1] = folderPath.Replace(Application.dataPath , "Assets");
                    folders = newFolders;
                }
            }
            GUILayout.Space(50);
            EditorGUILayout.EndHorizontal();

            return folders;
        }

        public void OnDestroy()
        {

        }
    }
}