// Use this script to get runtime access to the lighting box econtrolled effects
/// <summary>
/// example :
/// 
/// // Update bloom effect .
/// void Start ()
/// {
///   	GameObject.FindObjectOfType<LB_MobileLightingBoxHelper> ().Update_Bloom (true, 1f, 0.5f, Color.white);
/// }
/// </summary>
using UnityEngine;   
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

#region Emum Types
public enum WindowMode
{
	Part1,Part2,Part3,
	Finish
}
public enum AmbientLight
{
	Skybox,
	Color,
	Gradient
}
public enum LightingMode
{
	BakedProgressive,
	BakedEnlighten,
	RealtimeGI,
}
public enum LightSettings
{
	Default,
	Realtime,
	Mixed,
	Baked
}
public enum LightsShadow
{
	OnlyDirectionalSoft,OnlyDirectionalHard,
	AllLightsSoft,AllLightsHard,
	Off
}
public enum LightProbeMode
{
	Blend,
	Proxy
}
public enum DOFQuality
{
	Low,Medium,High
}
#endregion

public class LB_MobileLightingBoxHelper : MonoBehaviour {

	public LB_MobileLightingProfile mainLightingProfile;

	public void Update_MainProfile(LB_MobileLightingProfile profile)
	{
		if(profile)
			mainLightingProfile = profile;
	}

	public void Update_Bloom(Camera mainCamera,bool enabled,float intensity,float threshold)
	{
		if(enabled)
		{
			if (!mainCamera.GetComponent<BloomOptimized> ()) {
				mainCamera.gameObject.AddComponent<BloomOptimized> ();
				BloomOptimized bloom = mainCamera.GetComponent<BloomOptimized> ();

				#if UNITY_EDITOR
				if (mainCamera.GetComponent<AudioListener> ())
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<AudioListener> ());

				if (mainCamera.GetComponent<MobileColorGrading> ()) {
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<MobileColorGrading> ());
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<MobileColorGrading> ());
				}

				if(mainCamera.GetComponent<BloomOptimized> ())
				{
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<BloomOptimized> ());
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<BloomOptimized> ());
				}

				if (mainCamera.GetComponent<DepthOfFieldDeprecated> ()) {
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<DepthOfFieldDeprecated> ());
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<DepthOfFieldDeprecated> ());
				}
				#endif
				bloom.fastBloomShader = Shader.Find ("Hidden/FastBloom");
				bloom.intensity = intensity;
				bloom.threshold = threshold;
				bloom.blurSize = 5.5f;
				bloom.blurIterations = 4;
				bloom.blurType = BloomOptimized.BlurType.Standard;
				#if UNITY_EDITOR

				UnityEditorInternal.ComponentUtility.MoveComponentUp(bloom);
				#endif
			} else {
				BloomOptimized bloom = mainCamera.GetComponent<BloomOptimized> ();

				bloom.intensity = intensity;
				bloom.threshold = threshold;
				bloom.blurSize = 5.5f;
				bloom.blurIterations = 4;
				bloom.blurType = BloomOptimized.BlurType.Standard;

			}
		}
		else
		{
			if (mainCamera.GetComponent<BloomOptimized> ()) {
				if(Application.isPlaying)
					Destroy (mainCamera.GetComponent<BloomOptimized> ());					
				else
					DestroyImmediate (mainCamera.GetComponent<BloomOptimized> ());
			}
		}   
	}

	public void Update_DOF(Camera mainCamera ,bool enabled,DOFQuality quality,float blur,float range,bool visualize   )
	{
		if (enabled) {
			if (!mainCamera.GetComponent<DepthOfFieldDeprecated> ()) {
				mainCamera.gameObject.AddComponent<DepthOfFieldDeprecated> ();
				DepthOfFieldDeprecated dof = mainCamera.GetComponent<DepthOfFieldDeprecated> ();
				#if UNITY_EDITOR

				if (mainCamera.GetComponent<AudioListener> ())
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<AudioListener> ());

				if (mainCamera.GetComponent<MobileColorGrading> ()) {
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<MobileColorGrading> ());
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<MobileColorGrading> ());
				}

				if(mainCamera.GetComponent<BloomOptimized> ())
				{
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<BloomOptimized> ());
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<BloomOptimized> ());
				}

				if (mainCamera.GetComponent<DepthOfFieldDeprecated> ()) {
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<DepthOfFieldDeprecated> ());
					UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<DepthOfFieldDeprecated> ());
				}
				#endif
				dof.bokehShader = Shader.Find ("Hidden/Dof/Bokeh34");
				dof.dofBlurShader = Shader.Find ("Hidden/SeparableWeightedBlurDof34");
				dof.dofShader = Shader.Find ("Hidden/Dof/DepthOfField34");

				dof.simpleTweakMode = true;
				if(quality == DOFQuality.Low)
					dof.resolution = DepthOfFieldDeprecated.DofResolution.Low;
				if(quality == DOFQuality.Medium)
					dof.resolution = DepthOfFieldDeprecated.DofResolution.Medium;
				if(quality == DOFQuality.High)
					dof.resolution = DepthOfFieldDeprecated.DofResolution.High;
				
				dof.visualize = visualize;
				dof.smoothness = range;
				dof.bluriness = DepthOfFieldDeprecated.DofBlurriness.Low;
				dof.maxBlurSpread = blur;
				
			} else {
				DepthOfFieldDeprecated dof = mainCamera.GetComponent<DepthOfFieldDeprecated> ();


				dof.simpleTweakMode = true;
				if(quality == DOFQuality.Low)
					dof.resolution = DepthOfFieldDeprecated.DofResolution.Low;
				if(quality == DOFQuality.Medium)
					dof.resolution = DepthOfFieldDeprecated.DofResolution.Medium;
				if(quality == DOFQuality.High)
					dof.resolution = DepthOfFieldDeprecated.DofResolution.High;

				dof.visualize = visualize;
				dof.smoothness = range;
				dof.bluriness = DepthOfFieldDeprecated.DofBlurriness.Low;
				dof.maxBlurSpread = blur;

			}
		}
		if (!enabled) {
			if (mainCamera.GetComponent<DepthOfFieldDeprecated> ()) {
				if(Application.isPlaying)
					Destroy (mainCamera.GetComponent<DepthOfFieldDeprecated> ());					
				else
					DestroyImmediate (mainCamera.GetComponent<DepthOfFieldDeprecated> ());
			}

		}
	}

	public void Update_Foliage(float translucency, float ambient,float shadows, float windSpeed
		,float windScale, Color transColor)
	{
		Shader.SetGlobalFloat ("_WindScale", windScale);
		Shader.SetGlobalFloat ("_WindSpeed", windSpeed);

		Shader.SetGlobalColor ("_TranslucencyColor", transColor);
		Shader.SetGlobalFloat ("_TranslucencyIntensity", translucency);
		Shader.SetGlobalFloat ("_TransAmbient", ambient);
		Shader.SetGlobalFloat ("_TransShadow", shadows);
	}

	public void Update_ConvertMaterials()
	{

	}


	public void Update_ConvertSnowMaterials(string customShader)
	{
		
	}

	public void Update_ColorGrading(Camera mainCamera ,bool enabled,ToneMapping mode,float exposure,float contrast,float gamma,float saturation,float r,float g, float b,float vignette)
	{
		
		if (enabled) {
			if (!mainCamera.GetComponent<MobileColorGrading> ())
				mainCamera.gameObject.AddComponent<MobileColorGrading> ();

			MobileColorGrading colorGrading = mainCamera.GetComponent<MobileColorGrading> ();
			#if UNITY_EDITOR

			if (mainCamera.GetComponent<AudioListener> ())
				UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<AudioListener> ());

			if (mainCamera.GetComponent<MobileColorGrading> ()) {
				UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<MobileColorGrading> ());
				UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<MobileColorGrading> ());
			}

			if(mainCamera.GetComponent<BloomOptimized> ())
			{
				UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<BloomOptimized> ());
				UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<BloomOptimized> ());
			}

			if (mainCamera.GetComponent<DepthOfFieldDeprecated> ()) {
				UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<DepthOfFieldDeprecated> ());
				UnityEditorInternal.ComponentUtility.MoveComponentDown (mainCamera.GetComponent<DepthOfFieldDeprecated> ());
			}
			#endif
			if (!colorGrading.shader)
				colorGrading.shader = Shader.Find ("Hidden/ALIyerEdon/ColorGrading");
			colorGrading.Exposure = exposure;
			colorGrading.toneMapping = mode;
			colorGrading.Contrast = contrast;
			colorGrading.Gamma = gamma;
			colorGrading.Saturation = saturation;
			colorGrading.R = r;
			colorGrading.G = g;
			colorGrading.B = b;
			colorGrading.vignetteIntensity = vignette;
		} else {
			if (mainCamera.GetComponent<MobileColorGrading> ())
			if (Application.isPlaying) {
				Destroy (mainCamera.GetComponent<MobileColorGrading> ());
			} else {
				DestroyImmediate (mainCamera.GetComponent<MobileColorGrading> ());
			}			
		}
	}

	public void Update_SkyBox(bool enabled , Material material)
	{
		if (enabled) {
			if (material)
				RenderSettings.skybox = material;
		}		
	}

	public static Vector2 getMainGameViewSize ()
	{
		//The first thing I needed to do was get the GameView type. Since this was an internal type, I needed to make a special call using Type.GetType, which passes the class and the assembly that the class belongs to. The GameView class is within the UnityEditor assembly
		Type gameViewType = Type.GetType ("UnityEditor.GameView,UnityEditor");

		//Next I needed to get the MethodInfo of the GetSizeOfMainGameView function. MethodInfo holds all of the information about a function, including parameters, return types, and attributes. Also since GetSizeOfMainGameView is a static internal function, I needed to pass the NonPublic and Static binding flags so that the code knew where to look
		MethodInfo getSizeOfGameView = gameViewType.GetMethod ("GetSizeOfMainGameView", BindingFlags.NonPublic | BindingFlags.Static);

		//Now that I had the MethodInfo I wanted, I could invoke the function. The Invoke call has two parameters, the first is for the object that the function is being called on, and the second is for the parameters. Since this was a static function, I pass null for the first parameter, and since the function has no parameters, I pass null again for the second parameter
		return (Vector2)getSizeOfGameView.Invoke (null, null);
	}

	public void Update_LightingMode(bool enabled, LightingMode lightingMode, float indirectBounce, int directSamples, int indirectSamples,bool aoEnabled, float aoDistance, float aoIntensityDirect, float aoIntensityIndirect,float backfaceTolerance, float resolution)
	{
		if (enabled)
		{
			#if UNITY_EDITOR

			LB_LightingSettingsHepler.SetDirectSamples (directSamples);
			LB_LightingSettingsHepler.SetIndirectSamples (indirectSamples);
			LightmapEditorSettings.bakeResolution = resolution;
			LightmapParameters a = new LightmapParameters ();
			a.name = "Lighting Box Very-Low";
			a.resolution = 0.125f;
			a.clusterResolution = 0.4f;
			a.irradianceBudget = 96;
			a.irradianceQuality = 8192;
			a.modellingTolerance = 0.001f;
			a.stitchEdges = true;
			a.isTransparent = false;
			a.systemTag = -1;
			a.blurRadius = 2;
			a.antiAliasingSamples = 8;
			a.directLightQuality = 64;
			a.bakedLightmapTag = -1;
			a.AOQuality = 256;
			a.AOAntiAliasingSamples = 16;
			a.backFaceTolerance = backfaceTolerance;

			LB_LightingSettingsHepler.SetLightmapParameters (a);

			LB_LightingSettingsHepler.SetDirectionalMode (LightmapsMode.NonDirectional);
			LB_LightingSettingsHepler.SetAmbientOcclusion (aoEnabled);
			LB_LightingSettingsHepler.SetAmbientOcclusionDirect (aoIntensityDirect);
			LB_LightingSettingsHepler.SetAmbientOcclusionDistance (aoDistance);
			LB_LightingSettingsHepler.SetAmbientOcclusionIndirect (aoIntensityIndirect);
			LB_LightingSettingsHepler.SetBounceIntensity (indirectBounce);
			#endif
			RenderSettings.reflectionIntensity = 0;
			RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;

			#if UNITY_EDITOR
			if (lightingMode == LightingMode.RealtimeGI) {
				Lightmapping.realtimeGI = true;
				Lightmapping.bakedGI = false;
				LightmapEditorSettings.lightmapper = LightmapEditorSettings.Lightmapper.Enlighten;
			}
			if (lightingMode == LightingMode.BakedProgressive) {
				Lightmapping.realtimeGI = false;
				Lightmapping.bakedGI = true;
				LightmapEditorSettings.lightmapper = LightmapEditorSettings.Lightmapper.ProgressiveCPU;
			}
			if (lightingMode == LightingMode.BakedEnlighten) {
				Lightmapping.realtimeGI = false;
				Lightmapping.bakedGI = true;
				LightmapEditorSettings.lightmapper = LightmapEditorSettings.Lightmapper.Enlighten;
			}
			#endif
		}
	}

	public void Update_Ambient(bool enabled,AmbientLight ambientLight,Color ambientColor,Color skyColor,Color equatorColor
		,Color groundColor)
	{
		if (enabled) {
			if (ambientLight == AmbientLight.Color) {
				RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
				RenderSettings.ambientLight = ambientColor;
			}
			if (ambientLight == AmbientLight.Skybox)
				RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
			if (ambientLight == AmbientLight.Gradient) {
				RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
				RenderSettings.ambientSkyColor = skyColor;
				RenderSettings.ambientEquatorColor = equatorColor;
				RenderSettings.ambientGroundColor = groundColor;
			}
		}
	}

	#if UNITY_EDITOR
	public void Update_LightSettings(bool enabled, LightSettings lightSettings)
	{
		if(enabled)
		{
			if (lightSettings == LightSettings.Baked) {

				Light[] lights = GameObject.FindObjectsOfType<Light> ();

				foreach (Light l in lights) {
					SerializedObject serialLightSource = new SerializedObject(l);
					SerializedProperty SerialProperty  = serialLightSource.FindProperty("m_Lightmapping");
					SerialProperty.intValue = 2;
					serialLightSource.ApplyModifiedProperties ();
				}   
			}    
			if (lightSettings == LightSettings.Realtime) {

				Light[] lights = GameObject.FindObjectsOfType<Light> ();

				foreach (Light l in lights) {
					SerializedObject serialLightSource = new SerializedObject(l);
					SerializedProperty SerialProperty  = serialLightSource.FindProperty("m_Lightmapping");
					SerialProperty.intValue = 4;
					serialLightSource.ApplyModifiedProperties ();
				}
			}
			if (lightSettings == LightSettings.Mixed) {

				Light[] lights = GameObject.FindObjectsOfType<Light> ();

				foreach (Light l in lights) {
					SerializedObject serialLightSource = new SerializedObject(l);
					SerializedProperty SerialProperty  = serialLightSource.FindProperty("m_Lightmapping");
					SerialProperty.intValue = 1;
					serialLightSource.ApplyModifiedProperties ();
				}

			}
		}
	}
		
	public void Update_ColorSpace(bool enabled)
	{
		PlayerSettings.colorSpace = ColorSpace.Gamma;
	}

	public void Update_AutoMode(bool enabled)
	{
		if(enabled)
			Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.Iterative;
		else
			Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.OnDemand;
	}

	public void Update_LightProbes(bool enabled, LightProbeMode lightProbesMode)
	{
		if (enabled) {
			if (lightProbesMode == LightProbeMode.Blend) {

				MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer> ();

				foreach (MeshRenderer mr in renderers) {
					if (!mr.gameObject.isStatic) {
						if (mr.gameObject.GetComponent<LightProbeProxyVolume> ()) {
							if (Application.isPlaying)
								Destroy (mr.gameObject.GetComponent<LightProbeProxyVolume> ());
							else
								DestroyImmediate (mr.gameObject.GetComponent<LightProbeProxyVolume> ());
						}
						mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
					}
				}
			}
			if (lightProbesMode == LightProbeMode.Proxy) {

				MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer> ();

				foreach (MeshRenderer mr in renderers) {

					if (!mr.gameObject.isStatic) {
						if (!mr.gameObject.GetComponent<LightProbeProxyVolume> ())
							mr.gameObject.AddComponent<LightProbeProxyVolume> ();
						mr.gameObject.GetComponent<LightProbeProxyVolume> ().resolutionMode = LightProbeProxyVolume.ResolutionMode.Custom;
						mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.UseProxyVolume;
					}
				}
			}
		}
	}

	public void Update_Shadows(bool enabled, LightsShadow lightsShadow)
	{
		if (enabled) {
			if (lightsShadow == LightsShadow.AllLightsSoft) {

				Light[] lights = GameObject.FindObjectsOfType<Light> ();

				foreach (Light l in lights) {
					if (l.type == LightType.Directional)
						l.shadows = LightShadows.Soft;

					if (l.type == LightType.Spot)
						l.shadows = LightShadows.Soft;

					if (l.type == LightType.Point)
						l.shadows = LightShadows.Soft;
				}
			}
			if (lightsShadow == LightsShadow.AllLightsHard) {

				Light[] lights = GameObject.FindObjectsOfType<Light> ();

				foreach (Light l in lights) {
					if (l.type == LightType.Directional)
						l.shadows = LightShadows.Hard;

					if (l.type == LightType.Spot)
						l.shadows = LightShadows.Hard;

					if (l.type == LightType.Point)
						l.shadows = LightShadows.Hard;
				}
			}
			if (lightsShadow == LightsShadow.OnlyDirectionalSoft) {

				Light[] lights = GameObject.FindObjectsOfType<Light> ();

				foreach (Light l in lights) {
					if (l.type == LightType.Directional)
						l.shadows = LightShadows.Soft;

					if (l.type == LightType.Spot)
						l.shadows = LightShadows.None;

					if (l.type == LightType.Point)
						l.shadows = LightShadows.None;
				}
			}
			if (lightsShadow == LightsShadow.OnlyDirectionalHard) {

				Light[] lights = GameObject.FindObjectsOfType<Light> ();

				foreach (Light l in lights) {
					if (l.type == LightType.Directional)
						l.shadows = LightShadows.Hard;

					if (l.type == LightType.Spot)
						l.shadows = LightShadows.None;

					if (l.type == LightType.Point)
						l.shadows = LightShadows.None;
				}
			}
			if (lightsShadow == LightsShadow.Off) {
				Light[] lights = GameObject.FindObjectsOfType<Light> ();

				foreach (Light l in lights)
					l.shadows = LightShadows.None;



			}
		}
	}

	#endif

	public void Update_SunShaft(Camera mainCamera,bool enabled, SunShafts.SunShaftsResolution shaftQuality,float shaftDistance,float shaftBlur, Color shaftColor, Transform sun)
	{
		if (!sun) {
			Debug.Log ("Couldn't find sun for Sun Shaft effect");
			if (mainCamera.gameObject.GetComponent<SunShafts> ()) {
				if (Application.isPlaying)
					Destroy (mainCamera.gameObject.GetComponent<SunShafts> ());
				else
					DestroyImmediate (mainCamera.gameObject.GetComponent<SunShafts> ());
			}
			return;
		}

		if (enabled) {
			if (!mainCamera.gameObject.GetComponent<SunShafts> ())
				mainCamera.gameObject.AddComponent<SunShafts> ();

			mainCamera.gameObject.GetComponent<SunShafts> ().sunShaftsShader = Shader.Find
			("Hidden/SunShaftsComposite");
			mainCamera.gameObject.GetComponent<SunShafts> ().simpleClearShader = Shader.Find
			("Hidden/SimpleClear");
			mainCamera.gameObject.GetComponent<SunShafts> ().resolution = shaftQuality;
			mainCamera.gameObject.GetComponent<SunShafts> ().screenBlendMode = SunShafts.ShaftsScreenBlendMode.Screen;
			mainCamera.gameObject.GetComponent<SunShafts> ().sunShaftIntensity = 1f;
			mainCamera.gameObject.GetComponent<SunShafts> ().sunThreshold = Color.black;
			mainCamera.gameObject.GetComponent<SunShafts> ().sunColor = shaftColor;
			mainCamera.gameObject.GetComponent<SunShafts> ().sunShaftBlurRadius = shaftBlur;
			mainCamera.gameObject.GetComponent<SunShafts> ().radialBlurIterations = 2;
			mainCamera.gameObject.GetComponent<SunShafts> ().maxRadius = shaftDistance;
			if (!GameObject.Find ("Shaft Caster")) {
				GameObject shaftCaster = new GameObject ("Shaft Caster");
				shaftCaster.transform.parent = sun;
				shaftCaster.transform.localPosition = new Vector3 (0, 0, -7000f);
				mainCamera.gameObject.GetComponent<SunShafts> ().sunTransform = shaftCaster.transform;
			} else {
				GameObject.Find ("Shaft Caster").transform.parent = sun;
				GameObject.Find ("Shaft Caster").transform.localPosition = new Vector3 (0, 0, -7000f);
				mainCamera.gameObject.GetComponent<SunShafts> ().sunTransform = GameObject.Find ("Shaft Caster").transform;
			}
		} else {
			if (mainCamera.gameObject.GetComponent<SunShafts> ()) {
				if (Application.isPlaying)
					Destroy (mainCamera.gameObject.GetComponent<SunShafts> ());
				else
					DestroyImmediate (mainCamera.gameObject.GetComponent<SunShafts> ());
			}
		}
	}

	public void Update_GlobalFog(Camera mainCamera, bool enabled, FogMode fogMode,Color fogColor,
		float fogIntensity, float linearStart,float linearEnd)
	{
		RenderSettings.fog = enabled;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogIntensity;
		RenderSettings.fogMode = fogMode;
		RenderSettings.fogStartDistance = linearStart;
		RenderSettings.fogEndDistance = linearEnd;
	}

	public void Update_Sun(bool enabled,Light sunLight,Color sunColor,float indirectIntensity)
	{
		if (enabled) {
			if (!RenderSettings.sun) {
				Light[] lights = GameObject.FindObjectsOfType<Light> ();

				foreach (Light l in lights) {
					if (l.type == LightType.Directional) {
						sunLight = l;

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
			} else {
				sunLight = RenderSettings.sun;

				if (sunColor != Color.clear)
					sunColor = sunLight.color;
				else
					sunColor = Color.white;

				//sunLight.shadowNormalBias = 0.05f;  
				sunLight.color = sunColor;
				if (sunLight.bounceIntensity == 1f)
					sunLight.bounceIntensity = indirectIntensity;
			}
		}
	}

	bool effectsIsOn = true;

	public void Toggle_Effects()
	{
		effectsIsOn = !effectsIsOn;

		// Depth of Field
		DepthOfFieldDeprecated dof = GameObject.FindObjectOfType<DepthOfFieldDeprecated> ();
		if(dof)
			dof.enabled = effectsIsOn;
		
		// Bloom
		BloomOptimized bloom = GameObject.FindObjectOfType<BloomOptimized> ();
		if(bloom)
			bloom.enabled = effectsIsOn;


		// Sun Shaft
		SunShafts[] sunShaftS = GameObject.FindObjectsOfType<SunShafts> ();
		for (int a = 0; a < sunShaftS.Length; a++)
			sunShaftS [a].enabled = effectsIsOn;

		// Color Grading
		MobileColorGrading colorGrading = GameObject.FindObjectOfType<MobileColorGrading> ();
		if(colorGrading)
			colorGrading.enabled = effectsIsOn;
		

	}
}
