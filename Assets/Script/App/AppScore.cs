using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ゲーム本編：スコア
public class AppScore : MonoBehaviour
{
    private int _Score;

    // Start is called before the first frame update
    void Start()
    {
        _Score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Text>().text = "SCORE:" + _Score.ToString("D8");
    }

    // スコア加算
    public void AddScore(int AddScore)
    {
        _Score += AddScore;
    }

    // スコア取得
    public int GetScore()
    {
        return _Score;
    }
}
