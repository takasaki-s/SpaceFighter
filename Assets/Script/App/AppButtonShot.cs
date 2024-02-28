using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム本編：「発射」ボタン押下時処理
public class AppButtonShot : MonoBehaviour
{
    private GameObject _PlayerMain;
    int _count = 0;

    // Start is called before the first frame update
    void Start()
    {
        _PlayerMain = GameObject.Find("PlayerMain");
    }

    // ボタン押された
    public void ButtonPush()
    {
        // 弾発射
        _PlayerMain.GetComponent<AppPlayerMain>().BulletShot();

        // デバッグ表示テスト
        _count++;
        Debug.Log("Pushed Count = " + _count.ToString());
    }
}
