using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum tileTypes
{
    floor,
    water,
    wall
}

public enum levelTypes
{
    forest,
    snow,
    lava,
    barren
}

public class ArcadeMode : MonoBehaviour
{

    public levelTypes levelType;
    public NavMeshSurface surface;
    public UIManager m_UIManager;
    public ArcadeModeLevels level;

    public GameObject player;
    public GameObject friend;

    public NPCDialogue[] johhnyForestDialogues;
    public NPCDialogue[] johhnySnowDialogues;
    public NPCDialogue[] johhnyLavaDialogues;
    public NPCDialogue[] johhnyBarrenDialogues;

    int width;
    int height;
    int numberOfObstacles;
    Vector3[] obstaclePos;
    int numberOfEnemies;
    Vector3[] enemyPos;
    int numberOfSupplies;
    Vector3[] suppliesPos;
    GameObject[] enemyWaypoints;
    Vector3 mapCentre;
    Vector3 originPoint;
    Vector3 oppositeToOriginPoint;

    int[,] map;
    int[,] tempMap;

    bool playerExists = false;
    Vector3 m_PlayerSpawnPosition;
    GameObject playerReference;
    GameObject weaponReference;
    int weaponBulletsReference;
    GameObject friendReference;
    GameObject weapon2Reference;
    GameObject floor;

    bool[] everythingDone = new bool[8];

    // Use this for initialization
    void Start()
    {
        /*var f = Instantiate(floor, Vector3.zero, Quaternion.identity, transform);
        if (width  < 20)
        {
            width = 20;
        }
        if(height < 20)
        {
            height = 20;
        }
       
        f.transform.localScale = new Vector3 (width / 5,1, height / 5);*/
        for (int i = 0; i < level.startLevel; i++)
        {
            level.IncreaseLevel();
        }
        width = Random.Range(level.minWidth, level.maxWidth);
        height = Random.Range(level.minHeight, level.maxHeight);
        map = new int[width, height];
        tempMap = new int[width, height];
        levelType = (levelTypes)(int)Random.Range(0, 4);

        MakeMap();
        
        m_UIManager = FindObjectOfType<UIManager>();
        m_UIManager.UpdateArcadeUI(CalculateNumberOfActiveEnemies(), level.currentLevel);
    }

    void DestroyMap()
    {
        int children = transform.childCount;
        weaponReference = playerReference.GetComponent<PlayerController>().GetWeapon();
        if (weaponReference)
        {
            weaponBulletsReference = weaponReference.GetComponent<GunLogic>().m_BulletAmmo;
        }
        weapon2Reference = friendReference.GetComponent<FriendlyAIController>().GetWeapon();
        friendReference.GetComponent<FriendlyAIController>().DropWeapon();
        for (int i = 0; i < children; i++)
        {
            if (transform.GetChild(i).gameObject != playerReference && transform.GetChild(i).gameObject != friendReference)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    int CalculateNumberOfActiveEnemies()
    {
        var spawnPoints = GetComponentsInChildren<SpawnPoint>();
        int numberOfActiveEnemies = 0;
        foreach (SpawnPoint sp in spawnPoints)
        {
            numberOfActiveEnemies += sp.m_NumberOfActiveEnemies;
        }
        return numberOfActiveEnemies;
    }

    bool HaveEnemiesSpawned()
    {
        var spawnPoints = GetComponentsInChildren<SpawnPoint>();
        foreach (SpawnPoint sp in spawnPoints)
        {
            if (!sp.HasSpawned())
            {
                return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        BuildNavMeshCorrectly();
        FixSpawningUnderground();
        numberOfEnemies = CalculateNumberOfActiveEnemies();
        m_UIManager.UpdateArcadeUI(numberOfEnemies, level.currentLevel);
        if (HaveEnemiesSpawned() && numberOfEnemies <= 0)
        {
            level.IncreaseLevel();
            ChangeMap();
        }

    }

    void MakeMap()
    {
        int floors = 0;
        int waters = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int number = Random.Range(0, 100);
                if (number < 50)
                {
                    map[i, j] = 0;
                    floors++;
                }
                else
                {
                    map[i, j] = 1;
                    waters++;
                }
            }
        }
        Debug.Log("Floors " + floors);
        Debug.Log("Waters " + waters);
        CellularAutomata(50);
        PopulateMap();
        BuildTheWall();
        ProvideCover();
        DropSupplies();
        ShowMeDaWay();
        EvilSoldiers();
        LetsGODude();
        HereToHelp();      
    }

    void CellularAutomata(int numberOfIterations)
    {
        for (int k = 0; k < numberOfIterations; k++)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tileTypes aTile = (tileTypes)(map[i, j]);
                    tileTypes aboveTile;
                    if (i - 1 < 0)
                    {
                        aboveTile = tileTypes.wall;
                    }
                    else
                    {
                        aboveTile = (tileTypes)(map[i - 1, j]);
                    }
                    tileTypes belowTile;
                    if (i + 1 >= width)
                    {
                        belowTile = tileTypes.wall;
                    }
                    else
                    {
                        belowTile = (tileTypes)(map[i + 1, j]);
                    }
                    tileTypes leftTile;
                    if (j - 1 < 0)
                    {
                        leftTile = tileTypes.wall;
                    }
                    else
                    {
                        leftTile = (tileTypes)(map[i, j - 1]);
                    }
                    tileTypes rightTile;
                    if (j + 1 >= height)
                    {
                        rightTile = tileTypes.wall;
                    }
                    else
                    {
                        rightTile = (tileTypes)(map[i, j + 1]);
                    }
                    tileTypes topRightCornerTile;
                    if (j + 1 >= height || i - 1 < 0)
                    {
                        topRightCornerTile = tileTypes.wall;
                    }
                    else
                    {
                        topRightCornerTile = (tileTypes)(map[i - 1, j + 1]);
                    }
                    tileTypes topLeftCornerTile;
                    if (j - 1 < 0 || i - 1 < 0)
                    {
                        topLeftCornerTile = tileTypes.wall;
                    }
                    else
                    {
                        topLeftCornerTile = (tileTypes)(map[i - 1, j - 1]);
                    }
                    tileTypes bottomRightCornerTile;
                    if (j + 1 >= height || i + 1 >= width)
                    {
                        bottomRightCornerTile = tileTypes.wall;
                    }
                    else
                    {
                        bottomRightCornerTile = (tileTypes)(map[i + 1, j + 1]);
                    }
                    tileTypes bottowLeftCornerTile;
                    if (j - 1 < 0 || i + 1 >= width)
                    {
                        bottowLeftCornerTile = tileTypes.wall;
                    }
                    else
                    {
                        bottowLeftCornerTile = (tileTypes)(map[i + 1, j - 1]);
                    }

                    tileTypes[] neighboors = { aboveTile, belowTile, leftTile, rightTile, topLeftCornerTile, topRightCornerTile, bottomRightCornerTile, bottowLeftCornerTile };
                    int sameNeighboor = 0;
                    int differentNeightboor = 0;

                    if (aTile == tileTypes.water)
                    {
                        for (int l = 0; l < 8; l++)
                        {
                            if (aTile == neighboors[l])
                            {
                                sameNeighboor++;
                            }
                            else
                            {
                                if (neighboors[l] == tileTypes.floor)
                                {
                                    differentNeightboor++;
                                }
                            }
                        }
                        if (sameNeighboor > 5)
                        {
                            tempMap[i, j] = map[i, j];
                        }
                        else
                        {
                            if (differentNeightboor > sameNeighboor)
                            {
                                tempMap[i, j] = (int)tileTypes.floor;
                            }
                            else
                            {
                                tempMap[i, j] = map[i, j];
                            }
                        }
                    }
                    else if (aTile == tileTypes.floor)
                    {
                        for (int l = 0; l < 8; l++)
                        {
                            if (aTile == neighboors[l])
                            {
                                sameNeighboor++;
                            }
                            else
                            {
                                if (neighboors[l] == tileTypes.water)
                                {
                                    differentNeightboor++;
                                }
                            }
                        }
                        if (sameNeighboor > 2)
                        {
                            tempMap[i, j] = map[i, j];
                        }
                        else
                        {
                            if (differentNeightboor > sameNeighboor)
                            {
                                tempMap[i, j] = (int)tileTypes.water;
                            }
                            else
                            {
                                tempMap[i, j] = map[i, j];
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    map[i, j] = tempMap[i, j];

                }
            }
        }
    }

    void PopulateMap()
    {
        originPoint = new Vector3((-width / 2f) - 0.5f, 0f, (-height / 2f));
        oppositeToOriginPoint = new Vector3(((width - 1) * 2 - width / 2f) + 0.5f, 0f, ((height - 1) * 2 - height / 2f) + 0.5f);
        floor = level.GetFloor(levelType);
        var water = level.GetWater(levelType);
        tileTypes aTile;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                aTile = (tileTypes)map[x, y];
                Vector3 pos;
                if (x == width / 2 && y == height / 2)
                {
                    mapCentre = new Vector3(x * 2 - width / 2f, 0f, y * 2 - height / 2f);
                }

                switch (aTile)
                {
                    case tileTypes.floor:
                        pos = new Vector3(x * 2 - width / 2f, 0f, y * 2 - height / 2f);
                        Instantiate(floor, pos, Quaternion.identity, transform);
                        break;
                    case tileTypes.water:
                        pos = new Vector3(x * 2 - width / 2f, -1f, y * 2 - height / 2f);
                        Instantiate(water, pos, Quaternion.identity, transform);
                        break;
                }
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = -height; y < 0; y++)
            {
                Vector3 pos;
                pos = new Vector3(x * 2 - width / 2f, -1f, y * 2 - height / 2f);
                Instantiate(water, pos, Quaternion.identity, transform);
            }
        }
        everythingDone[0] = true;
    }

    void BuildTheWall()
    {
        var wallFront = level.GetFrontWall(levelType);
        var wallBack = level.GetBackWall(levelType);
        var wallLeft = level.GetLeftWall(levelType);
        var wallRight = level.GetRightWall(levelType);
        Vector3 frontWallPos = oppositeToOriginPoint + new Vector3(0, height / 2, -width);  //mapCentre + new Vector3((float)width+0.5f,height/2, 0);
        Vector3 backWallPos = originPoint + new Vector3(0, height / 2, width); //mapCentre + new Vector3(-width, height/2, 0);
        Vector3 leftWallPos = oppositeToOriginPoint + new Vector3(-height, height / 2, 0);//mapCentre + new Vector3(0 , width/2, height);
        Vector3 rightWallPos = originPoint + new Vector3(height, height / 2, 0);//mapCentre + new Vector3(0, width/2, -height);
        var w1 = Instantiate(wallFront, frontWallPos, wallFront.transform.rotation, transform);
        var w2 = Instantiate(wallBack, backWallPos, wallBack.transform.rotation, transform);
        var w3 = Instantiate(wallLeft, leftWallPos, wallLeft.transform.rotation, transform);
        var w4 = Instantiate(wallRight, rightWallPos, wallRight.transform.rotation, transform);
        w1.transform.localScale += new Vector3(height / 2, 1, height + 1) / 3; // w1.transform.localScale * height;
        w2.transform.localScale += new Vector3(height / 2, 1, height + 1) / 3;//w2.transform.localScale * height;
        w3.transform.localScale += new Vector3(width + 1, 1, width / 2) / 3;//w3.transform.localScale * width;
        w4.transform.localScale += new Vector3(width + 1, 1, width / 2) / 3; // w4.transform.localScale * width;
        everythingDone[1] = true;

    }

    void ProvideCover()
    {
        numberOfObstacles = (int)Random.Range(level.minObstacles, level.maxObstacles);
        obstaclePos = new Vector3[numberOfObstacles];
        int numberOfTries = 0;
        for (int i = 0; i < numberOfObstacles; i++)
        {
            bool coverProvided = true;
            int x = Random.Range(2, width - 2);
            int y = Random.Range(2, height - 2);
            if ((tileTypes)map[x, y] == tileTypes.water || (tileTypes)map[x, y] == tileTypes.wall)
            {
                coverProvided = false;
            }
            Vector3 pos = new Vector3(x * 2 - width / 2f, 0f, y * 2 - height / 2f);
            for (int j = 0; j < obstaclePos.Length; j++)
            {
                if (Vector3.Distance(pos, obstaclePos[j]) < 5)
                {
                    coverProvided = false;
                    break;
                }
            }
            if (coverProvided)
            {
                var obstacle = level.GetObstacle(levelType);
                Instantiate(obstacle, pos, Quaternion.identity, transform);
                obstaclePos[i] = pos;
                numberOfTries = 0;
            }
            else
            {
                i--;
                numberOfTries++;
                Debug.Log("Cover Failed " + numberOfTries + " times");
                continue;
            }
        }
        everythingDone[2] = true;
    }

    void EvilSoldiers()
    {
        numberOfEnemies = (int)Random.Range(level.minEnemies, level.maxEnemies);
        enemyPos = new Vector3[numberOfEnemies];
        int numberOfTries = 0;
        for (int i = 0; i < numberOfEnemies; i++)
        {
            bool evilUnleashed = true;
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);
            if ((tileTypes)map[x, y] == tileTypes.water || (tileTypes)map[x, y] == tileTypes.wall)
            {
                evilUnleashed = false;
            }
            Vector3 pos = new Vector3(x * 2 - width / 2f, 1f, y * 2 - height / 2f);
            for (int j = 0; j < obstaclePos.Length; j++)
            {
                if (Vector3.Distance(pos, obstaclePos[j]) < 5)
                {
                    evilUnleashed = false;
                    break;
                }
            }
            for (int k = 0; k < enemyPos.Length; k++)
            {
                if (Vector3.Distance(pos, enemyPos[k]) < 3)
                {
                    evilUnleashed = false;
                    break;
                }
            }
            if (evilUnleashed)
            {
                var enemy = level.GetEnemy();
                Instantiate(enemy, pos, Quaternion.identity, transform);
                enemyPos[i] = pos;
                numberOfTries = 0;
            }
            else
            {
                i--;
                numberOfTries++;
                Debug.Log("Evil failed " + numberOfTries + " times");
                continue;
            }
        }
        everythingDone[3] = true;
    }

    void DropSupplies()
    {
        numberOfSupplies = (int)Random.Range(level.minSupplies, level.maxSupplies);
        suppliesPos = new Vector3[numberOfSupplies];
        int numberOfTries = 0;
        for (int i = 0; i < numberOfSupplies; i++)
        {
            bool supplyDropped = true;
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);
            if ((tileTypes)map[x, y] == tileTypes.water || (tileTypes)map[x, y] == tileTypes.wall)
            {
                supplyDropped = false;
            }
            Vector3 pos = new Vector3(x * 2 - width / 2f, 1f, y * 2 - height / 2f);
            for (int j = 0; j < obstaclePos.Length; j++)
            {
                if (Vector3.Distance(pos, obstaclePos[j]) < 5)
                {
                    supplyDropped = false;
                    break;
                }
            }
            for (int k = 0; k < suppliesPos.Length; k++)
            {
                if (Vector3.Distance(pos, suppliesPos[k]) < 10)
                {
                    supplyDropped = false;
                    break;
                }
            }
            if (supplyDropped)
            {
                var supply = level.GetSupplies();
                Instantiate(supply, pos, Quaternion.identity, transform);
                suppliesPos[i] = pos;
                numberOfTries = 0;
            }
            else
            {
                i--;
                numberOfTries++;
                Debug.Log("Supplies failed " + numberOfTries + " times");
                continue;
            }
        }
        everythingDone[4] = true;
    }

    void ShowMeDaWay()
    {
        enemyWaypoints = new GameObject[level.numberOfWaypoints];
        int numberOfTries = 0;

        for (int i = 0; i < enemyWaypoints.Length; i++)
        {
            bool theWayIsClear = true;
            int x = Random.Range(2, width - 2);
            int y = Random.Range(2, height - 2);
            if ((tileTypes)map[x, y] == tileTypes.water || (tileTypes)map[x + 1, y] == tileTypes.water || (tileTypes)map[x + 1, y + 1] == tileTypes.water || (tileTypes)map[x + 1, y - 1] == tileTypes.water ||
                  (tileTypes)map[x - 1, y] == tileTypes.water || (tileTypes)map[x - 1, y + 1] == tileTypes.water || (tileTypes)map[x - 1, y - 1] == tileTypes.water || (tileTypes)map[x, y + 1] == tileTypes.water ||
                  (tileTypes)map[x, y - 1] == tileTypes.water || (tileTypes)map[x, y] == tileTypes.wall || (tileTypes)map[x + 1, y] == tileTypes.wall || (tileTypes)map[x + 1, y + 1] == tileTypes.wall ||
                  (tileTypes)map[x + 1, y - 1] == tileTypes.wall || (tileTypes)map[x - 1, y] == tileTypes.wall || (tileTypes)map[x - 1, y + 1] == tileTypes.wall || (tileTypes)map[x - 1, y - 1] == tileTypes.wall ||
                  (tileTypes)map[x, y + 1] == tileTypes.wall || (tileTypes)map[x, y - 1] == tileTypes.wall
               )
            {
                theWayIsClear = false;
            }

            Vector3 pos = new Vector3(x * 2 - width / 2f, 1f, y * 2 - height / 2f);
            for (int j = 0; j < obstaclePos.Length; j++)
            {
                if (Vector3.Distance(pos, obstaclePos[j]) < 3)
                {
                    theWayIsClear = false;
                    break;
                }
            }
            for (int k = 0; k < i; k++)
            {
                if (Vector3.Distance(pos, enemyWaypoints[k].transform.position) < 15)
                {
                    theWayIsClear = false;
                    break;
                }
            }
            if (theWayIsClear)
            {
                var waypoint = level.GetWaypoint();
                Debug.Log("index " + i);
                Debug.Log("Number of waypoints" + enemyWaypoints.Length);
                if (waypoint)
                {
                    waypoint = Instantiate(waypoint, pos, Quaternion.identity, transform);
                    enemyWaypoints[i] = waypoint;
                    numberOfTries = 0;
                }
            }
            else
            {
                i--;
                numberOfTries++;
                Debug.Log("Da way has failed " + numberOfTries + (" times"));
                continue;
            }
        }
        for (int i = 0; i < enemyWaypoints.Length; i++)
        {
            if (i + 1 < enemyWaypoints.Length)
            {
                enemyWaypoints[i].GetComponent<Waypoint>().nextWaypoint = enemyWaypoints[i + 1].GetComponent<Waypoint>();
            }
            else
            {
                enemyWaypoints[i].GetComponent<Waypoint>().nextWaypoint = enemyWaypoints[0].GetComponent<Waypoint>();
            }
            if (i - 1 > 0)
            {
                enemyWaypoints[i].GetComponent<Waypoint>().previousWaypoint = enemyWaypoints[i - 1].GetComponent<Waypoint>();
            }
            else
            {
                enemyWaypoints[i].GetComponent<Waypoint>().previousWaypoint = enemyWaypoints[enemyWaypoints.Length - 1].GetComponent<Waypoint>();
            }
        }
        everythingDone[5] = true;
    }

    void LetsGODude()
    {
        bool pleaseDontLetMeDrown;
        Vector3 pos;
        int numberOfTries = 0;
        do
        {
            pleaseDontLetMeDrown = true;
            int x = Random.Range(2, width-2);
            int y = Random.Range(2, height -2);
            if (
                 (tileTypes)map[x, y] == tileTypes.water || (tileTypes)map[x + 1, y] == tileTypes.water || (tileTypes)map[x + 1, y + 1] == tileTypes.water ||
                 (tileTypes)map[x + 1, y - 1] == tileTypes.water || (tileTypes)map[x - 1, y] == tileTypes.water || (tileTypes)map[x - 1, y + 1] == tileTypes.water ||
                 (tileTypes)map[x - 1, y - 1] == tileTypes.water || (tileTypes)map[x, y + 1] == tileTypes.water || (tileTypes)map[x, y - 1] == tileTypes.water
               )
            {
                pleaseDontLetMeDrown = false;
            }
            pos = new Vector3(x * 2 - width / 2f, 1f, y * 2 - height / 2f);
            for (int j = 0; j < obstaclePos.Length; j++)
            {
                if (Vector3.Distance(pos, obstaclePos[j]) < 8)
                {
                    pleaseDontLetMeDrown = false;
                    break;
                }
            }
            for (int k = 0; k < enemyPos.Length; k++)
            {
                if (Vector3.Distance(pos, enemyPos[k]) < 10)
                {
                    pleaseDontLetMeDrown = false;
                    break;
                }
            }
            Debug.Log("Hero has failed " + numberOfTries + " times");
            numberOfTries++;
        }
        while (!pleaseDontLetMeDrown);


        if (!playerExists)
        {
            playerReference = Instantiate(player, pos + Vector3.up * 2f, Quaternion.identity, transform);
            var v = Instantiate(level.GetGun(2), pos + Vector3.up, Quaternion.identity, transform);
            playerReference.GetComponent<PlayerController>().PickUpWeapon(v.GetComponent<Equipable>());
            Camera cam = FindObjectOfType<Camera>();
            if (cam)
            {
                cam.GetComponent<CameraFollow>().m_PlayerTransform = playerReference.transform;
            }
            m_PlayerSpawnPosition = pos;
        }
        else
        {
            playerReference.transform.position = pos + Vector3.up * 2f;
            m_PlayerSpawnPosition = pos;
            if (weaponReference)
            {
                var v = Instantiate(weaponReference, pos + Vector3.up, Quaternion.identity, transform);
                v.GetComponent<BoxCollider>().enabled = true;
                v.GetComponent<Equipable>().enabled = true;
                v.GetComponent<GunLogic>().enabled = true;
                v.GetComponent<AudioSource>().enabled = true;
                playerReference.GetComponent<PlayerController>().PickUpWeapon(v.GetComponent<Equipable>());
                v.GetComponent<GunLogic>().m_BulletAmmo = weaponBulletsReference;
                if(v.GetComponent<SniperLogic>())
                {
                    v.GetComponent<SniperLogic>().enabled = true;
                }
            }
        }
        everythingDone[6] = true;
    }

    void HereToHelp()
    {
        bool pleaseDontLetMeDrown;
        Vector3 pos;
        int numberOfTries = 0;
        do
        {
            pleaseDontLetMeDrown = true;
            int x = Random.Range(2, width-2);
            int y = Random.Range(2, height-2);
            if ((tileTypes)map[x, y] == tileTypes.water || (tileTypes)map[x, y] == tileTypes.wall)
            {
                pleaseDontLetMeDrown = false;
            }
            pos = new Vector3(x * 2 - width / 2f, 1f, y * 2 - height / 2f);
            for (int j = 0; j < obstaclePos.Length; j++)
            {
                if (Vector3.Distance(pos, obstaclePos[j]) < 8)
                {
                    pleaseDontLetMeDrown = false;
                    break;
                }
            }
            for (int k = 0; k < enemyPos.Length; k++)
            {
                if (Vector3.Distance(pos, enemyPos[k]) < 5)
                {
                    pleaseDontLetMeDrown = false;
                    break;
                }
            }
            if (Vector3.Distance(pos, m_PlayerSpawnPosition) < 2 || Vector3.Distance(pos, m_PlayerSpawnPosition) > 15)
            {
                pleaseDontLetMeDrown = false;
            }
            Debug.Log("Friend has failed " + numberOfTries + " times");
            numberOfTries++;
        }
        while (!pleaseDontLetMeDrown);


        if (!playerExists)
        {
            playerExists = true;
            friendReference = Instantiate(friend, pos, Quaternion.identity, transform);
            friendReference.GetComponent<FriendlyAIController>().m_SpawnLocation = pos;
            var v = Instantiate(level.GetGun(2), pos + Vector3.up, Quaternion.identity, transform);
            friendReference.GetComponent<FriendlyAIController>().PickUpWeapon(v.GetComponent<Equipable>());
        }
        else
        {
            if (friendReference.GetComponent<FriendlyAIController>().m_IsAlive)
            {
                friendReference.GetComponent<FriendlyAIController>().Warp(pos);
            }
            friendReference.GetComponent<FriendlyAIController>().m_SpawnLocation = pos;
            if (weapon2Reference)
            {
                var v = Instantiate(weapon2Reference, pos + Vector3.up, Quaternion.identity, transform);
                v.GetComponent<BoxCollider>().enabled = true;
                v.GetComponent<Equipable>().enabled = true;
                v.GetComponent<GunLogic>().enabled = true;
                v.GetComponent<AudioSource>().enabled = true;
                friendReference.GetComponent<FriendlyAIController>().PickUpWeapon(v.GetComponent<Equipable>());
                if (v.GetComponent<SniperLogic>())
                {
                    v.GetComponent<SniperLogic>().enabled = true;
                }
            }
        }
        ChangeJohhnyDialogues();
        everythingDone[7] = true;
    }

    void ChangeMap()
    {
        DestroyMap();
        width = Random.Range(level.minWidth, level.maxWidth);
        height = Random.Range(level.minHeight, level.maxHeight);
        map = new int[width, height];
        tempMap = new int[width, height];
        levelType = (levelTypes)(int)Random.Range(0, 4);
        MakeMap();
        m_UIManager.UpdateArcadeUI(CalculateNumberOfActiveEnemies(), level.currentLevel);
    }

    void ChangeJohhnyDialogues()
    {
        if (friendReference)
        {
            var johhny = friendReference.GetComponent<FriendlyAIController>();
            if (johhny)
            {
                johhny.m_DialogueIndex = 0;
                switch (levelType)
                {
                    case levelTypes.forest:
                        johhny.ChangeDialogue(johhnyForestDialogues[0], 0);
                        johhny.ChangeDialogue(johhnyForestDialogues[1], 1);
                        johhny.ChangeDialogue(johhnyForestDialogues[2], 2);
                        break;
                    case levelTypes.snow:
                        johhny.ChangeDialogue(johhnySnowDialogues[0], 0);
                        johhny.ChangeDialogue(johhnySnowDialogues[1], 1);
                        johhny.ChangeDialogue(johhnySnowDialogues[2], 2);
                        break;
                    case levelTypes.lava:
                        johhny.ChangeDialogue(johhnyLavaDialogues[0], 0);
                        johhny.ChangeDialogue(johhnyLavaDialogues[1], 1);
                        johhny.ChangeDialogue(johhnyLavaDialogues[2], 2);
                        break;
                    case levelTypes.barren:
                        johhny.ChangeDialogue(johhnyBarrenDialogues[0], 0);
                        johhny.ChangeDialogue(johhnyBarrenDialogues[1], 1);
                        johhny.ChangeDialogue(johhnyBarrenDialogues[2], 2);
                        break;
                }
            }
        }
    }

    void BuildNavMeshCorrectly()
    {
        bool allGood = false;
        for(int i = 0; i<everythingDone.Length;i++)
        {
            allGood = everythingDone[i];
        }
        if(allGood)
        {
            surface.BuildNavMesh();
            for (int i = 0; i < everythingDone.Length; i++)
            {
                everythingDone[i] = false;
            }
        }
    }

    void FixSpawningUnderground()
    {
        if(playerReference)
        {
            var player = playerReference.GetComponent<PlayerController>();
            if(player.IsPlayerAlive() && !player.m_IsDrowning && playerReference.transform.position.y < -2)
            {
                playerReference.transform.position = m_PlayerSpawnPosition;
            }
        }
    }
}
