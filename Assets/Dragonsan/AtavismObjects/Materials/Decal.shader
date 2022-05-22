Shader "Decal" {
Properties {
	[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_AlphaMultiplier("Alpha mul", Float) = 0.5
	_AlphaPow("Alpha pow", Float) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	Cull Off Lighting Off ZWrite Off
	Offset -1, -1

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _TintColor;
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			half _AlphaPow;
			half _AlphaMultiplier;

			struct appdata_t {
				float4 vertex : POSITION;
				half4 color : COLOR0;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half4 color : COLOR0;
				float4 texcoord : TEXCOORD0;
				float3 uvMainTex : TEXCOORD3;
				UNITY_FOG_COORDS(4)
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uvMainTex = mul(unity_Projector, v.vertex);
				o.texcoord.xy = TRANSFORM_TEX(o.uvMainTex.xyz, _MainTex);
				o.texcoord.zw = TRANSFORM_TEX(o.uvMainTex.xyz, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;
			
			half4 frag (v2f i) : SV_Target
			{
				float clampMutliplier = 1 - step(i.uvMainTex.x, 0);
				clampMutliplier *= 1 - step(1, i.uvMainTex.x);
				clampMutliplier *= 1 - step(i.uvMainTex.y, 0);
				clampMutliplier *= 1 - step(1, i.uvMainTex.y);
			
				float4 tex = tex2D(_MainTex, i.texcoord.xy);
				half4 col = 2.0f * i.color * _TintColor * tex;
				UNITY_APPLY_FOG(i.fogCoord, col);

				
				float projectedCordZ =  i.uvMainTex.z;
				clampMutliplier *= step(projectedCordZ, 1);
				clampMutliplier *= step(-1, projectedCordZ);
				col.a = tex.a * _TintColor.a * clampMutliplier;
				
				return half4(col.rgb,  saturate(pow(col.a * _AlphaMultiplier, _AlphaPow)));
			}
			ENDCG 
		}
	}	
}
}
