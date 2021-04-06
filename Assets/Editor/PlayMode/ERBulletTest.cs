using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class ERBulletTest
    {
        private GameObject[] getCount;
        //private GameObject[] bulletList;

        GameObject bulletObject;
        bool inBounds;

        [OneTimeSetUp]
        public void Setup()
        {
            SceneManager.LoadScene("StressTestScene");
        }

        [UnityTest]
        public IEnumerator TestSpeed()
        {
            yield return new WaitForSeconds(5.0f);

            for (int j = 16; j < 496; j *= 2)
            {
                Debug.Log("Bullet Speed: " + j);
                bulletObject = Resources.Load("Bullet_Plasma") as GameObject;

                var bulletAmount = 130; //increase to stress test bullet amount
                float angle = 0;

                for (int i = 0; i < bulletAmount; i++)
                {
                    GameObject bullet = GameObject.Instantiate(bulletObject, new Vector3(0, 0, 0), Quaternion.identity);
                    BulletScript bulletScript = bullet.GetComponent<BulletScript>();
                    bullet.tag = "Bullet";
                    bulletScript.bulletAngle = angle;
                    bulletScript.bulletSpeed = j; //increase to stress test bullet speed
                    bulletScript.bulletRange = 1000000;
                    bulletScript.passThrough = 100;
                    bulletScript.ricochet = 0; //increase to stress test richochet amount
                    angle += 0.05f;
                    yield return new WaitForSeconds(0.01f);
                }

                yield return new WaitForSeconds(2f);

                //yield return new WaitUntil(() => inBounds == false);

                getCount = GameObject.FindGameObjectsWithTag("Bullet");
                var bulletCurrent = getCount.Length;

                Assert.AreEqual(0, bulletCurrent);

                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator TestBullets()
        {
            yield return new WaitForSeconds(5.0f);
            bulletObject = Resources.Load("Bullet_Plasma") as GameObject;

            float bulletAmount = 300; //increase to stress test bullet amount
            float angle = 0;

            for (float i = 0; i < bulletAmount; i++)
            {
                GameObject bullet = GameObject.Instantiate(bulletObject, new Vector3(0, 0, 0), Quaternion.identity);
                BulletScript bulletScript = bullet.GetComponent<BulletScript>();
                bullet.tag = "Bullet";
                bulletScript.bulletAngle = angle;
                bulletScript.bulletSpeed = 10; //increase to stress test bullet speed
                bulletScript.bulletRange = 1000000;
                bulletScript.passThrough = 100;
                bulletScript.ricochet = 0; //increase to stress test richochet amount
                angle += 0.05f;

                var deltaTime = 0.0;
                deltaTime += Time.deltaTime;
                deltaTime /= 2.0;
                var fps = 1.0 / deltaTime;
                //Debug.Log("FPS: " + fps);

                float delayInMs = 0.1f * (1f - (i / bulletAmount));
                Debug.Log(0.5f * (1f - (i / bulletAmount)));
                float ms = Time.deltaTime;

                while (ms <= delayInMs)
                {
                    ms += Time.deltaTime;
                    yield return null;
                }

                //yield return new WaitForSeconds(0);//0.01f / i);
            }

            yield return new WaitForSeconds(2f);

            //yield return new WaitUntil(() => inBounds == false);

            getCount = GameObject.FindGameObjectsWithTag("Bullet");
            var bulletCurrent = getCount.Length;

            Assert.AreEqual(0, bulletCurrent);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestRicochet()
        {
            yield return new WaitForSeconds(5.0f);

            for (int j = 0; j < 5; j++)
            {
                Debug.Log("Ricochet Amount: " + j);
                bulletObject = Resources.Load("Bullet_Plasma") as GameObject;

                var bulletAmount = 130; //increase to stress test bullet amount
                float angle = 0;

                for (int i = 0; i < bulletAmount; i++)
                {
                    GameObject bullet = GameObject.Instantiate(bulletObject, new Vector3(0, 0, 0), Quaternion.identity);
                    BulletScript bulletScript = bullet.GetComponent<BulletScript>();
                    bullet.tag = "Bullet";
                    bulletScript.bulletAngle = angle;
                    bulletScript.bulletSpeed = 10; //increase to stress test bullet speed
                    bulletScript.bulletRange = 1000000;
                    bulletScript.passThrough = 100;
                    bulletScript.ricochet = j; //increase to stress test richochet amount
                    angle += 0.05f;
                    yield return new WaitForSeconds(0.01f);
                }

                yield return new WaitForSeconds(2f);

                //yield return new WaitUntil(() => inBounds == false);

                getCount = GameObject.FindGameObjectsWithTag("Bullet");
                var bulletCurrent = getCount.Length;

                Assert.AreEqual(0, bulletCurrent);

                yield return null;
            }
        }
    }
}