using UnityEngine;

// 爆発エフェクトを制御するコンポーネント
public class Explosion : MonoBehaviour
{
	// 爆発エフェクトが生成された時に呼び出される関数
	private void Start()
	{
		// 演出が完了したら削除する
		var particleSystem = GetComponent<ParticleSystem>();
		Destroy( gameObject, particleSystem.main.duration );
	}
}