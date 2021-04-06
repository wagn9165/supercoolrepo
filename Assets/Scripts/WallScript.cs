using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is given to the Wall prefab (MIGHT BE USELESS)////////////////////////////////////
public class WallScript : MonoBehaviour
{
    public Collider2D self;

    //public GameObject light;

    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(light, transform.position + new Vector3(0, -1, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Entered");
        if (other.tag == "Tile")
        {
            //Destroy(gameObject);
        }
    }
}
