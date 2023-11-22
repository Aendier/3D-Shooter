using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //每一波敌人的信息
    [Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;
    }
    public Wave[] waves;
    //敌人的脚本
    public Enemy enemy;
    //当前的波数
    public Wave currentWave;
    //当前波数的编号
    public int currentWaveNumber;
    //当前波数没生成敌人数量
    public int enemiesRemainingToSpawn;
    //当前波数存活敌人数量
    public int enemiesRemainingAlive;
    //下次一生成敌人的时间
    public float nextSpawnTime;

    private void Start()
    {
        //开始调用下一波函数开始生成敌人
        NextWave();
    }
    

    //启动下一波
    private void NextWave()
    {
        currentWaveNumber++;
        print("Wave: " + currentWaveNumber);
        if(currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

        }
    }

    private void Update()
    {
        //如果当前敌人需要生产的敌人数量大于0并且当前时间大于下一次生成敌人的时间
        if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            //需要生成的敌人数量减一
            enemiesRemainingToSpawn--;
            //下一次生成敌人的时间等于当前时间加上生成敌人的间隔时间
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
            //生成敌人
            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity);
            //当敌人死亡时调用OnEnemyDeath函数
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }
    }

    //当敌人死亡时调用
    private void OnEnemyDeath()
    {
        //当前波活着的敌人数量减一
        enemiesRemainingAlive--;
        //如果当前波活着的敌人数量等于0
        if(enemiesRemainingAlive == 0)
        {
            //启动下一波
            NextWave();
        }
    }
}
