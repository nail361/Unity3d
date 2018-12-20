using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class PostProcessor : AssetPostprocessor
{

    void OnPostprocessModel(GameObject g)
    {
        return;
        // Only operate on FBX files
        if (assetPath.ToLower().Contains(".fbx"))
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            modelImporter.ExtractTextures("Assets/Models/Textures");

            ExtractMaterials(assetPath, modelImporter);
        }
    }

    private void ExtractMaterials(string importedAssets, ModelImporter modelImporter)
    {
        string materialsPath = "Assets/Models/Materials";

        var materials = AssetDatabase.LoadAllAssetsAtPath(modelImporter.assetPath).Where(x => x.GetType() == typeof(Material));

        foreach (var material in materials)
        {
            var newAssetPath = string.Join(Path.DirectorySeparatorChar.ToString(), new[] { materialsPath, material.name }) + ".mat";
            AssetDatabase.AddObjectToAsset(material, newAssetPath);
        }

        AssetDatabase.WriteImportSettingsIfDirty(importedAssets);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}