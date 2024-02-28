using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ゲーム本編：自機HPバー制御
public class AppHpBar : MonoBehaviour
{
    private Slider _Slider;
    private float _NowHpValue;

    // Start is called before the first frame update
    void Start()
    {
        _Slider = this.GetComponent<Slider>();
        _Slider.value = _NowHpValue = 100.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // HP上昇
        if (_Slider.value < _NowHpValue)
        {
            _Slider.value += 1.0f;

            if (_Slider.value > 100.0f)
            {
                _Slider.value = 100.0f;
                _NowHpValue = 100.0f;
            }
        }

        // HP減少
        if (_Slider.value > _NowHpValue)
        {
            _Slider.value -= 1.0f;

            if (_Slider.value < 0.0f)
            {
                _Slider.value = 0.0f;
                _NowHpValue = 0.0f;
            }
        }
    }

    // HP値セット
    public void SetValue(float Value)
    {
        _NowHpValue = Value;

        if (_NowHpValue > 100.0f)
        {
            _NowHpValue = 100.0f;
        }
        else if (_NowHpValue < 0.0f)
        {
            _NowHpValue = 0.0f;
        }
    }

    // HP減算
    public void SubValue(float Value)
    {
        _NowHpValue -= Value;

        if (_NowHpValue < 0.0f)
        {
            _NowHpValue = 0.0f;
        }
    }

}
