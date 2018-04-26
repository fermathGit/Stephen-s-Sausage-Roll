﻿
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class WaterReflection : MonoBehaviour 
{
	public static int ReflectionSetting = 1; 

	//public static WaterReflection self = null;
	public static HashSet<WaterReflection> InstanceSets;

	public enum Quality
	{
		Fastest, 		// no-reflection
		Medium,			// Some thing reflection with 512 x 512 texture
		High			// All thing reflection with 512 x 512 texture
	}

	public Color BackgroundColor;
	public LayerMask  MediumReflectionMask;

	public LayerMask  HighReflectionMask;

	public Vector3 PlanePos = new Vector3(0, 97, 0);
	public Vector3 PlaneNormal = Vector3.up;
	public float Height { get { return PlanePos.y; } set { PlanePos.y = value; }}

	//private Dictionary<Camera, Camera>  m_HelperCameras = new Dictionary<Camera, Camera>();
	private RenderTexture m_Texture;
	private Camera m_ReflectionCam = null;

	// Water ChunkInRange in the camera sight. invoke by WaterDistanceDetect.cs
	[HideInInspector]
	public bool WaterChunkInRange = false;

	public static bool WaterChunkRenderVisible = true;
	private bool m_WaterInRange;

	private Camera m_CurCam;
	public Camera CurCam	 { get {return m_CurCam; } }
	
	//private Camera GetReflectionCamera(Camera current, Material mat, int textureSize)
	private Camera GetReflectionCamera(Camera current, int textureSize)
	{
		if (m_Texture == null)
		{
			m_Texture = new RenderTexture(textureSize, textureSize, 16);
			m_Texture.name = "_ReflectionTex" + GetInstanceID();
			m_Texture.isPowerOfTwo = true;
			m_Texture.hideFlags = HideFlags.DontSave;
		}

//		if (!m_HelperCameras.ContainsKey(current))
//			m_HelperCameras.Add(current, null);

//		Camera cam = null;

//		if (!m_HelperCameras.ContainsKey(current))
//		{
//			GameObject go = new GameObject("Mirror Refl Camera id" + GetInstanceID() + " for " + current.GetInstanceID(), typeof(Camera), typeof(Skybox));
//			//go.hideFlags = HideFlags.HideAndDontSave;
//
//			cam = go.camera;
//			cam.enabled = false;
//
//			Transform t = cam.transform;
//			t.position  = transform.position;
//			t.rotation  = transform.rotation;
//
//			m_HelperCameras[current] = cam;
//			//m_HelperCameras.Add(current, cam);
//		}
//
//		cam = m_HelperCameras[current];

		if (m_CurCam != current)
		{
			if (m_ReflectionCam != null)
				Destroy(m_ReflectionCam.gameObject);
			m_ReflectionCam = null;

			m_CurCam = current;
		}

		if (m_ReflectionCam == null)
		{
			GameObject go = new GameObject("Mirror Refl Camera id" + GetInstanceID() + " for " + current.GetInstanceID(), typeof(Camera), typeof(Skybox));
			go.hideFlags = HideFlags.HideAndDontSave;

			m_ReflectionCam = go.GetComponent<Camera>();
			m_ReflectionCam.enabled = false; 

			Transform t = m_ReflectionCam.transform;
			t.position  = transform.position;
			t.rotation  = transform.rotation;

		}

//		if (VFVoxelWater.self != null && VFVoxelWater.self.WaterMat)
//		{
//			VFVoxelWater.self.WaterMat.SetTexture("_ReflectionTex", m_Texture);
//		}

		return m_ReflectionCam;
	}

	private void CopyCameraSetting (Camera src, Camera dest)
	{
		if (src.clearFlags == CameraClearFlags.Skybox)
		{
			Skybox sky = src.GetComponent<Skybox>();
			Skybox mysky = dest.GetComponent<Skybox>();

			if (!sky || !sky.material)
			{
				mysky.enabled = false;
			}
			else
			{
				mysky.enabled = true;
				mysky.material = sky.material;
			}
		}

		dest.clearFlags = src.clearFlags;
		dest.backgroundColor = BackgroundColor;
		dest.farClipPlane = src.farClipPlane;
		dest.nearClipPlane = src.nearClipPlane;
		dest.orthographic = src.orthographic;
		dest.fieldOfView = src.fieldOfView;
		dest.aspect = src.aspect;
		dest.orthographicSize = src.orthographicSize;
		dest.depthTextureMode = DepthTextureMode.Depth;
		dest.renderingPath = RenderingPath.Forward;
		//dest.renderingPath = RenderingPath.DeferredLighting;
	}

	public static Matrix4x4 CalculateObliqueMatrix (Matrix4x4 projection, Vector4 clipPlane) 
	{
		Vector4 q = projection.inverse * new Vector4(
			sgn(clipPlane.x),
			sgn(clipPlane.y),
			1.0F,
			1.0F
			);
		Vector4 c = clipPlane * (2.0F / (Vector4.Dot (clipPlane, q)));
		// third row = clip plane - fourth row
		projection[2] = c.x - projection[3];
		projection[6] = c.y - projection[7];
		projection[10] = c.z - projection[11];
		projection[14] = c.w - projection[15];
		
		return projection;
	}	

	public static Matrix4x4 CalculateReflectionMatrix (Matrix4x4 reflectionMat, Vector4 plane) 
	{
		reflectionMat.m00 = (1.0F - 2.0F*plane[0]*plane[0]);
		reflectionMat.m01 = (   - 2.0F*plane[0]*plane[1]);
		reflectionMat.m02 = (   - 2.0F*plane[0]*plane[2]);
		reflectionMat.m03 = (   - 2.0F*plane[3]*plane[0]);
		
		reflectionMat.m10 = (   - 2.0F*plane[1]*plane[0]);
		reflectionMat.m11 = (1.0F - 2.0F*plane[1]*plane[1]);
		reflectionMat.m12 = (   - 2.0F*plane[1]*plane[2]);
		reflectionMat.m13 = (   - 2.0F*plane[3]*plane[1]);
		
		reflectionMat.m20 = (   - 2.0F*plane[2]*plane[0]);
		reflectionMat.m21 = (   - 2.0F*plane[2]*plane[1]);
		reflectionMat.m22 = (1.0F - 2.0F*plane[2]*plane[2]);
		reflectionMat.m23 = (   - 2.0F*plane[3]*plane[2]);
		
		reflectionMat.m30 = 0.0F;
		reflectionMat.m31 = 0.0F;
		reflectionMat.m32 = 0.0F;
		reflectionMat.m33 = 1.0F;
		
		return reflectionMat;
	}

	static float sgn (float a) 
	{
		if (a > 0.0F) return 1.0F;
		if (a < 0.0F) return -1.0F;
		return 0.0F;
	}	
	
	private Vector4 CameraSpacePlane (Camera cam, Vector3 pos, Vector3 normal, float sideSign) 
	{
		Vector3 offsetPos = pos ;
		Matrix4x4 m = cam.worldToCameraMatrix;
		Vector3 cpos = m.MultiplyPoint (offsetPos);
		Vector3 cnormal = m.MultiplyVector (normal).normalized * sideSign;
		
		return new Vector4 (cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot (cpos,cnormal));
	}

	private void Clear()
	{
		if (m_Texture != null)
		{
			DestroyImmediate(m_Texture);
			m_Texture = null;
		}

		//renderer.sharedMaterial.SetTexture("_ReflectionTex", null);
//		if (VFVoxelWater.self != null && VFVoxelWater.self.WaterMat)
//		{
//			VFVoxelWater.self.WaterMat.SetTexture("_ReflectionTex", null);
//		}
	}

	void Awake()
	{
		if (InstanceSets == null)
			InstanceSets = new HashSet<WaterReflection>();

		InstanceSets.Add(this);

	}

	// Use this for initialization
	void Start () 
	{
		m_WaterInRange = false;

		//Height = VFVoxelWater.c_fWaterLvl;
	}

	void Update ()
	{
//		if (VFVoxelWater.self != null && VFVoxelWater.self.WaterMat)
//		{
//			VFVoxelWater.self.WaterMat.SetTexture("_WaveMap", PEWaveSystem.Self.Target); 
//		}
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
//		if (0 == (Time.frameCount&63))
//			StartCoroutine(UpdateShader());
//		WaterChunkInRange = false;

		if(ReflectionSetting == 2)
		{
			m_WaterInRange = true;
		}
		else if(ReflectionSetting == 0)
		{
			m_WaterInRange = false;
		}
		else
		{
			m_WaterInRange = WaterChunkInRange;
		}
		
	}
	
	
	void OnDestroy()
	{
		Clear();
//		foreach (Camera cam in m_HelperCameras.Values)
//		{
//			if (cam != null)
//			{
//				DestroyImmediate(cam.gameObject);
//			}
//		}
//
//		m_HelperCameras.Clear();

		if (m_ReflectionCam != null)
			DestroyImmediate(m_ReflectionCam.gameObject);

		//InstanceSets.Remove(this);
	}

	void OnDisable()
	{
//		foreach (Camera cam in m_HelperCameras.Values)
//		{
//			if (cam != null)
//			{
//				DestroyImmediate(cam.gameObject);
//			}
//		}
//		
//		m_HelperCameras.Clear();
		if (m_ReflectionCam != null)
			DestroyImmediate(m_ReflectionCam.gameObject);
	}

	/// <summary>
	/// Called when the object is being renderered.
	/// </summary>
	
	void OnWillRenderObject ()
	{
		
	}

	void Disable()
	{

	}

	void OnPreRender ()
	{
		// Detect
		if(ReflectionSetting == 2)
		{
//			if (VFVoxelWater.s_chunkCnt > 0)
//				m_WaterInRange = true;
//			else
//				m_WaterInRange = false;

//			if (m_WaterInRange)
//			{
//				m_WaterInRange = WaterChunkInRange;
//			}

		}
		else if(ReflectionSetting == 0)
		{
			m_WaterInRange = false;
		}
		else
		{
			m_WaterInRange = WaterChunkInRange;
		}

		Camera cam =  Camera.current;
		if (cam == null)
		{
			Clear();
			return;
		}
		if (!enabled )
		{
			Clear();
			return;
		}
		
		LayerMask mask = HighReflectionMask;
		if (mask == 0 )
		{
			Clear();
			return;
		}

		Camera reflectCamera = GetReflectionCamera(cam, 512);

		if (!m_WaterInRange)
		{
			//Clear();
//			if (VFVoxelWater.self != null && VFVoxelWater.self.WaterMat)
//			{
//				VFVoxelWater.self.WaterMat.SetTexture("_ReflectionTex", null);
//			}
			return;
		}
		
		
		bool imageEffects = SystemInfo.supportsImageEffects;
		if (imageEffects) cam.depthTextureMode |= DepthTextureMode.Depth;
		
		//Material mat = renderer.sharedMaterial;
		
		//Camera reflectCamera = GetReflectionCamera(cam, mat, 512);
		
		Vector3 pos = PlanePos;//transform.position; 
		Vector3 normal = PlaneNormal ;//transform.up;
		
		CopyCameraSetting(cam, reflectCamera);
		
		float d = -Vector3.Dot(normal, pos);
		Vector4 reflectPlane = new Vector4(normal.x, normal.y, normal.z, d);
		
		Matrix4x4 rMatrix = Matrix4x4.zero;
		rMatrix = CalculateReflectionMatrix(rMatrix, reflectPlane);
		
		Vector3 oldpos = cam.transform.position;
		Vector3 newPos = rMatrix.MultiplyPoint(oldpos);
		reflectCamera.worldToCameraMatrix = cam.worldToCameraMatrix * rMatrix;
		
		Vector4 clipPlane = CameraSpacePlane(reflectCamera, pos, normal, 1.0f);
		Matrix4x4 pMatrix = Matrix4x4.zero;
		pMatrix = CalculateObliqueMatrix(cam.projectionMatrix, clipPlane);
		reflectCamera.projectionMatrix = pMatrix;
		reflectCamera.cullingMask = ~(1 << 4) & mask.value;
		reflectCamera.targetTexture = m_Texture; 
		
		GL.invertCulling = true;
		reflectCamera.transform.position = newPos;
		Vector3 euler = cam.transform.eulerAngles;
		reflectCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);	
		reflectCamera.Render();
		reflectCamera.transform.position = oldpos;
        GL.invertCulling = false;

    }
	
}
