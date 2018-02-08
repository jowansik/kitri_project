using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public struct Status
{
    public int hp;
    public int mp;
    public int power;
}

[RequireComponent(typeof(Animator))]
public class Actor : MyBaseObejct
{
    //public 변수

    public float JumpForce = 100;

    public float MoveSpeed = 2;
    public float NowMoveSpeed = 2;

    //공격판정 컬라이더 목록
    public List<Collider> ListAttackColliders;
    public List<AttackCollider> ListAttackCollidersComp;

    //protected 변수
    //protected Animator CompAnimator;
    protected Vector3 moveDirection;

    protected float sqrtVel;

    //지상 공중 판정
    protected bool[] isG;
    protected bool isGrounded;

    //공격으로 밀쳐질 방향
    protected Vector3 attackDirction;

    // init status
    [SerializeField]
    protected int hp;
    [SerializeField]
    protected int mp;
    [SerializeField]
    protected int power;
    [SerializeField]
    protected float nowAttackPower;
    [SerializeField]
    protected int nowHp;
    [SerializeField]
    protected int nowMp;
    [SerializeField]
    protected int nowPower;

    protected bool isAlive;
    protected bool isRolling;
    //중복공격 방지
    public Dictionary<GameObject, bool> attackedObject = new Dictionary<GameObject, bool>();



    public bool IsAlive
    {
        get { return isAlive; }
    }

    public float NowAttackPower
    {
        get { return nowAttackPower; }
        set { nowAttackPower = value; }
    }
    public Vector3 AttackDirction
    {
        get { return attackDirction; }
        set { attackDirction = value; }
    }

    public int HP
    {
        get { return hp; }
        set { hp = value; }
    }
    public int MP
    {
        get { return mp; }
        set { mp = value; }
    }
    public int POWER
    {
        get { return power; }
        set { power = value; }
    }

    public int NowHP
    {
        get { return nowHp; }
    }
    public int NowMP
    {
        get { return nowMp; }
    }
    public int NowPOWER
    {
        get { return nowPower; }
    }

    //property
    public virtual Vector3 Velocity
    {
        get { return moveDirection; }
        set { moveDirection = value; }
    }
    public float SqrtVel
    {
        get { return sqrtVel; }
    }

    public bool IsGrounded
    {
        get { return isGrounded; }

    }

    public bool IsRolling { get { return isRolling; } }
    //method
    public virtual void StatusInit(Status st)
    {
        hp = st.hp;
        mp = st.mp;
        power = st.power;
        isAlive = true;
    }


    public virtual void Init()
    {
        NowAttackPower = 1.0f;
        isAlive = true;
        NowMoveSpeed = MoveSpeed;
    }

    public virtual void Rolling()
    {
        isRolling = true;
    }

    public virtual void AttackRecoverMana()
    {
        print(gameObject.name + "의 공격 마나 회복!");
        if (nowMp < mp)
            nowMp += 5;
        if (nowMp > mp)
            nowMp = mp;
    }
    public virtual void DamagedRecoverMana()
    {
        print(gameObject.name + "의 피격 마나 회복!");
        if (nowMp < mp)
            nowMp += 5;
        if (nowMp > mp)
            nowMp = mp;
    }
    public virtual void EndRolling()
    {
        print("end ROlling!");
        isRolling = false;
    }
    public virtual void Move(Vector3 dir)
    {
        moveDirection += dir;
    }

    public virtual void Attack()
    {

    }
    public virtual void Skill()
    {

    }
    public virtual void onDamaged(int damage)
    {
        if (isRolling == false)
        {
            nowHp -= damage;
            print(gameObject.ToString() + "의 HP = " + nowHp);
            if (nowHp < 0)
                onDead();
        }
    }
    public virtual void onDead()
    {
        isAlive = false;
        //죽음 처리

    }
    public virtual void PowerUp(float coefficient)
    {
        nowPower = (int)(power * coefficient);
    }
    public virtual void PowerReset()
    {
        nowPower = power;
    }


    public void EndAttack()
    {
        List<GameObject> attacked = attackedObject.Keys.ToList<GameObject>();
        for (int i = 0; i < attacked.Count; i++)
        {
            attackedObject[attacked[i]] = false;
        }

        //print("Attackend!");
    }

    public virtual void UpperHit(int _power)
    {

    }
}
