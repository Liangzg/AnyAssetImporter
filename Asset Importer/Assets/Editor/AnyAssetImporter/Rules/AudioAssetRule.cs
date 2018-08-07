using UnityEditor;
using UnityEngine;

namespace AnyAssetImporter
{
    public class AudioAssetRule : ABaseRule
    {
        public bool LoadBackground;

        public AudioClipLoadType LoadType;

        public AudioCompressionFormat CompressionFormat;

        public AudioSampleRateSetting SampleRate;


        public override void ApplyDefaults()
        {
            base.ApplyDefaults();

            this.Suffix = ".ogg";
            
            this.LoadType = AudioClipLoadType.Streaming;
            this.CompressionFormat = AudioCompressionFormat.Vorbis;
            this.SampleRate = AudioSampleRateSetting.PreserveSampleRate;
        }

        public override void ApplySettings(UnityEditor.AssetImporter assetImporter)
        {
            AudioImporter importer = assetImporter as AudioImporter;

            if (importer == null) return;

            importer.loadInBackground = LoadBackground;

            AudioImporterSampleSettings sampleSetting = importer.defaultSampleSettings;
            sampleSetting.loadType = LoadType;
            sampleSetting.compressionFormat = CompressionFormat;
            sampleSetting.sampleRateSetting = SampleRate;

//            importer.GetOverrideSampleSettings("Android");

//            importer.SaveAndReimport();
        }
    }
}