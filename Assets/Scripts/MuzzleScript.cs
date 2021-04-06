using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleScript : MonoBehaviour
{
    float duration = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        duration--;

        if (duration <= 0)
        {
            Destroy(gameObject);
        }
    }
}
