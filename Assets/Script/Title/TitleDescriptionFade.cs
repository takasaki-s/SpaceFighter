using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// タイトル画面：説明項目のフェード制御
public class TitleDescriptionFade : MonoBehaviour
{
    // メンバ変数
    private Color _textColor;       // テキストのカラー変数
    private float _FadeTime;        // 消える・現れる時間
    private float _NowAlpha;        // 現在のアルファ値
    private float _CulcAlpha;       // 1フレームで増減するアルファ値
    private bool _FadeFlg;          // 現在のフェード状態
    private bool _IsEnable;         // 動作有効/無効

    // Start is called before the first frame update
    void Start()
    {
        _FadeTime = 1.0f * 60;      // 6sec
        _NowAlpha = 0.0f;
        _CulcAlpha = 1.0f / _FadeTime;
        _FadeFlg = true;
        _textColor = this.GetComponent<Text>().color;
        _IsEnable = false;
        this.GetComponent<Text>().enabled = false;      // 画面遷移時の初回フェードが終わるまでは無効化しておく
        UpdateAlpha();
    }

    // Update is called once per frame
    void Update()
    {
        if (_IsEnable == true)
        {
            // アルファ値更新
            if (_FadeFlg == false)
            {
                // だんだん消える
                _NowAlpha -= _CulcAlpha;
                if (_NowAlpha <= 0.0f)
                {
                    _NowAlpha = 0.0f;
                    _FadeFlg = true;
                }
            }
            else if (_FadeFlg == true)
            {
                // だんだん現れる
                _NowAlpha += _CulcAlpha;
                if (_NowAlpha >= 1.0f)
                {
                    _NowAlpha = 1.0f;
                    _FadeFlg = false;
                }
            }

            // アルファ値適用
            UpdateAlpha();
        }
    }

    // アルファ更新
    private void UpdateAlpha()
    {
        _textColor.a = _NowAlpha;
        this.GetComponent<Text>().color = _textColor;
    }

    // フェード開始
    public void FadeStart()
    {
        _IsEnable = true;
        this.GetComponent<Text>().enabled = true;
    }

}
