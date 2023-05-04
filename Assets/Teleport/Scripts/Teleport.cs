//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Handles all the teleport logic
//
//=============================================================================

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public class Teleport : MonoBehaviour
	{


		public LayerMask traceLayerMask;
		public LayerMask floorFixupTraceLayerMask;
		public float floorFixupMaximumTraceDistance = 1.0f;
		public Material areaVisibleMaterial;
		public Material areaLockedMaterial;
		public Material areaHighlightedMaterial;
		public Material pointVisibleMaterial;
		public Material pointLockedMaterial;
		public Material pointHighlightedMaterial;
		public Transform destinationReticleTransform;
		public Transform invalidReticleTransform;
		public GameObject playAreaPreviewCorner;
		public GameObject playAreaPreviewSide;
		public Color pointerValidColor;
		public Color pointerInvalidColor;
		public Color pointerLockedColor;
		public bool showPlayAreaMarker = true;

		public float teleportFadeTime = 0.1f;
		public float meshFadeTime = 0.2f;

		public float arcDistance = 10.0f;

		[Header("Effects")]
		public Transform onActivateObjectTransform;
		public Transform onDeactivateObjectTransform;
		public float activateObjectTime = 1.0f;
		public float deactivateObjectTime = 1.0f;

		[Header("Audio Sources")]
		public AudioSource pointerAudioSource;
		public AudioSource loopingAudioSource;
		public AudioSource headAudioSource;
		public AudioSource reticleAudioSource;

		[Header("Sounds")]
		public AudioClip teleportSound;
		public AudioClip pointerStartSound;
		public AudioClip pointerLoopSound;
		public AudioClip pointerStopSound;
		public AudioClip goodHighlightSound;
		public AudioClip badHighlightSound;

		[Header("Debug")]
		public bool debugFloor = false;
		public bool showOffsetReticle = false;
		public Transform offsetReticleTransform;
		public MeshRenderer floorDebugSphere;
		public LineRenderer floorDebugLine;

		private LineRenderer pointerLineRenderer;
		private GameObject teleportPointerObject;
		private Transform pointerStartTransform;

		private TeleportArc teleportArc = null;

		/*
		private bool visible = false;


		private Vector3 pointedAtPosition;
		private Vector3 prevPointedAtPosition;
		private bool teleporting = false;
		private float currentFadeTime = 0.0f;

		private float meshAlphaPercent = 1.0f;
		private float pointerShowStartTime = 0.0f;
		private float pointerHideStartTime = 0.0f;
		private bool meshFading = false;
		private float fullTintAlpha;

		private float invalidReticleMinScale = 0.2f;
		private float invalidReticleMaxScale = 1.0f;
		private float invalidReticleMinScaleDistance = 0.4f;
		private float invalidReticleMaxScaleDistance = 2.0f;
		private Vector3 invalidReticleScale = Vector3.one;
		private Quaternion invalidReticleTargetRotation = Quaternion.identity;

		private Transform playAreaPreviewTransform;
		private Transform[] playAreaPreviewCorners;
		private Transform[] playAreaPreviewSides;
		*/	
		/*
		private float loopingAudioMaxVolume = 0.0f;

		private Coroutine hintCoroutine = null;

		private bool originalHoverLockState = false;


		private Vector3 startingFeetOffset = Vector3.zero;
		private bool movedFeetFarEnough = false;
		*/
		public GameObject markerball;


		//-------------------------------------------------
		private static Teleport _instance;
		public static Teleport instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GameObject.FindObjectOfType<Teleport>();
				}

				return _instance;
			}
		}


		//-------------------------------------------------
		void Awake()
		{
			_instance = this;


			pointerLineRenderer = GetComponentInChildren<LineRenderer>();
			teleportPointerObject = pointerLineRenderer.gameObject;

#if UNITY_URP
			fullTintAlpha = 0.5f;
#else
			int tintColorID = Shader.PropertyToID("_TintColor");
			//fullTintAlpha = pointVisibleMaterial.GetColor(tintColorID).a;
#endif

			teleportArc = GetComponent<TeleportArc>();
			teleportArc.traceLayerMask = traceLayerMask;

			//loopingAudioMaxVolume = loopingAudioSource.volume;

			playAreaPreviewCorner.SetActive(false);
			playAreaPreviewSide.SetActive(false);

			float invalidReticleStartingScale = invalidReticleTransform.localScale.x;
			//invalidReticleMinScale *= invalidReticleStartingScale;
			//invalidReticleMaxScale *= invalidReticleStartingScale;
		}


		//-------------------------------------------------
		void Start()
		{


			Invoke("ShowTeleportHint", 5.0f);
		}












		//-------------------------------------------------
		private void UpdatePointer()
		{
			Vector3 pointerStart = pointerStartTransform.position;
			Vector3 pointerEnd = markerball.transform.position;
			Vector3 pointerDir = pointerStartTransform.forward;

			Vector3 arcVelocity = pointerDir * arcDistance;



			//Trace to see if the pointer hit anything
			//RaycastHit hitInfo;
			teleportArc.SetArcData();




			pointerLineRenderer.SetPosition(0, pointerStart);
			pointerLineRenderer.SetPosition(1, pointerEnd);
		}
	}
}







		
