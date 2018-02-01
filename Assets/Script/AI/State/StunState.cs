using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IState
{
    private Quaternion preRotation;

    public override void Enter(Enemy _parent)
    {
        parent = _parent;
        
        preRotation = parent.transform.rotation;

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.StopCoroutine(coroutine);

        parent.transform.rotation = preRotation;
    }

    public override void Update()
    {
        parent.transform.rotation = preRotation;

        animatorStateInfo = parent.Animator.GetCurrentAnimatorStateInfo(0); // LayerMask 사용 불가?

        if (animatorStateInfo.normalizedTime >= 0.8f)
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
