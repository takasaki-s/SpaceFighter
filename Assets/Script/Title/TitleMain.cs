using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// タイトル画面：メイン制御
public class TitleMain : MonoBehaviour
{
    // メンバ変数
    [SerializeField] private GameObject _FadePanel = null;      // 画面フェード用パネル
    [SerializeField] private GameObject _Description = null;    // 説明文
    [SerializeField] private AudioSource _SeSelect = null;      // 画面タップSE

    private bool IsTap;                                         // 画面がタップされたか

    // Start is called before the first frame update
    void Start()
    {
        CmnPlayInfo.NowProgress = CmnProgress.Start;
        IsTap = false;
    }

    // Update is called once per frame
    void Update()
    {
        // マウス押下 or タップ
        if (Input.GetMouseButtonDown(0))
        {
            if (_FadePanel.GetComponent<TitleScreenFade>().IsFadeIn() == true)
            {
                // 画面がフェードイン中の場合はキャンセル
                _FadePanel.GetComponent<TitleScreenFade>().FadeInCancel();
            }
            else if (_FadePanel.GetComponent<TitleScreenFade>().IsFadeOut() == false)
            {
                // 画面タップでフェードアウト
                if(IsTap == false)
                {
                    _FadePanel.GetComponent<TitleScreenFade>().SetFadeOut();
                    _Description.SetActive(false);
                    _SeSelect.PlayOneShot(_SeSelect.clip);
                    IsTap = true;
                }
            }
        }

        // フェードアウト終了でシーン遷移
        if (_FadePanel.GetComponent<TitleScreenFade>().IsFadeOutEnd() == true)
        {
            SceneManager.LoadScene("PrologueScene");    // プロローグへ
        }
    }
}
