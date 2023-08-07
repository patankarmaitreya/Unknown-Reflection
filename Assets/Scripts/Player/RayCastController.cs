using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RayRange
{
    public RayRange(float x1, float y1, float x2, float y2, Vector2 dir)
    {
        Start = new Vector2(x1, y1);
        End = new Vector2(x2, y2);
        Dir = dir;
    }

    public readonly Vector2 Start, End, Dir;
}

[RequireComponent(typeof(BoxCollider2D))]
public class RayCastController : MonoBehaviour
{
    public string[] layerNames = { "ObstacleLeft", "ObstacleRight", "Mirror" };

    public const float skinWidth = .015f;
    const float dstBetweenRays = .25f;
    [HideInInspector]
    public int horizontalRayCount;
    [HideInInspector]
    public int verticalRayCount;

    protected RayRange _raysUp, _raysRight, _raysDown, _raysLeft;

    [HideInInspector]
    public float horizontalRaySpacing;
    [HideInInspector]
    public float verticalRaySpacing;

    [HideInInspector]
    public BoxCollider2D playerCollider;

    public RaycastPoints rayCastPoints;

    public virtual void Awake()
    {
        playerCollider = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void UpdateRaycastOrigins()
    {
        Bounds bounds = playerCollider.bounds;
        bounds.Expand(skinWidth * -2);

        rayCastPoints.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        rayCastPoints.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        rayCastPoints.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        rayCastPoints.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = playerCollider.bounds;
        bounds.Expand(skinWidth * -2);

        float boundsWidth = bounds.size.x;
        float boundsHeight = bounds.size.y;

        horizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
        verticalRayCount = Mathf.RoundToInt(boundsWidth / dstBetweenRays);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastPoints
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
