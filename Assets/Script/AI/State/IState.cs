using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IState
{
    protected Enemy parent;
    protected Coroutine coroutine;
    protected AnimatorStateInfo animatorStateInfo;

    public virtual void Enter(Enemy _parent) { parent = _parent; }
    public virtual void Update() { }
    public virtual void Exit() { }
    public virtual IEnumerator CheckMobState() { yield break; }
}
