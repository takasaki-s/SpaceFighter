using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム本編：ボス制御
public class AppEnemyBoss : MonoBehaviour
{
    // 列挙
    enum Action                                 // 動作
    {
        None = 0,                               // 表示なし
        In,                                     // 導入
        Wait,                                   // 待機
        Move,                                   // 移動
        Beam,                                   // ビーム
        Dead,                                   // 撃破
        Max
    }

    enum MoveDirection                          // 移動方向
    {
        Left = 0,                               // 左
        Right,                                  // 右
        Max
    }

    // 弾発射制御
    private struct BulletInfo
    {
        public bool Enable;                     // 弾発射動作有効か
        public uint WaitCount;                  // 発射待機フレーム数
        public uint ShotCount;                  // 発射フレーム数
    }

    // メンバ変数
    [SerializeField] private GameObject _PrefabBullet = null;           // 小ビーム
    [SerializeField] private GameObject _PrefabCharge = null;           // チャージエフェクトプレハブ
    [SerializeField] private GameObject _PrefabExplosion = null;        // 爆発エフェクトプレハブ
    [SerializeField] private AudioSource _VoiceIn = null;               // インボイス
    [SerializeField] private AudioSource _SeBullet = null;              // 弾発射SE
    [SerializeField] private AudioSource _SeCharge = null;              // チャージ中SE
    [SerializeField] private AudioSource _SeChargeShot = null;          // チャージ発射SE

    private GameObject _AppPlayer = null;       // プレイヤー
    private GameObject _AppMain = null;         // メイン
    private GameObject _ChargeBullet = null;    // チャージ弾

    private Action _Action;                     // 動作
    private uint _ActionCount;                  // 動作カウント
    private Vector3 _Position;                  // 現座標
    private MoveDirection _MoveDirection;       // 移動方向
    private Vector3 _ChargeScal;                // チャージ弾の最大拡大率

    private BulletInfo _BulletInfo;             // 弾発射制御

    // 設定値
    private readonly Vector3 _StartPos = new Vector3(0f, 6f, 0f);       // 開始座標
    private readonly Vector3 _BasePos = new Vector3(0f, 3.5f, 0f);      // 基本座標

    private const uint _InCount = 60 * 5;                               // 開始～待機までの移動時間
    private const uint _WaitCount = 60 * 3;                             // 待機～移動までの時間

    private const uint _ChargeCount = 60 * 3;                           // チャージ弾溜め時間

    private const float _MoveLeft = -2.0f;                              // 左端最大座標（x軸）
    private const float _MoveRight = 2.0f;                              // 右端最大座標（x軸）

    // 関数テーブル
    private delegate void UpdateFunc();
    private UpdateFunc[] _UpdateFuncTbl;

    // Start is called before the first frame update
    void Start()
    {
        // メンバ変数
        _AppPlayer = GameObject.FindGameObjectWithTag("Player");
        _AppMain = GameObject.FindGameObjectWithTag("AppMain");

        _Action = Action.In;
        _ActionCount = 0;

        this.transform.localPosition = _Position = _StartPos;

        // 関数テーブル
        _UpdateFuncTbl = new UpdateFunc[(int)Action.Max];
        _UpdateFuncTbl[(int)Action.None] = null;
        _UpdateFuncTbl[(int)Action.In] = UpdateIn;
        _UpdateFuncTbl[(int)Action.Wait] = UpdateWait;
        _UpdateFuncTbl[(int)Action.Move] = UpdateMove;
        _UpdateFuncTbl[(int)Action.Beam] = UpdateBeam;
        _UpdateFuncTbl[(int)Action.Dead] = UpdateDead;

        // ステータス設定
        this.GetComponent<AppEnemyStatus>().SetHp(50);
    }

    // Update is called once per frame
    void Update()
    {
        // ゲームオーバーになった？
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            // サウンド鳴ってたら終了
            StopAudioAll();
            return;
        }

        // HPが0になった？
        if (_Action != Action.Dead && this.GetComponent<AppEnemyStatus>().GetHp() == 0)
        {
            _Action = Action.Dead;
            _ActionCount = 0;

            // サウンド鳴ってたら終了
            StopAudioAll();
        }

        // 更新関数実行
        if (_Action != Action.None)
        {
            _UpdateFuncTbl[(int)_Action]();
        }
    }

    // 更新処理：画面導入
    private void UpdateIn()
    {
        // 「開始座標」→「基本座標」まで移動
        float MovingDistance = _BasePos.y - _StartPos.y;
        _Position.y = _StartPos.y + (MovingDistance * _ActionCount / _InCount);
        this.transform.localPosition = _Position;

        if (_ActionCount >= _InCount)
        {
            _VoiceIn.Play();
            _Action = Action.Wait;
            _ActionCount = 0;
        }
        else
        {
            _ActionCount++;
        }
    }

    // 更新処理：待機
    private void UpdateWait()
    {
        if (_ActionCount >= _WaitCount)
        {
            // ボスの活動開始
            _Action = Action.Move;
            _ActionCount = 0;
            _AppMain.GetComponent<AppMainManager>().BossActiveStart();
        }
        else
        {
            _ActionCount++;
        }
    }

    // 更新処理：移動
    private void UpdateMove()
    {
        if (_MoveDirection == MoveDirection.Left)
        {
            // 左移動
            if (_Position.x > _BasePos.x + _MoveLeft)
            {
                _Position.x -= 0.025f;
            }
            else
            {
                _MoveDirection = MoveDirection.Right;
            }
        }
        else
        {
            // 右移動
            if (_Position.x < _BasePos.x + _MoveRight)
            {
                _Position.x += 0.025f;
            }
            else
            {
                _MoveDirection = MoveDirection.Left;
            }
        }
        this.transform.localPosition = _Position;

        // 小ビーム発射
        if(_BulletInfo.Enable == true)
        {
            if(_BulletInfo.ShotCount-- % 30 == 0)
            {
                Vector3 StartPos = this.transform.position;
                StartPos.y -= 0.5f;
                Instantiate(_PrefabBullet, StartPos, Quaternion.identity);
                _SeBullet.Play();
            }

            if(_BulletInfo.ShotCount == 0)
            {
                _BulletInfo.Enable = false;
                _BulletInfo.WaitCount = 60 * (uint)Random.Range(0, 3);
            }
        }
        else
        {
            if(_BulletInfo.WaitCount-- == 0)
            {
                _BulletInfo.ShotCount = 60 * 3;
                _BulletInfo.Enable = true;
            }
        }

        // 一定時間経過で大ビーム発射
        if (_ActionCount >= 60 * 5)
        {
            // 小ビーム発射中はやらない
            if(_BulletInfo.Enable == false)
            {
                _ActionCount = 0;
                _Action = Action.Beam;
            }
        }
        else
        {
            _ActionCount++;
        }
    }

    // 更新処理：ビーム
    private void UpdateBeam()
    {
        if (_ActionCount == 0)
        {
            // チャージエフェクト再生
            _ChargeBullet = Instantiate(_PrefabCharge, this.transform.position, Quaternion.identity);
            _ChargeScal = _ChargeBullet.GetComponent<AppCharge>().transform.localScale;
            _SeCharge.Play();
            _ActionCount++;
        }
        else if (_ActionCount >= _ChargeCount)
        {
            // チャージエフェクト発射
            _ChargeBullet.GetComponent<AppCharge>().SetMoveActive(this.transform.position, _AppPlayer.transform.position);
            _SeCharge.Stop();
            _SeChargeShot.Play();
            _ActionCount = 0;
            _Action = Action.Move;
        }
        else
        {
            // チャージエフェクト溜め
            _ActionCount++;

        }

        // チャージエフェクトを徐々に大きく
        if(_Action == Action.Beam && _ActionCount <= _ChargeCount)
        {
            // 徐々にエフェクトを大きく
            Vector3 NowScal = _ChargeBullet.GetComponent<AppCharge>().transform.localScale;
            NowScal.x = _ChargeScal.x * (float)_ActionCount / _ChargeCount;
            NowScal.y = _ChargeScal.y * (float)_ActionCount / _ChargeCount;
            _ChargeBullet.GetComponent<AppCharge>().transform.localScale = NowScal;
        }
    }

    // 更新処理：撃破
    private void UpdateDead()
    {
        if (_ActionCount == 0)
        {
            // チャージエフェクトが残ってたら消す
            if (_ChargeBullet != null)
            {
                Destroy(_ChargeBullet);
            }

            // 撃破処理
            if (_AppMain != null)
            {
                _AppMain.GetComponent<AppMainManager>().BossDown();
            }

            // 断末魔
            _VoiceIn.Play();

        }

        // 崩壊演出
        if (_ActionCount >= 60 * 3 && _ActionCount % 15 == 0)
        {
            // ボス画像を徐々に透明
            Color SetColor = this.GetComponent<SpriteRenderer>().color;
            SetColor.a -= 0.05f;
            this.GetComponent<SpriteRenderer>().color = SetColor;

            // 崩壊エフェクト
            Vector3 SetPos = this.transform.position;
            SetPos.x += Random.Range(-1.0f, 1.0f);
            SetPos.y += Random.Range(-1.0f, 1.0f);
            Instantiate(_PrefabExplosion, SetPos, Quaternion.identity);

            // SE再生
            GameObject SoundManager = GameObject.FindWithTag("SoundManager");
            if (SoundManager != null)
            {
                SoundManager.GetComponent<AppSoundManager>().SePlay("");
            }
        }

        // エピローグへ
        if (_ActionCount >= 60 * 8)
        {
            _AppMain.GetComponent<AppMainManager>().ToEpilogue();
            Destroy(this.gameObject);
        }
        else
        {
            _ActionCount++;
        }

    }

    // サウンド停止
    private void StopAudio(ref AudioSource Audio)
    {
        if (Audio.isPlaying == true)
        {
            Audio.Stop();
        }
    }

    // 全サウンド停止
    private void StopAudioAll()
    {
        StopAudio(ref _VoiceIn);
        StopAudio(ref _SeCharge);
        StopAudio(ref _SeChargeShot);
    }

}
