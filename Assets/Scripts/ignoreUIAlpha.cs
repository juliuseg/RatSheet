using UnityEngine;
using UnityEngine.UI;


public class ignoreUIAlpha : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
    }
}
