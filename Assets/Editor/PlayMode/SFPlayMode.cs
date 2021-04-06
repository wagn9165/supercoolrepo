using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class HazardAnimationTest
    {
        GameObject explosion;
        GameObject barrelObject;
        GameObject newBarrel;
        GameObject bulletObject;
        // A Test behaves as an ordinary method
        [OneTimeSetUp]
        public void LoadScene()
        {
            SceneManager.LoadScene("StressTestScene");
        }


        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestAnimation()
        {
            barrelObject = Resources.Load("Barrel") as GameObject;
            yield return new WaitForSeconds(.01f);
            GameObject barrel = GameObject.Instantiate(barrelObject, new Vector3(0, 0, 0), Quaternion.identity);
            yield return new WaitForSeconds(1f);
            bulletObject = Resources.Load("Bullet_Iron") as GameObject;
            GameObject bullet = GameObject.Instantiate(bulletObject, new Vector3(0, 0, 0), Quaternion.identity);

            yield return new WaitForSeconds(3f);
            GameObject[] end = GameObject.FindGameObjectsWithTag("Hazard");
            var length = end.Length;
            Debug.Log($"Length {length}");
            Assert.AreEqual(0, length);
        }

        [UnityTest]
        public IEnumerator StressTest()
        {
            yield return new WaitForSeconds(3f);
            var deltaTime = 0.0;
            var fps = 0.0;
            var i = 0;
            explosion = Resources.Load("explosion") as GameObject;

            explosionScript ExplosionScript = explosion.GetComponent<explosionScript>();
            ExplosionScript.dontDestroy = true;

            deltaTime += Time.deltaTime;
            deltaTime /= 2.0;
            fps = 1.0 / deltaTime;

            while (fps > 10f)
            {
                GameObject explo = GameObject.Instantiate(explosion, new Vector3(0, 0, 0), Quaternion.identity);
                i++;
                deltaTime += Time.deltaTime;
                deltaTime /= 2.0;
                fps = 1.0 / deltaTime;
                Debug.Log($"Total Explosions: {i} total FPS: {fps}");
                yield return new WaitForSeconds(0.01f);
            }
            Assert.That(fps < 10f);
        }
    }
}
