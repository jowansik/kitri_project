using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunawayState : IState
{
    private Vector3 prePos;
    private bool bDest;
    private float runTime;
    private bool isRunning;

    public override void Enter(Enemy _parent)
    {
        bDest = false;
        runTime = 1f;
        isRunning = false;

        parent = _parent;

        prePos = parent.transform.position;

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        bDest = false;
        runTime = 1f;
        isRunning = false;

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
                // 도망친 거리가 meleeAttackRange보다 짧으면 도망불가판정
                float runDist = Vector3.Distance(prePos, parent.transform.position);

                if (runDist < parent.meleeAttackRange)
                    parent.BRunaway = false;

                Debug.Log("도망친거리 : " + runDist + ", " + (runDist < parent.meleeAttackRange));
                parent.NavAgent.SetDestination(Vector3.zero);
                parent.NavAgent.isStopped = true;
                parent.AI.Idle();
            }
            else if (isRunning == false)
            {
                isRunning = true;

                Runaway();
            }
        }
    }

    private void Runaway()
    {
        Vector3 dir = parent.NavTR.position - parent.PlayerTR.position;
        dir.y = 0;

        Vector3 dest = dir.normalized * 3f + parent.NavTR.position;

        parent.NavTR.LookAt(dest);

        parent.NavAgent.SetDestination(dest);
        parent.NavAgent.isStopped = false;
    }
}
