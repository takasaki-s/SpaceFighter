using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム本編：ボスチャージ弾
public class AppCharge : MonoBehaviour
{
    // 列挙
    enum Action                             // 動作
    {
        In,                                 // 導入
        Move,                               // 移動
        Max
    }

    // メンバ変数
    private Action _Action;                 // 動作
    private Vector3 _MoveVec;               // 移動ベクトル

    // Start is called before the first frame update
    void Start()
    {
        _Action = Action.In;
        this.GetComponent<AppEnemyStatus>().SetHp(100); // 実質破壊不能オブジェクト
        this.GetComponent<AppEnemyStatus>().SetDamage(30.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // ゲームオーバーになった？
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        if (_Action == Action.Move)
        {
            this.transform.position += _MoveVec;
        }

        // 画面外に出たらオブジェクト破棄
        if (transform.position.y < -6)
        {
            Destroy(this.gameObject);
        }
    }

    // 移動可能指定
    public void SetMoveActive(Vector3 Current, Vector3 Target)
    {
        // 自機に向けてのベクトル作成
        _MoveVec = Target - Current;
        _MoveVec.Normalize();
        _MoveVec *= 0.15f;
        _Action = Action.Move;

        // タグとレイヤーを更新
        this.gameObject.tag = "Enemy";
        this.gameObject.layer = LayerMask.NameToLayer("Default");
    }

}
