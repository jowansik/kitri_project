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

    private IState currentState;

    public Transform MobTR
    {
        get { return mobTR; }
        set { mobTR = value; }
    }

    public Transform PlayerTR
    {
        get { return playerTR; }
        set { playerTR = value; }
    }

    public Animator Animator
    {
        get { return animator; }
        set { animator = value; }
    }

    private void Awake()
    {
        currentState = new IdleState();
    }

    // Use this for initialization
    void Start()
    {
        mobTR = GetComponent<Transform>();
        playerTR = GameObject.FindWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        StartCoroutine(CheckMobState());
    }

    // Update is called once per frame
    void Update()
    {
        currentState.Update();
    }

    IEnumerator CheckMobState()
    {
        while (life == true)
        {
            yield return new WaitForSeconds(0.2f);

            float dist = Vector3.Distance(mobTR.position, playerTR.position);

            if (dist < aggroRadius && dist > attackRange)
            {
                animator.SetBool("IsTracing", true);
                ChangeState(new FollowState());
            }
            else
            {
                animator.SetBool("IsTracing", false);
                ChangeState(new IdleState());
            }
        }
    }

    public void ChangeState(IState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;

        currentState.Enter(this);
    }
}
