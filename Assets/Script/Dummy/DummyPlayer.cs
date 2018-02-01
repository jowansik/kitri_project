using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : Actor
{
    public override void Attack()
    {
        base.Attack();
    }

    public override object GetData(string keyData, params object[] datas)
    {
        return base.GetData(keyData, datas);
    }

    public override void Init()
    {
        base.Init();
    }

    public override void Move(Vector3 dir)
    {
        base.Move(dir);
    }

    public override void onDamaged(int damage)
    {
        Debug.Log("DummyPlayer onDamaged : " + damage);
    }

    public override void onDead()
    {
        base.onDead();
    }

    public override void Skill()
    {
        base.Skill();
    }

    public override void ThrowEvent(string keyData, params object[] datas)
    {
        base.ThrowEvent(keyData, datas);
    }
}
