using UnityEngine;


/** Script para centralizar el manejo de inputs y sus valores. 
 * Utilizado para habilitar mas de un input.
 * Esteban.Hernandez
 */
namespace MainInput
{
	public class MainInputManager : MonoBehaviour {


		public float horizontal;
		public float vertical;
		public bool leftDown;
		public bool rightDown;
		public bool upDown;
		public bool downDown;



		public bool startBttnDown;
		public bool selectBttnDown;
		public bool button_ADown;
		public bool button_BDown;


		/*Detección de presionado*/
		public bool leftPressed;
		public bool rightPressed;
		public bool startBttnPressed;
		public bool selectBttnPressed;
		public bool button_APressed;
		public bool button_BPressed;

		public bool leftUp;
		public bool rightUp;
		public bool startBttnUp;
		public bool selectBttnUp;
		public bool button_AUp;
		public bool button_BUp;

		public bool playable;
		public bool pc; // Plataforma PC


		void Update () {
			if (playable && pc) {//Inputs de teclado

				horizontal = Input.GetAxisRaw ("Horizontal");
				vertical = Input.GetAxisRaw ("Vertical");

				startBttnDown = Input.GetButtonDown ("Dodge");
				selectBttnDown = Input.GetButtonDown ("Interact");
				button_ADown = Input.GetButtonDown ("Pause");

				startBttnUp = Input.GetButtonUp ("Dodge");
				selectBttnUp = Input.GetButtonUp ("Interact");
				button_AUp = Input.GetButtonUp ("Pause");

				startBttnPressed = Input.GetButton ("Dodge");
				selectBttnPressed = Input.GetButton ("Interact");
				button_APressed = Input.GetButton ("Pause");


			}
			if (playable && !pc) {//Inputs de botones de pantalla
				if (rightDown) {
					horizontal = 1;
				}
				if (leftDown) {
					horizontal = -1;
				}

				if (!rightDown && !leftDown) {
					horizontal = 0;
				}


				if (upDown) {
					vertical = 1;
				}
				if (downDown) {
					vertical = -1;
				}

				if (!upDown && !downDown) {
					vertical = 0;
				}
				
			}
		}

		public void pressRight(bool pressed){
			rightDown = pressed;
		}

		public void pressLeft(bool pressed){
			leftDown = pressed;
		}

		public void pressDown(bool pressed){
			downDown = pressed;
		}

		public void pressUp(bool pressed){
			upDown = pressed;
		}



	}
}
