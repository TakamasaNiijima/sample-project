using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // タッチされていたら次のシーンへ
        if (0 < Input.touchCount ||
             Input.GetMouseButton(0) )
        {
            SceneManager.LoadScene("GameMain");
        }
    }
    
}
