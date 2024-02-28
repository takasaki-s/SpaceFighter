using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppDamagePanel : MonoBehaviour
{
    // メンバ変数
    [SerializeField] private AudioSource _SeDamage = null;      // ダメージSE
    [SerializeField] private AudioSource _SeLost = null;        // 自機破壊SE

    private bool _OnDamage;
    private float _OnTime;

    // Start is called before the first frame update
    void Start()
    {
        _OnDamage = false;
        _OnTime = 0;
        this.GetComponent<Image>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 0.1s経過でダメージパネルOFF
        if (_OnDamage == true)
        {
            if (Time.time >= _OnTime + 0.1f)
            {
                this.Start();
            }
        }
    }

    // ダメージを受けた
    public void OnDamage()
    {
        _OnDamage = true;
        _OnTime = Time.time;
        this.GetComponent<Image>().enabled = true;
        _SeDamage.Play();
    }

    // HPがゼロになった
    public void OnLost()
    {
        _SeLost.Play();
    }
}
