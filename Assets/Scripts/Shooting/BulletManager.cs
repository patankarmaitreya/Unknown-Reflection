using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]

public class BulletManager : MonoBehaviour
{
    private Shooting shooting;
    private Health health;

    public string playerTag;
    public string collisionTag;
    private bool isCurrentlyColliding = false;

    private void Awake()
    {
        shooting = GameObject.FindGameObjectWithTag(playerTag).GetComponent<Shooting>();
        health = GameObject.FindGameObjectWithTag(collisionTag).GetComponent<Health>();
    }

    private void Update()
    {
        if (shooting.GetShootCount() == 2)
        {
            if(isCurrentlyColliding)
            {
                health.TakeDamage();
            }
            gameObject.SetActive(false);
            shooting.SetShootCount(0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == collisionTag) 
        {
            Debug.Log("correct");
            isCurrentlyColliding = true;
        }
    }

    void OnTriggerExit2D()
    {
        isCurrentlyColliding = false;
    }
}
