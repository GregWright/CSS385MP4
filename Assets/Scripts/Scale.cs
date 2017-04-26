using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        transform.localScale = new Vector3(1, 1, 1);

        var width = sr.sprite.bounds.size.x;
        var height = sr.sprite.bounds.size.y;

        var worldScreenHeight = Camera.main.orthographicSize * 2.0;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        float x = (float) worldScreenWidth / width;
        float y = (float) worldScreenHeight / height;
        transform.localScale = new Vector3(x, y, transform.localScale.z);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
