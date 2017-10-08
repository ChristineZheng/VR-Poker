using System;
using UnityEngine;

namespace VRStandardAssets.Utils
{
	// In order to interact with objects in the scene
	// this class casts a ray into the scene and if it finds
	// a VRInteractiveItem it exposes it for other classes to use.
	// This script should be generally be placed on the camera.
	public class VREyeRaycaster : MonoBehaviour
	{
		public event Action<RaycastHit> OnRaycasthit;                   // This event is called every frame that the user's gaze is over a collider.


		[SerializeField] private Transform m_Camera;
		[SerializeField] private LayerMask m_ExclusionLayers;           // Layers to exclude from the raycast.
		//[SerializeField] private Reticle m_Reticle;                     // The reticle, if applicable.
		[SerializeField] private VRInput m_VrInput;                     // Used to call input based events on the current VRInteractiveItem.
		[SerializeField] private bool m_ShowDebugRay;                   // Optionally show the debug ray.
		[SerializeField] private float m_DebugRayLength = 5f;           // Debug ray length.
		[SerializeField] private float m_DebugRayDuration = 1f;         // How long the Debug ray will remain visible.
		[SerializeField] private float m_RayLength = 500f;              // How far into the scene the ray is cast.


//		[SerializeField] private float m_DebugRayLength = 5f;           // Debug ray length.
//		[SerializeField] private float m_DebugRayDuration = 1f;         // How long the Debug ray will remain visible.
//		[SerializeField] private float m_RayLength = 500f;              // How far into the scene the ray is cast.

		private VRInteractiveItem m_CurrentInteractible;                //The current interactive item
		private VRInteractiveItem m_LastInteractible;                   //The last interactive item


		// Utility for other classes to get the current interactive item
		public VRInteractiveItem CurrentInteractible
		{
			get { return m_CurrentInteractible; }
		}


		private void OnEnable()
		{
			m_VrInput.OnClick += HandleClick;
			m_VrInput.OnDoubleClick += HandleDoubleClick;
			m_VrInput.OnUp += HandleUp;
			m_VrInput.OnDown += HandleDown;
		}


		private void OnDisable ()
		{
			m_VrInput.OnClick -= HandleClick;
			m_VrInput.OnDoubleClick -= HandleDoubleClick;
			m_VrInput.OnUp -= HandleUp;
			m_VrInput.OnDown -= HandleDown;
		}


		private void Update()
		{
			EyeRaycast();
		}


		private void EyeRaycast()
		{
			// Show the debug ray if required
			if (m_ShowDebugRay)
			{	
				Debug.DrawRay(m_Camera.position, m_Camera.forward * m_DebugRayLength, Color.blue, m_DebugRayDuration);
			}

			// Create a ray that points forwards from the camera.
			Ray ray = new Ray(m_Camera.position, m_Camera.forward);
			RaycastHit hit;

			// Do the raycast forweards to see if we hit an interactive item
			if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
			{
				VRInteractiveItem interactible = hit.collider.GetComponent<VRInteractiveItem>(); //attempt to get the VRInteractiveItem on the hit object
				m_CurrentInteractible = interactible;

				// If we hit an interactive item and it's not the same as the last interactive item, then call Over
				if (interactible && interactible != m_LastInteractible)
					interactible.Over(); 
					interactible.transform.LookAt (Vector3.back);
					
					//Vector3 pos = interactible.transform.position;
					//Debug.Log ("vector3: " + pos);
					//FlipCard (interactible);
				
//					Vector3 pos = interactible.transform.position;
//					// card face camera
//					if ((-0.5f <= pos.x <= 0.5f) && (-0.5f <= pos.y <= 0.5f) && (-0.5f <= pos.z <= 0.5f)) {
//						interactible.transform.LookAt (Vector3.back);
//					}
//					// card's back face camera
//					if (pos.x == 0 && pos.y == 0 && pos.z == -1) {
//						interactible.transform.LookAt (Vector3.zero);
//					}
					
					Debug.Log ("rotate from VREyecaster");

				// Deactive the last interactive item 
				if (interactible != m_LastInteractible)
					DeactiveLastInteractible();

				m_LastInteractible = interactible;

//				// Something was hit, set at the hit position.
//				if (m_Reticle)
//					m_Reticle.SetPosition(hit);

				if (OnRaycasthit != null)
					OnRaycasthit(hit);
			}
			else
			{
				// Nothing was hit, deactive the last interactive item.
				DeactiveLastInteractible();
				m_CurrentInteractible = null;

//				// Position the reticle at default distance.
//				if (m_Reticle)
//					m_Reticle.SetPosition();
			}
		}

		private void FlipCard(VRInteractiveItem item){
			Vector3 pos = item.transform.position;
			// card face camera
			//if ((-0.5f <= pos.x <= 0.5f) && (-0.5f <= pos.y <= 0.5f) && (-0.5f <= pos.z <= 0.5f)) {
			//if (Mathf.Approximately(0f, pos.x) && Mathf.Approximately(0f, pos.y) && Mathf.Approximately(0f, pos.z)) {
			if ((-0.5f <= pos.x) && (pos.x <= 0.5f) && (-0.5f <= pos.y) && (pos.y <= 0.5f) && (-0.5f <= pos.z) && (pos.z <= 0.5f)) {
				item.transform.LookAt (Vector3.back);
				Debug.Log ("face back");
			}
			// card's back face camera
			//if (Mathf.Approximately(0f, pos.x) && Mathf.Approximately(0f, pos.y) && Mathf.Approximately(-1f, pos.z)) {
			if ((-0.5f <= pos.x) && (pos.x <= 0.5f) && (-0.5f <= pos.y) && (pos.y <= 0.5f) && (-1.5f <= pos.z) && (pos.z <= 0.5f)) {
				item.transform.LookAt (Vector3.zero);
				Debug.Log ("face front");
			}
		}


		private void DeactiveLastInteractible()
		{
			if (m_LastInteractible == null)
				return;

			m_LastInteractible.Out();
			m_LastInteractible = null;
		}


		private void HandleUp()
		{
			if (m_CurrentInteractible != null)
				m_CurrentInteractible.Up();
				//m_CurrentInteractible.transform.LookAt (Vector3.back);
		}


		private void HandleDown()
		{
			if (m_CurrentInteractible != null)
				m_CurrentInteractible.Down();
				FlipCard (m_CurrentInteractible);
//				m_CurrentInteractible.transform.LookAt (Vector3.back);
//				Debug.Log ("down detected");
		}


		private void HandleClick()
		{
			if (m_CurrentInteractible != null)
				m_CurrentInteractible.Click();
				FlipCard (m_CurrentInteractible);
//				m_CurrentInteractible.transform.LookAt (Vector3.back);
//				Debug.Log ("click detected");
		}


		private void HandleDoubleClick()
		{
			if (m_CurrentInteractible != null)
				m_CurrentInteractible.DoubleClick();
				FlipCard (m_CurrentInteractible);
//				m_CurrentInteractible.transform.LookAt (Vector3.back);
//				Debug.Log ("double click detected");

		}
	}
}