// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "HNGamers_SyntyStandardCharacter_Urp"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin]_leatherMask("leatherMask", 2D) = "white" {}
		_metalmask("metalmask", 2D) = "white" {}
		_primaryMask("primaryMask", 2D) = "white" {}
		_skinhairMask("skinhairMask", 2D) = "white" {}
		_OriginalTexture("OriginalTexture", 2D) = "white" {}
		_colorMasks("colorMasks", 2D) = "white" {}
		_Color_Hair("Color_Hair", Color) = (1,0.8726415,0.8726415,0)
		_Color_Skin("Color_Skin", Color) = (1,0.8726415,0.8726415,0)
		_Color_Metal_Dark("Color_Metal_Dark", Color) = (0.7924528,0.06354576,0.06354576,0)
		_Color_Metal_Secondary("Color_Metal_Secondary", Color) = (0.764151,0.1622019,0.1622019,0)
		_Color_Metal_Primary("Color_Metal_Primary", Color) = (1,1,1,0)
		_Color_Leather_Tertiary("Color_Leather_Tertiary", Color) = (0.764151,0.1622019,0.1622019,0)
		_Color_Leather_Secondary("Color_Leather_Secondary", Color) = (0.764151,0.1622019,0.1622019,0)
		_Color_Secondary("Color_Secondary", Color) = (0.8983961,1,0.0990566,0)
		_Color_Tertiary("Color_Tertiary", Color) = (0.9339623,0,0,0)
		_Color_Stubble("Color_Stubble", Color) = (1,0.8726415,0.8726415,0)
		_Color_Primary("Color_Primary", Color) = (0,0.1698678,1,0)
		_Color1("Color 1", Color) = (1,1,1,0)
		_Color2("Color 2", Color) = (1,1,1,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Color_Leather_Primary("Color_Leather_Primary", Color) = (0.6037736,0.2933429,0.2933429,0)
		[ASEEnd]_Color_Medical("Color_Medical", Color) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		//_TransmissionShadow( "Transmission Shadow", Range( 0, 1 ) ) = 0.5
		//_TransStrength( "Trans Strength", Range( 0, 50 ) ) = 1
		//_TransNormal( "Trans Normal Distortion", Range( 0, 1 ) ) = 0.5
		//_TransScattering( "Trans Scattering", Range( 1, 50 ) ) = 2
		//_TransDirect( "Trans Direct", Range( 0, 1 ) ) = 0.9
		//_TransAmbient( "Trans Ambient", Range( 0, 1 ) ) = 0.1
		//_TransShadow( "Trans Shadow", Range( 0, 1 ) ) = 0.5
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Geometry" }
		Cull Back
		AlphaToMask Off
		HLSLINCLUDE
		#pragma target 2.0

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 70503

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_FORWARD

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#if ASE_SRP_VERSION <= 70108
			#define REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
			#endif

			#if defined(UNITY_INSTANCING_ENABLED) && defined(_TERRAIN_INSTANCED_PERPIXEL_NORMAL)
			    #define ENABLE_TERRAIN_PERPIXEL_NORMAL
			#endif

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord : TEXCOORD0;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 lightmapUVOrVertexSH : TEXCOORD0;
				half4 fogFactorAndVertexLight : TEXCOORD1;
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord : TEXCOORD2;
				#endif
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 screenPos : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _OriginalTexture_ST;
			float4 _TextureSample0_ST;
			float4 _Color_Medical;
			float4 _colorMasks_ST;
			float4 _Color_Stubble;
			float4 _Color_Hair;
			float4 _skinhairMask_ST;
			float4 _Color_Skin;
			float4 _Color_Metal_Dark;
			float4 _Color_Metal_Secondary;
			float4 _metalmask_ST;
			float4 _Color_Metal_Primary;
			float4 _Color_Leather_Tertiary;
			float4 _Color_Leather_Secondary;
			float4 _leatherMask_ST;
			float4 _Color_Leather_Primary;
			float4 _Color_Tertiary;
			float4 _Color_Secondary;
			float4 _primaryMask_ST;
			float4 _Color_Primary;
			float4 _Color2;
			float4 _Color1;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _OriginalTexture;
			sampler2D _primaryMask;
			sampler2D _leatherMask;
			sampler2D _metalmask;
			sampler2D _skinhairMask;
			sampler2D _colorMasks;
			sampler2D _TextureSample0;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord7.xy = v.texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord7.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 positionVS = TransformWorldToView( positionWS );
				float4 positionCS = TransformWorldToHClip( positionWS );

				VertexNormalInputs normalInput = GetVertexNormalInputs( v.ase_normal, v.ase_tangent );

				o.tSpace0 = float4( normalInput.normalWS, positionWS.x);
				o.tSpace1 = float4( normalInput.tangentWS, positionWS.y);
				o.tSpace2 = float4( normalInput.bitangentWS, positionWS.z);

				OUTPUT_LIGHTMAP_UV( v.texcoord1, unity_LightmapST, o.lightmapUVOrVertexSH.xy );
				OUTPUT_SH( normalInput.normalWS.xyz, o.lightmapUVOrVertexSH.xyz );

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					o.lightmapUVOrVertexSH.zw = v.texcoord;
					o.lightmapUVOrVertexSH.xy = v.texcoord * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				half3 vertexLight = VertexLighting( positionWS, normalInput.normalWS );
				#ifdef ASE_FOG
					half fogFactor = ComputeFogFactor( positionCS.z );
				#else
					half fogFactor = 0;
				#endif
				o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = positionWS;
				vertexInput.positionCS = positionCS;
				o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				
				o.clipPos = positionCS;
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				o.screenPos = ComputeScreenPos(positionCS);
				#endif
				return o;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_tangent = v.ase_tangent;
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_tangent = patch[0].ase_tangent * bary.x + patch[1].ase_tangent * bary.y + patch[2].ase_tangent * bary.z;
				o.texcoord = patch[0].texcoord * bary.x + patch[1].texcoord * bary.y + patch[2].texcoord * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag ( VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float2 sampleCoords = (IN.lightmapUVOrVertexSH.zw / _TerrainHeightmapRecipSize.zw + 0.5f) * _TerrainHeightmapRecipSize.xy;
					float3 WorldNormal = TransformObjectToWorldNormal(normalize(SAMPLE_TEXTURE2D(_TerrainNormalmapTexture, sampler_TerrainNormalmapTexture, sampleCoords).rgb * 2 - 1));
					float3 WorldTangent = -cross(GetObjectToWorldMatrix()._13_23_33, WorldNormal);
					float3 WorldBiTangent = cross(WorldNormal, -WorldTangent);
				#else
					float3 WorldNormal = normalize( IN.tSpace0.xyz );
					float3 WorldTangent = IN.tSpace1.xyz;
					float3 WorldBiTangent = IN.tSpace2.xyz;
				#endif
				float3 WorldPosition = float3(IN.tSpace0.w,IN.tSpace1.w,IN.tSpace2.w);
				float3 WorldViewDirection = _WorldSpaceCameraPos.xyz  - WorldPosition;
				float4 ShadowCoords = float4( 0, 0, 0, 0 );
				#if defined(ASE_NEEDS_FRAG_SCREEN_POSITION)
				float4 ScreenPos = IN.screenPos;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					ShadowCoords = IN.shadowCoord;
				#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
				#endif
	
				WorldViewDirection = SafeNormalize( WorldViewDirection );

				float2 uv_OriginalTexture = IN.ase_texcoord7.xy * _OriginalTexture_ST.xy + _OriginalTexture_ST.zw;
				float4 tex2DNode7 = tex2D( _OriginalTexture, uv_OriginalTexture );
				float2 uv_primaryMask = IN.ase_texcoord7.xy * _primaryMask_ST.xy + _primaryMask_ST.zw;
				float4 tex2DNode3 = tex2D( _primaryMask, uv_primaryMask );
				float temp_output_25_0_g2 = 0.0;
				float temp_output_22_0_g2 = step( tex2DNode3.r , temp_output_25_0_g2 );
				float temp_output_17_0 = ( temp_output_22_0_g2 - 0.0 );
				float4 lerpResult18 = lerp( float4( 0,0,0,0 ) , ( tex2DNode7.a * _Color_Primary * temp_output_17_0 ) , temp_output_17_0);
				float temp_output_25_0_g3 = 0.0;
				float temp_output_22_0_g3 = step( tex2DNode3.b , temp_output_25_0_g3 );
				float temp_output_19_0 = ( temp_output_22_0_g3 - 0.0 );
				float4 lerpResult22 = lerp( lerpResult18 , ( tex2DNode7.a * _Color_Secondary * temp_output_19_0 ) , temp_output_19_0);
				float temp_output_25_0_g4 = 0.0;
				float temp_output_22_0_g4 = step( tex2DNode3.g , temp_output_25_0_g4 );
				float temp_output_26_0 = ( temp_output_22_0_g4 - 0.0 );
				float4 lerpResult23 = lerp( lerpResult22 , ( tex2DNode7.a * _Color_Tertiary * temp_output_26_0 ) , temp_output_26_0);
				float2 uv_leatherMask = IN.ase_texcoord7.xy * _leatherMask_ST.xy + _leatherMask_ST.zw;
				float4 tex2DNode1 = tex2D( _leatherMask, uv_leatherMask );
				float temp_output_25_0_g5 = 0.0;
				float temp_output_22_0_g5 = step( tex2DNode1.r , temp_output_25_0_g5 );
				float temp_output_30_0 = ( temp_output_22_0_g5 - 0.0 );
				float4 lerpResult27 = lerp( lerpResult23 , ( tex2DNode7.a * _Color_Leather_Primary * temp_output_30_0 ) , temp_output_30_0);
				float temp_output_25_0_g6 = 0.0;
				float temp_output_22_0_g6 = step( tex2DNode1.g , temp_output_25_0_g6 );
				float temp_output_34_0 = ( temp_output_22_0_g6 - 0.0 );
				float4 lerpResult31 = lerp( lerpResult27 , ( tex2DNode7.a * _Color_Leather_Secondary * temp_output_34_0 ) , temp_output_34_0);
				float temp_output_25_0_g7 = 0.0;
				float temp_output_22_0_g7 = step( tex2DNode1.b , temp_output_25_0_g7 );
				float temp_output_38_0 = ( temp_output_22_0_g7 - 0.0 );
				float4 lerpResult35 = lerp( lerpResult31 , ( tex2DNode7.a * _Color_Leather_Tertiary * temp_output_38_0 ) , temp_output_38_0);
				float2 uv_metalmask = IN.ase_texcoord7.xy * _metalmask_ST.xy + _metalmask_ST.zw;
				float4 tex2DNode2 = tex2D( _metalmask, uv_metalmask );
				float temp_output_25_0_g8 = 0.0;
				float temp_output_22_0_g8 = step( tex2DNode2.r , temp_output_25_0_g8 );
				float temp_output_42_0 = ( temp_output_22_0_g8 - 0.0 );
				float4 lerpResult39 = lerp( lerpResult35 , ( tex2DNode7.a * _Color_Metal_Primary * temp_output_42_0 ) , temp_output_42_0);
				float temp_output_25_0_g9 = 0.0;
				float temp_output_22_0_g9 = step( tex2DNode2.g , temp_output_25_0_g9 );
				float temp_output_46_0 = ( temp_output_22_0_g9 - 0.0 );
				float4 lerpResult43 = lerp( lerpResult39 , ( tex2DNode7.a * _Color_Metal_Secondary * temp_output_46_0 ) , temp_output_46_0);
				float temp_output_25_0_g10 = 0.0;
				float temp_output_22_0_g10 = step( tex2DNode2.b , temp_output_25_0_g10 );
				float temp_output_50_0 = ( temp_output_22_0_g10 - 0.0 );
				float4 lerpResult47 = lerp( lerpResult43 , ( tex2DNode7.a * _Color_Metal_Dark * temp_output_50_0 ) , temp_output_50_0);
				float2 uv_skinhairMask = IN.ase_texcoord7.xy * _skinhairMask_ST.xy + _skinhairMask_ST.zw;
				float4 tex2DNode5 = tex2D( _skinhairMask, uv_skinhairMask );
				float temp_output_25_0_g23 = 0.0;
				float temp_output_22_0_g23 = step( tex2DNode5.r , temp_output_25_0_g23 );
				float temp_output_54_0 = ( temp_output_22_0_g23 - 0.0 );
				float4 lerpResult51 = lerp( lerpResult47 , ( tex2DNode7.a * _Color_Skin * temp_output_54_0 ) , temp_output_54_0);
				float temp_output_25_0_g28 = 0.0;
				float temp_output_22_0_g28 = step( tex2DNode5.g , temp_output_25_0_g28 );
				float temp_output_58_0 = ( temp_output_22_0_g28 - 0.0 );
				float4 lerpResult55 = lerp( lerpResult51 , ( tex2DNode7.a * _Color_Hair * temp_output_58_0 ) , temp_output_58_0);
				float temp_output_25_0_g31 = 0.0;
				float temp_output_22_0_g31 = step( tex2DNode5.b , temp_output_25_0_g31 );
				float temp_output_62_0 = ( temp_output_22_0_g31 - 0.0 );
				float4 lerpResult59 = lerp( lerpResult55 , ( tex2DNode7.a * _Color_Stubble * temp_output_62_0 ) , temp_output_62_0);
				float2 uv_colorMasks = IN.ase_texcoord7.xy * _colorMasks_ST.xy + _colorMasks_ST.zw;
				float4 lerpResult76 = lerp( tex2DNode7 , lerpResult59 , tex2D( _colorMasks, uv_colorMasks ));
				float2 uv_TextureSample0 = IN.ase_texcoord7.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
				float4 tex2DNode89 = tex2D( _TextureSample0, uv_TextureSample0 );
				float temp_output_25_0_g24 = 0.0;
				float temp_output_22_0_g24 = step( tex2DNode89.r , temp_output_25_0_g24 );
				float temp_output_77_0 = ( temp_output_22_0_g24 - 0.0 );
				float4 lerpResult79 = lerp( float4( 0,0,0,0 ) , ( tex2DNode7.a * _Color_Medical * temp_output_77_0 ) , temp_output_77_0);
				float temp_output_25_0_g29 = 0.0;
				float temp_output_22_0_g29 = step( tex2DNode89.b , temp_output_25_0_g29 );
				float temp_output_85_0 = ( temp_output_22_0_g29 - 0.0 );
				float4 lerpResult87 = lerp( lerpResult79 , ( tex2DNode7.a * _Color2 * temp_output_85_0 ) , temp_output_85_0);
				float temp_output_25_0_g32 = 0.0;
				float temp_output_22_0_g32 = step( tex2DNode89.g , temp_output_25_0_g32 );
				float temp_output_85_28 = ( temp_output_25_0_g29 + 0.2 );
				float4 lerpResult83 = lerp( lerpResult87 , ( tex2DNode7.a * _Color1 * ( temp_output_22_0_g32 - 0.0 ) ) , temp_output_85_28);
				float4 lerpResult90 = lerp( lerpResult76 , ( lerpResult83 + lerpResult83 + lerpResult83 ) , temp_output_85_28);
				
				float3 Albedo = lerpResult90.rgb;
				float3 Normal = float3(0, 0, 1);
				float3 Emission = 0;
				float3 Specular = 0.5;
				float Metallic = 0;
				float Smoothness = 0.5;
				float Occlusion = 1;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				float3 BakedGI = 0;
				float3 RefractionColor = 1;
				float RefractionIndex = 1;
				float3 Transmission = 1;
				float3 Translucency = 1;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				InputData inputData;
				inputData.positionWS = WorldPosition;
				inputData.viewDirectionWS = WorldViewDirection;
				inputData.shadowCoord = ShadowCoords;

				#ifdef _NORMALMAP
					#if _NORMAL_DROPOFF_TS
					inputData.normalWS = TransformTangentToWorld(Normal, half3x3( WorldTangent, WorldBiTangent, WorldNormal ));
					#elif _NORMAL_DROPOFF_OS
					inputData.normalWS = TransformObjectToWorldNormal(Normal);
					#elif _NORMAL_DROPOFF_WS
					inputData.normalWS = Normal;
					#endif
					inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
				#else
					inputData.normalWS = WorldNormal;
				#endif

				#ifdef ASE_FOG
					inputData.fogCoord = IN.fogFactorAndVertexLight.x;
				#endif

				inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
				#if defined(ENABLE_TERRAIN_PERPIXEL_NORMAL)
					float3 SH = SampleSH(inputData.normalWS.xyz);
				#else
					float3 SH = IN.lightmapUVOrVertexSH.xyz;
				#endif

				inputData.bakedGI = SAMPLE_GI( IN.lightmapUVOrVertexSH.xy, SH, inputData.normalWS );
				#ifdef _ASE_BAKEDGI
					inputData.bakedGI = BakedGI;
				#endif
				half4 color = UniversalFragmentPBR(
					inputData, 
					Albedo, 
					Metallic, 
					Specular, 
					Smoothness, 
					Occlusion, 
					Emission, 
					Alpha);

				#ifdef _TRANSMISSION_ASE
				{
					float shadow = _TransmissionShadow;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );
					half3 mainTransmission = max(0 , -dot(inputData.normalWS, mainLight.direction)) * mainAtten * Transmission;
					color.rgb += Albedo * mainTransmission;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 transmission = max(0 , -dot(inputData.normalWS, light.direction)) * atten * Transmission;
							color.rgb += Albedo * transmission;
						}
					#endif
				}
				#endif

				#ifdef _TRANSLUCENCY_ASE
				{
					float shadow = _TransShadow;
					float normal = _TransNormal;
					float scattering = _TransScattering;
					float direct = _TransDirect;
					float ambient = _TransAmbient;
					float strength = _TransStrength;

					Light mainLight = GetMainLight( inputData.shadowCoord );
					float3 mainAtten = mainLight.color * mainLight.distanceAttenuation;
					mainAtten = lerp( mainAtten, mainAtten * mainLight.shadowAttenuation, shadow );

					half3 mainLightDir = mainLight.direction + inputData.normalWS * normal;
					half mainVdotL = pow( saturate( dot( inputData.viewDirectionWS, -mainLightDir ) ), scattering );
					half3 mainTranslucency = mainAtten * ( mainVdotL * direct + inputData.bakedGI * ambient ) * Translucency;
					color.rgb += Albedo * mainTranslucency * strength;

					#ifdef _ADDITIONAL_LIGHTS
						int transPixelLightCount = GetAdditionalLightsCount();
						for (int i = 0; i < transPixelLightCount; ++i)
						{
							Light light = GetAdditionalLight(i, inputData.positionWS);
							float3 atten = light.color * light.distanceAttenuation;
							atten = lerp( atten, atten * light.shadowAttenuation, shadow );

							half3 lightDir = light.direction + inputData.normalWS * normal;
							half VdotL = pow( saturate( dot( inputData.viewDirectionWS, -lightDir ) ), scattering );
							half3 translucency = atten * ( VdotL * direct + inputData.bakedGI * ambient ) * Translucency;
							color.rgb += Albedo * translucency * strength;
						}
					#endif
				}
				#endif

				#ifdef _REFRACTION_ASE
					float4 projScreenPos = ScreenPos / ScreenPos.w;
					float3 refractionOffset = ( RefractionIndex - 1.0 ) * mul( UNITY_MATRIX_V, WorldNormal ).xyz * ( 1.0 - dot( WorldNormal, WorldViewDirection ) );
					projScreenPos.xy += refractionOffset.xy;
					float3 refraction = SHADERGRAPH_SAMPLE_SCENE_COLOR( projScreenPos ) * RefractionColor;
					color.rgb = lerp( refraction, color.rgb, color.a );
					color.a = 1;
				#endif

				#ifdef ASE_FINAL_COLOR_ALPHA_MULTIPLY
					color.rgb *= color.a;
				#endif

				#ifdef ASE_FOG
					#ifdef TERRAIN_SPLAT_ADDPASS
						color.rgb = MixFogColor(color.rgb, half3( 0, 0, 0 ), IN.fogFactorAndVertexLight.x );
					#else
						color.rgb = MixFog(color.rgb, IN.fogFactorAndVertexLight.x);
					#endif
				#endif
				
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif

				return color;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 70503

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _OriginalTexture_ST;
			float4 _TextureSample0_ST;
			float4 _Color_Medical;
			float4 _colorMasks_ST;
			float4 _Color_Stubble;
			float4 _Color_Hair;
			float4 _skinhairMask_ST;
			float4 _Color_Skin;
			float4 _Color_Metal_Dark;
			float4 _Color_Metal_Secondary;
			float4 _metalmask_ST;
			float4 _Color_Metal_Primary;
			float4 _Color_Leather_Tertiary;
			float4 _Color_Leather_Secondary;
			float4 _leatherMask_ST;
			float4 _Color_Leather_Primary;
			float4 _Color_Tertiary;
			float4 _Color_Secondary;
			float4 _primaryMask_ST;
			float4 _Color_Primary;
			float4 _Color2;
			float4 _Color1;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			

			
			float3 _LightDirection;

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif
				float3 normalWS = TransformObjectToWorldDir(v.ase_normal);

				float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );

				#if UNITY_REVERSED_Z
					clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#else
					clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = clipPos;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif

			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );
				
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
					outputDepth = DepthValue;
				#endif
				return 0;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 70503

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _OriginalTexture_ST;
			float4 _TextureSample0_ST;
			float4 _Color_Medical;
			float4 _colorMasks_ST;
			float4 _Color_Stubble;
			float4 _Color_Hair;
			float4 _skinhairMask_ST;
			float4 _Color_Skin;
			float4 _Color_Metal_Dark;
			float4 _Color_Metal_Secondary;
			float4 _metalmask_ST;
			float4 _Color_Metal_Primary;
			float4 _Color_Leather_Tertiary;
			float4 _Color_Leather_Secondary;
			float4 _leatherMask_ST;
			float4 _Color_Leather_Primary;
			float4 _Color_Tertiary;
			float4 _Color_Secondary;
			float4 _primaryMask_ST;
			float4 _Color_Primary;
			float4 _Color2;
			float4 _Color1;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			

			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;
				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(ASE_EARLY_Z_DEPTH_OPTIMIZE)
				#define ASE_SV_DEPTH SV_DepthLessEqual  
			#else
				#define ASE_SV_DEPTH SV_Depth
			#endif
			half4 frag(	VertexOutput IN 
						#ifdef ASE_DEPTH_WRITE_ON
						,out float outputDepth : ASE_SV_DEPTH
						#endif
						 ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				#ifdef ASE_DEPTH_WRITE_ON
				float DepthValue = 0;
				#endif

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				#ifdef ASE_DEPTH_WRITE_ON
				outputDepth = DepthValue;
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Meta"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 70503

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_META

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _OriginalTexture_ST;
			float4 _TextureSample0_ST;
			float4 _Color_Medical;
			float4 _colorMasks_ST;
			float4 _Color_Stubble;
			float4 _Color_Hair;
			float4 _skinhairMask_ST;
			float4 _Color_Skin;
			float4 _Color_Metal_Dark;
			float4 _Color_Metal_Secondary;
			float4 _metalmask_ST;
			float4 _Color_Metal_Primary;
			float4 _Color_Leather_Tertiary;
			float4 _Color_Leather_Secondary;
			float4 _leatherMask_ST;
			float4 _Color_Leather_Primary;
			float4 _Color_Tertiary;
			float4 _Color_Secondary;
			float4 _primaryMask_ST;
			float4 _Color_Primary;
			float4 _Color2;
			float4 _Color1;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _OriginalTexture;
			sampler2D _primaryMask;
			sampler2D _leatherMask;
			sampler2D _metalmask;
			sampler2D _skinhairMask;
			sampler2D _colorMasks;
			sampler2D _TextureSample0;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				o.clipPos = MetaVertexPosition( v.vertex, v.texcoord1.xy, v.texcoord1.xy, unity_LightmapST, unity_DynamicLightmapST );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 texcoord1 : TEXCOORD1;
				float4 texcoord2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.texcoord1 = v.texcoord1;
				o.texcoord2 = v.texcoord2;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.texcoord1 = patch[0].texcoord1 * bary.x + patch[1].texcoord1 * bary.y + patch[2].texcoord1 * bary.z;
				o.texcoord2 = patch[0].texcoord2 * bary.x + patch[1].texcoord2 * bary.y + patch[2].texcoord2 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_OriginalTexture = IN.ase_texcoord2.xy * _OriginalTexture_ST.xy + _OriginalTexture_ST.zw;
				float4 tex2DNode7 = tex2D( _OriginalTexture, uv_OriginalTexture );
				float2 uv_primaryMask = IN.ase_texcoord2.xy * _primaryMask_ST.xy + _primaryMask_ST.zw;
				float4 tex2DNode3 = tex2D( _primaryMask, uv_primaryMask );
				float temp_output_25_0_g2 = 0.0;
				float temp_output_22_0_g2 = step( tex2DNode3.r , temp_output_25_0_g2 );
				float temp_output_17_0 = ( temp_output_22_0_g2 - 0.0 );
				float4 lerpResult18 = lerp( float4( 0,0,0,0 ) , ( tex2DNode7.a * _Color_Primary * temp_output_17_0 ) , temp_output_17_0);
				float temp_output_25_0_g3 = 0.0;
				float temp_output_22_0_g3 = step( tex2DNode3.b , temp_output_25_0_g3 );
				float temp_output_19_0 = ( temp_output_22_0_g3 - 0.0 );
				float4 lerpResult22 = lerp( lerpResult18 , ( tex2DNode7.a * _Color_Secondary * temp_output_19_0 ) , temp_output_19_0);
				float temp_output_25_0_g4 = 0.0;
				float temp_output_22_0_g4 = step( tex2DNode3.g , temp_output_25_0_g4 );
				float temp_output_26_0 = ( temp_output_22_0_g4 - 0.0 );
				float4 lerpResult23 = lerp( lerpResult22 , ( tex2DNode7.a * _Color_Tertiary * temp_output_26_0 ) , temp_output_26_0);
				float2 uv_leatherMask = IN.ase_texcoord2.xy * _leatherMask_ST.xy + _leatherMask_ST.zw;
				float4 tex2DNode1 = tex2D( _leatherMask, uv_leatherMask );
				float temp_output_25_0_g5 = 0.0;
				float temp_output_22_0_g5 = step( tex2DNode1.r , temp_output_25_0_g5 );
				float temp_output_30_0 = ( temp_output_22_0_g5 - 0.0 );
				float4 lerpResult27 = lerp( lerpResult23 , ( tex2DNode7.a * _Color_Leather_Primary * temp_output_30_0 ) , temp_output_30_0);
				float temp_output_25_0_g6 = 0.0;
				float temp_output_22_0_g6 = step( tex2DNode1.g , temp_output_25_0_g6 );
				float temp_output_34_0 = ( temp_output_22_0_g6 - 0.0 );
				float4 lerpResult31 = lerp( lerpResult27 , ( tex2DNode7.a * _Color_Leather_Secondary * temp_output_34_0 ) , temp_output_34_0);
				float temp_output_25_0_g7 = 0.0;
				float temp_output_22_0_g7 = step( tex2DNode1.b , temp_output_25_0_g7 );
				float temp_output_38_0 = ( temp_output_22_0_g7 - 0.0 );
				float4 lerpResult35 = lerp( lerpResult31 , ( tex2DNode7.a * _Color_Leather_Tertiary * temp_output_38_0 ) , temp_output_38_0);
				float2 uv_metalmask = IN.ase_texcoord2.xy * _metalmask_ST.xy + _metalmask_ST.zw;
				float4 tex2DNode2 = tex2D( _metalmask, uv_metalmask );
				float temp_output_25_0_g8 = 0.0;
				float temp_output_22_0_g8 = step( tex2DNode2.r , temp_output_25_0_g8 );
				float temp_output_42_0 = ( temp_output_22_0_g8 - 0.0 );
				float4 lerpResult39 = lerp( lerpResult35 , ( tex2DNode7.a * _Color_Metal_Primary * temp_output_42_0 ) , temp_output_42_0);
				float temp_output_25_0_g9 = 0.0;
				float temp_output_22_0_g9 = step( tex2DNode2.g , temp_output_25_0_g9 );
				float temp_output_46_0 = ( temp_output_22_0_g9 - 0.0 );
				float4 lerpResult43 = lerp( lerpResult39 , ( tex2DNode7.a * _Color_Metal_Secondary * temp_output_46_0 ) , temp_output_46_0);
				float temp_output_25_0_g10 = 0.0;
				float temp_output_22_0_g10 = step( tex2DNode2.b , temp_output_25_0_g10 );
				float temp_output_50_0 = ( temp_output_22_0_g10 - 0.0 );
				float4 lerpResult47 = lerp( lerpResult43 , ( tex2DNode7.a * _Color_Metal_Dark * temp_output_50_0 ) , temp_output_50_0);
				float2 uv_skinhairMask = IN.ase_texcoord2.xy * _skinhairMask_ST.xy + _skinhairMask_ST.zw;
				float4 tex2DNode5 = tex2D( _skinhairMask, uv_skinhairMask );
				float temp_output_25_0_g23 = 0.0;
				float temp_output_22_0_g23 = step( tex2DNode5.r , temp_output_25_0_g23 );
				float temp_output_54_0 = ( temp_output_22_0_g23 - 0.0 );
				float4 lerpResult51 = lerp( lerpResult47 , ( tex2DNode7.a * _Color_Skin * temp_output_54_0 ) , temp_output_54_0);
				float temp_output_25_0_g28 = 0.0;
				float temp_output_22_0_g28 = step( tex2DNode5.g , temp_output_25_0_g28 );
				float temp_output_58_0 = ( temp_output_22_0_g28 - 0.0 );
				float4 lerpResult55 = lerp( lerpResult51 , ( tex2DNode7.a * _Color_Hair * temp_output_58_0 ) , temp_output_58_0);
				float temp_output_25_0_g31 = 0.0;
				float temp_output_22_0_g31 = step( tex2DNode5.b , temp_output_25_0_g31 );
				float temp_output_62_0 = ( temp_output_22_0_g31 - 0.0 );
				float4 lerpResult59 = lerp( lerpResult55 , ( tex2DNode7.a * _Color_Stubble * temp_output_62_0 ) , temp_output_62_0);
				float2 uv_colorMasks = IN.ase_texcoord2.xy * _colorMasks_ST.xy + _colorMasks_ST.zw;
				float4 lerpResult76 = lerp( tex2DNode7 , lerpResult59 , tex2D( _colorMasks, uv_colorMasks ));
				float2 uv_TextureSample0 = IN.ase_texcoord2.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
				float4 tex2DNode89 = tex2D( _TextureSample0, uv_TextureSample0 );
				float temp_output_25_0_g24 = 0.0;
				float temp_output_22_0_g24 = step( tex2DNode89.r , temp_output_25_0_g24 );
				float temp_output_77_0 = ( temp_output_22_0_g24 - 0.0 );
				float4 lerpResult79 = lerp( float4( 0,0,0,0 ) , ( tex2DNode7.a * _Color_Medical * temp_output_77_0 ) , temp_output_77_0);
				float temp_output_25_0_g29 = 0.0;
				float temp_output_22_0_g29 = step( tex2DNode89.b , temp_output_25_0_g29 );
				float temp_output_85_0 = ( temp_output_22_0_g29 - 0.0 );
				float4 lerpResult87 = lerp( lerpResult79 , ( tex2DNode7.a * _Color2 * temp_output_85_0 ) , temp_output_85_0);
				float temp_output_25_0_g32 = 0.0;
				float temp_output_22_0_g32 = step( tex2DNode89.g , temp_output_25_0_g32 );
				float temp_output_85_28 = ( temp_output_25_0_g29 + 0.2 );
				float4 lerpResult83 = lerp( lerpResult87 , ( tex2DNode7.a * _Color1 * ( temp_output_22_0_g32 - 0.0 ) ) , temp_output_85_28);
				float4 lerpResult90 = lerp( lerpResult76 , ( lerpResult83 + lerpResult83 + lerpResult83 ) , temp_output_85_28);
				
				
				float3 Albedo = lerpResult90.rgb;
				float3 Emission = 0;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				MetaInput metaInput = (MetaInput)0;
				metaInput.Albedo = Albedo;
				metaInput.Emission = Emission;
				
				return MetaFragment(metaInput);
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "Universal2D"
			Tags { "LightMode"="Universal2D" }

			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			HLSLPROGRAM
			#define _NORMAL_DROPOFF_TS 1
			#pragma multi_compile_instancing
			#pragma multi_compile _ LOD_FADE_CROSSFADE
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 70503

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS_2D

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			

			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _OriginalTexture_ST;
			float4 _TextureSample0_ST;
			float4 _Color_Medical;
			float4 _colorMasks_ST;
			float4 _Color_Stubble;
			float4 _Color_Hair;
			float4 _skinhairMask_ST;
			float4 _Color_Skin;
			float4 _Color_Metal_Dark;
			float4 _Color_Metal_Secondary;
			float4 _metalmask_ST;
			float4 _Color_Metal_Primary;
			float4 _Color_Leather_Tertiary;
			float4 _Color_Leather_Secondary;
			float4 _leatherMask_ST;
			float4 _Color_Leather_Primary;
			float4 _Color_Tertiary;
			float4 _Color_Secondary;
			float4 _primaryMask_ST;
			float4 _Color_Primary;
			float4 _Color2;
			float4 _Color1;
			#ifdef _TRANSMISSION_ASE
				float _TransmissionShadow;
			#endif
			#ifdef _TRANSLUCENCY_ASE
				float _TransStrength;
				float _TransNormal;
				float _TransScattering;
				float _TransDirect;
				float _TransAmbient;
				float _TransShadow;
			#endif
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _OriginalTexture;
			sampler2D _primaryMask;
			sampler2D _leatherMask;
			sampler2D _metalmask;
			sampler2D _skinhairMask;
			sampler2D _colorMasks;
			sampler2D _TextureSample0;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;
				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = positionCS;
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 WorldPosition = IN.worldPos;
				#endif
				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_OriginalTexture = IN.ase_texcoord2.xy * _OriginalTexture_ST.xy + _OriginalTexture_ST.zw;
				float4 tex2DNode7 = tex2D( _OriginalTexture, uv_OriginalTexture );
				float2 uv_primaryMask = IN.ase_texcoord2.xy * _primaryMask_ST.xy + _primaryMask_ST.zw;
				float4 tex2DNode3 = tex2D( _primaryMask, uv_primaryMask );
				float temp_output_25_0_g2 = 0.0;
				float temp_output_22_0_g2 = step( tex2DNode3.r , temp_output_25_0_g2 );
				float temp_output_17_0 = ( temp_output_22_0_g2 - 0.0 );
				float4 lerpResult18 = lerp( float4( 0,0,0,0 ) , ( tex2DNode7.a * _Color_Primary * temp_output_17_0 ) , temp_output_17_0);
				float temp_output_25_0_g3 = 0.0;
				float temp_output_22_0_g3 = step( tex2DNode3.b , temp_output_25_0_g3 );
				float temp_output_19_0 = ( temp_output_22_0_g3 - 0.0 );
				float4 lerpResult22 = lerp( lerpResult18 , ( tex2DNode7.a * _Color_Secondary * temp_output_19_0 ) , temp_output_19_0);
				float temp_output_25_0_g4 = 0.0;
				float temp_output_22_0_g4 = step( tex2DNode3.g , temp_output_25_0_g4 );
				float temp_output_26_0 = ( temp_output_22_0_g4 - 0.0 );
				float4 lerpResult23 = lerp( lerpResult22 , ( tex2DNode7.a * _Color_Tertiary * temp_output_26_0 ) , temp_output_26_0);
				float2 uv_leatherMask = IN.ase_texcoord2.xy * _leatherMask_ST.xy + _leatherMask_ST.zw;
				float4 tex2DNode1 = tex2D( _leatherMask, uv_leatherMask );
				float temp_output_25_0_g5 = 0.0;
				float temp_output_22_0_g5 = step( tex2DNode1.r , temp_output_25_0_g5 );
				float temp_output_30_0 = ( temp_output_22_0_g5 - 0.0 );
				float4 lerpResult27 = lerp( lerpResult23 , ( tex2DNode7.a * _Color_Leather_Primary * temp_output_30_0 ) , temp_output_30_0);
				float temp_output_25_0_g6 = 0.0;
				float temp_output_22_0_g6 = step( tex2DNode1.g , temp_output_25_0_g6 );
				float temp_output_34_0 = ( temp_output_22_0_g6 - 0.0 );
				float4 lerpResult31 = lerp( lerpResult27 , ( tex2DNode7.a * _Color_Leather_Secondary * temp_output_34_0 ) , temp_output_34_0);
				float temp_output_25_0_g7 = 0.0;
				float temp_output_22_0_g7 = step( tex2DNode1.b , temp_output_25_0_g7 );
				float temp_output_38_0 = ( temp_output_22_0_g7 - 0.0 );
				float4 lerpResult35 = lerp( lerpResult31 , ( tex2DNode7.a * _Color_Leather_Tertiary * temp_output_38_0 ) , temp_output_38_0);
				float2 uv_metalmask = IN.ase_texcoord2.xy * _metalmask_ST.xy + _metalmask_ST.zw;
				float4 tex2DNode2 = tex2D( _metalmask, uv_metalmask );
				float temp_output_25_0_g8 = 0.0;
				float temp_output_22_0_g8 = step( tex2DNode2.r , temp_output_25_0_g8 );
				float temp_output_42_0 = ( temp_output_22_0_g8 - 0.0 );
				float4 lerpResult39 = lerp( lerpResult35 , ( tex2DNode7.a * _Color_Metal_Primary * temp_output_42_0 ) , temp_output_42_0);
				float temp_output_25_0_g9 = 0.0;
				float temp_output_22_0_g9 = step( tex2DNode2.g , temp_output_25_0_g9 );
				float temp_output_46_0 = ( temp_output_22_0_g9 - 0.0 );
				float4 lerpResult43 = lerp( lerpResult39 , ( tex2DNode7.a * _Color_Metal_Secondary * temp_output_46_0 ) , temp_output_46_0);
				float temp_output_25_0_g10 = 0.0;
				float temp_output_22_0_g10 = step( tex2DNode2.b , temp_output_25_0_g10 );
				float temp_output_50_0 = ( temp_output_22_0_g10 - 0.0 );
				float4 lerpResult47 = lerp( lerpResult43 , ( tex2DNode7.a * _Color_Metal_Dark * temp_output_50_0 ) , temp_output_50_0);
				float2 uv_skinhairMask = IN.ase_texcoord2.xy * _skinhairMask_ST.xy + _skinhairMask_ST.zw;
				float4 tex2DNode5 = tex2D( _skinhairMask, uv_skinhairMask );
				float temp_output_25_0_g23 = 0.0;
				float temp_output_22_0_g23 = step( tex2DNode5.r , temp_output_25_0_g23 );
				float temp_output_54_0 = ( temp_output_22_0_g23 - 0.0 );
				float4 lerpResult51 = lerp( lerpResult47 , ( tex2DNode7.a * _Color_Skin * temp_output_54_0 ) , temp_output_54_0);
				float temp_output_25_0_g28 = 0.0;
				float temp_output_22_0_g28 = step( tex2DNode5.g , temp_output_25_0_g28 );
				float temp_output_58_0 = ( temp_output_22_0_g28 - 0.0 );
				float4 lerpResult55 = lerp( lerpResult51 , ( tex2DNode7.a * _Color_Hair * temp_output_58_0 ) , temp_output_58_0);
				float temp_output_25_0_g31 = 0.0;
				float temp_output_22_0_g31 = step( tex2DNode5.b , temp_output_25_0_g31 );
				float temp_output_62_0 = ( temp_output_22_0_g31 - 0.0 );
				float4 lerpResult59 = lerp( lerpResult55 , ( tex2DNode7.a * _Color_Stubble * temp_output_62_0 ) , temp_output_62_0);
				float2 uv_colorMasks = IN.ase_texcoord2.xy * _colorMasks_ST.xy + _colorMasks_ST.zw;
				float4 lerpResult76 = lerp( tex2DNode7 , lerpResult59 , tex2D( _colorMasks, uv_colorMasks ));
				float2 uv_TextureSample0 = IN.ase_texcoord2.xy * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
				float4 tex2DNode89 = tex2D( _TextureSample0, uv_TextureSample0 );
				float temp_output_25_0_g24 = 0.0;
				float temp_output_22_0_g24 = step( tex2DNode89.r , temp_output_25_0_g24 );
				float temp_output_77_0 = ( temp_output_22_0_g24 - 0.0 );
				float4 lerpResult79 = lerp( float4( 0,0,0,0 ) , ( tex2DNode7.a * _Color_Medical * temp_output_77_0 ) , temp_output_77_0);
				float temp_output_25_0_g29 = 0.0;
				float temp_output_22_0_g29 = step( tex2DNode89.b , temp_output_25_0_g29 );
				float temp_output_85_0 = ( temp_output_22_0_g29 - 0.0 );
				float4 lerpResult87 = lerp( lerpResult79 , ( tex2DNode7.a * _Color2 * temp_output_85_0 ) , temp_output_85_0);
				float temp_output_25_0_g32 = 0.0;
				float temp_output_22_0_g32 = step( tex2DNode89.g , temp_output_25_0_g32 );
				float temp_output_85_28 = ( temp_output_25_0_g29 + 0.2 );
				float4 lerpResult83 = lerp( lerpResult87 , ( tex2DNode7.a * _Color1 * ( temp_output_22_0_g32 - 0.0 ) ) , temp_output_85_28);
				float4 lerpResult90 = lerp( lerpResult76 , ( lerpResult83 + lerpResult83 + lerpResult83 ) , temp_output_85_28);
				
				
				float3 Albedo = lerpResult90.rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				half4 color = half4( Albedo, Alpha );

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				return color;
			}
			ENDHLSL
		}
		
	}
	/*ase_lod*/
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18900
2743;79;1536;986;1552.799;3025.634;5.785069;True;True
Node;AmplifyShaderEditor.SamplerNode;3;-1004.295,855.2948;Inherit;True;Property;_primaryMask;primaryMask;2;0;Create;True;0;0;0;False;0;False;-1;4d2aa66f9d16bf644aeced5c66a39109;4d2aa66f9d16bf644aeced5c66a39109;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;7;-2476.411,-180.0257;Inherit;True;Property;_OriginalTexture;OriginalTexture;5;0;Create;True;0;0;0;False;0;False;-1;e599ad2d07d640a49945ec695caacac0;e599ad2d07d640a49945ec695caacac0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;17;-1537.812,418.0414;Inherit;True;MaskingFunction;-1;;2;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;13;-1561.718,195.4911;Inherit;False;Property;_Color_Primary;Color_Primary;17;0;Create;True;0;0;0;False;0;False;0,0.1698678,1,0;0.6078432,0.6078432,0.6078432,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;19;-1132.836,431.679;Inherit;True;MaskingFunction;-1;;3;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1336.812,156.0414;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;20;-1120,240;Inherit;False;Property;_Color_Secondary;Color_Secondary;14;0;Create;True;0;0;0;False;0;False;0.8983961,1,0.0990566,0;0.6078432,0.6078432,0.6078432,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;90.32063,780.6279;Inherit;True;Property;_leatherMask;leatherMask;0;0;Create;True;0;0;0;False;0;False;-1;49123ea06040fdb4e86c6fb0b4288c09;49123ea06040fdb4e86c6fb0b4288c09;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;18;-1363.812,-80.95856;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;24;-702.3935,237.952;Inherit;False;Property;_Color_Tertiary;Color_Tertiary;15;0;Create;True;0;0;0;False;0;False;0.9339623,0,0,0;0.6078432,0.6078432,0.6078432,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;26;-658.2296,423.6309;Inherit;True;MaskingFunction;-1;;4;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-899.8356,200.6789;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;22;-948,-79;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-475.2291,198.6308;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;30;-183.4074,428.2433;Inherit;True;MaskingFunction;-1;;5;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;29;-205.5713,252.5644;Inherit;False;Property;_Color_Leather_Primary;Color_Leather_Primary;21;0;Create;True;0;0;0;False;0;False;0.6037736,0.2933429,0.2933429,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;23;-546.3935,-66.04803;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;33;274.0823,243.8959;Inherit;False;Property;_Color_Leather_Secondary;Color_Leather_Secondary;13;0;Create;True;0;0;0;False;0;False;0.764151,0.1622019,0.1622019,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;14.59301,213.2431;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;34;296.2462,419.575;Inherit;True;MaskingFunction;-1;;6;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;494.2467,204.5746;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;27;23.42867,-56.43567;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;38;819.2415,448.4698;Inherit;True;MaskingFunction;-1;;7;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SamplerNode;2;1750.514,884.5279;Inherit;True;Property;_metalmask;metalmask;1;0;Create;True;0;0;0;False;0;False;-1;f8e518b469cb2bd4c92100e04b7d4ab3;f8e518b469cb2bd4c92100e04b7d4ab3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;37;797.0776,272.7907;Inherit;False;Property;_Color_Leather_Tertiary;Color_Leather_Tertiary;12;0;Create;True;0;0;0;False;0;False;0.764151,0.1622019,0.1622019,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;1017.242,233.4694;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;31;482.0822,-44.10426;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;42;1410.048,493.0276;Inherit;True;MaskingFunction;-1;;8;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;41;1481.884,284.3486;Inherit;False;Property;_Color_Metal_Primary;Color_Metal_Primary;11;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;45;2030.884,298.796;Inherit;False;Property;_Color_Metal_Secondary;Color_Metal_Secondary;10;0;Create;True;0;0;0;False;0;False;0.764151,0.1622019,0.1622019,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;1702.049,245.0275;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;46;2053.049,474.4749;Inherit;True;MaskingFunction;-1;;9;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;35;1005.077,-15.20952;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;39;1689.884,-3.651451;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;49;2516.316,310.3539;Inherit;False;Property;_Color_Metal_Dark;Color_Metal_Dark;9;0;Create;True;0;0;0;False;0;False;0.7924528,0.06354576,0.06354576,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;2251.05,259.4748;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;89;717.1438,-1327.472;Inherit;True;Property;_TextureSample0;Texture Sample 0;20;0;Create;True;0;0;0;False;0;False;-1;ea89a3aa54bb30e4c82b012a1a158672;4d2aa66f9d16bf644aeced5c66a39109;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;50;2538.481,486.0329;Inherit;True;MaskingFunction;-1;;10;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SamplerNode;5;3103.774,1041.308;Inherit;True;Property;_skinhairMask;skinhairMask;4;0;Create;True;0;0;0;False;0;False;-1;83c8563e287501f4997504c772dd2387;83c8563e287501f4997504c772dd2387;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;54;3230.874,477.4809;Inherit;True;MaskingFunction;-1;;23;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;52;3208.709,301.8019;Inherit;False;Property;_Color_Skin;Color_Skin;8;0;Create;True;0;0;0;False;0;False;1,0.8726415,0.8726415,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;80;-112.8903,-1696.61;Inherit;False;Property;_Color_Medical;Color_Medical;22;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;77;-90.72515,-1520.931;Inherit;True;MaskingFunction;-1;;24;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;2736.482,271.0327;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;43;2238.884,10.79588;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;276.8357,-1665.075;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;47;2724.316,22.35382;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;88;1318.737,-1800.652;Inherit;False;Property;_Color2;Color 2;19;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;56;3760.708,321.3077;Inherit;False;Property;_Color_Hair;Color_Hair;7;0;Create;True;0;0;0;False;0;False;1,0.8726415,0.8726415,0;0.6078432,0.6078432,0.6078432,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;3428.875,262.4807;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;58;3782.873,496.9867;Inherit;True;MaskingFunction;-1;;28;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.FunctionNode;85;1340.902,-1624.973;Inherit;True;MaskingFunction;-1;;29;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.FunctionNode;81;550.1774,-1724.854;Inherit;True;MaskingFunction;-1;;32;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;79;95.10992,-1984.61;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;60;4251.75,340.8899;Inherit;False;Property;_Color_Stubble;Color_Stubble;16;0;Create;True;0;0;0;False;0;False;1,0.8726415,0.8726415,0;0.6078432,0.6078432,0.6078432,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;62;4273.915,516.5689;Inherit;True;MaskingFunction;-1;;31;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;51;3416.709,13.80192;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;1538.903,-1839.973;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;3980.873,281.9865;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;84;528.0123,-1900.533;Inherit;False;Property;_Color1;Color 1;18;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;4471.916,301.5688;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;748.1785,-1939.854;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;87;721.7371,-2186.652;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;55;3968.707,33.30767;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;59;4459.75,52.88976;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;74;-455.828,-804.2808;Inherit;True;Property;_colorMasks;colorMasks;6;0;Create;True;0;0;0;False;0;False;-1;7a76017a04248954cbb6d454bfa3900e;4d2aa66f9d16bf644aeced5c66a39109;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;83;1641.012,-2188.533;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;76;1701,-922.8419;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;94;1976.951,-1880.634;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;90;2442.779,-1794.977;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;972.7799,842.138;Inherit;True;Property;_shapeMask;shapeMask;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;99;5387.337,-1058.624;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;95;5387.337,-1058.624;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;96;5387.337,-1058.624;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;2;HNGamers_SyntyStandardCharacter_Urp;94348b07e5e8bab40bd6c8a1e3df54cd;True;Forward;0;1;Forward;18;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;38;Workflow;1;Surface;0;  Refraction Model;0;  Blend;0;Two Sided;1;Fragment Normal Space,InvertActionOnDeselection;0;Transmission;0;  Transmission Shadow;0.5,False,-1;Translucency;0;  Translucency Strength;1,False,-1;  Normal Distortion;0.5,False,-1;  Scattering;2,False,-1;  Direct;0.9,False,-1;  Ambient;0.1,False,-1;  Shadow;0.5,False,-1;Cast Shadows;1;  Use Shadow Threshold;0;Receive Shadows;1;GPU Instancing;1;LOD CrossFade;1;Built-in Fog;1;_FinalColorxAlpha;0;Meta Pass;1;Override Baked GI;0;Extra Pre Pass;0;DOTS Instancing;0;Tessellation;0;  Phong;0;  Strength;0.5,False,-1;  Type;0;  Tess;16,False,-1;  Min;10,False,-1;  Max;25,False,-1;  Edge Length;16,False,-1;  Max Displacement;25,False,-1;Write Depth;0;  Early Z;0;Vertex Position,InvertActionOnDeselection;1;0;6;False;True;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;97;5387.337,-1058.624;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;100;5387.337,-1058.624;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;True;1;1;False;-1;0;False;-1;1;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;LightMode=Universal2D;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;98;5387.337,-1058.624;Float;False;False;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;1;New Amplify Shader;94348b07e5e8bab40bd6c8a1e3df54cd;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;False;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
WireConnection;17;21;3;1
WireConnection;19;21;3;3
WireConnection;15;0;7;4
WireConnection;15;1;13;0
WireConnection;15;2;17;0
WireConnection;18;1;15;0
WireConnection;18;2;17;0
WireConnection;26;21;3;2
WireConnection;21;0;7;4
WireConnection;21;1;20;0
WireConnection;21;2;19;0
WireConnection;22;0;18;0
WireConnection;22;1;21;0
WireConnection;22;2;19;0
WireConnection;25;0;7;4
WireConnection;25;1;24;0
WireConnection;25;2;26;0
WireConnection;30;21;1;1
WireConnection;23;0;22;0
WireConnection;23;1;25;0
WireConnection;23;2;26;0
WireConnection;28;0;7;4
WireConnection;28;1;29;0
WireConnection;28;2;30;0
WireConnection;34;21;1;2
WireConnection;32;0;7;4
WireConnection;32;1;33;0
WireConnection;32;2;34;0
WireConnection;27;0;23;0
WireConnection;27;1;28;0
WireConnection;27;2;30;0
WireConnection;38;21;1;3
WireConnection;36;0;7;4
WireConnection;36;1;37;0
WireConnection;36;2;38;0
WireConnection;31;0;27;0
WireConnection;31;1;32;0
WireConnection;31;2;34;0
WireConnection;42;21;2;1
WireConnection;40;0;7;4
WireConnection;40;1;41;0
WireConnection;40;2;42;0
WireConnection;46;21;2;2
WireConnection;35;0;31;0
WireConnection;35;1;36;0
WireConnection;35;2;38;0
WireConnection;39;0;35;0
WireConnection;39;1;40;0
WireConnection;39;2;42;0
WireConnection;44;0;7;4
WireConnection;44;1;45;0
WireConnection;44;2;46;0
WireConnection;50;21;2;3
WireConnection;54;21;5;1
WireConnection;77;21;89;1
WireConnection;48;0;7;4
WireConnection;48;1;49;0
WireConnection;48;2;50;0
WireConnection;43;0;39;0
WireConnection;43;1;44;0
WireConnection;43;2;46;0
WireConnection;78;0;7;4
WireConnection;78;1;80;0
WireConnection;78;2;77;0
WireConnection;47;0;43;0
WireConnection;47;1;48;0
WireConnection;47;2;50;0
WireConnection;53;0;7;4
WireConnection;53;1;52;0
WireConnection;53;2;54;0
WireConnection;58;21;5;2
WireConnection;85;21;89;3
WireConnection;81;21;89;2
WireConnection;79;1;78;0
WireConnection;79;2;77;0
WireConnection;62;21;5;3
WireConnection;51;0;47;0
WireConnection;51;1;53;0
WireConnection;51;2;54;0
WireConnection;86;0;7;4
WireConnection;86;1;88;0
WireConnection;86;2;85;0
WireConnection;57;0;7;4
WireConnection;57;1;56;0
WireConnection;57;2;58;0
WireConnection;61;0;7;4
WireConnection;61;1;60;0
WireConnection;61;2;62;0
WireConnection;82;0;7;4
WireConnection;82;1;84;0
WireConnection;82;2;81;0
WireConnection;87;0;79;0
WireConnection;87;1;86;0
WireConnection;87;2;85;0
WireConnection;55;0;51;0
WireConnection;55;1;57;0
WireConnection;55;2;58;0
WireConnection;59;0;55;0
WireConnection;59;1;61;0
WireConnection;59;2;62;0
WireConnection;83;0;87;0
WireConnection;83;1;82;0
WireConnection;83;2;85;28
WireConnection;76;0;7;0
WireConnection;76;1;59;0
WireConnection;76;2;74;0
WireConnection;94;0;83;0
WireConnection;94;1;83;0
WireConnection;94;2;83;0
WireConnection;90;0;76;0
WireConnection;90;1;94;0
WireConnection;90;2;85;28
WireConnection;96;0;90;0
ASEEND*/
//CHKSM=C71D99A2961ECBDBE6283992926D3FDF4DB57B5B