using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// プロローグ画面：メイン
public class PrologueMain : MonoBehaviour
{
    // 列挙
    private enum TextStatus                // テキスト状態
    {
        None = 0,
        In,                                 // フェードイン（徐々に表れる）
        Lp,                                 // 表示
        Out,                                // フェードアウト（徐々に消える）
        Skip,                               // スキップ
        Next,                               // ゲーム本編へ遷移
        Max
    }

    // 文字データフォーマット
    private struct TextData
    {
        // メンバ変数
        public uint StartFrame { get; }
        public TextStatus TextStatus { get; }
        public string TextA { get; }
        public string TextB { get; }
        public string Subtitles { get; }

        // 初期化
        public TextData(uint StartFrame, TextStatus TextStatus, string TextA, string TextB, string Subtitles)
        {
            this.StartFrame = StartFrame;
            this.TextStatus = TextStatus;
            this.TextA = TextA;
            this.TextB = TextB;
            this.Subtitles = Subtitles;
        }
    }

    // 文字データテーブル（通常時）
    uint _TextDataIndex;
    private readonly TextData[] _TextDataTbl = {
            new TextData( 60 *  0, TextStatus.In,  "The universe is facing", "an unprecedented crisis.", "宇宙はかつてない危機に直面しています。"),
            new TextData( 60 *  5, TextStatus.Out, "The universe is facing", "an unprecedented crisis.", "宇宙はかつてない危機に直面しています。"),

            new TextData( 60 *  6, TextStatus.In,  "The Demon King sealed", "in M31 was revived.", "M31に封印された魔王が蘇ったのです。"),
            new TextData( 60 * 11, TextStatus.Out, "The Demon King sealed", "in M31 was revived.", "M31に封印された魔王が蘇ったのです。"),

            new TextData( 60 * 12, TextStatus.In,  "The universe will be", "dominated as it is", "このままでは宇宙は支配され"),
            new TextData( 60 * 17, TextStatus.Out, "The universe will be", "dominated as it is", "このままでは宇宙は支配され"),

            new TextData( 60 * 18, TextStatus.In,  "All life is dead. ", "", "全ての生命は息絶えます。"),
            new TextData( 60 * 23, TextStatus.Out, "All life is dead. ", "", "全ての生命は息絶えます。"),

            new TextData( 60 * 24, TextStatus.In,  "Please.", "", "どうかお願いです。"),
            new TextData( 60 * 29, TextStatus.Out, "Please.", "", "どうかお願いです。"),

            new TextData( 60 * 30, TextStatus.In,  "Defeat the Demon King ...", "", "魔王を倒してください..."),
            new TextData( 60 * 35, TextStatus.Out, "Defeat the Demon King ...", "", "魔王を倒してください..."),

            new TextData( 60 * 36, TextStatus.Next, "", "", "")        // フェード後ゲーム本編へ遷移
    };

    // 文字データテーブル（スキップ時）
    private readonly TextData[] _SkipDataTbl = {
            new TextData( 60 * 0, TextStatus.Skip, "", "", ""),
            new TextData( 60 * 1, TextStatus.Next, "", "", "")        // フェード後ゲーム本編へ遷移
    };

    // 関数テーブル
    private delegate void UpdateFunc();
    private UpdateFunc[] _UpdateFuncTbl;

    // メンバ変数
    [SerializeField] private AudioSource _PrologueBgm = null;       // プロローグ
    [SerializeField] private AudioSource _ButtonEnterSe = null;     // ボタン押下SE

    private GameObject _TextA = null;                       // テキストA
    private GameObject _TextB = null;                       // テキストB
    private GameObject _Subtitles = null;                   // 字幕
    private GameObject _SkipButton = null;                  // SKIPボタン

    private TextStatus _TextStatus;                         // テキスト状態

    private uint _FrameCount;                               // 経過時間
    private uint _FadeCount;                                // フェード時間

    private const uint _TextFadeTime = 60;                  // テキストフェード設定時間
    private const uint _BlackFadeTime = 60 * 10;            // 黒板フェード設定時間

    private bool _IsBgmFadeOut;                             // BGMフェードアウトする？
    private uint _BgmFadeCount;                             // BGMフェード経過時間
    private const uint _BgmFadeTime = 60 * 1;               // BGMフェード時間設定

    private bool _IsSkip;                                   // スキップする？

    // Start is called before the first frame update
    void Start()
    {
        _TextDataIndex = 0;

        _TextA = this.transform.Find("TextA").gameObject;
        _TextB = this.transform.Find("TextB").gameObject;
        _Subtitles = this.transform.Find("Subtitles").gameObject;
        _SkipButton = this.transform.Find("SkipButton").gameObject;

        _TextStatus = TextStatus.None;

        _FrameCount = 0;

        _UpdateFuncTbl = new UpdateFunc[(int)TextStatus.Max];
        _UpdateFuncTbl[(int)TextStatus.None] = null;
        _UpdateFuncTbl[(int)TextStatus.In] = UpdateIn;
        _UpdateFuncTbl[(int)TextStatus.Lp] = UpdateLp;
        _UpdateFuncTbl[(int)TextStatus.Out] = UpdateOut;
        _UpdateFuncTbl[(int)TextStatus.Skip] = UpdateSkip;
        _UpdateFuncTbl[(int)TextStatus.Next] = UpdateNext;

        _IsBgmFadeOut = false;
        _BgmFadeCount = 0;

        _IsSkip = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_TextStatus != TextStatus.Next)
        {
            // テキスト状態更新フレームになった？
            if(_IsSkip == true)
            {
                // スキップ状態
                if(_FrameCount == _SkipDataTbl[_TextDataIndex].StartFrame)
                {
                    // ※テキスト情報はそのまま引き継ぐ
                    _TextStatus = _SkipDataTbl[_TextDataIndex].TextStatus;
                    _TextDataIndex++;
                }

            }
            else if (_FrameCount == _TextDataTbl[_TextDataIndex].StartFrame)
            {
                // 通常状態
                _TextStatus = _TextDataTbl[_TextDataIndex].TextStatus;
                _TextA.GetComponent<Text>().text = _TextDataTbl[_TextDataIndex].TextA;
                _TextB.GetComponent<Text>().text = _TextDataTbl[_TextDataIndex].TextB;
                _Subtitles.GetComponent<Text>().text = _TextDataTbl[_TextDataIndex].Subtitles;

                // 状態遷移に応じた処理
                switch(_TextStatus)
                {
                    case TextStatus.In:
                        _FadeCount = 0;
                        break;

                    case TextStatus.Out:
                        _FadeCount = _TextFadeTime;
                        break;

                    case TextStatus.Next:
                        _FadeCount = _TextFadeTime;
                        _IsBgmFadeOut = true;
                        break;

                    default:
                        break;
                }

                _TextDataIndex++;
            }
        }

        // 更新関数実行
        if (_TextStatus != TextStatus.None)
        {
            _UpdateFuncTbl[(int)_TextStatus]();
        }

        // BGMフェード
        if(_IsBgmFadeOut == true)
        {
            if(_BgmFadeCount <= _BgmFadeTime)
            {
                _PrologueBgm.volume = 1.0f - ((float)_BgmFadeCount / _BgmFadeTime);
                _BgmFadeCount++;
            }
            else
            {
                _IsBgmFadeOut = false;
            }
        }

        // 時間値更新
        if (_FrameCount < uint.MaxValue)
        {
            _FrameCount++;
        }
    }

    // 更新処理：テキストにアルファ値適用
    private void UpdateTextAlpha()
    {
        Color _TextColor = _TextA.GetComponent<Text>().color;
        _TextColor.a = (float)_FadeCount / _TextFadeTime;
        _TextA.GetComponent<Text>().color = _TextColor;
        _TextB.GetComponent<Text>().color = _TextColor;
        _Subtitles.GetComponent<Text>().color = _TextColor;
    }

    // 更新処理：フェードイン
    private void UpdateIn()
    {
        UpdateTextAlpha();

        if (_FadeCount == _TextFadeTime)
        {
            _TextStatus = TextStatus.Lp;
        }
        else
        {
            _FadeCount++;
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
        UpdateTextAlpha();

        if (_FadeCount == 0)
        {
            _TextStatus = TextStatus.Lp;
        }
        else
        {
            _FadeCount--;
        }
    }

    // 更新処理：スキップ
    private void UpdateSkip()
    {
        UpdateOut();    // 現状はフェードアウトの使いまわしでOK
    }

    // 更新処理：待機時間経過後ゲーム本編へ遷移
    private void UpdateNext()
    {
        if (_FadeCount == 0)
        {
            SceneManager.LoadScene("AppScene");
        }
        else
        {
            _FadeCount--;
        }
    }

    // 「SKIP」ボタン押下時
    public void SkipButtonEnter()
    {
        // ※状態がNextまで行ってたらUpdateはそのまま流す
        if(_TextStatus != TextStatus.Next)
        {
            _IsSkip = true;
            _IsBgmFadeOut = true;

            _FrameCount = 0;
            _TextDataIndex = 0;
        }

        _ButtonEnterSe.Play();
        _SkipButton.SetActive(false);
    }

}
