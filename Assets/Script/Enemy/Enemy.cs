﻿using System.Collections;
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
    private bool arrowAttack = true;
    //public float runawayRange;

    private bool life = true;
    private Transform mobTR;
    private Transform playerTR;
    private Transform fireDir;
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
    public Transform FireDir { get { return fireDir; } }
    public Transform FirePos { get { return firePos; } }
    public Animator Animator { get { return animator; } }
    public NavMeshAgent NavAgent { get { return navAgent; } }
    public BaseAI AI { get { return _AI; } }

    public int ArrowPower { get { return arrowPower; } set { arrowPower = value; } }
    public bool Life { get { return life; } set { life = value; } }
    public bool ArrowAttack { get { return arrowAttack; } set { arrowAttack = value; } }

    private void Awake()
    {
    }

    void Start()
    {
        mobTR = GetComponentInParent<Transform>();
        playerTR = GameObject.FindWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        navAgent = GetComponentInParent<NavMeshAgent>();

        Init();

        _AI.Idle();
    }

    void Update()
    {
        _AI.UpdateAI();

        if (arrowAttack == false && reloadTime > 0)
        {
            reloadTime -= Time.deltaTime;
            if (reloadTime <= 0)
            {
                arrowAttack = true;
                reloadTime = OldReloadTime;
            }
        }

        transform.localPosition = Vector3.zero;
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

                    fireDir = FindInChild("FireDir");
                    firePos = FindInChild("FirePos");
                    OldReloadTime = reloadTime;

                    hp = 800;
                    mp = 50;
                    power = 5;
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
    }

    public void InstantiateArrow()
    {
        GameObject myArrow = Instantiate(EnemyManager.Instance.ArrowPrefab, firePos.position, Quaternion.identity);
        myArrow.GetComponent<Arrow>().SetArrow((playerTR.position - firePos.position).normalized, 10, arrowPower);
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
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.tag =="Player")
    //    {
    //        //GetComponent<Rigidbody>().AddForce(Vector3.up * 10);
    //        Debug.Log("충돌");
    //    }
    //}
}
