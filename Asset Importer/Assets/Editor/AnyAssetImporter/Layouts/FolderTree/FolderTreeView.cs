using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace AnyAssetImporter
{
    public class FolderTreeView : TreeViewWithTreeModel<FolderElement>
    {
        const float kRowHeights = 20f;
        const float kIconWidth = 18f;

        public event Action<FolderElement> clickItem;
        public event Action<FolderElement, ABaseRule[]> ReimportFolderEvent; 

        private AssetImporterWindow aiw;
        public FolderTreeView(TreeViewState state, TreeModel<FolderElement> model)
            : base(state, model)
        {
            this.Reload();
        }

        public FolderTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<FolderElement> model) 
            : base(state, multiColumnHeader, model)
        {
            // Custom setup
            rowHeight = kRowHeights;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            customFoldoutYOffset = (kRowHeights - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = kIconWidth;
            
            aiw = AssetImporterWindow.Instance;

            this.Reload();
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return base.BuildRows(root);
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            TreeViewItem<FolderElement> item = (TreeViewItem<FolderElement>)args.item;

            for (int i = 0 , columns =  args.GetNumVisibleColumns(); i < columns; i++)
            {
                drawItemGUI(args.GetCellRect(i) , item , args.GetColumn(i) , ref args);
            }
        }


        private void drawItemGUI(Rect cellRect, TreeViewItem<FolderElement> item, int column, ref RowGUIArgs args)
        {
            // Center cell rect vertically (makes it easier to place controls, icons etc in the cells)
            CenterRectUsingSingleLineHeight(ref cellRect);

            ImportPreferences.FolderTitle title = (ImportPreferences.FolderTitle) column;
            switch (title)
            {
                    case ImportPreferences.FolderTitle.Name:
                        Rect iconRect = cellRect;
                        iconRect.x += GetContentIndent(item);
                        iconRect.width = kIconWidth;

                        Texture2D iconTex = EditorGUIUtility.FindTexture("Folder Icon");
                        if (iconRect.xMax < cellRect.xMax)
                        {
                            GUI.DrawTexture(iconRect, iconTex);
                        }
                        // Default icon and label
                        args.rowRect = cellRect;
                        base.RowGUI(args);
                    break;
                    case ImportPreferences.FolderTitle.Option:
                        ABaseRule[] rules = aiw.GetCurrentRules(item.data.RelativePath);
                        if (rules == null || rules.Length <= 0) return;

                        Rect btnRect = cellRect;
                        btnRect.x += cellRect.width * 0.15f;
                        btnRect.width = cellRect.width *0.7f;

                        if (GUI.Button(btnRect, "Reimport"))
                        {
                            if(ReimportFolderEvent != null)
                                ReimportFolderEvent.Invoke(item.data , rules);
                        }
                    break;
            }

        }


        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);

            if (selectedIds.Count > 1) return;

            if(clickItem != null)
                clickItem.Invoke(this.treeModel.Find(selectedIds[0]));
        }
    }
}