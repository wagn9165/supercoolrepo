using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    public GameObject proximityKey;
    GameObject eKey;
    public GameObject shopUI;
    GameObject shop;

    public GameObject Item0;
    public GameObject Item1;
    public GameObject Item2;
    public GameObject Item3;
    public GameObject Item4;
    public GameObject Item5;
    public GameObject Item6;
    public GameObject Item7;
    public GameObject Item8;
    public GameObject Item9;

    GameObject player;
    Renderer eKeyRender;

    bool active;
    bool open;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
        open = false;

        eKey = Instantiate(proximityKey, new Vector3(transform.position.x, transform.position.y + 2, 0), Quaternion.identity);
        eKeyRender = eKey.GetComponent<Renderer>();
        eKeyRender.material.color = new Color(eKeyRender.material.color.r, eKeyRender.material.color.g, eKeyRender.material.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        eKey.transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y + 1.7f, 0);

        player = GameObject.FindGameObjectWithTag("Player");

        if (player)
        {
            if(Vector2.Distance(transform.position, player.transform.position) < 4)
            {
                eKeyRender.material.color = new Color(eKeyRender.material.color.r, eKeyRender.material.color.g, eKeyRender.material.color.b, 1);
                active = true;
            }
            else
            {
                eKeyRender.material.color = new Color(eKeyRender.material.color.r, eKeyRender.material.color.g, eKeyRender.material.color.b, 0);
                active = false;
            }
        }

        //Shop UI

        if (active && Input.GetKeyDown(KeyCode.E) && !open)
        {
            open = true;
            shop = Instantiate(shopUI, transform.position + new Vector3(4, 2, 0), Quaternion.identity);
            Global.shopActive = true;
            Global.shopObject = shop.gameObject;
        }
        else if ((Input.GetKeyDown(KeyCode.E) && open) || !active)
        {
            open = false;
            if (shop)
                Destroy(shop);
            Global.shopActive = false;
        }
    }
}
