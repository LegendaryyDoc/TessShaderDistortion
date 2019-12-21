Shader "TessGroundShader/Snow" 
{
	Properties
	{
		_EdgeLength("Edge length", Range(2,50)) = 5
		_Phong("Phong Strengh", Range(0,1)) = 0.5
		_MainTex("Base (RGB)", 2D) = "white" {}
		_DisplacementMap("Displacement Map (R)", 2D) = "black" {}
		_DisplacementAmount("Displacement Amount", Float) = 0.1
		_Color("Color", color) = (1,1,1,0)
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque"  "LightMode" = "ShadowCaster" }
		LOD 400

		CGPROGRAM
		#pragma surface surf Lambert vertex:dispNone tessellate:tessEdge tessphong:_Phong
		#include "Tessellation.cginc"

		struct appdata 
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
			float2 texcoord2 : TEXCOORD2;
		};

		sampler2D _DisplacementMap;
		float _DisplacementAmount;

		void dispNone(inout appdata v) 
		{
			float disp = tex2Dlod(_DisplacementMap, float4(v.texcoord1.xy, 0,0)).r;
			v.vertex.xyz += v.normal * _DisplacementAmount * disp;
			v.texcoord1 = float2(0,0);
		}

		float _Phong;
		float _EdgeLength;

		float4 tessEdge(appdata v0, appdata v1, appdata v2)
		{
			return UnityEdgeLengthBasedTess(v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
		}

		struct Input 
		{
			float2 uv_MainTex;
		};

		fixed4 _Color;
		sampler2D _MainTex;

		void surf(Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}

		ENDCG
	}
	FallBack "Diffuse"
}