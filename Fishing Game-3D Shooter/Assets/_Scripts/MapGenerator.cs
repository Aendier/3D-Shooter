using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    //public float obstaclePercent;
    //public int seed;

    //public float minObstacleHeight;
    //public float maxObstacleHeight;
    //public Color foregroundColor;
    //public Color backgroundColor;
    public Map[] maps;
    public int mapIndex;
    //砖块预制体
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navmeshFloor;
    public Transform navmeshMaskPrefab;
    //地图大小
    public Vector2 maxMapSize;


    //砖块之间的间隔
    [Range(0, 1)]
    public float outlinePercent;
    //是否自动更新
    public bool autoUpdate;

    public float tileSize;
    private List<Coord> allTileCoords;
    private Queue<Coord> shuffledTileCoords;


    private Map currentMap;
    void Start()
    {
        //脚本启用时生成地图
        GenerateMap();
    }

    //生成地图
    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.05f, currentMap.mapSize.y * tileSize);
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int z = 0; z < currentMap.mapSize.y; z++)
            {
                allTileCoords.Add(new Coord(x, z));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));
        //地图载体的名字
        string holderName = "Generated Map";

        //如果地图载体已经存在，删除
        if (transform.Find(holderName))
        {
            //DestroyImmediate立即销毁
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        //创建地图载体
        Transform mapHolder = new GameObject(holderName).transform;
        //设置地图载体的父物体为当前物体
        mapHolder.parent = transform;

        #region 生成地图
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                //砖块的位置 = （地图大小的一半 - 0.5 + x，0，地图大小的一半 - 0.5 + z）
                Vector3 tilePosition = CoordToPosition(x, y);
                //生成砖块
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90));
                //设置砖块的大小为（1 - outlinePercent）
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                //设置砖块的父物体为地图载体
                newTile.parent = mapHolder;
            }
        }
        #endregion

        #region 生成障碍物
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != currentMap.MapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity);
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize,obstacleHeight,(1 - outlinePercent) * tileSize);
                newObstacle.parent = mapHolder;

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
        #endregion

        #region 生成遮罩
        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity);
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity);
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity);
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity);
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;
        #endregion

        //设置寻路地图的大小
        navmeshFloor.localScale = new Vector3(maxMapSize.x,1, maxMapSize.y) * tileSize;
        
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.MapCenter);
        mapFlags[currentMap.MapCenter.x, currentMap.MapCenter.y] = true;
        int accessibleTileCount = 1;
        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + z;
                    if (x == 0 || z == 0)
                    {
                        if (neighborX >= 0 && neighborX < obstacleMap.GetLength(0) && neighborY >= 0 && neighborY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighborX, neighborY] && !obstacleMap[neighborX, neighborY])
                            {
                                mapFlags[neighborX, neighborY] = true;
                                queue.Enqueue(new Coord(neighborX, neighborY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    public Vector3 CoordToPosition(float x, float y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    [Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }

    [Serializable]
    public class Map
    {
        public Coord mapSize;
        [Range(0, 1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coord MapCenter
        {
            get { return new Coord(mapSize.x/2, mapSize.y/2); }
        }
    }
}