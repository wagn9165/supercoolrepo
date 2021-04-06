using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ERBulletDamage
    {
        [Test]
        public void Damage()
        {
            //This creates a new object
            var bullet = new GameObject();
            var bulletScript = bullet.AddComponent<BulletScript>();
            bulletScript.bulletDamage = 100;
            var tDamage = bulletScript.bulletDamage;
            Debug.Log($"Damage: {tDamage}");
            Assert.That(tDamage == 100);
        }
    }
}