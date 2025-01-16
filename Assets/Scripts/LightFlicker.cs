using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{

    public float flickerSpeed;

    public float minRad;
    public float maxRad;

    private Light2D l;
    
    // Start is called before the first frame update
    void Start()
    {
        l = GetComponent<Light2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float f = Mathf.Pow(Mathf.PerlinNoise(Time.time*flickerSpeed,0.5f)+0.5f, 1.6f);
        float mappedf = Map(1/f,0,1,minRad, maxRad);

        l.pointLightOuterRadius =  mappedf;
        l.intensity = mappedf*5;

        //print("light: " + f);
    }

    float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
}
