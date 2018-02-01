using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    //private Enemy parent;
    //private Coroutine coroutine;
    
    public override void Enter(Enemy _parent)
    {
        parent = _parent;

        parent.Animator.SetBool("IsIdle", true);

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.StopCoroutine(coroutine);

        parent.Animator.SetBool("IsIdle", false);
    }

    public override void Update()
    {
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.Life == true)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(parent.MobTR.position, parent.PlayerTR.position);
            
            if (dist <= parent.attackRange)
            {
                parent.AI.ChangeState(EEnemyState.State_Attack);
            }

            else if (dist < parent.aggroRadius)
            {
                parent.AI.ChangeState(EEnemyState.State_Follow);
            }
        }
    }
}
