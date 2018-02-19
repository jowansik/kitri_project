﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunawayState : IState
{
    private Vector3 prePos;
    private bool bDest;
    private float runTime;

    public override void Enter(Enemy _parent)
    {
        runTime = 1f;
        bDest = false;

        parent = _parent;

        prePos = parent.transform.position;

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        bDest = false;
        runTime = 1f;

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
                parent.NavAgent.SetDestination(Vector3.zero);
                parent.NavAgent.isStopped = true;
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
                parent.AI.Idle();
            }
            else
            {
                Runaway();
            }
        }
    }

    private void Runaway()
    {
        Vector3 dest = parent.NavTR.position - parent.PlayerTR.position;
        dest.y = 0;

        parent.NavAgent.SetDestination(dest.normalized);
        parent.NavAgent.isStopped = false;
    }
}
