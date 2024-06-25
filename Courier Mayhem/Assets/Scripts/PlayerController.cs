using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MainInput;


/** Script para definir el comportamiento del Heroe de Entregas dentro del juego
 * Incluyendo sus movimientos, interacciones, efectos de sonido asociados 
 * Esteban.Hernandez
 */
public class PlayerController : MonoBehaviour {

	//Sensibilidad para iniciar el movimiento
	public float xAxisThreshold = 0.02f;
	public float yAxisThreshold = 0.02f;
	//Velocidad de movimiento
	public Vector2 speed;

	[SerializeField]
	private Animator anim;
	[SerializeField]
	private Rigidbody2D mybody;
	[SerializeField]
	private Vector2 movement;
	//Asociacion con el mainInput manager para los controles
	public MainInputManager mainInput;
	public GameObject collisionFX;
	private bool moveSound = false;

	//Efectos de sonido para el jugador
	public AudioClip walk1;
	public AudioClip walk2;
	public AudioClip dieSound;
	public AudioClip dieExplosion;
	//Control del audio de caminada del personaje
	[SerializeField]
	private Vector2 lastStepPosition;
	public float deltaStep = 1f;


	// Use this for initialization
	void Awake () {
		mybody = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		movement = new Vector2(mybody.position.x, mybody.position.y);
		speed = new Vector2(0.2f,0.2f);
		lastStepPosition = mybody.position;
		collisionFX.SetActive (false);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		checkStep ();
		walk ();
	}


	/**Metodo para mover al personaje en un espacio XY*/
	private void walk(){


		Vector2 velocity = speed;
		float h = mainInput.horizontal;
		float l = mainInput.vertical;

		//Estado de Idle
		if (l > -yAxisThreshold && l < yAxisThreshold && h > -xAxisThreshold && h < xAxisThreshold) {
			anim.SetBool ("idle", true);
			anim.SetFloat ("ySpeed", 0);
			anim.SetFloat ("xSpeed", 0);
			mybody.velocity.Set(0f,0f);
			movement.x = mybody.position.x;
			movement.y = mybody.position.y;
			mybody.MovePosition (new Vector2 (movement.x, movement.y));

		}

		/*Normalizacion*/
		velocity = new Vector2(h,l);
		velocity = velocity.normalized;
		velocity.x = speed.x * velocity.x;
		velocity.y = speed.y * velocity.y;


		if (h > 0) {//derecha
			anim.SetBool ("idle", false);
			anim.SetFloat ("xSpeed", h);
			anim.SetFloat ("ySpeed", 0);
			movement.x = mybody.position.x + velocity.x;
		}

		else if (h < 0) {//izquierda
			anim.SetBool ("idle", false);
			anim.SetFloat ("xSpeed", h);
			anim.SetFloat ("ySpeed", 0);
			movement.x = mybody.position.x + velocity.x;
		}



		if (l > 0) {//arriba
			anim.SetBool ("idle", false);
			anim.SetFloat ("ySpeed", l);
			anim.SetFloat ("xSpeed", 0);
			movement.y = mybody.position.y + velocity.y;
		}

		else if (l < 0) {//abajo
			anim.SetBool ("idle", false);
			anim.SetFloat ("ySpeed", l);
			anim.SetFloat ("xSpeed", 0);
			movement.y = mybody.position.y + velocity.y;
		}


		if (l != 0 || h != 0) {
			mybody.MovePosition (new Vector2 (movement.x, movement.y));
			if (moveSound) {
				SoundManager.instance.RandomizePlayerFx (this.walk1, this.walk2);
				moveSound = false;
			}
		}
	
	}


	public void checkStep(){
		if (Vector2.Distance (mybody.position, lastStepPosition) >= deltaStep) { 
			moveSound = true;
			lastStepPosition = mybody.position;
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log(this.name + "colision con:" + coll.collider.tag);
        if (coll.collider.CompareTag("Enemy")) {
			StartCoroutine (ProcessCollisionEffect());
            StartCoroutine(ProcessDamage());
       }
	}

	IEnumerator ProcessCollisionEffect()
    {
        collisionFX.SetActive(true);
        yield return new WaitForSeconds (1f);
		collisionFX.SetActive (false);
	}

	IEnumerator ProcessDamage(){
		anim.SetBool ("isDamaged", true);
		SoundManager.instance.PlayOnce (this.dieSound);
		yield return new WaitForSeconds (0.5f);
		anim.SetBool("isDamaged", false);
	}
}
