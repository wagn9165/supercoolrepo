using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RendererScript : MonoBehaviour
{
    List<GameObject> rootObjects;
    public GameObject Grid;
    Scene scene;

    // Start is called before the first frame update
    void Start()
    {
        // get root objects in scene
        rootObjects = new List<GameObject>();
        scene = SceneManager.GetActiveScene();
        scene.GetRootGameObjects(rootObjects);
    }

    // Update is called once per frame
    void Update()
    {
        if (Global.startRender)
        {
            scene.GetRootGameObjects(rootObjects);

            // iterate root objects and do something
            for (int i = 0; i < rootObjects.Count; ++i)
            {
                GameObject Object = rootObjects[i];
                if (Vector2.Distance(transform.position, Object.transform.position) < 20)
                {
                    Object.SetActive(true);
                }
                else if (Object.name != "RoomLight" && Object.name != "Player" &&
                    Object.name != "Background" && Object.name != "Grid" &&
                    Object.name != "TempBullets" && Object.name != "TempRicochet" &&
                    Object.name != "TempEnemies" && Object.name != "PlayerSpawn(Clone)" &&
                    Object.name != "Main Camera" && Object.name != "Cursor(Clone)" &&
                    Object.name != "A*" && Object.tag != "Enemy")
                {
                    Object.SetActive(false);
                    //Object.GetComponent<SpriteRenderer>().enabled = false;
                }
            }

            // iterate Grid objects and do something
            foreach(Transform folder in Grid.transform)
            {
                foreach (Transform tile in folder.transform)
                {
                    GameObject Object = tile.gameObject;
                    if (Vector2.Distance(transform.position, Object.transform.position) < 20)
                    {
                        Object.SetActive(true);
                    }
                    else
                    {
                        Object.SetActive(false);
                    }
                }
            }
        }
    }
}
