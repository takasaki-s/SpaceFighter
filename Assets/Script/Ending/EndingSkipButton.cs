using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// エンディング画面：スキップボタン
public class EndingSkipButton : MonoBehaviour
{
    // メンバ変数
    [SerializeField] private GameObject _Epilogue = null;           // エピローグ
    [SerializeField] private GameObject _StasffRoll = null;         // スタッフロール
    [SerializeField] private GameObject _SkipFade = null;           // フェードパネル
    [SerializeField] private AudioSource _ButtonEnterSe = null;     // ボタン押下SE

    private bool _IsSkip;                                           // スキップする？

    private uint _FadeCount;                                        // フェード経過時間
    private const uint _FadeTime = 60 * 1;                          // フェード時間設定

    private uint _SceneCount;                                        // シーン遷移待機経過時間
    private const uint _SceneTime = 60 * 3;                          // シーン遷移待機時間設定

    // Start is called before the first frame update
    void Start()
    {
        _IsSkip = false;
        _FadeCount = 0;
        _SceneCount = 0;
        _SkipFade.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(_IsSkip == true)
        {
            // フェード
            if(_FadeCount <= _FadeTime)
            {
                Color NowColor = _SkipFade.GetComponent<Image>().color;
                NowColor.a = (float)_FadeCount / _FadeTime;
                _SkipFade.GetComponent<Image>().color = NowColor;
                _FadeCount++;
            }

            // シーン遷移待機
            if(_SceneCount == _SceneTime)
            {
                SceneManager.LoadScene("TitleScene");
            }
            else
            {
                _SceneCount++;
            }
        }
    }

    // 「SKIP」ボタン押下時
    public void SkipButtonEnter()
    {
        _IsSkip = true;

        _ButtonEnterSe.Play();

        this.GetComponent<Image>().enabled = false;
        this.GetComponent<Button>().enabled = false;
        this.transform.Find("Text").gameObject.SetActive(false);

        _Epilogue.GetComponent<EndingEpilogue>().SetBgmFadeOut();
        _StasffRoll.GetComponent<EndingStuffRoll>().SetSkip();

        _SkipFade.SetActive(true);
    }

}
