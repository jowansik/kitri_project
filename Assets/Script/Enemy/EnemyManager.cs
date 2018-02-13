using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MobInfo
{
    public int id;
    public int hp;
    public int maxHp;
    public float skillPoint;
}

public class EnemyManager : SingletonObejct<EnemyManager>
{
    public MobInfo lastHit = new MobInfo { id = 0, hp = 0, maxHp = 0, skillPoint = 0f };

    [SerializeField]
    private int lastHitMobID;
    [SerializeField]
    private int lastHitMobHP;
    [SerializeField]
    private int lastHitMobMaxHP;
    [SerializeField]
    private float lastHitMobSkillPoint;

    private Enemy lastHitMob = null;
    private GameObject arrowPrefab;
    private Dictionary<EEnemyType, GameObject> dicEnemyPrefab = new Dictionary<EEnemyType, GameObject>();
    private Dictionary<EEnemyType, List<Enemy>> dicEnemyList = new Dictionary<EEnemyType, List<Enemy>>();

    public int LastHitMobID { get { return lastHitMobID; } }
    public int LastHitMobHP { get { return lastHitMobHP; } }
    public int LastHitMobMaxHP { get { return lastHitMobMaxHP; } }
    public float LastHitMobSkillPoint { get { return LastHitMobSkillPoint; } }

    public Enemy LastHitMob { get { return lastHitMob; } set { lastHitMob = value; } }
    public GameObject ArrowPrefab { get { return arrowPrefab; } }
    public Dictionary<EEnemyType, GameObject> DicEnemyPrefab { get { return dicEnemyPrefab; } }
    public Dictionary<EEnemyType, List<Enemy>> DicEnemyList { get { return dicEnemyList; } }

    private void Start()
    {
        LoadPrefab();
    }

    private void Update()
    {
        if (lastHitMob != null)
        {
            UpdateMobInfo2();
        }
    }

    private void UpdateMobInfo2()
    {
        lastHitMobID = lastHitMob.mobId;
        lastHitMobHP = lastHitMob.NowHP;
        lastHitMobMaxHP = lastHitMob.HP;
        lastHitMobSkillPoint = lastHitMob.skillPoint;
    }

    public void UpdateMobInfo()
    {
        lastHitMobID = lastHit.id;
        lastHitMobHP = lastHit.hp;
        lastHitMobMaxHP = lastHit.maxHp;
        lastHitMobSkillPoint = lastHit.skillPoint;
    }

    public void ResetMobInfo()
    {
        lastHitMobID = 0;
        lastHitMobHP = 0;
        lastHitMobMaxHP = 0;
        lastHitMobSkillPoint = 0f;
    }

    public void LoadPrefab()
    {
        arrowPrefab = Resources.Load("jws/Prefab/Arrow") as GameObject;

        if (arrowPrefab == null)
            Debug.LogError("프리팹 로드 실패 : arrow");

        for (int i = 0; i < (int)EEnemyType.MAX; i++)
        {
            if (i == (int)EEnemyType.Enemy_Boss)
                continue;

            GameObject go = Resources.Load("jws/Prefab/" + ((EEnemyType)i).ToString("F")) as GameObject;

            if (go == null)
                Debug.LogError("프리팹 로드 실패 : " + ((EEnemyType)i).ToString("F"));
        }
    }

    public void AddEnemy(Enemy enemy)
    {
        List<Enemy> listEnemy = null;
        EEnemyType type = enemy.type;

        if (dicEnemyList.ContainsKey(type) == false)
        {
            listEnemy = new List<Enemy>();
            dicEnemyList.Add(type, listEnemy);
        }
        else
        {
            dicEnemyList.TryGetValue(type, out listEnemy);
        }

        listEnemy.Add(enemy);
    }

    public void RemoveEnemy(Enemy enemy, bool bDestroy = false)
    {
        EEnemyType enemyType = enemy.type;

        if (dicEnemyList.ContainsKey(enemyType) == true && dicEnemyList[enemyType].Count != 0)
        {
            List<Enemy> listEnemy = null;
            dicEnemyList.TryGetValue(enemyType, out listEnemy);
            listEnemy.Remove(enemy);
        }
        else
        {
            Debug.LogError("존재 하지 않는 Enemy를 삭제하려고 합니다.");
        }

        if (bDestroy)
        {
            if (enemy.gameObject != null)
                Destroy(enemy.gameObject);
        }
    }
}