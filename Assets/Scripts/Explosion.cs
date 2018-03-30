using UnityEngine;

// 爆発エフェクトを制御するコンポーネント
public class Explosion : MonoBehaviour
{
	public ParticleSystem m_particleSystem; // パーティクルシステム

	// 爆発エフェクトが生成された時に呼び出される関数
	private void Start()
	{
		// 演出が完了したら削除する
		Destroy( gameObject, m_particleSystem.main.duration );
	}
}
