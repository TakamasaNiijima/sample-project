using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartCount : MonoBehaviour {

    static private long count;
    private static bool gameStartFlag = false;
    private static bool gameEndFlag = false;

    public long count2;
    public long count1;
    public long count0;
    static public long countMax=210;

    static public int scoreNum;
    static public int scoreAdd=10;

    public Text text;
    public Text score;

    public static bool GetGameStartFlag()
    {
        return gameStartFlag;
    }

    public static bool GetGameEndFlag()
    {
        return gameEndFlag;
    }

    public static void GameEndCall()
    {
        gameEndFlag = true ;
    }

    public static bool GetGameStart()
    {
        return (count == countMax) ? true : false;
    }

    public static void AddScore()
    {
        scoreNum += scoreAdd;
    }

    public static void Init()
    {
        count = 0;
        scoreNum = 0;

        gameStartFlag = false;
        gameEndFlag = false;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        count++;

        if(count > countMax)
        {
            Destroy(text);
            gameStartFlag = true;
        }
        else if (count > count0)
        {
            text.text = "   Start!";
        }
        else if (count > count1)
        {
            text.text = "      1";
        }
        else if (count > count2)
        {
            text.text = "      2";
        }

        score.text = scoreNum.ToString();
	}
}
