using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundBehavior : MonoBehaviour {

    #region World bounds / camera bounds
    private Bounds worldBound;
    private Vector2 myWorldCenter;

public enum BoundStatus
    {
        CollideTop,
        CollideLeft,
        CollideRight,
        CollideBottom,
        Outside,
        Inside
    };
    #endregion

    #region level Managment
    public enum level
    {
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
    }
    public struct levelVariables
    {
        public float enemySpawnInterval;
        public int intialEnemies;
        public int killsToAdvance;
        public float enemySpeedMuilt; //enemeyspeed muiltiplier
        public int enemyHP;
        
        public levelVariables(float Interval, int intital, int advance, float enspeed, int enHP)
        {
            enemySpawnInterval = Interval;
            intialEnemies = intital;
            killsToAdvance = advance;
            enemySpeedMuilt = enspeed;
            enemyHP = enHP;
        }

    }

    private levelVariables level1 = new levelVariables(3.0f, 5,5,1f,3);
    private levelVariables level2 = new levelVariables(2.0f, 20,30,1.5f,3);
    private levelVariables level3 = new levelVariables(1.0f, 50,500,2.0f,5);
    public level currentlevel = level.Level1;

    bool Win = false;
    #endregion


    #region enemy control variables
    private bool enemyCanMove = false;
    private float nextEnemy = 3.0f;

    public float enemySpawnInterval;
    public int intialEnemies;
    public int killsToAdvance;

    public GameObject enemySpawn = null;
    public int currentEggs = 0;
    public int currentEnemies = 0;
    public int totalkilled = 0;

    public float enemySpeedMuilt; //enemeyspeed muiltiplier
    public int enemyHP;
    #endregion
    // Use this for initialization

    private GUIStyle style = new GUIStyle();
    void OnGUI()
    {
        if (Win)
        {
            style.fontSize = 72;
            style.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(Screen.width/2 - 200, Screen.height/2-50, 400, 100), "You Win!", style);
            style.fontSize = 36;
            if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 50,200 , 50), "Continue?",style))
            {
                switch (currentlevel)
                {
                    case level.Level2:
                        SceneManager.LoadScene("Level2", LoadSceneMode.Single);
                        break;
                    case level.Level3:
                        SceneManager.LoadScene("Level3", LoadSceneMode.Single);
                        break;
                    case level.Level1:
                        //??
                        break;
                }
            }
        }
        else
        {
            GUI.Box(Rect.MinMaxRect(10, 10, 110, 110), "Baskets:\n  Current: " + currentEnemies + "\n  filled: " + totalkilled + "\nEggs Depolyed:\n" + currentEggs);
        }
    }



    void Start () {
        //Scene check
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Level1")
            currentlevel = level.Level1;
        else if (sceneName == "Level2")
            currentlevel = level.Level2;
        else if (sceneName == "Level3")
            currentlevel = level.Level3;
        else //ERROR defult level1
            currentlevel = level.Level1;

            setWorldBound();
        if (enemySpawn == null) {
            enemySpawn = Resources.Load("Prefabs/Enemy") as GameObject;
        }

        switch (currentlevel)
        {
            case level.Level1:
                loadLevelVar(level1);
                break;
            case level.Level2:
                loadLevelVar(level2);
                break;
            case level.Level3:
                loadLevelVar(level3);
                break;
        }

        for (int i = 0; i < intialEnemies; i++)
        {
            GameObject e = Instantiate(enemySpawn) as GameObject;
            currentEnemies++;
        }
	}
	
    private void loadLevelVar (levelVariables lvl)
    {
        enemySpawnInterval = lvl.enemySpawnInterval;
        intialEnemies = lvl.intialEnemies;
        killsToAdvance = lvl.killsToAdvance;
        enemySpeedMuilt = lvl.enemySpeedMuilt; //enemeyspeed muiltiplier
        enemyHP = lvl.enemyHP;

}
    // Update is called once per frame
    void Update () {

        //level change
        if (totalkilled >= killsToAdvance)
        {
            switch (currentlevel)
            {
                case level.Level1:
                    currentlevel = level.Level2;
                    Win = true;
                    killsToAdvance = level2.killsToAdvance;
                    break;
                case level.Level2:
                    currentlevel = level.Level3;
                    killsToAdvance = level3.killsToAdvance;
                    Win = true;
                    break;
                case level.Level3:
                    //TODO you win
                    break;
            }
        }

        spawnEnemy();
	}

    private void spawnEnemy() {
        if (Time.time > nextEnemy)
        {
            nextEnemy = Time.time + enemySpawnInterval;
            GameObject e = Instantiate(enemySpawn) as GameObject;
            currentEnemies++;
        }
    }
    public bool canMove { get { return enemyCanMove; } }


    public void setWorldBound()
    {
        Vector3 cam = Camera.main.transform.position;
        cam.z = 0.0f;
        myWorldCenter = new Vector2(cam.x, cam.y); 
        float sizeY = Camera.main.orthographicSize*2;
        float sizeX = Camera.main.orthographicSize * Camera.main.aspect*2;
        float sizeZ = Mathf.Abs(Camera.main.farClipPlane - Camera.main.nearClipPlane);
        worldBound.center = cam;
        worldBound.size = new Vector3(sizeX, sizeY, sizeZ);
    }
    public Bounds worldBounds { get { return worldBound; } }
    public Vector2 WorldCenter { get { return myWorldCenter; } }

    public BoundStatus objectCollideWorldBound(Bounds objBound)
    {
        BoundStatus mySatus = BoundStatus.Inside;

        if (worldBound.Intersects(objBound))
        {
            if (objBound.max.x > worldBound.max.x)
                mySatus = BoundStatus.CollideRight;
            else if (objBound.min.x < worldBound.min.x)
                mySatus = BoundStatus.CollideLeft;
            else if (objBound.max.y > worldBound.max.y)
                mySatus = BoundStatus.CollideTop;
            else if (objBound.min.y < worldBound.min.y)
                mySatus = BoundStatus.CollideBottom;
            else if ((objBound.min.z < worldBound.min.z) || (objBound.max.z > worldBound.max.z))
                mySatus = BoundStatus.Outside;
        }
        return mySatus;
    }
}
