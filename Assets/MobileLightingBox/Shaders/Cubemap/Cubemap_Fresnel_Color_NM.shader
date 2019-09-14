// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LB Mobile/Cubemap/(Fresnel) Color + Normal Map"
{
	Properties
	{
		_Color("Color", Color) = (0,0,0,1)
		[Normal]_NormalMap("NormalMap", 2D) = "white" {}
		_Intensity("Intensity", Range( 0 , 1)) = 1
		_Cubemap("Cubemap", CUBE) = "white" {}
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		_FresnelBias("Fresnel Bias", Range( 0 , 1)) = 1
		_FresnelScale("Fresnel Scale", Range( 0 , 1)) = 1
		_FresnelPower("Fresnel Power", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 2.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldRefl;
			INTERNAL_DATA
			float3 worldPos;
			float3 worldNormal;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float4 _Color;
		uniform float _Intensity;
		uniform samplerCUBE _Cubemap;
		uniform float4 _FresnelColor;
		uniform float _FresnelBias;
		uniform float _FresnelScale;
		uniform float _FresnelPower;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 tex2DNode2 = UnpackNormal( tex2D( _NormalMap, uv_NormalMap ) );
			o.Normal = tex2DNode2;
			o.Albedo = _Color.rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNDotV18 = dot( WorldNormalVector( i , tex2DNode2 ), ase_worldViewDir );
			float fresnelNode18 = ( _FresnelBias + _FresnelScale * pow( 1.0 - fresnelNDotV18, _FresnelPower ) );
			o.Emission = ( ( _Intensity * texCUBE( _Cubemap, WorldReflectionVector( i , tex2DNode2 ) ) ) * ( _FresnelColor * fresnelNode18 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Lambert keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				float4 texcoords01 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord.xy = IN.texcoords01.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.worldRefl = -worldViewDir;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13801
9;80;1010;690;2906.064;365.535;2.284426;True;False
Node;AmplifyShaderEditor.CommentaryNode;16;-1975.541,-35.52749;Float;False;371;280;Normal;1;2;;0,0.5034485,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;14;-1701.177,419.7693;Float;False;694.0634;280;Cubemap;2;11;10;;1,0,0.8482761,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;26;-1073.934,767.9863;Float;False;900.8339;457.8168;Fresnel;6;23;24;25;18;20;19;;0,0.9172413,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;2;-1925.541,14.4725;Float;True;Property;_NormalMap;NormalMap;1;1;[Normal];None;True;0;True;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;23;-1022.934,920.2968;Float;False;Property;_FresnelBias;Fresnel Bias;5;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;25;-1022.934,1104.297;Float;False;Property;_FresnelPower;Fresnel Power;7;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;24;-1022.934,1010.297;Float;False;Property;_FresnelScale;Fresnel Scale;6;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.WorldReflectionVector;10;-1651.177,508.0596;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.CommentaryNode;13;-1360.672,200.4377;Float;False;636.7283;186.1361;Cubemap Intensity   ;2;3;12;;0,0.7103448,1,1;0;0
Node;AmplifyShaderEditor.FresnelNode;18;-554.8046,1046.803;Float;False;Tangent;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;12;-1310.672,250.4377;Float;False;Property;_Intensity;Intensity;2;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;20;-575.1152,817.9863;Float;False;Property;_FresnelColor;Fresnel Color;4;0;1,1,1,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;11;-1328.113,469.7693;Float;True;Property;_Cubemap;Cubemap;3;0;Assets/freeHDRI_assets/hdri/001_studioHDRI.hdr;True;0;False;white;Auto;False;Object;-1;Auto;Cube;6;0;SAMPLER2D;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0.0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-342.1006,982.7059;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-903.6674,248.2117;Float;False;2;2;0;FLOAT;0,0,0,0;False;1;COLOR;0;False;1;COLOR
Node;AmplifyShaderEditor.CommentaryNode;17;-701.7323,-329.7633;Float;False;284;257;Color;1;8;;0,1,0.213793,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-319.5437,328.5114;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;8;-651.7323,-279.7633;Float;False;Property;_Color;Color;0;0;0,0,0,1;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;0;Float;ASEMaterialInspector;0;0;Lambert;LB Mobile/Cubemap/(Fresnel) Color + Normal Map;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;10;0;2;0
WireConnection;18;0;2;0
WireConnection;18;1;23;0
WireConnection;18;2;24;0
WireConnection;18;3;25;0
WireConnection;11;1;10;0
WireConnection;19;0;20;0
WireConnection;19;1;18;0
WireConnection;3;0;12;0
WireConnection;3;1;11;0
WireConnection;21;0;3;0
WireConnection;21;1;19;0
WireConnection;0;0;8;0
WireConnection;0;1;2;0
WireConnection;0;2;21;0
ASEEND*/
//CHKSM=F5EB77F060BD3E1F207AF8EB643EB32331FD16D9