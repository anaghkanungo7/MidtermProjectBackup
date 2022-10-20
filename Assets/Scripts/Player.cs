using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Movement
    public int speed = 5;
    public Rigidbody2D rb;
    public Transform feet;
    public LayerMask groundLayer;
    public float jumpForce = 500f;
    public float doubleJumpForce = 250f;
    public bool grounded;
    public int extraJumps = 1;
    public float radius = 0.3f;

    // New Input System
    private PlayerControls controls;
    Vector2 move;

    // Combat
    public double endLag;
    public Transform[] hitboxes;
    public float[] hitboxSizes;
    public Vector2[] knockbacks;
    Vector2 kb;
    int direction;

    private void Awake() {
        controls = new PlayerControls();
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (endLag <= 0 || grounded == false) {
            move = controls.Player.Move.ReadValue<Vector2>();
        }

        if (endLag <= 0) {
            grounded = Physics2D.OverlapCircle(feet.position, radius, groundLayer);
            //controls.Player.Jump.ReadValue<float>();
            if (controls.Player.Jump.triggered) {
                if (grounded) {
                    rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
                } else {
                    if (extraJumps > 0) {
                        extraJumps -= 1;
                        rb.AddForce(new Vector2(rb.velocity.x, doubleJumpForce));
                    }
                }
            }
            if (grounded) {
                extraJumps = 1;
            }

            if(controls.Player.NormalAttack.triggered) {
                if (grounded) { // Grounded attacks
                    if (move.y > .5) {
                        Debug.Log("Up tilt");
                    } else if (move.y < -.5) {
                        Debug.Log("Down tilt");
                    } else if (move.x > .2 || move.x < -.2) {
                        Debug.Log("Side tilt");
                    } else {
                        endLag = 0.5;
                        Debug.Log("Jab");
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        Collider2D[] colliders = Physics2D.OverlapCircleAll(hitboxes[0].position, hitboxSizes[0]);
                        foreach (Collider2D nearby in colliders) {
                            if (nearby.tag == "Player") {
                                Rigidbody2D enemyRB = nearby.GetComponent<Rigidbody2D>();
                                //Vector2 direction = (nearby.transform.position - transform.position).normalized;
                                if (enemyRB != null) {
                                    kb = new Vector2(knockbacks[0].x * direction, knockbacks[0].y);
                                    enemyRB.AddForce(kb);
                                    //enemyRB.AddForce(knockbacks[0]);
                                }
                            }
                        }
                    }
                } else { // Aerial attacks
                    if (move.y > .5) {
                        Debug.Log("Up Air");
                    } else if (move.y < -.5) {
                        Debug.Log("Down Air");
                    } else if (move.x * direction > 0.2) {
                        Debug.Log("Forward Air");
                    } else if (move.x * direction < -0.2) {
                        Debug.Log("Back Air");
                    } else {
                        Debug.Log("Neutral Air");
                    }
                }
            }
        }
    }


    void FixedUpdate() {
        if (endLag <= 0) {
            rb.velocity = new Vector2(move.x * speed, rb.velocity.y);
        }
        if (move.x < -0.5 && grounded) { //facing left on ground
            gameObject.transform.localScale = new Vector3(-1,1,1);
            direction = -1;
        }
        if (move.x > 0.5 && grounded) { //facing right on ground
            gameObject.transform.localScale = new Vector3(1,1,1);
            direction = 1;
        }
    }


    void LateUpdate()
    {
        if (endLag > 0) {
            endLag -= Time.deltaTime;
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(hitboxes[0].position, hitboxSizes[0]);
    }
}
