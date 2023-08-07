using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatiionManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> leftSidePlatform;
    [SerializeField] private List<GameObject> rightSidePlatform;
    [SerializeField] private float angleOfRotation;
    [SerializeField] private float rotationDelay;
    [SerializeField] private float rotationFactor;
    [SerializeField] private float valueOfRoatation;
    private float newAngle;
   
    private void OnEnable()
    {
        EventManager.eventInstance.platformRotate += RotatePlatform;
        //EventManager.eventInstance.rightplatformRotate += RotatePlatformRight;
    }

    public void RotatePlatform(List<GameObject> leftPlatforms)
    {
        leftPlatforms = leftSidePlatform;
        if (!EventManager.eventInstance.isLeftRotated)
        {
            foreach (GameObject platformsPrefab in leftPlatforms)
            {
                StartCoroutine(SetLeftPlatformRotation(platformsPrefab));
            }
        }
    }
    IEnumerator SetLeftPlatformRotation(GameObject leftPlatforms)
    {
        float oldAngle = leftPlatforms.transform.rotation.eulerAngles.z;
        if (angleOfRotation != leftPlatforms.transform.rotation.z)
        {
            newAngle = angleOfRotation;  //angleOfRotation* randomRotation*direction;
            for (float theta = 0f; theta <= angleOfRotation; theta += rotationFactor)
            {
                if (Mathf.Abs(theta - angleOfRotation) < valueOfRoatation)
                {
                    leftPlatforms.transform.rotation = Quaternion.Euler(0, 0, angleOfRotation + oldAngle);
                    yield break;
                }
                leftPlatforms.transform.Rotate(Vector3.forward, rotationFactor);
                yield return new WaitForSeconds(rotationDelay);
                EventManager.eventInstance.isLeftRotated = true;
                EventManager.eventInstance.isRightRotated = false;
            }
        }
        yield break;
    }
    IEnumerator SetRightPlatformRotation(GameObject rightPlatforms)
    {
        float oldAngle = rightPlatforms.transform.rotation.eulerAngles.z;
        if (angleOfRotation != rightPlatforms.transform.rotation.z)
        {
            newAngle = angleOfRotation;
            for (float theta = 0f; theta <= newAngle; theta += rotationFactor)
            {
                if (Mathf.Abs(theta - newAngle) < valueOfRoatation)
                {
                    rightPlatforms.transform.rotation = Quaternion.Euler(0, 0, newAngle+oldAngle);
                    yield break;
                }
                rightPlatforms.transform.Rotate(Vector3.forward, rotationFactor);
                yield return new WaitForSeconds(rotationDelay);
                EventManager.eventInstance.isLeftRotated = false;
                EventManager.eventInstance.isRightRotated = true;
            }
        }


        yield break;
    }
    private void OnDisable()
    {
        EventManager.eventInstance.platformRotate -= RotatePlatform;
       // EventManager.eventInstance.rightplatformRotate -= RotatePlatformRight;
    }
}
