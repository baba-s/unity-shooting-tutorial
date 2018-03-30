using UnityEngine;

// 便利な関数を管理する静的クラス
public static class Utils
{
	// 移動可能な範囲
	public static readonly Vector2 m_moveLimit = new Vector2( 4.7f, 3.4f );

	// 指定された 2 つの位置から角度を求めて返す
	public static float GetAngle( Vector2 from, Vector2 to )
	{
		var dx = to.x - from.x;
		var dy = to.y - from.y;
		var rad = Mathf.Atan2( dy, dx );
		return rad * Mathf.Rad2Deg;
	}

	// 指定された角度（ 0 ～ 360 ）をベクトルに変換して返す
	public static Vector3 GetDirection( float angle )
	{
		return new Vector3
		(
			Mathf.Cos( angle * Mathf.Deg2Rad ),
			Mathf.Sin( angle * Mathf.Deg2Rad ),
			0
		);
	}

	// 指定された位置を移動可能な範囲に収めた値を返す
	public static Vector3 ClampPosition( Vector3 position )
	{
		return new Vector3
		(
			Mathf.Clamp( position.x, -m_moveLimit.x, m_moveLimit.x ),
			Mathf.Clamp( position.y, -m_moveLimit.y, m_moveLimit.y ),
			0
		);
	}
}
