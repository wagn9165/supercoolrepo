using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is given to the Player prefab////////////////////////////////////
public class PlayerScript : MonoBehaviour
{
    //Properites
    public float maxHealth = 50; //Total HP
    float health;
    public GameObject healthbar;
    Animator healthbarAnim;
    public float moveSpeed = 5f; //Determines player speed
    private SpriteRenderer selfsort; //Renders player sprite - used to read player's layer

    public GameObject Hand;

    //Player Weapon
    GameObject weapon; //This contains the weapon object
    public GameObject weapon0; //This contains the weapon object
    public GameObject weapon1; //This contains the weapon object
    private SpriteRenderer sort; //This will render the weapon sprite
    float weaponNum = 0;
    GunScript weaponScript;
    private SpriteRenderer weaponRender;
    private SpriteRenderer render;
    Vector3 gunFlip; //This will be used to determine if the gun should be flipped (if the player is looking left)

    //Conditions
    int hit = 0; //acts as a counter to turn red, then back to normal
    float movementx, movementy; //These variables determine the direction the player is looking
    public bool invincible; //This determines of the player is invincible

    //Links player with rigibody 2D physics
    public Rigidbody2D rb; //This contains the player's rigidbody
    public Animator animator; //This contains the player's animator, which contains the players animations

    //PlayerMovement
    public Vector2 movement; //This will be used to determine the players movement (x and y movement)
    Vector3 mouse_position; //This will be used to determine the mouse's coordinates

    //Flashlight
    bool flashlightOn = false;

    //Abilities
    public bool dash = false;

    // Start is called before the first frame update
    void Start()
    {
        //Create gun
        weapon = Instantiate(weapon0, transform.position, Quaternion.identity) as GameObject;
        weaponScript = weapon.GetComponent<GunScript>();
        weaponScript.parent = gameObject;
        weaponScript.Hand = Hand;
        weaponRender = weapon.GetComponent<SpriteRenderer>();
        render = GetComponent<SpriteRenderer>();

        //Healthbar
        healthbar = Instantiate(healthbar, transform.position, Quaternion.identity);
        healthbarAnim = healthbar.GetComponent<Animator>();
        health = maxHealth;

        invincible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Turn on/off flashlight
        if (Input.GetKeyDown(KeyCode.F) && flashlightOn == false)
        {
            flashlightOn = true;
        }
        else if (Input.GetKeyDown(KeyCode.F) && flashlightOn == true)
        {
            flashlightOn = false;
        }

        if (weapon)
        {
            weaponScript = weapon.GetComponent<GunScript>();
            weaponScript.flashlightOn = flashlightOn;
        }

        if (!Global.shopActive)
        {
            //Change Weapon
            if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0)
            {
                if (weapon)
                {
                    if (weaponNum == 0)
                    {
                        weapon.GetComponent<GunScript>().destroy = true;
                        weapon = Instantiate(weapon1, transform.position, Quaternion.identity) as GameObject;
                        weaponScript = weapon.GetComponent<GunScript>();
                        weaponScript.parent = gameObject;
                        weaponScript.Hand = Hand;
                        weaponRender = weapon.GetComponent<SpriteRenderer>();
                        render = GetComponent<SpriteRenderer>();
                        weaponNum = 1;
                    }
                    else if (weaponNum == 1)
                    {
                        weapon.GetComponent<GunScript>().destroy = true;
                        weapon = Instantiate(weapon0, transform.position, Quaternion.identity) as GameObject;
                        weaponScript = weapon.GetComponent<GunScript>();
                        weaponScript.parent = gameObject;
                        weaponScript.Hand = Hand;
                        weaponRender = weapon.GetComponent<SpriteRenderer>();
                        render = GetComponent<SpriteRenderer>();
                        weaponNum = 0;
                    }
                }
            }
        }

        //Healthbar Animation
        healthbar.transform.position = transform.position + new Vector3(0, 1f, 0);
        healthbarAnim.SetFloat("Health", health / maxHealth);

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

        //If hit, wait then turn back to normal
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

        if (!Global.shopActive)
        {
            selfsort = gameObject.GetComponent<SpriteRenderer>(); //Used to find sorting order

            mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition); //This is the mouse position

            if (!dash)
            {
                //Input
                movement.x = Input.GetAxisRaw("Horizontal"); //Input for x movement
                movement.y = Input.GetAxisRaw("Vertical"); //Input for y movement
            }

            movementx = mouse_position.x - transform.position.x; //Gets x direction from mouse
            movementy = mouse_position.y - transform.position.y; //Gets y direction from mouse

            //Setting Animation Values
            animator.SetFloat("Horizontal", movementx); //Moves based on mouse.x position, not movement
            animator.SetFloat("Vertical", movementy); //Moves based on mouse.y position, not movement
            animator.SetFloat("Speed", movement.sqrMagnitude); //Give Animator Speed, player speed

            animator.SetFloat("Latitude", movementx); //This determines last direction for idle position
            animator.SetFloat("Longitude", movementy); //This determines last direction for idle position

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
                    offset_right = new Vector3(0.45f, -0.5f, 0);//new Vector3(0.5f, -0.5f, 0); //Offset for the weapon on the right hand
                    offset_left = new Vector3(-0.45f, -0.5f, 0);//new Vector3(-0.5f, -0.5f, 0); //Offset for the weapon on the left hand
                }

                float rotation;

                //If player is moving right,
                if (Mathf.Sign(movementx) > 0)
                {
                    //Rotate the gun towards the mouse
                    rotation = Mathf.Atan2((mouse_position.y - 0.3f) - weapon.transform.position.y, mouse_position.x - weapon.transform.position.x) * Mathf.Rad2Deg;
                    weapon.transform.rotation = Quaternion.Euler(0, 0, rotation); //Apply the rotation to the gun

                    //Offset gun position
                    weapon.transform.position = transform.position + offset_right;
                }
                else
                {
                    //Rotate the gun towards the mouse
                    rotation = Mathf.Atan2(weapon.transform.position.y - (mouse_position.y - 0.3f), weapon.transform.position.x - mouse_position.x) * Mathf.Rad2Deg;
                    weapon.transform.rotation = Quaternion.Euler(0, 0, rotation); //Apply the rotation to the gun

                    //Offset gun position
                    weapon.transform.position = transform.position + offset_left;
                }

                //Weapon sorting layer
                sort = weapon.GetComponent<SpriteRenderer>();

                if (movementy > Mathf.Abs(movementx))
                {
                    sort.sortingOrder = selfsort.sortingOrder - 5;
                    weaponScript.layerOrder = 1;
                }
                else
                {
                    sort.sortingOrder = selfsort.sortingOrder + 5;
                    weaponScript.layerOrder = -1;
                }

                gunFlip = weapon.transform.localScale;
                gunFlip.x = Mathf.Sign(movementx);

                weapon.transform.localScale = gunFlip;
            }
        }
        else
        {
            //Completely stops the players
            movement.x = 0f;
            movement.y = 0f;
            animator.SetFloat("Speed", 0);
        }
    }

    void FixedUpdate()
    {
        //Movement
        rb.MovePosition(rb.position + (movement * moveSpeed * Time.fixedDeltaTime));
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Bullet Collision
        if (col.gameObject.tag == "EnemyBullet" && invincible == false)
        {
            BulletScript bulletScript = col.GetComponent<BulletScript>();

            health -= bulletScript.bulletDamage;
            bulletScript.passThrough--;
            GetComponent<SpriteRenderer>().color = Color.red;

            hit = 100;
            invincible = true;
        }
        if (col.gameObject.tag == "Hazard" && invincible == false)
        {
            explosionScript explosion = col.GetComponent<explosionScript>();

            health -= explosion.damage;
            GetComponent<SpriteRenderer>().color = Color.red;

            hit = 100;
            invincible = true;
        }
    }
}
