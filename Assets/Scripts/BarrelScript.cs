using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelScript : MonoBehaviour
{

    public float barrelDamage;

    public GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Bullet" || col.gameObject.tag == "EnemyBullet")
        {
            GameObject explosionObject = Instantiate(explosion, transform.position, Quaternion.identity);

            explosionObject.GetComponent<explosionScript>().damage = barrelDamage;
            Destroy(gameObject);
        }
        if (col.gameObject.tag == "Hazard")
        {
            GameObject explosionObject = Instantiate(explosion, transform.position, Quaternion.identity);

            explosionObject.GetComponent<explosionScript>().damage = barrelDamage;
            Destroy(gameObject);
        }
    }
}
  