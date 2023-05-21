using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Control")]
    public bool isLive;

    [Header("# Player Info")]
    public float health;
    public float maxhealth = 100;
   
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        
    }
}
