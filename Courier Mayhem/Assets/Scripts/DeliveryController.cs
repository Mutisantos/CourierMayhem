using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;



public enum DeliveryState
{
	PICKUP, DELIVERY
}

/** Script para controlar el estado de las entregas en la escena de juego.
 * Permite definir los lugares objetivo a donde debe desplazarse el jugador
 * asi como el flujo de recogida y entrega.
 * Esteban.Hernandez
 */
public class DeliveryController : MonoBehaviour {

    public GameObject commercesParent;
    public GameObject clientsParent;
    [SerializeField]
    private List<DeliveryPoint> CommerceLocation;
    [SerializeField]
    private List<DeliveryPoint> ClientsLocation;

	public TargetPointer Pointer;
	public Transform PlayerPosition;
	public int MaxDeliveryHealth = 100;
	public int RemainingHealth;
    public float timeframeByDistance = 10f;

    private Vector2 lastPickupPoint;
    private Vector2 lastDeliveryPoint;

	private void Awake()
    {

        if (commercesParent != null)
        {
            DeliveryPoint[] points = commercesParent.GetComponentsInChildren<DeliveryPoint>();
            CommerceLocation.Clear();
            for (int i = 0; i < points.Length; i++)
            {
                CommerceLocation.Add(points[i]);
            }
        }

        if (clientsParent != null)
        {
            DeliveryPoint[] points = clientsParent.GetComponentsInChildren<DeliveryPoint>();
            ClientsLocation.Clear();
            for (int i = 0; i < points.Length; i++)
            {
                ClientsLocation.Add(points[i]);
            }
        }
        //Deshabilita los puntos de entrega y recogida al inicio
        foreach (var comLocation in CommerceLocation)
        {
            comLocation.gameObject.SetActive(false);
        }
        foreach (var clientLocation in ClientsLocation)
        {
            clientLocation.gameObject.SetActive(false);
        }
    }

	void Start(){
        GameManager.instance.resetScore();
        GameManager.instance.resetValues();
		GameManager.instance.SetDeliveryController(this);
		GameManager.instance.SetDelivering(false);
		//Habilita el primer punto de recogida de la partida
		StartCoroutine(AssignRandomPickupPointOnDelay(0.5f));
	}

    public void StartDelivery()
    {
        RemainingHealth = MaxDeliveryHealth;
        DeliveryPoint nextDelivery = this.ClientsLocation[GetRandomIndex(this.ClientsLocation.Count - 1)];
        nextDelivery.gameObject.SetActive(true);
        Pointer.ChangeTargetPosition(nextDelivery.transform);
        lastDeliveryPoint = nextDelivery.transform.position;
        float currentDistance = Vector2.Distance(lastPickupPoint, lastDeliveryPoint);
        GameManager.instance.SetDeliveryTime(currentDistance * timeframeByDistance);
    }
    public void FinishDelivery()
    {
        GameManager.instance.AddScore(RemainingHealth * 10);
        DeliveryPoint nextPickup = this.CommerceLocation[GetRandomIndex(this.CommerceLocation.Count-1)];
        nextPickup.gameObject.SetActive(true);
        Pointer.ChangeTargetPosition(nextPickup.transform);
        lastPickupPoint = nextPickup.transform.position;
    }

    public int GetRandomIndex(int ListSize)
	{
		if (ListSize > 0)
		{
			System.Random rand = new System.Random();
			return rand.Next(ListSize);
		}
		return 0;
	}

    public int GetRemainingDeliveryHealth()
    {
        return this.RemainingHealth;
    }

    public void ReduceRemainingDeliveryHealth(int damage)
    {
        if(this.RemainingHealth - damage > 0)
        {
            this.RemainingHealth -= damage;
        }
        else
        {
            this.RemainingHealth = 0;
        }
    }

    IEnumerator AssignRandomPickupPointOnDelay(float delay)
    {
        DeliveryPoint newLocation = this.CommerceLocation[GetRandomIndex(this.CommerceLocation.Count)];
        newLocation.gameObject.SetActive(true);
		yield return new WaitForSeconds(delay);
        Pointer.ChangeTargetPosition(newLocation.transform);
        lastPickupPoint = newLocation.transform.position;

    }


}
