using UnityEditor;
using UnityEngine;

namespace AnyAssetImporter
{
    [System.Serializable]
    public class TextureAssetRule : ABaseRule
    {
        public TextureImporterType TextureType;
        public bool allowsAlphaSplitting;
        public bool IsReadable;
        public bool IsMipmap;
        public TextureImporterNPOTScale NpotScale;

        public TextureWrapMode WrapMode;
        public FilterMode FilterMode;
        public int AnisoLevel;

        public int MaxTextureSize;
        public TextureImporterCompression Compression;
        public TextureImporterFormat AndroidCompressFormat;
        public TextureImporterFormat iOSCompressFormat;
        public TextureCompressionQuality CompressionQuality;

        private bool dirty;

        public override void ApplyDefaults()
        {
            this.RuleKey = "New Texture Rule";
            this.Suffix = ".png";

            TextureType = TextureImporterType.Default;

            IsReadable = false;
            IsMipmap = false;
            allowsAlphaSplitting = false;
            NpotScale = TextureImporterNPOTScale.None;

            WrapMode = TextureWrapMode.Clamp;
            FilterMode = FilterMode.Bilinear;

            Compression = TextureImporterCompression.Compressed;
            AndroidCompressFormat = TextureImporterFormat.ETC2_RGBA8;
            iOSCompressFormat = TextureImporterFormat.PVRTC_RGBA4;
            CompressionQuality = TextureCompressionQuality.Best;
        }

        public bool IsMatch(UnityEditor.AssetImporter importer)
        {
            if (importer is TextureImporter)
            {
                return true;
            }
            return false;
        }


        public override void ApplySettings(UnityEditor.AssetImporter assetImporter)
        {
            TextureImporter importer = assetImporter as TextureImporter;
            if (importer != null)
                ApplyAssetSettings((TextureImporter)assetImporter);
        }

        private void ApplyAssetSettings(TextureImporter assetImporter)
        {
            dirty = true;
            assetImporter.textureType = TextureType;
            assetImporter.allowAlphaSplitting = allowsAlphaSplitting;
            assetImporter.isReadable = IsReadable;
            assetImporter.mipmapEnabled = IsMipmap;
            assetImporter.npotScale = NpotScale;

            assetImporter.wrapMode = WrapMode;
            assetImporter.filterMode = FilterMode;
            assetImporter.anisoLevel = AnisoLevel;

            assetImporter.maxTextureSize = MaxTextureSize;
            assetImporter.textureCompression = Compression;

            setAndroidTextureSetting(assetImporter , AndroidCompressFormat);
            setiPhoneTextureSetting(assetImporter , iOSCompressFormat);
        }


        private void setAndroidTextureSetting(TextureImporter importer, TextureImporterFormat format)
        {
            TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("Android");
            settings.maxTextureSize = MaxTextureSize;
            settings.overridden = true;
            settings.allowsAlphaSplitting = allowsAlphaSplitting;
            settings.format = format;
            settings.compressionQuality = (int)CompressionQuality;
            importer.SetPlatformTextureSettings(settings);
            
        }

        private void setiPhoneTextureSetting(TextureImporter importer, TextureImporterFormat format)
        {
            TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("iPhone");
            settings.maxTextureSize = MaxTextureSize;
            settings.allowsAlphaSplitting = allowsAlphaSplitting;
            settings.overridden = true;
            settings.format = format;
            settings.compressionQuality = (int)CompressionQuality;
            importer.SetPlatformTextureSettings(settings);
        }
    }
}