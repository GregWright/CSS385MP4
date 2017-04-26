using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #region enemy control variables
    private float enemySpawnInterval = 3.0f;
    private bool enemyCanMove = false;
    private float nextEnemy = 3.0f;
    //private bool spaceDetect = false;
    public GameObject enemySpawn = null;
    private int intialEnemies = 50;
    public int currentEggs = 0;
    public int currentEnemies = 0;
    public int totalkilled = 0;
    #endregion
    // Use this for initialization
    void OnGUI()
    {
        GUI.Box(Rect.MinMaxRect(10, 10, 110, 110), "Baskets:\n  Current: "+currentEnemies+"\n  filled: "+totalkilled+"\nEggs Depolyed:\n"+currentEggs);
    }



    void Start () {
        setWorldBound();
        if (enemySpawn == null) {
            enemySpawn = Resources.Load("Prefabs/Enemy") as GameObject;
        }
        for (int i = 0; i < intialEnemies; i++)
        {
            GameObject e = Instantiate(enemySpawn) as GameObject;
            currentEnemies++;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            enemyCanMove = !enemyCanMove;
        }
        if (canMove)
        {
            spawnEnemy();
        }
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
