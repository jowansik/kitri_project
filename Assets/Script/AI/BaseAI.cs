using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAI
{
    protected Enemy enemy;
    private IState currentState;

    public Enemy Enemy { set { enemy = value; } }

    public virtual void Idle() { ChangeState(EEnemyState.State_Idle); }
    public virtual void MeleeAttack() { ChangeState(EEnemyState.State_MeleeAttack); }
    public virtual void Hit() { ChangeState(EEnemyState.State_Hit); }
    public virtual void CriticalHit() { ChangeState(EEnemyState.State_CriticalHit); }
    public virtual void Stun() { ChangeState(EEnemyState.State_Stun); }
    public virtual void Die() { ChangeState(EEnemyState.State_Die); }
    public virtual void Follow() { ChangeState(EEnemyState.State_Follow); }
    public virtual void Wander() { ChangeState(EEnemyState.State_Wander); }
    public virtual void ArrowAttack() { ChangeState(EEnemyState.State_ArrowAttack); }
    public virtual void UpperHit() { ChangeState(EEnemyState.State_UpperHit); }
    public virtual void SkillState() { ChangeState(EEnemyState.State_Skill); }

    public virtual void UpdateAI()
    {
        currentState.Update();
    }

    private void ChangeState(EEnemyState eState)
    {
        if (eState == EEnemyState.MAX)
        {
            Debug.LogError("EEnemyState.MAX 를 매개변수로 사용했습니다.");
            return;
        }

        IState newState = enemy.ListStates[(int)eState];

        if (currentState != null)
        {
            currentState.Exit();
            //Debug.Log("Exit State : " + currentState.ToString());
        }

        currentState = newState;

        currentState.Enter(enemy);
        //Debug.Log("Enter State : " + newState.ToString());

        ChangeAnimation(eState);
    }

    private void ChangeAnimation(EEnemyState enemyState)
    {
        if (enemyState == EEnemyState.State_CriticalHit)
            enemy.Animator.SetTrigger("Critical Hit");

        else if (enemyState == EEnemyState.State_Hit)
            enemy.Animator.SetTrigger("Hit");

        else if (enemyState == EEnemyState.State_ArrowAttack)
            enemy.Animator.SetTrigger("ArrowAttack");

        enemy.Animator.SetInteger("State", (int)enemyState);
    }
}
