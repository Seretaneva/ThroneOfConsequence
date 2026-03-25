using UnityEngine;

public class PeasantMover : MonoBehaviour
{
    public float speed = 2f;
    private Vector3 target;
    private bool move = false;

    public void MoveTo(Vector3 pos)
    {
        target = pos;
        move = true;
    }

    void Update()
    {
        if (!move) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            move = false;
        }
    }
}