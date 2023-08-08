using System;
using System.Collections.Generic;
using UnityEngine;

namespace J2y
{
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// JPlayerInput
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public abstract class JPlayerInput : JComponent
	{
		public virtual bool GetButton(string name) { return false; }
		public virtual bool GetButtonDown(string name) { return false; }
		public virtual bool GetButtonUp(string name) { return false; }

		public virtual bool GetDoublePress(string name) { return false; }
		public virtual float GetAxis(string name) { return 0; }

		public virtual float GetAxisRaw(string name) { return 0; }
		public bool IsControllerConnected() { return UnityEngine.Input.GetJoystickNames().Length > 0; }
		public virtual Vector2 GetMousePosition() { return Vector2.zero; }
	}

}
