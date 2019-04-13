using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class StartupScript : Editor {

    private static string sAssetFolderPath = "Assets/Models";
    private static GameObject model;
    private static string m_FilePath = "";

    [MenuItem("Assets/ShowBounds")]
    public static void ShowBounds()
    {
        Vector3 size = model.GetComponent<BoxCollider>().bounds.size;
        Vector3 mesh = model.GetComponent<MeshFilter>().sharedMesh.bounds.size;
        Debug.Log(size.x + "-" + size.y + "-" + size.z);
        Debug.Log(mesh.x + "-" + mesh.y + "-" + mesh.z);
    }

    [MenuItem("Assets/AddModelsToScene")]
    public static void Init()
    {
        FileInfo filePath = new FileInfo(Directory.GetFiles(sAssetFolderPath, "*.fbx").FirstOrDefault());

        if (filePath != null)
        {
            m_FilePath = sAssetFolderPath + "/" + filePath.Name;

            Debug.Log(m_FilePath);
            
            try
            {
                //Prepare FBX
                ModelImporter modelImporter = AssetImporter.GetAtPath(m_FilePath) as ModelImporter;
                //modelImporter.ExtractTextures("Assets\\Models\\Textures");
                modelImporter.materialLocation = ModelImporterMaterialLocation.External;
                modelImporter.materialName = ModelImporterMaterialName.BasedOnMaterialName;
                modelImporter.animationType = ModelImporterAnimationType.Legacy;
                //ExtractMaterials(sFilePath, modelImporter);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                EditorApplication.Exit(1);
            }
            
            AssetDatabase.WriteImportSettingsIfDirty(m_FilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            try
            {
                PrepareModel();
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                EditorApplication.Exit(2);
            }
        }
    }

    private static void PrepareModel()
    {
        UnityEngine.Object modelFBX = AssetDatabase.LoadAssetAtPath(m_FilePath, typeof(UnityEngine.Object));

        model = Instantiate(modelFBX) as GameObject;
        model.transform.position = Vector3.zero;
        model.transform.localScale = Vector3.one;
        model.AddComponent<BoxCollider>();
        AttachBoxCollider.Init(model);

        //Scale object
        //ScaleModel(model);

        //DestroyImmediate(model.GetComponent<BoxCollider>());
        //model.AddComponent<BoxCollider>();
        //AttachBoxCollider.Init(model);

        //Attach textures
        FindTextures.Init(model);

        //ScreenCapture.CaptureScreenshot("Assets/Screenshot/screenshot.png");

        PrefabUtility.SaveAsPrefabAsset(model, "Assets/Prefabs/model.prefab");
        //DestroyImmediate(model);

        CreateAssetBundle();
    }

    private static void ScaleModel(GameObject model)
    {
        //model.GetComponent<BoxCollider>().bounds.SetMinMax(new Vector3(.1f,.1f,.1f), new Vector3(2.0f, 2.0f, 2.0f));

        Vector3 size = model.GetComponent<BoxCollider>().bounds.size;
        float deltaScale = 0f;
        float minSize = Mathf.Min(size.x, size.y, size.z);
        float maxSize = Mathf.Max(size.x, size.y, size.z);
        float scalePercent = 0.0f;

        if (maxSize > 2.0f){
            deltaScale = maxSize - 2.0f;

            if (minSize - 2.2f < 0.0f)
            {
                deltaScale -= (2.2f - minSize);
            }

            scalePercent = deltaScale / (maxSize / 100);
        }
        else if (maxSize < 2.0f)
        {
            deltaScale = 2.0f - maxSize;
            scalePercent = deltaScale / (maxSize / 100) *-1;
        }
        /*
        model.transform.localScale = new Vector3(
            model.transform.localScale.x - model.transform.localScale.x / 100 * scalePercent,
            model.transform.localScale.y - model.transform.localScale.y / 100 * scalePercent,
            model.transform.localScale.z - model.transform.localScale.z / 100 * scalePercent
            );
        */
        RecalcualteMesh.ScaleModel(model, scalePercent);
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
