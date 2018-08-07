using UnityEngine;
using UnityEditor;

namespace AnyAssetImporter
{
    [System.Serializable]
    public class ModelAssetRule : ABaseRule
    {
        //model
        public bool m_ImportMaterials;
        public bool m_IsReadable;
        public bool optimizeMeshForGPU;
        public bool m_ImportBlendShapes;
        public bool m_AddColliders;
        public bool keepQuads;
        public bool m_weldVertices;
        public bool swapUVChannels;
        public bool generateSecondaryUV;
        public float secondaryUVHardAngle;
        public float secondaryUVPackMargin;
        public float secondaryUVAngleDistortion;
        public float secondaryUVAreaDistortion;
        public float normalSmoothAngle;
        public ModelImporterNormals normalImportMode;
        public ModelImporterMeshCompression m_MeshCompression;
        public ModelImporterTangents tangentImportMode;
        public ModelImporterMaterialName m_MaterialName;
        public ModelImporterMaterialSearch m_MaterialSearch;
        //

        //Rig
        public ModelImporterAnimationType AnimationType;
        public bool isOptimizeObject;
        //


        //Animation
        public bool ImportAnimation;
        public ModelImporterAnimationCompression AnimCompression;

        public override void ApplyDefaults()
        {
            this.RuleKey = "New Model Rule";
            Suffix = ".FBX";

            m_MeshCompression = ModelImporterMeshCompression.Off;
            m_IsReadable = false;
            m_ImportBlendShapes = false;
            optimizeMeshForGPU = true;
            m_AddColliders = false;
            keepQuads = false;
            m_weldVertices = true;
            swapUVChannels = false;
            generateSecondaryUV = false;
            normalImportMode = ModelImporterNormals.None;
            tangentImportMode = ModelImporterTangents.CalculateMikk;
            m_ImportMaterials = true;
            m_MaterialName = ModelImporterMaterialName.BasedOnTextureName;
            m_MaterialSearch = ModelImporterMaterialSearch.Everywhere;

            AnimationType = ModelImporterAnimationType.None;

            ImportAnimation = false;
            AnimCompression = ModelImporterAnimationCompression.Off;
        }

        public override void ApplySettings(UnityEditor.AssetImporter assetImporter)
        {
            ModelImporter importer = assetImporter as ModelImporter;

            if (importer != null)
                ApplyMeshSettings((ModelImporter) assetImporter);
        }

        void ApplyMeshSettings(ModelImporter importer)
        {
            importer.meshCompression = m_MeshCompression;
            importer.isReadable = m_IsReadable;
            importer.optimizeMesh = optimizeMeshForGPU;
            importer.importBlendShapes = m_ImportBlendShapes;
            importer.addCollider = m_AddColliders;
            importer.keepQuads = keepQuads;
            importer.weldVertices = m_weldVertices;
            importer.swapUVChannels = swapUVChannels;
            importer.generateSecondaryUV = generateSecondaryUV;
            
            //Normals & Tangents
            importer.importNormals = normalImportMode;
            importer.normalSmoothingAngle = normalSmoothAngle;
            importer.importTangents = tangentImportMode;
            importer.importMaterials = m_ImportMaterials;
            importer.materialName = m_MaterialName;
            
            //rig
            importer.animationType = AnimationType;
            importer.optimizeGameObjects = isOptimizeObject;

            //animations
            importer.importAnimation = ImportAnimation;
            importer.animationCompression = AnimCompression;
            
            Debug.Log("Modifying Model Import Settings, An Import will now occur and the settings will be checked to be OK again during that import");
            //importer.SaveAndReimport();
        }

        public bool IsMatch(UnityEditor.AssetImporter importer)
        {
            if (importer is ModelImporter)
            {
                return true;
            }
            return false;
        }
    }

}

