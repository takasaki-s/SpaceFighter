using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 共通：ゲーム情報
// 進捗情報
public enum CmnProgress
{
    Start,      // 最初から
    Boss,       // ボスから
    Max
}

// 共通：プレイ情報
public class CmnPlayInfo
{
    public static CmnProgress NowProgress;
}
