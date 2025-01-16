using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneAddCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        float scalex = GetComponent<BoxCollider2D>().size.x-0.5f;
        float scaley = GetComponent<BoxCollider2D>().size.y-0.5f;

        if (scalex > 0 && scaley > 0)
        {
            

            GameObject smallerCollider = new GameObject();
            smallerCollider.transform.parent = transform;
            smallerCollider.transform.localPosition = Vector3.zero;
            smallerCollider.transform.localRotation = Quaternion.identity;
            smallerCollider.transform.localScale = Vector3.one;

            BoxCollider2D boxCollider = smallerCollider.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(scalex, scaley);
            boxCollider.offset = GetComponent<BoxCollider2D>().offset;
        }

    }
}
