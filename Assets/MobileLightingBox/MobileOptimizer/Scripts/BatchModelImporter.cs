
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;

public enum ModelImporterType
{
	AllModels,
	CustomModels,
	CustomPath
}

public class BatchModelImporter : EditorWindow {

	[MenuItem("Window/Mobile Optimizer/Batch Model Importer")]
	static void Init()
	{
		BatchModelImporter window = (BatchModelImporter)EditorWindow.GetWindow(typeof(BatchModelImporter));
		window.Show();
		window.autoRepaintOnSceneChange = true;
	}

	Vector2 scrollPos;

	public ModelImporterType importerType;
	public string[] CustomPath = {"Assets/MobileOptimizer/Models/Custom1"};
	public GameObject[] targets;
	public ModelImporterMeshCompression meshCompression = ModelImporterMeshCompression.Off;

	public float modelScale = 1f;

	public bool readWriteEnabled = false;
	public bool optimizeMesh = true;
	public bool importBlendShape = false;

	public bool generateColliders = false;
	public bool keepQuads = false;
	public bool weldVertices = true;
	public bool swapUV = false;
	public bool ImportMaterials = true;

	public bool generateLightmapUV = true;
	public bool importAnimation = false;
	bool advancedMode;

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

		importerType = (ModelImporterType)EditorGUILayout.EnumPopup("Importer Mode",importerType,GUILayout.Width(343));

		if (GUILayout.Button ("Batch Import")) 
		{
			if (importerType == ModelImporterType.CustomModels) {
				if (targets.Length > 0) {
					foreach (GameObject s in targets) 
					{
						ModelImporter tImporter = AssetImporter.GetAtPath (AssetDatabase.GetAssetPath (s)) as ModelImporter;

						tImporter.meshCompression = meshCompression;
						tImporter.isReadable = readWriteEnabled;
						tImporter.optimizeMesh = optimizeMesh;
						tImporter.importBlendShapes = importBlendShape;
						tImporter.generateSecondaryUV = generateLightmapUV;
						tImporter.addCollider = generateColliders;

						if (advancedMode)
						{						tImporter.importAnimation = importAnimation;
							
							tImporter.globalScale = modelScale;
							tImporter.keepQuads = keepQuads;
							tImporter.weldVertices = weldVertices;
							tImporter.swapUVChannels = swapUV;
							tImporter.importMaterials = ImportMaterials;
						}

						AssetDatabase.ImportAsset (AssetDatabase.GetAssetPath (s), ImportAssetOptions.ForceUpdate);
					}
				}
			}

			if (importerType == ModelImporterType.AllModels) {


				var files = Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories)
					.Where(s => s.EndsWith(".fbx") || s.EndsWith(".obj") || s.EndsWith(".ma") || s.EndsWith(".max") || s.EndsWith(".dae")
						|| s.EndsWith(".blend") || s.EndsWith(".dxf") || s.EndsWith(".3ds") || s.EndsWith(".skp")
						|| s.EndsWith(".c4d") || s.EndsWith(".lxo ") || s.EndsWith(".jas"));
				
				foreach  (string s in files)
				{
					ModelImporter tImporter = AssetImporter.GetAtPath (s) as ModelImporter;

					tImporter.meshCompression = meshCompression;
					tImporter.isReadable = readWriteEnabled;
					tImporter.optimizeMesh = optimizeMesh;
					tImporter.importBlendShapes = importBlendShape;
					tImporter.generateSecondaryUV = generateLightmapUV;
					tImporter.addCollider = generateColliders;

					if (advancedMode)
					{					tImporter.importAnimation = importAnimation;
						
						tImporter.globalScale = modelScale;
						tImporter.keepQuads = keepQuads;
						tImporter.weldVertices = weldVertices;
						tImporter.swapUVChannels = swapUV;
						tImporter.importMaterials = ImportMaterials;
					}

					AssetDatabase.ImportAsset (s, ImportAssetOptions.ForceUpdate);
				}
			}

			if (importerType == ModelImporterType.CustomPath) {
				if (CustomPath.Length > 0) {
					foreach (string f in CustomPath) 
					{
						var files = Directory.GetFiles(f, "*.*", SearchOption.AllDirectories)
							.Where(s => s.EndsWith(".fbx") || s.EndsWith(".obj") || s.EndsWith(".ma") || s.EndsWith(".max") || s.EndsWith(".dae")
								|| s.EndsWith(".blend") || s.EndsWith(".dxf") || s.EndsWith(".3ds") || s.EndsWith(".skp")
								|| s.EndsWith(".c4d") || s.EndsWith(".lxo ") || s.EndsWith(".jas"));
						
						foreach  (string ss in files)
						{
							ModelImporter tImporter = AssetImporter.GetAtPath (ss) as ModelImporter;
							tImporter.meshCompression = meshCompression;
							tImporter.isReadable = readWriteEnabled;
							tImporter.optimizeMesh = optimizeMesh;
							tImporter.importBlendShapes = importBlendShape;
							tImporter.generateSecondaryUV = generateLightmapUV;
							tImporter.addCollider = generateColliders;

							if (advancedMode)
							{
								tImporter.importAnimation = importAnimation;
								tImporter.globalScale = modelScale;
								tImporter.keepQuads = keepQuads;
								tImporter.weldVertices = weldVertices;
								tImporter.swapUVChannels = swapUV;
								tImporter.importMaterials = ImportMaterials;
							}

							AssetDatabase.ImportAsset (ss, ImportAssetOptions.ForceUpdate);
						}
					}
				}
			}
		}

		EditorGUILayout.Space ();EditorGUILayout.Space ();

		if (importerType == ModelImporterType.CustomModels) {
			ScriptableObject target = this;
			SerializedObject so = new SerializedObject(target);
			SerializedProperty stringsProperty = so.FindProperty("targets");
			EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
			so.ApplyModifiedProperties(); // Remember to apply modified properties
		}

		if (importerType == ModelImporterType.CustomPath) {

			ScriptableObject target = this;
			SerializedObject so = new SerializedObject(target);
			SerializedProperty stringsProperty = so.FindProperty("CustomPath");
			EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
			so.ApplyModifiedProperties(); // Remember to apply modified properties
		}

		EditorGUILayout.Space ();EditorGUILayout.Space ();
		meshCompression = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup("Compression Mode",meshCompression,GUILayout.Width(343));
		readWriteEnabled = EditorGUILayout.Toggle("Read / Write Enabled",readWriteEnabled,GUILayout.Width(343));
		optimizeMesh = EditorGUILayout.Toggle("Optimize Mesh",optimizeMesh,GUILayout.Width(343));
		importBlendShape = EditorGUILayout.Toggle("Import Blend Shape",importBlendShape,GUILayout.Width(343));
		generateColliders = EditorGUILayout.Toggle("Generate Colliders",generateColliders,GUILayout.Width(343));
		generateLightmapUV = EditorGUILayout.Toggle("Generate Lightmap UV",generateLightmapUV,GUILayout.Width(343));

		advancedMode = EditorGUILayout.Toggle ("Show Advanced Options",advancedMode);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		if (advancedMode) 
		{
			modelScale = EditorGUILayout.FloatField ("Model Scale", modelScale, GUILayout.Width (343));
			importAnimation = EditorGUILayout.Toggle ("Import Animation", importAnimation, GUILayout.Width (343));

			keepQuads = EditorGUILayout.Toggle ("Keep Quads", keepQuads, GUILayout.Width (343));
			weldVertices = EditorGUILayout.Toggle ("Weld Vertices", weldVertices, GUILayout.Width (343));
			swapUV = EditorGUILayout.Toggle ("Swap UV", swapUV, GUILayout.Width (343));
			ImportMaterials = EditorGUILayout.Toggle ("Import Materials", ImportMaterials, GUILayout.Width (343));
		}
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.EndScrollView();

	}
}
#endif

