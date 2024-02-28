using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 共通：FPS設定
// ※各シーンのマネージャに設定すること
public class CmnSetFps : MonoBehaviour
{
    // インスタンスがロードされたときに呼び出される。
    private void Awake()
    {
        Application.targetFrameRate = 60;   // 60FPS
    }
}
