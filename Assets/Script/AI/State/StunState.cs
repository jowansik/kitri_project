using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IState
{
    private Quaternion preRotation;
    private float stunTime = 0f;

    public override void Enter(Enemy _parent)
    {
        parent = _parent;
        
        preRotation = parent.MobTR.rotation;

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.StopCoroutine(coroutine);

        parent.MobTR.rotation = preRotation;

        stunTime = 0f;
    }

    public override void Update()
    {
        parent.MobTR.rotation = preRotation;

        stunTime += Time.deltaTime;

        if (stunTime >= parent.recoveryTime)
            parent.AI.Idle();
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.Life == true)
        {
            yield return new WaitForSeconds(0.2f);
        }
    }
}
