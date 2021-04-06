using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is given to the Background object////////////////////////////////////
public class Background : MonoBehaviour
{
    private SpriteRenderer sprite;
    public BoxCollider2D mapBounds;

    public float offset = 1;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>(); //Set the reference to our SpriteRenderer component
        float width = sprite.bounds.extents.x * 2; //Distance to the right side * 2, from your center point
        float height = sprite.bounds.extents.x * 2; //Distance to the top * 2, from your center point

        float xMin = mapBounds.bounds.min.x;
        float yMin = mapBounds.bounds.min.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
