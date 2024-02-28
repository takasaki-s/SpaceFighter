using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 共通：FPS計測スクリプト
// ※ヒエラルキーにテキストを配置してアタッチする
public class CmnDebugFps : MonoBehaviour
{
    // メンバ変数
    private uint _FpsCount;     // フレーム数カウント値
    private float _LastTime;    // 前回時間

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        this.gameObject.SetActive(true);   // 
#else
        this.gameObject.SetActive(false);
#endif

        _FpsCount = 0;
        _LastTime = Time.time;

        UpdateFpsCount();
    }

    // Update is called once per frame
    void Update()
    {
        // 前回時間から1s経過していればFPS値を更新
        float NowTime = Time.time;
        if (NowTime >= _LastTime + 1.0f)
        {
            UpdateFpsCount();
            _LastTime = NowTime;
            _FpsCount = 0;
        }
        else
        {
            _FpsCount++;
        }
    }

    // FPS表示更新
    private void UpdateFpsCount()
    {
        this.GetComponent<Text>().text = _FpsCount.ToString() + " fps";
    }
}
