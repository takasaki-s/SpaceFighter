using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム本編：エネミー（岩）制御
public class AppEnemyRock : MonoBehaviour
{
    // メンバ変数
    private GameObject _AppMain;    // ゲームマネージャ
    private float _FallSpeed;       // 落下（接近）速度
    private float _RotSpeed;        // 回転速度
    
    // Start is called before the first frame update
    void Start()
    {
        _AppMain = GameObject.FindGameObjectWithTag("AppMain");
        if (_AppMain != null)
        {
            _AppMain.GetComponent<AppMainManager>().AddRockCount();
        }

        this._FallSpeed = 0.01f + 0.1f * Random.value;
        this._RotSpeed = 3f * Random.value;
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
        transform.Translate(0, -_FallSpeed, 0, Space.World);

        // 回転
        transform.Rotate(0, 0, _RotSpeed);

        // 画面外に出たらオブジェクト破棄
        if (transform.position.y < -5.5f)
        {
            Destroy(gameObject);
        }
    }

    // オブジェクト破棄
    private void OnDestroy()
    {
        if (_AppMain != null)
        {
            _AppMain.GetComponent<AppMainManager>().SubRockCount();
        }
    }
}
