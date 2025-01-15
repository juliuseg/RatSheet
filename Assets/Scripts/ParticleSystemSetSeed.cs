using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemSetSeed : MonoBehaviour
{
    public uint seed;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ParticleSystem>().randomSeed = seed;
        GetComponent<ParticleSystem>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
