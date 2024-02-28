using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム本編：背景制御
public class AppBackground : MonoBehaviour
{
    // メンバ変数
    private bool _IsScroll;     // 背景をスクロールさせるか。
    private uint _MoveCount;    // スクロール時間
    private Vector3 _BasePos;   // 基準となる座標

    // 設定値
    private const uint _MoveTime = 60 * 20;      // スクロール一周にかかる時間
    private const float _MoveLength = 10.24f;    // スクロール一周にかかる移動量

    // Start is called before the first frame update
    void Start()
    {
        _IsScroll = true;
        _MoveCount = 0;
        _BasePos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // スクロールさせていい？
        if ( _IsScroll == false || Mathf.Approximately(Time.timeScale, 0f) == true)
        {
            return;
        }

        // スクロール（インクリすると誤差が蓄積することがあるので総移動量から計算で求める）
        Vector3 NowPos = this.transform.position;
        NowPos.y = _BasePos.y - (_MoveLength * _MoveCount / _MoveTime);
        this.transform.position = NowPos;

        // 時間値更新
        if (_MoveCount >= _MoveTime)
        {
            _MoveCount = 0;
        }
        else
        {
            _MoveCount++;
        }

    }

    // 背景スクロール設定
    public void SetScroll(bool IsScroll)
    {
        _IsScroll = IsScroll;
    }
}
