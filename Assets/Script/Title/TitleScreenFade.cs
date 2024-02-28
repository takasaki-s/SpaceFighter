using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// タイトル画面：画面フェード制御
public class TitleScreenFade : MonoBehaviour
{
    // 列挙
    enum FadeInfo                   // フェード状態
    {
        FadeIn,                     // フェードイン
        FadeInEnd,                  // フェードイン完了
        FadeOut,                    // フェードアウト
        FadeOutEnd,                 // フェードアウト完了
    }

    // メンバ変数
    private GameObject _Description;    // 説明表示
    private Color _ImageColor;      // イメージのカラー変数
    private float _FadeTime;        // 消える・現れる時間
    private float _NowAlpha;        // 現在のアルファ値
    private float _CulcAlpha;       // 1フレームで増減するアルファ値
    private FadeInfo _FadeInfo;     // フェード状態

    // Start is called before the first frame update
    void Start()
    {
        _Description = GameObject.Find("Description");
        this.GetComponent<Image>().enabled = true;
        _FadeTime = 3.0f * 60;      // 3sec
        _NowAlpha = 1.0f;
        _CulcAlpha = 1.0f / _FadeTime;
        _FadeInfo = FadeInfo.FadeIn;
        _ImageColor = this.GetComponent<Image>().color;
    }

    // Update is called once per frame
    void Update()
    {
        if (_FadeInfo == FadeInfo.FadeIn)
        {
            // フェードイン中

            // 終了判定
            if (_NowAlpha <= 0.0f)
            {
                this.FirstFadeEnd();
            }

            // だんだん透明度を上げる（画面が現れる）
            _NowAlpha -= _CulcAlpha;
            if (_NowAlpha < 0.0f)
            {
                _NowAlpha = 0.0f;
            }

            // 黒パネルにアルファ値適用
            _ImageColor.a = _NowAlpha;
            this.GetComponent<Image>().color = _ImageColor;

        }
        else if (_FadeInfo == FadeInfo.FadeOut)
        {
            // フェードアウト中

            // 終了判定
            if (_NowAlpha >= 1.0f)
            {
                _FadeInfo = FadeInfo.FadeOutEnd;
            }

            // だんだん透明度を上げる（画面が現れる）
            _NowAlpha += _CulcAlpha;
            if (_NowAlpha >= 1.0f)
            {
                _NowAlpha = 1.0f;
            }

            // 黒パネルにアルファ値適用
            _ImageColor.a = _NowAlpha;
            this.GetComponent<Image>().color = _ImageColor;

        }
    }

    // 初回フェード終了処理
    private void FirstFadeEnd()
    {
        _FadeInfo = FadeInfo.FadeInEnd;
        _NowAlpha = 0;
        this.GetComponent<Image>().enabled = false;

        // 初回フェード終了したら説明アイコン表示
        _Description.GetComponent<TitleDescriptionFade>().FadeStart();
    }

    // フェードイン中？
    public bool IsFadeIn()
    {
        if (_FadeInfo == FadeInfo.FadeIn)
        {
            return true;
        }
        return false;
    }

    // フェードアウト中？
    public bool IsFadeOut()
    {
        if (_FadeInfo == FadeInfo.FadeOut)
        {
            return true;
        }
        return false;
    }

    // フェードアウト終了？
    public bool IsFadeOutEnd()
    {
        if (_FadeInfo == FadeInfo.FadeOutEnd)
        {
            return true;
        }
        return false;
    }

    // フェードインキャンセル
    public void FadeInCancel()
    {
        if (_FadeInfo == FadeInfo.FadeIn)
        {
            this.FirstFadeEnd();
        }
    }

    // フェードアウト開始
    public void SetFadeOut()
    {
        _FadeInfo = FadeInfo.FadeOut;
        _NowAlpha = 0;
        _ImageColor.a = _NowAlpha;
        this.GetComponent<Image>().color = _ImageColor;
        this.GetComponent<Image>().enabled = true;
    }

}
