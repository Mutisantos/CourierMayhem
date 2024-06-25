using UnityEngine;
using UnityEngine.UI;

/** Script para contrlar el HUD de la pantalla de juego
 * Esteban.Hernandez
 */
public class HUDController : MonoBehaviour {

	public Text moneyLabel;
	public Text deliveryTimeLabel;
	public Text timeLimitLabel;
    public GameObject deliveryPanel;
    public GameObject gameOverPanel;
    public Slider deliveryHealthBar;
	public Image multiplierFill;



	void Start(){
		deliveryHealthBar.maxValue = 100;
		GameManager.instance.SetHUDController(this);
        this.setGameOverPanelVisible(false);
        this.setDeliveryPanelVisible(false);
    }
	
	void Update () {
			updateBar();
	}


	private void updateBar(){
		int value = GameManager.instance.GetDeliveryHealth();
		deliveryHealthBar.value = value;
	}

	public void updateScore(int score){
		moneyLabel.text = score.ToString("D9");
	}



    public void setDeliveryPanelVisible(bool show)
    {
        deliveryPanel.SetActive(show);
    }

    public void setGameOverPanelVisible(bool show)
    {
        gameOverPanel.SetActive(show);
    }

    public void UpdateClockTime(float time)
    {
        System.TimeSpan t = System.TimeSpan.FromSeconds(time);
        deliveryTimeLabel.text = string.Format("{0:00}:{1:00}:{2:00}", t.Minutes, t.Seconds, (time * 1000) % 100);
    }

    public void UpdateGameTime(float time)
    {
        timeLimitLabel.text = string.Format("{000}s.", Mathf.FloorToInt(time));
    }



}
