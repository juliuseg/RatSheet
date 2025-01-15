using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class SelectionUtils {
    public static bool IsPointWithinBounds(Vector2 point, Vector2 bound1, Vector2 bound2, float padding = 0)
    {
        // Calculate min and max bounds
        float minX = Mathf.Min(bound1.x, bound2.x) - padding;
        float maxX = Mathf.Max(bound1.x, bound2.x) + padding;
        float minY = Mathf.Min(bound1.y, bound2.y) - padding;
        float maxY = Mathf.Max(bound1.y, bound2.y) + padding;

        // Check if the point is within the bounds
        return point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY;
    }

    public static Vector3 GetMousePos(Vector3 mousePos)
    {

        // Get the screen resolution
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        Vector2 refresolution = GameObject.Find("MainCanvas").GetComponent<CanvasScaler>().referenceResolution;
        float referenceWidth = refresolution.x;
        float referenceHeight = refresolution.y;

        float mousex = MapClamped(mousePos.x, 0, screenWidth, 0, referenceWidth);
        float mousey = MapClamped(mousePos.y, 0, screenHeight, 0, referenceHeight);

        float aspectRatioScreen = screenWidth / screenHeight;
        float aspectRatioRef = referenceWidth / referenceHeight;
        float yratio = referenceHeight / screenHeight;

        //print ($"aspectRatioScreen: {aspectRatioScreen}, aspectRatioRef: {aspectRatioRef}, yratio: {yratio}");
        return  new Vector3(mousex, mousey*aspectRatioRef/aspectRatioScreen, 0);
    }

    public static float MapClamped(float value, float inMin, float inMax, float outMin, float outMax)
    {
        float mappedValue = outMin + (value - inMin) * (outMax - outMin) / (inMax - inMin);
        return Mathf.Clamp(mappedValue, Mathf.Min(outMin, outMax), Mathf.Max(outMin, outMax));
    }
    

    public static bool IsPointerOverUIElement()
    {
        bool isPointerOverUIElement = IsPointerOverUIElement(GetEventSystemRaycastResults());
        //print("IsPointerOverUIElement: " + isPointerOverUIElement);
        return isPointerOverUIElement;
    }

    private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        int UILayer = LayerMask.NameToLayer("UI");
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }

    public static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = SelectionUtils.GetMousePos(Input.mousePosition);
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}