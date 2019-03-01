using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class StartupScript : Editor {

    private static string sAssetFolderPath = "Assets/Models";
    private static GameObject model;
    private static string m_FilePath = "";

    [MenuItem("Assets/AddModelsToScene")]
    public static void Init()
    {

        string[] aux = sAssetFolderPath.Split(new char[] { '/' });
        string onlyFolderPath = aux[0] + "/" + aux[1] + "/";

        string[] aFilePaths = Directory.GetFiles(onlyFolderPath);

        foreach (string sFilePath in aFilePaths)
        {
            if (Path.GetExtension(sFilePath).ToLower() == ".fbx")
            {
                Debug.Log(sFilePath);
                m_FilePath = sFilePath;

                //Prepare FBX
                ModelImporter modelImporter  = AssetImporter.GetAtPath(sFilePath) as ModelImporter;
                //modelImporter.ExtractTextures("Assets\\Models\\Textures");
                modelImporter.materialLocation = ModelImporterMaterialLocation.External;
                modelImporter.materialName = ModelImporterMaterialName.BasedOnMaterialName;
                modelImporter.animationType = ModelImporterAnimationType.Legacy;
                //ExtractMaterials(sFilePath, modelImporter);

                AssetDatabase.WriteImportSettingsIfDirty(sFilePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                PrepareModel();
            }
        }
    }

    private static void PrepareModel()
    {
        Object modelFBX = AssetDatabase.LoadAssetAtPath(m_FilePath, typeof(Object));

        model = Instantiate(modelFBX) as GameObject;

        model.AddComponent<BoxCollider>();
        AttachBoxCollider.Init(model);
        //Scale object
        //ScaleModel(model);
        //Attach textures
        FindTextures.Init(model);

        CreateScreenshoot();
    }

    private static void CreateScreenshoot()
    {
        ScreenCapture.CaptureScreenshot("Assets/Screenshot/screenshot.png");

        PrefabUtility.SaveAsPrefabAsset(model, "Assets/Prefabs/model.prefab");

        //DestroyImmediate(model);

        CreateAssetBundle();
    }

    private static void ScaleModel(GameObject model)
    {
        Vector3 size = model.GetComponent<BoxCollider>().bounds.size;
        float deltaScale = 0f;

        if (Mathf.Max(size.x, size.y, size.z) > 2f){
            deltaScale = Mathf.Max(size.x, size.y, size.z) - 2f;
        }

    }

    private static void ExtractMaterials(string importedAssets, ModelImporter modelImporter)
    {
        string materialsPath = "Assets\\Models\\Materials\\";

        var materials = AssetDatabase.LoadAllAssetsAtPath(modelImporter.assetPath).Where(x => x.GetType() == typeof(Material));

        foreach (var material in materials)
        {
            var newAssetPath = materialsPath + material.name + ".mat";
            AssetDatabase.MoveAsset(importedAssets+"\\"+ material.name + ".mat", newAssetPath);
            //AssetDatabase.AddObjectToAsset(material, newAssetPath);
        }
    }

    private static void CreateAssetBundle()
    {
        // Create the array of bundle build details.
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

        string[] modelAssets = new string[1];
        modelAssets[0] = "Assets/Prefabs/model.prefab";

        buildMap[0].assetNames = modelAssets;
        buildMap[0].assetBundleName = "model";

        //CreateAssetBundle
		BuildPipeline.BuildAssetBundles("Assets/AssetBundles/android", buildMap, BuildAssetBundleOptions.None, BuildTarget.Android);
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles/ios", buildMap, BuildAssetBundleOptions.None, BuildTarget.iOS);

        RemoveTrash();
    }

    private static void RemoveTrash()
    {
        AssetDatabase.DeleteAsset(m_FilePath);
        AssetDatabase.Refresh();

        EditorApplication.Exit(0);
    }
}

/*
// Helper function for getting the command line arguments
private static string GetArg(string name)
{
    var args = System.Environment.GetCommandLineArgs();
    for (int i = 0; i < args.Length; i++)
    {
        if (args[i] == name && args.Length > i + 1)
        {
            return args[i + 1];
        }
    }
    return null;
}
*/
// "Unity.exe path" -batchmode -quit -projectPath "project path" -executeMethod MyScripName.MyMethod
