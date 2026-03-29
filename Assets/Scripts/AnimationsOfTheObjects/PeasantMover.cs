using UnityEngine;

public class PeasantMover : MonoBehaviour
{
    public Transform stopPoint;
    public float speed = 2f;

    private Animator anim;
    private bool isMoving = true;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isWalking", true);
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                stopPoint.position,
                speed * Time.deltaTime
            );

            float distance = Vector3.Distance(
                transform.position,
                stopPoint.position
            );

            if (distance < 0.1f)
            {
                isMoving = false;
                anim.SetBool("isWalking", false);
            }
        }
    }
}