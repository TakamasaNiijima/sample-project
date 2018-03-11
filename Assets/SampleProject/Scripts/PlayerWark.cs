using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerWark : PlayerBody {

    public GameVector dstickDir;

    public GameObject prefabRetry;
    public GameObject prefabSphere;
    public GameObject prefabBody;


    public QuerySDMecanimController anm;


    // stick傾き有効値
    public float stickSlope = 0.5f;
    public float speed;

    // 操作再受付時間
    public int stickTime=50;
    private int stickCount=50;

    // 向き
    public enum GameVector
    {
        Up,
        Down,
        Right,
        Left,

        Non
    }

    [SerializeField]
    // プレイヤーのむき
    private GameVector playerDir = GameVector.Down;

    // 胴体部用のプレイヤー(頭)情報
    [SerializeField]
    private const int maxBodyNum = 20;
    [SerializeField]
    private int posTime;
    private int posCnt=1;
    public struct BodyInfo
    {
        public Vector3 pos;
        public Quaternion qua;
    }
    private Stack<BodyInfo> bodyInfo = new Stack<BodyInfo>();
    //最後尾の胴体
    private PlayerBody bodyTarm;

    // 次の得点球体発生までの時間
    [SerializeField]
    private int sphereTime;
    private int sphereCnt=-1;

    // ゲームオーバーまでの待機時間
    [SerializeField]
    private int gameOverTime;
    private int gameOverCnt=-1;

    
	// Use this for initialization
	public override void Start () {

        // FrameRate固定
        Application.targetFrameRate = 60;

        // 最初は自身が最後尾
        bodyTarm = this;
		
	}
	
	// Update is called once per frame
	public override void Update () {

        if (StartCount.GetGameStart())
        {
            anm.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_WALK);
            //NORMAL_LOSE
        }

        // ゲームオーバ処理終了 リトライポップ
        if(gameOverCnt != -1 )
        {
            gameOverCnt++;
            if( gameOverCnt == gameOverTime )
            {
              Instantiate(prefabRetry);
            }
        }

        // ゲーム開始フラグがたっていれば
            if ( StartCount.GetGameStartFlag() == false || 
             StartCount.GetGameEndFlag() == true )
        {
            return;
        }
        

        transform.position += (transform.forward * speed);

        GameVector stickVec = GetVectorStick();

        dstickDir = stickVec;

        // 生成フラグが立っているなら時間をカウントし特定時間になったら球体得点を生成
        if (sphereCnt != -1)
        {
            sphereCnt++;
            if (sphereCnt > sphereTime)
            {
                System.Random rnd = new System.Random(stickCount);
                Vector3 vec = new Vector3(rnd.Next(-4, 4), 0.37f, rnd.Next(-4, 4));
                Instantiate(prefabSphere, vec, Quaternion.identity);

                // 生成したのでフラグを下す
                sphereCnt = -1;
            }
        }

        
        
        // 一つ後ろの胴体が参照する状態を更新
        BodyInfo info = new BodyInfo();
        info.pos = transform.position;
        info.qua = transform.rotation;
        ofterPos.Enqueue(info);

        // ofterFrm以上の情報は必要ではない
        if (ofterPos.Count > ofterFrm)
        {
            ofterPos.Dequeue();
        }


        // 時間毎にプレイヤーの情報を格納
        posCnt++;
        if ((posCnt % posTime) == 0)
        {
            BodyInfo body = new BodyInfo();
            body.pos = transform.position;
            body.qua = transform.rotation;
            bodyInfo.Push(body);
        }

        // 操作受付時間を満たしていないなら無視
        stickCount++;
        if( stickTime > stickCount )
        {
            return;
        }

        // stick操作なしなら無視
        if (stickVec == GameVector.Non)
        {
            return;
        }

        // 同じ向きなら操作を無視
        if (stickVec == playerDir)
        {
            return;
        }

        switch(stickVec)
        {
            case GameVector.Up:
                {
                    if(playerDir==GameVector.Down)
                    {
                        break;
                    }

                    transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    playerDir = GameVector.Up;
                    stickCount = 0;
                    break;
                }
            case GameVector.Down:
                {
                    if (playerDir == GameVector.Up)
                    {
                        break;
                    }

                    transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                    playerDir = GameVector.Down;
                    stickCount = 0;
                    break;
                }
            case GameVector.Right:
                {
                    if (playerDir == GameVector.Left)
                    {
                        break;
                    }
                    
                    transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    playerDir = GameVector.Right;
                    stickCount = 0;
                    break;
                }
            case GameVector.Left:
                {
                    if (playerDir == GameVector.Right)
                    {
                        break;
                    }

                    transform.rotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
                    playerDir = GameVector.Left;
                    stickCount = 0;
                    break;
                }
        }
        
    }

    GameVector GetVectorStick()
    {
        Vector3 stickVec = new Vector3();

        stickVec.x = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        stickVec.z = CrossPlatformInputManager.GetAxisRaw("Vertical");
        
        
        if ( stickVec.magnitude < stickSlope)
        {
            return GameVector.Non;
        }

        stickVec.Normalize();

        float absX = System.Math.Abs(stickVec.x);
        float absZ = System.Math.Abs(stickVec.z);

        if( absX > absZ )
        {
            if(0 < stickVec.x)
            {
                return GameVector.Right;
            }
            else
            {
                return GameVector.Left;
            }
        }
        else
        {
            if(0 < stickVec.z)
            {
                return GameVector.Up;
            }
            else
            {
                return GameVector.Down;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 得点球体に衝突
        if (other.name == "Sphere" ||
            other.name == "Sphere(Clone)" )
        {
            Destroy(other.gameObject);

            //得点加算
            StartCount.AddScore();

            // 新しい得点球体を生成を予約
            sphereCnt = 0;


            // 過去のプレイヤー情報の位置に胴体部分の作成
            BodyInfo body = bodyInfo.Pop();
            var bodyObj = Instantiate(prefabBody, body.pos , body.qua);
            var playerBody = bodyObj.GetComponent<PlayerBody>();
            playerBody.forwardBody = bodyTarm;
            bodyTarm = playerBody;
            
        }
        else
        {
            Debug.Log(other.name);

            // 衝突したのでゲームオーバー処理
            StartCount.GameEndCall();
            anm.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_LOSE);

            gameOverCnt = 0;
        }

    }
    
}
