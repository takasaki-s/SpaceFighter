using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;

// ゲーム本編：自機メイン処理
public class AppPlayerMain : MonoBehaviour
{
    // メンバ変数
    [SerializeField] private GameObject _Bullet = null;         // 弾のPrefabを指定
    [SerializeField] private GameObject _DamageEffect = null;   // ダメージエフェクトのパーティクルを指定
    [SerializeField] private GameObject _DamagePanel = null;    // ダメージパネルのオブジェクトを指定
    [SerializeField] private GameObject _HpBarSlider = null;    // HPバーのオブジェクトを指定
    [SerializeField] private AudioSource _AudioShot = null;     // 発射音SEを指定

    private GameObject _AppMain = null;                         // ゲーム本編アプリケーションマネージャ

    private SpriteRenderer _ImageComponent;                     // Sprite描画関連
    private Sprite _FighterSprite_C;
    private Sprite _FighterSprite_L;
    private Sprite _FighterSprite_R;

    private float _Hp;                                          // 耐久値

    private float _MoveSpeed;                                   // 移動速度
    private Vector3 _TouchPos;                                  // タッチ座標

    private bool _IsControll;                                   // 操作可能状態か

    private bool _IsAutoMove;                                   // 自動移動状態か
    private Vector3 _StartPos;                                  // 移動開始座標
    private Vector3 _MoveVec;                                   // 目標移動ベクトル
    private uint _MoveCount;                                    // 現在の移動時間
    private uint _MoveTime;                                     // 総移動時間

    // 設定値
    private const float _PosLimitMax_X = 2.8f;                  // x座標 最大値
    private const float _PosLimitMin_X = -2.8f;                 // x座標 最小値

    private const float _PosLimitMax_Y = 5.0f;                  // y座標 最大値
    private const float _PosLimitMin_Y = -5.0f;                 // y座標 最小値


    // Start is called before the first frame update
    void Start()
    {
        // 自機表示用コンポーネント
        _ImageComponent = this.GetComponent<SpriteRenderer>();

        // 自機のImageを取得（スライスしたSpriteは取得が少し厄介）
        Sprite[] sprites = Resources.LoadAll<Sprite>("space_glider");
        _FighterSprite_C = System.Array.Find<Sprite>(sprites, (sprite) => sprite.name.Equals("space_glider_0"));
        _FighterSprite_L = System.Array.Find<Sprite>(sprites, (sprite) => sprite.name.Equals("space_glider_1"));
        _FighterSprite_R = System.Array.Find<Sprite>(sprites, (sprite) => sprite.name.Equals("space_glider_2"));

        // 各種初期化
        if (Application.isEditor)
        {
            // Unityエディタ
            _MoveSpeed = 0.1f;
        }
        else
        {
            // 端末
            _MoveSpeed = 0.2f;
        }

        _Hp = 100.0f;
        //_Hp = 50.0f;    // デバッグ用

        _AppMain = GameObject.FindWithTag("AppMain");

        _IsControll = true;
        _IsAutoMove = false;
        _MoveCount = 0;
        _MoveTime = 0;

    }

    // Update is called once per frame
    void Update()
    {
        // ゲームオーバーになった？、ボス倒された？、操作可能状態？
        if (Mathf.Approximately(_Hp, 0f) || _AppMain.GetComponent<AppMainManager>().GetBossDown() == true)
        {
            return;
        }

        // 動作環境に応じた処理の分岐
        if(_IsAutoMove == true)
        {
            // 自動移動状態
            this.AutoMove();
        }
        else if (_IsControll == false)
        {
            // 操作不可状態中
            return;
        }
        else if (Application.isEditor)
        {
            // ☆Unityエディタでの動作☆

            // ボタンボタン押下時は自機移動を処理しない。
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // 自機移動
            if (Input.GetKey(KeyCode.UpArrow))
            {
                this.Transform_Up();
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                this.Transform_Down();
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                this.Transform_L();
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                this.Transform_R();
            }
            else
            {
                this.Transform_None();
            }

            // 弾発射
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.BulletShot();
            }
        }
        else
        {
            // ☆端末での動作☆

            // ボタン押下時はタッチインデックスを切り替え
            int TouchIndex = 0;
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                TouchIndex = 1;
            }

            if (Input.touchCount > 0)
            {
                TouchPhase NowTouchPhase = Input.GetTouch(TouchIndex).phase;
                if (NowTouchPhase != TouchPhase.Ended)
                {
                    // タッチ座標取得
                    Vector3 NowTouchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(TouchIndex).position);
                    NowTouchPos.z = 0.0f;

                    // 指の移動量に応じて自機移動
                    if (NowTouchPhase == TouchPhase.Began)
                    {
                        // タッチ開始
                        _TouchPos = NowTouchPos;
                    }
                    else if (NowTouchPhase == TouchPhase.Moved)
                    {
                        // タッチ移動
                        Vector3 MovePos = NowTouchPos - _TouchPos;
                        _TouchPos = NowTouchPos;
                        this.transform.Translate(MovePos);
                    }
                }
            }
            else
            {
                this.Transform_None();
            }
        }

        // 自機座標限界値チェック
        CheckPositionLimit();

    }

    // 自機：上移動
    private void Transform_Up()
    {
        if (this.transform.position.y < _PosLimitMax_Y)
        {
            this.transform.Translate(0, _MoveSpeed, 0);
        }
    }

    // 自機：下移動
    private void Transform_Down()
    {
        if (this.transform.position.y > _PosLimitMin_Y)
        {
            this.transform.Translate(0, -_MoveSpeed, 0);
        }
    }

    // 自機：左移動
    private void Transform_L()
    {
        if (this.transform.position.x > _PosLimitMin_X)
        {
            this.transform.Translate(-_MoveSpeed, 0, 0);
        }
        //_ImageComponent.sprite = _FighterSprite_L;
    }

    // 自機：右移動
    private void Transform_R()
    {
        if (this.transform.position.x < _PosLimitMax_X)
        {
            this.transform.Translate(_MoveSpeed, 0, 0);
        }
        //_ImageComponent.sprite = _FighterSprite_R;
    }

    // 自機：移動なし
    private void Transform_None()
    {
        //_ImageComponent.sprite = _FighterSprite_C;
    }

    // 自機：座標限界値チェック
    // ※予期せぬ値を取得して画面外へ出ないように移動処理とは独立させる。
    private void CheckPositionLimit()
    {
        Vector3 NowPos = this.transform.position;

        // x座標
        if (NowPos.x > _PosLimitMax_X)
        {
            NowPos.x = _PosLimitMax_X;
        }
        else if (NowPos.x < _PosLimitMin_X)
        {
            NowPos.x = _PosLimitMin_X;
        }

        // y座標
        if (NowPos.y > _PosLimitMax_Y)
        {
            NowPos.y = _PosLimitMax_Y;
        }
        else if (NowPos.y < _PosLimitMin_Y)
        {
            NowPos.y = _PosLimitMin_Y;
        }

        this.transform.position = NowPos;
    }

    // 自機：弾発射
    public void BulletShot()
    {
        if (Mathf.Approximately(_Hp, 0f))
        {
            return;
        }

        // 弾インスタンス生成
        Instantiate(_Bullet, this.transform.position, Quaternion.identity);

        // 弾発射SE
        _AudioShot.Play();
    }

    // 自機：HP取得
    public float GetHp()
    {
        return _Hp;
    }

    // オブジェクトが衝突した
    void OnCollisionEnter2D(Collision2D collision)
    {
        // HPが0になってたら処理しない
        if (Mathf.Approximately(_Hp, 0f))
        {
            return;
        }

        string CollisionTag = collision.gameObject.tag;

        if (CollisionTag == "Player" || CollisionTag == "Other")
        {
            // プレイヤーまたはOtherオブジェクトに当たった（無視する）
            return;
        }
        else if (CollisionTag == "Enemy" || CollisionTag == "Boss")
        {
            // 敵に当たった

            // 雑魚敵だけの処理
            if (CollisionTag == "Enemy")
            {
                // オブジェクト破棄
                Destroy(collision.gameObject);
            }

            // ダメージ処理
            float Damage = collision.gameObject.GetComponent<AppEnemyStatus>().GetDamage();
            if (_Hp > Damage)
            {
                // HP減算
                _Hp -= Damage;
                _DamagePanel.GetComponent<AppDamagePanel>().OnDamage();

                // 衝突エフェクト生成（雑魚敵だけ）
                if (CollisionTag == "Enemy")
                {
                    GameObject DamageEffect = Instantiate(_DamageEffect, collision.transform.position, Quaternion.identity);
                }
            }
            else
            {
                // HPがゼロになった
                _Hp = 0.0f;
                _DamagePanel.GetComponent<AppDamagePanel>().OnLost();
                this.GetComponent<SpriteRenderer>().enabled = false;

                // 衝突エフェクト生成
                GameObject DamageEffect = Instantiate(_DamageEffect, collision.transform.position, Quaternion.identity);
            }
            _HpBarSlider.GetComponent<AppHpBar>().SetValue(_Hp);
        }
    }

    // 操作状態設定
    // IsControll = true:操作可能、IsControll = false:操作不可
    public void SetControll(bool IsControll)
    {
        _IsControll = IsControll;
    }

    // 自動移動状態設定
    // TargetPos:移動目標座標、MoveTime:移動時間（フレーム指定）
    public void SetAutoMove(Vector3 TargetPos, uint MoveTime)
    {
        // 目標地点に向けてのベクトル作成
        _MoveVec = TargetPos - this.transform.position;

        _MoveCount = 0;
        _MoveTime = MoveTime;

        _StartPos = this.transform.position;
        //_MoveVec.Normalize();
        //_MoveVec *= 0.1f;

        _IsAutoMove = true;
    }

    // 自動移動処理
    private void AutoMove()
    {
        // 毎フレーム加算だと誤差が蓄積するので比率計算にする
        this.transform.position = _StartPos + (_MoveVec * ((float)_MoveCount / _MoveTime));

        if(_MoveCount < _MoveTime)
        {
            _MoveCount++;
        }
        else
        {
            _IsAutoMove = false;
        }
    }
}
