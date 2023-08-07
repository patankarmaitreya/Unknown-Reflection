using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EventManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private GameObject player1Prefab;
    [SerializeField] private GameObject player2Prefab;

    public static EventManager eventInstance;
    public event Action<List<GameObject>> platformRotate;
    //public event Action<List<GameObject>> rightplatformRotate;

    public bool isRightRotated = false;
    public bool isLeftRotated = false;
   
    private void Awake()
    {
        if (eventInstance == null)
        {
            eventInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
    }

    public void StartPlatformRotate(List<GameObject> platforms)
    {
        platformRotate?.Invoke(platforms);
    }
   
}
