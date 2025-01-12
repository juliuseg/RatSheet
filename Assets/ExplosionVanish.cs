using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionVanish : MonoBehaviour
{
    public float vanishTimeMax = 1.0f;

    private float vanishTime;

    // Start is called before the first frame update
    void Start()
    {
        vanishTime = vanishTimeMax;
    }

    // Update is called once per frame
    void Update()
    {
        vanishTime -= Time.deltaTime;

        float vanishPer = (vanishTime / vanishTimeMax);
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, vanishPer);
        if (vanishTime <= 0)
        {


            Destroy(gameObject);
        }
    }


}
