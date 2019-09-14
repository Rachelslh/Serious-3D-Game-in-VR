
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;



public enum TextureCompressionType
{
	None,
	LowQuality,
	HighQuality
}
public enum TexImporterType
{
	AllTextures,
	CustomTextures,
	CustomPath
}
public class BatchTextureImporter : EditorWindow {

	// Add menu named "My Window" to the Window menu
	[MenuItem("Window/Mobile Optimizer/Batch Texture Importer")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		BatchTextureImporter window = (BatchTextureImporter)EditorWindow.GetWindow(typeof(BatchTextureImporter));
		window.Show();
		window.autoRepaintOnSceneChange = true;
	}

	Vector2 scrollPos;
	public TexImporterType importerType = TexImporterType.AllTextures;
	public int textureSize = 1024;
	public TextureImporterType textureType = TextureImporterType.Default;
	public TextureImporterShape textureShape = TextureImporterShape.Texture2D;
	public bool textureRGB = true;
	public TextureImporterAlphaSource textureAlphaSource = TextureImporterAlphaSource.FromInput;
	public bool AlphaIsTransparent = false;
	public bool ReadWriteEnabled = false;
	public bool GenerateMipMaps = true;
	public TextureWrapMode wrapMode = TextureWrapMode.Repeat;
	public FilterMode filterMode = FilterMode.Bilinear;
	public int AnisoLevel = 1;
	public TextureCompressionType comprissonMode = TextureCompressionType.LowQuality;
	public string[] CustomPath = {"Assets/MobileOptimizer/Textures/Custom1"};
	public Texture2D[] targets;
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

		importerType = (TexImporterType)EditorGUILayout.EnumPopup("Importer Mode",importerType,GUILayout.Width(343));
		if (GUILayout.Button ("Batch Import"))
		{

			if (importerType == TexImporterType.AllTextures)
			{
				var files = Directory.GetFiles ("Assets", "*.*", SearchOption.AllDirectories)
					.Where (s => s.EndsWith (".png") || s.EndsWith (".jpg") || s.EndsWith (".psd") || s.EndsWith (".gif")
						|| s.EndsWith (".iff") || s.EndsWith (".tga") || s.EndsWith (".tiff") || s.EndsWith (".bmp")
						|| s.EndsWith (".pict"));
			
				foreach (string s in files) {

					TextureImporter tImporter = AssetImporter.GetAtPath (s) as TextureImporter;
					tImporter.maxTextureSize = textureSize;

					if (comprissonMode == TextureCompressionType.None)
						tImporter.textureCompression = TextureImporterCompression.Uncompressed;
					if (comprissonMode == TextureCompressionType.LowQuality)
						tImporter.textureCompression = TextureImporterCompression.CompressedLQ;
					if (comprissonMode == TextureCompressionType.HighQuality)
						tImporter.textureCompression = TextureImporterCompression.CompressedHQ;
					
					if (advancedMode) 
					{
						tImporter.textureType = textureType;
						tImporter.textureShape = textureShape;
						tImporter.sRGBTexture = textureRGB;
						tImporter.alphaSource = textureAlphaSource;
						tImporter.alphaIsTransparency = AlphaIsTransparent;
						tImporter.isReadable = ReadWriteEnabled;
						tImporter.mipmapEnabled = GenerateMipMaps;
						tImporter.wrapMode = wrapMode;
						tImporter.filterMode = filterMode;
						tImporter.anisoLevel = AnisoLevel;
					}

					AssetDatabase.ImportAsset (s, ImportAssetOptions.ForceUpdate);
				}
			}

			if (importerType == TexImporterType.CustomPath)
			{

				if (CustomPath.Length > 0) {
					foreach (string f in CustomPath) {
						var files = Directory.GetFiles (f, "*.*", SearchOption.AllDirectories)
							.Where (s => s.EndsWith (".png") || s.EndsWith (".jpg") || s.EndsWith (".psd") || s.EndsWith (".gif")
						            || s.EndsWith (".iff") || s.EndsWith (".tga") || s.EndsWith (".tiff") || s.EndsWith (".bmp")
						            || s.EndsWith (".pict"));

						foreach (string ss in files) {
							TextureImporter tImporter = AssetImporter.GetAtPath (ss) as TextureImporter;
							tImporter.maxTextureSize = textureSize;

							if (comprissonMode == TextureCompressionType.None)
								tImporter.textureCompression = TextureImporterCompression.Uncompressed;
							if (comprissonMode == TextureCompressionType.LowQuality)
								tImporter.textureCompression = TextureImporterCompression.CompressedLQ;
							if (comprissonMode == TextureCompressionType.HighQuality)
								tImporter.textureCompression = TextureImporterCompression.CompressedHQ;

							if (advancedMode) 
							{
								tImporter.textureType = textureType;
								tImporter.textureShape = textureShape;
								tImporter.sRGBTexture = textureRGB;
								tImporter.alphaSource = textureAlphaSource;
								tImporter.alphaIsTransparency = AlphaIsTransparent;
								tImporter.isReadable = ReadWriteEnabled;
								tImporter.mipmapEnabled = GenerateMipMaps;
								tImporter.wrapMode = wrapMode;
								tImporter.filterMode = filterMode;
								tImporter.anisoLevel = AnisoLevel;
							}
							AssetDatabase.ImportAsset (ss, ImportAssetOptions.ForceUpdate);

						}
					}
				}
			}


			if (importerType == TexImporterType.CustomTextures) {
				if (targets.Length > 0) {
					foreach (Texture2D s in targets) {
						TextureImporter tImporter = AssetImporter.GetAtPath (AssetDatabase.GetAssetPath (s)) as TextureImporter;
						tImporter.maxTextureSize = textureSize;

						if (comprissonMode == TextureCompressionType.None)
							tImporter.textureCompression = TextureImporterCompression.Uncompressed;
						if (comprissonMode == TextureCompressionType.LowQuality)
							tImporter.textureCompression = TextureImporterCompression.CompressedLQ;
						if (comprissonMode == TextureCompressionType.HighQuality)
							tImporter.textureCompression = TextureImporterCompression.CompressedHQ;

						if (advancedMode) 
						{
							tImporter.textureType = textureType;
							tImporter.textureShape = textureShape;
							tImporter.sRGBTexture = textureRGB;
							tImporter.alphaSource = textureAlphaSource;
							tImporter.alphaIsTransparency = AlphaIsTransparent;
							tImporter.isReadable = ReadWriteEnabled;
							tImporter.mipmapEnabled = GenerateMipMaps;
							tImporter.wrapMode = wrapMode;
							tImporter.filterMode = filterMode;
							tImporter.anisoLevel = AnisoLevel;
						}
						AssetDatabase.ImportAsset (AssetDatabase.GetAssetPath (s), ImportAssetOptions.ForceUpdate);

					}
				}
			}
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		if (importerType == TexImporterType.CustomTextures) {
			ScriptableObject target = this;
			SerializedObject so = new SerializedObject(target);
			SerializedProperty stringsProperty = so.FindProperty("targets");
			EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
			so.ApplyModifiedProperties(); // Remember to apply modified properties
		}

		if (importerType == TexImporterType.CustomPath) {

			ScriptableObject target = this;
			SerializedObject so = new SerializedObject(target);
			SerializedProperty stringsProperty = so.FindProperty("CustomPath");
			EditorGUILayout.PropertyField(stringsProperty, true); // True means show children
			so.ApplyModifiedProperties(); // Remember to apply modified properties
		}

		textureSize = EditorGUILayout.IntField("Texture Size",textureSize,GUILayout.Width(343));
		EditorGUILayout.Space ();EditorGUILayout.Space ();

		comprissonMode = (TextureCompressionType)EditorGUILayout.EnumPopup ("Comprisson Mode", comprissonMode, GUILayout.Width (343));
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		advancedMode = EditorGUILayout.Toggle ("Show Advanced Options",advancedMode);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		if (advancedMode) {
			
			EditorGUILayout.LabelField("Recommended non-advanced mode to avoid problems");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			textureType = (TextureImporterType)EditorGUILayout.EnumPopup ("Texture Type", textureType, GUILayout.Width (343));
			textureShape = (TextureImporterShape)EditorGUILayout.EnumPopup ("Texture Shape", textureShape, GUILayout.Width (343));
			textureRGB = EditorGUILayout.Toggle ("sRGB Mode", textureRGB, GUILayout.Width (343));
			textureAlphaSource = (TextureImporterAlphaSource)EditorGUILayout.EnumPopup ("Alpha Source", textureAlphaSource, GUILayout.Width (343));
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			AlphaIsTransparent = EditorGUILayout.Toggle ("Alpha Is Transparent", AlphaIsTransparent, GUILayout.Width (343));
			ReadWriteEnabled = EditorGUILayout.Toggle ("Read Write Enabled", ReadWriteEnabled, GUILayout.Width (343));
			GenerateMipMaps = EditorGUILayout.Toggle ("Generate Mip Maps", GenerateMipMaps, GUILayout.Width (343));
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup ("Wrap Mode", wrapMode, GUILayout.Width (343));
			filterMode = (FilterMode)EditorGUILayout.EnumPopup ("Filter Mode", filterMode, GUILayout.Width (343));
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			AnisoLevel = EditorGUILayout.IntField ("Aniso Level", AnisoLevel, GUILayout.Width (343));
		}
		EditorGUILayout.EndScrollView();
	}

}
#endif