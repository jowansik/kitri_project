using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    public float aggroRadius;
    public float maxAggroRadius;
    public float attackRange;

    private Transform mobTR;
    private Transform playerTR;
    private bool life = true;

	// Use this for initialization
	void Start ()
    {
        mobTR = GetComponent<Transform>();
        playerTR = GameObject.FindWithTag("Player").GetComponent<Transform>();
        StartCoroutine(CheckMobState());
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    IEnumerator CheckMobState()
    {
        while(life == true)
        {
            yield return new WaitForSeconds(0.2f);

            // 상태변경 코드
        }
    }
}
