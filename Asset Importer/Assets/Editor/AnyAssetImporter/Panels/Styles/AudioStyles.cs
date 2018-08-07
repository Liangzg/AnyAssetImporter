using UnityEngine;

namespace AnyAssetImporter
{
    public class AudioStyles
    {
        public GUIContent LoadBackground = new GUIContent("Load In Background");
        
        public GUIContent LoadType = new GUIContent("Load Type");
        
        public GUIContent CompressionFormat = new GUIContent("Compression Format");

        public GUIContent[] CompressionFormatOpts = new GUIContent[]
        {
            new GUIContent("PCM"),
            new GUIContent("Vorbis"),
            new GUIContent("ADPCM"),   
        };

        public AudioCompressionFormat[] CompressionEnumOpts = new[]
        {
            AudioCompressionFormat.PCM,
            AudioCompressionFormat.Vorbis,
            AudioCompressionFormat.ADPCM,
        };


        public GUIContent SampleRateSetting = new GUIContent("Sample Rate Setting");
    }
}