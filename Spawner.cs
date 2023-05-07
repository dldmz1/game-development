using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData; 

    int level;
    float timer;
    bool flag = false;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 5f), 2);
        // 오버라이딩 방식으로 몬스터에 따라 spawnTime을 지정해놓았으나 이는 시간에 따른 spawnTime을 다루는 데 이용

        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            Spawn();
        }



        if (!flag && GameManager.instance.gameTime>= GameManager.instance.maxGameTime) // 시간 조건만 따짐(포인트 관련 추가 예정)
        {
            boss_Spawn();
        }
    }

    void Spawn() //스폰관련 코드(일반 몬스터)
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[Random.Range(0,level+1)]);
    }

    void boss_Spawn() //보스 스폰관련(1마리만 소환)
    {
        flag = true;
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[3]);
    }
}

[System.Serializable]
public class SpawnData //스폰 데이터에 사용될 몬스터의
{
    public float spawnTime;
    public int spriteType;
    public int health;
    public float speed;
    public int point;
}
