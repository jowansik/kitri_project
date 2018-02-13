using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunawayState : IState
{
    private Vector3 prePos;
    private bool bDest = false;
    private float runTime = 1f;

    public override void Enter(Enemy _parent)
    {
        parent = _parent;

        prePos = parent.transform.position;

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        bDest = false;

        parent.StopCoroutine(coroutine);
    }

    public override void Update()
    {
        base.Update();

        if (bDest == false)
        {
            runTime -= Time.deltaTime;

            if (runTime <= 0f)
            {
                runTime = 0f;
                bDest = true;
            }
        }
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.IsAlive == true)
        {
            yield return new WaitForSeconds(0.2f);
            
            if (bDest)
            {
                parent.NavAgent.isStopped = true;

                // 도망친 거리가 meleeAttackRange보다 짧으면 도망불가판정
                float runDist = Vector3.Distance(prePos, parent.transform.position);

                if (runDist < parent.meleeAttackRange)
                    parent.BRunaway = false;

                parent.AI.Idle();
            }
            else
            {
                parent.NavAgent.isStopped = false;
                Runaway();
            }
        }
    }

    private void Runaway()
    {
        Vector3 dest = parent.transform.position - parent.PlayerTR.position;
        dest.y = 0;

        parent.NavAgent.SetDestination(dest.normalized);
    }
}
