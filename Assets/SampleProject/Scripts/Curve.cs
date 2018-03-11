using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve : MonoBehaviour {

    private double angle = 0;
    [SerializeField]
    private double scale;
    [SerializeField]
    private double speed;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        angle += speed;
        double sc = 1.0f + (System.Math.Sin(angle) * scale);
        transform.localScale = new Vector3((float)sc, (float)sc, (float)sc);

    }
    
}
