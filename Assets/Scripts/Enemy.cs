using UnityEngine;
using System.Linq;

// 敵の出現位置の種類
public enum RESPAWN_TYPE
{
    UP, // 上
    RIGHT, // 右
    DOWN, // 下
    LEFT, // 左
    SIZEOF, // 敵の出現位置の数
}

// 敵を制御するコンポーネント
public class Enemy : MonoBehaviour
{
    public Vector2 m_respawnPosInside; // 敵の出現位置（内側）
    public Vector2 m_respawnPosOutside; // 敵の出現位置（外側）
    public float m_speed; // 移動する速さ
    public int m_hpMax; // HP の最大値
    public int m_exp; // この敵を倒した時に獲得できる経験値
    public int m_damage; // この敵がプレイヤーに与えるダメージ

    private int m_hp; // HP
    private Vector3 m_direction; // 進行方向

    public Explosion m_explosionPrefab; // 爆発エフェクトのプレハブ

    public bool m_isFollow; // プレイヤーを追尾する場合 true

    public Gem[] m_gemPrefabs;  // 宝石のプレハブを管理する配列
    public float m_gemSpeedMin; // 生成する宝石の移動の速さ（最小値）
    public float m_gemSpeedMax; // 生成する宝石の移動の速さ（最大値）
    public AudioClip m_deathClip; // 敵を倒した時に再生する SE

    // 敵が生成された時に呼び出される関数
    private void Start()
    {
        // HP を初期化する
        m_hp = m_hpMax;
    }

    // 毎フレーム呼び出される関数
    private void Update()
    {
	    // プレイヤーを追尾する場合
	    if ( m_isFollow )
	    {
		    // プレイヤーの現在位置へ向かうベクトルを作成する
		    var angle = Utils.GetAngle( 
			    transform.localPosition, 
			    Player.m_instance.transform.localPosition );
		    var direction = Utils.GetDirection( angle );

		    // プレイヤーが存在する方向に移動する
		    transform.localPosition += direction * m_speed;

		    // プレイヤーが存在する方向を向く
		    var angles = transform.localEulerAngles;
		    angles.z                   = angle - 90;
		    transform.localEulerAngles = angles;
		    return;
	    }

        // まっすぐ移動する
        transform.localPosition += m_direction * m_speed;
    }

    // 敵が出現する時に初期化する関数
    public void Init( RESPAWN_TYPE respawnType )
    {
        var pos = Vector3.zero;

        // 指定された出現位置の種類に応じて、
        // 出現位置と進行方向を決定する
        switch ( respawnType )
        {
            // 出現位置が上の場合
            case RESPAWN_TYPE.UP:
                pos.x = Random.Range( 
                    -m_respawnPosInside.x, m_respawnPosInside.x );
                pos.y = m_respawnPosOutside.y;
                m_direction = Vector2.down;
                break;

            // 出現位置が右の場合
            case RESPAWN_TYPE.RIGHT:
                pos.x = m_respawnPosOutside.x;
                pos.y = Random.Range( 
                    -m_respawnPosInside.y, m_respawnPosInside.y );
                m_direction = Vector2.left;
                break;

            // 出現位置が下の場合
            case RESPAWN_TYPE.DOWN:
                pos.x = Random.Range( 
                    -m_respawnPosInside.x, m_respawnPosInside.x );
                pos.y = -m_respawnPosOutside.y;
                m_direction = Vector2.up;
                break;

            // 出現位置が左の場合
            case RESPAWN_TYPE.LEFT:
                pos.x = -m_respawnPosOutside.x;
                pos.y = Random.Range( 
                    -m_respawnPosInside.y, m_respawnPosInside.y );
                m_direction = Vector2.right;
                break;
        }

        // 位置を反映する
        transform.localPosition = pos;
    }

    // 他のオブジェクトと衝突した時に呼び出される関数
    private void OnTriggerEnter2D( Collider2D collision )
    {
	    // 弾と衝突した場合
	    if ( collision.name.Contains( "Shot" ) )
	    {
		    // 弾が当たった場所に爆発エフェクトを生成する
		    Instantiate( 
			    m_explosionPrefab, 
			    collision.transform.localPosition, 
			    Quaternion.identity );

		    // 弾を削除する
		    Destroy( collision.gameObject );

		    // 敵の HP を減らす
		    m_hp--;

		    // 敵の HP がまだ残っている場合はここで処理を終える
		    if ( 0 < m_hp ) return;

		    // 敵を倒した時の SE を再生する
		    var audioSource = FindObjectOfType<AudioSource>();
		    audioSource.PlayOneShot( m_deathClip );

		    // 敵を削除する
		    Destroy( gameObject );

		    /*
 * 敵が死亡した場合は宝石を散らばらせる
 *
 * 例えば、敵を倒した時に獲得できる経験値が 4 で、
 * 経験値を 1 獲得できる宝石 A と、経験値を 2 獲得できる宝石 B が存在する場合、
 *
 * 1. 宝石 A を 4 個
 * 2. 宝石 A を 2 個、宝石 B を 1 個
 * 3. 宝石 B を 2 個
 *
 * のいずれかのパターンで宝石が散らばる
 */
		    var exp = m_exp;

		    while ( 0 < exp )
		    {
			    // 生成可能な宝石を配列で取得する
			    var gemPrefabs = m_gemPrefabs.Where( c => c.m_exp <= exp ).ToArray();

			    // 生成可能な宝石の配列から、生成する宝石をランダムに決定する
			    var gemPrefab = gemPrefabs[ Random.Range( 0, gemPrefabs.Length ) ];

			    // 敵の位置に宝石を生成する
			    var gem = Instantiate( 
				    gemPrefab, transform.localPosition, Quaternion.identity );

			    // 宝石を初期化する
			    gem.Init( m_exp, m_gemSpeedMin, m_gemSpeedMax );

			    // まだ宝石を生成できるかどうか計算する
			    exp -= gem.m_exp;
		    }
	    }

	    // プレイヤーと衝突した場合
	    if ( collision.name.Contains( "Player" ) )
	    {
		    // プレイヤーにダメージを与える
		    var player = collision.GetComponent<Player>();
		    player.Damage( m_damage );
		    return;
	    }
    }
}