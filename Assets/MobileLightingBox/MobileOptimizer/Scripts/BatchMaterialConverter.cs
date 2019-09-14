
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;

public enum MatType
{
	MobileDiffuse,
	MobileVertexLit,
	LegacyDiffuse,
	Standard,
	UnlitTexture,
}
public enum MaterialConverterType
{
	AllMaterials,
	CustomMaterials,
	CustomPath
}
public class BatchMaterialConverter : EditorWindow {

	[MenuItem("Window/Mobile Optimizer/Batch Material Converter")]
	static void Init()
	{
		BatchMaterialConverter window = (BatchMaterialConverter)EditorWindow.GetWindow(typeof(BatchMaterialConverter));
		window.Show();
		window.autoRepaintOnSceneChange = true;
	}

	Vector2 scrollPos;

	public MaterialConverterType converterType;
	public string[] CustomPath = {"Assets/MobileOptimizer/Materials"};
	public Material[] targets;
	MatType matType;
	Material m ;
	void OnGUI()
	{
		Undo.RecordObject (this,"lb");

		scrollPos = EditorGUILayout.BeginScrollView (scrollPos,
			false,
			false,
			GUILayout.Width(Screen.width ),   
			GUILayout.Height(Screen.height));
		
		EditorGUILayout.Space ();EditorGUILayout.Space ();
		EditorGUILayout.Space ();EditorGUILayout.Space ();

		converterType = (MaterialConverterType)EditorGUILayout.EnumPopup("Importer Mode",converterType,GUILayout.Width(343));
		matType = (MatType)EditorGUILayout.EnumPopup("Target Shader",matType,GUILayout.Width(343));

		if (GUILayout.Button ("Batch Convert")) {
			if (converterType == MaterialConverterType.AllMaterials) {
				var files = Directory.GetFiles ("Assets", "*.*", SearchOption.AllDirectories)
				.Where (s => s.EndsWith (".mat"));

				foreach (string s in files) {
					m = (Material)AssetDatabase.LoadAssetAtPath (s, typeof(Material));

					if (m.shader == Shader.Find ("Standard") || m.shader == Shader.Find ("Standard (Specular setup)")
					   || m.shader == Shader.Find ("Legacy Shaders/Diffuse") || m.shader == Shader.Find ("Legacy Shaders/Bumped Diffuse")
						|| m.shader == Shader.Find ("Legacy Shaders/Bumped Specular") || m.shader == Shader.Find ("Mobile/Diffuse")
						|| m.shader == Shader.Find ("Mobile/VertexLit") || m.shader == Shader.Find ("Unlit/Texture"))
					{
						if (matType == MatType.Standard)
							m.shader = Shader.Find ("Standard");
						if (matType == MatType.LegacyDiffuse)
							m.shader = Shader.Find ("Legacy Shaders/Diffuse");
						if (matType == MatType.MobileDiffuse)
							m.shader = Shader.Find ("Mobile/Diffuse");
						if (matType == MatType.MobileVertexLit)
							m.shader = Shader.Find ("Mobile/VertexLit");
						if (matType == MatType.UnlitTexture)
							m.shader = Shader.Find ("Unlit/Texture");
					}
				}
			}
			if (converterType == MaterialConverterType.CustomPath) {
				if (CustomPath.Length > 0) {
					foreach (string f in CustomPath) {
						var files = Directory.GetFiles (f, "*.*", SearchOption.AllDirectories)
							.Where (s => s.EndsWith (".mat"));

						foreach (string ss in files) {
							m = (Material)AssetDatabase.LoadAssetAtPath (ss, typeof(Material));

							if (m.shader == Shader.Find ("Standard") || m.shader == Shader.Find ("Standard (Specular setup)")
							    || m.shader == Shader.Find ("Legacy Shaders/Diffuse") || m.shader == Shader.Find ("Legacy Shaders/Bumped Diffuse")
							    || m.shader == Shader.Find ("Legacy Shaders/Bumped Specular") || m.shader == Shader.Find ("Mobile/Diffuse")
							    || m.shader == Shader.Find ("Mobile/VertexLit") || m.shader == Shader.Find ("Unlit/Texture")) {
								if (matType == MatType.Standard)
									m.shader = Shader.Find ("Standard");
								if (matType == MatType.LegacyDiffuse)
									m.shader = Shader.Find ("Legacy Shaders/Diffuse");
								if (matType == MatType.MobileDiffuse)
									m.shader = Shader.Find ("Mobile/Diffuse");
								if (matType == MatType.MobileVertexLit)
									m.shader = Shader.Find ("Mobile/VertexLit");
								if (matType == MatType.UnlitTexture)
									m.shader = Shader.Find ("Unlit/Texture");
							}
						}
					}
				}
			}
			if (converterType == MaterialConverterType.CustomMaterials) {
				if (targets.Length > 0) {
					foreach (Material s in targets) {
						if (matType == MatType.Standard)
							s.shader = Shader.Find ("Standard");
						if (matType == MatType.LegacyDiffuse)
							s.shader = Shader.Find ("Legacy Shaders/Diffuse");
						if (matType == MatType.MobileDiffuse)
							s.shader = Shader.Find ("Mobile/Diffuse");
						if (matType == MatType.MobileVertexLit)
							s.shader = Shader.Find ("Mobile/VertexLit");
						if (matType == MatType.UnlitTexture)
							s.shader = Shader.Find ("Unlit/Texture");
					}
				}
			}
		}
		EditorGUILayout.Space ();EditorGUILayout.Space ();

		if (converterType == MaterialConverterType.CustomMaterials) 
		{
			ScriptableObject target = this;
			SerializedObject so = new SerializedObject(target);
			SerializedProperty stringsProperty = so.FindProperty("targets");
			EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
			so.ApplyModifiedProperties(); // Remember to apply modified properties
		}

		if (converterType == MaterialConverterType.CustomPath) 
			{

			ScriptableObject target = this;
			SerializedObject so = new SerializedObject(target);
			SerializedProperty stringsProperty = so.FindProperty("CustomPath");
			EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
			so.ApplyModifiedProperties(); // Remember to apply modified properties
		}

		EditorGUILayout.Space ();EditorGUILayout.Space ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		if (converterType == MaterialConverterType.AllMaterials || converterType == MaterialConverterType.CustomPath)
			EditorGUILayout.LabelField("Transparent-Particles-Skybox materials will be skipped on this mode");
		EditorGUILayout.EndScrollView();

	}
}
#endif

