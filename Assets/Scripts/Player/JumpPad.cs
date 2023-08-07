using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float bounce;
    [SerializeField] Vector2 direction;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("in");
            direction = new Vector2(collision.transform.position.x, collision.transform.position.y) - collision.contacts[0].point;
            direction = direction.normalized;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector3(direction.x, 0, 0) * bounce, ForceMode2D.Force);
            Debug.Log(new Vector3(direction.x, 0, 0) * bounce);
        }

    }
}
