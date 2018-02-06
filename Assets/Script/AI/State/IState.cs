using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IState
{
    protected Enemy parent;
    protected Coroutine coroutine;
    protected AnimatorStateInfo animatorStateInfo;
    protected float normalizedTime;

    public virtual void Enter(Enemy _parent) { parent = _parent; }
    public virtual void Update()
    {
        animatorStateInfo = parent.Animator.GetCurrentAnimatorStateInfo(0);
        normalizedTime = animatorStateInfo.normalizedTime;
    }
    public virtual void Exit() { }
    public virtual IEnumerator CheckMobState() { yield break; }
}
