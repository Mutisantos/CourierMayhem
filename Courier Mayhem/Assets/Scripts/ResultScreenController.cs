using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

/** Script para controlar los elementos de UI para la pantalla de resultados
 * Esteban.Hernandez
 */
public class ResultScreenController : MonoBehaviour {

	public Text moneyLabel;
    public float fillingStep = 1000;
    public float remainingMoney = 0;
    public float expectedMoney = 0;
    public AudioClip cashClip;
    public AudioClip celebrationClip;

    void Start(){
        SoundManager.instance.PlayOnce(cashClip);
        expectedMoney = GameManager.instance.GetScore();
        remainingMoney = GameManager.instance.GetScore();
        Time.timeScale = 1;
    }
	
	void Update () {
        Debug.Log(remainingMoney + " - " + expectedMoney);
        if (remainingMoney > 0)
        {
            SoundManager.instance.PlayOnce(cashClip);
            remainingMoney -= Time.deltaTime * fillingStep;
            float difference = expectedMoney - remainingMoney;
            moneyLabel.text = Mathf.FloorToInt(difference).ToString("D9") + "$";
        }
        else
        {
            moneyLabel.text = Mathf.FloorToInt(expectedMoney).ToString("D9") + "$";
            SoundManager.instance.PlayOnce(celebrationClip);
        }
    }
}
