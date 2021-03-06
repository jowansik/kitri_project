﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IState
{
    private Quaternion preRotation;
    private float stunTime = 0f;

    public override void Enter(Enemy _parent)
    {
        parent = _parent;
        
        preRotation = parent.NavTR.rotation;

        coroutine = parent.StartCoroutine(CheckMobState());
    }

    public override void Exit()
    {
        parent.StopCoroutine(coroutine);

        parent.NavTR.rotation = preRotation;

        stunTime = 0f;
    }

    public override void Update()
    {
        parent.NavTR.rotation = preRotation;

        stunTime += Time.deltaTime;

        if (stunTime >= parent.recoveryTime)
            parent.AI.Idle();
    }

    public override IEnumerator CheckMobState()
    {
        while (parent.IsAlive == true)
        {
            yield return new WaitForSeconds(0.2f);
        }
    }
}
