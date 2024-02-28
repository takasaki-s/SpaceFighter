using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// エンディング画面：エピローグ
public class EndingEpilogue : MonoBehaviour
{
    // 列挙
    private enum TextStatus                 // テキスト状態
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

    // 文字データテーブル
    uint _TextDataIndex;
    private readonly TextData[] _TextDataTbl = {
            new TextData( 60 *  0, TextStatus.In,  "Thanks to you", "the Demon King has perished.", "あなたのおかけで魔王は滅びました。"),
            new TextData( 60 *  5, TextStatus.Out, "Thanks to you", "the Demon King has perished.", "あなたのおかけで魔王は滅びました。"),
            new TextData( 60 *  6, TextStatus.In,  "This will bring peace", "to the universe.", "これで宇宙にも平和が訪れるでしょう。"),
            new TextData( 60 * 11, TextStatus.Out, "This will bring peace", "to the universe.", "これで宇宙にも平和が訪れるでしょう。"),
            new TextData( 60 * 12, TextStatus.In,  "You are a true hero.", "", "あなたは真の勇者です。"),
            new TextData( 60 * 17, TextStatus.Out, "You are a true hero.", "", "あなたは真の勇者です。"),
            new TextData( 60 * 18, TextStatus.In,  "Thanks so much.", "", "本当にありがとう。"),
            new TextData( 60 * 24, TextStatus.Out, "Thanks so much.", "", "本当にありがとう。"),
            new TextData( 60 * 25, TextStatus.In,  "Good luck to you", "in the future ...", "あなたの今後に幸あらんことを..."),
            new TextData( 60 * 36, TextStatus.Out, "Good luck to you", "in the future ...", "あなたの今後に幸あらんことを..."),
            new TextData( 60 * 37, TextStatus.Next, "", "", "")        // フェード後スタッフロールへ遷移
    };

    // 関数テーブル
    private delegate void UpdateFunc();
    private UpdateFunc[] _UpdateFuncTbl;

    // メンバ変数
    [SerializeField] private GameObject _StuffRoll = null;  // スタッフロール
    [SerializeField] private AudioSource _EndingBgm = null; // エンディングBGM

    private GameObject _TextA = null;                       // テキストA
    private GameObject _TextB = null;                       // テキストB
    private GameObject _Subtitles = null;                   // 字幕
    private GameObject _FadePanel = null;                   // フェードパネル

    private TextStatus _TextStatus;                         // テキスト状態

    private uint _FrameCount;                               // 経過時間
    private uint _FadeCount;                                // フェード時間

    private const uint _TextFadeTime = 60;                  // テキストフェード設定時間
    private const uint _BlackFadeTime = 60 * 10;            // 黒板フェード設定時間

    private bool _IsBgmFadeOut;                             // BGMフェードアウトする？
    private uint _BgmFadeCount;                             // BGMフェード経過時間
    private const uint _BgmFadeTime = 60 * 1;               // BGMフェード時間設定

    // Start is called before the first frame update
    void Start()
    {
        _TextDataIndex = 0;

        _TextA = this.transform.Find("TextA").gameObject;
        _TextB = this.transform.Find("TextB").gameObject;
        _Subtitles = this.transform.Find("Subtitles").gameObject;
        _FadePanel = this.transform.Find("FadePanel").gameObject;

        _TextStatus = TextStatus.None;

        _FrameCount = 0;

        _UpdateFuncTbl = new UpdateFunc[(int)TextStatus.Max];
        _UpdateFuncTbl[(int)TextStatus.None] = null;
        _UpdateFuncTbl[(int)TextStatus.In] = UpdateIn;
        _UpdateFuncTbl[(int)TextStatus.Lp] = UpdateLp;
        _UpdateFuncTbl[(int)TextStatus.Out] = UpdateOut;
        _UpdateFuncTbl[(int)TextStatus.Next] = UpdateNext;

        _IsBgmFadeOut = false;
        _BgmFadeCount = 0;
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
                _TextA.GetComponent<Text>().text = _TextDataTbl[_TextDataIndex].TextA;
                _TextB.GetComponent<Text>().text = _TextDataTbl[_TextDataIndex].TextB;
                _Subtitles.GetComponent<Text>().text = _TextDataTbl[_TextDataIndex].Subtitles;

                if (_TextStatus == TextStatus.Next)
                {
                    _FadePanel.SetActive(true);
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

        // BGMフェード
        if (_IsBgmFadeOut == true)
        {
            if (_BgmFadeCount <= _BgmFadeTime)
            {
                _EndingBgm.volume = 1.0f - ((float)_BgmFadeCount / _BgmFadeTime);
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

    // 更新処理：フェードイン
    private void UpdateIn()
    {
        Color _TextColor = _TextA.GetComponent<Text>().color;
        _TextColor.a = (float)(_TextFadeTime - _FadeCount) / _TextFadeTime;
        _TextA.GetComponent<Text>().color = _TextColor;
        _TextB.GetComponent<Text>().color = _TextColor;
        _Subtitles.GetComponent<Text>().color = _TextColor;
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
        Color _TextColor = _TextA.GetComponent<Text>().color;
        _TextColor.a = (float)_FadeCount / _TextFadeTime;
        _TextA.GetComponent<Text>().color = _TextColor;
        _TextB.GetComponent<Text>().color = _TextColor;
        _Subtitles.GetComponent<Text>().color = _TextColor;

        if (_FadeCount == 0)
        {
            _TextStatus = TextStatus.Lp;
        }
        else
        {
            _FadeCount--;
        }
    }

    // 更新処理：スタッフロールへ遷移
    private void UpdateNext()
    {
        Color _PanelColor = _FadePanel.GetComponent<Image>().color;
        _PanelColor.a = (float)(_BlackFadeTime -_FadeCount) / _BlackFadeTime;
        _FadePanel.GetComponent<Image>().color = _PanelColor;

        if (_FadeCount == 0)
        {
            // スタッフロールへ
            _StuffRoll.SetActive(true);
        }
        else
        {
            _FadeCount--;
        }
    }

    // BGMフェードアウト指定（SKIPボタン押下時に使用）
    public void SetBgmFadeOut()
    {
        _IsBgmFadeOut = true;
    }

}
