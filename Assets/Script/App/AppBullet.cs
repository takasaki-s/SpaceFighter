using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム本編：弾制御
public class AppBullet : MonoBehaviour
{
    // メンバ変数
    [SerializeField] private GameObject _Explosion = null;      // 撃破エフェクトのPrefabを指定
    [SerializeField] private GameObject _Smoke = null;          // ヒットエフェクトのPrefabを指定
    private GameObject _AppMain = null;

    // Start is called before the first frame update
    void Start()
    {
        _AppMain = GameObject.FindWithTag("AppMain");
    }

    // Update is called once per frame
    void Update()
    {
        // ボスを倒してたら破棄する
        if (_AppMain.GetComponent<AppMainManager>().GetBossDown() == true)
        {
            Destroy(this.gameObject);
        }

        // ゲームオーバーになった？
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            return;
        }

        // 弾移動
        transform.Translate(0, 0.2f, 0);

        // 画面外に出たらオブジェクト破棄
        if (transform.position.y > 5)
        {
            Destroy(this.gameObject);
        }
    }

    // オブジェクトが衝突した
    void OnCollisionEnter2D(Collision2D collision)
    {
        string CollisionTag = collision.gameObject.tag;

        if (CollisionTag == "Player")
        {
            // プレイヤーに当たった（無視する）
            return;
        }

        if (CollisionTag == "Enemy")
        {
            // 雑魚敵に当たった
            if (collision.gameObject.GetComponent<AppEnemyStatus>().Damage(transform.position, 1) == true)
            {
                // HPが0になった

                // 撃破エフェクト生成
                Instantiate(_Explosion, transform.position, Quaternion.identity);

                // スコア加算
                GameObject Score = GameObject.FindWithTag("Score");
                if (Score != null)
                {
                    Score.GetComponent<AppScore>().AddScore(collision.gameObject.GetComponent<AppEnemyStatus>().GetScore());
                }

                // SE再生
                GameObject SoundManager = GameObject.FindWithTag("SoundManager");
                if (SoundManager != null)
                {
                    SoundManager.GetComponent<AppSoundManager>().SePlay("");
                }

                // 破棄のタイミングは敵オブジェクト側に委ねる
                //Destroy(collision.gameObject);
            }
            else
            {
                // HPが残っている
                foreach (ContactPoint2D point in collision.contacts)
                {
                    // 衝突した場所にエフェクト生成
                    GameObject SmokeObject = Instantiate(_Smoke, point.point, Quaternion.identity);
                    //SmokeObject.transform.parent = collision.gameObject.transform;
                    //SmokeObject.transform.localPosition = this.transform.InverseTransformPoint(point.point);
                }
            }
        }
        else if (CollisionTag == "Boss")
        {
            // ボスに当たった
            if (collision.gameObject.GetComponent<AppEnemyStatus>().Damage(transform.position, 1) == false)
            {
                // HPが残っている
                foreach (ContactPoint2D point in collision.contacts)
                {
                    // 衝突した場所にエフェクト生成
                    GameObject SmokeObject = Instantiate(_Smoke, point.point, Quaternion.identity);
                }
            }

            // ※HPが0になったときの挙動はボスオブジェクトに委ねる
        }

        // 弾を破棄
        Destroy(this.gameObject);
    }
}
