using System.IO;
using UnityEngine;
using UnityEditor;

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
            if (Path.GetExtension(sFilePath) == ".FBX" || Path.GetExtension(sFilePath) == ".fbx")
            {
                Debug.Log(sFilePath);

                Object modelFBX = AssetDatabase.LoadAssetAtPath(sFilePath, typeof(Object));

                GameObject model = Instantiate(modelFBX) as GameObject;
                model.AddComponent<AttachBoxCollider>();
                //Attach textures
                //Create Snapshot
                PrefabUtility.CreatePrefab("Assets/Prefabs/model.prefab", model);
                //AssetDatabase.CreateAsset(model, "Assets/Prefabs/model.prefab");
                //AssetDatabase.AddObjectToAsset();
                DestroyImmediate(model);

                //CreateAssetBundle
                
            }
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
