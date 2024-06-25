using UnityEngine;

//**Clase para definir la brujula del jugador, apuntando a la direccion
//a la que debe dirigirse, así como el comportamiento de calor para ayudar
//a indicar que tan cerca o que tan lejos se encuentra de su destino*//
public class TargetPointer : MonoBehaviour {


	//Velocidad de actualización de la rotación
	public float speed = 10;
	//Valor del Matiz(Hue) mas frio permitido
	public float coldHue;
	//Distancia inicial para calcular el calor de proximidad
	private float initialDistance;
	//Distancia actual a la que el jugador se encuentra del objetivo
	private float currentDistance;
	//Transform de la flecha
	public Transform ObjectTransform;
	//Referencia al script para manipular el Matiz de la flecha
	public HueSpriteShifter HueShifter;
	[SerializeField]
	private Transform TargetPosition;
	private float currentHue;

	void Awake()
	{
		this.TargetPosition = this.transform;
	}

	//Modificar el punto al cual el jugador debe de desplazarse.
	public void ChangeTargetPosition(Transform newTarget)
    {
		this.TargetPosition = newTarget;
		initialDistance = Vector2.Distance(ObjectTransform.position, newTarget.position);
		HueShifter.ShiftHue(coldHue);
	}

    void Update()
    {
        UpdatePointerHue();
        RotatePointer();
    }

    private void UpdatePointerHue()
    {
        currentDistance = Vector2.Distance(ObjectTransform.position, TargetPosition.position);
        float percentDis = (currentDistance * 100) / initialDistance;
        currentHue = (coldHue * percentDis) / 100;
        HueShifter.ShiftHue(currentHue >= coldHue ? coldHue : currentHue);
    }

    private void RotatePointer()
    {
        var step = speed * Time.deltaTime;
        float angle = Mathf.Atan2(TargetPosition.position.y - transform.position.y, TargetPosition.position.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        ObjectTransform.rotation = Quaternion.RotateTowards(ObjectTransform.rotation, targetRotation, step);
    }
}
