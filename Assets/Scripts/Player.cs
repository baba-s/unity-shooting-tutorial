using UnityEngine;

// プレイヤーを制御するコンポーネント
public class Player : MonoBehaviour
{
	public float m_speed; // 移動の速さ
	public Shot  m_shotPrefab;     // 弾のプレハブ
	public float m_shotSpeed;      // 弾の移動の速さ
	public float m_shotAngleRange; // 複数の弾を発射する時の角度
	public float m_shotTimer;      // 弾の発射タイミングを管理するタイマー
	public int   m_shotCount;      // 弾の発射数
	public float m_shotInterval;   // 弾の発射間隔（秒）
	public int m_hpMax; // HP の最大値
	public int m_hp;    // HP
	public float m_magnetDistance; // 宝石を引きつける距離
	public int m_nextExpBase;     // 次のレベルまでに必要な経験値の基本値
	public int m_nextExpInterval; // 次のレベルまでに必要な経験値の増加値
	public int m_level;           // レベル
	public int m_exp;             // 経験値
	public int m_prevNeedExp;     // 前のレベルに必要だった経験値
	public int m_needExp;         // 次のレベルに必要な経験値
	public AudioClip m_levelUpClip; // レベルアップした時に再生する SE
	public AudioClip m_damageClip;  // ダメージを受けた時に再生する SE
	public int   m_levelMax;           // レベルの最大値
	public int   m_shotCountFrom;      // 弾の発射数（レベルが最小値の時）
	public int   m_shotCountTo;        // 弾の発射数（レベルが最大値の時）
	public float m_shotIntervalFrom;   // 弾の発射間隔（秒）（レベルが最小値の時）
	public float m_shotIntervalTo;     // 弾の発射間隔（秒）（レベルが最大値の時）
	public float m_magnetDistanceFrom; // 宝石を引きつける距離（レベルが最小値の時）
	public float m_magnetDistanceTo;   // 宝石を引きつける距離（レベルが最大値の時）

	// プレイヤーのインスタンスを管理する static 変数
	public static Player m_instance;

	// ゲーム開始時に呼び出される関数
	private void Awake()
	{
		// 他のクラスからプレイヤーを参照できるように
// static 変数にインスタンス情報を格納する
		m_instance = this;

		m_hp = m_hpMax; // HP

		m_level   = 1;               // レベル
		m_needExp = GetNeedExp( 1 ); // 次のレベルに必要な経験値

		m_shotCount      = m_shotCountFrom;      // 弾の発射数
		m_shotInterval   = m_shotIntervalFrom;   // 弾の発射間隔（秒）
		m_magnetDistance = m_magnetDistanceFrom; // 宝石を引きつける距離
	}

	// 毎フレーム呼び出される関数
	private void Update()
	{
		// 矢印キーの入力情報を取得する
		var h = Input.GetAxis( "Horizontal" );
		var v = Input.GetAxis( "Vertical" );

		// 矢印キーが押されている方向にプレイヤーを移動する
		var velocity = new Vector3( h, v ) * m_speed;
		transform.localPosition += velocity;

		// プレイヤーが画面外に出ないように位置を制限する
		transform.localPosition = Utils.ClampPosition( transform.localPosition );

		// プレイヤーのスクリーン座標を計算する
		var screenPos = Camera.main.WorldToScreenPoint( transform.position );

// プレイヤーから見たマウスカーソルの方向を計算する
		var direction = Input.mousePosition - screenPos;

// マウスカーソルが存在する方向の角度を取得する
		var angle = Utils.GetAngle( Vector3.zero, direction );

// プレイヤーがマウスカーソルの方向を見るようにする
		var angles = transform.localEulerAngles;
		angles.z                   = angle - 90;
		transform.localEulerAngles = angles;

		// 弾の発射タイミングを管理するタイマーを更新する
		m_shotTimer += Time.deltaTime;

// まだ弾の発射タイミングではない場合は、ここで処理を終える
		if ( m_shotTimer < m_shotInterval ) return;

// 弾の発射タイミングを管理するタイマーをリセットする
		m_shotTimer = 0;

// 弾を発射する
		ShootNWay( angle, m_shotAngleRange, m_shotSpeed, m_shotCount );
	}

	// 弾を発射する関数
	private void ShootNWay( 
		float angleBase, float angleRange, float speed, int count )
	{
		var pos = transform.localPosition; // プレイヤーの位置
		var rot = transform.localRotation; // プレイヤーの向き

		// 弾を複数発射する場合
		if ( 1 < count )
		{
			// 発射する回数分ループする
			for ( int i = 0; i < count; ++i )
			{
				// 弾の発射角度を計算する
				var angle = angleBase + 
				            angleRange * ( ( float )i / ( count - 1 ) - 0.5f );

				// 発射する弾を生成する
				var shot = Instantiate( m_shotPrefab, pos, rot );

				// 弾を発射する方向と速さを設定する
				shot.Init( angle, speed );
			}
		}
		// 弾を 1 つだけ発射する場合
		else if ( count == 1 )
		{
			// 発射する弾を生成する
			var shot = Instantiate( m_shotPrefab, pos, rot );

			// 弾を発射する方向と速さを設定する
			shot.Init( angleBase, speed );
		}
	}

	// ダメージを受ける関数
// 敵とぶつかった時に呼び出される
	public void Damage( int damage )
	{
		// ダメージを受けた時の SE を再生する
		var audioSource = FindObjectOfType<AudioSource>();
		audioSource.PlayOneShot( m_damageClip );

		// HP を減らす
		m_hp -= damage;

		// HP がまだある場合、ここで処理を終える
		if ( 0 < m_hp ) return;

		// プレイヤーが死亡したので非表示にする
		// 本来であれば、ここでゲームオーバー演出を再生したりする
		gameObject.SetActive( false );
	}

	// 経験値を増やす関数
// 宝石を取得した時に呼び出される
	public void AddExp( int exp )
	{
		// 経験値を増やす
		m_exp += exp;

		// まだレベルアップに必要な経験値に足りていない場合、ここで処理を終える
		if ( m_exp < m_needExp ) return;

		// レベルアップする
		m_level++;

		// 今回のレベルアップに必要だった経験値を記憶しておく
		// （経験値ゲージの表示に使用するため）
		m_prevNeedExp = m_needExp;

		// 次のレベルアップに必要な経験値を計算する
		m_needExp = GetNeedExp( m_level );

		// レベルアップした時にボムを発動する
		// ボムの発射数や速さは決め打ちで定義しているため
		// 必要であれば public 変数にして、
		// Unity エディタ上で設定できるように変更してください
		var angleBase  = 0;
		var angleRange = 360;
		var count      = 28;
		ShootNWay( angleBase, angleRange, 0.15f, count );
		ShootNWay( angleBase, angleRange, 0.2f, count );
		ShootNWay( angleBase, angleRange, 0.25f, count );

		// レベルアップした時の SE を再生する
		var audioSource = FindObjectOfType<AudioSource>();
		audioSource.PlayOneShot( m_levelUpClip );

		// レベルアップしたので、各種パラメータを更新する
		var t = ( float )( m_level - 1 ) / ( m_levelMax - 1 );
		m_shotCount = Mathf.RoundToInt( 
			Mathf.Lerp( m_shotCountFrom, m_shotCountTo, t ) ); // 弾の発射数
		m_shotInterval = Mathf.Lerp( 
			m_shotIntervalFrom, m_shotIntervalTo, t ); // 弾の発射間隔（秒）
		m_magnetDistance = Mathf.Lerp( 
			m_magnetDistanceFrom, m_magnetDistanceTo, t ); // 宝石を引きつける距離
	}

	// 指定されたレベルに必要な経験値を計算する関数
	private int GetNeedExp( int level )
	{
		/*
		 * 例えば、m_nextExpBase が 16、m_nextExpInterval が 18 の場合、
		 *
		 * レベル 1：16 + 18 * 0 = 16
		 * レベル 2：16 + 18 * 1 = 34
		 * レベル 3：16 + 18 * 4 = 88
		 * レベル 4：16 + 18 * 9 = 178
		 *
		 * このような計算式になり、レベルが上がるほど必要な経験値が増えていく
		 */
		return m_nextExpBase + 
		       m_nextExpInterval * ( ( level - 1 ) * ( level - 1 ) );
	}
}