using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public int hp;
    public float movementSpeed;
    public float jumpStrength;
    public float bulletStrength;
    public float setMoveLimit;
    
    public LayerMask groundLayer;

    public GameObject bullet;

    private Rigidbody2D rb2d;
    private Transform arm;
    private bool currentTurn;
    private bool grounded;
    private bool shot;
    private float moveLimit;
    private bool isPlayer1;
    private SpriteRenderer sprite;

    private Text playerUI;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 0;
        
        currentTurn = false;
        grounded = false;
        shot = false;
        moveLimit = setMoveLimit;

        arm = transform.GetChild(0);
        sprite = GetComponent<SpriteRenderer>();
        playerUI = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        playerUI.color = sprite.color;
        playerUI.text = "HP: " + hp;

        if (currentTurn && Time.timeScale != 0)
        {
            checkGrounded();

            FaceMouse();

            if (Input.GetMouseButtonDown(0) && !shot)
            {
                Fire();
            }
        } else if (hp <= 0 && Time.timeScale != 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (currentTurn)
        {
            if (Input.GetKeyDown(KeyCode.Space) && grounded)
            {
                rb2d.AddForce(transform.up * jumpStrength * 100);
            }

            if (moveLimit > 0)
            {
                Move();
            }
        }
    }

    private void Move()
    {
        Vector3 input = Vector3.zero;

        if (Input.GetKey(KeyCode.D))
        {
            input += Vector3.right;
        }

        if (Input.GetKey(KeyCode.A))
        {
            input += Vector3.left;
        }

        input.Normalize();

        Vector2 movement = input * movementSpeed * Time.deltaTime;

        rb2d.position += movement;
        if (movement.x > 0)
        {
            moveLimit -= movement.x;
        } else
        {
            moveLimit += movement.x;
        }
    }

    private void FaceMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        arm.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
    }

    private void Fire()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseDir = mousePos - transform.position;

        mouseDir.z = 0f;
        mouseDir = mouseDir.normalized;

        GameObject newBullet = Instantiate(bullet, transform.position, new Quaternion(0, 0, 0, 0));
        if (isPlayer1)
        {
            newBullet.layer = 9;
        } else
        {
            newBullet.layer = 10;
        }
        newBullet.GetComponent<SpriteRenderer>().color = sprite.color;
        newBullet.GetComponent<Rigidbody2D>().AddForce(mouseDir * bulletStrength * 1000);

        shot = true;
    }

    private void checkGrounded()
    {
        RaycastHit2D downRay = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);
        RaycastHit2D halfDownRay1 = Physics2D.Raycast(transform.position + (Vector3.left * 3 / 4), Vector3.down, 0.75f, groundLayer);
        RaycastHit2D halfDownRay2 = Physics2D.Raycast(transform.position + (Vector3.right * 3 / 4), Vector3.down, 0.75f, groundLayer);

        if (downRay.collider != null || halfDownRay1.collider != null || halfDownRay2.collider != null)
        {
            grounded = true;
        } else
        {
            grounded = false;
        }
    }

    public void gameStart(bool player1)
    {
        rb2d.gravityScale = 1f;
        isPlayer1 = player1;
    }

    public void startTurn()
    {
        currentTurn = true;
        shot = false;
        moveLimit = setMoveLimit;
    }

    public void endTurn()
    {
        currentTurn = false;
        shot = false;
    }

    public bool didShoot()
    {
        return shot;
    }

    public float getMoveLimit()
    {
        if (moveLimit > 0f)
        {
            return moveLimit;
        }  else
        {
            return 0f;
        }
    }

    public void doDamage(int dmg)
    {
        hp -= dmg;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Lava")
        {
            Destroy(gameObject);
        }
    }
}
