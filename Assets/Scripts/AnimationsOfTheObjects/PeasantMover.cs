using UnityEngine;
using System;

public class PeasantMover : MonoBehaviour
{
    public Transform stopPoint;
    public float speed = 2f;
    public Action onReachedTarget;

    private Animator anim;
    private bool isMoving = false; 

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isWalking", false); 
    }

    public void StartMoving() 
    {
        isMoving = true;
        if (anim != null) anim.SetBool("isWalking", true);
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            stopPoint.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, stopPoint.position) < 0.1f)
        {
            isMoving = false;
            if (anim != null) anim.SetBool("isWalking", false);
            onReachedTarget?.Invoke();
        }
    }
}