using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class SearchExample : MonoBehaviour
{
    [MenuItem("Search/Get Texture In Example Assets")]
    public static void GetTexture()
    {
        string[] assetGuids = AssetDatabase.FindAssets("t:texture", new[] { "Assets/ExampleAssets" });
        string[] assetPathList = Array.ConvertAll<string, string>(assetGuids, AssetDatabase.GUIDToAssetPath);

        foreach(string assetPath in assetPathList)
        {
            Texture texture = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture)) as Texture;
            Debug.Log("에셋 이름 : " + GetAssetName(assetPath));
            Debug.Log("에셋 경로 : " + GetAssetPath(assetPath));
        }
    }

    // Asset path를 넣으면 name 반환 
    public static string GetAssetName(string assetPath)
    {
        return assetPath.Substring(assetPath.LastIndexOf("/") + 1);
    }

    // Asset path를 넣으면 경로만 반환 name 제외
    public static string GetAssetPath(string assetPath)
    {
        return assetPath.Substring(0, assetPath.LastIndexOf("/"));
    }

    // 선택된 Asset의 Path를 가져옵니다.
    [MenuItem("Search/Get Select Asset Path")]
    public static void GetSelectedAssetPath()
    {
        UnityEngine.Object selectObj = Selection.activeObject;
        string assetPath = AssetDatabase.GetAssetPath(selectObj.GetInstanceID());
        Debug.Log(assetPath);
    }

    public static bool IsFolder(UnityEngine.Object obj)
    {
        if(obj == null)
        {
            Debug.LogError("folder is null");
            return false;
        }

        string path = "";
        path = AssetDatabase.GetAssetPath(obj.GetInstanceID());

        if(path.Length > 0)
        {
            if(Directory.Exists(path))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsExist(string assetPath)
    {
        if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(assetPath)))
        {
            return false;
        }
        return true;
    }

    [MenuItem("Tools/CreateMaterialFile")]
    public static void CreateMaterialFile()
    {
        UnityEngine.Object selectObj = Selection.activeObject;
        bool isFolder = IsFolder(selectObj);
        if(!isFolder)
        {
            Debug.LogError("폴더를 선택하세요.");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(selectObj.GetInstanceID());
        string assetPath = string.Format(folderPath + "/{0}.mat", "SampleMaterial");

        bool isExist = IsExist(assetPath);

        if(isExist)
        {
            Debug.LogWarning("File is Exist");
            return;
        }

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        AssetDatabase.CreateAsset(mat, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/EditModelFile")]
    public static void EditModelFile()
    {
        UnityEngine.Object selectObj = Selection.activeObject;
        string assetPath = AssetDatabase.GetAssetPath(selectObj.GetInstanceID());

        string assetName = GetAssetName(assetPath);
        if(assetName.Contains(".fbx") || assetName.Contains("FBX"))
        {
            ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            importer.materialImportMode = ModelImporterMaterialImportMode.None;
            importer.SaveAndReimport();
            EditorUtility.SetDirty(selectObj);
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("It is not FBX");
        }
    }
}
