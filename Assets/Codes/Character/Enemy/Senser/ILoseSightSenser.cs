using UnityEngine;

/// <summary>
/// 「ターゲットを見失ったか」を判定するセンサーのインターフェース。
/// ChaseSenser（発見）とは独立して動作し、ChaseStateが毎フレーム問い合わせる。
/// </summary>
public interface ILoseSightSenser
{
    bool HasLostSight(GameObject target);
}
