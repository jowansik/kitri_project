using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MyBaseObejct
{
    Actor actor;
    public int attackState;
    public int AttackState
    {
        get { return attackState; }
        set { attackState = value; }
    }

    // Use this for initialization
    void Start()
    {
        //actor = FindInParentComp<Actor>();
        actor = GetComponentInParent<Actor>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isinAttacked = false;
        if (!actor.attackedObject.TryGetValue(other.gameObject, out isinAttacked))
        {
            actor.attackedObject.Add(other.gameObject, true);
        }
        actor.attackedObject[other.gameObject] = true;

        if (isinAttacked == false)
        {
            print("Hit다 hit! 맞은놈 : " + other.ToString() + "데미지 : " + actor.NowPOWER * actor.NowAttackPower + " 밀리는 방향 " + actor.AttackDirction);
            other.SendMessage("onDamaged", actor.NowPOWER * actor.NowAttackPower);
           
            actor.AttackRecoverMana();
            other.SendMessage("DamagedRecoverMana");
        }
    }
}
