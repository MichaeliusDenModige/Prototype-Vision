﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rg;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;

    public int HorizontalSpeed;
    public int VerticalJumpSpeed;
    public LayerMask Terrain;
    public float CeilingDistance;

    private bool grounded = false;
    private bool FacingLeft;
    private bool knockedback = false;
    private Vector2 knockbackDirection;
    private float xInput;

    public float KnockbackPower;

    // Use this for initialization
    void Start()
    {
        rg = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }


    // Update is called once per frame
    void Update()
    {
        grounded = (rg.velocity.y == 0);


    }

    private void FixedUpdate()
    {
        //var xInput = (Input.GetAxis("Horizontal") * Input.GetAxis("Horizontal") > 0.5) ? Input.GetAxis("Horizontal") : 0;
        xInput = Input.GetAxis("Horizontal");
        if (xInput > -0.2 && xInput < 0.2)
            xInput = 0;
        var x = HorizontalSpeed * xInput;

        var origin = rg.position;
        RaycastHit2D topLeftHit = Physics2D.Raycast(new Vector2(origin.x - 0.4f, origin.y), Vector2.up, CeilingDistance, Terrain);
        RaycastHit2D topRightHit = Physics2D.Raycast(new Vector2(origin.x + 0.4f, origin.y), Vector2.up, CeilingDistance, Terrain);

        //var y = (Input.GetKey(KeyCode.UpArrow) && grounded) ? VerticalJumpSpeed : rg.velocity.y;

        float y = rg.velocity.y;

        if (!topLeftHit && !topRightHit)
            if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.Joystick1Button0) || Input.GetKey(KeyCode.Joystick1Button1)) && grounded)
                y = VerticalJumpSpeed;




        if (!knockedback)
        {
            rg.velocity = new Vector2(x, y);
        }

        //if (x == 0)
        //Debug.Log(Input.GetAxis("Horizontal"));


        //StopWallSticking();
        //StopCeilingSticking();

        // Restarts game
        if (Input.GetKey(KeyCode.Joystick1Button6))
            RestartGame();
    }


    private void LateUpdate()
    {
        FacingLeft = spriteRenderer.flipX;
        FlipSprite();

        // Stops animation when not walking on ground.
        GetComponent<Animator>().enabled = !(rg.velocity.x == 0 || rg.velocity.y != 0);
    }

    private void FlipSprite()
    {
        if (xInput > 0)
            spriteRenderer.flipX = false;
        if (xInput < 0)
            spriteRenderer.flipX = true;

        //if (rg.velocity.x != 0)
        //    if ((rg.velocity.x > 0 && spriteRenderer.flipX) || (rg.velocity.x < 0 && !spriteRenderer.flipX))
        //        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    private void StopWallSticking()
    {
        //var layer = LayerMask.NameToLayer("Terrain");
        var direction = (FacingLeft) ? Vector2.left : Vector2.right;
        Vector2 origin = new Vector2(rg.position.x + direction.x * 0.4f, rg.position.y);

        float distance = 0.1f;

        RaycastHit2D topCast = Physics2D.Raycast(origin + new Vector2(0f, 0.7f), direction, distance);
        RaycastHit2D midCast = Physics2D.Raycast(origin, direction, distance);
        RaycastHit2D bottomCast = Physics2D.Raycast(origin + new Vector2(0f, -0.75f), direction, distance);

        if (topCast.collider || midCast.collider || bottomCast.collider)
            if ((FacingLeft && rg.velocity.x < 0) || (!FacingLeft && rg.velocity.x > 0))
                rg.velocity = new Vector2(0, rg.velocity.y);

    }

    private void StopCeilingSticking()
    {
        Vector2 origin = rg.position;
        RaycastHit2D topLeftHit = Physics2D.Raycast(new Vector2(origin.x - 0.4f, origin.y), Vector2.up, CeilingDistance, Terrain);
        RaycastHit2D topRightHit = Physics2D.Raycast(new Vector2(origin.x + 0.4f, origin.y), Vector2.up, CeilingDistance, Terrain);

        if (topRightHit.collider != null || topLeftHit.collider != null)
        { // Did hit!
            grounded = false;
            rg.velocity = new Vector2(rg.velocity.x, 0);
            rg.AddForce(new Vector2(0, -5));
        }
    }

    private void OnTriggerEnter2D(Collider2D other) //Enemy interaction and damage
    {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 10) // 9 is Enemy Layer, 10 is Traps
        {
            if (transform.position.x - other.transform.position.x < 0)
                knockbackDirection = new Vector2(-1.0f, 0.3f).normalized;
            else
                knockbackDirection = new Vector2(1.0f, 0.3f).normalized;
            knockbackDirection *= KnockbackPower;
            rg.velocity = new Vector2(0f, 0f);
            rg.AddForce(knockbackDirection * 500);
            StartCoroutine("KnockbackTimer");
        }

    }

    IEnumerator KnockbackTimer()
    {
        knockedback = true;
        yield return new WaitForSeconds(0.4f);
        knockedback = false;
    }

    private void RestartGame()
    {

        SceneManager.LoadScene("Level 1");
    }

}
