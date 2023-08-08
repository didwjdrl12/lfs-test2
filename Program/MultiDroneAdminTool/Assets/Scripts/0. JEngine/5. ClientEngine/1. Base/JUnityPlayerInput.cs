using System;
using System.Collections.Generic;
using UnityEngine;

namespace J2y
{
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
	//
	// JUnityPlayerInput
	//
	//
	//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

	public class JUnityPlayerInput : JPlayerInput
	{
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// 변수
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


		#region [변수] Base


		#endregion


		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Input
		//
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

		#region [Input] Button 
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public override bool GetButton(string name) { return Input.GetButton(name); }
		public override bool GetButtonDown(string name) { return Input.GetButtonDown(name); }
		public override bool GetButtonUp(string name) { return Input.GetButtonUp(name); }
		//public override bool GetDoublePress(string name) { return Input.GetDoublePress(name); }
		#endregion
	
		#region [Input] Axis
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public override float GetAxis(string name) { return Input.GetAxis(name); }
		public override float GetAxisRaw(string name) { return Input.GetAxisRaw(name); }
		#endregion

		#region [Input] Mouse
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public override Vector2 GetMousePosition() { return Vector2.zero; }
		#endregion

	
		#region [CanInteract] Item
		//------------------------------------------------------------------------------------------------------------------------------------------------------
		public bool CanInteractItem()
		{
			return true;
		}
		#endregion



    }

}
