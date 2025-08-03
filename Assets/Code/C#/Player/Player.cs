
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ISetHealthUI;
/// <summary>
/// player类
/// </summary>
public class Player : MonoBehaviour, IHurt, ISetHealthUI, IReactionsUI, ISufferBuff, IHeal , IAbleFaceToCamera
{// 定义输入类型和优先级

    public CharacterDataSO OccupationData;
    //public AdditionalValueSO GeneAdditionalValueData;                   //基因额外数值

    [SerializeField] private GameObject root_equipments;
    [SerializeField] private GameObject root_Model;
    [SerializeField] private GameObject root_Colli;
    [SerializeField] private GameObject root_Hands;



    [SerializeField] private InventorySO inventory;
    [SerializeField] private int handInventorySize;
    [SerializeField] private int handInventoryIndex;
    [SerializeField] private InventorySO equipmentsInventory;           //基因栏
    [SerializeField] private int equipmentNum = 6;
    [SerializeField] private LayerMask groundLayer;

    public GameObject lightBall;
    public int lightBallCountCD = 8;
    public int lightBallCountMax = 3;
    public float lightBallForce = 10;

    public event EventHandler OnHandleItemChanged;
    public event EventHandler<int> OnResourceChanged;
    public event EventHandler OnHurt;
    public event EventHandler<OnProgressChangedEventArgs> OnHealthUIChanged;
    public event EventHandler<IReactionsUI.OnBuffChangedEventArgs> OnBuffChanged;
    public event EventHandler OnDeath;

    public event EventHandler<OnTakeDamageEventArgs> OnTakeDamage;
    public class OnTakeDamageEventArgs : EventArgs
    {
        public Transform target;
        public Transform source;
    }

    [SerializeField] private Vector3Int mousePosOffset = new Vector3Int(0, 2, 0);
    public Vector3Int MousePos
    {
        get
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, 20, groundLayer))
            {
                return new Vector3Int((int)hit.point.x, (int)hit.point.y, (int)hit.point.z) + mousePosOffset;
            }
            else
            {
                return new Vector3Int(0, 0, 0) + mousePosOffset;
            }
        }
    }

    private ReactionsBuff reactionsBuff = new ReactionsBuff(ReactionsBuff.DamageEnum.normal, 0);

    private float injuryIntervalTimer = 0;
    private List<int> buffList = new List<int>();

    public Rigidbody rb;

    private int health;
    private int magic;
    private int resource;

    private int currentMouseDown = 0;
    private IAble2Right currentRight;
    private IAble2Left currentLeft;

    private Vector3Int lastpos;
    private Vector3Int currentpos;


    protected void Start()
    {
        TickManager.Instance.OnTick_1 += HealWithTime;
        GameInputManager.Instance.OnSwitchItem += GameInputManager_SwitchItem;
        GameInputManager.Instance.OnLeftDown += GameInputManager_OnLeftDown;
        GameInputManager.Instance.OnRightDown += GameInputManager_OnRightDown;
        GameInputManager.Instance.OnMouseUp += GameInputManager_OnMouseUp;
        Debug.Log(SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == "Main")
        {
            GetComponent<LightSource>().enabled = true;
        }
    }


    protected void Update()
    {
        InjuryIntervalTimer();
        PressMouse();
        HandheldInEffect();
        UpdateGeneEffect();
    }

    protected void FixedUpdate()
    {
        Movement();
        UpdatePos();
    }

    private void HandheldInEffect()
    {
        foreach (Transform child in root_Hands.transform)
        {
            if (child.TryGetComponent<IHandheldInEffect>(out IHandheldInEffect handheldInEffect))
            {
                handheldInEffect.HandheldInEffect();
            }
        }
    }

    private void PressMouse()
    {
        if (currentMouseDown == 1)
        {
            currentRight?.OnRight();
        }
        if (currentMouseDown == -1)
        {
            currentLeft?.OnLeft();
        }
    }

    private void GameInputManager_OnMouseUp(object sender, EventArgs e)
    {
        currentMouseDown = 0;
    }

    private void GameInputManager_OnRightDown(object sender, EventArgs e)
    {
        currentMouseDown = 1;
        foreach (Transform child in root_Hands.transform)
        {
            if (child.TryGetComponent<IAble2Right>(out IAble2Right able2Right))
            {
                currentRight = able2Right;
            }

        }
    }

    private void GameInputManager_OnLeftDown(object sender, EventArgs e)
    {
        currentMouseDown = -1;
        foreach (Transform child in root_Hands.transform)
        {
            if (child.TryGetComponent<IAble2Left>(out IAble2Left able2Left))
            {
                currentLeft = able2Left;
            }
        }
    }

    private void GameInputManager_SwitchItem(object sender, Vector2 e)
    {
        if (root_Hands.transform.childCount > 0)
        {
            Destroy(root_Hands.transform.GetChild(0).gameObject);
            currentRight = null;
        }
        if (SwitchItem(e))
        {
            ItemManager.ItemEnum itemEnum = inventory.GetItemType(handInventoryIndex);
            if (itemEnum != ItemManager.ItemEnum.Empty
                && itemEnum != ItemManager.ItemEnum.Ammo
                && itemEnum != ItemManager.ItemEnum.Equipment
                && itemEnum != ItemManager.ItemEnum.Magic
                )
            {

                GameObject item = Instantiate(inventory.GetGameObject(handInventoryIndex), root_Hands.transform);
                item.SetActive(true);
            }

        }
    }

    public void SetOccupationDataSO(CharacterDataSO occupationData)
    {
        OccupationData.Copy(occupationData);
        foreach (Transform child in root_Model.transform)
        {
            Destroy(child.gameObject);
        }
        Init();
    }

    private void Movement()
    {
        Vector3 movement = GameInputManager.Instance.GetMovement();
       // HandleInput()




        if (movement == Vector3.zero)
        {
            rb.velocity *= 0.5f;
            return;
        }

        rb.velocity = movement.normalized * OccupationData.MoveSpeedMultiplier;
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, OccupationData.MoveSpeedMax);


    }

    private void Init()
    {
        Instantiate(OccupationData.Model, root_Model.transform);
        rb = GetComponent<Rigidbody>();
        SetEquipment();
        health = OccupationData.HealthMax;
        magic = OccupationData.MagicMax;
        resource = OccupationData.Resource;
    }

    private void HealWithTime(object sender, EventArgs e)
    {
        if (health < OccupationData.HealthMax)
        {
            Heal(OccupationData.Heal);
        }
        if (magic < OccupationData.MagicMax)
        {
            magic *= OccupationData.Magicheal;
            magic = Mathf.Clamp(magic, 0, OccupationData.MagicMax);
            HpMpUIChanged();
        }
    }

    //添加回血接口方法方便外部调用
    public void Heal(int HealValue)
    {
        health += HealValue;
        health = Mathf.Clamp(health, 0, OccupationData.HealthMax);
        HpMpUIChanged();
    }

    private void InjuryIntervalTimer()
    {
        if (injuryIntervalTimer >= 0)
        {
            injuryIntervalTimer -= Time.deltaTime;
        }
    }

    private GameObject SwitchItem(Vector2 e)
    {

        if (e.y > 0)
        {
            handInventoryIndex++;
            if (handInventoryIndex >= handInventorySize)
                handInventoryIndex = 0;
        }
        if (e.y < 0)
        {
            handInventoryIndex--;
            if (handInventoryIndex < 0)
                handInventoryIndex = handInventorySize - 1;
        }
        OnHandleItemChanged?.Invoke(this, EventArgs.Empty);
        return inventory.GetGameObject(handInventoryIndex);
    }

    private GameInputManager.InputCommand currentInput;

    // 处理输入并根据优先级更新
    public void HandleInput(GameInputManager.InputCommand newInput)
    {
        if (newInput.Priority >= currentInput.Priority)
        {
            currentInput = newInput;
            // 处理方向
            if (currentInput.Direction.x != 0)
            {
                SetFaceDir(currentInput.Direction.x);
            }
            // 这里可以根据 Type 做不同的处理
        }
    }

    // 协程用于平滑地切换角色朝向
    public IEnumerator FlipModel(float from, float to, float duration = 0.1f)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float scaleX = Mathf.Lerp(from, to, t);
            root_Model.transform.localScale = new Vector3(scaleX, 1, 1);
            elapsed += Time.deltaTime;
            yield return null;
        }
        root_Model.transform.localScale = new Vector3(to, 1, 1);
    }

    // 设置角色面朝方向
    public void SetFaceDir(float dir)
    {
        float currentScaleX = root_Model.transform.localScale.x;
        float targetScaleX = dir > 0 ? 1 : -1;
        if (Mathf.Approximately(currentScaleX, targetScaleX))
            return;
        StopAllCoroutines();
        StartCoroutine(FlipModel(currentScaleX, targetScaleX));
    }

    public void OnTakeDamageEvent(Transform transform)
    {
        OnTakeDamage?.Invoke(this, new OnTakeDamageEventArgs
        {
            target = transform,
            source = transform
        });
    }

    public bool ChangeMagicAmount(int amount)
    {
        magic += amount;
        OnHealthUIChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            hp = health,
            hpMax = OccupationData.HealthMax,
            mp = magic,
            mpMax = OccupationData.MagicMax
        });
        return true;
    }

    public bool MagicAmountEnough(int amount)
    {
        if (magic >= amount)
        {
            return true;
        }
        return false;
    }

    public bool ChangeResourceAmount(int amount)
    {
        resource += amount;
        OnResourceChanged?.Invoke(this, resource);
        return true;
    }

    public bool ChangeResourceAmount(ItemSO item)
    {
        if (item.itemType != ItemManager.ItemEnum.Ammo)
        {
            return false;
        }
        resource += item.complexity;
        OnResourceChanged?.Invoke(this, resource);
        return true;
    }

    public bool ResourceAmountEnough(int amount)
    {
        if (resource >= amount)
        {
            return true;
        }
        return false;
    }

    public int GetHandInventorySizeSize()
    {
        return handInventorySize;
    }
    public int GetInventoryIndex()
    {
        return handInventoryIndex;
    }
    public InventorySO GetInventory()
    {
        return inventory;
    }

    public void SetEquipment()
    {
        foreach (Transform child in root_equipments.transform)
        {
            if (child.TryGetComponent<IGeneEffect>(out IGeneEffect iGeneEffect))
            {
                iGeneEffect.StopEffect();
            }
        }
        List<GameObject> oriGameObjects = new List<GameObject>();

        for (int i = 0; i < root_equipments.transform.childCount; i++)
        {
            oriGameObjects.Add(root_equipments.transform.GetChild(i).gameObject);
        }

        List<GameObject> targetGameObjects = new List<GameObject>();

        for (int i = 0; i < equipmentNum; i++)
        {
            if (!equipmentsInventory.IsNull(i) && equipmentsInventory.GetItemType(i) == ItemManager.ItemEnum.Equipment)
            {
                targetGameObjects.Add(equipmentsInventory.GetGameObject(i));
            }
        }

        foreach (GameObject targetObj in targetGameObjects)
        {
            if (Contains(oriGameObjects, targetObj, 0))
            {
                continue;
            }
            else
            {
                Instantiate(targetObj, root_equipments.transform);

            }


        }
        foreach (GameObject oriObj in oriGameObjects)
        {
            if (Contains(targetGameObjects, oriObj, 1))
            {

                continue;
            }
            else
            {
                Destroy(oriObj);

            }
        }

        foreach (Transform child in root_equipments.transform)
        {
            if (child.TryGetComponent<IGeneEffect>(out IGeneEffect iGeneEffect))
            {
                iGeneEffect.StartEffect();
            }
        }


        static bool Contains(List<GameObject> oriGameObjects, GameObject targetObj, int whoHasClone)
        {
            if (whoHasClone == 0)
            {
                foreach (GameObject oriObj in oriGameObjects)
                {


                    if (oriObj.name == targetObj.name + "(Clone)")
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                foreach (GameObject oriObj in oriGameObjects)
                {


                    if (oriObj.name + "(Clone)" == targetObj.name)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }

    public void UpdateGeneEffect()
    {
        foreach (Transform child in root_equipments.transform)
        {
            if (child.TryGetComponent<IGeneEffect>(out IGeneEffect iGeneEffect))
            {
                iGeneEffect.Effect();
            }
        }
    }

    public ReactionsBuff Hurt(ReactionsBuff reactionsBuffTakeDamage, GameObject source = null)
    {
        if (injuryIntervalTimer >= 0)
            return reactionsBuff;
        OnHurt?.Invoke(this, EventArgs.Empty);
        if (source != null)
        {
            Vector2 hitBackDir = transform.position - source.transform.position;
            hitBackDir += Vector2.up;
            rb.AddForce(hitBackDir.normalized * reactionsBuffTakeDamage.GetHitback() * (1 - OccupationData.HitBackRsistance), ForceMode.Impulse);
        }
        if (reactionsBuffTakeDamage.GetDamageEnum() == ReactionsBuff.DamageEnum.honkai)
        {

            health -= (int)(reactionsBuffTakeDamage.GetDamage() * (1 - OccupationData.MagicResistance));

            UniversalEffectsManager.Instance.GenerateDamageNum(transform, reactionsBuffTakeDamage);


        }
        else
        {
            reactionsBuff.AddBuff(reactionsBuffTakeDamage);
            reactionsBuff.SetTransform(transform);
            health -= Mathf.Clamp(reactionsBuff.GetDamage() - OccupationData.Defense, 1, 99999);

            ReactionsBuff tempReactionsBuff = new ReactionsBuff(reactionsBuffTakeDamage.GetDamageEnum(), reactionsBuff.GetDamage() - OccupationData.Defense);
            UniversalEffectsManager.Instance.GenerateDamageNum(transform, tempReactionsBuff);//其他伤害反应就用反应后伤害数值
            reactionsBuff.SetDamage(0);
        }

        injuryIntervalTimer = OccupationData.InvincibleTime;
        HpMpUIChanged();


        OnBuffChanged?.Invoke(this, new IReactionsUI.OnBuffChangedEventArgs { buff = reactionsBuff });

        if (health <= 0)
        {
            gameObject.SetActive(false);
            //PlayerStatusManager.Instance.Rebirth();
            OnDeath?.Invoke(this, EventArgs.Empty);
            Debug.Log("Player Dead");
        }
        return reactionsBuff;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return OccupationData.HealthMax;
    }

    private void HpMpUIChanged()
    {
        OnHealthUIChanged?.Invoke(this, new OnProgressChangedEventArgs
        {
            hp = health,
            hpMax = OccupationData.HealthMax,
            mp = magic,
            mpMax = OccupationData.MagicMax
        });
    }

    public List<int> GetBuffStructs()
    {
        return buffList;
    }

    public Transform GetBuffParent()
    {
        Debug.Log("去canvas中找");
        return BuffManager.Instance.buffParent;
    }

    private void UpdatePos()
    {
        currentpos = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        if (currentpos != lastpos)
        {
            lastpos = currentpos;
            Vector2Int playerPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
            if (PathManager.Instance != null)
                PathManager.Instance.AddTargetPos(playerPos);
        }
    }

    public void FaceToCamera(GameObject TargetCamera , Vector3 Adjustment)
    {
        //面向镜头功能
        //TODO:可能需要略微修改角度，正对情况感官略微一般
        Vector3 cameraEuler = TargetCamera.transform.eulerAngles;
        Vector3 adjustedEuler = cameraEuler + Adjustment;
        transform.eulerAngles = adjustedEuler;
    }

    private void OnDestroy()
    {
        TickManager.Instance.OnTick_1 -= HealWithTime;
        GameInputManager.Instance.OnSwitchItem -= GameInputManager_SwitchItem;
        GameInputManager.Instance.OnLeftDown -= GameInputManager_OnLeftDown;
        GameInputManager.Instance.OnRightDown -= GameInputManager_OnRightDown;
        GameInputManager.Instance.OnMouseUp -= GameInputManager_OnMouseUp;
        OccupationData.bonusList.Clear();
        Debug.LogError("Player OnDestroy" + Time.time);
    }
}
