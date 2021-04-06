using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is given to the GameManager object (NOT ACTUAL GAMEMANAGER)////////////////////////////////////
public class GameStart : MonoBehaviour
{
    //public Camera mainCamera;
    public GameObject Cursor;

    // Start is called before the first frame update
    void Start()
    {
        //mainCamera = Instantiate(mainCamera, new Vector3(0, 0, -10), Quaternion.identity) as Camera;
        Instantiate(Cursor, new Vector3(0, 0, 0), Quaternion.identity);
        GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
