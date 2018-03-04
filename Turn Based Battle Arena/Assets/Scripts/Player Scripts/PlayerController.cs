using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    public int hp;
    public float movementSpeed;
    public float jumpStrength;
    public float bulletStrength;
    
    public LayerMask groundLayer;

    public GameObject bullet;

    private BattleStateManager manager;
    private Rigidbody2D rb2d;
    private Transform arm;
    private bool activeCharacter;
    private bool grounded;
    private bool isPlayer1;
    private SpriteRenderer sprite;

    private Text playerUI;

    private void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<BattleStateManager>();

        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 0;

        activeCharacter = false;
        grounded = false;

        arm = transform.GetChild(0);
        sprite = GetComponent<SpriteRenderer>();
        playerUI = GetComponentInChildren<Text>();
    }

    private void Update()
    {
        playerUI.color = sprite.color;
        playerUI.text = "HP: " + hp;

        if (activeCharacter && Time.timeScale != 0)
        {
            FaceMouse();

            if (Input.GetMouseButtonDown(0) && !manager.shot)
            {
                Fire();
            }

            if (Input.GetKey(KeyCode.Space) && grounded)
            {
                rb2d.AddForce(Vector2.up * jumpStrength * 100);
            }
        } else if (hp <= 0 && Time.timeScale != 0)
        {
            manager.deleteCharacter(isPlayer1, this);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (activeCharacter && manager.movementLimit > 0f)
        {
            Move();
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

        if (movement.x > 0f)
        {
            manager.movementLimit -= movement.x;
        } else
        {
            manager.movementLimit += movement.x;
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

        manager.shot = true;
    }

    public void gameStart(bool player1)
    {
        rb2d.gravityScale = 1f;
        isPlayer1 = player1;
    }

    public void activateCharacter()
    {
        activeCharacter = true;
    }

    public void deactivateCharacter()
    {
        activeCharacter = false;
    }

    public void doDamage(int dmg)
    {
        hp -= dmg;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Walls")
        {
            grounded = true;
        }

        if (collision.gameObject.tag == "Lava")
        {
            manager.deleteCharacter(isPlayer1, this);
            Destroy(gameObject);
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Walls")
        {
            grounded = false;
        }
    }
}
