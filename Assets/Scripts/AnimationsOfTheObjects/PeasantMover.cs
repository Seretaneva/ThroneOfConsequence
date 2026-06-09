using UnityEngine;
using System;

public class PeasantMover : MonoBehaviour
{
    public Transform stopPoint;
    public float speed = 2f;
    public Action onReachedTarget;

    private Animator anim;
    private bool isMoving = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        SetWalking(false);
    }

    public void StartMoving()
    {
        anim = GetComponent<Animator>();
        isMoving = true;
        SetWalking(true);
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
            SetWalking(false);
            onReachedTarget?.Invoke();
        }
    }

    private void SetWalking(bool value)
    {
        if (anim != null)
            anim.SetBool("isWalking", value);
    }
}