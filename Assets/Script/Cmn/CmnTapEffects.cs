using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 共通：画面タップ時パーティクルエフェクト
public class CmnTapEffects : MonoBehaviour
{
    // インスペクターウィンドウで編集可能な項目
    [SerializeField] GameObject CLICK_PARTICLE = default; // PS_TouchStarを割り当てること
    [SerializeField] GameObject DRAG_PARTICLE = default;  // PS_DragStarを割り当てること

    // メンバ変数
    private GameObject      _ClickParticle;
    private GameObject      _DragParticle;
    private ParticleSystem  _ClickParticleSystem;
    private ParticleSystem  _DragParticleSystem;
    private bool            _ButtonDown;

    // Start is called before the first frame update
    void Start()
    {
        // パーティクルを生成
        _ClickParticle = (GameObject)Instantiate(CLICK_PARTICLE);
        _DragParticle = (GameObject)Instantiate(DRAG_PARTICLE);

        // パーティクルの再生停止を制御するためにコンポーネントを取得
        _ClickParticleSystem = _ClickParticle.GetComponent<ParticleSystem>();
        _DragParticleSystem = _DragParticle.GetComponent<ParticleSystem>();
        _ClickParticleSystem.Stop();
        _DragParticleSystem.Stop();

        // ボタン押下状態
        _ButtonDown = false;
        //File.WriteAllText("Assets/Data/" + "test.txt", "hoge");
    }

    // Update is called once per frame
    void Update()
    {
        // パーティクルをマウスカーソルに追従させる
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 20f;  // ※Canvasよりは手前に位置させること
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        _DragParticle.transform.position = mousePosition;

        // マウス押下orタップ
        if (Input.GetMouseButtonDown(0))
        {
            // 左ボタンダウンを検知したら、マウスカーソル位置から破裂エフェクトとキラキラエフェクトを再生する。
            _ClickParticle.transform.position = mousePosition;
            if (_ButtonDown == false)
            {
                _ClickParticleSystem.Play();   // １回再生(ParticleSystemのLoopingがfalseだから)
                _ButtonDown = true;
            }
            _DragParticleSystem.Play();    // ループ再生(ParticleSystemのLoopingがtrueだから)
        }

        // マウス解放orタップ離し
        if (Input.GetMouseButtonUp(0))
        {
            // 左ボタンアップを検知したら、Particleの放出を停止する
            _ClickParticleSystem.Stop();
            _DragParticleSystem.Stop();
            _ButtonDown = false;
        }
    }
}
