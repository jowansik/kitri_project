﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowState : IState
{
    public override void Enter(Enemy _parent)
    {
        parent = _parent;

        parent.NavAgent.SetDestination(parent.PlayerTR.position);
        parent.NavAgent.isStopped = false;
        
        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.NavAgent.SetDestination(Vector3.zero);
        parent.NavAgent.isStopped = true;

        parent.StopCoroutine(coroutine);        
    }

    public override void Update()
    {
        Quaternion look = Quaternion.identity;
        Vector3 dir = (parent.PlayerTR.position - parent.MobTR.position).normalized;
        dir.y = 0f;
        look.SetLookRotation(dir);

        parent.MobTR.rotation = look;
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.Life == true)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(parent.MobTR.position, parent.PlayerTR.position);

            // y축 감지 ?
            if (dist < parent.aggroRadius && dist > parent.attackRange)
            {
                parent.NavAgent.SetDestination(parent.PlayerTR.position);
            }
            else
            {
                parent.AI.Idle();
            }
        }
    }
}
