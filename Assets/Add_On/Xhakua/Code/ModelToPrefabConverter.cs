#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class BatchPrefabConverter : EditorWindow
{
    private string outputPath = "Assets/Prefabs";
    private float progress;
    private bool isProcessing;
    private int processedCount;
    private int totalCount;
    private DefaultAsset targetFolder;

    [MenuItem("Tools/Batch Prefab Converter")]
    public static void ShowWindow()
    {
        GetWindow<BatchPrefabConverter>("����Ԥ����ת��");
    }

    void OnGUI()
    {
        GUILayout.Space(10);

        // ·��ѡ��
        EditorGUILayout.BeginHorizontal();
        targetFolder = EditorGUILayout.ObjectField("Դ�ļ���", targetFolder, typeof(DefaultAsset), false) as DefaultAsset;
        if (GUILayout.Button("���", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("ѡ��ģ���ļ���", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                targetFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path.Replace(Application.dataPath, "Assets"));
            }
        }
        EditorGUILayout.EndHorizontal();

        // ���·��
        outputPath = EditorGUILayout.TextField("���·��", outputPath);

        GUILayout.Space(20);

        // ������ʾ
        if (isProcessing)
        {
            Rect progressRect = EditorGUILayout.GetControlRect();
            EditorGUI.ProgressBar(progressRect, progress, $"���ڴ��� ({processedCount}/{totalCount})");
            Repaint();
        }

        GUILayout.FlexibleSpace();

        // ������ť
        EditorGUI.BeginDisabledGroup(isProcessing);
        if (GUILayout.Button("��ʼת��", GUILayout.Height(30)))
        {
            StartBatchConversion();
        }
        EditorGUI.EndDisabledGroup();
    }

    private void StartBatchConversion()
    {
        if (targetFolder == null)
        {
            EditorUtility.DisplayDialog("����", "����ѡ��Դ�ļ���", "ȷ��");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(targetFolder);
        List<GameObject> models = LoadModelsFromFolder(folderPath);

        totalCount = models.Count;
        processedCount = 0;
        isProcessing = true;

        EditorApplication.update += ProcessNextModel;
    }

    private List<GameObject> LoadModelsFromFolder(string folderPath)
    {
        List<GameObject> models = new List<GameObject>();
        string[] extensions = { ".fbx", ".obj", ".prefab" };

        foreach (string extension in extensions)
        {
            string[] guids = AssetDatabase.FindAssets($"t:GameObject", new[] { folderPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null && extension == Path.GetExtension(path).ToLower())
                {
                    models.Add(obj);
                }
            }
        }
        return models.Distinct().ToList();
    }

    private void ProcessNextModel()
    {
        if (processedCount >= totalCount)
        {
            FinishProcessing();
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(targetFolder);
        List<GameObject> models = LoadModelsFromFolder(folderPath);

        GameObject currentModel = models[processedCount];
        try
        {
            ConvertToPrefab(currentModel);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"ת��ʧ��: {currentModel.name}\n{e.Message}");
        }

        processedCount++;
        progress = (float)processedCount / totalCount;
    }

    private void ConvertToPrefab(GameObject model)
    {
        // �������Ŀ¼
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
            AssetDatabase.Refresh();
        }

        // ����Ψһ�ļ���
        string prefabName = $"{model.name}_Prefab.prefab";
        string finalPath = Path.Combine(outputPath, prefabName);
        finalPath = AssetDatabase.GenerateUniqueAssetPath(finalPath);

        // ʵ��������
        GameObject instance = PrefabUtility.InstantiatePrefab(model) as GameObject;

        // ������ʺ���ͼ
        HandleMaterials(instance);

        // ����Ԥ����
        PrefabUtility.SaveAsPrefabAsset(instance, finalPath);
        DestroyImmediate(instance);

        Debug.Log($"�ɹ�����Ԥ����: {finalPath}");
    }

    private void HandleMaterials(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null)
                {
                    string materialPath = Path.Combine(outputPath, "Materials", $"{materials[i].name}.mat");
                    Material newMaterial = SaveMaterial(materials[i], materialPath);
                    materials[i] = newMaterial;
                }
            }
            renderer.sharedMaterials = materials;
        }
    }

    private Material SaveMaterial(Material original, string path)
    {
        if (!AssetDatabase.Contains(original))
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            Material newMaterial = new Material(original);
            AssetDatabase.CreateAsset(newMaterial, path);
            return newMaterial;
        }
        return original;
    }

    private void FinishProcessing()
    {
        isProcessing = false;
        EditorApplication.update -= ProcessNextModel;
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("���", $"�ɹ�ת�� {processedCount} ��Ԥ����", "ȷ��");
    }
}
#endif