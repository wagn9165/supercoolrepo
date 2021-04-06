using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGeneration : MonoBehaviour
{
    //Room Generator
    public GameObject background;

    public GameObject tile;

    GameObject tileParent;
    GameObject wallParent;
    GameObject tileObj;
    GameObject wallObj;

    public GameObject wallHorizontal;
    public GameObject wallVertical;
    public GameObject wallVerticalUpRight;
    public GameObject wallVerticalUpLeft;
    public GameObject wallVerticalDownRight;
    public GameObject wallVerticalDownLeft;

    public GameObject playerSpawn;

    [HideInInspector]
    public int max_width = 50;
    [HideInInspector]
    public int max_height = 50;

    public int doorAmount = 1;
    public int roomAmount = 6;

    public int minRoomSize = 3;
    public int maxRoomSize = 5;

    public int space = 4;

    public int[] xDoor;
    public int[] yDoor;
    public char[,] Pos;

    public float scale;
    float tileCoeff;

    public GameObject pathingScript;

    // Start is called before the first frame update
    void Start()
    {
        tileParent = GameObject.Find("Tiles");
        GameObject wallParent = GameObject.Find("Walls");

        tileCoeff = 2.61996f * scale;

        //Determine room size
        max_width = 25 * (roomAmount) - 25;
        max_height = 10 * (roomAmount);

        //Background
        background.transform.localScale = new Vector3(max_width * tileCoeff, max_height * tileCoeff, background.transform.localScale.z);

        Pos = new char[max_width, max_height];
        xDoor = new int[roomAmount * doorAmount];
        yDoor = new int[roomAmount * doorAmount];

        //Make rooms
        int check;
        int counter = 0;
        int loopCheck = 0;

        do
        {
            if (loopCheck > 10)
            {
                Debug.Log("Room Generation Failed");
                max_width += 10;//Mathf.RoundToInt(max_width * 1.5f);
                max_height += 10;//Mathf.RoundToInt(max_height * 1.5f);
                loopCheck = 0;
                Pos = new char[max_width, max_height];
                //loopCheck = 51;
                //break;
            }

            counter++;

            //Console.WriteLine("Generation {0}...", counter);

            check = 0;

            for (int i = 0; i < max_height; i++)
            {
                for (int j = 0; j < max_width; j++)
                {
                    Pos[j, i] = '.';
                }
            }

            bool spawn;

            int newGeneration = 0;

            for (int n = 0; n < roomAmount; n++)
            {
                spawn = false;
                //var rand = new Random();

                int xRandPos;
                int yRandPos;
                int xRandWidth;
                int yRandHeight;

                do
                {
                    xRandPos = Random.Range(5, max_width - maxRoomSize - 4 - 7); //5, 15
                    yRandPos = Random.Range(5, max_height - maxRoomSize - 4 - 7);

                    xRandWidth = Random.Range(minRoomSize + 4, maxRoomSize + 4); //7, 10
                    yRandHeight = Random.Range(minRoomSize + 4, maxRoomSize + 4);

                    if (newGeneration > 100)
                    {
                        n = 0;
                        newGeneration = 0;
                        //Console.Write("Retrying...\n");
                        for (int i = 0; i < max_height; i++)
                        {
                            for (int j = 0; j < max_width; j++)
                            {
                                Pos[j, i] = '.';
                            }
                        }
                        loopCheck++;
                        check = 1;
                        break;
                    }
                    newGeneration++;

                } while (collisionCheck(xRandPos, yRandPos, xRandWidth, yRandHeight));

                if (check == 1)
                    break;

                if (n == 0)
                    spawn = true;

                makeRoom(xRandPos, yRandPos, xRandWidth, yRandHeight, spawn);
            }

            //Make makeHallways

            if (check != 1)
            {
                int door = 0;
                for (int i = 0; i < max_height; i++)
                {
                    for (int j = 0; j < max_width; j++)
                    {
                        if (Pos[j, i] == 'X')
                        {
                            xDoor[door] = j;
                            yDoor[door++] = i;
                            ////Console.Write(j);
                            ////Console.Write(",");
                            ////Console.WriteLine(i);

                            Pos[j, i] = (char)(door + 64);
                        }
                    }
                }

                //Console.WriteLine();

                check += makeHallways();

                //Remove 'O'
                for (int i = 0; i < max_height; i++)
                {
                    for (int j = 0; j < max_width; j++)
                    {
                        if (Pos[j, i] == 'O')
                        {
                            Pos[j, i] = '.';
                        }
                        else if (Pos[j, i] != '.' && Pos[j, i] != 'Q')
                        {
                            Pos[j, i] = 'O';
                        }
                    }
                }

                check += walls();
                corners();
                directcorners();
            }

        } while (check > 0);

        if (loopCheck < 50)
        {

            //Remove 'O'
            for (int i = 1; i < max_height - 1; i++)
            {
                for (int j = 1; j < max_width - 1; j++)
                {
                    if (Pos[j, i] == '|')
                    {
                        if (Pos[j + 1, i] == 'O')
                        {
                            Pos[j, i] = ']';
                        }
                        else if (Pos[j - 1, i] == 'O')
                        {
                            Pos[j, i] = '[';
                        }
                    }
                    else if (Pos[j, i] == '-')
                    {
                        if (Pos[j, i - 1] == 'O')
                        {
                            Pos[j, i] = '-';
                        }
                        else if (Pos[j, i + 1] == 'O')
                        {
                            Pos[j, i] = '_';
                        }
                    }
                }
            }

            printRooms();

            for (int i = 0; i < max_height; i++)
            {
                for (int j = 0; j < max_width; j++)
                {
                    tilePlacement(j, i, Pos[j, i]);
                }
            }
        }

        background.transform.localScale = new Vector3(background.transform.localScale.x + 20, background.transform.localScale.y + 20, background.transform.localScale.z);//new Vector3(max_width * tileCoeff, max_height * tileCoeff, background.transform.localScale.z);

        pathingScript.GetComponent<AstarPath>().Scan();
    }

    void tilePlacement(int x, int y, char text)
    {
        float xPos = (-(max_width * tileCoeff)/2) + (x*tileCoeff);
        float yPos = (-(max_height * tileCoeff)/2) + (y* tileCoeff);

        if (text == 'O')
        {
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    tileObj = Instantiate(tile, new Vector3(xPos + (i * tileCoeff / scale), yPos + (j * tileCoeff / scale), 0), transform.rotation);
                    //tileObj.transform.parent = tileParent.transform;
                }
            }
        }
        else if (text == 'Q')
        {
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    tileObj = Instantiate(tile, new Vector3(xPos + (i * tileCoeff / scale), yPos + (j * tileCoeff / scale), 0), transform.rotation);
                    //tileObj.transform.parent = tileParent.transform;
                }
            }
            Instantiate(playerSpawn, new Vector3(xPos + (tileCoeff / scale), yPos + (tileCoeff / scale), 0), transform.rotation);
        }
        else if (text == '-')
        {
            for (int i = 0; i < scale; i++)
            {
                wallObj = Instantiate(wallHorizontal, new Vector3(xPos + (i * tileCoeff / scale), yPos, 0), transform.rotation);
                //wallObj.transform.parent = wallParent.transform;
            }
        }
        else if (text == '_')
        {
            for (int i = 0; i < scale; i++)
            {
                wallObj = Instantiate(wallHorizontal, new Vector3(xPos + (i * tileCoeff / scale), yPos + (tileCoeff / scale), 0), transform.rotation);
                //wallObj.transform.parent = wallParent.transform;
            }
        }
        else if (text == ']')
        {
            for (int i = 0; i < scale; i++)
            {
                wallObj = Instantiate(wallVertical, new Vector3(xPos + (tileCoeff/8) + (tileCoeff / scale), yPos + (i * tileCoeff / scale), 0), transform.rotation);
                //wallObj.transform.parent = wallParent.transform;
            }
        }
        else if (text == '[')
        {
            for (int i = 0; i < scale; i++)
            {
                wallObj = Instantiate(wallVertical, new Vector3(xPos - (tileCoeff / 8), yPos + (i * tileCoeff / scale), 0), transform.rotation);
                //wallObj.transform.parent = wallParent.transform;
            }
        }
        else if (text == '┐')
        {
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    if(j == 0)
                        wallObj = Instantiate(wallHorizontal, new Vector3(xPos + (i * (tileCoeff/2) / scale), yPos + (j * tileCoeff / scale), 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                    if (j == 0 && i == 1)
                        wallObj = Instantiate(wallVerticalDownLeft, new Vector3(xPos + (tileCoeff / 8) + (i * tileCoeff / scale), yPos + (j * tileCoeff / scale), 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                    if (j == 1 && i == 1)
                        wallObj = Instantiate(wallVertical, new Vector3(xPos + (tileCoeff / 8) + (tileCoeff / scale), yPos + (j * tileCoeff / scale), 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                }
            }
        }
        else if (text == '┌')
        {
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    if (j == 0)
                        wallObj = Instantiate(wallHorizontal, new Vector3(xPos + ((tileCoeff/2) / scale) + (i * (tileCoeff / scale)), yPos + (j * tileCoeff / scale), 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                    if (j == 0 && i == 0)
                        wallObj = Instantiate(wallVerticalDownRight, new Vector3(xPos - (tileCoeff / 8), yPos + (j * tileCoeff / scale), 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                    if (j == 1 && i == 1)
                        wallObj = Instantiate(wallVertical, new Vector3(xPos - (tileCoeff / 8), yPos + (j * tileCoeff / scale), 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                }
            }
        }
        else if (text == '└')
        {
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    if (j == 1)
                        wallObj = Instantiate(wallHorizontal, new Vector3(xPos + (tileCoeff / 4) + (i * (tileCoeff/2) / scale), yPos + (tileCoeff / scale), 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                    if (j == 1 && i == 0)
                        wallObj = Instantiate(wallVerticalUpRight, new Vector3(xPos - (tileCoeff / 8), yPos + (tileCoeff / scale), 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                    if (j == 1 && i == 1)
                        wallObj = Instantiate(wallVertical, new Vector3(xPos - (tileCoeff / 8), yPos, 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                }
            }
        }
        else if (text == '┘')
        {
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    if (j == 1)
                        wallObj = Instantiate(wallHorizontal, new Vector3(xPos + (i * (tileCoeff / 2) / scale), yPos + (tileCoeff / scale), 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                    if (j == 1 && i == 0)
                        wallObj = Instantiate(wallVerticalUpLeft, new Vector3(xPos + (tileCoeff / 8) + (tileCoeff / scale), yPos + (tileCoeff / scale), 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                    if (j == 1 && i == 1)
                        wallObj = Instantiate(wallVertical, new Vector3(xPos + (tileCoeff / 8) + (tileCoeff / scale), yPos, 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                }
            }
        }
        else if (text == 'L')
        {
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    if (j == 1 && i == 0)
                        wallObj = Instantiate(wallVerticalDownRight, new Vector3(xPos + (tileCoeff / 8) + (tileCoeff / scale), yPos + (tileCoeff / scale), 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                }
            }
        }
        else if (text == 'D')
        {
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    if (j == 1 && i == 0)
                        wallObj = Instantiate(wallVerticalUpRight, new Vector3(xPos + (tileCoeff / 8) + (tileCoeff / scale), yPos, 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                }
            }
        }
        else if (text == 'R')
        {
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    if (j == 1 && i == 0)
                        wallObj = Instantiate(wallVerticalUpLeft, new Vector3(xPos - (tileCoeff / 8), yPos, 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                }
            }
        }
        else if (text == 'U')
        {
            for (int i = 0; i < scale; i++)
            {
                for (int j = 0; j < scale; j++)
                {
                    if (j == 1 && i == 0)
                        wallObj = Instantiate(wallVerticalDownLeft, new Vector3(xPos - (tileCoeff / 8), yPos + (tileCoeff / scale), 0), transform.rotation);
                        //wallObj.transform.parent = wallParent.transform;
                }
            }
        }
    }

    void directcorners()
    {
        for (int i = 0; i < max_height; i++)
        {
            for (int j = 0; j < max_width; j++)
            {
                char upright = '.';
                char upleft = '.';
                char downright = '.';
                char downleft = '.';

                if (j > 0 && i > 0)
                    upleft = Pos[j - 1, i - 1];
                if (j < max_width - 1 && i > 0)
                    upright = Pos[j + 1, i - 1];
                if (j > 0 && i < max_height - 1)
                    downleft = Pos[j - 1, i + 1];
                if (j < max_width - 1 && i < max_height - 1)
                    downright = Pos[j + 1, i + 1];

                if (Pos[j, i] == '┌')
                {
                    if (downright == 'O')
                    {
                        Pos[j, i] = 'L';
                    }
                }
                else if (Pos[j, i] == '└')
                {
                    if (upright == 'O')
                    {
                        Pos[j, i] = 'D';
                    }
                }
                else if (Pos[j, i] == '┐')
                {
                    if (downleft == 'O')
                    {
                        Pos[j, i] = 'U';
                    }
                }
                else if (Pos[j, i] == '┘')
                {
                    if (upleft == 'O')
                    {
                        Pos[j, i] = 'R';
                    }
                }
            }
        }
    }

    bool collisionCheck(int x, int y, int width, int height)
    {
        for (int i = 0; i < height + space * 2; i++)
        {
            for (int j = 0; j < width + space * 2; j++)
            {
                if ((x - space) + j >= 0 && (x - space) + j < max_width && (y - space) + i >= 0 && (y - space) + i < max_height)
                    if (Pos[(x - space) + j, (y - space) + i] == 'O')
                        return true;
            }
        }
        return false;
    }

    int makeHallways()
    {
        int distx = 0;
        int disty = 0;
        int xPos = 0;
        int yPos = 0;
        for (int n = 0; n < (doorAmount * roomAmount) - 1; n++)
        {
            distx = xDoor[n] - xDoor[n + 1];
            disty = yDoor[n] - yDoor[n + 1];

            ////Console.WriteLine((char)(65+n));
            ////Console.Write(distx);
            ////Console.Write(",");
            ////Console.WriteLine(disty);

            xPos = xDoor[n];
            yPos = yDoor[n];

            int dist = 0;
            int loop = 0;

            bool yBlocked = false;
            bool xBlocked = false;

            while (Mathf.Abs(distx) > dist || Mathf.Abs(disty) > dist)
            {
                loop++;

                if (Mathf.Abs(distx) > dist && Pos[xPos - Mathf.RoundToInt(Mathf.Sign(distx)), yPos] != 'O' && Pos[xPos - Mathf.RoundToInt(Mathf.Sign(distx)), yPos] != 'M' && xBlocked == false)
                {
                    xPos -= Mathf.RoundToInt(Mathf.Sign(distx));
                    distx -= Mathf.RoundToInt(Mathf.Sign(distx));
                }
                else if (Mathf.Abs(disty) > dist && Pos[xPos, yPos - Mathf.RoundToInt(Mathf.Sign(disty))] != 'O' && Pos[xPos, yPos - Mathf.RoundToInt(Mathf.Sign(disty))] != 'M' && yBlocked == false)
                {
                    yPos -= Mathf.RoundToInt(Mathf.Sign(disty));
                    disty -= Mathf.RoundToInt(Mathf.Sign(disty));
                }
                else if (Mathf.Abs(distx) > 0 && (Pos[xPos - Mathf.RoundToInt(Mathf.Sign(distx)), yPos] == 'O' ||
                Pos[xPos - Mathf.RoundToInt(Mathf.Sign(distx)), yPos] == 'M'))
                {
                    if (Pos[xPos, yPos - 1] != '.')
                    {
                        yPos += 1;
                        disty += 1;
                    }
                    else
                    {
                        yPos -= 1;
                        disty -= 1;
                    }

                    yBlocked = true;
                    xBlocked = false;
                    ////Console.WriteLine("BLOCKED IN X DIRECTION");
                }
                else if (Mathf.Abs(disty) > 0 && (Pos[xPos, yPos - Mathf.RoundToInt(Mathf.Sign(disty))] == 'O' ||
                Pos[xPos, yPos - Mathf.RoundToInt(Mathf.Sign(disty))] == 'M'))
                {
                    if (Pos[xPos - 1, yPos] != '.')
                    {
                        xPos += 1;
                        distx += 1;
                    }
                    else
                    {
                        xPos -= 1;
                        distx -= 1;
                    }

                    xBlocked = true;
                    yBlocked = false;
                    ////Console.WriteLine("BLOCKED IN Y DIRECTION");
                }
                else
                {
                    yBlocked = false;
                    xBlocked = false;
                }

                /*else if(Mathf.Abs(distx) > 0 && Pos[xPos - Mathf.RoundToInt(Mathf.Sign(distx), yPos] != 'O' &&
				Pos[xPos - Mathf.RoundToInt(Mathf.Sign(distx), yPos] != 'M'' && yBlocked == true')
				{
					yPos -= 1;
					disty -= 1;
					yBlocked = false;
				}
				else if(Mathf.Abs(disty) > 0 && Pos[xPos, yPos - Mathf.RoundToInt(Mathf.Sign(disty)] != 'O' &&
				Pos[xPos, yPos - Mathf.RoundToInt(Mathf.Sign(disty)] != 'M' && xBlocked == true)
				{
					xPos -= 1;
					distx -= 1;
					xBlocked = false;
				}*/

                ////Console.WriteLine("{0}:({1}x{2}) to {3}:({4}x{5})", (char)(65+n), xPos, yPos, (char)(66+n), xDoor[n+1], yDoor[n+1]);
                ////Console.WriteLine("   Distx: {0}, Disty: {1}",distx, disty);

                if (Pos[xPos, yPos] == 'O' || Pos[xPos, yPos] == '.')
                    Pos[xPos, yPos] = 'Z';

                if (loop > 100 || xPos == 0 || yPos == 0)
                {
                    loop = 0;
                    //break; break;

                    return 1;
                }
            }

            wrapPath();
            unwrapDoors();
        }
        return 0;
    }

    void unwrapDoors()
    {
        for (int i = 0; i < max_height; i++)
        {
            for (int j = 0; j < max_width; j++)
            {
                if (Pos[j, i] == 'A' | Pos[j, i] == 'B' | Pos[j, i] == 'C' |
                Pos[j, i] == 'D' | Pos[j, i] == 'E' | Pos[j, i] == 'F' |
                Pos[j, i] == 'G' | Pos[j, i] == 'H' | Pos[j, i] == 'I')
                {
                    char left = 'X';
                    char right = 'X';
                    char up = 'X';
                    char down = 'X';

                    if (j != 0)
                        left = Pos[j - 1, i];
                    if (j < max_width - 1)
                        right = Pos[j + 1, i];
                    if (i != 0)
                        up = Pos[j, i - 1];
                    if (i < max_height - 1)
                        down = Pos[j, i + 1];

                    if (left == 'O')
                        Pos[j - 1, i] = '.';
                    if (right == 'O')
                        Pos[j + 1, i] = '.';
                    if (up == 'O')
                        Pos[j, i - 1] = '.';
                    if (down == 'O')
                        Pos[j, i + 1] = '.';
                }
            }
        }
    }

    void wrapPath()
    {
        for (int i = 0; i < max_height; i++)
        {
            for (int j = 0; j < max_width; j++)
            {
                if (Pos[j, i] == 'Z')
                {
                    char left = 'X';
                    char right = 'X';
                    char up = 'X';
                    char down = 'X';

                    char left2 = 'X';
                    char right2 = 'X';
                    char up2 = 'X';
                    char down2 = 'X';

                    char upleft = 'X';
                    char upright = 'X';
                    char downleft = 'X';
                    char downright = 'X';

                    char upleft2 = 'X';
                    char upright2 = 'X';
                    char downleft2 = 'X';
                    char downright2 = 'X';

                    char upleft3 = 'X';
                    char upright3 = 'X';
                    char downleft3 = 'X';
                    char downright3 = 'X';

                    char upleft4 = 'X';
                    char upright4 = 'X';
                    char downleft4 = 'X';
                    char downright4 = 'X';

                    if (j != 0)
                        left = Pos[j - 1, i];
                    if (j < max_width - 1)
                        right = Pos[j + 1, i];
                    if (i != 0)
                        up = Pos[j, i - 1];
                    if (i < max_height - 1)
                        down = Pos[j, i + 1];

                    if (j > 1)
                        left2 = Pos[j - 2, i];
                    if (j < max_width - 2)
                        right2 = Pos[j + 2, i];
                    if (i > 1)
                        up2 = Pos[j, i - 2];
                    if (i < max_height - 2)
                        down2 = Pos[j, i + 2];

                    if (j > 0 && i > 0)
                        upleft = Pos[j - 1, i - 1];
                    if (j < max_width - 1 && i > 0)
                        upright = Pos[j + 1, i - 1];
                    if (j > 0 && i < max_height - 1)
                        downleft = Pos[j - 1, i + 1];
                    if (j < max_width - 1 && i < max_height - 1)
                        downright = Pos[j + 1, i + 1];

                    if (j > 0 && i > 1)
                        upleft2 = Pos[j - 1, i - 2];
                    if (j < max_width - 1 && i > 1)
                        upright2 = Pos[j + 1, i - 2];
                    if (j > 0 && i < max_height - 2)
                        downleft2 = Pos[j - 1, i + 2];
                    if (j < max_width - 1 && i < max_height - 2)
                        downright2 = Pos[j + 1, i + 2];

                    if (j > 1 && i > 0)
                        upleft3 = Pos[j - 2, i - 1];
                    if (j < max_width - 2 && i > 0)
                        upright3 = Pos[j + 2, i - 1];
                    if (j > 1 && i < max_height - 1)
                        downleft3 = Pos[j - 2, i + 1];
                    if (j < max_width - 2 && i < max_height - 1)
                        downright3 = Pos[j + 2, i + 1];

                    if (j > 1 && i > 1)
                        upleft4 = Pos[j - 2, i - 2];
                    if (j < max_width - 2 && i > 1)
                        upright4 = Pos[j + 2, i - 2];
                    if (j > 1 && i < max_height - 2)
                        downleft4 = Pos[j - 2, i + 2];
                    if (j < max_width - 2 && i < max_height - 2)
                        downright4 = Pos[j + 2, i + 2];

                    if (left == '.')
                        Pos[j - 1, i] = 'O';
                    if (right == '.')
                        Pos[j + 1, i] = 'O';
                    if (up == '.')
                        Pos[j, i - 1] = 'O';
                    if (down == '.')
                        Pos[j, i + 1] = 'O';

                    if (left2 == '.')
                        Pos[j - 2, i] = 'O';
                    if (right2 == '.')
                        Pos[j + 2, i] = 'O';
                    if (up2 == '.')
                        Pos[j, i - 2] = 'O';
                    if (down2 == '.')
                        Pos[j, i + 2] = 'O';

                    if (upleft == '.')
                        Pos[j - 1, i - 1] = 'O';
                    if (upright == '.')
                        Pos[j + 1, i - 1] = 'O';
                    if (downleft == '.')
                        Pos[j - 1, i + 1] = 'O';
                    if (downright == '.')
                        Pos[j + 1, i + 1] = 'O';

                    if (upleft2 == '.')
                        Pos[j - 1, i - 2] = 'O';
                    if (upright2 == '.')
                        Pos[j + 1, i - 2] = 'O';
                    if (downleft2 == '.')
                        Pos[j - 1, i + 2] = 'O';
                    if (downright2 == '.')
                        Pos[j + 1, i + 2] = 'O';

                    if (upleft3 == '.')
                        Pos[j - 2, i - 1] = 'O';
                    if (upright3 == '.')
                        Pos[j + 2, i - 1] = 'O';
                    if (downleft3 == '.')
                        Pos[j - 2, i + 1] = 'O';
                    if (downright3 == '.')
                        Pos[j + 2, i + 1] = 'O';

                    if (upleft4 == '.')
                        Pos[j - 2, i - 2] = 'O';
                    if (upright4 == '.')
                        Pos[j + 2, i - 2] = 'O';
                    if (downleft4 == '.')
                        Pos[j - 2, i + 2] = 'O';
                    if (downright4 == '.')
                        Pos[j + 2, i + 2] = 'O';
                }
            }
        }
    }

    void corners()
    {
        for (int check = 0; check < 2; check++)
        {
            for (int i = 0; i < max_height; i++)
            {
                for (int j = 0; j < max_width; j++)
                {
                    if (Pos[j, i] == '.')
                    {
                        char left = '.';
                        char right = '.';
                        char up = '.';
                        char down = '.';

                        if (j != 0)
                            left = Pos[j - 1, i];
                        if (j < max_width - 1)
                            right = Pos[j + 1, i];
                        if (i != 0)
                            up = Pos[j, i - 1];
                        if (i < max_height - 1)
                            down = Pos[j, i + 1];

                        if ((left == '-' || left == '┌' || left == '└') &&
                            (right != '-' && right != '┐' && right != '┘') &&
                            (up != '|' && up != '┌' && up != '┐') &&
                            (down == '|' || down == '┘' || down == '└'))
                        {
                            Pos[j, i] = '┐';
                        }
                        else if ((left != '-' && left != '┌' && left != '└') &&
                            (right == '-' || right == '┐' || right == '┘') &&
                            (up != '|' && up != '┌' && up != '┐') &&
                            (down == '|' || down == '┘' || down == '└'))
                        {
                            Pos[j, i] = '┌';
                        }
                        else if ((left != '-' && left != '┌' && left != '└') &&
                            (right == '-' || right == '┐' || right == '┘') &&
                            (up == '|' || up == '┌' || up == '┐') &&
                            (down != '|' && down != '┘' && down != '└'))
                        {
                            Pos[j, i] = '└';
                        }
                        else if ((left == '-' || left == '┌' || left == '└') &&
                            (right != '-' && right != '┐' && right != '┘') &&
                            (up == '|' || up == '┌' || up == '┐') &&
                            (down != '|' && down != '┘' && down != '└'))
                        {
                            Pos[j, i] = '┘';
                        }

                        if (left == 'O' && right != 'O' && up != 'O' && down == 'O')
                        {
                            Pos[j, i] = '└';
                        }
                        else if (left != 'O' && right == 'O' && up != 'O' && down == 'O')
                        {
                            Pos[j, i] = '┘';
                        }
                        else if (left != 'O' && right == 'O' && up == 'O' && down != 'O')
                        {
                            Pos[j, i] = '┐';
                        }
                        else if (left == 'O' && right != 'O' && up == 'O' && down != 'O')
                        {
                            Pos[j, i] = '┌';
                        }
                    }
                }
            }
        }
    }

    int walls()
    {
        for (int i = 0; i < max_height; i++)
        {
            for (int j = 0; j < max_width; j++)
            {
                if (Pos[j, i] == '.')
                {
                    char left = '.';
                    char right = '.';
                    char up = '.';
                    char down = '.';

                    if (j != 0)
                        left = Pos[j - 1, i];
                    if (j < max_width - 1)
                        right = Pos[j + 1, i];
                    if (i != 0)
                        up = Pos[j, i - 1];
                    if (i < max_height - 1)
                        down = Pos[j, i + 1];

                    if (left == 'O' && right != 'O' && up != 'O' && down != 'O')
                    {
                        Pos[j, i] = '|';
                    }
                    else if (left != 'O' && right == 'O' && up != 'O' && down != 'O')
                    {
                        Pos[j, i] = '|';
                    }
                    else if (left != 'O' && right != 'O' && up != 'O' && down == 'O')
                    {
                        Pos[j, i] = '-';
                    }
                    else if (left != 'O' && right != 'O' && up == 'O' && down != 'O')
                    {
                        Pos[j, i] = '-';
                    }
                    else if (left != 'O' && right != 'O' && up == 'O' && down == 'O')
                    {
                        Pos[j, i] = '=';
                        return 1;
                    }
                    else if (left == 'O' && right != 'O' && up == 'O' && down == 'O')
                    {
                        Pos[j, i] = 'C';
                        return 1;
                    }
                    else if (left != 'O' && right == 'O' && up == 'O' && down == 'O')
                    {
                        Pos[j, i] = 'D';
                        return 1;
                    }
                    else if (left == 'O' && right == 'O' && up != 'O' && down != 'O')
                    {
                        Pos[j, i] = 'H';
                        return 1;
                    }
                    else if (left == 'O' && right == 'O' && up == 'O' && down != 'O')
                    {
                        Pos[j, i] = 'N';
                        return 1;
                    }
                    else if (left == 'O' && right == 'O' && up != 'O' && down == 'O')
                    {
                        Pos[j, i] = 'U';
                        return 1;
                    }
                }
            }
        }
        return 0;
    }

    void printRooms()
    {
        for (int i = 0; i < max_height; i++)
        {
            for (int j = 0; j < max_width; j++)
            {
                //Console.Write("{0}", Pos[j, i]);
            }
            //Console.WriteLine("");
        }
    }

    void makeRoom(int x, int y, int width, int height, bool spawn)
    {
        int randx = 4;
		int randy = 4;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (i > 1 && i < height - 2 && j > 1 && j < width - 2)
                {
                    Pos[x + j, y + i] = 'M';

                    if (spawn == true && randx == j && randy == i)
                    {
                        Pos[x + j, y + i] = 'Q';
                    }
                }
                else
                {
                    Pos[x + j, y + i] = 'O';
                }
            }
        }
        var rand = new Random();

        int dest = -1;

        for (int i = 0; i < doorAmount; i++)
        {
            dest = Random.Range(0, 4);

            int r;
            if (dest == 0)
            {
                r = Random.Range(2, height - 2);
                //Pos[x-1, y+r] = 'O';
                //Pos[x-2, y+r] = 'X';
                Pos[x + 1, y + r] = 'M';
                Pos[x, y + r] = 'M';
                Pos[x - 1, y + r] = 'X';
            }
            else if (dest == 1)
            {
                r = Random.Range(2, height - 2);
                //Pos[x+width, y+r] = 'O';
                //Pos[x+width+1, y+r] = 'X';
                Pos[x + width - 2, y + r] = 'M';
                Pos[x + width - 1, y + r] = 'M';
                Pos[x + width, y + r] = 'X';
            }
            else if (dest == 2)
            {
                r = Random.Range(2, width - 2);
                //Pos[x+r, y+height] = 'O';
                //Pos[x+r, y+height+1] = 'X';
                Pos[x + r, y + height - 2] = 'M';
                Pos[x + r, y + height - 1] = 'M';
                Pos[x + r, y + height] = 'X';
            }
            else if (dest == 3)
            {
                r = Random.Range(2, width - 2);
                //Pos[x+r, y-1] = 'O';
                //Pos[x+r, y-2] = 'X';
                Pos[x + r, y + 1] = 'M';
                Pos[x + r, y] = 'M';
                Pos[x + r, y - 1] = 'X';
            }
        }
    }
}
