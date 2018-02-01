using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private float idleTime;

    public override void Enter(Enemy _parent)
    {
        idleTime = 0f;

        parent = _parent;
        
        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.StopCoroutine(coroutine);        
    }

    public override void Update()
    {
        idleTime += Time.deltaTime;

        if (idleTime > parent.wanderingTime)
        {
            idleTime = 0f;
            parent.AI.Wander();
        }
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.Life == true)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(parent.MobTR.position, parent.PlayerTR.position);
            
            if (dist <= parent.attackRange)
            {
                parent.AI.Attack();
            }

            else if (dist < parent.aggroRadius)
            {
                parent.AI.Follow();
            }
        }
    }
}
