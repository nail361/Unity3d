using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class FindTextures : MonoBehaviour {

    private int maxTextureSize = 512;

    private List<Material> materials;
    private List<string> textures;
    private List<string> texturesReady = new List<string>();

    private const string MAIN_TEX = "MainTex";
    private const string NORMAL_MAP = "NormalMap";

    private void Start()
    {
        materials = new List<Material>();

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] mats = renderer.sharedMaterials;
            foreach (Material mat in mats)
                if (!materials.Contains(mat))
                    materials.Add(mat);
        }

        textures = GetTextures();
        AssignTextures();
    }

    private List<string> GetTextures()
    {
        List<string> texturePathsList = new List<string>();
        string[] pngPaths = null;
        string[] jpgPaths = null;
        
        pngPaths = Directory.GetFiles("Assets\\Models\\", "*.png", SearchOption.AllDirectories);
        jpgPaths = Directory.GetFiles("Assets\\Models\\", "*.jpg", SearchOption.AllDirectories);

        texturePathsList.AddRange(pngPaths);
        texturePathsList.AddRange(jpgPaths);

        foreach (string path in texturePathsList)
            Debug.Log("path: " + path);

        return texturePathsList;
    }

    private void AssignTextures()
    {
        foreach(Material mat in materials)
        {
            foreach (string texturePath in textures)
            {
                string[] textureNameSplit = Path.GetFileName(texturePath).Split('_');
                string textureRole = textureNameSplit[textureNameSplit.Length-1];

                for (int i = 0; i < textureNameSplit.Length - 1; i++)
                {
                    if (mat.name.Contains(textureNameSplit[i]))
                    {
                        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
                        if (!texturesReady.Contains(texturePath))
                        {
                            importer.maxTextureSize = maxTextureSize;
                            if (textureRole.Contains(NORMAL_MAP))
                            {
                                importer.textureType = TextureImporterType.NormalMap;
                                importer.convertToNormalmap = true;
                            }

                            AssetDatabase.WriteImportSettingsIfDirty(texturePath);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();

                            texturesReady.Add(texturePath);
                        }

                        Texture2D texture = (Texture2D)(AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)));

                        switch (Path.GetFileNameWithoutExtension(textureRole))
                        {
                            case MAIN_TEX:
                                    mat.SetTexture("_MainTex", texture);
                                    mat.SetColor("_MainTex", Color.white);
                                break;
                            case NORMAL_MAP:
                                    mat.SetTexture("_BumpMap", texture);
                                    mat.SetFloat("_BumpScale", 0.1f);
                                break;
                        }
                    }
                }
            }
        }
    }
}
