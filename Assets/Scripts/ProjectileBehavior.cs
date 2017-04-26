using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour {

    #region Control Variables
    private float maxSpeed = 100f;
    BackgroundBehavior myBackground = null;
    #endregion

    // Use this for initialization
    void Start () {
        myBackground = GameObject.Find("Manager").GetComponent<BackgroundBehavior>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.position += maxSpeed * Time.smoothDeltaTime * transform.up;
        BackgroundBehavior.BoundStatus status =
    myBackground.objectCollideWorldBound(GetComponent<Renderer>().bounds);
        if (status != BackgroundBehavior.BoundStatus.Inside)
        {
            Destroy(this.gameObject);
            myBackground.currentEggs--;
        }
    }

    public void setDirection(Vector3 dir)
    {
        transform.up = dir;
    }
}
