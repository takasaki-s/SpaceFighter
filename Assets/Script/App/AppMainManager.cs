using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// ゲーム本編：アプリケーションマネージャ
public class AppMainManager : MonoBehaviour
{
    // 敵プレハブ
    [SerializeField] private GameObject _PrefabRock_0 = null;       // 岩
    [SerializeField] private GameObject _PrefabRock_1 = null;
    [SerializeField] private GameObject _PrefabRock_2 = null;
    [SerializeField] private GameObject _PrefabRock_3 = null;
    [SerializeField] private GameObject _PrefabRock_4 = null;
    [SerializeField] private GameObject _PrefabRock_5 = null;
    [SerializeField] private GameObject _PrefabRock_6 = null;
    [SerializeField] private GameObject _PrefabRock_7 = null;

    [SerializeField] private GameObject _PrefabEnemy_1 = null;      // 雑魚
    [SerializeField] private GameObject _PrefabEnemy_Boss = null;   // ボス

    // ゲームオブジェクト
    [SerializeField] private GameObject _Background = null;         // 背景
    [SerializeField] private GameObject _BossStage = null;          // ボス背景
    [SerializeField] private GameObject _PlayerMain = null;         // 自機
    [SerializeField] private GameObject _ButtonShot = null;         // 弾発射ボタン
    [SerializeField] private GameObject _ProgressBar = null;        // 進行状況バー
    [SerializeField] private GameObject _ProgressPoint = null;      // 進行状況ポイント
    [SerializeField] private GameObject _GameOver = null;           // ゲームオーバー画面
    [SerializeField] private GameObject _GameClear = null;          // ゲームクリア画面

    // サウンド
    [SerializeField] private AudioSource _BgmApp = null;            // メインBGM
    [SerializeField] private AudioSource _BgmBoss = null;           // ボスBGM
    [SerializeField] private AudioSource _BgmGameOver = null;       // ゲームオーバーBGM

    // メンバ変数
    private uint _ProgressCount;                                    // 進行時間
    private uint _LostCount;                                        // ゲームオーバー待機時間

    private uint _PrefabRockCount;                                  // 岩プレハブ生成数
    private uint _PrefabEnemyCount;                                 // 敵プレハブ生成数

    private Vector3 _ProgressPos;                                   // 進行状況ポイント座標

    private bool _IsBossDown;                                       // ボス撃破フラグ
    private bool _IsClear;                                          // クリアフラグ
    private uint _ClearCount;                                       // クリア進行時間

    // 設定値
    private const uint _BossCount = 60 * 59;                     // ゲーム開始からボス出現までの時間
    private const float _PointMove = 3.0f;                       // 進行状況のポイント座標開始から終了までの総移動量
    private const uint _ClearFadeTime = 60 * 5;                  // クリア時にホワイトアウトするまでの時間
    private const uint _ClearWaitTime = 60 * 3;                  // クリア時にホワイトアウトしてからエピローグに突入するまでの待機時間

    // 進行時間テーブル
    uint _ProgressIndex;
    readonly uint[,] _ProgressTbl = new uint[,]
    {
        // 進行時間(f), 岩の最大数, 敵の最大数, ボスの出現有無
        {       0, 1, 1, 0 },       //  0s
        { 60 * 10, 2, 2, 0 },       // 10s
        { 60 * 20, 3, 3, 0 },       // 20s
        { 60 * 30, 4, 4, 0 },       // 30s
        { 60 * 40, 5, 5, 0 },       // 40s
        { 60 * 50, 6, 6, 0 },       // 50s(ボス背景を表示するのでこの地点から雑魚敵はゼロにする)
        { 60 * 57, 0, 0, 1 },       // 60s(ボス出現)
        { uint.MaxValue, 0, 0, 0 }
    };

    // Start is called before the first frame update
    void Start()
    {
        // 起動直後ではゲームオーバー画面は無効化しておく
        _GameOver.SetActive(false);

        // 1sおきに雑魚敵生成処理を呼び出し
        InvokeRepeating("GenerateRock", 1, 1);
        InvokeRepeating("GenerateEnemy", 1, 1);

        // タイマー値リセット
        Time.timeScale = 1.0f;
        _LostCount = 60 * 3;        // 3s

        // プレハブ生成数
        _PrefabRockCount = 0;
        _PrefabEnemyCount = 0;

        // 進行時間
        if (CmnPlayInfo.NowProgress == CmnProgress.Boss)
        {
            // ボスから
            _ProgressCount = 60 * 50;
        }
        else
        {
            // 最初から
            //_ProgressCount = 60 * 50;
            _ProgressCount = 0;
        }
        _ProgressIndex = 0;
        _ProgressPos = _ProgressPoint.transform.localPosition;

        // その他
        _IsBossDown = false;
        _IsClear = false;
        _ClearCount = 0;

    }

    // Update is called once per frame
    void Update()
    {
        // ゲームクリア？
        if (_IsClear == true)
        {
            if (_ClearCount >= _ClearFadeTime + _ClearWaitTime)
            {
                // エピローグへ
                SceneManager.LoadScene("EndingScene");
            }
            else if (_ClearCount <= _ClearFadeTime)
            {
                // 画面ホワイトアウト
                Color PanelColor = _GameClear.GetComponent<Image>().color;
                PanelColor.a = 1.0f * _ClearCount / _ClearFadeTime;
                _GameClear.GetComponent<Image>().color = PanelColor;
            }
            _ClearCount++;
            return;
        }

        // ゲームオーバーになった？
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        // 自機のHPが0になった？
        if (_PlayerMain.GetComponent<AppPlayerMain>().GetHp() <= 0.0f)
        {
            // BGMが再生されていたら停止
            if (_BgmApp.isPlaying == true)
            {
                _ButtonShot.SetActive(false);
                _BgmApp.Stop();
            }
            else if (_BgmBoss.isPlaying == true)
            {
                _ButtonShot.SetActive(false);
                _BgmBoss.Stop();
            }

            // 自機破壊演出を流した後にゲームオーバー画面表示
            if (_LostCount-- == 0)
            {
                _GameOver.GetComponent<AppGameOver>().SetActive();
                _BgmGameOver.Play();
                Time.timeScale = 0.0f;
            }
        }

        // 進行時間更新
        if (_ProgressCount < uint.MaxValue)
        {
            // 進行状況確認
            if (_ProgressTbl[_ProgressIndex,0] != uint.MaxValue)
            {
                if (_ProgressCount == 60 * 50)
                {
                    // ボスステージ表示開始
                    _BossStage.GetComponent<AppBossStage>().ShowStart();
                    CmnPlayInfo.NowProgress = CmnProgress.Boss;
                }
                else if (_ProgressCount == 60 * 55)
                {
                    // ※プレイヤー操作無効期間に入る。ボスが動き出したら再開する。
                    _ButtonShot.SetActive(false);
                    _PlayerMain.GetComponent<AppPlayerMain>().SetControll(false);

                    // プレイヤーの座標を原点まで戻す
                    _PlayerMain.GetComponent<AppPlayerMain>().SetAutoMove(new Vector3(0.0f, -3.0f, 0.0f), 60 * 3);

                }

                // ボス出現？
                if (
                    _ProgressCount == _ProgressTbl[_ProgressIndex, 0] &&
                    _ProgressTbl[_ProgressIndex, 3] == 1
                    )
                {
                    Instantiate(_PrefabEnemy_Boss, new Vector3(0, 0, 0), Quaternion.identity);
                    _BgmApp.Stop();
                    _BgmBoss.Play();
                    _ProgressBar.SetActive(false);
                    _Background.GetComponent<AppBackground>().SetScroll(false);
                }

                // インデックスを進める？
                if (_ProgressCount >= _ProgressTbl[_ProgressIndex, 0])
                {
                    _ProgressIndex++;
                }
            }

            // 進行状況ポイントの位置を更新
            if (_ProgressCount <= _BossCount)
            {
                // 開始1mでボスに到達
                Vector3 NowProgressPos = _ProgressPos;
                NowProgressPos.y += (_PointMove * _ProgressCount / _BossCount);
                _ProgressPoint.transform.localPosition = NowProgressPos;
            }

            _ProgressCount++;
        }

    }

    // 岩オブジェクト生成
    private void GenerateRock()
    {
        // 生成予定数を上回っていたら生成しない
        if (_PrefabRockCount >= _ProgressTbl[_ProgressIndex, 1])
        {
            return;
        }

        // インスタンス生成
        switch (Random.Range(0, 8))
        {
            case 0:
                Instantiate(_PrefabRock_0, new Vector3(-2.5f + 5 * Random.value, 6, 0), Quaternion.identity);
                break;
            case 1:
                Instantiate(_PrefabRock_1, new Vector3(-2.5f + 5 * Random.value, 6, 0), Quaternion.identity);
                break;
            case 2:
                Instantiate(_PrefabRock_2, new Vector3(-2.5f + 5 * Random.value, 6, 0), Quaternion.identity);
                break;
            case 3:
                Instantiate(_PrefabRock_3, new Vector3(-2.5f + 5 * Random.value, 6, 0), Quaternion.identity);
                break;
            case 4:
                Instantiate(_PrefabRock_4, new Vector3(-2.5f + 5 * Random.value, 6, 0), Quaternion.identity);
                break;
            case 5:
                Instantiate(_PrefabRock_5, new Vector3(-2.5f + 5 * Random.value, 6, 0), Quaternion.identity);
                break;
            case 6:
                Instantiate(_PrefabRock_6, new Vector3(-2.5f + 5 * Random.value, 6, 0), Quaternion.identity);
                break;
            case 7:
                Instantiate(_PrefabRock_7, new Vector3(-2.5f + 5 * Random.value, 6, 0), Quaternion.identity);
                break;
            default:
                break;
        }
    }

    // 敵オブジェクト生成
    private void GenerateEnemy()
    {
        // 生成予定数を上回っていたら生成しない
        if (_PrefabRockCount >= _ProgressTbl[_ProgressIndex, 1])
        {
            return;
        }

        // インスタンス生成
        Instantiate(_PrefabEnemy_1, new Vector3(-2.5f + 5 * Random.value, 6, 0), Quaternion.identity);
    }

    // 敵プレハブ数加算
    public void AddEnemyCount()
    {
        _PrefabEnemyCount++;
    }

    // 敵プレハブ数減算
    public void SubEnemyCount()
    {
        if (_PrefabEnemyCount > 0)
        {
            _PrefabEnemyCount--;
        }
    }

    // 岩プレハブ数加算
    public void AddRockCount()
    {
        _PrefabRockCount++;
    }

    // 岩プレハブ数減算
    public void SubRockCount()
    {
        if (_PrefabRockCount > 0)
        {
            _PrefabRockCount--;
        }
    }

    // ボス撃破時処理
    public void BossDown()
    {
        // BGM終了
        if (_BgmApp.isPlaying == true)
        {
            // メインBGMが再生されていたら停止
            _BgmApp.Stop();
        }
        else if (_BgmBoss.isPlaying == true)
        {
            // ボスBGMが再生されていたら停止
            _BgmBoss.Stop();
        }

        // 各種処理
        _ButtonShot.SetActive(false);
        _IsBossDown = true;
    }

    // エピローグへ
    public void ToEpilogue()
    {
        // ホワイトアウトパネル有効
        if (_GameClear != null)
        {
            Color PanelColor = _GameClear.GetComponent<Image>().color;
            PanelColor.a = 0.0f;
            _GameClear.GetComponent<Image>().color = PanelColor;
            _GameClear.SetActive(true);
        }
        _IsClear = true;
    }

    // ボス活動開始
    public void BossActiveStart()
    {
        _ButtonShot.SetActive(true);
        _PlayerMain.GetComponent<AppPlayerMain>().SetControll(true);
    }

    // ボス撃破したか取得
    public bool GetBossDown()
    {
        return _IsBossDown;
    }

}
