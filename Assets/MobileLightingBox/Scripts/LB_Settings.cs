//--------------------------------------------------------------
//
//                    Car Parking kit 3.0
//        
//           Contact me : aliyeredon@gmail.com
//
//--------------------------------------------------------------

// This script used for game settings menu

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class LB_Settings : MonoBehaviour
{

	public Toggle Shaft,colorGrading,bloom,ShowFPS,vSync;

	public Dropdown resolutionDrop, qualityDrop,antiAliasing,textureSampler;

	public int[] resolutionListWidth;
	public int[] resolutionListHeight;

	Camera mainCamera;

	[Header("First Run Values")]
	public bool sunShaftEnabled = false;
	public bool ColorGardingEnabled = true;
	public bool BloomEnabled = true;
	public bool showFpsEnabled = false;
	public bool vSyncEnabled = true;
	public int resolutionValue = 0;
	public int qualityValue = 0;
	public int AAValue = 0;
	public int textureValue = 0;

	void Start ()
	{
		if (PlayerPrefs.GetInt ("FirstRun") != 3) {  
			if(sunShaftEnabled)
				PlayerPrefs.SetInt("Shaft",3);
			if(ColorGardingEnabled)
				PlayerPrefs.SetInt("colorGrading",3);
			if(BloomEnabled)
				PlayerPrefs.SetInt("Bloom",3);
			if(showFpsEnabled)
				PlayerPrefs.SetInt("ShowFPS",3);
			if(vSyncEnabled)
				PlayerPrefs.SetInt("vSync",3);

			PlayerPrefs.SetInt("Resolution",resolutionValue);
			PlayerPrefs.SetInt("Quality",qualityValue);
			PlayerPrefs.SetInt("AntiAliasing",AAValue);
			PlayerPrefs.SetInt("TextureSampler",textureValue);

			PlayerPrefs.SetInt ("FirstRun", 3);
		}

		mainCamera = GameObject.Find (GameObject.FindObjectOfType<LB_MobileLightingBoxHelper>().mainLightingProfile.mainCameraName).GetComponent<Camera> ();

		// Read starting setting values
		if (PlayerPrefs.GetInt ("Shaft") == 3)
			Shaft.isOn = true;
		else
			Shaft.isOn = false;

		if (PlayerPrefs.GetInt ("colorGrading") == 3)
			colorGrading.isOn = true;
		else
			colorGrading.isOn = false;

		if (PlayerPrefs.GetInt ("ShowFPS") == 3)
			ShowFPS.isOn = true;
		else
			ShowFPS.isOn = false;

		if (PlayerPrefs.GetInt ("Bloom") == 3)
			bloom.isOn = true;
		else
			bloom.isOn = false;
		
		if (PlayerPrefs.GetInt ("vSync") == 3)
			vSync.isOn = true;
		else
			vSync.isOn = false;
		
		qualityDrop.value =PlayerPrefs.GetInt ("Quality");

		resolutionDrop.value = PlayerPrefs.GetInt ("Resolution");

		antiAliasing.value = PlayerPrefs.GetInt ("AntiAliasing");

		textureSampler.value = PlayerPrefs.GetInt ("TextureSampler");

		UpdateSettings ("Resolution");
		UpdateSettings ("AntiAliasing");
		UpdateSettings ("TextureSampler");
		UpdateSettings ("ColorGrading");	
		UpdateSettings ("ShowFPS");	
		UpdateSettings ("Bloom");	
		UpdateSettings ("vSync");		
		UpdateSettings ("Shaft");

	}

	public void Set_Shaft ()
	{
		StartCoroutine (Shaft_Save ());
	}

	public void Set_colorGrading ()
	{
		StartCoroutine (colorGrading_Save ());
	}

	public void Set_ShowFPS ()
	{
		StartCoroutine (ShowFPS_Save ());
	}

	public void Set_bloom ()
	{
		StartCoroutine (Bloom_Save ());
	}

	public void Set_vSync ()
	{
		StartCoroutine (vSync_Save ());
	}

	public void SetQualityLevel ()
	{
		PlayerPrefs.SetInt ("Quality", qualityDrop.value);
		QualitySettings.SetQualityLevel (qualityDrop.value, false);
	}

	public void SetResolution ()
	{
		PlayerPrefs.SetInt ("Resolution", resolutionDrop.value);
		UpdateSettings ("Resolution");
	}

	public void SetAntiAliasing ()
	{
		PlayerPrefs.SetInt ("AntiAliasing", antiAliasing.value);
		UpdateSettings ("AntiAliasing");

	}

	public void SetTextureSampler ()
	{
		PlayerPrefs.SetInt ("TextureSampler", textureSampler.value);
		UpdateSettings ("TextureSampler");
	}

	IEnumerator colorGrading_Save ()
	{
		yield return new WaitForEndOfFrame ();
		if (colorGrading.isOn)
			PlayerPrefs.SetInt ("colorGrading", 3);  //3 = true;
		else
			PlayerPrefs.SetInt ("colorGrading", 0);//0 = false;

		UpdateSettings ("ColorGrading");
	}

	IEnumerator ShowFPS_Save ()
	{
		yield return new WaitForEndOfFrame ();
		if (ShowFPS.isOn)
			PlayerPrefs.SetInt ("ShowFPS", 3);  //3 = true;
		else
			PlayerPrefs.SetInt ("ShowFPS", 0);//0 = false;

		UpdateSettings ("ShowFPS");
	}

	IEnumerator Bloom_Save ()
	{
		yield return new WaitForEndOfFrame ();
		if (bloom.isOn)
			PlayerPrefs.SetInt ("Bloom", 3);  //3 = true;
		else
			PlayerPrefs.SetInt ("Bloom", 0);//0 = false;

		UpdateSettings ("Bloom");
	}

	IEnumerator vSync_Save ()
	{
		yield return new WaitForEndOfFrame ();
		if (vSync.isOn)
			PlayerPrefs.SetInt ("vSync", 3);  //3 = true;
		else
			PlayerPrefs.SetInt ("vSync", 0);//0 = false;

		UpdateSettings ("vSync");

	}

	IEnumerator Shaft_Save ()
	{
		yield return new WaitForEndOfFrame ();
		if (Shaft.isOn)
			PlayerPrefs.SetInt ("Shaft", 3);  //3 = true;
		else
			PlayerPrefs.SetInt ("Shaft", 0);//0 = false;

		UpdateSettings ("Shaft");
	}


	void UpdateSettings(string name)
	{

		if (!mainCamera)
			return;

		if (mainCamera.GetComponent<SunShafts> ()) {
			if (name == "Shaft") {
				if (PlayerPrefs.GetInt ("Shaft") != 3)
					mainCamera.GetComponent<SunShafts> ().enabled = false;
				else
					mainCamera.GetComponent<SunShafts> ().enabled = true;
			}
		}

		if (mainCamera.GetComponent<MobileColorGrading> ()) {
			if (name == "ColorGrading") {
				if (PlayerPrefs.GetInt ("colorGrading") != 3)
					mainCamera.GetComponent<MobileColorGrading> ().enabled = false;
				else
					mainCamera.GetComponent<MobileColorGrading> ().enabled = true;
			}
		}


			if (name == "ShowFPS") {
			if (PlayerPrefs.GetInt ("ShowFPS") != 3) {
				if (mainCamera.GetComponent<LB_ShowFPS> ()) {
					Destroy (mainCamera.GetComponent<LB_ShowFPS> ());
				}
			}
				else
					mainCamera.gameObject.AddComponent<LB_ShowFPS> ();
			}

		if (name == "Bloom") {
			if (PlayerPrefs.GetInt ("Bloom") != 3) {
				if(mainCamera.GetComponent<BloomOptimized> ())
					mainCamera.GetComponent<BloomOptimized> ().enabled = false;
			}
			else {
				if(mainCamera.GetComponent<BloomOptimized> ())
					mainCamera.GetComponent<BloomOptimized> ().enabled = true;
			}
		}

		if (name == "Resolution") {
			Screen.SetResolution (resolutionListWidth [PlayerPrefs.GetInt ("Resolution")], resolutionListHeight [PlayerPrefs.GetInt ("Resolution")], true);

			Camera[] c = GameObject.FindObjectsOfType<Camera> ();

			for (int a = 0; a < c.Length; a++)
				c [a].aspect = 16f / 9f;
		}

		if (name == "AntiAliasing") {
			if (PlayerPrefs.GetInt ("AntiAliasing") == 0)
				QualitySettings.antiAliasing = 0;
			if (PlayerPrefs.GetInt ("AntiAliasing") == 1)
				QualitySettings.antiAliasing = 2;
			if (PlayerPrefs.GetInt ("AntiAliasing") == 2)
				QualitySettings.antiAliasing = 4;
			if (PlayerPrefs.GetInt ("AntiAliasing") == 3)
				QualitySettings.antiAliasing = 8;
		}

		if (name == "TextureSampler")
				QualitySettings.masterTextureLimit = PlayerPrefs.GetInt ("TextureSampler");

		if (name == "vSync") {
			if(PlayerPrefs.GetInt("vSync") == 0)
				QualitySettings.vSyncCount = 0;
			if(PlayerPrefs.GetInt("vSync") == 3)
				QualitySettings.vSyncCount = 1;
		}
	}

	public void SetTrue(GameObject target)
	{
		target.SetActive (true);
	}

	public void SetFalse(GameObject target)
	{
		target.SetActive (false);
	}

	public void ToggleGameObject(GameObject target)
	{
		target.SetActive (!target.activeSelf);
	}
}
