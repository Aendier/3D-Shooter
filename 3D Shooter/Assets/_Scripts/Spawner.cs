using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
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

    private MapGenerator map;

    LivingEntity playerEntity;
    Transform playerTF;

    private float timeBetweenCampingChecks = 2;
    private float campThresholdDistance = 1.5f;
    private float nextCampCheckTime;
    private Vector3 campPositionOld;
    private bool isCamping;

    private bool isDisabled;

    public event Action<int> OnNewWave;

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerTF = playerEntity.transform;
        playerEntity.OnDeath += OnPlayerDeath;
        map = FindObjectOfType<MapGenerator>();
        //开始调用下一波函数开始生成敌人
        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerTF.position;
        NextWave();
    }

    private void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;
                isCamping = (Vector3.Distance(playerTF.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerTF.position;
            }

            //如果当前敌人需要生产的敌人数量大于0并且当前时间大于下一次生成敌人的时间
            if ((currentWave.infinite || enemiesRemainingToSpawn > 0) && Time.time > nextSpawnTime)
            {
                //需要生成的敌人数量减一
                enemiesRemainingToSpawn--;
                //下一次生成敌人的时间等于当前时间加上生成敌人的间隔时间
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine(SpawnEnemy());
            }
        }
    }

    private void ResetPlayerPosition()
    {
        playerTF.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }
    private void OnPlayerDeath()
    {
        isDisabled = true;
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
            OnNewWave?.Invoke(currentWaveNumber);
            ResetPlayerPosition();
        }
        
    }

    

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;
        //获取地图上的一个随机位置
        Transform spawnTile = map.GetRandomOpenTile();
        //如果玩家在露营，那么位置将会是玩家的位置
        if(isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerTF.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initalColor = Color.white;
        Color flashColor = Color.red;
        float spawnTimer = 0;
        while(spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initalColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }
        //生成敌人
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity);
        //当敌人死亡时调用OnEnemyDeath函数
        spawnedEnemy.OnDeath += OnEnemyDeath;
        //设置敌人的属性
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);

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
