using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {
    

    #region Control variables
    public float speed;
    public float runAwayRotation = 45f;
    public float rotationspeed = 9.0f;
    BackgroundBehavior myBackground = null;
    GameObject player = null;
    private int hits = 3;
    EnemyState currentState = EnemyState.Normal;
    private float rundist = 30f;
    private float stunTimer = 0.0f;
    private float stunTime = 5f;

    private enum EnemyState
    {
        Normal,
        Run,
        Stunned
    };
    #endregion
    // Use this for initialization
    void Start () {
		myBackground = GameObject.Find("Manager").GetComponent<BackgroundBehavior>();
        player = GameObject.Find("Player");
        hits = myBackground.enemyHP;
        speed = Random.Range(20, 40)*myBackground.enemySpeedMuilt;
        float tempX = Random.Range(myBackground.worldBounds.min.x, myBackground.worldBounds.max.x);
        float tempY = Random.Range(myBackground.worldBounds.min.x, myBackground.worldBounds.max.x);
        transform.position = new Vector3(tempX, tempY, 0.0f);
        NewDirection();
    }
	
	// Update is called once per frame
	void Update () {
        if(currentState == EnemyState.Stunned)
        {
            if(Time.time > stunTimer + stunTime)
            {
                setNormal();
            }
            else
            {
                transform.Rotate(Vector3.forward, rotationspeed * Time.smoothDeltaTime);
            }
        }
        if (currentState != EnemyState.Stunned)
        {
            if (checkRun())
            {
                setRun();
            }
            else
            {
                setNormal();
            }
        }
        if (currentState == EnemyState.Run)
        {
            runAway();
        }
        if (/*myBackground.canMove && */currentState != EnemyState.Stunned)
        {
            transform.position += speed * Time.smoothDeltaTime * transform.up;
            BackgroundBehavior.BoundStatus status =
                myBackground.objectCollideWorldBound(GetComponent<Renderer>().bounds);
            if (status != BackgroundBehavior.BoundStatus.Inside)
            {
                NewDirection();
            }
        }
	}
    private void NewDirection()
    {
        Vector2 v = myBackground.WorldCenter - new Vector2(transform.position.x, transform.position.y);
        // this is vector that will take us back to world center
        v.Normalize();
        Vector2 vn = new Vector2(v.y, -v.x); // this is a direciotn that is perpendicular to V

        float useV = 1.0f - Mathf.Clamp(0.5f, 0.01f, 1.0f);
        float tanSpread = Mathf.Tan(useV * Mathf.PI / 2.0f);

        float randomX = Random.Range(0f, 1f);
        float yRange = tanSpread * randomX;
        float randomY = Random.Range(-yRange, yRange);

        Vector2 newDir = randomX * v + randomY * vn;
        newDir.Normalize();
        transform.up = newDir;

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Hit!");

        if(other.gameObject.name == "egg(Clone)")
        {
            Destroy(other.gameObject);
            myBackground.currentEggs--;
            if (hits <= 1)
            {
                Destroy(this.gameObject);
                myBackground.currentEnemies--;
                myBackground.totalkilled++;
            }
            else
            {
                hits--;
                setStun();
            }
        }
    }

    private bool checkRun()
    {
        Vector3 dist = transform.position - player.transform.position;
        Vector3 temp = transform.position;
        if (dist.magnitude > rundist)
            return false;
        dist.Normalize();
        temp.Normalize();
        if (Vector3.Dot(temp, dist) > 0)
        {
            return true;
        }
        return false;
    }
    private void runAway()
    {
        Vector3 dist = transform.position - player.transform.position;
        Vector3 temp = transform.position;
        dist.Normalize();
        temp.Normalize();
        if (Vector3.Cross(temp, dist).z <= 0f)
        {
            transform.Rotate(Vector3.forward, runAwayRotation * Time.smoothDeltaTime * -1f);
        }
        else
        {
            transform.Rotate(Vector3.forward, runAwayRotation * Time.smoothDeltaTime);
        }
        transform.position +=  transform.up * speed * Time.smoothDeltaTime;

    }
    private void setStun()
    {
        currentState = EnemyState.Stunned;
        GetComponent<SpriteRenderer>().color = Color.blue;
        stunTimer = Time.time;
        
    }

    private void setRun()
    {
        currentState = EnemyState.Run;
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void setNormal()
    {
        currentState = EnemyState.Normal;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
