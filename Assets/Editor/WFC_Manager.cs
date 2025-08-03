using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static ChunkPartSO;
using System.IO;
namespace Xhakua.Editor
{
    public class WFC_Manager : EditorWindow
    {
        #region �ֶ�����
        private static string soFolderPath;
        private static string prefabFolderPath;
        private static string listSoFolderPath;
        public MapChunkSO chunkSO;
        private enum AxisType { None, X, Y, Z }
        private struct DirectionVariant
        {
            public string suffix;
            public int multiplier;
        }
        #endregion

        #region �༭������
        [MenuItem("Tools/WFC Manager")]
        public static void ShowWindow()
        {
            GetWindow<WFC_Manager>("WFC Manager");
        }

        private void OnGUI()
        {
            DrawPathSelector("Prefab �ļ���", ref prefabFolderPath);
            DrawPathSelector("SO����·��", ref soFolderPath);
            DrawPathSelector("�����б�SO·��", ref listSoFolderPath);

            GUILayout.Space(15);
            if (GUILayout.Button("���ɸ���SO", GUILayout.MaxHeight(30)))
                CreateParentChunkParts();

            if (GUILayout.Button("�����Ӽ���SO", GUILayout.MaxHeight(30)))
                CreateChildChunkParts();

            if (GUILayout.Button("���ɷ����б�", GUILayout.MaxHeight(30)))
                CreateDirectionLists();

            if (GUILayout.Button("�Զ���䷽���б�", GUILayout.MaxHeight(30)))
                AutoPopulateDirectionLists();

            if (GUILayout.Button("�Զ����SO", GUILayout.MaxHeight(30)))
                AutoPopulateSO();

            DrawObjectSelector("MapChunkSO", ref chunkSO);

            if (GUILayout.Button("�Զ����MapChunk", GUILayout.MaxHeight(30)))
                AutoPopulateMapChunk();
        }


        private void DrawPathSelector(string label, ref string path)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField(label, path);
            if (GUILayout.Button("ѡ��", GUILayout.Width(60)))
            {
                var newPath = EditorUtility.OpenFolderPanel(label, Application.dataPath, "");
                if (!string.IsNullOrEmpty(newPath))
                    path = newPath.Replace(Application.dataPath, "Assets");
            }
            GUILayout.EndHorizontal();
        }

        private void DrawObjectSelector(string label, ref MapChunkSO so)
        {
            GUILayout.BeginHorizontal();
            so = (MapChunkSO)EditorGUILayout.ObjectField(label, so, typeof(MapChunkSO), false);
            if (GUILayout.Button("ѡ��", GUILayout.Width(60)))
            {
                var newSo = EditorUtility.OpenFilePanel(label, Application.dataPath, "asset");
                if (!string.IsNullOrEmpty(newSo))
                    so = AssetDatabase.LoadAssetAtPath<MapChunkSO>(newSo.Replace(Application.dataPath, "Assets"));
            }
            GUILayout.EndHorizontal();
        }
        #endregion

        #region �����߼�
        private void CreateParentChunkParts()
        {

            if (!IsValidPath(prefabFolderPath) || !IsValidPath(soFolderPath))
                return;
            try
            {
                Debug.Log("��ʼ������������SO...");
                EditorUtility.DisplayProgressBar("���ڴ���", "����Prefab...", 0.1f);
                var prefabs = LoadPrefabs(prefabFolderPath);

                EditorUtility.DisplayProgressBar("���ڴ���", "������������...", 0.3f);
                var created = prefabs.Select(CreateParentChunkPart).Where(so => so != null).ToList();

                FinalizeCreation(created, "���������������");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void CreateChildChunkParts()
        {

            if (!IsValidPath(soFolderPath))
                return;
            try
            {
                EditorUtility.DisplayProgressBar("���ڴ���", "���ظ���SO...", 0.1f);
                var parents = LoadParentChunkParts(soFolderPath);

                EditorUtility.DisplayProgressBar("���ڴ���", "�����Ӽ���ת...", 0.3f);
                var created = new List<ChunkPartSO>();

                foreach (var parent in parents)
                {
                    var axis = DetectRotationAxis(parent);
                    if (axis == AxisType.None) continue;

                    for (int i = 0; i < 3; i++)
                    {
                        created.Add(CreateChildChunkPart(parent, axis, i));
                    }
                }

                FinalizeCreation(created, "�Ӽ������������");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void CreateDirectionLists()
        {
            if (!IsValidPath(listSoFolderPath))
                return;
            try
            {
                var variants = new[]
                {
                    new DirectionVariant { suffix = "+", multiplier = 1 },
                    new DirectionVariant { suffix = "-", multiplier = -1 }
                };

                var created = (from dir in SquareDirections.SixDirections
                               from variant in variants
                               select CreateDirectionList(dir, variant)).ToList();

                FinalizeCreation(created, "�����б������");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        private void AutoPopulateMapChunk()
        {
            if (!ValidatePaths()) return;
            if (chunkSO == null)
            {
                Debug.LogError("����ѡ��һ��MapChunkSO");
                return;
            }
            try
            {
                Debug.Log("��ʼ�Զ����MapChunk...");
                var chunkParts = LoadChunkParts(soFolderPath);
                chunkSO.AllChunkPartSO = chunkParts.ToArray(); // �޸����� List ת��Ϊ����  
                EditorUtility.SetDirty(chunkSO);
                Debug.Log("MapChunk������");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        private void AutoPopulateDirectionLists()
        {
            if (!ValidatePaths()) return;
            Debug.Log("��ʼ�Զ���䷽���б�...");
            try
            {
                var chunkParts = LoadChunkParts(soFolderPath);
                Debug.Log($"���ظ���SO: {chunkParts.Count} ��");
                var directionLists = LoadDirectionLists(listSoFolderPath);
                Debug.Log($"���ط����б�SO: {directionLists.Count} ��");

                // Ԥ��������ӳ���
                var directionMap = new Dictionary<Vector3Int, (Vector3Int positive, Vector3Int negative)>
                     {
                       { Vector3Int.left,   (Vector3Int.left,  Vector3Int.right) },
                       { Vector3Int.right,  (Vector3Int.right, Vector3Int.left) },
                       { Vector3Int.down,   (Vector3Int.down,  Vector3Int.up) },
                       { Vector3Int.up,     (Vector3Int.up,    Vector3Int.down) },
                       { Vector3Int.back,   (Vector3Int.back,  Vector3Int.forward) },
                       { Vector3Int.forward,(Vector3Int.forward, Vector3Int.back) }
                     };

                foreach (var list in directionLists.Where(ValidateDirectionList))
                {
                    // ����δ����ķ�������
                    if (!directionMap.TryGetValue(list.direction, out var directions))
                        continue;

                    // ����negative��־ȷ����鷽��
                    var (mainDir, oppositeDir) = list.negative ?
                        (directions.positive, directions.negative) :
                        (directions.positive, directions.negative);

                    PopulateList(list, chunkParts, mainDir, oppositeDir);
                    EditorUtility.SetDirty(list);
                    Debug.Log($"��䷽���б� {list.name} ��ɣ����� {list.chunkPartSOs.Count} ������");
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();

            }



        }
        private void AutoPopulateSO()
        {
            if (!ValidatePaths()) return;

            Debug.Log("��ʼ�Զ�����ھӹ�ϵ...");
            try
            {
                // ��������
                var chunkParts = LoadChunkParts(soFolderPath);
                Debug.Log($"���ظ���SO: {chunkParts.Count} ��");
                var directionLists = LoadDirectionLists(listSoFolderPath);
                Debug.Log($"���ط����б�SO: {directionLists.Count} ��");


                // ���������б��ֵ�
                var directionDict = directionLists
                    .Where(list => list.negative != true)
                    .ToDictionary(list => list.direction);

                var directionDictNegative = directionLists
                    .Where(list => list.negative == true)
                    .ToDictionary(list => list.direction);
                foreach (var part in chunkParts)
                {
                    Debug.Log($"������� {part.name} ���ھӹ�ϵ...");
                    foreach (var dir in SquareDirections.SixDirections)
                    {
                        part.SetNeighbor(directionDictNegative[dir], dir);
                        Debug.Log($"��� {dir} ������ھ� {directionDictNegative[dir].name}");
                    }
                    foreach (var dir in part.chunkPartData.directions)
                    {
                        part.SetNeighbor(directionDict[-dir], dir);
                        Debug.Log($"��� {dir} ������ھ� {directionDict[-dir].name}");
                    }
                    EditorUtility.SetDirty(part);
                }

                Debug.Log("�ھӹ�ϵ������");
            }
            //catch (Exception ex)
            //{
            //    Debug.LogError($"�Զ����ʧ��: {ex.Message}");
            //}
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }



        #endregion

        #region ��������
        private bool ValidatePaths()
        {
            return IsValidPath(soFolderPath) && IsValidPath(listSoFolderPath);
        }

        private bool ValidateDirectionList(ChunkPartListSO list)
        {
            if (list.direction == Vector3Int.zero)
            {
                Debug.LogError($"�����б� {list.name} �ķ���Ϊ��������������䡣");
                return false;
            }
            return true;
        }

        private void PopulateList(ChunkPartListSO targetList,
                                IEnumerable<ChunkPartSO> sourceParts,
                                Vector3Int requiredDirection,
                                Vector3Int excludedDirection)
        {
            foreach (var part in sourceParts)
            {
                var partDirections = part.chunkPartData.directions;

                if (!targetList.negative && partDirections.Contains(requiredDirection))
                {
                    targetList.chunkPartSOs.Add(part);
                }
                else if (targetList.negative && !partDirections.Contains(excludedDirection))
                {
                    targetList.chunkPartSOs.Add(part);
                }
            }
        }
        private bool IsValidPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("·������Ϊ��");
                return false;
            }
            return true;
        }


        private List<GameObject> LoadPrefabs(string path)
        {
            return AssetDatabase.FindAssets("t:Prefab", new[] { path })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(p => AssetDatabase.LoadAssetAtPath<GameObject>(p))
                .Where(p => p != null).ToList();
        }
        private List<ChunkPartSO> LoadChunkParts(string path)
        {
            return AssetDatabase.FindAssets("t:ChunkPartSO", new[] { path })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(p => AssetDatabase.LoadAssetAtPath<ChunkPartSO>(p))
                .Where(so => so != null).ToList();
        }
        private List<ChunkPartSO> LoadParentChunkParts(string path)
        {
            return AssetDatabase.FindAssets("t:ChunkPartSO", new[] { path })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(p => AssetDatabase.LoadAssetAtPath<ChunkPartSO>(p))
                .Where(so => so?.chunkPartData.isParent ?? false).ToList();
        }
        private List<ChunkPartListSO> LoadDirectionLists(string path)
        {
            return AssetDatabase.FindAssets("t:ChunkPartListSO", new[] { path })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(p => AssetDatabase.LoadAssetAtPath<ChunkPartListSO>(p))
                .Where(so => so != null).ToList();
        }

        private ChunkPartSO CreateParentChunkPart(GameObject prefab)
        {
            var so = ScriptableObject.CreateInstance<ChunkPartSO>();

            so.name = $"{prefab.name}_Parent";
            so.chunkPartData = new ChunkPartData
            {
                isParent = true,
            };
            so.SetPrefab(prefab);
            SaveAsset(so, soFolderPath);
            return so;
        }

        private ChunkPartSO CreateChildChunkPart(ChunkPartSO parent, AxisType axis, int index)
        {
            var rotation = CalculateRotation(axis, index);

            var child = ScriptableObject.CreateInstance<ChunkPartSO>();




            child.chunkPartData.isParent = false;
            child.SetPrefab(parent.GetPrefab().Item1);
            switch (axis)
            {
                case AxisType.X:
                    child.needDir = new Vector3Int(rotation.x, 0, 0);
                    child.chunkPartData.directions = CalculateDirections(parent.chunkPartData.directions, axis, rotation.x);
                    break;
                case AxisType.Y:
                    child.needDir = new Vector3Int(0, rotation.y, 0);
                    child.chunkPartData.directions = CalculateDirections(parent.chunkPartData.directions, axis, rotation.y);
                    break;
                case AxisType.Z:
                    child.needDir = new Vector3Int(0, 0, rotation.z);
                    child.chunkPartData.directions = CalculateDirections(parent.chunkPartData.directions, axis, rotation.z);
                    break;
            }
            string dirString = "";
            foreach (var dir in child.chunkPartData.directions)
            {
                if (dir.x == -1) dirString += "L";
                else if (dir.x == 1) dirString += "R";
                else if (dir.y == 1) dirString += "U";
                else if (dir.y == -1) dirString += "D";
                else if (dir.z == 1) dirString += "F";
                else if (dir.z == -1) dirString += "B";
            }

            child.name = $"{parent.name}_Child_{dirString}";

            SaveAsset(child, soFolderPath);
            return child;
        }

        private ChunkPartListSO CreateDirectionList(Vector3Int dir, DirectionVariant variant)
        {
            var finalDir = new Vector3Int(
                dir.x * variant.multiplier,
                dir.y * variant.multiplier,
                dir.z * variant.multiplier
            );

            var so = ScriptableObject.CreateInstance<ChunkPartListSO>();
            string dirString = "";
            if (finalDir.x == -1) dirString = "L";
            else if (finalDir.x == 1) dirString = "R";
            else if (finalDir.y == 1) dirString = "U";
            else if (finalDir.y == -1) dirString = "D";
            else if (finalDir.z == 1) dirString = "F";
            else if (finalDir.z == -1) dirString = "B";

            so.name = $"{variant.suffix}DirList_{dirString}";
            if (variant.suffix == "+")
            {
                so.negative = false;
            }
            else
            {
                so.negative = true;
            }
            so.direction = finalDir;
            SaveAsset(so, listSoFolderPath);
            return so;
        }

        private void SaveAsset(UnityEngine.Object asset, string folder)
        {
            var path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{asset.name}.asset");
            AssetDatabase.CreateAsset(asset, path);
            Undo.RegisterCreatedObjectUndo(asset, "Create Asset");
        }

        private void FinalizeCreation<T>(List<T> assets, string message) where T : UnityEngine.Object
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (assets.Count > 0)
            {
                Selection.activeObject = assets.Last();
                EditorGUIUtility.PingObject(assets.Last());
            }

            Debug.Log($"{message}�������� {assets.Count} ���ʲ�");
        }

        private AxisType DetectRotationAxis(ChunkPartSO so)
        {
            var shaft = so.chunkPartData.rotaryShaft;
            if (shaft.x > 0.9f) return AxisType.X;
            if (shaft.y > 0.9f) return AxisType.Y;
            if (shaft.z > 0.9f) return AxisType.Z;
            return AxisType.None;
        }

        private Vector3Int CalculateRotation(AxisType axis, int index)
        {
            var degrees = 90 * (index + 1);
            return axis switch
            {
                AxisType.X => new Vector3Int(degrees, 0, 0),
                AxisType.Y => new Vector3Int(0, degrees, 0),
                AxisType.Z => new Vector3Int(0, 0, degrees),
                _ => Vector3Int.zero
            };
        }


        // ʹ����ת������Ż��汾
        private List<Vector3Int> CalculateDirections(List<Vector3Int> dirs, AxisType axis, int degrees)
        {
            // ��֤������Ч��
            if (dirs == null) return new List<Vector3Int>();
            if (!new[] { 90, 180, 270 }.Contains(degrees))
                throw new ArgumentException(degrees + " ��֧��90�ȱ�����ת");

            // ������ת����
            var rotationMatrix = GetRotationMatrix(axis, degrees);

            return dirs.Select(dir =>
            {
                // Ӧ����ת����
                Vector3 rotated = rotationMatrix.MultiplyVector(dir);
                // �������봦���㾫��
                return new Vector3Int(
                    Mathf.RoundToInt(rotated.x),
                    Mathf.RoundToInt(rotated.y),
                    Mathf.RoundToInt(rotated.z)
                );
            }).ToList();
        }

        /// <summary>
        /// ���ɱ�׼��������ϵ��ת����
        /// </summary>
        private Matrix4x4 GetRotationMatrix(AxisType axis, int degrees)
        {
            float radians = Mathf.Deg2Rad * degrees;

            switch (axis)
            {
                case AxisType.X:
                    return Matrix4x4.Rotate(Quaternion.Euler(degrees, 0, 0));

                case AxisType.Y:
                    return Matrix4x4.Rotate(Quaternion.Euler(0, degrees, 0));

                case AxisType.Z:
                    return Matrix4x4.Rotate(Quaternion.Euler(0, 0, degrees));

                default:
                    return Matrix4x4.identity;
            }
        }
        #endregion
    }
}