using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionSystem : MonoBehaviour
{
    [Header("float values")]
    [SerializeField] private float coolDownDuration;

    private static float alphaValue = 0.5f;

    [Header("Players")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [Header("LeverPlatforms")]
    [SerializeField] private BoxCollider2D leverPlatform1;
    [SerializeField] private BoxCollider2D leverPlatform2;

    public bool leverOnCoolDown { get; private set; }

    [Header("List")]
    public List<GameObject> leftPlatforms;
    public List<GameObject> rightPlatforms;

    [SerializeField] private float angleOfRotation;
    [SerializeField] private float rotationDelay;
    [SerializeField] private float rotationFactor;
    [SerializeField] private float valueOfRoatation;

    private bool isRightRotated = false;
    private bool isLeftRotated = false;

    void Awake()
    {
        player1.GetComponent<Player>().triggRotation += RotateRightPlatforms;
        player2.GetComponent<Player>().triggRotation += RotateLeftPlatforms;

        leverOnCoolDown = false;
    }

    void RotateRightPlatforms()
    {
        if (!leverOnCoolDown && !isRightRotated)
        {
            //Debug.Log("in");
            StartCoroutine(LeverCoolDown());
            RotatePlatform(rightPlatforms);
        }
        isRightRotated = true;
        isLeftRotated = false;
    }

    void RotateLeftPlatforms()
    {
        if (!leverOnCoolDown && !isLeftRotated)
        {
            //Debug.Log("in");
            StartCoroutine(LeverCoolDown());
            RotatePlatform(leftPlatforms);
        }
        isRightRotated = false;
        isLeftRotated = true;
    }

    IEnumerator LeverCoolDown()
    {
        leverOnCoolDown = true;
        DeactivateLever(alphaValue);
        yield return new WaitForSeconds(coolDownDuration);
        leverOnCoolDown = false;
        DeactivateLever(-alphaValue);
        yield break;
    }

    void DeactivateLever(float alphaValue)
    {
        leverPlatform1.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, alphaValue);
        leverPlatform2.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, alphaValue);
        leverPlatform1.enabled = leverPlatform1.enabled == false;
        leverPlatform2.enabled = leverPlatform2.enabled == false;
    }

    public void RotatePlatform(List<GameObject> Platforms)
    {
        foreach (GameObject platform in Platforms)
        {
            StartCoroutine(StartPlatformRotation(platform));
        }
    }

    IEnumerator StartPlatformRotation(GameObject platform)
    {
        float oldAngle = platform.transform.rotation.eulerAngles.z;
        if (angleOfRotation != platform.transform.rotation.z)
        {
            for (float theta = 0f; theta <= angleOfRotation; theta += rotationFactor)
            {
                if (Mathf.Abs(theta - angleOfRotation) < valueOfRoatation)
                {
                    platform.transform.localRotation = Quaternion.Euler(0, 0, angleOfRotation + oldAngle);
                    yield break;
                }
                platform.transform.Rotate(Vector3.forward, rotationFactor);
                yield return new WaitForSeconds(rotationDelay);
            }
        }
        yield break;
    }
}

#region Old code
/*    void LeverGroundCheckLeft()
    {
        Collider2D hitLeft = Physics2D.OverlapCircle(player1Prefab.transform.position, radius, 1 << LayerMask.NameToLayer("ObstacleLeft"));
        if (hitLeft && hitLeft.gameObject.tag == "LeverGroundLeft")
        {
            inRangePlayer1 = true;
        }
        else
        {
            inRangePlayer1 = false;
        }
    }

    void LeverGroundCheckRight()
    {
        Collider2D hitRight = Physics2D.OverlapCircle(player2Prefab.transform.position, radius, 1 << LayerMask.NameToLayer("ObstacleRight"));
        if(hitRight && hitRight.gameObject.tag == "LeverGroundRight")
        {
            inRangePlayer2 = true;
        }
        else
        {
            inRangePlayer2 = false;
        }
    }*/
#endregion
