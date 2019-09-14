// Optimize and log mobile settings
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;

public enum LogType
{
	Lighting,
	Camera,
	Terrain,
	Build,
	Material
}
public class MobileOptimizer : EditorWindow {

	// Add menu named "My Window" to the Window menu
	[MenuItem("Window/Mobile Optimizer/Mobile Optimizer")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		MobileOptimizer window = (MobileOptimizer)EditorWindow.GetWindow(typeof(MobileOptimizer));
		window.Show();
		window.autoRepaintOnSceneChange = true;
	}

	void OnEnable()
	{
		Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
	}

	GUIStyle myFoldoutStyle,myFoldoutStyle2;
	public LogType logType;
	Vector2 scrollPos;

	void OnGUI()
	{
		Undo.RecordObject (this,"lb");

		scrollPos = EditorGUILayout.BeginScrollView (scrollPos,
			false,
			false,
			GUILayout.Width(Screen.width ),
			GUILayout.Height(Screen.height));


		myFoldoutStyle = new GUIStyle(GUI.skin.button);
		myFoldoutStyle.normal.textColor = Color.black;  //ew Color32 (184, 26, 26, 255);
		myFoldoutStyle.fontStyle = FontStyle.Bold;
		myFoldoutStyle.fontStyle = FontStyle.Bold;

		myFoldoutStyle2 = new GUIStyle(GUI.skin.button);
		myFoldoutStyle2.normal.textColor = Color.black;  //ew Color32 (184, 26, 26, 255);
		myFoldoutStyle2.fontStyle = FontStyle.Bold;
		myFoldoutStyle2.fontStyle = FontStyle.Bold;
		myFoldoutStyle2.fixedHeight = 43f;
		myFoldoutStyle2.fixedWidth = 100f;

		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();

		EditorGUILayout.BeginHorizontal ();

		if (GUILayout.Button ("Lighting",myFoldoutStyle2))
			logType = LogType.Lighting;
		if (GUILayout.Button ("Camera",myFoldoutStyle2))
			logType = LogType.Camera;
		if (GUILayout.Button ("Build",myFoldoutStyle2))
			logType = LogType.Build;
		if (GUILayout.Button ("Terrain",myFoldoutStyle2))
			logType = LogType.Terrain;
		if (GUILayout.Button ("Material",myFoldoutStyle2))
			logType = LogType.Material;

		EditorGUILayout.EndHorizontal ();


		if(logType == LogType.Lighting)
			CheckLightingSettings ();
		if(logType == LogType.Camera)
			CheckCameraSettings ();
		if(logType == LogType.Build)
			CheckBuildSettings ();
		if(logType == LogType.Terrain)
			TerrainSettings ();
		if(logType == LogType.Material)
			CheckMaterials ();
		EditorGUILayout.EndScrollView();

		EditorUtility.SetDirty (this);

	}
	void CheckLightingSettings()
	{
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();

		Light[] lights = GameObject.FindObjectsOfType<Light> ();
		foreach (Light l in lights) {

			SerializedObject serialLightSource = new SerializedObject (l);
			SerializedProperty SerialProperty = serialLightSource.FindProperty ("m_Lightmapping");
			if (SerialProperty.intValue != 2) {

				if (GUILayout.Button (l.name + " : Change light type to baked mode for maximum performance", myFoldoutStyle)) {
					SerialProperty.intValue = 2;
					serialLightSource.ApplyModifiedProperties ();
				}

			}
		}

		if (PlayerSettings.colorSpace == ColorSpace.Linear) {
			if (GUILayout.Button ("Switch to Gamma color space for maximum performance", myFoldoutStyle)) {

				PlayerSettings.colorSpace = ColorSpace.Gamma;
			}
		}
		if (RenderSettings.skybox) {
			if (RenderSettings.skybox.name == "Default-Skybox") {
				if (GUILayout.Button ("Default skybox material is not good from performance side", myFoldoutStyle)) {
					RenderSettings.skybox = null;
				}
			}
		}

		if (Lightmapping.realtimeGI == true) {
			if (GUILayout.Button ("Disable Realtime GI for mobile targets", myFoldoutStyle)) {
				Lightmapping.realtimeGI = false;
			}
		}

		if (LightmapSettings.lightmapsMode != LightmapsMode.NonDirectional) {
			if (GUILayout.Button ("Use non-directional lightmap mode for mobile targets", myFoldoutStyle)) {
				LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
			}
		}
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		GUILayout.Label("End of the Lighting logs");
	}

	void CheckCameraSettings()
	{
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();

		Camera[] cams = GameObject.FindObjectsOfType<Camera> ();

		foreach (Camera c in cams) {
			if (c.renderingPath != RenderingPath.Forward) {

				if (GUILayout.Button (c.name  + " : Change rendering path to Forward", myFoldoutStyle)) {
					c.renderingPath = RenderingPath.Forward;
				}
			}

			if (c.allowHDR) {
				if (GUILayout.Button (c.name  + " : Disable Allow HDR mode", myFoldoutStyle)) {
					c.allowHDR = false;
				}
			}

			if (!c.useOcclusionCulling) {
				if (GUILayout.Button (c.name  + " : Enable Occlusion Culling", myFoldoutStyle)) {
					c.useOcclusionCulling = true;
				}
			}
			if (c.farClipPlane > 1000f) {
				if (GUILayout.Button (c.name  + " : Reduce far clip plane", myFoldoutStyle)) {
					c.farClipPlane = 1000f;
				}
			}
		}
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		GUILayout.Label("End of the Camera logs");
	}

	void CheckBuildSettings()
	{
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();

		if (PlayerSettings.Android.preferredInstallLocation != AndroidPreferredInstallLocation.Auto) {
			if (GUILayout.Button ("Change Instal Location mode to Automattic", myFoldoutStyle)) {
				PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
			}
		}
		if (PlayerSettings.Android.targetArchitectures != AndroidArchitecture.All) {
			if (GUILayout.Button ("Change target device to FAT for maximum Compatibility", myFoldoutStyle)) {
				PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
			}
		}

		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		GUILayout.Label("End of the Build logs");
	}

	void TerrainSettings()
	{
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();

		Terrain[] terrains = GameObject.FindObjectsOfType<Terrain> ();
		foreach (Terrain t in terrains) 
		{
			if (t.heightmapPixelError < 10f)
			{
				if (GUILayout.Button (t.name + " : You can reduce terrain polycounts with higher pixel error", myFoldoutStyle)) {
					t.heightmapPixelError = 170f;
				}
			}
			if (t.heightmapMaximumLOD > 513)
			{
				if (GUILayout.Button (t.name + " : Terrain heightmap resolution is too high for mobile platforms", myFoldoutStyle))
				{
				
				}
			}
			if (t.materialType != Terrain.MaterialType.BuiltInLegacyDiffuse)
			{
				if (GUILayout.Button (t.name + " : Switch to Legacy Diffuse shader for maximum performance", myFoldoutStyle)) {
					t.materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;
				}
			}
			if (t.reflectionProbeUsage != UnityEngine.Rendering.ReflectionProbeUsage.Off)
			{
				if (GUILayout.Button (t.name + " : Disable Reflection Probes fro better performance", myFoldoutStyle)) {
					t.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
				}
			}
			if (t.bakeLightProbesForTrees)
			{
				if (GUILayout.Button (t.name + " : Disable Light Probes for trees for better performance", myFoldoutStyle)) {
					t.bakeLightProbesForTrees = false;
				}
			}
			/*SerializedObject so = new SerializedObject (t.);
			if (so.FindProperty ("m_ScaleInLightmap").floatValue != 1f) {
				if (GUILayout.Button (t.name + " : Change lightmap scale to 1 for better quality on terrain", myFoldoutStyle)) {
					so.FindProperty ("m_ScaleInLightmap").floatValue = 1f;
				}
			}*/
		}
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		GUILayout.Label("End of the Terrain logs");
	}

	void CheckMaterials()
	{
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();

		MeshRenderer[] renderersT = GameObject.FindObjectsOfType<MeshRenderer> ();

		foreach (MeshRenderer m in renderersT) 
		{
			if (m.sharedMaterial.shader.name == "Standard" || m.sharedMaterial.shader.name == "Standard (Specular setup)" ) 
			{
				if (GUILayout.Button (m.name + " : Standard shader is not switable for mobile targets", myFoldoutStyle)) 
				{
					m.sharedMaterial.shader = Shader.Find("Legacy Shaders/Diffuse");
				}
			}
		}
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		GUILayout.Label("End of the Material logs");
	}
}
#endif