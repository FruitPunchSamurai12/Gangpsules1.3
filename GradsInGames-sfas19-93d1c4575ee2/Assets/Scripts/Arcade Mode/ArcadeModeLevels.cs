using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArcadeModeLevels {

    public int startLevel = 0;
    public int currentLevel = 0;
    public int minWidth = 40;
    public int maxWidth = 50;
    public int minHeight = 40;
    public int maxHeight = 50;
    public int minObstacles = 20;
    public int maxObstacles = 30;
    public int minEnemies = 10;
    public int maxEnemies = 15;
    public int minSupplies = 2;
    public int maxSupplies = 3;
    public int chanceForAK47Guy = 4;
    public int chanceForSMGGuy = 4;
    public int chanceForShotgunGuy = 1;
    public int chanceForSniperGuy = 1;
    [SerializeField] GameObject pistolGuy;
    [SerializeField] GameObject AK47Guy;
    [SerializeField] GameObject SMGGuy;
    [SerializeField] GameObject sniperGuy;
    [SerializeField] GameObject shotgunGuy;
    [SerializeField] GameObject[] floor;
    [SerializeField] GameObject[] wallFront;
    [SerializeField] GameObject[] wallBack;
    [SerializeField] GameObject[] wallRight;
    [SerializeField] GameObject[] wallLeft;
    [SerializeField] GameObject[] water;
    [SerializeField] GameObject[] forestObstacles;
    [SerializeField] GameObject[] snowObstacles;
    [SerializeField] GameObject[] lavaObstacles;
    [SerializeField] GameObject[] barrenObstacles;
    [SerializeField] GameObject[] healthPots;
    [SerializeField] GameObject waypoint;

    [SerializeField] GameObject[] guns;

    public int numberOfWaypoints = 5;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < startLevel; i++)
        {
            IncreaseLevel();
        }
    }

    public void IncreaseLevel()
    {
        currentLevel++;
        minWidth += 2;
        maxWidth += 2;
        minHeight += 2;
        maxHeight += 2;
        minObstacles += 2;
        maxObstacles += 2;
        minEnemies += 1;
        maxEnemies += 1;
        chanceForAK47Guy += 2;
        chanceForSMGGuy += 2;
        chanceForSniperGuy += 2;
        chanceForShotgunGuy += 1;
        if(currentLevel%2==0)
        {
            maxEnemies += 1;
            minSupplies++;
            maxSupplies++;
        }

    }

    public GameObject GetWaypoint()
    {
        return waypoint;
    }

    public GameObject GetObstacle(levelTypes levelType)
    {
        int random;
        switch (levelType)
        {
            case levelTypes.forest:
                random = Random.Range(0, forestObstacles.Length - 1);
                return forestObstacles[random];
            case levelTypes.snow:
                random = Random.Range(0, snowObstacles.Length - 1);
                return snowObstacles[random];
            case levelTypes.lava:
                random = Random.Range(0, lavaObstacles.Length - 1);
                return lavaObstacles[random];
            case levelTypes.barren:
                random = Random.Range(0, barrenObstacles.Length - 1);
                return barrenObstacles[random];
            default:
                return null;
        }
    }

    public GameObject GetFloor(levelTypes levelType)
    {
        switch (levelType)
        {
            case levelTypes.forest:
                return floor[0];
            case levelTypes.snow:
                return floor[1];
            case levelTypes.lava:
                return floor[2];
            case levelTypes.barren:
                return floor[3];
            default:
                return null;
        }
    }
    public GameObject GetWater(levelTypes levelType)
    {
        switch (levelType)
        {
            case levelTypes.forest:
                return water[0];
            case levelTypes.snow:
                return water[1];
            case levelTypes.lava:
                return water[2];
            case levelTypes.barren:
                return water[3];
            default:
                return null;
        }
    }

    public GameObject GetFrontWall(levelTypes levelType)
    {
        switch (levelType)
        {
            case levelTypes.forest:
                return wallFront[0];
            case levelTypes.snow:
                return wallFront[1];
            case levelTypes.lava:
                return wallFront[2];
            case levelTypes.barren:
                return wallFront[3];
            default:
                return null;
        }
    }

    public GameObject GetBackWall(levelTypes levelType)
    {
        switch (levelType)
        {
            case levelTypes.forest:
                return wallBack[0];
            case levelTypes.snow:
                return wallBack[1];
            case levelTypes.lava:
                return wallBack[2];
            case levelTypes.barren:
                return wallBack[3];
            default:
                return null;
        }
    }

    public GameObject GetRightWall(levelTypes levelType)
    {
        switch (levelType)
        {
            case levelTypes.forest:
                return wallRight[0];
            case levelTypes.snow:
                return wallRight[1];
            case levelTypes.lava:
                return wallRight[2];
            case levelTypes.barren:
                return wallRight[3];
            default:
                return null;
        }
    }

    public GameObject GetLeftWall(levelTypes levelType)
    {
        switch (levelType)
        {
            case levelTypes.forest:
                return wallLeft[0];
            case levelTypes.snow:
                return wallLeft[1];
            case levelTypes.lava:
                return wallLeft[2];
            case levelTypes.barren:
                return wallLeft[3];
            default:
                return null;
        }
    }

    public GameObject GetHealthPot()
    {
        int random = (int)Random.Range(0, 100f);
        if(random<50)
        {
            return healthPots[0];
        }
        else if(random<90)
        {
            return healthPots[1];
        }
        else
        {
            return healthPots[2];
        }
    }

    public GameObject GetEnemy()
    {
        int random = (int)Random.Range(0, 100f);
        if (random<chanceForShotgunGuy)
        {
            return shotgunGuy;
        }
        else if(random<chanceForSniperGuy+ chanceForShotgunGuy)
        {
            return sniperGuy;
        }
        else if (random < chanceForSniperGuy + chanceForShotgunGuy + chanceForSMGGuy)
        {
            return SMGGuy;
        }
        else if (random < chanceForSniperGuy + chanceForShotgunGuy + chanceForSMGGuy + chanceForAK47Guy)
        {
            return AK47Guy;
        }
        else
        {
            return pistolGuy;
        }
    }

    public GameObject GetSupplies()
    {
        int rand = Random.Range(0, 100);
        if(rand<60)
        {
            return GetHealthPot();
        }
        else
        {
            return GetGun();
        }
    }

    public GameObject GetGun()
    {
        int random = (int)Random.Range(0, 100f);
        if (random < 50)
        {
            return guns[0];
        }
        else if (random < 65)
        {
            return guns[1];
        }
        else if (random < 75)
        {
            return guns[2];
        }
        else if (random < 85)
        {
            return guns[3];
        }
        else if (random < 95)
        {
            return guns[4];
        }
        else
        {
            return guns[5];
        }
    }

    public GameObject GetGun(int index)
    {
        if(index < guns.Length)
        {
            return guns[index];
        }
        else
        {
            return null;
        }
    }

}
