using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class StartupScript : Editor {

    public static string sAssetFolderPath = "Assets/Models";

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

                //Prepare FBX
                ModelImporter modelImporter  = AssetImporter.GetAtPath(sFilePath) as ModelImporter;
                modelImporter.ExtractTextures("Assets\\Models\\Textures");
                //ExtractMaterials(sFilePath, modelImporter);

                Object modelFBX = AssetDatabase.LoadAssetAtPath(sFilePath, typeof(Object));

                GameObject model = Instantiate(modelFBX) as GameObject;
                //Scale object
                //ScaleCOde HERE
                model.AddComponent<BoxCollider>();
                AttachBoxCollider.Init(model);
                //Attach textures
                FindTextures.Init(model);

                AssetDatabase.WriteImportSettingsIfDirty(sFilePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                //ScreenCapture.CaptureScreenshot("Assets/Screenshot/screenshot.png");

                //PrefabUtility.CreatePrefab("Assets/Prefabs/model.prefab", model);

                //DestroyImmediate(model);

                //CreateAssetBundle();
            }
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
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", buildMap, BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}

/*
private float m_LastEditorUpdateTime;

protected virtual void OnEnable()
{
#if UNITY_EDITOR
    m_LastEditorUpdateTime = Time.realtimeSinceStartup;
    EditorApplication.update += OnEditorUpdate;
#endif
}

protected virtual void OnDisable()
{
#if UNITY_EDITOR
    EditorApplication.update -= OnEditorUpdate;
#endif
}

protected virtual void OnEditorUpdate()
{
    // In here you can check the current realtime, see if a certain
    // amount of time has elapsed, and perform some task.
}
*/

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
