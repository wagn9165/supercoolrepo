using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class Global
{
    //public static Vector3 mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    public static bool shopActive = false;
    public static bool startRender = false;
    public static GameObject shopObject;

}

public class GameManager : MonoBehaviour
{
    Transform target;

    Vector3 mouse_position;

    Transform playerTransform;
    public BoxCollider2D mapBounds;

    public GameObject playerSpawn;

    //Camera Follow
    private float xMin, xMax, yMin, yMax;
    private float camY, camX;
    private float camOrthsize;
    private float cameraRatio;
    Vector2 offset;
    private Camera mainCam;
    private float maxOffset = 4;
    private float minZoom = 6f;
    private float maxZoom = 10f;

    //Testing Enemies
    public GameObject Enemy;
    public float maxEnemiesOnScreen;
    //float enemiesOnScreen;
    public float spawnRate = 100;
    public float spawn = 0;
    private float xmin, xmax, ymin, ymax;
    public float[] xTilePos;
    public float[] yTilePos;
    float[] xTileRep;
    float[] yTileRep;
    int maxPos = 0;

    public GameObject Cursor;

    //Zoom
    public float targetZoom;
    [SerializeField] private float zoomLerpSpeed = 10;

    //Player Spawn
    public GameObject playerObj;
    GameObject player;

    //Tile
    public GameObject tileParent;
    GameObject EnemyParent;

    bool spawnStart = false;

    //float numFrames = 0; //FPS Counter
    float frameSum;

    private void Start()
    {
        //VSync
        //QualitySettings.vSyncCount = 1;

        //enemiesOnScreen = 0;

        Instantiate(Cursor, new Vector3(0, 0, 0), Quaternion.identity);

        EnemyParent = GameObject.Find("TempEnemies");

        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");

        if (playerSpawn)
        {
            player = Instantiate(playerObj, new Vector3(playerSpawn.transform.position.x, playerSpawn.transform.position.y, 0), transform.rotation) as GameObject;
            playerTransform = player.GetComponent<Transform>();
            Destroy(playerSpawn);
        }

        target = GetComponent<Transform>();

        //Camera Follow
        xMin = mapBounds.bounds.min.x;
        xMax = mapBounds.bounds.max.x;
        yMin = mapBounds.bounds.min.y;
        yMax = mapBounds.bounds.max.y;
        mainCam = GetComponent<Camera>();
        camOrthsize = mainCam.orthographicSize;
        cameraRatio = (xMax + camOrthsize) / 2.0f;

        //Zoom
        mainCam = Camera.main;
        targetZoom = mainCam.orthographicSize;

        /*float room_width = 3;
        float room_height = 3;

        float tileCoeff = 2.61996f;

        float xStart = -(room_width * tileCoeff) / 2;//2.20445f;
        float yStart = (room_height * tileCoeff) / 2;//1.7047f;

        float xTile = 0.65626f;
        float yTile = 0.65626f;

        float wallOffset = 0.65625f;//0.87f;
        */
        //maxPos = TileParent.transform.childCount;

        //float point;

        xTilePos = new float[9999];
        yTilePos = new float[9999];

        /*for (int i = 0; i < maxPos; i++)
        {
            xTilePos[i] = TileParent.transform.GetChild(i).transform.position.x;
            yTilePos[i] = TileParent.transform.GetChild(i).transform.position.y;
        }*/

        //GameObject[] shadows = GameObject.FindGameObjectsWithTag("Shadow");

        maxPos = 0;
        /*foreach (GameObject shadow in shadows)
        {
            Debug.Log("Here");
            SpriteRenderer shadowRender = shadow.GetComponent<SpriteRenderer>();

            shadowRender.material.color = new Color(255, 0, 0, 0);
        }*/

        /*foreach (Transform child in tileParent.transform)
        {
            xTilePos[maxPos] = child.transform.position.x;
            yTilePos[maxPos] = child.transform.position.y;
            maxPos++;
        }
        maxPos--;*/

        //Store Tile Coordinates
        

        /*for(int i = 0; i < maxPos; i++)
        {
            xTilePos[i] = tile[i].transform.position.x;
            yTilePos[i] = tile[i].transform.position.y;
            Debug.Log("Here");
        }*/

        //Room Generator
        //float counter = 0;
        //int room_amount = 2;

        //string roomType = "StartRight";
        /*
         * Vertical
         * Horizontal
         * StartRight
         * StartLeft
         * StartUp
         * StartDown
         * UpRight
         * UpLeft
         * DownRight
         * DownLeft
        */
        
        /*room_width++;
        room_height++;

        maxPos = Mathf.CeilToInt(room_width * room_height * room_amount * 4);
        xTilePos = new float[maxPos];
        yTilePos = new float[maxPos];
        xTileRep = new float[maxPos];
        yTileRep = new float[maxPos];

        int posCounter = 0;
        for (int i = 0; i < room_amount; i++)
        {
            //choose direction
            for (int y = 0; y <= room_height; y++)
            {
                for (int x = 0; x <= room_width; x++)
                {
                    float xPos = tileCoeff * x;
                    float yPos = tileCoeff * y;

                    if (y == 0 && x != 0 && x != room_width)
                    {
                        if (roomType != "StartUp" && roomType != "Vertical" && roomType != "DownLeft")
                            Instantiate(wallHorizontal, new Vector3(xStart + xPos, yStart, 0), transform.rotation);
                    }
                    else if (y == room_height && x != 0 && x != room_width)
                    {
                        if (roomType != "StartDown" && roomType != "Vertical")
                            Instantiate(wallHorizontal, new Vector3(xStart + xPos, yStart - (tileCoeff * room_height), 0), transform.rotation);
                    }
                    else if (y == 0 && x == 0)
                    {
                        if (roomType != "StartLeft" && roomType != "Horizontal" && roomType != "Vertical" && roomType != "DownLeft")
                            Instantiate(wallVerticalUpRight, new Vector3(xStart + wallOffset, yStart - yPos, 0), transform.rotation);
                        else if (roomType != "DownLeft")
                            Instantiate(wallVerticalDownLeft, new Vector3(xStart + wallOffset + wallOffset, yStart + wallOffset - yPos, 0), transform.rotation);
                    }
                    else if (y == 0 && x == room_width)
                    {
                        if (roomType != "StartRight" && roomType != "Horizontal" && roomType != "Vertical" && roomType != "DownLeft")
                            Instantiate(wallVerticalUpLeft, new Vector3(xStart - wallOffset + (tileCoeff * room_width), yStart - yPos, 0), transform.rotation);
                    }
                    else if (y == room_height && x == 0)
                    {
                        if (roomType != "StartLeft" && roomType != "Horizontal" && roomType != "Vertical" && roomType != "DownLeft")
                            Instantiate(wallVerticalDownRight, new Vector3(xStart + wallOffset, yStart - yPos, 0), transform.rotation);
                    }
                    else if (y == room_height && x == room_width)
                    {
                        if (roomType != "StartRight" && roomType != "Horizontal" && roomType != "Vertical")
                            Instantiate(wallVerticalDownLeft, new Vector3(xStart - wallOffset + (tileCoeff * room_width), yStart - yPos, 0), transform.rotation);
                    }
                    else if (x == 0)
                    {
                        if (roomType != "StartLeft" && roomType != "Horizontal")
                            Instantiate(wallVertical, new Vector3(xStart + wallOffset, yStart - yPos, 0), transform.rotation);
                    }
                    else if (x == room_width)
                    {
                        if(roomType != "StartRight" && roomType != "Horizontal")
                            Instantiate(wallVertical, new Vector3(xStart - wallOffset + (tileCoeff * room_width), yStart - yPos, 0), transform.rotation);
                    }
                    else
                    {
                        if (counter == 0)
                        {
                            xmin = xStart - xTile + (x * tileCoeff);
                            ymin = yStart - yTile - (y * tileCoeff);
                        }
                        counter += 4;

                        float xTile1 = xStart - xTile + (x * tileCoeff);
                        float yTile1 = yStart - yTile - (y * tileCoeff);

                        float xTile2 = xStart - xTile + (x * tileCoeff) + (tileCoeff / 2);
                        float yTile2 = yStart - yTile - (y * tileCoeff);

                        float xTile3 = xStart - xTile + (x * tileCoeff);
                        float yTile3 = yStart - yTile - (y * tileCoeff) + (tileCoeff / 2);

                        float xTile4 = xStart - xTile + (x * tileCoeff) + (tileCoeff / 2);
                        float yTile4 = yStart - yTile - (y * tileCoeff) + (tileCoeff / 2);

                        Instantiate(tile, new Vector3(xTile1, yTile1, 0), transform.rotation);
                        Instantiate(tile, new Vector3(xTile2, yTile2, 0), transform.rotation);
                        Instantiate(tile, new Vector3(xTile3, yTile3, 0), transform.rotation);
                        Instantiate(tile, new Vector3(xTile4, yTile4, 0), transform.rotation);
                        
                        xTilePos[posCounter] = xTile1;
                        yTilePos[posCounter++] = yTile1;

                        xTilePos[posCounter] = xTile2;
                        yTilePos[posCounter++] = yTile2;

                        xTilePos[posCounter] = xTile3;
                        yTilePos[posCounter++] = yTile3;

                        xTilePos[posCounter] = xTile4;
                        yTilePos[posCounter++] = yTile4;

                        if (counter >= ((((room_width - 1) * 2) * ((room_height - 1) * 2))) * room_amount - 1)
                        {
                            xmax = xStart - xTile + (x * tileCoeff) + (tileCoeff / 2);
                            ymax = yStart - yTile - (y * tileCoeff) + (tileCoeff / 2);
                        }
                    }
                }
            }
            roomType = "DownLeft";

            int rand = Mathf.RoundToInt(Random.Range(0, 100));

            if (rand >= 0)
            {
                xStart += tileCoeff * (room_width - 1);
            }
            else
            {
                yStart -= tileCoeff * (room_height - 1);
            }
        }*/
    }

    void Update()
    {
        //FPS
        /*frameSum += 1.0f / Time.deltaTime;
        numFrames++;

        Debug.Log(frameSum / numFrames);
        Debug.ClearDeveloperConsole();*/

        //Testing Enemies

        //COMMENT

        GameObject tileExists = GameObject.FindGameObjectWithTag("Tile");

        if (tileExists && !spawnStart)
        {
            GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");

            maxPos = 0;
            foreach (GameObject tile in tiles)
            {
                xTilePos[maxPos] = tile.transform.position.x;
                yTilePos[maxPos] = tile.transform.position.y;
                maxPos++;
            }
            maxPos--;

            spawnStart = true;
            Global.startRender = true;
        }

        if (spawnStart)
        {
            spawn++;

            //Enemies in level
            int enemiesOnScreen = EnemyParent.transform.childCount;

            if (spawn >= spawnRate && enemiesOnScreen < maxEnemiesOnScreen)
            {
                spawn = 0;
                Vector3 randPos;
                int rand;
                int loopCount = 0;

                if (player)
                {
                    do
                    {
                        rand = Mathf.RoundToInt(Random.Range(0, maxPos));
                        randPos = new Vector3(xTilePos[rand], yTilePos[rand], 0);
                        loopCount++;
                        if (loopCount > 100)
                            break;
                    } while (Vector2.Distance(randPos, player.transform.position) < 20);
                }
                else
                {
                    rand = Mathf.RoundToInt(Random.Range(0, maxPos));
                    randPos = new Vector3(xTilePos[rand], yTilePos[rand], 0);
                }


                GameObject enemy = Instantiate(Enemy, new Vector3(xTilePos[rand], yTilePos[rand], 0), transform.rotation) as GameObject;

                //enemy.gameObject.GetComponent<BulletScript>();
                //BulletScript enemyScript = enemy.GetComponent<BulletScript>();
            }
            //spawn = 2;

            //COMMENT

            playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn");

            if (!player && playerSpawn)
            {
                player = Instantiate(playerObj, new Vector3(playerSpawn.transform.position.x, playerSpawn.transform.position.y, 0), transform.rotation) as GameObject;
                playerTransform = player.GetComponent<Transform>();
                Destroy(playerSpawn);
            }

            //Restart with Backspace //REMOVEEE
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }

    void FixedUpdate()
    {
        if (!Global.shopActive)
        {
            xMin = mapBounds.bounds.min.x;
            xMax = mapBounds.bounds.max.x;
            yMin = mapBounds.bounds.min.y;
            yMax = mapBounds.bounds.max.y;

            //Camera Follow
            mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (player)
            {
                if ((Vector2.Distance(mouse_position, playerTransform.position) / 4) < (maxOffset))
                {
                    offset = new Vector2(mouse_position.x - playerTransform.position.x, mouse_position.y - playerTransform.position.y) / 4;
                }
                else
                {
                    float xDist = mouse_position.x - playerTransform.position.x;
                    float yDist = mouse_position.y - playerTransform.position.y;

                    float angle = Mathf.Atan(yDist / xDist);

                    float xMax = Mathf.Sign(xDist) * maxOffset * Mathf.Cos(angle);
                    float yMax = Mathf.Sign(xDist) * maxOffset * Mathf.Sin(angle);

                    offset.x = xMax;
                    offset.y = yMax;
                }

                //Zoom
                float scrollData = Input.GetAxis("Mouse ScrollWheel");
                targetZoom = (Vector2.Distance(new Vector2(0f, 0f), offset) / 1.2f) + minZoom;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);

                //Debug.Log(targetZoom);

                mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
                camOrthsize = mainCam.orthographicSize;
                cameraRatio = (xMax / 2.0f + camOrthsize) / 2.0f;

                //Coordinate Text
                //coord_x.text = transform.position.x.ToString();
                //coord_y.text = transform.position.y.ToString();

                //Camera Movement and Clamp
                camY = Mathf.Clamp(playerTransform.position.y + offset.y, yMin + camOrthsize, yMax - camOrthsize);
                camX = Mathf.Clamp(playerTransform.position.x + offset.x, xMin + cameraRatio, xMax - cameraRatio);

                this.transform.position = new Vector3(camX, camY, this.transform.position.z);
            }
        }
        else if (Global.shopObject)
        {
            this.transform.position = new Vector3(Global.shopObject.transform.position.x, Global.shopObject.transform.position.y, this.transform.position.z);
        }

        ScreenShake shake = GetComponent<ScreenShake>();
        shake.initialPos.x = transform.position.x;
        shake.initialPos.y = transform.position.y;
        shake.initialPos.z = -10;
    }
}
