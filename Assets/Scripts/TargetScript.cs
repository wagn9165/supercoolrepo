using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is given to the Cusor prefab////////////////////////////////////
public class TargetScript : MonoBehaviour
{
    Vector3 mouse_position;

    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        //Movement
        rb.MovePosition(mouse_position);
    }
}
