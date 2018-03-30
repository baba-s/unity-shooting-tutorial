using UnityEngine;
using UnityEngine.UI;

// 情報表示用の UI を制御するコンポーネント
public class Hud : MonoBehaviour
{
	public Player m_player; // プレイヤー
	public Image m_hpGauge; // HP ゲージ
	public Image m_expGauge; // 経験値ゲージ
	public Text m_levelText;// レベルのテキスト
	public GameObject m_gameOverText; // ゲームオーバーのテキスト

	// 毎フレーム呼び出される関数
	private void Update()
	{
		// HP のゲージの表示を更新する
		var hp = m_player.m_hp;
		var hpMax = m_player.m_hpMax;
		m_hpGauge.fillAmount = ( float ) hp/ hpMax;

		// 経験値のゲージの表示を更新する
		var exp = m_player.m_exp;
		var prevNeedExp = m_player.m_prevNeedExp;
		var needExp = m_player.m_needExp;
		m_expGauge.fillAmount = ( float )( exp - prevNeedExp ) / ( needExp - prevNeedExp );

		// レベルのテキストの表示を更新する
		m_levelText.text = m_player.m_level.ToString();

		// プレイヤーが非表示ならゲームオーバーと表示する
		m_gameOverText.SetActive( !m_player.gameObject.activeSelf );
	}
}
