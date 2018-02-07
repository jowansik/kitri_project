using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Actor
{
    public EEnemyType type;

    public float meleeAttackRange;
    public float arrowAttackRange;
    public float recoveryTime;
    public float aggroRadius;
    public float maxAggroRadius;

    // Melee
    public float wanderingTime;

    // Archor
    public float reloadTime;
    private float OldReloadTime;
    private int arrowPower;
    private bool bReload = false;   // 쏘고나서 장전
    //public float runawayRange;
    
    private Transform mobTR;
    private Transform playerTR;
    private Transform firePos;
    private Animator animator;
    private NavMeshAgent navAgent;
    private BaseAI _AI;

    private List<IState> listStates;
    private Dictionary<EEnemyState, IState> dicState;

    public List<IState> ListStates { get { return listStates; } }
    public Dictionary<EEnemyState, IState> DicState { get { return dicState; } }
    public Transform MobTR { get { return mobTR; } set { mobTR = value; } }
    public Transform PlayerTR { get { return playerTR; } set { playerTR = value; } }
    public Transform FirePos { get { return firePos; } }
    public Animator Animator { get { return animator; } }
    public NavMeshAgent NavAgent { get { return navAgent; } }
    public BaseAI AI { get { return _AI; } }

    public int ArrowPower { get { return arrowPower; } set { arrowPower = value; } }
    public bool BReload { get { return bReload; } set { bReload = value; } }

    private void Awake()
    {
    }

    void Start()
    {
        mobTR = transform.parent.GetComponent<Transform>();
        playerTR = GameObject.FindWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        navAgent = GetComponentInParent<NavMeshAgent>();

        Init();

        _AI.Idle();
    }

    void Update()
    {
        _AI.UpdateAI();

        if (bReload)
        {
            CalcReloadTime();
        }

        //Vector3 tmp = transform.localPosition;
        //tmp.x = 0f;
        //tmp.z = 0f;
        //transform.localPosition = tmp;
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
                        mp = 200,
                        power = 30
                    };

                    base.StatusInit(st);
                }
                break;
            case EEnemyType.Enemy_Archor:
                {
                    _AI = new ArchorAI();

                    firePos = FindInChild("FirePos");
                    OldReloadTime = reloadTime;

                    Status st = new Status
                    {
                        hp = 800,
                        mp = 150,
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
        newArrow.GetComponent<Arrow>().SetArrow((playerTR.position - firePos.position).normalized, 10, arrowPower);
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
    }

    public override void onDead()
    {
        _AI.Die();
        isAlive = false;
    }

    public override void Skill()
    {
        base.Skill();
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
            new ArrowAttackState()
        };

        dicState = new Dictionary<EEnemyState, IState>();
        for (int i = 0; i < (int)EEnemyState.MAX; i++)
        {
            dicState.Add((EEnemyState)i, listStates[i]);
        }

        ListAttackColliders = new List<Collider>();
        ListAttackColliders.Add(FindInChild("AttackColliderLeftArm").GetComponent<Collider>());
        ListAttackColliders.Add(FindInChild("AttackColliderRightArm").GetComponent<Collider>());

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
    }

    public void LookPlayer()
    {
        Quaternion look = Quaternion.identity;
        Vector3 dir = (PlayerTR.position - MobTR.position).normalized;
        dir.y = 0f;
        look.SetLookRotation(dir);

        MobTR.rotation = look;
    }
}
