using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]

public class BulletManager : MonoBehaviour
{
    public GameObject Player;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(Player.GetComponent<Shooting>().GetShootCount() == 1)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log($"Colliding with {collision.gameObject.name}");
            }
            gameObject.SetActive(false);
        }
    }
}
