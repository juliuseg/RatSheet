using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zLayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    [ContextMenu("Run MyButtonFunction")]
    public void MyButtonFunction() {
        UpdatePosition();
    }
    
    void UpdatePosition(){
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y*0.001f);

    }
    
}
