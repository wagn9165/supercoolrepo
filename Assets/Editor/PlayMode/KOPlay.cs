using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class KOPlay
    {
        // A Test behaves as an ordinary method
        [OneTimeSetUp]
        public void LoadScene()
        {
            // Use the Assert class to test conditions
            SceneManager.LoadScene("StressTestScene");
        }
        [UnityTest]
        public IEnumerator EnemiesSpawn()
        {
            for (int i = 0; i < 600; i++) //This one is long so that the scene gets fully loaded
            {
                yield return null;
            }
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            int numberOfEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
            Assert.That(numberOfEnemies > 5);
        }
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator GunsLoad() //This one piggy backs of the other where the scene is alredy loaded
        {
            for (int i = 0; i < 600; i++) //This one is long so that the scene gets fully loaded
            {
                yield return null;
            }
            GameObject gun = GameObject.FindWithTag("Weapon");
            Assert.True(gun.activeInHierarchy);
        }
        /*[UnityTest] //Still a work in progress
        public IEnumerator Stress() //This one piggy backs of the other where the scene is alredy loaded
        {
           yield return null;//It gets angry when this is not included, doesn't hurt anything
           for (i<1000)
            {
               GameObject weapon; //This contains the weapon object
               GameObject weapon1; //This contains the weapon object

               weapon = Instantiate(weapon1, transform.position, Quaternion.identity) as GameObject;
               weaponScript = weapon.GetComponent<GunScript>();
               weaponScript.parent = gameObject;
               weaponScript.Hand = Hand;
               weaponRender = weapon.GetComponent<SpriteRenderer>();
               render = GetComponent<SpriteRenderer>();
               shootCheck = true;
               Debug.log("Count: " + i);
               i++;
            }

        }*/

    }
}
