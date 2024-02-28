using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム本編：敵の弾（ボスで使用）
public class AppEnemyBullet : MonoBehaviour
{
    // メンバ変数
    private GameObject _BossObject = null;      // ボスオブジェクト

    // 設定値
    private const float _MoveSpeed = -0.1f;     // 弾の移動スピード
    private const float _OutScreen = -6.0f;     // 画面外に出たときに弾を破棄するy座標値

    // Start is called before the first frame update
    void Start()
    {
        _BossObject = GameObject.FindWithTag("Boss");

    }

    // Update is called once per frame
    void Update()
    {
        // HPが0になったら破棄（ボス本体と自身いずれか）
        // ※ゲームオーバーより先に判定しないと崩壊演出で弾が破棄されない
        if (_BossObject.GetComponent<AppEnemyStatus>().GetHp() == 0 || this.GetComponent<AppEnemyStatus>().GetHp() == 0)
        {
            Destroy(this.gameObject);
        }

        // ゲームオーバーになった？
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        // 弾移動
        transform.Translate(0, _MoveSpeed, 0);

        // 画面外に出たらオブジェクト破棄
        if (transform.position.y < _OutScreen)
        {
            Destroy(this.gameObject);
        }
    }
}
