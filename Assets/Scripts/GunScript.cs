using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is given to ALL Gun prefabs////////////////////////////////////
public class GunScript : MonoBehaviour
{
    SoundHandler soundHandler;

    public GameObject Bullet;
    [Range(-2, 2)]
    public float bulletOffsetX;
    [Range(-2, 2)]
    public float bulletOffsetY;
    [HideInInspector]
    public GameObject Hand;
    [Range(-2, 2)]
    public float handOffsetX;
    [Range(-2, 2)]
    public float handOffsetY;
    GameObject leftHand;
    GameObject rightHand;
    Renderer leftHandRender;
    Renderer rightHandRender;

    [HideInInspector]
    public GameObject muzzleFlash;
    //[Range(-2, 2)]
    [HideInInspector]
    public float muzzleFlashOffsetX;
    //[Range(-2, 2)]
    [HideInInspector]
    public float muzzleFlashOffsetY;

    public GameObject flashlight;
    [Range(-2, 2)]
    public float flashlightOffsetX;
    [Range(-2, 2)]
    public float flashlightOffsetY;
    private ScreenShake Shaker;
    [Range(0, 1)]
    public float screenShakeDuration;
    [Range(1, 100)]
    public float screenShakeIntensity;
    public bool twoHanded;
    public bool automaticAssault;
    [Range(0, 50)]
    public float automaticDelay;
    [Range(0, 100)]
    public int ricochetAmount;
    [Range(1, 100)]
    public int passThroughAmount;
    [Range(1, 100)]
    public float bulletDamage;
    [Range(1, 100)]
    public float bulletSpeed;
    [Range(1, 100)]
    public float bulletRange;
    [Range(1, 100)]
    public int bulletSpread;
    [Range(0, 100)]
    public float bulletSpreadRandom;

    //Type of weapon

    float tempAssault = 0;

    float width;
    float height;
    float angle_width;
    float angle_height;
    float offsetx_width = 0;
    float offsety_width = 0;
    float offsetx_height = 0;
    float offsety_height = 0;
    float pos_x;
    float pos_y;
    float sign;
    bool shoot = false;
    bool shootCheck;

    float offset_angle = 0;

    private SpriteRenderer sprite;
    //public Sprite bulletImage;

    [HideInInspector]
    public GameObject parent;

    float semi_delay = 100; //AI semi-automatic delay
    float semi_temp = 100;

    [HideInInspector]
    public int layerOrder = -1;

    [HideInInspector]
    public bool destroy = false;

    //Flashlight
    [HideInInspector]
    public bool flashlightOn = false;

    GameObject light;

    // Start is called before the first frame update
    void Start()
    {
        soundHandler = GetComponent<SoundHandler>();

        if (passThroughAmount == 0)
            passThroughAmount = 1;

        //Hands
        leftHand = Instantiate(Hand, new Vector3(0, 0, 0), transform.rotation);
        rightHand = Instantiate(Hand, new Vector3(0, 0, 0), transform.rotation);
        leftHandRender = leftHand.GetComponent<SpriteRenderer>();
        rightHandRender = rightHand.GetComponent<SpriteRenderer>();

        //Size of weapon
        sprite = GetComponent<SpriteRenderer>(); //Set the reference to our SpriteRenderer component
        width = sprite.bounds.extents.x * 2; //Distance to the right side * 2, from your center point
        height = sprite.bounds.extents.y * 2; //Distance to the top * 2, from your center point

        Shaker = Object.FindObjectOfType<ScreenShake>();

        pos_x = transform.position.x;
        pos_y = transform.position.y;

        if (Mathf.Sign(transform.localScale.x) > 0)
        {
            sign = 1;

            angle_width = transform.rotation.eulerAngles.z * (Mathf.PI / 180);
            angle_height = (90 + transform.rotation.eulerAngles.z) * (Mathf.PI / 180);

            offsetx_width = Mathf.Cos(angle_width);
            offsety_width = Mathf.Sin(angle_width);

            offsetx_height = Mathf.Cos(angle_height);
            offsety_height = Mathf.Sin(angle_height);
        }
        else
        {
            sign = -1;

            angle_width = (transform.rotation.eulerAngles.z + 180) * (Mathf.PI / 180);
            angle_height = (90 + (transform.rotation.eulerAngles.z + 180)) * (Mathf.PI / 180);

            offsetx_width = Mathf.Cos(angle_width);
            offsety_width = Mathf.Sin(angle_width);

            offsetx_height = Mathf.Cos(angle_height);
            offsety_height = Mathf.Sin(angle_height);
        }

        if (flashlight)
        {
            light = Instantiate(flashlight, transform.position, Quaternion.identity) as GameObject;
            light.transform.parent = this.transform;
            light.transform.eulerAngles = new Vector3(light.transform.eulerAngles.x, light.transform.eulerAngles.y, transform.eulerAngles.z + 270*sign);
            light.transform.position = new Vector3(light.transform.position.x + (offsetx_width * flashlightOffsetX) + (sign * offsetx_height * flashlightOffsetY), light.transform.position.y + (offsety_width * flashlightOffsetX) + (sign * offsety_height * flashlightOffsetY), light.transform.position.z);

            if (light)
            {
                if (flashlightOn == true)
                {
                    light.gameObject.SetActive(true);
                }
                else
                {
                    light.SetActive(false);
                }
            }
        }

        //Hand Sorting Order
        if (twoHanded)
        {
            leftHand.transform.position = new Vector3(pos_x + (offsetx_width * ((-width / 4) + handOffsetX)) + (sign * offsetx_height * (handOffsetY)), pos_y + (offsety_width * ((-width / 4) + handOffsetX)) + (sign * offsety_height * (handOffsetY)), 0);
            rightHand.transform.position = new Vector3(pos_x + (offsetx_width * ((width / 4) + handOffsetX)) + (sign * offsetx_height * (handOffsetY)), pos_y + (offsety_width * ((width / 4) + handOffsetX)) + (sign * offsety_height * (handOffsetY)), 0);

            leftHand.transform.eulerAngles = new Vector3(
                leftHand.transform.eulerAngles.x,
                leftHand.transform.eulerAngles.y,
                transform.eulerAngles.z
            );
            rightHand.transform.eulerAngles = new Vector3(
                rightHand.transform.eulerAngles.x,
                rightHand.transform.eulerAngles.y,
                transform.eulerAngles.z
            );

            leftHandRender.sortingOrder = sprite.sortingOrder + 1;//+ layerOrder;
            rightHandRender.sortingOrder = sprite.sortingOrder + 1;//+ layerOrder;
        }
        else
        {
            Vector3 offset_right = new Vector3(0.3f, -0.5f, 0);//new Vector3(0.5f, -0.5f, 0); //Offset for the weapon on the right hand
            Vector3 offset_left = new Vector3(-0.3f, -0.5f, 0);//new Vector3(-0.5f, -0.5f, 0); //Offset for the weapon on the left hand

            leftHand.transform.position = parent.transform.position + offset_left;
            rightHand.transform.position = parent.transform.position + offset_right;

            leftHand.transform.eulerAngles = new Vector3(
                leftHand.transform.eulerAngles.x,
                leftHand.transform.eulerAngles.y,
                0
            );
            rightHand.transform.eulerAngles = new Vector3(
                rightHand.transform.eulerAngles.x,
                rightHand.transform.eulerAngles.y,
                0
            );

            leftHandRender.sortingOrder = sprite.sortingOrder + 1;//+ layerOrder;
            rightHandRender.sortingOrder = sprite.sortingOrder + 1;//+ layerOrder;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (light)
        {
            if (flashlightOn == true)
            {
                light.gameObject.SetActive(true);
            }
            else
            {
                light.SetActive(false);
            }
        }
        
        if (destroy == true)
        {
            Destroy(leftHand);
            Destroy(rightHand);
            Destroy(gameObject);
        }
        
        if (tag == "EnemyWeapon")
        {
            if (parent)
            {
                semi_temp--;
                EnemyScript enemyScript = parent.gameObject.GetComponent<EnemyScript>();
                shoot = enemyScript.shoot;
            }
        }

        pos_x = transform.position.x;
        pos_y = transform.position.y;

        if (Mathf.Sign(transform.localScale.x) > 0)
        {
            sign = 1;

            angle_width = transform.rotation.eulerAngles.z * (Mathf.PI / 180);
            angle_height = (90 + transform.rotation.eulerAngles.z) * (Mathf.PI / 180);

            offsetx_width = Mathf.Cos(angle_width);
            offsety_width = Mathf.Sin(angle_width);

            offsetx_height = Mathf.Cos(angle_height);
            offsety_height = Mathf.Sin(angle_height);
        }
        else
        {
            sign = -1;

            angle_width = (transform.rotation.eulerAngles.z + 180) * (Mathf.PI / 180);
            angle_height = (90 + (transform.rotation.eulerAngles.z + 180)) * (Mathf.PI / 180);

            offsetx_width = Mathf.Cos(angle_width);
            offsety_width = Mathf.Sin(angle_width);

            offsetx_height = Mathf.Cos(angle_height);
            offsety_height = Mathf.Sin(angle_height);
        }

        //Hand Sorting Order
        if (twoHanded)
        {
            leftHand.transform.position = new Vector3(pos_x + (offsetx_width * ((-width / 4) + handOffsetX)) + (sign * offsetx_height * (handOffsetY)), pos_y + (offsety_width * ((-width / 4) + handOffsetX)) + (sign * offsety_height * (handOffsetY)), 0);
            rightHand.transform.position = new Vector3(pos_x + (offsetx_width * ((width / 4) + handOffsetX)) + (sign * offsetx_height * (handOffsetY)), pos_y + (offsety_width * ((width / 4) + handOffsetX)) + (sign * offsety_height * (handOffsetY)), 0);

            leftHandRender.sortingOrder = sprite.sortingOrder + 1;//+ layerOrder;
            rightHandRender.sortingOrder = sprite.sortingOrder + 1;//+ layerOrder;

            leftHand.transform.eulerAngles = new Vector3(
                leftHand.transform.eulerAngles.x,
                leftHand.transform.eulerAngles.y,
                transform.eulerAngles.z
            );
            rightHand.transform.eulerAngles = new Vector3(
                rightHand.transform.eulerAngles.x,
                rightHand.transform.eulerAngles.y,
                transform.eulerAngles.z
            );
        }
        else
        {
            Vector3 offset_right = new Vector3(0.3f, -0.5f, 0);//new Vector3(0.5f, -0.5f, 0); //Offset for the weapon on the right hand
            Vector3 offset_left = new Vector3(-0.3f, -0.5f, 0);//new Vector3(-0.5f, -0.5f, 0); //Offset for the weapon on the left hand

            leftHand.transform.position = parent.transform.position + offset_left;
            rightHand.transform.position = parent.transform.position + offset_right;

            leftHand.transform.eulerAngles = new Vector3(
                leftHand.transform.eulerAngles.x,
                leftHand.transform.eulerAngles.y, 
                0
            );
            rightHand.transform.eulerAngles = new Vector3(
                rightHand.transform.eulerAngles.x,
                rightHand.transform.eulerAngles.y,
                0
            );

            leftHandRender.sortingOrder = sprite.sortingOrder + 1;//+ layerOrder;
            rightHandRender.sortingOrder = sprite.sortingOrder + 1;//+ layerOrder;
        }

        //Hand Properties

        leftHandRender.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
        rightHandRender.GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;

        if ((Global.shopActive == false && tag == "Weapon") || tag == "EnemyWeapon")
        {
            if ((((Input.GetMouseButtonDown(0) && automaticAssault == false) ||
                 (Input.GetMouseButtonDown(0) && automaticAssault == true && tempAssault <= 0) ||
                 (Input.GetMouseButton(0) && automaticAssault == true && tempAssault <= 0)) && tag == "Weapon") ||
                 (shoot == true && semi_temp <= 0 && (automaticAssault == false) || (shoot == true && automaticAssault == true && tempAssault <= 0) && tag == "EnemyWeapon"))
            {
                shootCheck = true;
            }
        }

        //Laser Sight
        /*if (ricochetAmount > 0)
        {
            //MAYBE ADD IN A FOR LOOP FOR BULLET SPREAD ALSO********************************

            GameObject laserSightObject = Instantiate(laserSight, new Vector3(pos_x + (offsetx_width * (width / 2)) + (sign * offsetx_height * (height / 6)), pos_y + (offsety_width * (width / 2)) + (sign * offsety_height * (height / 6)), 0), transform.rotation) as GameObject;

            Laser laserSightScript = laserSightObject.GetComponent<Laser>();

            if (Mathf.Sign(transform.localScale.x) > 0)
            {
                bulletAngle = transform.rotation.eulerAngles.z;
            }
            else
            {
                bulletAngle = transform.rotation.eulerAngles.z + 180;
            }

            laserSightScript.lightAngle = bulletAngle * (Mathf.PI / 180);
            laserSightScript.ricochetAmount = ricochetAmount;
        }*/
    }

    private void FixedUpdate()
    {
        if (automaticAssault == true)
        {
            tempAssault--;
        }

        //Bullet Angle
        float bulletAngle;

        //Shoot Action
        if (shootCheck)
        {
            shootCheck = false;
            /*GameObject muzzle = Instantiate(muzzleFlash, new Vector3(pos_x + (offsetx_width * ((width / 2) + muzzleFlashOffsetX)) + (sign * offsetx_height * ((height / 6) + muzzleFlashOffsetY)), pos_y + (offsety_width * ((width / 2) + muzzleFlashOffsetX)) + (sign * offsety_height * ((height / 6) + muzzleFlashOffsetY)), 0), transform.rotation) as GameObject;

            Renderer muzzleRender = muzzle.GetComponent<Renderer>();

            muzzleRender.sortingOrder = sprite.sortingOrder + 5;

            if (Mathf.Sign(transform.localScale.x) <= 0)
            {
                muzzle.transform.eulerAngles = new Vector3(
                    muzzle.transform.eulerAngles.x,
                    muzzle.transform.eulerAngles.y,
                    muzzle.transform.eulerAngles.z + 180
                );
            }*/

            if (soundHandler)
            {
                soundHandler.PlaySound();
            }

            if (tag == "Weapon")
            {
                Shaker.Shake(screenShakeDuration, screenShakeIntensity);
            }

            if (automaticAssault == true)
            {
                tempAssault = automaticDelay;
            }

            if (tag == "EnemyWeapon")
            {
                semi_temp = semi_delay;
            }

            for (int i = 0; i < bulletSpread; i++)
            {
                //Bullet Spread
                float bulletSpreadAngle;

                GameObject bullet = Instantiate(Bullet, new Vector3(pos_x + (offsetx_width * ((width / 2) + bulletOffsetX)) + (sign * offsetx_height * ((height / 6) + bulletOffsetY)), pos_y + (offsety_width * ((width / 2) + bulletOffsetX)) + (sign * offsety_height * ((height / 6) + bulletOffsetY)), 0), transform.rotation) as GameObject;

                GameObject Bullets = GameObject.Find("TempBullets");
                bullet.transform.parent = Bullets.transform;

                if (gameObject.tag == "EnemyWeapon")
                {
                    bullet.gameObject.tag = "EnemyBullet";
                }

                SpriteRenderer bulletRender = bullet.GetComponent<SpriteRenderer>();
                BulletScript bulletScript = bullet.GetComponent<BulletScript>();

                //Bullet Richochet
                bulletScript.ricochet = ricochetAmount;
                bulletScript.passThrough = passThroughAmount;

                if (Mathf.Sign(transform.localScale.x) > 0)
                {
                    bulletAngle = transform.rotation.eulerAngles.z;
                }
                else
                {
                    bulletAngle = transform.rotation.eulerAngles.z + 180;
                }

                //Bullet Spread
                if (bulletSpread > 1)
                {
                    bulletSpreadAngle = (((i) / ((float)bulletSpread - 1)) * ((float)bulletSpread * 6)) - ((float)bulletSpread * 6) / 2;
                }
                else
                {
                    bulletSpreadAngle = 0;
                }

                offset_angle = Random.Range(-bulletSpreadRandom, bulletSpreadRandom);

                //Data given to bullet

                bulletScript.bulletAngle = (bulletAngle + bulletSpreadAngle + offset_angle) * (Mathf.PI / 180);

                bullet.transform.eulerAngles = new Vector3(
                    bullet.transform.eulerAngles.x,
                    bullet.transform.eulerAngles.y,
                    bullet.transform.eulerAngles.z + bulletSpreadAngle + offset_angle
                );

                bulletScript.bulletDamage = bulletDamage;
                bulletScript.bulletSpeed = bulletSpeed;
                bulletScript.bulletRange = bulletRange;

                //bulletScript.sortingLayer = sprite.sortingLayerName;
            }
        }
    }
}