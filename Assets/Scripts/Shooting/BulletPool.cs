using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletPool : MonoBehaviour
{
    public static BulletPool poolInstance;

    private List<GameObject> pooledObjectLeft = new();
    private List<GameObject> pooledObjectRight = new();

    public GameObject objectToPoolLeft;
    public GameObject objectToPoolRight;
    
    public int amountToPool;

    private void Awake()
    {
        if (poolInstance == null)
            poolInstance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject objLeft = Instantiate(objectToPoolLeft);
            GameObject objRight = Instantiate(objectToPoolRight);

            objLeft.transform.parent = transform;
            objRight.transform.parent = transform;
            
            objLeft.SetActive(false);
            objRight.SetActive(false);

            pooledObjectLeft.Add(objLeft);
            pooledObjectRight.Add(objRight);
        }
    }

    public GameObject GetPooledObjects(string side)
    {
        if(side == "Left")
        {
            for (int i = 0; i < pooledObjectLeft.Count; i++)
            {
                if (!pooledObjectLeft[i].activeInHierarchy)
                {
                    return pooledObjectLeft[i];
                }
            }
        }

        if(side == "Right")
        {
            for (int i = 0; i < pooledObjectRight.Count; i++)
            {
                if (!pooledObjectRight[i].activeInHierarchy)
                {
                    return pooledObjectRight[i];
                }
            }
        }

        return null;
    }

}