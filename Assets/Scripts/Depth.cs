using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is given to ALL active objects and determines their depth in their current layer////////////////////////////////////
public class Depth : MonoBehaviour
{
    public bool lowestDepth = false;
    public bool highestDepth = false;
    public int offset = 0;

    int max = 32767;

    private SpriteRenderer sort;
    float height;

    // Start is called before the first frame update
    void Start()
    {
        //Sorting Order
        sort = gameObject.GetComponent<SpriteRenderer>();
        height = sort.bounds.size.y / 2; //Distance to the top, from your center point

        if (!highestDepth && !lowestDepth)
        {
            sort.sortingOrder = Mathf.RoundToInt((max - (transform.position.y - height)) * 10) + offset;
        }
        else if (!highestDepth && lowestDepth)
        {
            sort.sortingOrder = 0 + offset;
        }
        else if (highestDepth && !lowestDepth)
        {
            sort.sortingOrder = max + offset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!highestDepth && !lowestDepth)
        {
            //Sorting Order
            height = sort.bounds.size.y / 2; //Distance to the top, from your center point
            sort.sortingOrder = Mathf.RoundToInt((100000 - (transform.position.y - height)) * 10) + offset;
        }
        else if (!highestDepth && lowestDepth)
        {
            sort.sortingOrder = 0 + offset;
        }
        else if (highestDepth && !lowestDepth)
        {
            sort.sortingOrder = max + offset;
        }
    }
}
