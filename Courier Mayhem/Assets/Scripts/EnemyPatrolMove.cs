using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RoutineState
{
	PATROL, PURSUIT, RETURNING
}

/**
 * Script para definir el comportamiento de seguimiento de enemigos al jugador o mantener 
 * una patrulla definida en puntos secuenciales en el espacio 2D. 
 * Tambien define la estructura del enemigo con sus colliders, rango de visión y 
 * bonificación de velocidad en persecusión¿, bonificación si se logra evadir al enemigo
 * y penalizacion en caso que haya una colisión.
 */
public class EnemyPatrolMove : MonoBehaviour
{
	//Contenedor de puntos del recorrido programado del enemigo.
	public GameObject pointsContainer;
	//Puntos de recorrido de patrulla
	[SerializeField]
	private List<Transform> waypoints;
	//Tasa de movimiento del enemigo por frame
	public float step = 0.01f;
	//Multiplicador de velocidad cuando inicia la persecusión
	public float pursuitMultiplier = 1.5f;
	//Determina si se requiere rotar el collider del objeto cuando cambia de orientación (para enemigos no-simétricos) 
	public bool rotateCollider = false;
    //Bonificación de dinero si se logra evadir a un enemigo
    public int avoidanceBonus = 100;
    public int deliveryDamage = 10;

    //Componente fisico
    public Rigidbody2D myBody;
	//Componente de animacion
	private Animator anim;
	//Velocidad de la animación
	private float animSpeed;
	//Colisionador fisico del enemigo
	private Collider2D enemyCollider;
	//Cuerpo del Enemigo 
	private Collider2D enemyBody;
	//Vision del enemigo (Trigger)
	private Collider2D enemyRange;

	//Indice actual del punto objetivo.
	public int targetPoint = 0;
	//Posicion Actual
	private Vector2 currentPosition;
	//Posicion del waypoint siguiente 
	private Vector2 followingPosition;
	//Velocidad de movimiento base 
	private float speed;
	//Para saber el estado del enemigo si está patrullando o en persecusión
	private RoutineState currentState ;
	//Sonidos para indicar que el enemigo ha visto al jugador
	public AudioClip[] detectionFX;
	//Margen de error para mover al enemigo a la posición correcta
	private float maxDistance = 0.2f;
	//Si el enemigo ya fue evadido, no vva a sumar mas en la puntacion
	private bool counted = false;

	private int collided = 0;



	void Start ()
	{
        enemyRange = GetComponentsInChildren<Collider2D> () [1];
		if (rotateCollider) {
			enemyBody = GetComponentsInChildren<Collider2D> () [0];
			enemyCollider = GetComponentsInChildren<Collider2D> () [2];
		}
		if (myBody == null)
		{
			myBody = GetComponent<Rigidbody2D>();
		}
		anim = GetComponent<Animator> ();
		currentPosition = new Vector2 (0f, 0f);
		followingPosition = new Vector2 (0f, 0f);
		speed = step;
		currentState = RoutineState.PATROL;
		animSpeed = anim.GetFloat("Speed");
		counted = false;
		if (pointsContainer != null) {
			Transform[] points = pointsContainer.GetComponentsInChildren<Transform> ();
			waypoints.Clear ();
			for (int i = 1; i < points.Length; i++) {
				waypoints.Add(points[i]);
			}
		}

	}
	
	void FixedUpdate ()
	{
		if (!GameManager.instance.isEnded ()) {//Los enemigos no pueden seguir persiguiendo si el juego se termina
			MovePosition ();
		}
	}

	void Update (){
		if (!GameManager.instance.isEnded ()) {//Los enemigos no pueden seguir persiguiendo si el juego se termina
			ChangeDirection ();
		}
	}


	/**Metodo que mueve el enemigo hacia una posicion determinada por sus waypoints o el enemigo*/

	public void MovePosition ()
	{
		currentPosition = myBody.position;
		if (!currentState.Equals(RoutineState.PURSUIT)) {
			followingPosition = waypoints [targetPoint].position;
		}
		if (Vector2.Distance (this.currentPosition, this.followingPosition) > this.maxDistance) {
			myBody.MovePosition (Vector2.MoveTowards (this.currentPosition, this.followingPosition, step));
		} else {
			if (targetPoint >= waypoints.Count - 1) {
				targetPoint = 0;
			} else {
				targetPoint++;
			}
		}	
	}




	/**Cambiar la direccion a la que apunta el sprite, apoyado en el animator*/
	public void ChangeDirection(){
		/*Establecer la dirección de vision y del sprite*/
		Vector3 temp = enemyRange.transform.rotation.eulerAngles;
		float deltaX = currentPosition.x - followingPosition.x;
		float deltaY = currentPosition.y - followingPosition.y;
		int direction = 0;
		if (Mathf.Abs (deltaX) > Mathf.Abs (deltaY)) {//Darle mas prelación al X que al Y
			if (currentPosition.x > followingPosition.x) {//Apuntar a la derecha
				temp.z = -90f;
				direction = 0;
			}
			if (currentPosition.x < followingPosition.x) {//Apuntar a la izquierda
				temp.z = 90f;
				direction = 2;
			}

			enemyRange.transform.rotation = Quaternion.Euler (temp);
			if (rotateCollider) {
				enemyBody.transform.rotation = Quaternion.Euler (temp);
				enemyCollider.transform.rotation = Quaternion.Euler (temp);
			}

		}
		else {
			if (currentPosition.y < followingPosition.y) {//Apuntar a arriba
				temp.z = 180f;
				direction = 3;
			}
			if (currentPosition.y > followingPosition.y) {//Apuntar a abajo
				temp.z = 0f;
				direction = 1;
			}

			enemyRange.transform.rotation = Quaternion.Euler (temp);
			if (rotateCollider) {
				enemyBody.transform.rotation = Quaternion.Euler (temp);
				enemyCollider.transform.rotation = Quaternion.Euler (temp);
			}

		}
		if(direction != anim.GetInteger("Direction")){
			anim.SetBool ("Idle", false);
			anim.SetInteger ("Direction", direction);
			anim.SetFloat ("Speed", animSpeed);
		}
	}

	void OnTriggerEnter2D (Collider2D coll)
	{

		if (coll.CompareTag("Player") && currentState.Equals(RoutineState.PATROL)) {
			SoundManager.instance.RandomizeFx (detectionFX);
			currentState = RoutineState.PURSUIT;
			step = pursuitMultiplier * speed;
			Rigidbody2D player = coll.attachedRigidbody;
			followingPosition = player.position;
		}
		else if (coll.tag == "Enemy"){
			AssignTargetToPreviousWaypoint();		
		}
	}

	void OnTriggerStay2D (Collider2D coll)
	{
		if (coll.CompareTag("Player")) {
			currentState = RoutineState.PURSUIT;
			step = pursuitMultiplier * speed;
			Rigidbody2D player = coll.attachedRigidbody;
			followingPosition = player.position;
		}
	}


	void OnTriggerExit2D (Collider2D coll)
	{
		if (coll.CompareTag("Player")) {
			currentState = RoutineState.PATROL;
			step = speed;
			followingPosition = waypoints [targetPoint].position;
			if (!counted)
			{
				GameManager.instance.AddScore(avoidanceBonus);
				counted = true;
			}
		}
		if(collided==0){
			step = speed;
		}
	}


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Enemy"))
        {
            ReverseWaypointOrder();
        }
        //Ante la colisión contra el jugador, el enemigo 
        //vuelve a su patrulla e ignora por un tiempo al jugador
        else if (other.collider.CompareTag("Player"))
        {
            Debug.Log(this.name + "colision con:" + other.collider.tag);
			if (currentState.Equals(RoutineState.PURSUIT))
			{
				AssignTargetToPreviousWaypoint();
				StartCoroutine(DelayedReturnalToPatrol());
				GameManager.instance.ReduceDeliveryHealth(deliveryDamage);
            }
        }
    }

    public void SetWaypoints (List<Transform> waypoints){
		this.waypoints = waypoints;
	}

	public void SetTargetPoint(int index){
		this.targetPoint = index;
	}


	//Metodo que hace que el enemigo invierta el orden en el que sigue su recorrido
	void ReverseWaypointOrder(){
		int newTarget = waypoints.Count - 1 - this.targetPoint;
		if(newTarget < 0)
			newTarget = 0;
		else if (newTarget > waypoints.Count -1)
			newTarget = waypoints.Count -1;
		this.targetPoint = newTarget;
		this.waypoints.Reverse();
	}

	void AssignTargetToPreviousWaypoint(){
		if(0 == this.targetPoint){
			this.targetPoint = waypoints.Count - 1;
		}
		else{
			this.targetPoint -= 1;
		}
	}

	IEnumerator DelayedReturnalToPatrol()
    {
		currentState = RoutineState.RETURNING;
        yield return new WaitForSeconds(0.2f);
        this.step = 0.0f;
		yield return new WaitForSeconds(3.0f);
		this.step = speed;
		currentState = RoutineState.PATROL;
    }
}
