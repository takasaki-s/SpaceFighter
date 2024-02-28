using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム本編：ボス背景
// ※ボス到達のタイミングで上からスクロールして表示
public class AppBossStage : MonoBehaviour
{
    // メンバ変数
    private bool _IsMove;                               // 移動させる？
    private uint _MoveCount;                            // 現在移動時間
    private Vector3 _NowPos;                            // 現在座標

    // 設定値
    private const float _StartPos = 6.7f;            // 開始座標(Y軸値)
    private const float _TargetPos = 3.4f;           // 目標座標(Y軸値)
    private const uint _MoveTime = 60 * 7;           // 開始座標 → 目標座標への移動時間

    // Start is called before the first frame update
    void Start()
    {
        _IsMove = true;     // オブジェクトが有効になったら移動開始とする
        _MoveCount = 0;
        _NowPos.y = _StartPos;
        this.transform.position = _NowPos;
    }

    // Update is called once per frame
    void Update()
    {
        // ゲームオーバーになった？
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        if (_IsMove == true)
        {
            // 座標値更新
            _NowPos.y = _StartPos + ((_TargetPos - _StartPos) * (float)_MoveCount / _MoveTime);
            this.transform.position = _NowPos;

            // 時間値更新
            if (_MoveCount >= _MoveTime)
            {
                _IsMove = false;
            }
            else
            {
                _MoveCount++;
            }
        }
    }

    // ボス背景表示開始
    public void ShowStart()
    {
        this.gameObject.SetActive(true);
    }
}
