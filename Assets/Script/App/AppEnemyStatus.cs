using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム本編：敵ステータス管理
// ※本クラスは衝突判定に共通で使用します。
// https://teratail.com/questions/208741
public class AppEnemyStatus : MonoBehaviour
{
    // メンバ変数
    private int _Hp;        // HP
    private int _Score;     // 倒したときに取得できるスコア
    private float _Damage;  // 衝突したときのダメージ

    // インスタンス生成時
    private void Awake()
    {

        _Hp = 1;
        _Score = 50;
        _Damage = 10.0f;    // デフォルトでは10回ヒットでゲームオーバー
    }

    // Start is called before the first frame update
    void Start()
    {
        // 他コンポーネントからセットした値を上書きしてしまう可能性があるので初期化は「Awake」で行うこと
    }

    // 残りHP設定
    public void SetHp(int Hp)
    {
        _Hp = Hp;
    }

    // 残りHP取得
    public int GetHp()
    {
        return _Hp;
    }

    // スコア設定
    public void SetScore(int Score)
    {
        _Score = Score;
    }

    // スコア取得
    public int GetScore()
    {
        return _Score;
    }

    // ダメージ設定
    public void SetDamage(float Damage)
    {
        _Damage = Damage;
    }

    // ダメージ取得
    public float GetDamage()
    {
        return _Damage;
    }

    // 衝突判定によるダメージ処理
    // Damage：ダメージ値、Position：対象との衝突座標
    // 戻り値：true（HPが0になった）、false（HPが残っている）
    public bool Damage(Vector3 Position, int Damage)
    {
        if (Damage >= _Hp)
        {
            _Hp = 0;
            return true;
        }
        _Hp -= Damage;
        return false;
    }

}
