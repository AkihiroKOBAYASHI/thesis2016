using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
	[RequireComponent(typeof (MyCarController))]
	public class MyCarUserControl : MonoBehaviour
	{

		[SerializeField] private bool isKeyboardMode = false;

		private MyCarController m_Car;
		private bool enableToPush = true;
		private float timeForPermitPush = 1f;

		private bool EnableToPush {
			get {
				return enableToPush;
			}
			set {
				enableToPush = value;
			}
		}

		private void Awake()
		{
			// get the car controller
			m_Car = GetComponent<MyCarController>();
		}

		private void FixedUpdate() {
			// lr
			float h;
			if (isKeyboardMode) {
				h = CrossPlatformInputManager.GetAxis ("Horizontal");
			} else {
				h = Input.GetAxis("Handle");
			}

			// straight
			float v;
			if (isKeyboardMode) {
				v = CrossPlatformInputManager.GetAxis ("Vertical");
			} else {
				v = Input.GetAxis("Accel") * (-1f);
				v = (v  + 1f) * 0.5f;
			}

			// brake
			float s;
			if (isKeyboardMode) {
				s = CrossPlatformInputManager.GetAxis("Space");
			} else {
				s = Input.GetAxis("Brake") * (-1f);
				s = (s  + 1f) * 0.5f;
			}

			// back = backtrigger + straight
			float b = Input.GetAxis("BackTrigger");

			// decide
			bool d = CrossPlatformInputManager.GetButtonDown("Decide");

			if(s > 0) {
				m_Car.Move(h, 0, 0, s); // stop
			}
			else {
				if (v == 0) {
					m_Car.Move (h, 0, 0, 0); // do nothing
				} else {
					if (b == 0) {
						m_Car.Move (h, v, 0, 0); // go
					} else {
						m_Car.Move (h, 0, (-1) * v, 0); // back
					}
				}
			}

			if (d && EnableToPush) {
				EnableToPush = false;
				GameController.instance.ChangeGameScene ();
				StartCoroutine(PreventSuccessionPush ());
			}
		}

		private IEnumerator PreventSuccessionPush() {
			yield return new WaitForSeconds(timeForPermitPush);
			EnableToPush = true;
		}
	}
}
