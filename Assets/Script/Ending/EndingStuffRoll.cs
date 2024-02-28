using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingStuffRoll : MonoBehaviour
{
    // 列挙
    private enum TextStatus                // テキスト状態
    {
        None = 0,
        In,                                 // フェードイン（徐々に表れる）
        Lp,                                 // 表示
        Out,                                // フェードアウト（徐々に消える）
        Next,                               // スタッフロールへ遷移
        Max
    }

    // 文字データフォーマット
    private struct TextData
    {
        // メンバ変数
        public uint StartFrame { get; }
        public TextStatus TextStatus { get; }
        public string PartCredit { get; }
        public string NameCredit1 { get; }
        public string NameCredit2 { get; }
        public string NameCredit3 { get; }

        // 初期化
        public TextData(uint StartFrame, TextStatus TextStatus, string PartCredit, string NameCredit1, string NameCredit2, string NameCredit3)
        {
            this.StartFrame = StartFrame;
            this.TextStatus = TextStatus;
            this.PartCredit = PartCredit;
            this.NameCredit1 = NameCredit1;
            this.NameCredit2 = NameCredit2;
            this.NameCredit3 = NameCredit3;
        }
    }

    // 文字データテーブル
    uint _TextDataIndex;
    private readonly TextData[] _TextDataTbl = {
            new TextData( 60 *  0, TextStatus.In,  "企画", "髙﨑　翔", "", ""),
            new TextData( 60 *  5, TextStatus.Out,  "企画", "髙﨑　翔", "", ""),

            new TextData( 60 *  6, TextStatus.In,  "2D素材", "ぴぽや倉庫", "かくめる素材工房", "宇宙壁紙"),// 空想曲線
            new TextData( 60 * 11, TextStatus.Out, "2D素材", "ぴぽや倉庫", "かくめる素材工房", "宇宙壁紙"),

            new TextData( 60 * 12, TextStatus.In, "サウンド素材", "魔王魂", "効果音ファクトリー", ""),
            new TextData( 60 * 18, TextStatus.Out, "サウンド素材", "魔王魂", "効果音ファクトリー", ""),

            new TextData( 60 * 19, TextStatus.In,  "プログラム", "髙﨑　翔", "", ""),
            new TextData( 60 * 25, TextStatus.Out,  "プログラム", "髙﨑　翔", "", ""),

            new TextData( 60 * 26, TextStatus.In, "", "Thank you for playing", "", ""),
            new TextData( 60 * 34, TextStatus.Out, "", "Thank you for playing", "", ""),

            new TextData( 60 * 37, TextStatus.Next, "", "", "", "")        // タイトルへ遷移
    };

    // 関数テーブル
    private delegate void UpdateFunc();
    private UpdateFunc[] _UpdateFuncTbl;

    // メンバ変数
    private GameObject _PartCredit = null;          // 役割
    private GameObject _NameCredit1 = null;         // クレジット1
    private GameObject _NameCredit2 = null;         // クレジット2
    private GameObject _NameCredit3 = null;         // クレジット3

    private TextStatus _TextStatus;                 // テキスト状態

    private bool _IsSkip;                           // スキップする？

    private uint _FrameCount;                       // 経過時間
    private uint _FadeCount;                        // フェード時間

    private const uint _TextFadeTime = 60;          // テキストフェード設定時間
    private const uint _BlackFadeTime = 60 * 10;    // 黒板フェード設定時間

    // Start is called before the first frame update
    void Start()
    {
        _TextDataIndex = 0;

        _PartCredit = this.transform.Find("PartCredit ").gameObject;
        _NameCredit1 = this.transform.Find("NameCredit1").gameObject;
        _NameCredit2 = this.transform.Find("NameCredit2").gameObject;
        _NameCredit3 = this.transform.Find("NameCredit3").gameObject;

        Color _TextColor = _PartCredit.GetComponent<Text>().color;
        _TextColor.a = 0.0f;
        _PartCredit.GetComponent<Text>().color = _TextColor;
        _NameCredit1.GetComponent<Text>().color = _TextColor;
        _NameCredit2.GetComponent<Text>().color = _TextColor;
        _NameCredit3.GetComponent<Text>().color = _TextColor;

        _TextStatus = TextStatus.None;

        _FrameCount = 0;

        _UpdateFuncTbl = new UpdateFunc[(int)TextStatus.Max];
        _UpdateFuncTbl[(int)TextStatus.None] = null;
        _UpdateFuncTbl[(int)TextStatus.In] = UpdateIn;
        _UpdateFuncTbl[(int)TextStatus.Lp] = UpdateLp;
        _UpdateFuncTbl[(int)TextStatus.Out] = UpdateOut;
        _UpdateFuncTbl[(int)TextStatus.Next] = UpdateNext;

        _IsSkip = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_TextStatus != TextStatus.Next)
        {
            // テキスト状態更新フレームになった？
            if (_FrameCount == _TextDataTbl[_TextDataIndex].StartFrame)
            {
                _TextStatus = _TextDataTbl[_TextDataIndex].TextStatus;
                _PartCredit.GetComponent<Text>().text = _TextDataTbl[_TextDataIndex].PartCredit;
                _NameCredit1.GetComponent<Text>().text = _TextDataTbl[_TextDataIndex].NameCredit1;
                _NameCredit2.GetComponent<Text>().text = _TextDataTbl[_TextDataIndex].NameCredit2;
                _NameCredit3.GetComponent<Text>().text = _TextDataTbl[_TextDataIndex].NameCredit3;

                if (_TextStatus == TextStatus.Next)
                {
                    _FadeCount = _BlackFadeTime;
                }
                else
                {
                    _FadeCount = _TextFadeTime;
                }

                _TextDataIndex++;
            }
        }

        // 更新関数実行
        if (_TextStatus != TextStatus.None)
        {
            _UpdateFuncTbl[(int)_TextStatus]();
        }

        // 時間値更新
        if (_FrameCount < uint.MaxValue)
        {
            _FrameCount++;
        }
    }

    // 更新処理：フェードイン
    private void UpdateIn()
    {
        Color _TextColor = _PartCredit.GetComponent<Text>().color;
        _TextColor.a = (float)(_TextFadeTime - _FadeCount) / _TextFadeTime;
        _PartCredit.GetComponent<Text>().color = _TextColor;
        _NameCredit1.GetComponent<Text>().color = _TextColor;
        _NameCredit2.GetComponent<Text>().color = _TextColor;
        _NameCredit3.GetComponent<Text>().color = _TextColor;
        _FadeCount--;

        if (_FadeCount == 0)
        {
            _TextStatus = TextStatus.Lp;
        }
        else
        {
            _FadeCount--;
        }
    }

    // 更新処理：表示
    private void UpdateLp()
    {
        // 表示ループ中なので何もしない
    }

    // 更新処理：フェードアウト
    private void UpdateOut()
    {
        Color _TextColor = _PartCredit.GetComponent<Text>().color;
        _TextColor.a = (float)_FadeCount / _TextFadeTime;
        _PartCredit.GetComponent<Text>().color = _TextColor;
        _NameCredit1.GetComponent<Text>().color = _TextColor;
        _NameCredit2.GetComponent<Text>().color = _TextColor;
        _NameCredit3.GetComponent<Text>().color = _TextColor;

        if (_FadeCount == 0)
        {
            _TextStatus = TextStatus.Lp;
        }
        else
        {
            _FadeCount--;
        }
    }

    // 更新処理：タイトルへ遷移
    private void UpdateNext()
    {
        // ※スキップされた場合、シーン遷移はボタン側で行う
        if(_IsSkip == false)
        {
            SceneManager.LoadScene("TitleScene");
        }
    }

    // スキップ指定（SKIPボタン押下時に使用）
    public void SetSkip()
    {
        _IsSkip = true;
    }

}
