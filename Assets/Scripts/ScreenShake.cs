using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is given to the the players gun object, which will make the screen shake if shot////////////////////////////////////
public class ScreenShake : MonoBehaviour
{
    Transform target;
    public Vector3 initialPos;

    float threshold = 1;
    float intensity = 0.5f;
    float givenDuration = 0;

    public float deltaTime;

    // Start is called before the first frame update
    void Start()
    {
        target = GetComponent<Transform>();
        initialPos = target.localPosition;
    }

    public float pendingShakeDuration = 0.0f;

    public void Shake(float duration, float intense)
    {
        if (duration > 0)
        {
            isShaking = false;

            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;

            pendingShakeDuration = duration * fps;
            givenDuration = duration * fps;
            intensity = intense;
        }
    }

    bool isShaking = false;

    // Update is called once per frame
    void Update()
    {
        if (pendingShakeDuration > 0 && !isShaking)
        {
            StartCoroutine(DoShake());
        }
    }

    IEnumerator DoShake()
    {
        isShaking = true;

        //var startTime = Time.realtimeSinceStartup;
        //while(Time.realtimeSinceStartup - startTime < pendingShakeDuration)
        while (pendingShakeDuration > 0)
        {
            pendingShakeDuration--;
            //threshold = (1 - ((Time.realtimeSinceStartup - startTime) / pendingShakeDuration)) * intensity;
            threshold = (pendingShakeDuration / givenDuration) * (intensity/100);

            target.localPosition = initialPos;
            var randomPoint = new Vector3(Random.Range(-1f, 1f) * threshold, Random.Range(-1f, 1f) * threshold, -10);
            target.localPosition += randomPoint;

            yield return null;
        }

        pendingShakeDuration = 0.0f;
        target.localPosition = initialPos;
        isShaking = false;
    }
}
