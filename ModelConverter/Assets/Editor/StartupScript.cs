using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class StartupScript : Editor {

    private static string sAssetFolderPath = "Assets/Models";
    private static GameObject model;
    private static string m_FilePath = "";

    private static float waiteTime = 2000.0f;
    
    private delegate void Callback();
    private static Callback callback;

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

                Enabled();
                callback += PrepareModel;
            }
        }

        //EditorApplication.Exit(0);
    }

    private static void PrepareModel()
    {
        callback -= PrepareModel;
        Disabled();

        Object modelFBX = AssetDatabase.LoadAssetAtPath(m_FilePath, typeof(Object));

        model = Instantiate(modelFBX) as GameObject;

        model.AddComponent<BoxCollider>();
        AttachBoxCollider.Init(model);
        //Scale object
        //ScaleModel(model);
        //Attach textures
        FindTextures.Init(model);

        Enabled();
        callback += CreateScreenshoot;
    }

    private static void CreateScreenshoot()
    {
        callback -= CreateScreenshoot;
        Disabled();
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
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", buildMap, BuildAssetBundleOptions.None, BuildTarget.iOS);
    }

    //private static float m_LastEditorUpdateTime;
    private static float timeElapsed = 0.0f;
    private static void Enabled()
    {
        //m_LastEditorUpdateTime = Time.realtimeSinceStartup;
        timeElapsed = 0.0f;
        EditorApplication.update += OnEditorUpdate;
    }
    private static void Disabled()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        if (timeElapsed > waiteTime)
        {
            Debug.Log(timeElapsed);
            callback();
        }
        else
        {
            timeElapsed += 1;
        }
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
// ./Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -quit -projectPath ${PROJECT_PATH} -executeMethod MyScripName.MyMethod
