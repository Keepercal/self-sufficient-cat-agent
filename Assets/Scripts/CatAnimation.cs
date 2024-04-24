using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnimation : MonoBehaviour
{

    private Rigidbody2D rBody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rBody = transform.parent.GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        float speed = rBody.velocity.magnitude;
        animator.SetFloat("Speed", speed);
        
        FlipSprite();
    }

    void FlipSprite()
    {
        if (rBody.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (rBody.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }
}
