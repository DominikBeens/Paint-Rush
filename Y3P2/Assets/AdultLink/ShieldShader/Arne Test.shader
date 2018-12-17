// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Arne Test"
{
	Properties
	{
		_BaseColor("Base Color", Color) = (0.1058824,0.9058824,1,0.454902)
		_Emission("Emission", Color) = (0,0,0,0)
		_ScrollSpeed("Scroll Speed", Float) = 0.05
		_Scale("Scale", Vector) = (17,6,0,0)
		_Texture0("Texture 0", 2D) = "white" {}
		_Timescale("Time scale", Float) = 1
		_Smoothness("Smoothness", Float) = 0.5
		_Metallic("Metallic", Float) = 0
		_AmbientOcclusion("Ambient Occlusion", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		AlphaToMask On
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float4 screenPos;
		};

		uniform float4 _BaseColor;
		uniform float4 _Emission;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _AmbientOcclusion;
		uniform sampler2D _Texture0;
		uniform float2 _Scale;
		uniform float _Timescale;
		uniform float _ScrollSpeed;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexTangent = v.tangent.xyz;
			v.vertex.xyz += ase_vertexTangent;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			o.Normal = ase_worldTangent;
			o.Albedo = _BaseColor.rgb;
			o.Emission = _Emission.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Occlusion = _AmbientOcclusion;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float mulTime12 = _Time.y * _Timescale;
			float temp_output_14_0 = ( mulTime12 * _ScrollSpeed );
			o.Alpha = tex2D( _Texture0, (ase_screenPosNorm*float4( _Scale, 0.0 , 0.0 ) + temp_output_14_0).xy ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			AlphaToMask Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 screenPos : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15600
424;470;1275;848;145.7971;238.144;1;False;False
Node;AmplifyShaderEditor.RangedFloatNode;18;-1141.77,540.6473;Float;False;Property;_Timescale;Time scale;5;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-947.9758,781.1982;Float;False;Property;_ScrollSpeed;Scroll Speed;2;0;Create;True;0;0;False;0;0.05;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;12;-944.0522,559.0647;Float;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;9;-809.0522,244.0647;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-721.9758,570.1982;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;11;-789.0522,409.0647;Float;False;Property;_Scale;Scale;3;0;Create;True;0;0;False;0;17,6;17,6;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;4;-809.786,53.25881;Float;True;Property;_Texture0;Texture 0;4;0;Create;True;0;0;False;0;847f8869c44b2cd48959cb0dd3929ed2;847f8869c44b2cd48959cb0dd3929ed2;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;26;-573.5527,305.5462;Float;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;1,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VertexTangentNode;13;-148.0522,13.0647;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;22;-117.77,344.6473;Float;False;Property;_AmbientOcclusion;Ambient Occlusion;8;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TangentVertexDataNode;44;159.1796,491.2365;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;17;-299.0146,154.1073;Float;False;Property;_Emission;Emission;1;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;20;-107.77,270.6473;Float;False;Property;_Smoothness;Smoothness;6;0;Create;True;0;0;False;0;0.5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-371,-106;Float;False;Property;_BaseColor;Base Color;0;0;Create;True;0;0;False;0;0.1058824,0.9058824,1,0.454902;0.1058824,0.9058824,1,0.1960784;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;21;-47.77002,198.6473;Float;False;Property;_Metallic;Metallic;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;25;-565.5527,416.5463;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-122,436;Float;True;Property;_Sample;Sample;0;0;Create;True;0;0;False;0;847f8869c44b2cd48959cb0dd3929ed2;847f8869c44b2cd48959cb0dd3929ed2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;386,3;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Arne Test;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Translucent;0.5;True;True;0;False;Opaque;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;True;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;18;0
WireConnection;14;0;12;0
WireConnection;14;1;16;0
WireConnection;26;0;9;0
WireConnection;26;1;11;0
WireConnection;26;2;14;0
WireConnection;25;0;11;0
WireConnection;25;1;14;0
WireConnection;3;0;4;0
WireConnection;3;1;26;0
WireConnection;0;0;1;0
WireConnection;0;1;13;0
WireConnection;0;2;17;0
WireConnection;0;3;21;0
WireConnection;0;4;20;0
WireConnection;0;5;22;0
WireConnection;0;9;3;0
WireConnection;0;11;44;0
ASEEND*/
//CHKSM=62B552C88CBC5FAECE8B7ABC1156E6CA043E73D9