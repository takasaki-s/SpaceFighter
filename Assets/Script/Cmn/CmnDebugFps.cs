using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ���ʁFFPS�v���X�N���v�g
// ���q�G�����L�[�Ƀe�L�X�g��z�u���ăA�^�b�`����
public class CmnDebugFps : MonoBehaviour
{
    // �����o�ϐ�
    private uint _FpsCount;     // �t���[�����J�E���g�l
    private float _LastTime;    // �O�񎞊�

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        this.gameObject.SetActive(true);   // 
#else
        this.gameObject.SetActive(false);
#endif

        _FpsCount = 0;
        _LastTime = Time.time;

        UpdateFpsCount();
    }

    // Update is called once per frame
    void Update()
    {
        // �O�񎞊Ԃ���1s�o�߂��Ă����FPS�l���X�V
        float NowTime = Time.time;
        if (NowTime >= _LastTime + 1.0f)
        {
            UpdateFpsCount();
            _LastTime = NowTime;
            _FpsCount = 0;
        }
        else
        {
            _FpsCount++;
        }
    }

    // FPS�\���X�V
    private void UpdateFpsCount()
    {
        this.GetComponent<Text>().text = _FpsCount.ToString() + " fps";
    }
}
