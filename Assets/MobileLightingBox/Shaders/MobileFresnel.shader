// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LB Mobile/MobileFresnel"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_MainTex("MainTex", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)
		_Scale("Scale", Range( 0 , 1)) = 0
		_Power("Power", Range( 0 , 5)) = 0
		_MainColor("Main Color", Color) = (1,1,1,1)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "ForceNoShadowCasting" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 2.0
		#pragma exclude_renderers xboxone ps4 psp2 n3ds wiiu 
		#pragma surface surf Lambert keepalpha noshadow nodynlightmap nodirlightmap noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float4 _MainColor;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float4 _Color;
		uniform float _Scale;
		uniform float _Power;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = ( _MainColor * tex2DNode1 ).rgb;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelFinalVal2 = (0.0 + _Scale*pow( 1.0 - dot( ase_worldNormal, worldViewDir ) , _Power));
			o.Emission = ( ( _Color * fresnelFinalVal2 ) * tex2DNode1 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
206;186;1010;692;1422.167;-120.0568;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;7;-975.7695,423.8904;Float;False;Property;_Scale;Scale;2;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;8;-993.7695,521.8904;Float;False;Property;_Power;Power;3;0;0;0;5;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;6;-997.7695,334.8904;Float;False;Constant;_Bias;Bias;4;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;5;-670.7695,182.8904;Float;False;Property;_Color;Color;1;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.FresnelNode;2;-690.7695,366.8904;Float;False;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-433.7695,254.8904;Float;False;2;2;0;COLOR;0.0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;9;-624,-250;Float;False;Property;_MainColor;Main Color;4;0;1,1,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-743,-56;Float;True;Property;_MainTex;MainTex;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-227.4475,166.7888;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-222,-71;Float;False;2;2;0;COLOR;0.0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;0;Float;ASEMaterialInspector;0;0;Lambert;LB Mobile/MobileFresnel;False;False;False;False;False;False;False;True;True;False;False;True;False;False;False;True;False;Back;0;0;False;0;0;Opaque;0.5;True;False;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;False;False;False;False;False;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;1;6;0
WireConnection;2;2;7;0
WireConnection;2;3;8;0
WireConnection;3;0;5;0
WireConnection;3;1;2;0
WireConnection;11;0;3;0
WireConnection;11;1;1;0
WireConnection;10;0;9;0
WireConnection;10;1;1;0
WireConnection;0;0;10;0
WireConnection;0;2;11;0
ASEEND*/
//CHKSM=4B4BEC340CD2C43C6F455551070C07F1D5A9DA5C