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
        #region 字段声明
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

        #region 编辑器窗口
        [MenuItem("Tools/WFC Manager")]
        public static void ShowWindow()
        {
            GetWindow<WFC_Manager>("WFC Manager");
        }

        private void OnGUI()
        {
            DrawPathSelector("Prefab 文件夹", ref prefabFolderPath);
            DrawPathSelector("SO保存路径", ref soFolderPath);
            DrawPathSelector("方向列表SO路径", ref listSoFolderPath);

            GUILayout.Space(15);
            if (GUILayout.Button("生成父级SO", GUILayout.MaxHeight(30)))
                CreateParentChunkParts();

            if (GUILayout.Button("生成子级级SO", GUILayout.MaxHeight(30)))
                CreateChildChunkParts();

            if (GUILayout.Button("生成方向列表", GUILayout.MaxHeight(30)))
                CreateDirectionLists();

            if (GUILayout.Button("自动填充方向列表", GUILayout.MaxHeight(30)))
                AutoPopulateDirectionLists();

            if (GUILayout.Button("自动填充SO", GUILayout.MaxHeight(30)))
                AutoPopulateSO();

            DrawObjectSelector("MapChunkSO", ref chunkSO);

            if (GUILayout.Button("自动填充MapChunk", GUILayout.MaxHeight(30)))
                AutoPopulateMapChunk();
        }


        private void DrawPathSelector(string label, ref string path)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField(label, path);
            if (GUILayout.Button("选择", GUILayout.Width(60)))
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
            if (GUILayout.Button("选择", GUILayout.Width(60)))
            {
                var newSo = EditorUtility.OpenFilePanel(label, Application.dataPath, "asset");
                if (!string.IsNullOrEmpty(newSo))
                    so = AssetDatabase.LoadAssetAtPath<MapChunkSO>(newSo.Replace(Application.dataPath, "Assets"));
            }
            GUILayout.EndHorizontal();
        }
        #endregion

        #region 核心逻辑
        private void CreateParentChunkParts()
        {

            if (!IsValidPath(prefabFolderPath) || !IsValidPath(soFolderPath))
                return;
            try
            {
                Debug.Log("开始创建父级部件SO...");
                EditorUtility.DisplayProgressBar("正在处理", "加载Prefab...", 0.1f);
                var prefabs = LoadPrefabs(prefabFolderPath);

                EditorUtility.DisplayProgressBar("正在处理", "创建父级部件...", 0.3f);
                var created = prefabs.Select(CreateParentChunkPart).Where(so => so != null).ToList();

                FinalizeCreation(created, "父级部件创建完成");
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
                EditorUtility.DisplayProgressBar("正在处理", "加载父级SO...", 0.1f);
                var parents = LoadParentChunkParts(soFolderPath);

                EditorUtility.DisplayProgressBar("正在处理", "生成子级旋转...", 0.3f);
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

                FinalizeCreation(created, "子级部件创建完成");
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

                FinalizeCreation(created, "方向列表创建完成");
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
                Debug.LogError("请先选择一个MapChunkSO");
                return;
            }
            try
            {
                Debug.Log("开始自动填充MapChunk...");
                var chunkParts = LoadChunkParts(soFolderPath);
                chunkSO.AllChunkPartSO = chunkParts.ToArray(); // 修复：将 List 转换为数组  
                EditorUtility.SetDirty(chunkSO);
                Debug.Log("MapChunk填充完成");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
        private void AutoPopulateDirectionLists()
        {
            if (!ValidatePaths()) return;
            Debug.Log("开始自动填充方向列表...");
            try
            {
                var chunkParts = LoadChunkParts(soFolderPath);
                Debug.Log($"加载父级SO: {chunkParts.Count} 个");
                var directionLists = LoadDirectionLists(listSoFolderPath);
                Debug.Log($"加载方向列表SO: {directionLists.Count} 个");

                // 预构建方向映射表
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
                    // 跳过未定义的方向类型
                    if (!directionMap.TryGetValue(list.direction, out var directions))
                        continue;

                    // 根据negative标志确定检查方向
                    var (mainDir, oppositeDir) = list.negative ?
                        (directions.positive, directions.negative) :
                        (directions.positive, directions.negative);

                    PopulateList(list, chunkParts, mainDir, oppositeDir);
                    EditorUtility.SetDirty(list);
                    Debug.Log($"填充方向列表 {list.name} 完成，包含 {list.chunkPartSOs.Count} 个部件");
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

            Debug.Log("开始自动填充邻居关系...");
            try
            {
                // 加载数据
                var chunkParts = LoadChunkParts(soFolderPath);
                Debug.Log($"加载父级SO: {chunkParts.Count} 个");
                var directionLists = LoadDirectionLists(listSoFolderPath);
                Debug.Log($"加载方向列表SO: {directionLists.Count} 个");


                // 建立方向列表字典
                var directionDict = directionLists
                    .Where(list => list.negative != true)
                    .ToDictionary(list => list.direction);

                var directionDictNegative = directionLists
                    .Where(list => list.negative == true)
                    .ToDictionary(list => list.direction);
                foreach (var part in chunkParts)
                {
                    Debug.Log($"正在填充 {part.name} 的邻居关系...");
                    foreach (var dir in SquareDirections.SixDirections)
                    {
                        part.SetNeighbor(directionDictNegative[dir], dir);
                        Debug.Log($"添加 {dir} 方向的邻居 {directionDictNegative[dir].name}");
                    }
                    foreach (var dir in part.chunkPartData.directions)
                    {
                        part.SetNeighbor(directionDict[-dir], dir);
                        Debug.Log($"添加 {dir} 方向的邻居 {directionDict[-dir].name}");
                    }
                    EditorUtility.SetDirty(part);
                }

                Debug.Log("邻居关系填充完成");
            }
            //catch (Exception ex)
            //{
            //    Debug.LogError($"自动填充失败: {ex.Message}");
            //}
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }



        #endregion

        #region 辅助方法
        private bool ValidatePaths()
        {
            return IsValidPath(soFolderPath) && IsValidPath(listSoFolderPath);
        }

        private bool ValidateDirectionList(ChunkPartListSO list)
        {
            if (list.direction == Vector3Int.zero)
            {
                Debug.LogError($"方向列表 {list.name} 的方向为零向量，跳过填充。");
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
                Debug.LogError("路径不能为空");
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

            Debug.Log($"{message}，共生成 {assets.Count} 个资产");
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


        // 使用旋转矩阵的优化版本
        private List<Vector3Int> CalculateDirections(List<Vector3Int> dirs, AxisType axis, int degrees)
        {
            // 验证输入有效性
            if (dirs == null) return new List<Vector3Int>();
            if (!new[] { 90, 180, 270 }.Contains(degrees))
                throw new ArgumentException(degrees + " 仅支持90度倍数旋转");

            // 生成旋转矩阵
            var rotationMatrix = GetRotationMatrix(axis, degrees);

            return dirs.Select(dir =>
            {
                // 应用旋转矩阵
                Vector3 rotated = rotationMatrix.MultiplyVector(dir);
                // 四舍五入处理浮点精度
                return new Vector3Int(
                    Mathf.RoundToInt(rotated.x),
                    Mathf.RoundToInt(rotated.y),
                    Mathf.RoundToInt(rotated.z)
                );
            }).ToList();
        }

        /// <summary>
        /// 生成标准右手坐标系旋转矩阵
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