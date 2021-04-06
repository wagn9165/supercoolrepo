using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapScript : MonoBehaviour
{
    public GameObject camera;
    public BoxCollider2D mapBounds;

    public GameObject pixel;
    List<GameObject> pixels;
    //List<GameObject> enemyPixels;
    GameObject playerPixel;
    float xPlayer, yPlayer;
    float pixelCoefficient = 0.02f;

    public float scale;

    private float xMin, xMax, yMin, yMax;

    int maxPos = 0;
    //int enemiesPos = 0;

    int start;

    public float xOffset = 0;//-150;
    public float yOffset = 0;//-50;

    List<float> xTilePos;
    List<float> yTilePos;

    //List<float> xEnemyPos;
    //List<float> yEnemyPos;

    GameObject player;

    float xDist = 1000;
    float yDist = 1000;

    float xDistOffset = 1000;
    float yDistOffset = 1000;

    public float distance;

    GameObject background;
    Vector2 topRight;
    Vector2 bottomLeft;

    // Start is called before the first frame update
    void Start()
    {
        distance = 25f;
        scale = 1;
        start = 0;

        xTilePos = new List<float>();
        yTilePos = new List<float>();

        //xEnemyPos = new List<float>();
        //yEnemyPos = new List<float>();

        pixels = new List<GameObject>();
        //enemyPixels = new List<GameObject>();

        xMin = mapBounds.bounds.min.x;
        xMax = mapBounds.bounds.max.x;
        yMin = mapBounds.bounds.min.y;
        yMax = mapBounds.bounds.max.y;

        player = GameObject.FindGameObjectWithTag("Player");

        if (player)
        {
            Debug.Log("Happened");
            playerPixel = Instantiate(pixel, new Vector3(0, 0, 0), Quaternion.identity);
            playerPixel.GetComponent<Renderer>().material.color = Color.red;
            playerPixel.GetComponent<Renderer>().sortingLayerName = "UI";
        }
    }

    private void Update()
    {
        scale = camera.GetComponent<Camera>().orthographicSize / 6;

        pixelCoefficient = 0.02f * scale;

        xMin = mapBounds.bounds.min.x;
        xMax = mapBounds.bounds.max.x;
        yMin = mapBounds.bounds.min.y;
        yMax = mapBounds.bounds.max.y;

        //Enemies
        /*GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        xEnemyPos.Clear();
        yEnemyPos.Clear();
        
        for (int i = 0; i < enemiesPos; i++)
        {
            Destroy(enemyPixels[i]);
        }
        enemyPixels.Clear();

        enemiesPos = 0;
        foreach (GameObject enemy in enemies)
        {
            xEnemyPos.Add(enemy.transform.position.x);
            yEnemyPos.Add(enemy.transform.position.y);
            enemiesPos++;
        }
        enemiesPos--;

        if(enemies.Length > 0)
        {
            for (int i = 0; i < enemiesPos; i++)
            {
                Debug.Log("Here");

                enemyPixels.Add(Instantiate(pixel, transform.position + new Vector3((xMin + xEnemyPos[i]) * pixelCoefficient, (yMin + yEnemyPos[i]) * pixelCoefficient, 0), Quaternion.identity));

                enemyPixels[i].transform.parent = transform;
                //enemyPixels[i].GetComponent<SpriteRenderer>().material.color = new Color(255, 255, 255, 0);
                enemyPixels[i].GetComponent<Renderer>().sortingLayerName = "UI";
                enemyPixels[i].GetComponent<Renderer>().sortingOrder = 1;
            }
        }*/

        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

        if (tiles.Length > 0 && start == 0)
        {
            //transform.position = new Vector3(-4, 0, 0);

            start = 1;

            maxPos = 0;
            foreach (GameObject tile in tiles)
            {
                xTilePos.Add(tile.transform.position.x);
                yTilePos.Add(tile.transform.position.y);
                maxPos++;
            }

            background = GameObject.Find("Background");

            if (background)
            {
                topRight = new Vector2(background.transform.position.x + (background.transform.localScale.x/2), background.transform.position.y + (background.transform.localScale.y / 2));
                bottomLeft = new Vector2(background.transform.position.x - (background.transform.localScale.x / 2), background.transform.position.y - (background.transform.localScale.y / 2));

                /* xTilePos.Add(topRight.x);
                yTilePos.Add(topRight.y);
                maxPos++;

                xTilePos.Add(bottomLeft.x);
                yTilePos.Add(bottomLeft.y);
                maxPos++; */
            }

            for (int i = 0; i < maxPos; i++)
            {
                pixels.Add(Instantiate(pixel, transform.position + new Vector3((xMin + xTilePos[i]) * pixelCoefficient, (yMin + yTilePos[i]) * pixelCoefficient, 0), Quaternion.identity));

                pixels[i].transform.parent = transform;

                Color temp = pixels[i].GetComponent<SpriteRenderer>().color;
                temp.a = 0f;                                                                            //Change to 0/////////////////////////////
                pixels[i].GetComponent<SpriteRenderer>().color = temp;

                pixels[i].GetComponent<Renderer>().sortingLayerName = "UI";

                if (transform.position.x - pixels[i].transform.position.x < xDist)
                {
                    xDist = transform.position.x - pixels[i].transform.position.x;
                }
                if (transform.position.y - pixels[i].transform.position.y < yDist)
                {
                    yDist = transform.position.y - pixels[i].transform.position.y;
                }

                if (xTilePos[i] < xDistOffset)
                {
                    xDistOffset = xTilePos[i];
                }
                if (yTilePos[i] < yDistOffset)
                {
                    yDistOffset = yTilePos[i];
                }
            }

            bottomLeft = new Vector2(xDistOffset, yDistOffset);
        }

        if (tiles.Length > 0)
        {
            for (int i = 0; i < maxPos; i++)
            {
                pixels[i].transform.localScale = new Vector3(scale, scale, 0);

                Vector2 tilePos = new Vector2(xTilePos[i], yTilePos[i]);

                if (player)
                {
                    //Color temp = pixels[i].GetComponent<SpriteRenderer>().color;
                    float temp = (-Vector2.Distance(tilePos, player.transform.position) + distance) / 4;
                    //pixels[i].GetComponent<SpriteRenderer>().color = temp;

                    if (temp < pixels[i].GetComponent<SpriteRenderer>().color.a)
                        temp = pixels[i].GetComponent<SpriteRenderer>().color.a;
                    else if (temp > 1)
                        temp = 1;

                    pixels[i].GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, temp);
                }
            }
        }
        if (player)
        {
            playerPixel.transform.localScale = new Vector3(scale, scale, 0);
            playerPixel.GetComponent<Renderer>().material.color = Color.red;
        }
        /*if (enemies.Length > 0)
        {
            for (int i = 0; i < enemiesPos; i++)
            {
                enemyPixels[i].transform.localScale = new Vector3(scale, scale, 0);
                enemyPixels[i].GetComponent<Renderer>().material.color = Color.red;
            }
        }*/

        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");

            if (player)
            {
                xPlayer = player.transform.position.x;
                yPlayer = player.transform.position.y;
                
                playerPixel = Instantiate(pixel, transform.position + new Vector3((xMin + xPlayer) * pixelCoefficient, (yMin + yPlayer) * pixelCoefficient, 0), Quaternion.identity);
                playerPixel.GetComponent<Renderer>().material.color = Color.red;
                playerPixel.GetComponent<Renderer>().sortingLayerName = "UI";
                playerPixel.GetComponent<Renderer>().sortingOrder = 1;
                playerPixel.transform.parent = transform;
            }
        }
        else
        {
            xPlayer = player.transform.position.x;
            yPlayer = player.transform.position.y;
        }
    }
    
    void FixedUpdate()
    {
        //transform.position = (camera.transform.position + new Vector3(camera.GetComponent<GameManager>().targetZoom * 2, camera.GetComponent<GameManager>().targetZoom, 10));

        //transform.position = camera.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition);

        /*Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);*/


        //transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        //transform.position = camera.transform.position;
        //transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        //transform.position = new Vector3(5, 0, 0);

        //xOffset = 2 * camera.GetComponent<RoomGeneration>().max_width;// / 2;
        //yOffset = 2 * camera.GetComponent<RoomGeneration>().max_height;// / 2;

        if (background)
        {
            xOffset = ((-bottomLeft.x + background.transform.localScale.x / 2) - 530) + 50;// / pixelCoefficient;
            yOffset = ((-bottomLeft.y + background.transform.localScale.y / 2) - 300) + 50;// / pixelCoefficient;
        }

        if (start == 1)
        {
            for (int i = 0; i < maxPos; i++)
            {
                pixels[i].transform.position = transform.position + new Vector3((xMin + xDist + xTilePos[i] + xOffset) * pixelCoefficient, (yMin + yDist + yTilePos[i] + yOffset) * pixelCoefficient, 0);// + new Vector3(xDist, yDist, 0f);
            }
            /*+ new Vector3(camera.GetComponent<GameManager>().targetZoom * 1.8f, camera.GetComponent<GameManager>().targetZoom, 0)*/
        }

        if (player)
            playerPixel.transform.position = transform.position + new Vector3((xMin + xDist + xPlayer + xOffset) * pixelCoefficient, (yMin + yDist + yPlayer + yOffset) * pixelCoefficient, 0);// + new Vector3(xDist, yDist, 0f);
    }
}