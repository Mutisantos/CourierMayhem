using UnityEngine;
using System.Collections;


/** Singleton para el control de la escena de juego y pasar variables a otras escenas posteriormente
 * Enlaza el comportamiento del juego con la Interfaz de juego tambien.
 * Esteban.Hernandez
 */
public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public HUDController hudController;
	public DeliveryController deliveryController;

    private float deliveryTime = 0f;
	public float maximumTime = 600f;
	private float remainingPlayTime = 600f;
    public int score = 0;
	private bool ended = false;
	private bool IsDelivering = false;

	void Awake() {
		MakeSingleton ();
        Time.timeScale = 1;
        remainingPlayTime = maximumTime;
    }

	public void ResetGameManagerValues()
	{
        Time.timeScale = 1;
		remainingPlayTime = maximumTime;
    }

	private void Update()
	{
		if (this.hudController != null) { 
			if (this.remainingPlayTime <= 0)
			{
				this.hudController.setGameOverPanelVisible(true);
				Time.timeScale = 0;
			}
			else
			{
                this.hudController.setGameOverPanelVisible(false);
                this.remainingPlayTime -= Time.deltaTime;
				this.hudController.UpdateGameTime(this.remainingPlayTime);
			}
			if (this.deliveryTime > 0 && this.IsDelivering)
			{
                this.deliveryTime -= Time.deltaTime;
				this.hudController.UpdateClockTime(this.deliveryTime);
			}
			else
			{
				this.deliveryTime = 0f;
			}
		}
	}

    private void MakeSingleton() {
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

	public void SetHUDController(HUDController controller)
	{
		this.hudController = controller;
	}
	public void SetDeliveryController(DeliveryController controller)
	{
		this.deliveryController = controller;
    }
    public int GetDeliveryHealth()
    {
        if (this.deliveryController != null)
        {
            return this.deliveryController.GetRemainingDeliveryHealth();
        }
        return 0;
    }
    public void ReduceDeliveryHealth(int penalty)
    {
        this.deliveryController.ReduceRemainingDeliveryHealth(penalty);
    }

    public void CompleteDelivery()
    {
        this.deliveryController.FinishDelivery();
        this.hudController.setDeliveryPanelVisible(false);
    }
    public void StartDelivery()
    {
        this.deliveryController.StartDelivery();
        this.hudController.setDeliveryPanelVisible(true);
    }

	public float GetDeliveryTime()
	{
		return this.deliveryTime;
	}

	public void SetDeliveryTime(float time)
	{
		this.deliveryTime = time;
	}

	public bool IsPlayerDelivering()
	{
		return this.IsDelivering;
	}


	public void SetDelivering(bool delivering)
	{
		this.IsDelivering = delivering;
	}

	public int GetScore(){
		return this.score;
	}

	public void AddScore(int points){
		this.score += points;
		this.score += Mathf.FloorToInt(deliveryTime * 10);
        this.hudController.updateScore(this.score);
	}

    public bool isEnded()
    {
        return this.ended;
	}

	public void setEnded(bool ended){
		this.ended = ended;
	}


    public void resetValues()
    {
        this.ended = false;
        this.deliveryTime = 0f;
        remainingPlayTime = maximumTime;
    }
    public void resetScore()
    {
		this.score = 0;
    }


} 
