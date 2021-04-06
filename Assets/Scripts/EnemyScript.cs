using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//This script is given to the Enemy prefab////////////////////////////////////
public class EnemyScript : MonoBehaviour
{
    private SpriteRenderer sort;
    float deltaTime;

    int hit = 0;
    bool dead = false;

    bool invincible = false;

    public int startDelay;

    public float maxHealth;
    float health;
    public GameObject healthbar;
    Animator healthbarAnim;

    public GameObject Hand;
    Vector2 playerPosition;
    //Enemy Weapon
    public GameObject weapon;
    private SpriteRenderer selfsort;
    Vector3 gunFlip;

    //Determines enemy speed
    public float moveSpeed = 5f;

    //Links player with rigibody 2D physics
    public Rigidbody2D rb;
    public Animator animator;

    float movementx, movementy;

    Vector2 movement;
    Vector3 player_position;
    Vector3 dir;

    public bool shoot = false;
    GunScript weaponScript;

    //Follow player
    GameObject player;
    SpriteRenderer weaponRender;
    SpriteRenderer render;

    GameObject EnemyParent;

    bool playerSeen = false;
    float speed;

    // Start is called before the first frame update
    void Start()
    {
        //Create gun
        weapon = Instantiate(weapon, transform.position, Quaternion.identity) as GameObject;
        weaponScript = weapon.GetComponent<GunScript>();
        weapon.tag = "EnemyWeapon";
        weaponScript.parent = gameObject;
        weaponScript.Hand = Hand;
        weaponRender = weapon.GetComponent<SpriteRenderer>();
        render = GetComponent<SpriteRenderer>();

        //AI Pathing
        player = GameObject.FindWithTag("Player");
        playerSeen = false;

        //Healthbar
        healthbar = Instantiate(healthbar, transform.position, Quaternion.identity);
        healthbarAnim = healthbar.GetComponent<Animator>();
        health = maxHealth;

        //Parent
        EnemyParent = GameObject.Find("TempEnemies");
        transform.parent = EnemyParent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //Healthbar Animation
        healthbar.transform.position = transform.position + new Vector3(0, 1f, 0);
        healthbarAnim.SetFloat("Health", health / maxHealth);

        //AI Pathing
        player = GameObject.FindWithTag("Player");

        if (player && playerSeen == true && (Vector2.Distance(transform.position, player.transform.position) > 5 || shoot == false))// && (GetComponent<SpriteRenderer>().isVisible))
        {
            GetComponent<AIPath>().destination = player.transform.position + new Vector3(0, -0.5f, 0);
            dir = GetComponent<AIPath>().desiredVelocity;
            speed = 1;
        }
        else
        {
            GetComponent<AIPath>().destination = transform.position;
            speed = 0;
        }

        //Check if dead
        if (health <= 0)
        {
            if (weapon)
            {
                weaponScript.destroy = true;
            }
            if (healthbar)
            {
                Destroy(healthbar);
            }
            Destroy(gameObject);
        }

        selfsort = gameObject.GetComponent<SpriteRenderer>(); //Used to find sorting order

        if (player)
        {
            playerPosition = player.transform.position; //This is the mouse position
        }

        //Input
        if (playerSeen == true)
        {
            
            if (shoot == true) //If enemy can see player
            {
                movementx = playerPosition.x - transform.position.x; //Gets x direction from mouse
                movementy = playerPosition.y - transform.position.y; //Gets y direction from mouse
            }
            else //If enemy can't see player
            {
                movementx = dir.x;
                movementy = dir.y;
            }

            //Setting Animation Values
            animator.SetFloat("Horizontal", movementx); //Moves based on mouse.x position, not movement
            animator.SetFloat("Vertical", movementy); //Moves based on mouse.y position, not movement

            animator.SetFloat("Speed", speed); //Give Animator Speed, player speed

            animator.SetFloat("Latitude", movementx); //This determines last direction for idle position
            animator.SetFloat("Longitude", movementy); //This determines last direction for idle position
        }

        //Weapon point system
        if (weapon)
        {
            float width = weaponRender.bounds.extents.x / 2; //Distance to the right side, from your center point
            Vector3 offset_right;
            Vector3 offset_left;

            if (weaponScript.twoHanded == true)
            {
                //If looking up
                if (movementy > Mathf.Abs(movementx))
                {
                    offset_right = new Vector3(0.2f, 0f, 0);//new Vector3(0.5f, -0.5f, 0); //Offset for the weapon on the right hand
                    offset_left = new Vector3(-0.2f, 0f, 0);//new Vector3(-0.5f, -0.5f, 0); //Offset for the weapon on the left hand
                }
                else
                {
                    offset_right = new Vector3(0.2f, -0.5f, 0);//new Vector3(0.5f, -0.5f, 0); //Offset for the weapon on the right hand
                    offset_left = new Vector3(-0.2f, -0.5f, 0);//new Vector3(-0.5f, -0.5f, 0); //Offset for the weapon on the left hand
                }
            }
            else
            {
                offset_right = new Vector3(0.5f, -0.5f, 0);//new Vector3(0.5f, -0.5f, 0); //Offset for the weapon on the right hand
                offset_left = new Vector3(-0.5f, -0.5f, 0);//new Vector3(-0.5f, -0.5f, 0); //Offset for the weapon on the left hand
            }

            float rotation;

            //If player is moving right,
            if (Mathf.Sign(movementx) > 0)
            {
                //Rotate the gun towards the player
                if (playerSeen == true)
                {
                    if (shoot == true) //If enemy can see player
                    {
                        rotation = Mathf.Atan2((playerPosition.y - 1f) - weapon.transform.position.y, playerPosition.x - weapon.transform.position.x) * Mathf.Rad2Deg;
                        weapon.transform.rotation = Quaternion.Euler(0, 0, rotation); //Apply the rotation to the gun
                    }
                    else //If enemy can't see player
                    {
                        rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        weapon.transform.rotation = Quaternion.Euler(0, 0, rotation); //Apply the rotation to the gun
                    }
                }

                //Offset gun position
                weapon.transform.position = transform.position + offset_right;
            }
            else
            {
                //Rotate the gun towards the player
                if (playerSeen == true)
                {
                    if (shoot == true) //If enemy can see player
                    {
                        rotation = Mathf.Atan2(weapon.transform.position.y - (playerPosition.y - 1f), weapon.transform.position.x - playerPosition.x) * Mathf.Rad2Deg;
                        weapon.transform.rotation = Quaternion.Euler(0, 0, rotation); //Apply the rotation to the gun
                    }
                    else //If enemy can't see player
                    {
                        rotation = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
                        weapon.transform.rotation = Quaternion.Euler(0, 0, rotation); //Apply the rotation to the gun
                    }
                }

                //Offset gun position
                weapon.transform.position = transform.position + offset_left;
            }

            //Weapon sorting layer
            sort = weapon.GetComponent<SpriteRenderer>();

            if (movementy > Mathf.Abs(movementx))
            {
                //sort.sortingLayerName = "Weapons_Behind";
                sort.sortingOrder = selfsort.sortingOrder - 2;
                weaponScript.layerOrder = 1;
            }
            else
            {
                //sort.sortingLayerName = "Weapons_Front";
                sort.sortingOrder = selfsort.sortingOrder + 2;
                weaponScript.layerOrder = -1;
            }

            gunFlip = weapon.transform.localScale;
            gunFlip.x = Mathf.Sign(movementx);

            weapon.transform.localScale = gunFlip;

            //Debug.Log(transform.position.y);
        }
    }

    void FixedUpdate()
    {
        //If hit, wait then turn back to normal (blink red and transparent)
        if (hit > 0)
        {
            if (Mathf.FloorToInt(hit / 8) % 2 == 0)
            {
                render.color = new Color(0, 0, 0, 0);
                if (weapon)
                    weaponRender.color = new Color(0, 0, 0, 0);
            }
            else
            {
                render.color = Color.red;
                if (weapon)
                    weaponRender.color = Color.red;
            }

            hit--;
            if (hit <= 0)
            {
                render.color = Color.white;
                if (weapon)
                    weaponRender.color = Color.white;
                invincible = false;
            }
        }

        //Check if able to shoot at player
        if (player)
        {
            shoot = true;

            Vector2 enemyPoint = new Vector2(transform.position.x, transform.position.y);
            Vector2 playerPoint = new Vector2(player.transform.position.x, player.transform.position.y - 0.5f);

            RaycastHit2D[] hit = Physics2D.LinecastAll(enemyPoint, playerPoint);
            int arrayLength = hit.Length;

            //If something was hit, the RaycastHit2D.collider will not be null
            for (int i = 0; i < arrayLength; i++)
            {
                if (hit[i].collider != null)
                {
                    //if there is a wall in between the player and the enemy, it will not shoot
                    if (hit[i].collider.tag == "Collision")
                    {
                        //Debug.Log(hit[i].collider.name);
                        shoot = false;
                    }
                }
            }

            if (shoot == true)
                startDelay--;

            if (startDelay > 0)
            {
                shoot = false;
            }

            if (invincible == true)
            {
                shoot = false;
            }

            if (shoot == true)
                playerSeen = true;
        }
        else
        {
            shoot = false;
        }

        //Movement
        rb.MovePosition(rb.position + (movement * moveSpeed * Time.fixedDeltaTime));
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Bullet" && invincible == false)
        {
            BulletScript bulletScript = col.GetComponent<BulletScript>();

            health -= bulletScript.bulletDamage;
            bulletScript.passThrough--;
            GetComponent<SpriteRenderer>().color = Color.red;


            if (dead == false)
                hit = 50;
            //invincible = true;
        }
        if (col.gameObject.tag == "Hazard" && invincible == false)
        {
            explosionScript explosion = col.GetComponent<explosionScript>();

            health -= explosion.damage;
            GetComponent<SpriteRenderer>().color = Color.red;

            if (dead == false)
                hit = 50;
            //invincible = true;
        }
    }
}