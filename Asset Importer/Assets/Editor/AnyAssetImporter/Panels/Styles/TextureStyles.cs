using UnityEditor;
using UnityEngine;

namespace AnyAssetImporter
{
    public class TextureStyles
    {
        public GUIContent TextureType = new GUIContent("Texture Type");

        public GUIContent[] TextureTypeOpts = new GUIContent[]
        {
            new GUIContent("Default"), 
            new GUIContent("Lightmap"),
            new GUIContent("NormalMap"),
            new GUIContent("Sprite"),
            new GUIContent("SingleChannel"),   
        };

        public TextureImporterType[] TextureTypeEnumOpts = new TextureImporterType[]
        {
            TextureImporterType.Default,
            TextureImporterType.Lightmap,
            TextureImporterType.NormalMap,
            TextureImporterType.Sprite,
            TextureImporterType.SingleChannel
        };

        public GUIContent Advanced = new GUIContent("Advanced");

        public GUIContent NonPowerof2 = new GUIContent("Non Power of 2");
        

        public GUIContent ReadWriteEnable = new GUIContent("Read/Write Enabled", "Enable to be able to access ");

        public GUIContent GenMipMaps = new GUIContent("Generate Mip Maps");

        public GUIContent AlphaSource = new GUIContent("Alpha Source");

        public GUIContent WrapMode = new GUIContent("Wrap Mode");

        public GUIContent FilterMode = new GUIContent("Filter Mode");

        public GUIContent AnisoLevel = new GUIContent("Aniso Level");

        public GUIContent MaxSize = new GUIContent("Max Size");

        public GUIContent[] MaxSizeOpts = new GUIContent[]
        {
            new GUIContent("128"),new GUIContent("256"),
            new GUIContent("512"),new GUIContent("1024"),
            new GUIContent("2048"),new GUIContent("4096"),
        };

        public GUIContent Compress = new GUIContent("Compression");

        public GUIContent CompressFormat = new GUIContent("Compression Format");

        public GUIContent CompressionQuality = new GUIContent("Compression Quality");

        public TextureStyles()
        {
                    
    }

    }
}