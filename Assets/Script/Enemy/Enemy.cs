using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Actor
{
    #region Variables
    public bool StatusInit = false;
    //public Vector3 NavTRPos;
    //public Vector3 transformPos;
    //public Vector3 localPos;

    public int mobId = 0;
    public float skillPoint = 0f;
    public float skillChargeTime = 5f;
    public EEnemyType type;

    public float meleeAttackRange;
    public float arrowAttackRange;
    public float recoveryTime;
    public float aggroRadius;
    public float maxAggroRadius;

    // Melee
    public float wanderingTime;

    // Archer
    public float reloadTime;
    private float runawayResetTime = 3f;
    private float oldReloadTime;
    private int arrowPower;
    private bool bReload = false;
    private bool bRunaway = true;

    private bool bUpperHit = false;
    private bool bIsFalling = false;
    private bool bSkillReady = false;
    private Vector3 arrowOffset = Vector3.up * 0.5f;
    private Vector3 preWorldPos;
    private Transform navTR;
    private Transform playerTR;
    [SerializeField]
    private CapsuleCollider physicsCollider;
    [SerializeField]
    private CapsuleCollider triggerCollider;
    private Transform firePos;
    private GameObject arrow;
    private Animator animator;
    private NavMeshAgent navAgent;
    private NavMeshPath navPath;
    private BaseAI _AI;
    private Rigidbody rigidBody;
    [SerializeField]
    private ParticleSystem sparkEffect;
    [SerializeField]
    private SphereCollider meleeSkillCollider;
    private List<IState> listStates;
    private Dictionary<EEnemyState, IState> dicState;
    private StageManager stageManager;
    #endregion

    #region Property
    public List<IState> ListStates { get { return listStates; } }
    public Dictionary<EEnemyState, IState> DicState { get { return dicState; } }
    public Transform NavTR { get { return navTR; } set { navTR = value; } }
    public Transform PlayerTR { get { return playerTR; } set { playerTR = value; } }
    public CapsuleCollider PhysicsCollider { get { return physicsCollider; } }
    public CapsuleCollider TriggerCollider { get { return triggerCollider; } }
    public Transform FirePos { get { return firePos; } }
    public Animator Animator { get { return animator; } }
    public NavMeshAgent NavAgent { get { return navAgent; } }
    public NavMeshPath NavPath { get { return navPath; } set { navPath = value; } }
    public BaseAI AI { get { return _AI; } }
    public GameObject Arrow { get { return arrow; } }
    public Rigidbody RigidBody { get { return rigidBody; } set { rigidBody = value; } }

    public int ArrowPower { get { return arrowPower; } set { arrowPower = value; } }
    public bool BReload { get { return bReload; } set { bReload = value; } }
    public bool BRunaway { get { return bRunaway; } set { bRunaway = value; } }
    public bool BUpperHit { get { return bUpperHit; } set { bUpperHit = value; } }
    public bool BIsFalling { get { return bIsFalling; } set { bIsFalling = value; } }
    public bool BSkillReady { get { return bSkillReady; } set { bSkillReady = value; } }
    #endregion

    private void Start()
    {
        navTR = transform.parent.GetComponent<Transform>();
        playerTR = GameObject.FindWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        navAgent = GetComponentInParent<NavMeshAgent>();
        navPath = new NavMeshPath();
        rigidBody = GetComponent<Rigidbody>();
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();

        Init();

        _AI.Idle();
    }

    private void Update()
    {
        //NavTRPos = navTR.position;
        //transformPos = transform.position;
        //localPos = transform.localPosition;

        if (IsAlive == false)
            return;

        if (bSkillReady == false)
            skillPoint += Time.deltaTime * (100f / skillChargeTime);

        if (skillPoint > 100)
        {
            skillPoint = 100f;
            bSkillReady = true;
        }

        if (bRunaway == false && _AI.CurrentState is RunawayState == false)
        {
            runawayResetTime -= Time.deltaTime;

            if (runawayResetTime <= 0f)
            {
                runawayResetTime = 3f;
                bRunaway = true;
            }
        }

        _AI.UpdateAI();

        if (bReload)
            CalcReloadTime();
    }

    private void LateUpdate()
    {
        preWorldPos = transform.position;
    }

    #region Init Func
    public override void Init()
    {
        SetCollections();

        switch (type)
        {
            case EEnemyType.Enemy_Melee:
                {
                    _AI = new MeleeAI();

                    if (StatusInit)
                    {
                        Status st = new Status
                        {
                            hp = 1000,
                            mp = 100,
                            power = 30
                        };

                        base.StatusInit(st);
                    }
                }
                break;
            case EEnemyType.Enemy_Archer:
                {
                    _AI = new ArcherAI();

                    firePos = FindInChild("FirePos");

                    arrow = FindInChild("Elven Long Bow Arrow").gameObject;
                    arrow.SetActive(false);

                    oldReloadTime = reloadTime;

                    if (StatusInit)
                    {
                        Status st = new Status
                        {
                            hp = 800,
                            mp = 100,
                            power = 5
                        };

                        base.StatusInit(st);
                    }
                    arrowPower = 20;
                }
                break;
            case EEnemyType.Enemy_Boss:
                {

                }
                break;
            default:
                break;
        }

        _AI.Enemy = this;

        base.Init();
        nowPower = power;
        nowHp = hp;
        nowMp = mp;
    }

    private void SetCollections()
    {
        listStates = new List<IState>
        {
            new IdleState(),
            new MeleeAttackState(),
            new HitState(),
            new CriticalHitState(),
            new StunState(),
            new DieState(),
            new FollowState(),
            new WanderState(),
            new ArrowAttackState(),
            new UpperHitState(),
            new SkillState(),
            new RunawayState()
        };

        if (listStates.Count != (int)EEnemyState.MAX)
            Debug.LogError("State 갯수 불일치");

        dicState = new Dictionary<EEnemyState, IState>();
        for (int i = 0; i < (int)EEnemyState.MAX; i++)
        {
            dicState.Add((EEnemyState)i, listStates[i]);
        }

        ListAttackColliders = new List<Collider>();

        switch (type)
        {
            case EEnemyType.Enemy_Melee:
                ListAttackColliders.Add(FindInChild("AttackColliderRightArm").GetComponent<Collider>());
                break;
            case EEnemyType.Enemy_Archer:
                ListAttackColliders.Add(FindInChild("AttackColliderRightLeg").GetComponent<Collider>());
                break;
            case EEnemyType.Enemy_Boss:
                break;
            default:
                break;
        }

        foreach (Collider coll in ListAttackColliders)
        {
            coll.gameObject.tag = "Enemy";
        }
    }
    #endregion

    #region Override
    public override void onDamaged(int damage)
    {
        if (isAlive == false)
            return;

        Debug.Log(this + " onDamaged : " + damage);
        nowHp -= damage;

        if (nowHp <= 0)
        {
            nowHp = 0;
            onDead();
        }
        else
        {
            if (damage >= 300)
                _AI.CriticalHit();
            else
                _AI.Hit();
        }

        EnemyManager.Instance.lastHitMob = this;
        //UpdateMobInfo();
    }

    public override void onDead()
    {
        _AI.Die();
        isAlive = false;

        stageManager.EnemyCount--;

        Invoke("Kill", 1.5f);
    }

    public override void UpperHit(int _power)
    {
        Invoke("SwitchPhysicsCollider", 0.2f);

        Vector3 tmp = Vector3.up * _power;

        //rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        rigidBody.AddForce(tmp);

        ResetLocalPos();

        bUpperHit = true;
        _AI.UpperHit();
    }
    #endregion

    #region Private Func
    private void CalcReloadTime()
    {
        if (reloadTime > 0)
        {
            reloadTime -= Time.deltaTime;

            if (reloadTime <= 0)
            {
                bReload = false;
                reloadTime = oldReloadTime;
            }
        }
    }

    private void UpdateMobInfo()
    {
        if (isAlive)
        {
            EnemyManager.Instance.lastHit.id = mobId;
            EnemyManager.Instance.lastHit.hp = nowHp;
            EnemyManager.Instance.lastHit.maxHp = hp;
            EnemyManager.Instance.lastHit.skillPoint = skillPoint;
        }
        else
        {
            EnemyManager.Instance.lastHit.id = 0;
            EnemyManager.Instance.lastHit.hp = 0;
            EnemyManager.Instance.lastHit.maxHp = 0;
            EnemyManager.Instance.lastHit.skillPoint = 0f;
        }

        EnemyManager.Instance.UpdateMobInfo();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_AI.CurrentState is UpperHitState && collision.transform.tag == "Stage")
        {
            Debug.Log("착지");
            ((UpperHitState)_AI.CurrentState).BLand = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "Player")
        //{
        //    Vector3 tmp = new Vector3();
        //    tmp = Vector3.up * 500;

        //    gameObject.GetComponent<Rigidbody>().AddForce(tmp);
        //    Debug.Log("충돌");
        //}

        //Debug.Log(other);

        //if (other.tag == "Player")
        //  UpperHit(200);
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Public Func
    public void InstantiateArrow(bool _skill = false)
    {
        GameObject newArrow = Instantiate(EnemyManager.Instance.ArrowPrefab, firePos.position, Quaternion.identity);
        newArrow.GetComponent<Arrow>().SetArrow(((playerTR.position + arrowOffset) - firePos.position).normalized, 10, arrowPower, playerTR, arrowOffset, _skill);
    }

    public bool CalcIsGround()
    {
        float tmp = transform.localPosition.y;

        if (tmp < 0.1f && tmp > -0.1f)
        {
            isGrounded = true;
            //bIsFalling = false;
        }
        else
            isGrounded = false;

        return isGrounded;
    }

    public bool CalcIsFalling()
    {
        if (transform.position.y < preWorldPos.y)
            bIsFalling = true;
        else
        {
            bIsFalling = false;
        }

        return bIsFalling;
    }

    public void Hit()
    {
        if (type == EEnemyType.Enemy_Melee)
        {
            if (bSkillReady)
                meleeSkillCollider.gameObject.SetActive(true);
            else
                foreach (Collider coll in ListAttackColliders)
                    coll.gameObject.SetActive(true);
        }
        else
        {
            foreach (Collider coll in ListAttackColliders)
                coll.gameObject.SetActive(true);
        }

        return;
    }

    public void HitEnd()
    {
        if (type == EEnemyType.Enemy_Melee)
        {
            if (bSkillReady)
                meleeSkillCollider.gameObject.SetActive(false);
            else
                foreach (Collider coll in ListAttackColliders)
                    coll.gameObject.SetActive(false);
        }
        else
        {
            foreach (Collider coll in ListAttackColliders)
            {
                coll.gameObject.SetActive(false);
            }
        }

        return;
    }

    public void LookPlayer()
    {
        Quaternion look = Quaternion.identity;
        Vector3 dir = (PlayerTR.position - NavTR.position).normalized;
        dir.y = 0f;
        look.SetLookRotation(dir);

        NavTR.rotation = look;
    }

    public void ResetLocalPos()
    {
        transform.localPosition = Vector3.zero;
    }

    public bool SwitchPhysicsCollider()
    {
        physicsCollider.enabled = !physicsCollider.enabled;

        return physicsCollider.enabled;
    }

    public void PlayEffect()
    {
        if (_AI.CurrentState is SkillState)
            sparkEffect.Play();
    }
    #endregion
}
