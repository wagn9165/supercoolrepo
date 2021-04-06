using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SFUnitTests
    {
        [Test]
        public void GivenDamage()
        {
            var go = new GameObject();
            var goScript = go.AddComponent<BarrelScript>();
            goScript.barrelDamage = 25f;
            var testDamage = goScript.barrelDamage;
            Debug.Log("Damage:" + testDamage);
            Assert.That(testDamage > 0);
        }
    }
}