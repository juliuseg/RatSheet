using System;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    public Transform startPoint; // Starting point
    public Transform endPoint;   // Ending point

    [Range(2, 50)] public int segments = 20; // Number of points on the curve

    private float t;
    public float baseSpeed = 1f; // Base speed for scaling
    private float scaledSpeed;

    public LayerMask targetLayer; // Layer to check

    public float radius = 1f; // Radius of the sphere

    public GameObject explosion;


    private void Start()
    {
        t = 0;

        
    }

    private void OnDrawGizmos()
    {
        if (startPoint == null || endPoint == null) return;

        Vector3 previousPoint = startPoint.position;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 currentPoint = CalculateBezier(t, startPoint.position, endPoint.position);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }

    private void Update()
    {
        if (startPoint == null || endPoint == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            t = 0;
        }

        transform.position = CalculateBezier(t, startPoint.position, endPoint.position);

        // Calculate the scaled speed based on the distance
        float distance = Vector3.Distance(startPoint.position, endPoint.position);
        scaledSpeed = baseSpeed / distance;

        // Increment t
        t = Mathf.Clamp01(t + Time.deltaTime * scaledSpeed);

        if (t == 1.0f){
            Impact();
        }
    }

    void Impact(){
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);

        foreach (Collider2D hitCollider in hitColliders)
        {
            AgentControllerBoid agent = hitCollider.GetComponent<AgentControllerBoid>();
            if (agent != null && Vector2.Distance(transform.position,hitCollider.transform.position) < radius)
            {
                agent.health.TakeDamage(2);

            }
        }

        GameObject ex = Instantiate(explosion, transform.position, Quaternion.identity);
        ex.transform.localScale = new Vector3(radius*2, radius*2, 1);
        Destroy(gameObject);
    }

    private Vector3 CalculateBezier(float t, Vector3 p0, Vector3 p3)
    {
        Vector3 p1 = new Vector3(p0.x, 3.0f + p0.y + Math.Abs(p0.y - p3.y)/2, 0);
        Vector3 p2 = new Vector3(p3.x, 3.0f + p3.y + Math.Abs(p0.y - p3.y)/2, 0);

        //Vector3 p2 = new Vector3((p0.x + p3.x) / 2, p3.y + Math.Abs(p0.y - p3.y), 0);

        return CalculateCubicBezierPoint(t, p0, p1, p2, p3);
    }

    private Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        // Compute the cubic Bézier point
        Vector3 point = (uuu * p0) + // (1 - t)^3 * P0
                        (3 * uu * t * p1) + // 3 * (1 - t)^2 * t * P1
                        (3 * u * tt * p2) + // 3 * (1 - t) * t^2 * P2
                        (ttt * p3); // t^3 * P3
        return point;
    }

}
