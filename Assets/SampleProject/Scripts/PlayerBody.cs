using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour {

    // 一つ前の胴体
    public PlayerBody forwardBody;
    
    // 過去状態 一つ後ろの胴体が参照する
    public Queue<PlayerWark.BodyInfo> ofterPos = new Queue<PlayerWark.BodyInfo>();
    protected int ofterFrm = 60;

    // Use this for initialization
    public virtual void Start () {

        // 一つ後ろの胴体が参照する状態を更新
        PlayerWark.BodyInfo info = new PlayerWark.BodyInfo();
        info.pos = transform.position;
        info.qua = transform.rotation;

        ofterPos.Enqueue(info);
    }

    // Update is called once per frame
    public virtual void Update () {

        if (StartCount.GetGameEndFlag() == true)
        {
            return;
        }

        // 一つ前の胴体のofterFrm前の状態に更新
        PlayerWark.BodyInfo ownpos = forwardBody.ofterPos.Peek();
        Debug.Log(gameObject.name + " : " + forwardBody.gameObject.name  );


        transform.position = ownpos.pos;
        transform.rotation = ownpos.qua;

        // 一つ後ろの胴体が参照する状態を更新
        PlayerWark.BodyInfo info = new PlayerWark.BodyInfo();
        info.pos = transform.position;
        info.qua = transform.rotation;
        ofterPos.Enqueue(info);

        // ofterFrm以上の情報は必要ではない
        if (ofterPos.Count > ofterFrm)
        {
            ofterPos.Dequeue();
        }

	}
}
