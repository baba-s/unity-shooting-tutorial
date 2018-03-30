using UnityEngine;

// 背景を制御するコンポーネント
public class Background : MonoBehaviour
{
	public Transform m_player; // プレイヤー
	public Vector2 m_limit; // 背景の移動範囲

	// 毎フレーム呼び出される関数
	private void Update()
	{
		// プレイヤーの現在地を取得する
		var pos = m_player.localPosition;

		// 画面端の位置を取得する
		var limit = Utils.m_moveLimit;

		// プレイヤーが画面のどの位置に存在するのかを、0 から 1 の値に置き換える
		var tx = 1 - Mathf.InverseLerp( -limit.x, limit.x, pos.x );
		var ty = 1 - Mathf.InverseLerp( -limit.y, limit.y, pos.y );

		// プレイヤーの現在地から背景の表示位置を算出する
		var x = Mathf.Lerp( -m_limit.x, m_limit.x, tx );
		var y = Mathf.Lerp( -m_limit.y, m_limit.y, ty );

		// 背景の表示位置を更新する
		transform.localPosition = new Vector3( x, y, 0 );
	}
}
