using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Controller2D : RayCastController
{
    public float maxSlopeAngle = 60;

    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 playerInput;

    public override void Start() 
    {
        base.Start();
        collisions.faceDir = 1;
    }

    public void Move(Vector2 moveAmount)
    {
        UpdateRaycastOrigins();

        collisions.Reset();
        collisions.moveAmountOld = moveAmount;

        if (moveAmount.y < 0)
        {
            DescendSlope(ref moveAmount);
        }

        if (moveAmount.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
        }

        HorizontalCollisions(ref moveAmount);
        if (moveAmount.y != 0)
        {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount);
    }

    private void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? rayCastPoints.bottomLeft : rayCastPoints.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, LayerMask.GetMask(layerNames));

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                if(hit.collider.gameObject.tag == "knockback")
                {
                    collisions.knockBackX = true;
                }

                if (hit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    //If we start a ascend immediatly after a descend
                    if(collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        moveAmount = collisions.moveAmountOld;
                    }

                    float distanceToSlopeStart = 0;
                    if(slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
                    moveAmount.x += distanceToSlopeStart * directionX;//we pass velocity as ref hence need to reset
                }

                if(!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    //using min cause since we are on slope our X displacement is less than when moving on flat surface may not yet be in collision
                    moveAmount.x = Mathf.Min(Mathf.Abs(moveAmount.x), (hit.distance - skinWidth)) * directionX;
                    rayLength = Mathf.Min(Mathf.Abs(moveAmount.x) + skinWidth, hit.distance);

                    if (collisions.climbingSlope)
                    {
                        moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    private void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmount.x);
        float climbVelocity = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

/*        
        If player jumps velocity.y set before in player script hence dont override it if player is in jump
        also we dont want collisions.climbingslope = true as Airborne velocity is handled by the if condition
        after the call to this func in HorizontalCollisions func
*/
        if (moveAmount.y <= climbVelocity) 
        {
            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
            moveAmount.y = climbVelocity;
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector2 moveAmount)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(rayCastPoints.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, LayerMask.GetMask(layerNames));
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(rayCastPoints.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + skinWidth, LayerMask.GetMask(layerNames));
/*        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {*/
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        //}

        if (!collisions.slidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? rayCastPoints.bottomRight : rayCastPoints.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, LayerMask.GetMask(layerNames));

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    private void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;

            }
        }
    }

    private void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? rayCastPoints.bottomLeft : rayCastPoints.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, LayerMask.GetMask(layerNames));

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                if (hit.collider.gameObject.tag == "knockback")
                {
                    collisions.knockBackY = true;
                }

                moveAmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        //If ther is change in slope angle the to avoid jitter im movement 
        if(collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? rayCastPoints.bottomLeft : rayCastPoints.bottomRight) + Vector2.up * moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, LayerMask.GetMask(layerNames));

            if(hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(slopeAngle != collisions.slopeAngle)
                {
                    moveAmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        
        }
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public bool knockBackX;
        public bool knockBackY;

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector2 moveAmountOld;
        public int faceDir;
        public bool fallingThroughPlatform;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
