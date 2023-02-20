using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PortalRender : MonoBehaviour
{
	public float renderQuality = 1;
	public bool enableObliqueProjection = false;
	public LayerMask alwaysVisibleMask;

	private RenderTexture leftTexture;
	private RenderTexture rightTexture;
	private Camera mainCamera;
	private Camera renderCam;

	private Skybox camSkybox;
	private Renderer meshRenderer;

	private Vector3 leftEyePosition;
	private Vector3 rightEyePosition;

	private Quaternion leftEyeRotation;
	private Quaternion rightEyeRotation;

	private List<XRNodeState> nodeStatesCache = new List<XRNodeState>();
	private float portalSwitchDistance = 0.03f;
	private bool triggerZDirection;

	// Start is called before the first frame update
	void Start()
    {
		mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	/*
	void OnWillRenderObject()
	{
		// Create the textures and camera if they don't exist.
		if (!leftTexture)
		{
			Vector2 texSize = new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight);
			leftTexture = new RenderTexture((int)(texSize.x * renderQuality), (int)(texSize.y * renderQuality), 16);

			rightTexture = new RenderTexture ((int)(texSize.x * renderQuality), (int)(texSize.y * renderQuality), 16);

			renderCam = new GameObject("render camera", typeof(Camera), typeof(Skybox)).GetComponent<Camera>();

			renderCam.name = "render camera";
			renderCam.tag = "Untagged";

			if (renderCam.GetComponent<Skybox>())
			{
				camSkybox = renderCam.GetComponent<Skybox>();
			}
			else
			{
				renderCam.gameObject.AddComponent<Skybox>();
				camSkybox = renderCam.GetComponent<Skybox>();
			}

			CameraExtensions.ClearCameraComponents(renderCam.GetComponent<Camera>());

			renderCam.hideFlags = HideFlags.HideInHierarchy;
			renderCam.enabled = false;
		}

		meshRenderer.material.SetFloat("_RecursiveRender", (gameObject.layer != Camera.current.gameObject.layer) ? 1 : 0);
		RenderSteamVR(Camera.current);
	}

	
	private void RenderSteamVR(Camera camera)
	{
		if (camera.stereoTargetEye == StereoTargetEyeMask.Both || camera.stereoTargetEye == StereoTargetEyeMask.Left) 
		{
			InputTracking.GetNodeStates(nodeStatesCache);

			Vector3 eyePos = Vector3.zero;
			Quaternion eyeRot = Quaternion.identity;

			for (int i = 0; i < nodeStatesCache.Count; i++)
			{
				XRNodeState nodeState = nodeStatesCache[i];
				if (nodeState.nodeType == XRNode.LeftEye)
				{
					if (nodeState.TryGetPosition(out leftEyePosition))
                    {
						eyePos = camera.transform.TransformPoint(leftEyePosition);
					}
                    else
                    {
						print("can't find left eye position");
                    }

                    if (nodeState.TryGetRotation(out leftEyeRotation))
                    {
						eyeRot = camera.transform.rotation * leftEyeRotation;
					}
					else
					{
						print("can't find left eye rotation");
					}
				}
			}

			Matrix4x4 projectionMatrix = camera.GetStereoProjectionMatrix(Camera.StereoscopicEye eye);
				GetSteamVRProjectionMatrix (camera, Valve.VR.EVREye.Eye_Left);
			RenderTexture target = leftTexture;

			RenderPlane (renderCam, target, eyePos, eyeRot, projectionMatrix);
			meshRenderer.material.SetTexture ("_LeftTex", target);
		}

		if (camera.stereoTargetEye == StereoTargetEyeMask.Both || camera.stereoTargetEye == StereoTargetEyeMask.Right) {
			Vector3 eyePos = camera.transform.TransformPoint (SteamVR.instance.eyes [1].pos);
			Quaternion eyeRot = camera.transform.rotation * SteamVR.instance.eyes [1].rot;
			Matrix4x4 projectionMatrix = GetSteamVRProjectionMatrix (camera, Valve.VR.EVREye.Eye_Right);
			RenderTexture target = rightTexture;

			RenderPlane (renderCam, target, eyePos, eyeRot, projectionMatrix);
			meshRenderer.material.SetTexture ("_RightTex", target);
		}
	}
	*/

	protected void RenderPlane(Camera portalCamera, RenderTexture targetTexture, Vector3 camPosition, Quaternion camRotation, Matrix4x4 camProjectionMatrix)
	{
		// Copy camera position/rotation/projection data into the reflectionCamera
		portalCamera.transform.position = camPosition;
		portalCamera.transform.rotation = camRotation;
		portalCamera.targetTexture = targetTexture;
		portalCamera.ResetWorldToCameraMatrix();

		// Change the project matrix to use oblique culling (only show things BEHIND the portal)
		Vector3 pos = transform.position;
		Vector3 normal = transform.forward;
		bool isForward = transform.InverseTransformPoint(portalCamera.transform.position).z < 0;
		Vector4 clipPlane = CameraSpacePlane(portalCamera, pos, normal, isForward ? 1.0f : -1.0f);
		Matrix4x4 projection = camProjectionMatrix;
		if (this.enableObliqueProjection)
		{
			CalculateObliqueMatrix(ref projection, clipPlane);
		}
		portalCamera.projectionMatrix = projection;

		// Hide the other dimensions
		portalCamera.enabled = false;
		portalCamera.cullingMask = 0;

		//CameraExtensions.LayerCullingShow(portalCamera, ToDimension().layer);
		//CameraExtensions.LayerCullingShowMask(portalCamera, alwaysVisibleMask);

		// Update values that are used to generate the Skybox and whatnot.
		portalCamera.farClipPlane = mainCamera.farClipPlane;
		portalCamera.nearClipPlane = mainCamera.nearClipPlane;
		portalCamera.orthographic = mainCamera.orthographic;
		portalCamera.fieldOfView = mainCamera.fieldOfView;
		portalCamera.aspect = mainCamera.aspect;
		portalCamera.orthographicSize = mainCamera.orthographicSize;

		portalCamera.Render();

		portalCamera.targetTexture = null;
	}

	// Creates a clip plane for the projection matrix that clips to the portal.
	private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
	{
		Vector4 q = projection.inverse * new Vector4(
			sgn(clipPlane.x),
			sgn(clipPlane.y),
			1.0f,
			1.0f
		);
		Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));

		// third row = clip plane - fourth row
		projection[2] = c.x - projection[3];
		projection[6] = c.y - projection[7];
		projection[10] = c.z - projection[11];
		projection[14] = c.w - projection[15];
	}

	// Given position/normal of the plane, calculates plane in camera space.
	private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	{
		Vector3 offsetPos = pos + normal * portalSwitchDistance * (triggerZDirection ? -1 : 1);
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint(offsetPos);
		Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
		return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
	}

	// Extended sign: returns -1, 0 or 1 based on sign of a
	private static float sgn(float a)
	{
		if (a > 0.0f) return 1.0f;
		if (a < 0.0f) return -1.0f;
		return 0.0f;
	}
}
