using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Object#")]
    public Player player;
    public PoolManager pool;
    public ParticleSystem deadEffect;

    [Header("# Player Info #")]
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 10, 20, 30 };

    private void Awake()
    {
        instance = this;
    }

    public void GetExp()
    {
        exp++;

        if(exp == nextExp[level])
        {
            level++;
            exp = 0;
        }
    }
}
