﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEquipmentState
{
    CharEqState_Fight ,
    CharEqState_Sword ,
    CharEqState_Gun,
    CharEqState_End
}
public enum ECharaterState
{
    CharState_idle,
    CharState_move,
    CharState_attack,
    CharState_dead,
}

public enum EAttackColliderIndex
{
    ACI_LeftFoot = 0,
    ACI_RightFoot = 1,
    ACI_LeftHand = 2,
    ACI_RightHand = 3,
}
public enum EBaseObjectState
{
    ObjectState_Normal,
    ObjectState_Die,
}

public enum EButtonList
{
    EBL_AttackA,
    EBL_AttackB,
    EBL_Skill,
    EBL_Jump,
}

public enum EEnemyType
{
    Enemy_Melee,
    Enemy_Archor,
    Enemy_Boss,
	MAX
}

public enum EEnemyState
{
    State_Idle,
    State_MeleeAttack,
    State_Hit,
    State_CriticalHit,
    State_Stun,
    State_Die,
    State_Follow,
    State_Wander,
    State_ArrowAttack,
    State_UpperHit,
    State_Skill,
    MAX
}
