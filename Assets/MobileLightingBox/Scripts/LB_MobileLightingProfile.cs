
using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

[CreateAssetMenu(fileName = "Mobile Lighting Data", menuName = "Mobile Lighting Profile", order = 1)]
public class LB_MobileLightingProfile : ScriptableObject {

	[Header("Camera")]
	public string mainCameraName = "Main Camera";

	public string objectName = "LB_LightingProfile";

	[Header("Global")]

	public  LightSettings lightSettings = LightSettings.Mixed;

	[Header("Environment")]
	public Material skyBox;
	public  AmbientLight ambientLight = AmbientLight.Color;
	public  Color ambientColor = new Color32(96,104,116,255);
	public Color skyColor = Color.white;
	public Color equatorColor = Color.white,groundColor = Color.white;

	[Header("Sun")]
	public  Color sunColor = Color.white;
	public float sunIntensity = 2.1f;
	public Flare sunFlare;
	public float indirectIntensity = 1.43f;

	[Header("Fog")]
	public FogMode fogMode = FogMode.Exponential;
	public Color fogColor = Color.white;
	public float fogIntensity;
	public float linearStart;
	public float linearEnd;

	[Header("Bloom")]
	public float bIntensity = 0.73f;
	public float bThreshould = 0.5f;

	[Header("Lightmapping")]
	public  LightingMode lightingMode = LightingMode.RealtimeGI;
	public  float bakedResolution = 10f;

	// Progressive
	public int directSamples;
	public int indirectSamples;
	public float aoIntensityDirect;
	public float aoIntensityIndirect;
	public bool aoEnabled;
	public float aoDistance;
	public float indirectBounce;
	public float backfaceTolerance = 0.7f;

	[Header("Other")]
	public LightsShadow lightsShadow = LightsShadow.OnlyDirectionalSoft;
	public LightProbeMode lightProbesMode;
	public bool automaticLightmap = false;

	[Header("Depth of Field Legacy")]
	public float dofRange;
	public float dofBlur;   
	public DOFQuality dofQuality;
	public bool visualize;

	[Header("Color settings")]
	public float exposure;
	public float contrast;
	public float gamma;
	public float saturation;
	public float colorR;
	public float colorG;
	public float colorB;
	public ToneMapping tonemap = ToneMapping.ACES;
	public float vignette;

	[Header("Sun Shaft")]
	public SunShafts.SunShaftsResolution shaftQuality = SunShafts.SunShaftsResolution.High;
	public float shaftDistance = 0.5f;
	public float shaftBlur = 4f;
	public Color shaftColor = new Color32 (255,189,146,255);

	[Header("Enabled Options")]
	public bool Ambient_Enabled = true;
	public bool Scene_Enabled = true;
	public bool Sun_Enabled = true;
	public bool SunShaft_Enabled = false;
	public bool Fog_Enabled = false;
	public bool DOF_Enabled = true;
	public bool Bloom_Enabled = false;
	public bool color_Enabled = true;

	public bool ambientState = false;
	public bool sunState = false;
	public bool lightSettingsState = false;
	public bool cameraState = false;
	public bool profileState = false;
	public bool buildState = false;
	public bool sunShaftState = false;
	public bool fogState = false;
	public bool dofState = false;
	public bool colorState = false;
	public bool bloomState = false;
	public bool OptionsState = true;
	public bool LightingBoxState = true;
}