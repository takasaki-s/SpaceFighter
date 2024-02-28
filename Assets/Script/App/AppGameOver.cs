using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// ゲーム本編：ゲームオーバー画面
public class AppGameOver : MonoBehaviour
{
    // 列挙
    enum GameOverInfo                       // ゲームオーバー状態
    {
        None = 0,                           // 表示なし
        In,                                 // 画面導入
        Lp,                                 // ループ
        Out,                                // 画面捌け
    }

    enum GameOverType                       // ゲームオーバー遷移先
    {
        None = 0,                           // 未選択
        Retry,                              // リトライ
        Return,                             // タイトルに戻る
    }

    // メンバ変数
    [SerializeField] private AudioSource _SeSelect = null;  // 選択SE

    private GameObject _FadePanel;          // フェードパネル

    private GameOverInfo _GameOverInfo;     // 状態
    private GameOverType _GameOverType;     // 種類

    private Color _FadeColor;               // フェード色制御

    // Start is called before the first frame update
    void Start()
    {
        // 変数初期化
        _FadePanel = this.transform.Find("FadePanel").gameObject;

        _GameOverInfo = GameOverInfo.None;
        _GameOverType = GameOverType.None;

        _FadeColor = _FadePanel.GetComponent<Image>().color;

        // ゲームオブジェクト状態初期化
        _FadePanel.SetActive(false);
        _FadeColor.a = 0.0f;
        _FadePanel.GetComponent<Image>().color = _FadeColor;
    }

    // Update is called once per frame
    void Update()
    {
        // 状態に応じた処理
        if (_GameOverInfo == GameOverInfo.Out)
        {
            if (_FadeColor.a >= 1.0f)
            {
                // 画面遷移
                if (_GameOverType != GameOverType.None)
                {
                    switch (_GameOverType)
                    {
                        case GameOverType.Retry:    // リトライ
                            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                            break;

                        case GameOverType.Return:   // タイトルに戻る
                        default:
                            SceneManager.LoadScene("TitleScene");
                            break;
                    }
                }
            }
            else
            {
                // フェードパネルが徐々に表れる
                _FadeColor.a += 0.01f;
                if (_FadeColor.a >= 1.0f)
                {
                    _FadeColor.a = 1.0f;
                }
                _FadePanel.GetComponent<Image>().color = _FadeColor;
            }
        }
    }

    // 画面有効
    public void SetActive()
    {
        this.gameObject.SetActive(true);
    }

    // 「RETRY」ボタン押下
    public void PushRetry()
    {
        _GameOverType = GameOverType.Retry;
        _GameOverInfo = GameOverInfo.Out;
        _FadePanel.SetActive(true);
        this.transform.Find("ButtonReturn").gameObject.SetActive(false);
        _SeSelect.Play();
    }

    // 「RETURN TO TITLE」ボタン押下
    public void PushReturn()
    {
        _GameOverType = GameOverType.Return;
        _GameOverInfo = GameOverInfo.Out;
        _FadePanel.SetActive(true);
        this.transform.Find("ButtonRetry").gameObject.SetActive(false);
        _SeSelect.Play();
    }
}
