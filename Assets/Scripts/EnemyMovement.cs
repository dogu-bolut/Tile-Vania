using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float scaleY = 1f;
    [SerializeField] float Xfactor = 1f;
    Rigidbody2D enemyRigidbody;
    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsFacingRight())
        {
            enemyRigidbody.linearVelocity = new Vector2(moveSpeed, 0f);
        }
        else
        {
            enemyRigidbody.linearVelocity = new Vector2(-moveSpeed, 0f);
        }
    }
    bool IsFacingRight()
    {
        return transform.localScale.x > 0;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        transform.localScale = new Vector2(-(Mathf.Sign(enemyRigidbody.linearVelocity.x)) * Xfactor, scaleY);
    }
}
