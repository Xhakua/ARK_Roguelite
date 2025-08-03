//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;

//namespace Xhakua.Editor
//{
//    public class ChunkPainter : EditorWindow
//    {
//        #region 字段声明
//        public enum AxisType { None, X, Y, Z }
//        public MapChunkSO pigment;
//        public Material sketchpadMaterial;
//        public GameObject mouseObject;
//        private AxisType _axisType;
//        private Dictionary<Vector3Int, ChunkState> _chunkStates = new();
//        [SerializeField] private GUISkin _uiSkin;
//        [SerializeField] private Vector2 _uiSize = new Vector2(200, 150);
//        [SerializeField] private float _uiOffset = 20f;
//        private Vector2 _iconScrollPos;
//        private int _selectedIndex = -1;
//        private Dictionary<GameObject, Texture2D> _iconCache = new Dictionary<GameObject, Texture2D>();

//        private bool _showUI;
//        private Vector3 _uiWorldPosition;
//        private Vector3Int selectedChunkPos;
//        public AxisType AxisTypeProperty
//        {
//            get => _axisType;
//            set
//            {
//                _axisType = value;
//                UpdateSketchpadOrientation();
//            }
//        }

//        private GameObject sketchpad;
//        #endregion

//        #region 编辑器窗口
//        [MenuItem("Tools/Chunk Painter")]
//        public static void ShowWindow()
//        {
//            GetWindow<ChunkPainter>("Chunk Painter");
//        }

//        private void OnGUI()
//        {
//            GUILayout.Label("Chunk Painter Settings", EditorStyles.boldLabel);
//            pigment = (MapChunkSO)EditorGUILayout.ObjectField("Map Chunk SO", pigment, typeof(MapChunkSO), false);
//            sketchpadMaterial = (Material)EditorGUILayout.ObjectField("Sketchpad Material", sketchpadMaterial, typeof(Material), false);

//            AxisTypeProperty = (AxisType)EditorGUILayout.EnumPopup("Drawing Axis", AxisTypeProperty);

//            if (mouseObject == null)
//            {
//                mouseObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//                mouseObject.name = "ChunkPainter_MouseObject";
//                mouseObject.transform.localScale = Vector3.one * 0.5f;
//                mouseObject.GetComponent<Collider>().enabled = false;
//                mouseObject.hideFlags = HideFlags.HideInHierarchy;
//            }
//            EditorGUILayout.Space();

//            if (GUILayout.Button("Create Sketchpad"))
//            {
//                CreateSketchpad();
//            }

//            if (sketchpad != null)
//            {
//                if (GUILayout.Button("Move Sketchpad Up"))
//                {
//                    MoveSketchpadToNewHeight(1);
//                }
//                if (GUILayout.Button("Move Sketchpad Down"))
//                {
//                    MoveSketchpadToNewHeight(-1);
//                }
//            }

//            EditorGUILayout.Space();

//            if (GUILayout.Button("Paint Chunk"))
//            {
//                // 添加绘制逻辑
//            }
//        }
//        #endregion

//        #region 核心逻辑


//        public void AddNewState(Vector3Int pos, ChunkState chunkState)
//        {
//            _chunkStates[pos] = chunkState;
//        }
//        public void RemoveState(Vector3Int pos)
//        {
//            _chunkStates.Remove(pos);
//        }

//        private void OnSceneGUI(SceneView sceneView)
//        {
//            Event currentEvent = Event.current;
//            if (_showUI)
//            {
//                Handles.BeginGUI();
//                DrawSceneUIWindow();
//                Handles.EndGUI();
//            }
//            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
//            {

//                DrawChunk();
//                if (pigment != null)
//                {
//                    selectedChunkPos = new Vector3Int(
//                       (int)mouseObject.transform.position.x,
//                       (int)mouseObject.transform.position.y,
//                       (int)mouseObject.transform.position.z
//                   );
//                    ShowEditUI(selectedChunkPos);
//                    currentEvent.Use();
//                }
//            }
//            if (!_showUI && currentEvent.type == EventType.MouseMove)
//            {
//                RaycastHit hit;
//                Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
//                if (Physics.Raycast(ray, out hit))
//                {
//                    Vector3 hitPoint = hit.point;

//                    Vector3Int gridPosition = new Vector3Int(
//                        (int)hitPoint.x,
//                        (int)(hitPoint.y),
//                        (int)(hitPoint.z)
//                    );
//                    if (mouseObject != null)
//                    {
//                        mouseObject.transform.position = gridPosition;
//                    }
//                    else
//                    {
//                        mouseObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//                        mouseObject.name = "ChunkPainter_MouseObject";
//                        mouseObject.transform.position = gridPosition;
//                        mouseObject.transform.localScale = Vector3.one * 0.5f;
//                        mouseObject.GetComponent<Collider>().enabled = false;
//                        mouseObject.hideFlags = HideFlags.HideInHierarchy;
//                    }

//                }
//            }
//        }

//        private void CreateSketchpad()
//        {
//            // 清理旧画板
//            if (sketchpad != null)
//            {
//                DestroyImmediate(sketchpad);
//            }

//            // 创建新画板
//            sketchpad = GameObject.CreatePrimitive(PrimitiveType.Plane);
//            sketchpad.name = "ChunkPainter_Sketchpad";
//            sketchpad.transform.position = Vector3.zero;

//            // 配置渲染组件
//            var renderer = sketchpad.GetComponent<Renderer>();
//            renderer.sharedMaterial = sketchpadMaterial != null
//                ? sketchpadMaterial
//                : CreateDefaultMaterial();

//            // 配置碰撞体
//            var collider = sketchpad.GetComponent<Collider>();
//            if (collider != null) collider.enabled = true;

//            // 初始定位
//            UpdateSketchpadOrientation();
//            MoveSketchpadToNewHeight(0);

//            // 隐藏对象
//            sketchpad.hideFlags = HideFlags.HideInHierarchy;
//        }

//        private Material CreateDefaultMaterial()
//        {
//            var mat = new Material(Shader.Find("Standard"));
//            mat.color = new Color(0.8f, 0.8f, 0.8f, 0.3f);
//            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
//            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
//            mat.EnableKeyword("_ALPHABLEND_ON");
//            mat.renderQueue = 3000;
//            return mat;
//        }

//        private void MoveSketchpadToNewHeight(int direction)
//        {
//            if (sketchpad == null) return;

//            Vector3 newPos = sketchpad.transform.position;
//            switch (AxisTypeProperty)
//            {
//                case AxisType.X:
//                    newPos.x += direction;
//                    break;
//                case AxisType.Y:
//                    newPos.y += direction;
//                    break;
//                case AxisType.Z:
//                    newPos.z += direction;
//                    break;
//            }
//            sketchpad.transform.position = newPos;
//        }

//        private void UpdateSketchpadOrientation()
//        {
//            if (sketchpad == null) return;

//            switch (AxisTypeProperty)
//            {
//                case AxisType.X:
//                    sketchpad.transform.rotation = Quaternion.Euler(0, 0, 90);
//                    break;
//                case AxisType.Y:
//                    sketchpad.transform.rotation = Quaternion.identity;
//                    break;
//                case AxisType.Z:
//                    sketchpad.transform.rotation = Quaternion.Euler(90, 0, 0);
//                    break;
//            }
//        }

//        private void DrawChunk()
//        {
//            ChunkState state = new ChunkState(pigment.AllChunkPartSO);

//            Debug.Log("Chunk Position: " + selectedChunkPos);
//            AddNewState(selectedChunkPos, state);

//        }
//        private void EraseChunk()
//        {
//        }
//        private void ShowEditUI(Vector3Int gridPosition)
//        {
//            _showUI = true;
//            _uiWorldPosition = gridPosition + Vector3.one * 0.5f;

//            // 加载现有数据或创建新数据

//        }

//        private void DrawSceneUIWindow()
//        {
//            // 坐标转换
//            Vector3 screenPos = HandleUtility.WorldToGUIPoint(_uiWorldPosition);
//            Rect uiRect = new Rect(
//                screenPos.x + _uiOffset,
//                screenPos.y + _uiOffset,
//                _uiSize.x,
//                _uiSize.y
//            );

//            // 应用自定义皮肤
//            GUI.skin = _uiSkin ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);

//            GUILayout.Window(0, uiRect, DrawUIContents, "区块属性");
//        }

//        private void DrawUIContents(int windowID)
//        {
//            GUILayout.Space(5);
//            EditorGUI.BeginChangeCheck();

//            // 使用滚动视图防止图标过多溢出
//            _iconScrollPos = GUILayout.BeginScrollView(_iconScrollPos, GUILayout.Height(200));

//            // 网格布局参数
//            const int columns = 4;
//            const float iconSize = 50f;
//            const float padding = 5f;
//            int iconCount = 0;
//            try { iconCount = _chunkStates[selectedChunkPos].possibleChunks.Count; }
//            catch
//            {
//                GUILayout.EndScrollView();
//                return;
//            }


//            int rows = Mathf.CeilToInt(iconCount / (float)columns);

//            // 自动计算网格布局
//            for (int row = 0; row < rows; row++)
//            {
//                GUILayout.BeginHorizontal();
//                for (int col = 0; col < columns; col++)
//                {
//                    int index = row * columns + col;
//                    if (index >= iconCount) break;

//                    var chunkData = _chunkStates[selectedChunkPos].possibleChunks.ElementAt(index);
//                    GameObject prefab = chunkData.GetPrefab().Item1;

//                    // 获取或生成图标
//                    if (!_iconCache.TryGetValue(prefab, out Texture2D icon))
//                    {
//                        icon = AssetPreview.GetAssetPreview(prefab) ??
//                              AssetPreview.GetMiniThumbnail(prefab) ??
//                              EditorGUIUtility.FindTexture("GameObject Icon");
//                        _iconCache[prefab] = icon;
//                    }

//                    // 绘制图标按钮
//                    Rect rect = GUILayoutUtility.GetRect(iconSize, iconSize);
//                    bool isSelected = index == _selectedIndex;

//                    // 绘制背景框
//                    if (Event.current.type == EventType.Repaint)
//                    {
//                        EditorStyles.helpBox.Draw(rect, GUIContent.none, false, false, isSelected, false);
//                    }

//                    // 绘制图标
//                    if (icon != null)
//                    {
//                        GUI.DrawTexture(rect, icon, ScaleMode.ScaleToFit);
//                    }

//                    // 处理点击事件
//                    if (Event.current.type == EventType.MouseDown &&
//                        Event.current.button == 0 &&
//                        rect.Contains(Event.current.mousePosition))
//                    {
//                        _selectedIndex = index;
//                        Event.current.Use();
//                        OnChunkSelected(chunkData);
//                    }
//                }
//                GUILayout.EndHorizontal();
//            }

//            GUILayout.EndScrollView();

//            // 显示选中信息
//            if (_selectedIndex != -1)
//            {
//                var selectedChunk = _chunkStates[selectedChunkPos].possibleChunks.ElementAt(_selectedIndex);
//                GUILayout.Label($"当前选择: {selectedChunk.name}", EditorStyles.boldLabel);
//            }

//            if (EditorGUI.EndChangeCheck())
//            {
//                PreviewChanges();
//            }
//        }

//        private void OnChunkSelected(ChunkPartSO chunkPartSO)
//        {
//            // 执行选择后的操作
//            Debug.Log($"选中预制体: {chunkPartSO.name}");

//            // 示例：实例化预制体
//            if (chunkPartSO.GetPrefab() is (GameObject prefab, _))
//            {
//                GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
//                instance.transform.position = selectedChunkPos;
//            }

//            // 关闭UI窗口
//            CloseUI();
//        }

//        private void ClearIconCache()
//        {
//            _iconCache.Clear();
//            Resources.UnloadUnusedAssets();
//        }
//        private void SaveDataAndClose()
//        {
//            Undo.RecordObject(pigment, "Edit Chunk Properties");
//            //pigment.SetChunkData(_uiWorldPosition, _currentChunkData);
//            EditorUtility.SetDirty(pigment);
//            CloseUI();
//        }

//        private void PreviewChanges()
//        {

//        }

//        private void CloseUI()
//        {
//            _showUI = false;

//            SceneView.RepaintAll();
//        }
//        #endregion

//        #region 生命周期
//        private void OnEnable()
//        {
//            SceneView.duringSceneGui -= OnSceneGUI;
//            SceneView.duringSceneGui += OnSceneGUI;
//        }

//        private void OnDestroy()
//        {
//            if (sketchpad != null)
//            {
//                DestroyImmediate(sketchpad);
//            }
//            SceneView.duringSceneGui -= OnSceneGUI;
//            if (mouseObject != null)
//            {
//                DestroyImmediate(mouseObject);
//            }
//        }
//        #endregion
//    }
//}