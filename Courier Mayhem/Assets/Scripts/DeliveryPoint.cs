using UnityEngine;

public enum PointType
{
	COMMERCE, CLIENT
}

/** Script para definir el comportamiento de los puntos de recoleccion o entrega
 * Esteban.Hernandez
 */
public class DeliveryPoint : MonoBehaviour {


	public PointType type;
	public Vector2 position;
	public AudioClip soundFx; 

	void Start() {
		this.position = this.GetComponent<Transform>().position;
	}


	void OnTriggerEnter2D (Collider2D coll)
	{
		if (coll.tag == "Player" ) {
			SoundManager.instance.PlayOnce(soundFx);
            if (!GameManager.instance.IsPlayerDelivering() && this.type.Equals(PointType.COMMERCE))
            {
				GameManager.instance.SetDelivering(true);
				GameManager.instance.StartDelivery();
            }
            if (GameManager.instance.IsPlayerDelivering() && this.type.Equals(PointType.CLIENT))
            {
                GameManager.instance.SetDelivering(false);
                GameManager.instance.CompleteDelivery();
            }

            this.gameObject.SetActive(false);
        }
	}

}
