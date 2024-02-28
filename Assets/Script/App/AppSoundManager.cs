using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲーム本編：サウンドマネージャ
public class AppSoundManager : MonoBehaviour
{
    // メンバ変数
    [SerializeField] private AudioSource _AudioSeExplosion = null;      // 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // SE再生
    public void SePlay(string SeName)
    {
        _AudioSeExplosion.Play();
    }
}
