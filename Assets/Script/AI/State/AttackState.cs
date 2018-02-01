using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{    private float normalizedTime;

    public override void Enter(Enemy _parent)
    {
        parent = _parent;
        
        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.StopCoroutine(coroutine);        
    }

    public override void Update()
    {
        animatorStateInfo = parent.Animator.GetCurrentAnimatorStateInfo(0);

        normalizedTime = animatorStateInfo.normalizedTime;
        
        // 공격 시작시 방향 전환
        if ((normalizedTime * 10.0f) % 10 < 1f)
        {
            Quaternion look = Quaternion.identity;
            Vector3 dir = (parent.PlayerTR.position - parent.MobTR.position).normalized;
            dir.y = 0f;
            look.SetLookRotation(dir);

            parent.MobTR.rotation = look;
        }
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.Life == true)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(parent.MobTR.position, parent.PlayerTR.position);

            if (dist > parent.attackRange)
            {
                parent.AI.Idle();
            }
        }
    }
}
