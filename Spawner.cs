using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData; 

    int level;
    float timer;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 7f), spawnData.Length - 1);

        if (timer > spawnData[level].spawnTime)
        {       
            timer = 0;
            Spawn();
        }
    }

    void Spawn() //스폰관련 코드(현재는 몬스터 랜덤만 이후 포인트랑 시간을 통한 보스몹 소환 만들 예정)
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        enemy.GetComponent<Enemy>().Init(spawnData[Random.Range(0,level+1)]);
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
