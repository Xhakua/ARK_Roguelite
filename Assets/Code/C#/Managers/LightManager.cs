using UnityEngine;
/// <summary>
/// 光照管理器
/// </summary>
public class LightManager : MonoBehaviour
{

    public static LightManager Instance { get; private set; }
    public bool useGPU = false;
    [SerializeField] ComputeShader computeShader;
    private Texture2D lightTextureCPU;
    private RenderTexture lightTextureGPU;
    [SerializeField] private Material lightShader;

    [SerializeField] FilterMode filterMode = FilterMode.Point;
    private int _kernel;
    public int[,] tempLightData;
    public Color darkColor;
    [SerializeField] Vector2Int shadowSize;
    [SerializeField] Vector2Int updateSize;
    [SerializeField] Vector3 offset;
    //空气衰减系数
    [SerializeField] private float airAttenuation;
    //方块衰减系数
    [SerializeField] private float blockAttenuation;
    public readonly int maxlight = 25;
    //日光
    public int sunlight;
    private Vector2Int worldSize;
    private Vector2Int leftDownAnchors;
    private Vector2Int rightUpAnchors;
    private Vector3Int lastPlayerPosition = new Vector3Int(0, 0, 0);
    uint threadX = 32;
    uint threadY = 32;
    uint threadZ = 32;




    ComputeBuffer bufferLightData;
    ComputeBuffer bufferWorldDataFloor;
    ComputeBuffer bufferWorldDataWall;
    ComputeBuffer bufferWorldDataLight;





    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }



    public void Init()
    {
        worldSize = new Vector2Int(MapManager.Instance.GetWorldSize().x, MapManager.Instance.GetWorldSize().z);
        tempLightData = new int[worldSize.x, worldSize.y];


        if (!useGPU)
        {
            _kernel = computeShader.FindKernel("InitLight");
            lightTextureCPU = new Texture2D(shadowSize.x, shadowSize.y);
            transform.localScale = new Vector3(shadowSize.x, shadowSize.y, 1) * 1.5f;
            transform.localPosition = new Vector3Int((int)PlayerManager.Instance.GetPlayer().transform.position.x, (int)PlayerManager.Instance.GetPlayer().transform.position.y, (int)PlayerManager.Instance.GetPlayer().transform.position.z);
            lightTextureCPU.filterMode = filterMode;
            lightShader.SetTexture("_shadowTexture", lightTextureCPU);
            //InitLightCPU();

            bufferLightData = new ComputeBuffer(tempLightData.Length, sizeof(int));
            bufferLightData.SetData(tempLightData);
            computeShader.SetBuffer(_kernel, "_lightData", bufferLightData);

            bufferWorldDataFloor = new ComputeBuffer(MapManager.Instance.GetWorldData(MapManager.TileLayer.Floor).Length, sizeof(int));
            bufferWorldDataFloor.SetData(MapManager.Instance.GetWorldData(MapManager.TileLayer.Floor));
            computeShader.SetBuffer(_kernel, "_worldData0", bufferWorldDataFloor);


            bufferWorldDataWall = new ComputeBuffer(MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall).Length, sizeof(int));
            bufferWorldDataWall.SetData(MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall));
            computeShader.SetBuffer(_kernel, "_worldData2", bufferWorldDataWall);


            bufferWorldDataLight = new ComputeBuffer(MapManager.Instance.lightData.Length, sizeof(int));
            bufferWorldDataLight.SetData(MapManager.Instance.lightData);
            computeShader.SetBuffer(_kernel, "_worldData3", bufferWorldDataLight);

            computeShader.SetVector("_worldSize", new Vector2(worldSize.x, worldSize.y));
            computeShader.SetInt("_updateSizeX", updateSize.x);
            computeShader.SetInt("_updateSizeY", updateSize.y);
            computeShader.SetInt("_maxlight", maxlight);
            computeShader.SetInt("_sunlight", sunlight);

            //Vector2 vector2 = new Vector2(Player.Instance.transform.position.x, Player.Instance.transform.position.y);
            //computeShader.SetVector("_playerPosition", vector2);
            computeShader.GetKernelThreadGroupSizes(_kernel, out threadX, out threadY, out threadZ);
            computeShader.Dispatch(_kernel, updateSize.x / (int)threadX, updateSize.y / (int)threadY, 1);
            bufferLightData.GetData(tempLightData);
            bufferLightData.Release();
            bufferWorldDataFloor.Release();
            bufferWorldDataWall.Release();
            bufferWorldDataLight.Release();
        }
        else
        {

            _kernel = computeShader.FindKernel("ComputeLight");
            lightTextureGPU = new RenderTexture(shadowSize.x, shadowSize.y, 0);
            lightTextureGPU.enableRandomWrite = true;
            lightTextureGPU.Create();
            lightTextureGPU.filterMode = filterMode;
            transform.localScale = new Vector3(shadowSize.x, shadowSize.y, 1);
            transform.localPosition = new Vector3(shadowSize.x / 2f, shadowSize.y / 2f, 0);
            lightShader.SetTexture("_shadowTexture", lightTextureGPU);
            computeShader.SetTexture(_kernel, "Result", lightTextureGPU);

            bufferLightData = new ComputeBuffer(tempLightData.Length, sizeof(int));
            bufferLightData.SetData(tempLightData);
            computeShader.SetBuffer(_kernel, "_lightData", bufferLightData);


            bufferWorldDataFloor = new ComputeBuffer(MapManager.Instance.GetWorldData(MapManager.TileLayer.Floor).Length, sizeof(int));
            bufferWorldDataFloor.SetData(MapManager.Instance.GetWorldData(MapManager.TileLayer.Floor));
            computeShader.SetBuffer(_kernel, "_worldData0", bufferWorldDataFloor);


            bufferWorldDataWall = new ComputeBuffer(MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall).Length, sizeof(int));
            bufferWorldDataWall.SetData(MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall));
            computeShader.SetBuffer(_kernel, "_worldData2", bufferWorldDataWall);


            bufferWorldDataLight = new ComputeBuffer(MapManager.Instance.lightData.Length, sizeof(int));
            bufferWorldDataLight.SetData(MapManager.Instance.lightData);
            computeShader.SetBuffer(_kernel, "_worldData3", bufferWorldDataLight);

            computeShader.SetVector("_worldSize", new Vector2(worldSize.x, worldSize.y));
            computeShader.SetInt("_updateSizeX", updateSize.x);
            computeShader.SetInt("_updateSizeY", updateSize.y);
            computeShader.SetInt("_maxlight", maxlight);
            computeShader.SetInt("_sunlight", sunlight);
            computeShader.SetVector("_darkColor", darkColor);
            computeShader.SetFloat("_airAttenuation", airAttenuation);
            computeShader.SetFloat("_blockAttenuation", blockAttenuation);


            Vector2 vector2 = new Vector2((int)PlayerManager.Instance.GetPlayer().transform.position.x, (int)PlayerManager.Instance.GetPlayer().transform.position.y);
            computeShader.SetVector("_playerPosition", vector2);
            computeShader.GetKernelThreadGroupSizes(_kernel, out threadX, out threadY, out threadZ);
            computeShader.Dispatch(_kernel, updateSize.x / (int)threadX, updateSize.y / (int)threadY, 1);


            computeShader.Dispatch(0, worldSize.x / (int)threadX, worldSize.y / (int)threadY, 1);
        }

    }



    public void UpdateLightCPU(Vector3 position)
    {

        transform.localPosition = new Vector3Int((int)position.x, (int)position.y, (int)position.z) + offset;

        leftDownAnchors.x = (int)position.x - updateSize.x / 2;
        leftDownAnchors.y = (int)position.z - updateSize.y / 2;
        rightUpAnchors.x = (int)position.x + updateSize.x / 2;
        rightUpAnchors.y = (int)position.z + updateSize.y / 2;
        leftDownAnchors.x = Mathf.Clamp(leftDownAnchors.x, 0, worldSize.x - 1);
        leftDownAnchors.y = Mathf.Clamp(leftDownAnchors.y, 0, worldSize.y - 1);
        rightUpAnchors.x = Mathf.Clamp(rightUpAnchors.x, 0, worldSize.x - 1);
        rightUpAnchors.y = Mathf.Clamp(rightUpAnchors.y, 0, worldSize.y - 1);



        for (int trun = 0; trun < 1; trun++)
        {
            for (int j = leftDownAnchors.y; j < rightUpAnchors.y; j++)
            {
                for (int i = leftDownAnchors.x; i < rightUpAnchors.x; i++)
                {
                    float light = MapManager.Instance.GetWorldData(MapManager.TileLayer.Light)[i, j];
                    //float light = MapManager.Instance.tempLightData[i, j];
                    if (light > 0)
                    {
                        light = Mathf.Clamp(light, 0, maxlight);
                        tempLightData[i, j] = (int)light;
                    }

                    int xL = Mathf.Clamp(i - 1, 0, worldSize.x - 1);
                    int yD = Mathf.Clamp(j - 1, 0, worldSize.y - 1);
                    int xR = Mathf.Clamp(i + 1, 0, worldSize.x - 1);
                    int yU = Mathf.Clamp(j + 1, 0, worldSize.y - 1);
                    light = Mathf.Max(tempLightData[xR, j], tempLightData[i, yU], tempLightData[xL, j], tempLightData[i, yD], tempLightData[i, j], sunlight);
                    if (MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall)[i, j] != 0)
                    {
                        light *= blockAttenuation;
                    }
                    else// if (MapManager.Instance.GetWorldData(new Vector3Int(i, j, 0)) != -1)
                    {
                        light *= airAttenuation;
                    }


                    light = Mathf.Clamp(light, 0, maxlight);
                    tempLightData[i, j] = (int)light;
                }
            }

            for (int j = rightUpAnchors.y - 1; j >= leftDownAnchors.y; j--)
            {
                for (int i = rightUpAnchors.x - 1; i >= leftDownAnchors.x; i--)
                {
                    float light = MapManager.Instance.GetWorldData(MapManager.TileLayer.Light)[i, j];
                    //float light = MapManager.Instance.tempLightData[i, j];
                    if (light > 0)
                    {
                        light = Mathf.Clamp(light, 0, maxlight);
                        tempLightData[i, j] = (int)light;
                    }

                    int xL = Mathf.Clamp(i - 1, 0, worldSize.x - 1);
                    int yD = Mathf.Clamp(j - 1, 0, worldSize.y - 1);
                    int xR = Mathf.Clamp(i + 1, 0, worldSize.x - 1);
                    int yU = Mathf.Clamp(j + 1, 0, worldSize.y - 1);
                    light = Mathf.Max(tempLightData[xR, j], tempLightData[i, yU], tempLightData[xL, j], tempLightData[i, yD], tempLightData[i, j], sunlight);
                    if (MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall)[i, j] != 0)
                    {
                        light *= blockAttenuation;
                    }
                    else// if (MapManager.Instance.GetWorldData(new Vector3Int(i, j, 0)) != -1)
                    {
                        light *= airAttenuation;
                    }


                    light = Mathf.Clamp(light, 0, maxlight);
                    tempLightData[i, j] = (int)light;
                }
            }
        }

        for (int j = leftDownAnchors.y; j < rightUpAnchors.y; j++)
        {
            for (int i = leftDownAnchors.x; i < rightUpAnchors.x; i++)
            {
                darkColor.a = 1f - tempLightData[i, j] / (float)maxlight;
                lightTextureCPU.SetPixel(i - leftDownAnchors.x, j - leftDownAnchors.y, darkColor);
                tempLightData[i, j] = 0;
                //tempLightData[i, j] = (int)(tempLightData[i, j] * 0.5f);

            }
        }

        lightTextureCPU.Apply();
    }
    //private void Start()
    //{
    //    TickManager.Instance.OnTick_30 += UpdateLight;
    //}

    //private void UpdateLight(object sender, EventArgs e)
    //{
    //    if (useGPU)
    //        UpdateLight(Player.Instance.transform.position);
    //    else
    //        UpdateLight(Player.Instance.transform.position);
    //}

    //private void LateUpdate()
    //{


    //}
    private void FixedUpdate()
    {
        if (useGPU)
            UpdateLight(PlayerManager.Instance.GetPlayer().transform.position);
        else
            UpdateLight(PlayerManager.Instance.GetPlayer().transform.position);

        //sunlight = (int)(Time.time % 10);
    }


    public void UpdateLight(Vector3 position)
    {
        if (!useGPU)
        {
            UpdateLightCPU(position);
        }
        else
        {
            transform.localPosition = new Vector3Int((int)position.x, (int)position.y, (int)transform.position.z);

            computeShader.SetVector("_playerPosition", new Vector2((int)position.x, (int)position.y));


            bufferWorldDataFloor.SetData(MapManager.Instance.GetWorldData(MapManager.TileLayer.Floor));
            computeShader.SetBuffer(_kernel, "_worldData0", bufferWorldDataFloor);


            bufferWorldDataWall.SetData(MapManager.Instance.GetWorldData(MapManager.TileLayer.Wall));
            computeShader.SetBuffer(_kernel, "_worldData2", bufferWorldDataWall);



            bufferWorldDataLight.SetData(MapManager.Instance.lightData);
            computeShader.SetBuffer(_kernel, "_worldData3", bufferWorldDataLight);

            computeShader.SetInt("_sunlight", sunlight);

            computeShader.Dispatch(0, worldSize.x / (int)threadX, worldSize.y / (int)threadY, 1);
            bufferLightData.GetData(tempLightData);
        }
    }

    public void UpdateComputeShader()
    {
        computeShader.Dispatch(0, worldSize.x / (int)threadX, worldSize.y / (int)threadY, 1);
    }


    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.F))
    //    {
    //        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        Vector3Int v3Int = new Vector3Int((int)mousePos.x, (int)mousePos.y, 0);

    //        //Debug.Log(lightTextureCPU.GetPixel(v3Int.x, v3Int.y));
    //        //tempLightData[v3Int.y, v3Int.x] = 15;
    //        Debug.Log(tempLightData[v3Int.x, v3Int.y]);
    //        // Debug.Log(MapManager.Instance.worldData[v3Int.y, v3Int.x, 3]);
    //        //bufferLightData.GetData(debug);

    //        //// 使用数据，例如打印到控制台
    //        //for (int i = 0; i < worldSize.x; i++)
    //        //{
    //        //    for (int j = 0; j < worldSize.y; j++)
    //        //    {
    //        //        Debug.Log(debug[i, j]);
    //        //    }
    //        //}
    //    }
    //}

    private void UpdateLightSize(Vector3 pos)
    {
        Vector2Int dynamicMapSize = MapDynamicLoadingManager.Instance.dynamicMapSize;
        Vector2Int playerChunk = new Vector2Int((int)pos.x / dynamicMapSize.x, (int)pos.y / dynamicMapSize.y);
        Vector2Int lastPlayerChunk = new Vector2Int((int)lastPlayerPosition.x / dynamicMapSize.x, (int)lastPlayerPosition.y / dynamicMapSize.y);
        if (playerChunk != lastPlayerChunk)
        {
            transform.localPosition = new Vector3(playerChunk.x * dynamicMapSize.x + dynamicMapSize.x / 2, playerChunk.y * dynamicMapSize.y + 0.5f * dynamicMapSize.y - 15, 0);
        }
    }

    private void OnDisable()
    {
        if (bufferLightData != null)
        {
            bufferLightData.Release();
        }
        if (bufferWorldDataFloor != null)
        {
            bufferWorldDataFloor.Release();
        }
        if (bufferWorldDataWall != null)
        {
            bufferWorldDataWall.Release();
        }
        if (bufferWorldDataLight != null)
        {
            bufferWorldDataLight.Release();
        }
    }
    public void SetLightData(Vector3Int pos, int lightIntensity)
    {
        pos.x = Mathf.Clamp(pos.x, 0, worldSize.x - 1);
        pos.y = Mathf.Clamp(pos.y, 0, worldSize.y - 1);
        if (lightIntensity > tempLightData[pos.x, pos.y])
            tempLightData[pos.x, pos.y] = lightIntensity;
    }

    public int GetLightData(Vector3Int pos)
    {
        pos.x = Mathf.Clamp(pos.x, 0, worldSize.x - 1);
        pos.y = Mathf.Clamp(pos.y, 0, worldSize.y - 1);

        return tempLightData[pos.x, pos.y];
    }


    public Vector2Int GetShadowSize()
    {
        return shadowSize;
    }


}
