using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム本編：コウモリ型エネミー制御
public class AppEnemyBat : MonoBehaviour
{
    // メンバ変数
    private GameObject _AppMain;        // ゲームマネージャ
    private float _FallSpeed;           // 落下（接近）速度
    private float _MeanderingSpeed;     // 蛇行速度
    
    // Start is called before the first frame update
    void Start()
    {
        _AppMain = GameObject.FindGameObjectWithTag("AppMain");
        if (_AppMain != null)
        {
            _AppMain.GetComponent<AppMainManager>().AddEnemyCount();
        }

        this._FallSpeed = 0.01f + 0.1f * Random.value;
        this._MeanderingSpeed = Random.Range(1.0f, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // ゲームオーバーになった？
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        // HPが0になったら破棄
        if (this.GetComponent<AppEnemyStatus>().GetHp() == 0)
        {
            Destroy(this.gameObject);
        }

        // 移動
        transform.Translate(0.02f * Mathf.Sin(Time.time * this._MeanderingSpeed), -_FallSpeed, 0, Space.World);

        // 画面外に出たら破棄
        if (transform.position.y < -5.5f)
        {
            Destroy(this.gameObject);
        }
    }

    // オブジェクト破棄
    private void OnDestroy()
    {
        if (_AppMain != null)
        {
            _AppMain.GetComponent<AppMainManager>().SubEnemyCount();
        }
    }
}
