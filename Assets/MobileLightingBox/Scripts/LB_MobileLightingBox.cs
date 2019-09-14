
// AliyerEdon@gmail.com/
// Mobile Lighting Box is an "paid" asset. Don't share it for free

#if UNITY_EDITOR   
using UnityEngine;   
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;   
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
public class LB_MobileLightingBox : EditorWindow
{
	#region Variables

	public WindowMode winMode = WindowMode.Part1;
	public LB_MobileLightingBoxHelper helper;
	public bool webGL_Mobile = false;

	// Sun Shaft
	public SunShafts.SunShaftsResolution shaftQuality = SunShafts.SunShaftsResolution.High;
	public float shaftDistance = 0.5f;
	public float shaftBlur = 4f;
	public Color shaftColor = new Color32 (255,189,146,255);

	// Bloom
	public float bIntensity = 1f;
	public float bThreshould = 0.5f;
	public Texture2D dirtTexture;
	public float dirtIntensity;
	public bool mobileOptimizedBloom;

	public bool visualize;

	// Profiles
	public LB_MobileLightingProfile LB_LightingProfile;

	public LightingMode lightingMode;
	public AmbientLight ambientLight;
	public LightSettings lightSettings;
	public LightProbeMode lightprobeMode;

	// Depth of field
	public float dofRange;
	public float dofBlur;
	public float falloff = 30f;
	public DOFQuality dofQuality;

	// Sky and Sun
	public Material skyBox;
	public Light sunLight;
	public Flare sunFlare;
	public Color sunColor = Color.white;
	public float sunIntensity = 2.1f;
	public float indirectIntensity = 1.43f;
	public  Color ambientColor = new Color32(96,104,116,255);
	public Color skyColor;
	public Color equatorColor,groundColor;

	public bool autoMode;

	// Global Fog
	public FogMode fogMode = FogMode.Exponential;
	public Color fogColor = Color.white;
	public float fogIntensity;
	public float linearStart;
	public float linearEnd;

	public LightsShadow psShadow;
	public float bakedResolution = 10f;
	public bool helpBox;

	// Private variabled
	Color redColor;
	bool lightError;
	bool lightChecked;
	GUIStyle myFoldoutStyle;
	bool showLogs;
	// Display window elements (Lighting Box)   
	Vector2 scrollPos = Vector2.zero;

	// Camera
	public Camera mainCamera;

	// Lightmapping
	// Progressive
	public int directSamples;
	public int indirectSamples;
	public float aoIntensityDirect;
	public float aoIntensityIndirect;
	public bool aoEnabled;
	public float aoDistance;
	public float indirectBounce;
	public float backfaceTolerance = 0.7f;

	// Color grading
	public float exposure;
	public float contrast;
	public float gamma;
	public float saturation;
	public float colorR;
	public float colorG;
	public float colorB;
	public ToneMapping tonemap = ToneMapping.ACES;
	public float vignette;


	#endregion

	#region Init()
	// Add menu named "My Window" to the Window menu
	[MenuItem("Window/Mobile Lighting Box %E")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		////	LB_LightingBox window = (LB_LightingBox)EditorWindow.GetWindow(typeof(LB_LightingBox));
		System.Type inspectorType = System.Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
		LB_MobileLightingBox window = (LB_MobileLightingBox)EditorWindow.GetWindow<LB_MobileLightingBox>("Mobile Lighting Box", true, new System.Type[] {inspectorType} );

		window.Show();
		window.autoRepaintOnSceneChange = true;
		window.maxSize = new Vector2 (1000f, 1000f);
		window.minSize = new Vector2 (387f, 1000f);

	}
	#endregion

	#region Options
	// Internal Usage
	public bool LightingBoxState = true,OptionsState = true;
	public bool ambientState = true;
	public bool sunState = true;
	public bool lightSettingsState = true;
	public bool cameraState = true;
	public bool profileState = true;
	public bool buildState = true;
	public bool vLightState = true;
	public bool sunShaftState = true;
	public bool fogState = true;
	public bool dofState = true;
	public bool autoFocusState =  true;
	public bool colorState = true;
	public bool bloomState = true;
	public bool aaState = true;
	public bool aoState = true;
	public bool motionBlurState = true;
	public bool vignetteState = true;
	public bool chromatticState = true;
	public bool ssrState = true;
	public bool st_ssrState;

	// Effects enabled
	public bool Ambient_Enabled = true;
	public bool Scene_Enabled = true;
	public bool Sun_Enabled = true;
	public bool VL_Enabled = false;
	public bool SunShaft_Enabled = false;
	public bool Fog_Enabled = false;
	public bool DOF_Enabled = true;
	public bool Bloom_Enabled = false;
	public bool AA_Enabled = true;
	public bool AO_Enabled = false;
	public bool MotionBlur_Enabled = true;
	public bool Vignette_Enabled = true;
	public bool Chromattic_Enabled = true;
	public bool SSR_Enabled = false;
	public bool AutoFocus_Enabled = false;
	public bool ST_SSR_Enabled = false;
	public bool color_Enabled = true;

	Texture2D arrowOn,arrowOff;

	#endregion

	void NewSceneInit()
	{
		var LB_LightingProfileRef = LB_LightingProfile;

		if (EditorSceneManager.GetActiveScene ().name == "") 
		{

			LB_LightingProfile = Resources.Load ("DefaultSettings")as LB_MobileLightingProfile;
			helper.Update_MainProfile(LB_LightingProfile);


			currentScene = EditorSceneManager.GetActiveScene ().name;

		} 
		else
		{
			if (System.String.IsNullOrEmpty (EditorPrefs.GetString (EditorSceneManager.GetActiveScene ().name))) 
			{
				LB_LightingProfile = Resources.Load ("DefaultSettings")as LB_MobileLightingProfile;
				helper.Update_MainProfile(LB_LightingProfile);

			} else 
			{
				LB_LightingProfile = (LB_MobileLightingProfile)AssetDatabase.LoadAssetAtPath (EditorPrefs.GetString (EditorSceneManager.GetActiveScene ().name), typeof(LB_MobileLightingProfile));
				helper.Update_MainProfile(LB_LightingProfile);

			}

		  
			currentScene = EditorSceneManager.GetActiveScene ().name;

		}

		if (LB_LightingProfileRef != LB_LightingProfile) {
				OnLoad ();
		}

	}

	// Load and apply default settings when a new scene opened
	void OnNewSceneOpened()
	{
		NewSceneInit ();
	}

	void OnDisable()
	{
		EditorApplication.hierarchyWindowChanged -= OnNewSceneOpened;
	}

	void OnEnable()
	{
		arrowOn = Resources.Load ("arrowOn") as Texture2D;
		arrowOff = Resources.Load ("arrowOff") as Texture2D;

		if (!GameObject.Find ("LightingBox_Helper")) 
		{
			GameObject helperObject = new GameObject ("LightingBox_Helper");
			helperObject.AddComponent<LB_MobileLightingBoxHelper> ();
			helper = helperObject.GetComponent<LB_MobileLightingBoxHelper> ();
		}

		EditorApplication.hierarchyWindowChanged += OnNewSceneOpened;

		currentScene = EditorSceneManager.GetActiveScene().name;

		if (System.String.IsNullOrEmpty (EditorPrefs.GetString (EditorSceneManager.GetActiveScene ().name)))
			LB_LightingProfile = Resources.Load ("DefaultSettings")as LB_MobileLightingProfile;
		else
			LB_LightingProfile = (LB_MobileLightingProfile)AssetDatabase.LoadAssetAtPath (EditorPrefs.GetString (EditorSceneManager.GetActiveScene ().name), typeof(LB_MobileLightingProfile));

		if(LB_LightingProfile)
			OnLoad ();

	}

	void OnGUI()
	{

		#region Styles
		GUIStyle redStyle = new GUIStyle (EditorStyles.label);
		redStyle.alignment = TextAnchor.MiddleLeft;
		redStyle.normal.textColor = Color.red;

		GUIStyle blueStyle = new GUIStyle (EditorStyles.label);
		blueStyle.alignment = TextAnchor.MiddleLeft;
		blueStyle.normal.textColor = Color.blue;


		GUIStyle stateButton = new GUIStyle ();
		stateButton = "Label";
		stateButton.alignment = TextAnchor.MiddleLeft;
		stateButton.fontStyle = FontStyle.Bold;

		GUIStyle buttonMain = new GUIStyle ();
		buttonMain = "Box";
		buttonMain.alignment = TextAnchor.MiddleCenter;
		buttonMain.fontStyle = FontStyle.Bold;

		#endregion

		#region GUI start implementation
		Undo.RecordObject (this, "lb");

		scrollPos = EditorGUILayout.BeginScrollView (scrollPos,
			false,
			false,
			GUILayout.Width (this.position.width),
			GUILayout.Height (this.position.height));

		EditorGUILayout.Space ();

		GUILayout.Label ("Lighting Box 2 - ALIyerEdon@gmail.com", EditorStyles.helpBox);


		EditorGUILayout.BeginHorizontal ();

		if (!helpBox) {
			if (GUILayout.Button ("Show Help", buttonMain, GUILayout.Width (177), GUILayout.Height (24f))) {
				helpBox = !helpBox;
			}
		} else {
			if (GUILayout.Button ("Hide Help", buttonMain, GUILayout.Width (177), GUILayout.Height (24f))) {
				helpBox = !helpBox;
			}
		}
		if (GUILayout.Button ("Refresh", buttonMain, GUILayout.Width (179), GUILayout.Height (24f))) {
			UpdateSettings ();
			UpdatePostEffects ();
		}

		EditorGUILayout.EndHorizontal ();

		if (EditorPrefs.GetInt ("RateLB") != 3) {

			if (GUILayout.Button ("Rate Lighting Box")) {
				EditorPrefs.SetInt ("RateLB", 3);
				Application.OpenURL ("http://u3d.as/Se9");
			}
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();


		#endregion

		#region Tabs
		EditorGUILayout.BeginHorizontal ();
		//----------------------------------------------
		if (winMode == WindowMode.Part1)
			GUI.backgroundColor = Color.green;
		else
			GUI.backgroundColor = Color.white;
		//----------------------------------------------
		if (GUILayout.Button ("Scene", buttonMain, GUILayout.Width (87), GUILayout.Height (43)))
			winMode = WindowMode.Part1;
		//----------------------------------------------
		if (winMode == WindowMode.Part2)
			GUI.backgroundColor = Color.green;
		else
			GUI.backgroundColor = Color.white;
		//----------------------------------------------
		if (GUILayout.Button ("Effect", buttonMain, GUILayout.Width (87), GUILayout.Height (43)))
			winMode = WindowMode.Part2;
		//----------------------------------------------
		if (winMode == WindowMode.Part3)
			GUI.backgroundColor = Color.green;
		else
			GUI.backgroundColor = Color.white;
		//----------------------------------------------
		if (GUILayout.Button ("Color", buttonMain, GUILayout.Width (87), GUILayout.Height (43)))
			winMode = WindowMode.Part3;
		//----------------------------------------------
		if (winMode == WindowMode.Finish)
			GUI.backgroundColor = Color.green;
		else
			GUI.backgroundColor = Color.white;
		//----------------------------------------------
		if (GUILayout.Button ("Optimize", buttonMain, GUILayout.Width (87), GUILayout.Height (43)))
			winMode = WindowMode.Finish;
		//----------------------------------------------
		GUI.backgroundColor = Color.white;
		//----------------------------------------------//----------------------------------------------//----------------------------------------------
		
	    EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		#endregion

		#region Toolbar

		EditorGUILayout.BeginHorizontal();
		if (LightingBoxState) 
		{
			if (GUILayout.Button ("Effects Off", buttonMain, GUILayout.Width (177), GUILayout.Height (24))) {
				helper.Toggle_Effects ();

				LightingBoxState = !LightingBoxState;

				if(LB_LightingProfile)
					LB_LightingProfile.LightingBoxState = LightingBoxState;
			}
		} else {
			if (GUILayout.Button ("Effects On", buttonMain, GUILayout.Width (177), GUILayout.Height (24))) {
				helper.Toggle_Effects ();
				LightingBoxState = !LightingBoxState;

				if(LB_LightingProfile)
					LB_LightingProfile.LightingBoxState = LightingBoxState;
			}
		}
		if(OptionsState)
		{
			if (GUILayout.Button ("Expand All", buttonMain, GUILayout.Width (179), GUILayout.Height (24))){
				ambientState = sunState = lightSettingsState = true;
				cameraState = profileState = buildState = true;
				vLightState = sunShaftState = fogState = true;
				dofState = autoFocusState = colorState = true;
				bloomState = aaState = aoState = true;
				motionBlurState = vignetteState = chromatticState = true;
				ssrState = st_ssrState = true;
				OptionsState = !OptionsState;

				if(LB_LightingProfile)
				{
					LB_LightingProfile.ambientState = ambientState;
					LB_LightingProfile.sunState = sunState;
					LB_LightingProfile.lightSettingsState = lightSettingsState;
					LB_LightingProfile.cameraState = cameraState;
					LB_LightingProfile.profileState = profileState;
					LB_LightingProfile.buildState = buildState;
					LB_LightingProfile.sunShaftState = sunShaftState;
					LB_LightingProfile.fogState = fogState;
					LB_LightingProfile.dofState = dofState;
					LB_LightingProfile.colorState = colorState;
					LB_LightingProfile.bloomState = bloomState;
					LB_LightingProfile.OptionsState  = OptionsState;
					EditorUtility.SetDirty (LB_LightingProfile);
				}

			}
		}
		else
		{
			if (GUILayout.Button ("Close All", buttonMain, GUILayout.Width (179), GUILayout.Height (24))) {

				ambientState = sunState = lightSettingsState = false;
				cameraState = profileState = buildState = false;
				vLightState = sunShaftState = fogState = false;
				dofState = autoFocusState = colorState = false;
				bloomState = aaState = aoState = false;
				motionBlurState = vignetteState = chromatticState = false;
				ssrState = st_ssrState = false;
				OptionsState = !OptionsState;

				if(LB_LightingProfile)
				{
					LB_LightingProfile.ambientState = ambientState;
					LB_LightingProfile.sunState = sunState;
					LB_LightingProfile.lightSettingsState = lightSettingsState;
					LB_LightingProfile.cameraState = cameraState;
					LB_LightingProfile.profileState = profileState;
					LB_LightingProfile.buildState = buildState;
					LB_LightingProfile.sunShaftState = sunShaftState;
					LB_LightingProfile.fogState = fogState;
					LB_LightingProfile.dofState = dofState;
					LB_LightingProfile.colorState = colorState;
					LB_LightingProfile.bloomState = bloomState;
					LB_LightingProfile.OptionsState  = OptionsState;
					EditorUtility.SetDirty (LB_LightingProfile);
				}

			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		#endregion
		   
		if (winMode == WindowMode.Part1) {
			
			#region Toggle Settings


			#endregion

			#region Profiles

			//-----------Profile----------------------------------------------------------------------------
			GUILayout.BeginVertical ("Box");

			EditorGUILayout.BeginHorizontal ();

			if(profileState)
				GUILayout.Label(arrowOn,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			else
				GUILayout.Label(arrowOff,stateButton,GUILayout.Width(20),GUILayout.Height(14));

			var profileStateRef = profileState;

			if (GUILayout.Button ("Profile", stateButton, GUILayout.Width (300), GUILayout.Height (17f))) {
				profileState = !profileState;
			}

			if(profileStateRef != profileState)
			{
				
				if (LB_LightingProfile)
				{
					LB_LightingProfile.profileState = profileState;				
					EditorUtility.SetDirty (LB_LightingProfile);
				}

			}
			EditorGUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
			//---------------------------------------------------------------------------------------

			if (profileState) {
				if (helpBox)
					EditorGUILayout.HelpBox ("1. LB_MobileLightingBox settings profile   2.Post Processing Stack Profile", MessageType.Info);

				var lightingProfileRef = LB_LightingProfile;

				EditorGUILayout.BeginHorizontal ();
				LB_LightingProfile = EditorGUILayout.ObjectField ("Lighting Profile", LB_LightingProfile, typeof(LB_MobileLightingProfile), true) as LB_MobileLightingProfile;

				if (GUILayout.Button ("New", GUILayout.Width (43), GUILayout.Height (17))) {

					if (EditorSceneManager.GetActiveScene ().name == "")
						EditorSceneManager.SaveScene (EditorSceneManager.GetActiveScene ());

					string path = EditorUtility.SaveFilePanelInProject ("Save As ...", "Lighting_Profile_"+EditorSceneManager.GetActiveScene ().name, "asset", "");

					if (path != "")
					{
						LB_LightingProfile = new LB_MobileLightingProfile ();

						AssetDatabase.CreateAsset (LB_LightingProfile, path);
						AssetDatabase.CopyAsset (AssetDatabase.GetAssetPath (Resources.Load ("DefaultSettings_LB")), path);
						LB_LightingProfile = (LB_MobileLightingProfile)AssetDatabase.LoadAssetAtPath (path, typeof(LB_MobileLightingProfile));
						helper.Update_MainProfile(LB_LightingProfile);

						AssetDatabase.Refresh ();
						    
					}
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.Space ();

				if (lightingProfileRef != LB_LightingProfile){
					
					helper.Update_MainProfile(LB_LightingProfile);
					OnLoad ();
					EditorPrefs.SetString (EditorSceneManager.GetActiveScene ().name, AssetDatabase.GetAssetPath (LB_LightingProfile));

					if (LB_LightingProfile)
						EditorUtility.SetDirty (LB_LightingProfile);
				}

				if (helpBox)
					EditorGUILayout.HelpBox ("Which camera should has effects", MessageType.Info);

				var mainCameraRef = mainCamera;

				mainCamera = EditorGUILayout.ObjectField ("Target Camera", mainCamera, typeof(Camera), true) as Camera;

				EditorGUILayout.Space ();

				if (mainCameraRef != mainCamera) 
				{
					UpdatePostEffects ();
					UpdateSettings ();

					if (LB_LightingProfile)
					{
						LB_LightingProfile.mainCameraName = mainCamera.name;
						EditorUtility.SetDirty (LB_LightingProfile);
					}
				}

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

			}

			#endregion

			#region Ambient

			//-----------Ambient----------------------------------------------------------------------------
			GUILayout.BeginVertical ("Box");

			EditorGUILayout.BeginHorizontal ();

			if(ambientState)
				GUILayout.Label(arrowOn,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			else
				GUILayout.Label(arrowOff,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			
			var Ambient_EnabledRef = Ambient_Enabled;
			var ambientStateRef = ambientState;

			if (GUILayout.Button ("Ambient", stateButton, GUILayout.Width (300), GUILayout.Height (17f))) {
				ambientState = !ambientState;
			}

			if(ambientStateRef != ambientState )
			{
				if (LB_LightingProfile)
				{
					LB_LightingProfile.ambientState = ambientState;
					EditorUtility.SetDirty (LB_LightingProfile);
				}
			}

			EditorGUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
			//---------------------------------------------------------------------------------------


			if (ambientState) {
				if (helpBox)
					EditorGUILayout.HelpBox ("Assign scene skybox material here   ", MessageType.Info);

				var skyboxRef = skyBox;

				Ambient_Enabled = EditorGUILayout.Toggle("Enabled",Ambient_Enabled);
				EditorGUILayout.Space();

				skyBox = EditorGUILayout.ObjectField ("SkyBox Material", skyBox, typeof(Material), true) as Material;

				if (skyboxRef != skyBox) {

					helper.Update_SkyBox (Ambient_Enabled,skyBox);

					if (LB_LightingProfile)
					{
						LB_LightingProfile.skyBox = skyBox;
						EditorUtility.SetDirty (LB_LightingProfile);
					}
				}


				if (helpBox)
					EditorGUILayout.HelpBox ("Set ambient lighting source as Skybox(IBL) or a simple color", MessageType.Info);

				var ambientLightRef = ambientLight;
				var ambientColorRef = ambientColor;
				var skyBoxRef = skyBox;
				var skyColorRef = skyColor;
				var equatorColorRef = equatorColor;
				var groundColorRef = groundColor;

				// choose ambient lighting mode   (color or skybox)
				ambientLight = (AmbientLight)EditorGUILayout.EnumPopup ("Ambient Source", ambientLight, GUILayout.Width (343));

				if(ambientLight == AmbientLight.Color)
					ambientColor = EditorGUILayout.ColorField ("Color", ambientColor);
				if(ambientLight == AmbientLight.Gradient)
				{
					skyColor = EditorGUILayout.ColorField ("Sky Color", skyColor);
					equatorColor = EditorGUILayout.ColorField ("Equator Color", equatorColor);
					groundColor = EditorGUILayout.ColorField ("Ground Color", groundColor);
				}
				
				if (ambientLightRef != ambientLight || ambientColorRef != ambientColor
					|| skyBoxRef != skyBox || skyColorRef != skyColor
					|| equatorColorRef != equatorColor || groundColorRef != groundColor
					|| Ambient_EnabledRef != Ambient_Enabled  )
				{
					helper.Update_Ambient (Ambient_Enabled,ambientLight, ambientColor,skyColor,equatorColor,groundColor);

					if (LB_LightingProfile)
					{
						LB_LightingProfile.ambientColor = ambientColor;
						LB_LightingProfile.ambientLight = ambientLight;
						LB_LightingProfile.skyBox = skyBox;
						LB_LightingProfile.skyColor = skyColor;
						LB_LightingProfile.equatorColor = equatorColor;
						LB_LightingProfile.groundColor = groundColor;
						LB_LightingProfile.Ambient_Enabled = Ambient_Enabled;
						EditorUtility.SetDirty (LB_LightingProfile);
					}
				}
				//----------------------------------------------------------------------
			}
			#endregion

			#region Sun Light
			//-----------Sun----------------------------------------------------------------------------
			GUILayout.BeginVertical ("Box");

			EditorGUILayout.BeginHorizontal ();

			if(sunState)
				GUILayout.Label(arrowOn,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			else
				GUILayout.Label(arrowOff,stateButton,GUILayout.Width(20),GUILayout.Height(14));

			var sunStateRef = sunState;

			if (GUILayout.Button ("Sun", stateButton, GUILayout.Width (300), GUILayout.Height (17f))) {
				sunState = !sunState;
			}

			if(sunStateRef != sunState)
			{
				if (LB_LightingProfile)
				{
					LB_LightingProfile.sunState = sunState;
					EditorUtility.SetDirty (LB_LightingProfile);
				}
			}

			EditorGUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
			//---------------------------------------------------------------------------------------


			if (sunState) {
				if (helpBox)
					EditorGUILayout.HelpBox ("Sun /  Moon light settings", MessageType.Info);
				
				var Sun_EnabledRef = Sun_Enabled;

				Sun_Enabled = EditorGUILayout.Toggle("Enabled",Sun_Enabled);

				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal ();
				sunLight = EditorGUILayout.ObjectField ("Sun Light", sunLight, typeof(Light), true) as Light;
				if (!sunLight){
					if (GUILayout.Button ("Find"))
						Update_Sun ();
				}
				EditorGUILayout.EndHorizontal ();
				var sunColorRef = sunColor;

				sunColor = EditorGUILayout.ColorField ("Color", sunColor);

				var sunIntensityRef = sunIntensity;
				var indirectIntensityRef = indirectIntensity;

				sunIntensity = EditorGUILayout.Slider ("Intenity", sunIntensity, 0, 4f);
				indirectIntensity = EditorGUILayout.Slider ("Indirect Intensity", indirectIntensity, 0, 4f);

				var sunFlareRef = sunFlare;

				sunFlare = EditorGUILayout.ObjectField ("Lens Flare", sunFlare, typeof(Flare), true) as Flare;

				if (Sun_EnabledRef != Sun_Enabled)
				{					
					if (LB_LightingProfile)
					{
						LB_LightingProfile.Sun_Enabled = Sun_Enabled;
						EditorUtility.SetDirty (LB_LightingProfile);
					}
				}

				if(Sun_Enabled)
				{

					if (sunColorRef != sunColor || Sun_EnabledRef != Sun_Enabled) {
						
						if (sunLight)
							sunLight.color = sunColor;
						else
							Update_Sun ();		
						if (LB_LightingProfile)
						{
							LB_LightingProfile.sunColor = sunColor;
							EditorUtility.SetDirty (LB_LightingProfile);
						}
					}

					if (sunIntensityRef != sunIntensity || indirectIntensityRef != indirectIntensity
						|| Sun_EnabledRef != Sun_Enabled) {

						if (sunLight) {
							sunLight.intensity = sunIntensity;
							sunLight.bounceIntensity = indirectIntensity;
						} else
							Update_Sun ();
						if (LB_LightingProfile) {
							LB_LightingProfile.sunIntensity = sunIntensity;
							LB_LightingProfile.indirectIntensity = indirectIntensity;
							LB_LightingProfile.Sun_Enabled = Sun_Enabled;
						}
						if (LB_LightingProfile)
						{
							LB_LightingProfile.sunState = sunState;
							EditorUtility.SetDirty (LB_LightingProfile);
						}
					}
					if (sunFlareRef != sunFlare) {
						if (sunFlare) {
							if (sunLight)
								sunLight.flare = sunFlare;
						} else {
							if (sunLight)
								sunLight.flare = null;
						}

						if (LB_LightingProfile)
						{
							LB_LightingProfile.sunFlare = sunFlare;
							LB_LightingProfile.sunState = sunState;
							EditorUtility.SetDirty (LB_LightingProfile);
						}
					}
				}
			}
			#endregion

			#region Lighting Mode


			//-----------Light Settings----------------------------------------------------------------------------
			GUILayout.BeginVertical ("Box");

			EditorGUILayout.BeginHorizontal ();

			if(lightSettingsState)
				GUILayout.Label(arrowOn,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			else
				GUILayout.Label(arrowOff,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			
			var lightSettingsStateRef = lightSettingsState;
			var Scene_EnabledRef = Scene_Enabled;

			if (GUILayout.Button ("Lightmap", stateButton, GUILayout.Width (300), GUILayout.Height (17f))) {
				lightSettingsState = !lightSettingsState;
			}

			if(lightSettingsStateRef != lightSettingsState)
			{
				if (LB_LightingProfile)
				{
					LB_LightingProfile.lightSettingsState = lightSettingsState;  
					LB_LightingProfile.Scene_Enabled = Scene_Enabled;
					EditorUtility.SetDirty (LB_LightingProfile);
				}
			}

			EditorGUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
			//---------------------------------------------------------------------------------------


			if (lightSettingsState) {
				if (helpBox)
					EditorGUILayout.HelpBox ("Fully realtime without GI, Enlighten Realtime GI or Baked Progressive Lightmapper", MessageType.Info);

				var lightingModeRef = lightingMode;


				Scene_Enabled = EditorGUILayout.Toggle("Enabled",Scene_Enabled);
				EditorGUILayout.Space();

				// Choose lighting mode (realtime GI or baked GI)
				lightingMode = (LightingMode)EditorGUILayout.EnumPopup ("Lighting Mode", lightingMode, GUILayout.Width (343));

				if (lightingMode == LightingMode.BakedProgressive)
				{
					EditorGUILayout.Space ();

					if (helpBox)
						EditorGUILayout.HelpBox ("Baked lightmapping resolution. Higher value needs more RAM and longer bake time. Check task manager about RAM usage during bake time", MessageType.Info);

					var bakedResolutionRef = bakedResolution;
					var directSamplesRef = directSamples;
					var indirectSamplesRef = indirectSamples;
					var aoIntensityDirectRef = aoIntensityDirect;
					var aoIntensityIndirectRef = aoIntensityIndirect;
					var aoEnabledRef = aoEnabled;
					var aoDistanceRef = aoDistance;
					var indirectBounceRef = indirectBounce;
					var backfaceToleranceRef = backfaceTolerance;

					// Baked lightmapping resolution   
					bakedResolution = EditorGUILayout.FloatField ("Baked Resolution", bakedResolution);
					EditorGUILayout.Space();
					indirectBounce = EditorGUILayout.Slider ("Indirect Bounce", indirectBounce,0,5);
					EditorGUILayout.Space();
					directSamples = EditorGUILayout.IntSlider ("Direct Samples", directSamples,30,1000);
					indirectSamples = EditorGUILayout.IntSlider ("Indirect Samples", indirectSamples,50,10000);
					backfaceTolerance = EditorGUILayout.Slider ("Backface Tolerance", backfaceTolerance,0,0.9f);
					EditorGUILayout.Space();
					aoEnabled = EditorGUILayout.Toggle ("Use AO", aoEnabled);
					if(aoEnabled)
					{
						aoDistance = EditorGUILayout.Slider ("AO Distance", aoDistance,0,10);
						aoIntensityDirect = EditorGUILayout.Slider ("AO Direct", aoIntensityDirect,0,5);
						aoIntensityIndirect = EditorGUILayout.Slider ("AO Indirect", aoIntensityIndirect,0,5);
					}


					EditorGUILayout.Space();

					if(lightingModeRef != lightingMode || bakedResolutionRef != bakedResolution || directSamplesRef != directSamples
						|| indirectSamplesRef != indirectSamples || aoIntensityDirectRef != aoIntensityDirect
						|| aoIntensityIndirectRef != aoIntensityIndirect || aoEnabledRef != aoEnabled
						|| aoDistanceRef != aoDistance || indirectBounceRef != indirectBounce || backfaceToleranceRef != backfaceTolerance)
					{
						helper.Update_LightingMode (Scene_Enabled,lightingMode,indirectBounce,directSamples,indirectSamples,aoEnabled,aoDistance, aoIntensityDirect,aoIntensityIndirect,backfaceTolerance,bakedResolution);

						if (LB_LightingProfile)
						{
							LB_LightingProfile.lightingMode = lightingMode;
							LB_LightingProfile.bakedResolution = bakedResolution;
							LB_LightingProfile.directSamples = directSamples;
							LB_LightingProfile.indirectSamples = indirectSamples;
							LB_LightingProfile.backfaceTolerance = backfaceTolerance;
							LB_LightingProfile.aoIntensityDirect= aoIntensityDirect;
							LB_LightingProfile.aoIntensityIndirect = aoIntensityIndirect;
							LB_LightingProfile.aoEnabled = aoEnabled;
							LB_LightingProfile.aoDistance = aoDistance;
							LB_LightingProfile.indirectBounce = indirectBounce;
							EditorUtility.SetDirty (LB_LightingProfile);
						}
					}
				}

				if (lightingMode == LightingMode.BakedEnlighten)
				{
					EditorGUILayout.Space ();

					if (helpBox)
						EditorGUILayout.HelpBox ("Baked lightmapping resolution. Higher value needs more RAM and longer bake time. Check task manager about RAM usage during bake time", MessageType.Info);

					var bakedResolutionRef = bakedResolution;
					var aoIntensityDirectRef = aoIntensityDirect;
					var aoIntensityIndirectRef = aoIntensityIndirect;
					var aoEnabledRef = aoEnabled;
					var aoDistanceRef = aoDistance;
					var indirectBounceRef = indirectBounce;
					var backfaceToleranceRef = backfaceTolerance;

					// Baked lightmapping resolution   
					bakedResolution = EditorGUILayout.FloatField ("Baked Resolution", bakedResolution);
					EditorGUILayout.Space();
					indirectBounce = EditorGUILayout.Slider ("Indirect Bounce", indirectBounce,0,5);
					EditorGUILayout.Space();
					backfaceTolerance = EditorGUILayout.Slider ("Backface Tolerance", backfaceTolerance,0,1);
					EditorGUILayout.Space();
					aoEnabled = EditorGUILayout.Toggle ("Use AO", aoEnabled);
					if(aoEnabled)
					{
						aoDistance = EditorGUILayout.Slider ("AO Distance", aoDistance,0,10);
						aoIntensityDirect = EditorGUILayout.Slider ("AO Direct", aoIntensityDirect,0,5);
						aoIntensityIndirect = EditorGUILayout.Slider ("AO Indirect", aoIntensityIndirect,0,5);
					}


					EditorGUILayout.Space();

					if(lightingModeRef != lightingMode || bakedResolutionRef != bakedResolution|| aoIntensityDirectRef != aoIntensityDirect
						|| aoIntensityIndirectRef != aoIntensityIndirect || aoEnabledRef != aoEnabled
						|| aoDistanceRef != aoDistance || indirectBounceRef != indirectBounce || backfaceToleranceRef != backfaceTolerance )
					{
						helper.Update_LightingMode (Scene_Enabled,lightingMode,indirectBounce,directSamples,indirectSamples,aoEnabled,aoDistance, aoIntensityDirect,aoIntensityIndirect,backfaceTolerance,bakedResolution);

						if (LB_LightingProfile)
						{
							LB_LightingProfile.lightingMode = lightingMode;
							LB_LightingProfile.bakedResolution = bakedResolution;
							LB_LightingProfile.directSamples = directSamples;
							LB_LightingProfile.indirectSamples = indirectSamples;
							LB_LightingProfile.aoIntensityDirect= aoIntensityDirect;
							LB_LightingProfile.aoIntensityIndirect = aoIntensityIndirect;
							LB_LightingProfile.aoEnabled = aoEnabled;
							LB_LightingProfile.aoDistance = aoDistance;
							LB_LightingProfile.indirectBounce = indirectBounce;
							LB_LightingProfile.backfaceTolerance = backfaceTolerance;
							EditorUtility.SetDirty (LB_LightingProfile);
						}
					}
				}

				if (lightingModeRef != lightingMode || Scene_EnabledRef != Scene_Enabled) {
					//----------------------------------------------------------------------
					// Update Lighting Mode
					helper.Update_LightingMode (Scene_Enabled,lightingMode,indirectBounce,directSamples,indirectSamples,aoEnabled,aoDistance, aoIntensityDirect,aoIntensityIndirect,backfaceTolerance,bakedResolution);
					//----------------------------------------------------------------------
					if (LB_LightingProfile)
					{
						LB_LightingProfile.lightingMode = lightingMode; 
						LB_LightingProfile.Scene_Enabled = Scene_Enabled;
						EditorUtility.SetDirty (LB_LightingProfile);
					}
				}
				#endregion

			#region Light Types
			EditorGUILayout.Space ();

			if (helpBox)
				EditorGUILayout.HelpBox ("Changing the type of all light sources (Realtime,Baked,Mixed)", MessageType.Info);

			var lightSettingsRef = lightSettings;

			// Change file lightmapping type mixed,realtime baked
			lightSettings = (LightSettings)EditorGUILayout.EnumPopup ("Lights Type", lightSettings, GUILayout.Width (343));

			//----------------------------------------------------------------------
			// Light Types
			if (lightSettingsRef != lightSettings) {

					helper.Update_LightSettings (Scene_Enabled,lightSettings);

				if (LB_LightingProfile)
				{
					LB_LightingProfile.lightSettings = lightSettings;
					EditorUtility.SetDirty (LB_LightingProfile);
				}
			}
			//----------------------------------------------------------------------
			#endregion

			#region Light Shadows Settings
			EditorGUILayout.Space ();

			if (helpBox)
				EditorGUILayout.HelpBox ("Activate shadows for point and spot lights   ", MessageType.Info);

			var pshadRef = psShadow;
			// Choose hard shadows state on off for spot and point lights
			psShadow = (LightsShadow)EditorGUILayout.EnumPopup ("Shadows", psShadow, GUILayout.Width (343));

			if (pshadRef != psShadow) {

				// Shadows
					helper.Update_Shadows (Scene_Enabled,psShadow);

				//----------------------------------------------------------------------
				if (LB_LightingProfile)
				{
					LB_LightingProfile.lightsShadow = psShadow;
					EditorUtility.SetDirty (LB_LightingProfile);
				}
			}
			#endregion

			#region Light Probes
			EditorGUILayout.Space ();

			if (helpBox)
				EditorGUILayout.HelpBox ("Adjust light probes settings for non-static objects, Blend mode is more optimized", MessageType.Info);

			var lightprobeModeRef = lightprobeMode;

			lightprobeMode = (LightProbeMode)EditorGUILayout.EnumPopup ("Light Probes", lightprobeMode, GUILayout.Width (343));

			if (lightprobeModeRef != lightprobeMode) {

				// Light Probes
				helper.Update_LightProbes (Scene_Enabled, lightprobeMode);

				//----------------------------------------------------------------------
				if (LB_LightingProfile)
				{
					LB_LightingProfile.lightProbesMode = lightprobeMode;
					EditorUtility.SetDirty (LB_LightingProfile);
				}
			}
				EditorGUILayout.Space();
				EditorGUILayout.Space();

				var automodeRef = autoMode;

				if (helpBox)
					EditorGUILayout.HelpBox ("Automatic lightmap baking", MessageType.Info);


				autoMode = EditorGUILayout.Toggle ("Auto Mode", autoMode);

				if (automodeRef != autoMode) {
					// Auto Mode
					if (autoMode)
						Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.Iterative;
					else
						Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
					//----------------------------------------------------------------------
					if (LB_LightingProfile)
					{
						LB_LightingProfile.automaticLightmap = autoMode;
						EditorUtility.SetDirty (LB_LightingProfile);
					}
				}

				// Start bake
				if (!Lightmapping.isRunning) {

					if (helpBox)
						EditorGUILayout.HelpBox ("Bake lightmap", MessageType.Info);

					if (GUILayout.Button ("Bake")) {
						if (!Lightmapping.isRunning) {
							Lightmapping.BakeAsync ();
						}
					}

					if (helpBox)
						EditorGUILayout.HelpBox ("Clear lightmap data", MessageType.Info);

					if (GUILayout.Button ("Clear")) {
						Lightmapping.Clear ();
					}
				} else {

					if (helpBox)
						EditorGUILayout.HelpBox ("Cancel lightmap baking", MessageType.Info);

					if (GUILayout.Button ("Cancel")) {
						if (Lightmapping.isRunning) {
							Lightmapping.Cancel ();
						}
					}
				}

				if (Input.GetKey (KeyCode.F)) {
					if (Lightmapping.isRunning)
						Lightmapping.Cancel ();
				}
				if (Input.GetKey (KeyCode.LeftControl) && Input.GetKey (KeyCode.E)) {
					if (!Lightmapping.isRunning)
						Lightmapping.BakeAsync ();
				}

				if (helpBox) {
					EditorGUILayout.HelpBox ("Bake : Shift + B", MessageType.Info);
					EditorGUILayout.HelpBox ("Cancel : Shift + C", MessageType.Info);
					EditorGUILayout.HelpBox ("Clear : Shift + E", MessageType.Info);

				}
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				EditorGUILayout.Space();

		}
		#endregion

		}

			if (winMode == WindowMode.Part2)
			{

				#region Sun Shaft

			//-----------Sun Shaft----------------------------------------------------------------------------
			GUILayout.BeginVertical ("Box");

			var SunShaft_EnabledRef = SunShaft_Enabled;

			EditorGUILayout.BeginHorizontal ();

			if(sunShaftState)
				GUILayout.Label(arrowOn,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			else
				GUILayout.Label(arrowOff,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			
			SunShaft_Enabled = EditorGUILayout.Toggle("",SunShaft_Enabled ,GUILayout.Width(30f),GUILayout.Height(17f));

			var sunShaftStateRef = sunShaftState;

			if (GUILayout.Button ("Sun Shaft", stateButton, GUILayout.Width (300), GUILayout.Height (17f))) {
				sunShaftState = !sunShaftState;
			}

			if(sunShaftStateRef != sunShaftState)
			{
				if (LB_LightingProfile)
					LB_LightingProfile.sunShaftState = sunShaftState;
				if (LB_LightingProfile)
					EditorUtility.SetDirty (LB_LightingProfile);
			}

			EditorGUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
			//---------------------------------------------------------------------------------------

			if(sunShaftState)
			{
				if (helpBox)
					EditorGUILayout.HelpBox ("Activate Sun Shaft for sun", MessageType.Info);
			}
				var shaftDistanceRef = shaftDistance;
				var shaftBlurRef = shaftBlur;
				var shaftColorRef = shaftColor;
				var shaftQualityRef = shaftQuality;

				// Activate Sun Shaft for sun
				///	shaftMode = (ShaftMode)EditorGUILayout.EnumPopup("Sun Shaft",shaftMode,GUILayout.Width(343));


				if(sunShaftState)
				{
					shaftQuality = (SunShafts.SunShaftsResolution)EditorGUILayout.EnumPopup ("Quality", shaftQuality, GUILayout.Width (343));
					shaftDistance = 1.0f - EditorGUILayout.Slider ("Distance falloff", 1.0f - shaftDistance, 0.1f, 1.0f);
					shaftBlur = EditorGUILayout.Slider ("Blur", shaftBlur, 1f, 10f);
					shaftColor = (Color)EditorGUILayout.ColorField ("Color", shaftColor);
				}

			if (SunShaft_EnabledRef != SunShaft_Enabled || shaftDistanceRef != shaftDistance
				  || shaftBlurRef != shaftBlur || shaftColorRef != shaftColor || shaftQualityRef != shaftQuality) {

					// Sun Shaft
				if(sunLight)					
					helper.Update_SunShaft (mainCamera, SunShaft_Enabled, shaftQuality, shaftDistance, shaftBlur, shaftColor, sunLight.transform);

				//----------------------------------------------------------------------
				if (LB_LightingProfile)
				{
						LB_LightingProfile.SunShaft_Enabled = SunShaft_Enabled;
						LB_LightingProfile.shaftQuality = shaftQuality;
						LB_LightingProfile.shaftDistance = shaftDistance;
						LB_LightingProfile.shaftBlur = shaftBlur;
						LB_LightingProfile.shaftColor = shaftColor;
						EditorUtility.SetDirty (LB_LightingProfile);
				}
				}

				#endregion

				#region Global Fog


			//-----------Global Fog----------------------------------------------------------------------------
			GUILayout.BeginVertical ("Box");

			var Fog_EnabledRef = Fog_Enabled;

			EditorGUILayout.BeginHorizontal ();

			if(fogState)
				GUILayout.Label(arrowOn,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			else
				GUILayout.Label(arrowOff,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			
			Fog_Enabled = EditorGUILayout.Toggle("",Fog_Enabled ,GUILayout.Width(30f),GUILayout.Height(17f));

			var fogStateRef = fogState;

			if (GUILayout.Button ("Global Fog", stateButton, GUILayout.Width (300), GUILayout.Height (17f))) {
				fogState = !fogState;
			}

			if(fogStateRef != fogState)
			{
				if (LB_LightingProfile)
				{
					LB_LightingProfile.fogState = fogState;
					EditorUtility.SetDirty (LB_LightingProfile);
				}
			}
			EditorGUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
			//---------------------------------------------------------------------------------------

			if(fogState)
			{
				var fogModeRef = fogMode;
				var fogColorRef = fogColor;
				var fogIntensityRef = fogIntensity;
				var linearStartRef = linearStart;
				var linearEndRef = linearEnd;

				fogMode = (FogMode)EditorGUILayout.EnumPopup ("Mode", fogMode, GUILayout.Width (343));
				fogColor = EditorGUILayout.ColorField("Color",fogColor);
				fogIntensity = EditorGUILayout.Slider("Intensity",fogIntensity,0,0.1f);
				if(fogMode == FogMode.Linear)
				{
					linearStart = EditorGUILayout.Slider("Start",linearStart,0,3000);
					linearEnd = EditorGUILayout.Slider("End",linearEnd,0,3000);
				}

				if(fogModeRef != fogMode || fogColorRef != fogColor || fogIntensityRef != fogIntensity 
					|| linearStartRef != linearStart || linearEndRef != linearEnd || Fog_EnabledRef != Fog_Enabled)
				{

					helper.Update_GlobalFog(mainCamera,Fog_Enabled,fogMode,fogColor,fogIntensity,linearStart,linearEnd);

					if(LB_LightingProfile)
					{
						LB_LightingProfile.Fog_Enabled = Fog_Enabled;
						LB_LightingProfile.fogMode = fogMode;
						LB_LightingProfile.fogColor = fogColor;
						LB_LightingProfile.fogIntensity = fogIntensity;
						LB_LightingProfile.linearStart = linearStart;
						LB_LightingProfile.linearEnd = linearEnd;
					}
				}
			}
				#endregion

				#region Depth of Field Legacy    

			//-----------Depth of Field----------------------------------------------------------------------------
			GUILayout.BeginVertical ("Box");

			var DOF_EnabledRef = DOF_Enabled;

			EditorGUILayout.BeginHorizontal ();

			if(dofState)
				GUILayout.Label(arrowOn,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			else
				GUILayout.Label(arrowOff,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			
			DOF_Enabled = EditorGUILayout.Toggle("",DOF_Enabled ,GUILayout.Width(30f),GUILayout.Height(17f));

			var dofStateRef = dofState;

			if (GUILayout.Button ("Depth of Field", stateButton, GUILayout.Width (300), GUILayout.Height (17f))) {
				dofState = !dofState;
			}

			if(dofStateRef != dofState)
			{
				if (LB_LightingProfile)
					LB_LightingProfile.dofState = dofState;
				if (LB_LightingProfile)					
					EditorUtility.SetDirty (LB_LightingProfile);
			}

			EditorGUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
			//---------------------------------------------------------------------------------------

			if(dofState)
			{
				if (helpBox)
					EditorGUILayout.HelpBox ("Activate Depth Of Field for the camera", MessageType.Info);
			}
				var dofRangeRef = dofRange;
				var dofBlurRef = dofBlur;
				var dofQualityRef = dofQuality;
				var visulaizeRef = visualize;
				
			if(dofState)
			{
				dofQuality = (DOFQuality)EditorGUILayout.EnumPopup ("Quality", dofQuality, GUILayout.Width (343));
				dofRange = (float)EditorGUILayout.Slider ("Range", dofRange, 0, 1000);
				dofBlur = (float)EditorGUILayout.Slider ("Blur", dofBlur, 0, 10);
				visualize = EditorGUILayout.Toggle ("Visualize", visualize);
			}


				if (DOF_EnabledRef != DOF_Enabled || dofRangeRef != dofRange || dofBlurRef != dofBlur
				|| dofQualityRef != dofQuality || visulaizeRef != visualize ) {

				helper.Update_DOF(mainCamera,DOF_Enabled,dofQuality,dofBlur,dofRange,visualize);

					//----------------------------------------------------------------------
					if (LB_LightingProfile)
				{
						LB_LightingProfile.DOF_Enabled = DOF_Enabled;
						LB_LightingProfile.dofRange = dofRange;
						LB_LightingProfile.dofQuality = dofQuality;
						LB_LightingProfile.dofBlur = dofBlur;
						LB_LightingProfile.visualize = visualize;
						EditorUtility.SetDirty (LB_LightingProfile);
				}
				}

				#endregion

			#region Bloom

			//-----------Bloom----------------------------------------------------------------------------
			GUILayout.BeginVertical ("Box");

			var Bloom_EnabledRef = Bloom_Enabled;

			EditorGUILayout.BeginHorizontal ();

			if(bloomState)
				GUILayout.Label(arrowOn,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			else
				GUILayout.Label(arrowOff,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			
			Bloom_Enabled = EditorGUILayout.Toggle("",Bloom_Enabled ,GUILayout.Width(30f),GUILayout.Height(17f));

			var bloomStateRef = bloomState;

			if (GUILayout.Button ("Bloom", stateButton, GUILayout.Width (300), GUILayout.Height (17f))) {
				bloomState = !bloomState;
			}

			if(bloomStateRef != bloomState)
			{
				if (LB_LightingProfile)
				{
					LB_LightingProfile.bloomState = bloomState;			
					EditorUtility.SetDirty (LB_LightingProfile);
				}
			}

				EditorGUILayout.EndHorizontal ();

				GUILayout.EndVertical ();
				//---------------------------------------------------------------------------------------
				if(bloomState)
				{
					if (helpBox)
						EditorGUILayout.HelpBox ("Activate Mobile Optimized Bloom for the camera", MessageType.Info);
				}
				var bIntensityRef = bIntensity;
				var bThreshouldRef = bThreshould;

			if (bloomState)
			{
				bIntensity = (float)EditorGUILayout.Slider ("Intensity", bIntensity, 0, 3f);
				bThreshould = (float)EditorGUILayout.Slider ("Threshould", bThreshould, 0, 2f);

					EditorGUILayout.Space();
			}

			if (Bloom_EnabledRef != Bloom_Enabled || bIntensityRef != bIntensity || bThreshouldRef != bThreshould || bIntensityRef != bIntensity) {


				helper.Update_Bloom(mainCamera,Bloom_Enabled,bIntensity,bThreshould);

				//----------------------------------------------------------------------

				if (LB_LightingProfile)
				{
						LB_LightingProfile.Bloom_Enabled = Bloom_Enabled;
						LB_LightingProfile.bIntensity = bIntensity;
						LB_LightingProfile.bThreshould = bThreshould;
						EditorUtility.SetDirty (LB_LightingProfile);
				}
				}

				#endregion

			}

			if (winMode == WindowMode.Part3) {

				#region Color Grading

			//-----------Color Grading----------------------------------------------------------------------------
			GUILayout.BeginVertical ("Box");

			EditorGUILayout.BeginHorizontal ();

			var color_EnabledRef = color_Enabled;

			if(colorState)
				GUILayout.Label(arrowOn,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			else
				GUILayout.Label(arrowOff,stateButton,GUILayout.Width(20),GUILayout.Height(14));
			
			color_Enabled = EditorGUILayout.Toggle("",color_Enabled ,GUILayout.Width(30f),GUILayout.Height(17f));

			var colorStateRef = colorState;

			if (GUILayout.Button ("Color Grading", stateButton, GUILayout.Width (300), GUILayout.Height (17f))) {
				colorState = !colorState;
			}

			if(colorStateRef != colorState)
			{
				if (LB_LightingProfile)
				{
					LB_LightingProfile.colorState = colorState;				
					EditorUtility.SetDirty (LB_LightingProfile);
				}
			}

			EditorGUILayout.EndHorizontal ();

			GUILayout.EndVertical ();
			//---------------------------------------------------------------------------------------

			if(colorState)
			{
				if (helpBox)
					EditorGUILayout.HelpBox ("Color grading settings", MessageType.Info);
			}
			var exposureRef = exposure;
			var contrastRef = contrast;
			var gammaRef = gamma;
			var saturationRef = saturation;
			var colorRRef = colorR;
			var colorGRef = colorG;
			var colorBRef = colorB;
			var tonemapRef = tonemap;
			var vignetteRef = vignette;

			if(colorState)
			{
				tonemap = (ToneMapping)EditorGUILayout.EnumPopup ("Mode", tonemap, GUILayout.Width (343));
				EditorGUILayout.Space();
				exposure = (float)EditorGUILayout.Slider ("Exposure", exposure, 0.3f, 1);
				contrast = (float)EditorGUILayout.Slider ("Contrast", contrast, 1, 2);
				saturation = (float)EditorGUILayout.Slider ("Saturation", saturation, 1, 0);
				EditorGUILayout.Space();
				gamma = (float)EditorGUILayout.Slider ("Gamma", gamma, 0.3f, 1);
				EditorGUILayout.Space();
				vignette = (float)EditorGUILayout.Slider ("Vignette", vignette, 0, 0.5f);
				EditorGUILayout.Space();
				EditorGUILayout.Space();
				colorR = (float)EditorGUILayout.Slider ("Red", colorR, -100, 100);
				colorG = (float)EditorGUILayout.Slider ("Green", colorG, -100, 100);
				colorB = (float)EditorGUILayout.Slider ("Blue", colorB, -100, 100);

			}

			if (color_EnabledRef != color_Enabled || exposureRef != exposure || contrastRef != contrast || gammaRef != gamma || saturationRef != saturation
				|| colorRRef != colorR || colorGRef != colorG || colorBRef != colorB || tonemapRef != tonemap || vignetteRef != vignette ) {

				helper.Update_ColorGrading (mainCamera,color_Enabled, tonemap,exposure,contrast,gamma,saturation,colorR,colorG,colorB,vignette);

					//----------------------------------------------------------------------
				if (LB_LightingProfile)
				{
					LB_LightingProfile.tonemap = tonemap;
					LB_LightingProfile.exposure = exposure;
					LB_LightingProfile.contrast = contrast;
					LB_LightingProfile.gamma = gamma;
					LB_LightingProfile.saturation = saturation;
					LB_LightingProfile.colorR = colorR;
					LB_LightingProfile.colorG = colorG;
					LB_LightingProfile.colorB = colorB;
					LB_LightingProfile.color_Enabled = color_Enabled;
					LB_LightingProfile.vignette = vignette;
					EditorUtility.SetDirty (LB_LightingProfile);
				}
			}

				#endregion

			}

			if (winMode == WindowMode.Finish) {

			#region Optimizers
			if(GUILayout.Button("Material Converter"))
				EditorApplication.ExecuteMenuItem ("Window/Mobile Optimizer/Batch Material Converter");
			EditorGUILayout.Space ();

			if(GUILayout.Button("Texture Importer"))
				EditorApplication.ExecuteMenuItem ("Window/Mobile Optimizer/Batch Texture Importer");
			EditorGUILayout.Space ();

			if(GUILayout.Button("Model Importer"))
				EditorApplication.ExecuteMenuItem ("Window/Mobile Optimizer/Batch Model Importer");
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			#endregion

				#region Check for updates

				if (GUILayout.Button ("Check for updates")) {
				
					EditorApplication.ExecuteMenuItem ("Assets/Lighting Box Updates");
				}

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				#endregion

			}

			EditorGUILayout.EndScrollView ();
		}


		#region Update Settings
		void UpdateSettings()
		{
		if (!mainCamera)
			OnLoad ();
		
			// Sun Light Update
			if (sunLight) {
				sunLight.color = sunColor;
				sunLight.intensity = sunIntensity;
				sunLight.bounceIntensity = indirectIntensity;
			} else {
				Update_Sun ();
			}

			if (sunFlare)
			{
				if(sunLight)
					sunLight.flare = sunFlare;
			}
			else
			{
				if(sunLight)
					sunLight.flare = null;
			}

			// Skybox
			helper.Update_SkyBox (Ambient_Enabled,skyBox);
	
			// Update Lighting Mode
			helper.Update_LightingMode (Scene_Enabled,lightingMode,indirectBounce,directSamples,indirectSamples,aoEnabled,aoDistance, aoIntensityDirect,aoIntensityIndirect,backfaceTolerance,bakedResolution);

			// Update Ambient
			helper.Update_Ambient (Ambient_Enabled,ambientLight, ambientColor,skyColor,equatorColor,groundColor);

			// Lights settings
			helper.Update_LightSettings (Scene_Enabled,lightSettings);

			// Sun Shaft
		if(sunLight)
			helper.Update_SunShaft(mainCamera,SunShaft_Enabled, shaftQuality,shaftDistance,shaftBlur,shaftColor,sunLight.transform);

			// Shadows
			helper.Update_Shadows(Scene_Enabled,psShadow);
			
			// Light Probes
			helper.Update_LightProbes(Scene_Enabled,lightprobeMode);

			// Auto Mode
			helper.Update_AutoMode(autoMode);

			// Global Fog
			helper.Update_GlobalFog(mainCamera,Fog_Enabled,fogMode,fogColor,fogIntensity,linearStart,linearEnd);

		}
		#endregion

		#region On Load
		// load saved data based on project and scene name
		void OnLoad()
		{

		if (PlayerSettings.colorSpace != ColorSpace.Gamma)
			PlayerSettings.colorSpace = ColorSpace.Gamma;
				
		if (!mainCamera) {
			if (GameObject.Find (LB_LightingProfile.mainCameraName))
				mainCamera = GameObject.Find (LB_LightingProfile.mainCameraName).GetComponent<Camera> ();
			else
				mainCamera = GameObject.FindObjectOfType<Camera> ();
		}
		
		if (!GameObject.Find ("LightingBox_Helper")){
			GameObject helperObject = new GameObject ("LightingBox_Helper");
			helperObject.AddComponent<LB_MobileLightingBoxHelper> ();
			helper = helperObject.GetComponent<LB_MobileLightingBoxHelper> ();
		}


		if (LB_LightingProfile) {

			lightingMode = LB_LightingProfile.lightingMode;
			if (LB_LightingProfile.skyBox)
				skyBox = LB_LightingProfile.skyBox;
			else
				skyBox = RenderSettings.skybox;
			sunFlare = LB_LightingProfile.sunFlare;
			ambientLight = LB_LightingProfile.ambientLight;
			lightSettings = LB_LightingProfile.lightSettings;
			sunColor = LB_LightingProfile.sunColor;

			lightprobeMode = LB_LightingProfile.lightProbesMode;

			// Shadows
			psShadow = LB_LightingProfile.lightsShadow;

			// DOF Legacy
			dofRange = LB_LightingProfile.dofRange;
			dofBlur = LB_LightingProfile.dofBlur;
			visualize = LB_LightingProfile.visualize;
			dofQuality = LB_LightingProfile.dofQuality;

			// Bloom
			bIntensity = LB_LightingProfile.bIntensity;
			bThreshould = LB_LightingProfile.bThreshould;
			// Fog
			fogMode = LB_LightingProfile.fogMode;
			fogColor = LB_LightingProfile.fogColor;
			fogIntensity = LB_LightingProfile.fogIntensity;
			linearStart = LB_LightingProfile.linearStart;
			linearEnd = LB_LightingProfile.linearEnd;

			// Color Grading
			exposure = LB_LightingProfile.exposure;
			contrast = LB_LightingProfile.contrast;
			gamma = LB_LightingProfile.gamma;
			saturation = LB_LightingProfile.saturation;
			colorR = LB_LightingProfile.colorR;
			colorG = LB_LightingProfile.colorG;
			colorB = LB_LightingProfile.colorB;
			vignette = LB_LightingProfile.vignette;
			color_Enabled = LB_LightingProfile.color_Enabled;
			tonemap = LB_LightingProfile.tonemap;

			// Lightmap
			bakedResolution = LB_LightingProfile.bakedResolution;
			sunIntensity = LB_LightingProfile.sunIntensity;
			indirectIntensity = LB_LightingProfile.indirectIntensity;

			ambientColor = LB_LightingProfile.ambientColor;
			ambientLight = LB_LightingProfile.ambientLight;
			skyBox = LB_LightingProfile.skyBox;
			skyColor = LB_LightingProfile.skyColor;
			equatorColor = LB_LightingProfile.equatorColor;
			groundColor = LB_LightingProfile.groundColor;

			// Auto lightmap
			autoMode = LB_LightingProfile.automaticLightmap;

			// Sun Shaft
			shaftDistance = LB_LightingProfile.shaftDistance;
			shaftBlur = LB_LightingProfile.shaftBlur;
			shaftColor = LB_LightingProfile.shaftColor;
			shaftQuality = LB_LightingProfile.shaftQuality;

			Ambient_Enabled = LB_LightingProfile.Ambient_Enabled;
			Scene_Enabled = LB_LightingProfile.Scene_Enabled;
			Sun_Enabled = LB_LightingProfile.Sun_Enabled;
			SunShaft_Enabled = LB_LightingProfile.SunShaft_Enabled;
			Fog_Enabled = LB_LightingProfile.Fog_Enabled;
			DOF_Enabled = LB_LightingProfile.DOF_Enabled;
			Bloom_Enabled = LB_LightingProfile.Bloom_Enabled;
			buildState = LB_LightingProfile.buildState;
			profileState = LB_LightingProfile.profileState;
			cameraState = LB_LightingProfile.cameraState;
			lightSettingsState = LB_LightingProfile.lightSettingsState;
			sunState = LB_LightingProfile.sunState;
			ambientState = LB_LightingProfile.ambientState;

			bloomState = LB_LightingProfile.bloomState;
			colorState = LB_LightingProfile.colorState;
			dofState = LB_LightingProfile.dofState;
			fogState = LB_LightingProfile.fogState;
			sunShaftState = LB_LightingProfile.sunShaftState;
			OptionsState = LB_LightingProfile.OptionsState;
			LightingBoxState = LB_LightingProfile.LightingBoxState;

			// Lightmapping
			directSamples = LB_LightingProfile.directSamples;
			indirectSamples = LB_LightingProfile.indirectSamples;
			aoIntensityDirect = LB_LightingProfile.aoIntensityDirect;
			aoIntensityIndirect = LB_LightingProfile.aoIntensityIndirect;
			aoEnabled = LB_LightingProfile.aoEnabled;
			aoDistance = LB_LightingProfile.aoDistance;
			indirectBounce = LB_LightingProfile.indirectBounce;
			backfaceTolerance = LB_LightingProfile.backfaceTolerance;

			mainCamera.allowHDR = false;
			mainCamera.allowMSAA = true;
		}

			UpdatePostEffects ();

			UpdateSettings ();

			Update_Sun();

	}
		#endregion

		#region Update Post Effects Settings

		public void UpdatePostEffects()
		{

			if(!helper)
				helper = GameObject.Find("LightingBox_Helper").GetComponent<LB_MobileLightingBoxHelper> ();

		helper.Update_Bloom(mainCamera,Bloom_Enabled,bIntensity,bThreshould);

			// Depth of Field 1 
		helper.Update_DOF(mainCamera,DOF_Enabled,dofQuality,dofBlur,dofRange,visualize);

			// Color Grading
		helper.Update_ColorGrading (mainCamera,color_Enabled, tonemap,exposure,contrast,gamma,saturation,colorR,colorG,colorB,vignette);

	}
		#endregion

		#region Scene Delegate

		string currentScene;    
		void SceneChanging ()
	{
		if (currentScene != EditorSceneManager.GetActiveScene ().name) {
			if (System.String.IsNullOrEmpty (EditorPrefs.GetString (EditorSceneManager.GetActiveScene ().name)))
				LB_LightingProfile = Resources.Load ("DefaultSettings")as LB_MobileLightingProfile;
			else
				LB_LightingProfile = (LB_MobileLightingProfile)AssetDatabase.LoadAssetAtPath (EditorPrefs.GetString (EditorSceneManager.GetActiveScene ().name), typeof(LB_MobileLightingProfile));

			helper.Update_MainProfile (LB_LightingProfile);

			OnLoad ();
			currentScene = EditorSceneManager.GetActiveScene ().name;
		}

	}
		#endregion

		#region Sun Light
		public void Update_Sun()
		{
		if (Sun_Enabled) {
			if (!RenderSettings.sun) {
				Light[] lights = GameObject.FindObjectsOfType<Light> ();
				foreach (Light l in lights) {
					if (l.type == LightType.Directional) {
						sunLight = l;

						if (sunColor != Color.clear)
							sunColor = l.color;
						else
							sunColor = Color.white;

						//sunLight.shadowNormalBias = 0.05f;  
						sunLight.color = sunColor;
						if (sunLight.bounceIntensity == 1f)
							sunLight.bounceIntensity = indirectIntensity;
					}
				}
			} else {		
				sunLight = RenderSettings.sun;

				if (sunColor != Color.clear)
					sunColor = sunLight.color;
				else
					sunColor = Color.white;

				//	sunLight.shadowNormalBias = 0.05f;  
				sunLight.color = sunColor;
				if (sunLight.bounceIntensity == 1f)
					sunLight.bounceIntensity = indirectIntensity;
			}
		}
	}

		#endregion

		#region On Download Completed
		void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
	{
		if (e.Error != null)
			Debug.Log (e.Error);
	}
		#endregion
}
#endif