using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Actor
{
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
    private float OldReloadTime;
    private int arrowPower;
    private bool bReload = false;
    //public float runawayRange;

    private bool bUpperHit = false;
    private bool bIsFalling = false;
    // private bool bFallingEndFlag = false;
    private bool bSkillReady = false;
    private Vector3 arrowOffset = Vector3.up * 0.5f;
    private Vector3 prePos;
    private Transform mobTR;
    private Transform playerTR;
    [SerializeField]
    private CapsuleCollider physicsCollider;
    [SerializeField]
    private CapsuleCollider triggerCollider;
    private Transform firePos;
    private GameObject arrow;
    private Animator animator;
    private NavMeshAgent navAgent;
    private BaseAI _AI;
    private Rigidbody rigidBody;

    private List<IState> listStates;
    private Dictionary<EEnemyState, IState> dicState;

    public List<IState> ListStates { get { return listStates; } }
    public Dictionary<EEnemyState, IState> DicState { get { return dicState; } }
    public Transform MobTR { get { return mobTR; } set { mobTR = value; } }
    public Transform PlayerTR { get { return playerTR; } set { playerTR = value; } }
    public CapsuleCollider PhysicsCollider { get { return physicsCollider; } }
    public CapsuleCollider TriggerCollider { get { return triggerCollider; } }
    public Transform FirePos { get { return firePos; } }
    public Animator Animator { get { return animator; } }
    public NavMeshAgent NavAgent { get { return navAgent; } }
    public BaseAI AI { get { return _AI; } }
    public GameObject Arrow { get { return arrow; } }
    public Rigidbody RigidBody { get { return rigidBody; } set { rigidBody = value; } }

    public int ArrowPower { get { return arrowPower; } set { arrowPower = value; } }
    public bool BReload { get { return bReload; } set { bReload = value; } }
    public bool BUpperHit { get { return bUpperHit; } set { bUpperHit = value; } }
    public bool BIsFalling { get { return bIsFalling; } set { bIsFalling = value; } }
    //public bool BFallingEndFlag { get { return bFallingEndFlag; } }
    public bool BSkillReady { get { return bSkillReady; } set { bSkillReady = value; } }

    private void Start()
    {
        mobTR = transform.parent.GetComponent<Transform>();
        playerTR = GameObject.FindWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        navAgent = GetComponentInParent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();

        Init();

        _AI.Idle();
    }

    private void Update()
    {
        if (IsAlive == false)
            return;

        if (bSkillReady == false)
            skillPoint += Time.deltaTime * (100 / skillChargeTime);

        if (skillPoint > 100)
        {
            skillPoint = 100f;

            bSkillReady = true;
        }

        _AI.UpdateAI();

        if (bReload)
        {
            CalcReloadTime();
        }
    }

    private void LateUpdate()
    {
        prePos = transform.position;
        //bFallingEndFlag = false;
    }

    public override void Init()
    {
        SetCollections();

        switch (type)
        {
            case EEnemyType.Enemy_Melee:
                {
                    _AI = new MeleeAI();

                    Status st = new Status
                    {
                        hp = 1000,
                        mp = 100,
                        power = 30
                    };

                    base.StatusInit(st);
                }
                break;
            case EEnemyType.Enemy_Archer:
                {
                    _AI = new ArcherAI();

                    firePos = FindInChild("FirePos");

                    arrow = FindInChild("Elven Long Bow Arrow").gameObject;
                    arrow.SetActive(false);

                    OldReloadTime = reloadTime;

                    Status st = new Status
                    {
                        hp = 800,
                        mp = 100,
                        power = 5
                    };

                    base.StatusInit(st);

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

    public void InstantiateArrow()
    {
        GameObject newArrow = Instantiate(EnemyManager.Instance.ArrowPrefab, firePos.position, Quaternion.identity);
        newArrow.GetComponent<Arrow>().SetArrow(((playerTR.position + arrowOffset) - firePos.position).normalized, 10, arrowPower, playerTR, arrowOffset);
    }

    private void CalcReloadTime()
    {
        if (reloadTime > 0)
        {
            reloadTime -= Time.deltaTime;

            if (reloadTime <= 0)
            {
                bReload = false;
                reloadTime = OldReloadTime;
            }
        }
    }

    public bool CalcIsGround()
    {
        float tmp = mobTR.position.y - transform.position.y;

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
        if (transform.position.y < prePos.y)
            bIsFalling = true;
        else
        {
            bIsFalling = false;
        }

        return bIsFalling;
    }

    public override void onDamaged(int damage)
    {
        if (IsAlive == false)
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
            {
                _AI.CriticalHit();
            }
            else
            {
                _AI.Hit();
            }
        }

        if (isAlive)
        {
            EnemyManager.Instance.lastHit.id = mobId;
            EnemyManager.Instance.lastHit.hp = nowHp;
            EnemyManager.Instance.lastHit.maxHp = hp;
        }
        else
        {
            EnemyManager.Instance.lastHit.id = 0;
            EnemyManager.Instance.lastHit.hp = 0;
            EnemyManager.Instance.lastHit.maxHp = 0;
        }

        EnemyManager.Instance.UpdateMobInfo();
    }

    public override void onDead()
    {
        _AI.Die();
        isAlive = false;

        Invoke("Kill", 1.5f);
    }

    public override void Skill()
    {

    }

    public override object GetData(string keyData, params object[] datas)
    {
        return base.GetData(keyData, datas);
    }

    public override void ThrowEvent(string keyData, params object[] datas)
    {
        base.ThrowEvent(keyData, datas);
    }

    public void Hit()
    {
        foreach (Collider coll in ListAttackColliders)
        {
            coll.gameObject.SetActive(true);
        }

        return;
    }

    public void HitEnd()
    {
        foreach (Collider coll in ListAttackColliders)
        {
            coll.gameObject.SetActive(false);
        }

        return;
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
            new SkillState()
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
                //ListAttackColliders.Add(FindInChild("AttackColliderLeftArm").GetComponent<Collider>());
                ListAttackColliders.Add(FindInChild("AttackColliderRightArm").GetComponent<Collider>());
                break;
            case EEnemyType.Enemy_Archer:
                //ListAttackColliders.Add(FindInChild("AttackColliderLeftLeg").GetComponent<Collider>());
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
        //    UpperHit(200);
    }

    public void LookPlayer()
    {
        Quaternion look = Quaternion.identity;
        Vector3 dir = (PlayerTR.position - MobTR.position).normalized;
        dir.y = 0f;
        look.SetLookRotation(dir);

        MobTR.rotation = look;
    }

    public override void UpperHit(int _power)
    {
        Vector3 tmp = Vector3.up * _power;

        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        rigidBody.AddForce(tmp);

        bUpperHit = true;
        _AI.UpperHit();
    }

    private void Kill()
    {
        Destroy(gameObject);
    }
}
