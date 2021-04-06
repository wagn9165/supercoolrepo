using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is given to the ALL Bullet prefabs////////////////////////////////////
public class BulletScript : MonoBehaviour
{
    [HideInInspector]
    public float bulletAngle;
    [HideInInspector]
    public float angleTemp;
    [HideInInspector]
    public float bulletDamage;
    [HideInInspector]
    public float bulletSpeed;
    [HideInInspector]
    public float bulletRange;
    [HideInInspector]
    public float passThrough;

    public GameObject ricochetCollision;
    GameObject ricochetLeft;
    GameObject ricochetRight;
    GameObject ricochetUp;
    GameObject ricochetDown;

    [HideInInspector]
    public float deltaTime;

    [HideInInspector]
    public string sortingLayer;

    Vector3 lastPosition;
    Vector3 currentPosition;

    Rigidbody2D rb;

    private SpriteRenderer sort;

    Vector2 movement;

    public float ricochet;
    int signX;
    int signY;

    float initialX;
    float initialY;

    SpriteRenderer sprite;

    float width;
    float height;

    bool left = true, right = true, up = true, down = true;

    int live; //REMOVE
    Vector3 sizeOffset;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //Debug.Log(bulletAngle);
        if (passThrough == 0)
            passThrough = 1;
        
        lastPosition = transform.position;
        
        //Sorting Layer
        sort = GetComponent<SpriteRenderer>();
        //sort.sortingLayerName = sortingLayer;

        movement.x = signX * Mathf.Sin(-bulletAngle - (Mathf.PI / 2));
        movement.y = signY * Mathf.Cos(-bulletAngle - (Mathf.PI / 2));

        signY = 1;
        signX = 1;

        initialX = signX * Mathf.Sin(-bulletAngle - (Mathf.PI / 2));
        initialY = signY * Mathf.Cos(-bulletAngle - (Mathf.PI / 2));

        if (ricochet > 0)
        {
            ricochetLeft = Instantiate(ricochetCollision, new Vector3(0, 0, 0), Quaternion.identity);
            ricochetRight = Instantiate(ricochetCollision, new Vector3(0, 0, 0), Quaternion.identity);
            ricochetUp = Instantiate(ricochetCollision, new Vector3(0, 0, 0), Quaternion.identity);
            ricochetDown = Instantiate(ricochetCollision, new Vector3(0, 0, 0), Quaternion.identity);

            GameObject TempRicochet = GameObject.Find("TempRicochet");
            ricochetLeft.transform.parent = TempRicochet.transform;
            ricochetRight.transform.parent = TempRicochet.transform;
            ricochetUp.transform.parent = TempRicochet.transform;
            ricochetDown.transform.parent = TempRicochet.transform;

            sizeOffset = ricochetCollision.transform.localScale;

            ricochet++;
        }

        sprite = GetComponent<SpriteRenderer>(); //Set the reference to our SpriteRenderer component
        width = sprite.bounds.extents.x; //Distance to the right side * 2, from your center point
        height = sprite.bounds.extents.y; //Distance to the top * 2, from your center point
    }

    // Update is called once per frame
    void Update()
    {
        if (passThrough <= 0)
        {
            Destroy(ricochetLeft);
            Destroy(ricochetRight);
            Destroy(ricochetUp);
            Destroy(ricochetDown);
            Destroy(gameObject);
        }

        if (ricochet > 0)
        {
            sizeOffset.x = Mathf.Abs(movement.x * -bulletSpeed) / 10;
            sizeOffset.y = Mathf.Abs(movement.y * -bulletSpeed) / 10;

            if (sizeOffset.y < 0.1)
            {
                sizeOffset.y = 0.1f;
            }
            if (sizeOffset.x < 0.1)
            {
                sizeOffset.x = 0.1f;
            }

            ricochetLeft.transform.localScale = sizeOffset;
            ricochetRight.transform.localScale = sizeOffset;
            ricochetUp.transform.localScale = sizeOffset;
            ricochetDown.transform.localScale = sizeOffset;

            ricochetLeft.transform.position = transform.position + new Vector3(-width - 0.2f, 0, 0);
            ricochetRight.transform.position = transform.position + new Vector3(width + 0.2f, 0, 0);
            ricochetUp.transform.position = transform.position + new Vector3(0, height + 0.2f, 0);
            ricochetDown.transform.position = transform.position + new Vector3(0, -height - 0.2f, 0);
        }

        //Debug.Log((width + 0.2f) + ", " + (height + 0.2f));

        //Change angle
        var angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(180 + angle, Vector3.forward);

        if (ricochet > 0)
        {
            if (ricochetLeft.GetComponent<RicochetScript>().collided && left == true)
            {
                signX = Mathf.RoundToInt(-1 * Mathf.Sign(initialX));

                ricochet--;

                if (ricochet <= 0)
                {
                    Destroy(ricochetLeft);
                    Destroy(ricochetRight);
                    Destroy(ricochetUp);
                    Destroy(ricochetDown);
                    Destroy(gameObject);
                }

                left = false;
                right = true;
                up = true;
                down = true;
            }
            else if (ricochetRight.GetComponent<RicochetScript>().collided && right == true)
            {
                signX = Mathf.RoundToInt(1 * Mathf.Sign(initialX));

                ricochet--;

                if (ricochet <= 0)
                {
                    Destroy(ricochetLeft);
                    Destroy(ricochetRight);
                    Destroy(ricochetUp);
                    Destroy(ricochetDown);
                    Destroy(gameObject);
                }

                left = true;
                right = false;
                up = true;
                down = true;
            }
            else if (ricochetDown.GetComponent<RicochetScript>().collided && down == true)
            {
                signY = Mathf.RoundToInt(-1 * Mathf.Sign(initialY));

                ricochet--;

                if (ricochet <= 0)
                {
                    Destroy(ricochetLeft);
                    Destroy(ricochetRight);
                    Destroy(ricochetUp);
                    Destroy(ricochetDown);
                    Destroy(gameObject);
                }

                left = true;
                right = true;
                up = true;
                down = false;
            }
            else if (ricochetUp.GetComponent<RicochetScript>().collided && up == true)
            {
                signY = Mathf.RoundToInt(1 * Mathf.Sign(initialY));

                ricochet--;

                if (ricochet <= 0)
                {
                    Destroy(ricochetLeft);
                    Destroy(ricochetRight);
                    Destroy(ricochetUp);
                    Destroy(ricochetDown);
                    Destroy(gameObject);
                }

                left = true;
                right = true;
                up = false;
                down = true;
            }
        }

        //Sorting Order
        sort.sortingOrder = 100000 - Mathf.RoundToInt(transform.position.y) - Mathf.RoundToInt(height)*2;

        //Current position
        currentPosition = transform.position;

        //bulletAngle = angleTemp * (Mathf.PI/180);
        movement.x = signX * Mathf.Sin(-bulletAngle - (Mathf.PI / 2));
        movement.y = signY * Mathf.Cos(-bulletAngle - (Mathf.PI / 2));
        //Debug.Log(bulletAngle);

        if (Vector2.Distance(lastPosition, currentPosition) > bulletRange)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Collision" && ricochet == 0)
        {
            Destroy(gameObject);
        }
        if (collision.tag == "Hazard" && ricochet == 0)
        {
            Destroy(gameObject);
        }
    }


    void FixedUpdate()
    {
        //Movement
        rb.MovePosition(rb.position + (movement * -bulletSpeed * Time.fixedDeltaTime));
    }
}