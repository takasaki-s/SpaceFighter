using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ʁFFPS�ݒ�
// ���e�V�[���̃}�l�[�W���ɐݒ肷�邱��
public class CmnSetFps : MonoBehaviour
{
    // �C���X�^���X�����[�h���ꂽ�Ƃ��ɌĂяo�����B
    private void Awake()
    {
        Application.targetFrameRate = 60;   // 60FPS
    }
}
