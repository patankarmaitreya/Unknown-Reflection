using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{

    public GameObject bullet;
    public string side;
    public float launchForce;
    public Transform firePoint;

    public InputAction shoot;
    private float direction;

    private int shootCount = 0;

    public int GetShootCount()
    {
        return shootCount;
    }

    public void SetShootCount(int value)
    {
        shootCount = value;
    }
    private void Awake()
    {
        shoot.performed += Shoot_performed;
        direction = Mathf.Cos(firePoint.localEulerAngles.y * Mathf.Deg2Rad);
    }

    private void OnEnable()
    {
        shoot.Enable();
    }

    private void OnDisable()
    {
        shoot.Disable();
    }

    private void Shoot_performed(InputAction.CallbackContext obj)
    {
        switch (shootCount)
        {
            case 0:
                Shoot();
                shootCount++;
                break;
            case 1:
                shootCount++;
                break;

        }
    }

    private void Shoot()
    {
        //GameObject bulletIns = Instantiate(bullet,transform.position, transform.rotation);
        //bulletIns.GetComponent<Rigidbody2D>().velocity = Vector2.one * launchForce;
        bullet = BulletPool.poolInstance.GetPooledObjects(side);

        if(bullet != null)
        {
            bullet.SetActive(true);
            bullet.transform.position = firePoint.position;
            Vector3 velocity = new Vector3(firePoint.localPosition.x * direction * 0.5f * launchForce, 0, 0);
            bullet.GetComponent<Rigidbody2D>().velocity = velocity;
        }
    }

}
