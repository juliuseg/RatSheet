using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightInGameCont : MonoBehaviour
{

    public float intensity;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Light2D>().intensity = intensity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
