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
    private Animator animator;

	// Use this for initialization
	void Start ()
    {
        mobTR = GetComponent<Transform>();
        playerTR = GameObject.FindWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
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

            float dist = Vector2.Distance(mobTR.position, playerTR.position);

            // 상태변경 코드
            if (dist < aggroRadius && dist > attackRange)
            {
                Quaternion look = Quaternion.identity;
                Vector3 dir = (playerTR.position - mobTR.position).normalized;
                look.SetLookRotation(dir);

                mobTR.rotation = look;

                animator.SetBool("IsTracing", true);
            }
            else
            {
                animator.SetBool("IsTracing", false);
            }
        }
    }
}
