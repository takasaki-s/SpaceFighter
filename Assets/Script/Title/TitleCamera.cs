using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// タイトル画面：カメラ制御
public class TitleCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // x軸を-30°からスタート
        this.transform.RotateAround(this.transform.position, Vector3.right, -30.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // 毎フレームカメラをy軸回転
        Quaternion qRot = Quaternion.AngleAxis(0.05f, Vector3.up);
        Quaternion qNow = this.transform.rotation;
        this.transform.rotation = qRot * qNow;
    }
}
