Shader "Hidden/TerrainEngine/Details/BillboardWavingDoublePass" {
	Properties {
		_WavingTint ("Fade Color", Color) = (.7,.6,.5, 0)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_WaveAndDistance ("Wave and distance", Vector) = (12, 3.6, 1, 1)
		_Cutoff ("Cutoff", float) = 0.5
	}
	
CGINCLUDE
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"

struct v2f {
	float4 pos : SV_POSITION;
	fixed4 color : COLOR;
	float4 uv : TEXCOORD0;
};
v2f BillboardVert (appdata_full v) {
	v2f o;
	WavingGrassBillboardVert (v);
	o.color = v.color;
		
	o.color.rgb *= ShadeVertexLights (v.vertex, v.normal);



	o.pos = UnityObjectToClipPos(v.vertex);	
	o.uv = v.texcoord;
	return o;
}
ENDCG

	SubShader {
		Tags {
			"Queue" = "Geometry+200"
			"IgnoreProjector"="True"
			"RenderType"="GrassBillboard"
			"DisableBatching"="True"
		}
		Cull Off
		LOD 200
		ColorMask RGB
				
CGPROGRAM
#pragma surface surf Lambert vertex:WavingGrassBillboardVert addshadow exclude_path:deferred
			
sampler2D _MainTex;
float4 _MainTex_TexelSize;
sampler2D _SnowTex;
float4 _SnowTex_TexelSize;
float _Snow_Amount;
fixed _Cutoff;
float _XScale;
float _YScale;


struct Input {
	float2 uv_MainTex;
	fixed4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {

	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;


	//float2 scaled_uv = IN.uv_MainTex *_SnowTex_TexelSize.w/ _MainTex_TexelSize.w;
	float2 scaled_uv = IN.uv_MainTex;// *2048/ 1024;
	//scaled_uv.x *=_XScale;
	//scaled_uv.y *= _YScale;//scaled_uv.y/_MainTex_TexelSize.w;
	fixed4 snowC = tex2D(_SnowTex, scaled_uv);

	o.Albedo =  lerp( c.rgb,snowC.rgb,_Snow_Amount*snowC.a);
	o.Alpha = c.a;
	clip (o.Alpha - _Cutoff);
	o.Alpha *= IN.color.a; 
}

ENDCG			
	}

	Fallback Off
}
