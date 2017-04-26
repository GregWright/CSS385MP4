using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

    #region Control Variables
    private float maxSpeed = 30f;
    private float maxRotate = 67.5f;
    BackgroundBehavior myBackground = null;
    #endregion

    #region other variables
    public GameObject myProjectile = null;
    private float fireRate = 0.1f;
    private float nextFire = 0.0f;
    #endregion

    // Use this for initialization
    void Start () {
        if (null == myProjectile)
        {
            myProjectile = Resources.Load("Prefabs/egg") as GameObject;
        }
        myBackground = GameObject.Find("Manager").GetComponent<BackgroundBehavior>();

    }

    // Update is called once per frame
    void Update () {
        #region player movement
        transform.position += Input.GetAxis("Vertical") * transform.up * maxSpeed * Time.smoothDeltaTime;
        transform.Rotate(Vector3.forward, Input.GetAxis("Horizontal") * maxRotate * Time.smoothDeltaTime*-1f);
        #endregion

        #region player world clamp
        BackgroundBehavior.BoundStatus status =
            myBackground.objectCollideWorldBound(GetComponent<Renderer>().bounds);
        Bounds myBounds = myBackground.worldBounds;
        if (status != BackgroundBehavior.BoundStatus.Inside) { 
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, myBounds.min.x+5, myBounds.max.x-5),
                    Mathf.Clamp(transform.position.y, myBounds.min.y+5, myBounds.max.y-5),0.0f);
        }
        #endregion
        #region Lanch Projectile
        if (Input.GetAxis("Fire1") > 0f && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject clone = Instantiate(myProjectile) as GameObject;
            ProjectileBehavior behavior = clone.GetComponent<ProjectileBehavior>();
            if(behavior != null)
            {
                clone.transform.position = transform.position;
                behavior.setDirection(transform.up);
                myBackground.currentEggs++;
            }
        }
        #endregion
    }
}
