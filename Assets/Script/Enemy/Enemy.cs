using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Actor
{
    public EEnemyType type;

    public float meleeAttackRange;
    public float recoveryTime;
    public float aggroRadius;
    public float maxAggroRadius;

    // Melee
    public float wanderingTime;

    // Archor
    public float reloadTime;
    //public float runawayRange;

    private bool life = true;
    private Transform mobTR;
    private Transform playerTR;
    private Animator animator;

    private NavMeshAgent navAgent;
    private BaseAI _AI;

    private List<IState> listStates;
    private Dictionary<EEnemyState, IState> dicState = new Dictionary<EEnemyState, IState>();

    public List<IState> ListStates { get { return listStates; } }
    public Dictionary<EEnemyState, IState> DicState { get { return dicState; } }
    public Transform MobTR { get { return mobTR; } set { mobTR = value; } }
    public Transform PlayerTR { get { return playerTR; } set { playerTR = value; } }
    public Animator Animator { get { return animator; } }
    public NavMeshAgent NavAgent { get { return navAgent; } }
    public BaseAI AI { get { return _AI; } }

    public bool Life
    {
        get { return life; }
        set { life = value; }
    }

    private void Awake()
    {
    }

    void Start()
    {
        mobTR = GetComponent<Transform>();
        playerTR = GameObject.FindWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        Init();

        _AI.Idle();
    }

    void Update()
    {
        _AI.UpdateAI();
    }

    public override void Init()
    {
        SetCollections();

        switch (type)
        {
            case EEnemyType.Enemy_Melee:
                {
                    _AI = new MeleeAI();

                    hp = 1000;
                    mp = 0;
                    power = 30;
                }
                break;
            case EEnemyType.Enemy_Archor:
                {
                    _AI = new ArchorAI();

                    hp = 800;
                    mp = 50;
                    power = 20;
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
    }

    public override void onDamaged(int damage)
    {
        if (life == false)
            return;

        Debug.Log(this + " onDamaged : " + damage);
        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;

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
        life = false;
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
            new AttackState(),
            new HitState(),
            new CriticalHitState(),
            new StunState(),
            new DieState(),
            new FollowState(),
            new RunawayState(),
            new WanderState()
        };

        for (int i = 0; i < (int)EEnemyState.MAX; i++)
        {
            dicState.Add((EEnemyState)i, listStates[i]);
        }

        ListAttackColliders = new List<Collider>();
        ListAttackColliders.Add(FindInChild("AttackColliderLeftArm").GetComponent<Collider>());
        ListAttackColliders.Add(FindInChild("AttackColliderRightArm").GetComponent<Collider>());
    }
}
