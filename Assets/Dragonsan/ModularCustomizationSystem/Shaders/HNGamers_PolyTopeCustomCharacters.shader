// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "HNGamers/PolyTopeCustomCharacter"
{
	Properties
	{
		_Color_Primary("Color_Primary", Color) = (0.1333333,0.4960412,0.9098039,0)
		_Color_Tertiary("Color_Tertiary", Color) = (0.1137255,0.972549,0.9014137,0)
		_Color_Secondary("Color_Secondary", Color) = (0.1137255,0.972549,0.5000383,0)
		_Color_Leather_Primary("Color_Leather_Primary", Color) = (0.6698113,0.5611847,0.3380651,0)
		_Color_Metal_Primary("Color_Metal_Primary", Color) = (0.6320754,0.6290939,0.6290939,0)
		_Color_Leather_Tertiary("Color_Leather_Tertiary", Color) = (0.4433962,0.3422169,0.131764,1)
		_Color_Leather_Secondary("Color_Leather_Secondary", Color) = (0.5566038,0.4571441,0.2546725,1)
		_Color_Metal_Dark("Color_Metal_Dark", Color) = (0.1603774,0.1603774,0.1603774,0)
		_Color_Metal_Secondary("Color_Metal_Secondary", Color) = (0.4622642,0.4382788,0.4382788,0)
		_Color_Hair("Color_Hair", Color) = (0.3773585,0.3152891,0.3079388,0)
		_Color_Skin("Color_Skin", Color) = (0.9056604,0.6070963,0.4314703,1)
		_Color_Stubble("Color_Stubble", Color) = (0.7735849,0.646368,0.5582948,1)
		_Color_Scar("Color_Scar", Color) = (0.8113208,0.2870239,0.3075845,1)
		_Color_Eyes("Color_Eyes", Color) = (0.5283019,0.3862585,0.3862585,1)
		_Emission("Emission", Range( 0 , 1)) = 0
		_Texture("Texture", 2D) = "white" {}
		_PT_Medieval_Armors_Skin_Eye_Hair_Mask_01("PT_Medieval_Armors_Skin_Eye_Hair_Mask_01", 2D) = "white" {}
		_Color_Mouth("Color_Mouth", Color) = (0.6132076,0.2053667,0.2389537,1)
		_PT_Medieval_Armors_Lips_Scars_Mask_01("PT_Medieval_Armors_Lips_Scars_Mask_01", 2D) = "white" {}
		_PT_Medieval_Armors_Metal_Mask_01("PT_Medieval_Armors_Metal_Mask_01", 2D) = "white" {}
		_PT_Medieval_Coat_of_Arms_01("PT_Medieval_Coat_of_Arms_01", 2D) = "white" {}
		_PT_Medieval_Armors_Leather_Mask_01("PT_Medieval_Armors_Leather_Mask_01", 2D) = "white" {}
		_PT_Medieval_Armors_Cloth_Mask_01("PT_Medieval_Armors_Cloth_Mask_01", 2D) = "white" {}
		_BlueEyeRedMouthGreenHair("BlueEye-RedMouth-GreenHair", 2D) = "white" {}
		[Toggle]_Metallic("Metallic", Float) = 0
		[Toggle]_Smoothness("Smoothness", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Texture;
		uniform float4 _Texture_ST;
		uniform sampler2D _PT_Medieval_Armors_Cloth_Mask_01;
		uniform float4 _PT_Medieval_Armors_Cloth_Mask_01_ST;
		uniform float4 _Color_Primary;
		uniform float4 _Color_Secondary;
		uniform float4 _Color_Tertiary;
		uniform sampler2D _PT_Medieval_Armors_Leather_Mask_01;
		uniform float4 _PT_Medieval_Armors_Leather_Mask_01_ST;
		uniform float4 _Color_Leather_Primary;
		uniform float4 _Color_Leather_Secondary;
		uniform float4 _Color_Leather_Tertiary;
		uniform sampler2D _PT_Medieval_Armors_Metal_Mask_01;
		uniform float4 _PT_Medieval_Armors_Metal_Mask_01_ST;
		uniform float4 _Color_Metal_Primary;
		uniform float4 _Color_Metal_Secondary;
		uniform float4 _Color_Metal_Dark;
		uniform sampler2D _PT_Medieval_Armors_Skin_Eye_Hair_Mask_01;
		uniform float4 _PT_Medieval_Armors_Skin_Eye_Hair_Mask_01_ST;
		uniform float4 _Color_Hair;
		uniform float4 _Color_Skin;
		uniform sampler2D _PT_Medieval_Armors_Lips_Scars_Mask_01;
		uniform float4 _PT_Medieval_Armors_Lips_Scars_Mask_01_ST;
		uniform float4 _Color_Mouth;
		uniform float4 _Color_Scar;
		uniform float4 _Color_Stubble;
		uniform sampler2D _BlueEyeRedMouthGreenHair;
		uniform float4 _BlueEyeRedMouthGreenHair_ST;
		uniform float4 _Color_Eyes;
		uniform sampler2D _PT_Medieval_Coat_of_Arms_01;
		uniform float4 _PT_Medieval_Coat_of_Arms_01_ST;
		uniform float _Emission;
		uniform float _Metallic;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			float4 tex2DNode37 = tex2D( _Texture, uv_Texture, float2( 0,0 ), float2( 0,0 ) );
			float2 uv_PT_Medieval_Armors_Cloth_Mask_01 = i.uv_texcoord * _PT_Medieval_Armors_Cloth_Mask_01_ST.xy + _PT_Medieval_Armors_Cloth_Mask_01_ST.zw;
			float4 tex2DNode222 = tex2D( _PT_Medieval_Armors_Cloth_Mask_01, uv_PT_Medieval_Armors_Cloth_Mask_01 );
			float temp_output_25_0_g22 = 0.0;
			float temp_output_22_0_g22 = step( tex2DNode222.r , temp_output_25_0_g22 );
			float temp_output_236_0 = ( temp_output_22_0_g22 - 0.0 );
			float4 lerpResult251 = lerp( ( tex2DNode37 * float4( 0,0,0,0 ) ) , ( tex2DNode37 * temp_output_236_0 * _Color_Primary ) , temp_output_236_0);
			float temp_output_25_0_g23 = 0.0;
			float temp_output_22_0_g23 = step( tex2DNode222.g , temp_output_25_0_g23 );
			float temp_output_235_0 = ( temp_output_22_0_g23 - 0.0 );
			float4 lerpResult252 = lerp( lerpResult251 , ( tex2DNode37 * temp_output_235_0 * _Color_Secondary ) , temp_output_235_0);
			float temp_output_25_0_g44 = 0.0;
			float temp_output_22_0_g44 = step( tex2DNode222.b , temp_output_25_0_g44 );
			float temp_output_295_0 = ( temp_output_22_0_g44 - 0.0 );
			float4 lerpResult298 = lerp( lerpResult252 , ( tex2DNode37 * temp_output_295_0 * _Color_Tertiary ) , temp_output_295_0);
			float2 uv_PT_Medieval_Armors_Leather_Mask_01 = i.uv_texcoord * _PT_Medieval_Armors_Leather_Mask_01_ST.xy + _PT_Medieval_Armors_Leather_Mask_01_ST.zw;
			float4 tex2DNode220 = tex2D( _PT_Medieval_Armors_Leather_Mask_01, uv_PT_Medieval_Armors_Leather_Mask_01 );
			float temp_output_25_0_g45 = 0.0;
			float temp_output_22_0_g45 = step( tex2DNode220.r , temp_output_25_0_g45 );
			float temp_output_234_0 = ( temp_output_22_0_g45 - 0.0 );
			float4 lerpResult253 = lerp( lerpResult298 , ( tex2DNode37 * temp_output_234_0 * _Color_Leather_Primary ) , temp_output_234_0);
			float temp_output_25_0_g46 = 0.0;
			float temp_output_22_0_g46 = step( tex2DNode220.g , temp_output_25_0_g46 );
			float temp_output_233_0 = ( temp_output_22_0_g46 - 0.0 );
			float4 lerpResult254 = lerp( lerpResult253 , ( tex2DNode37 * temp_output_233_0 * _Color_Leather_Secondary ) , temp_output_233_0);
			float temp_output_25_0_g47 = 0.0;
			float temp_output_22_0_g47 = step( tex2DNode220.b , temp_output_25_0_g47 );
			float temp_output_302_0 = ( temp_output_22_0_g47 - 0.0 );
			float4 lerpResult299 = lerp( lerpResult254 , ( tex2DNode37 * temp_output_302_0 * _Color_Leather_Tertiary ) , temp_output_302_0);
			float2 uv_PT_Medieval_Armors_Metal_Mask_01 = i.uv_texcoord * _PT_Medieval_Armors_Metal_Mask_01_ST.xy + _PT_Medieval_Armors_Metal_Mask_01_ST.zw;
			float4 tex2DNode217 = tex2D( _PT_Medieval_Armors_Metal_Mask_01, uv_PT_Medieval_Armors_Metal_Mask_01 );
			float temp_output_25_0_g48 = 0.0;
			float temp_output_22_0_g48 = step( tex2DNode217.r , temp_output_25_0_g48 );
			float temp_output_231_0 = ( temp_output_22_0_g48 - 0.0 );
			float4 lerpResult255 = lerp( lerpResult299 , ( tex2DNode37 * temp_output_231_0 * _Color_Metal_Primary ) , temp_output_231_0);
			float temp_output_25_0_g49 = 0.0;
			float temp_output_22_0_g49 = step( tex2DNode217.g , temp_output_25_0_g49 );
			float temp_output_232_0 = ( temp_output_22_0_g49 - 0.0 );
			float4 lerpResult256 = lerp( lerpResult255 , ( tex2DNode37 * temp_output_232_0 * _Color_Metal_Secondary ) , temp_output_232_0);
			float temp_output_25_0_g50 = 0.0;
			float temp_output_22_0_g50 = step( tex2DNode217.b , temp_output_25_0_g50 );
			float temp_output_230_0 = ( temp_output_22_0_g50 - 0.0 );
			float4 lerpResult257 = lerp( lerpResult256 , ( tex2DNode37 * temp_output_230_0 * _Color_Metal_Dark ) , temp_output_230_0);
			float2 uv_PT_Medieval_Armors_Skin_Eye_Hair_Mask_01 = i.uv_texcoord * _PT_Medieval_Armors_Skin_Eye_Hair_Mask_01_ST.xy + _PT_Medieval_Armors_Skin_Eye_Hair_Mask_01_ST.zw;
			float4 tex2DNode187 = tex2D( _PT_Medieval_Armors_Skin_Eye_Hair_Mask_01, uv_PT_Medieval_Armors_Skin_Eye_Hair_Mask_01 );
			float temp_output_25_0_g51 = 0.0;
			float temp_output_22_0_g51 = step( tex2DNode187.g , temp_output_25_0_g51 );
			float temp_output_212_0 = ( temp_output_22_0_g51 - 0.0 );
			float4 lerpResult258 = lerp( lerpResult257 , ( tex2DNode37 * temp_output_212_0 * _Color_Hair ) , temp_output_212_0);
			float temp_output_25_0_g52 = 0.0;
			float temp_output_22_0_g52 = step( tex2DNode187.r , temp_output_25_0_g52 );
			float temp_output_200_0 = ( temp_output_22_0_g52 - 0.0 );
			float4 lerpResult260 = lerp( lerpResult258 , ( tex2DNode37 * temp_output_200_0 * _Color_Skin ) , temp_output_200_0);
			float2 uv_PT_Medieval_Armors_Lips_Scars_Mask_01 = i.uv_texcoord * _PT_Medieval_Armors_Lips_Scars_Mask_01_ST.xy + _PT_Medieval_Armors_Lips_Scars_Mask_01_ST.zw;
			float4 tex2DNode215 = tex2D( _PT_Medieval_Armors_Lips_Scars_Mask_01, uv_PT_Medieval_Armors_Lips_Scars_Mask_01 );
			float temp_output_25_0_g54 = 0.0;
			float temp_output_22_0_g54 = step( tex2DNode215.g , temp_output_25_0_g54 );
			float temp_output_214_0 = ( temp_output_22_0_g54 - 0.0 );
			float4 lerpResult262 = lerp( lerpResult260 , ( tex2DNode37 * temp_output_214_0 * _Color_Mouth ) , temp_output_214_0);
			float temp_output_25_0_g58 = 0.0;
			float temp_output_22_0_g58 = step( tex2DNode215.b , temp_output_25_0_g58 );
			float temp_output_274_0 = ( temp_output_22_0_g58 - 0.0 );
			float4 lerpResult276 = lerp( lerpResult262 , ( tex2DNode37 * temp_output_274_0 * _Color_Scar ) , temp_output_274_0);
			float temp_output_25_0_g57 = 0.0;
			float temp_output_22_0_g57 = step( tex2DNode215.r , temp_output_25_0_g57 );
			float temp_output_201_0 = ( temp_output_22_0_g57 - 0.0 );
			float4 lerpResult261 = lerp( lerpResult276 , ( tex2DNode37 * temp_output_201_0 * _Color_Stubble ) , temp_output_201_0);
			float2 uv_BlueEyeRedMouthGreenHair = i.uv_texcoord * _BlueEyeRedMouthGreenHair_ST.xy + _BlueEyeRedMouthGreenHair_ST.zw;
			float temp_output_25_0_g59 = 0.0;
			float temp_output_22_0_g59 = step( tex2D( _BlueEyeRedMouthGreenHair, uv_BlueEyeRedMouthGreenHair ).b , temp_output_25_0_g59 );
			float temp_output_272_0 = ( temp_output_22_0_g59 - 0.0 );
			float4 lerpResult265 = lerp( lerpResult261 , ( tex2DNode37 * temp_output_272_0 * _Color_Eyes ) , temp_output_272_0);
			float4 color293 = IsGammaSpace() ? float4(1,0,0,0) : float4(1,0,0,0);
			float2 uv_PT_Medieval_Coat_of_Arms_01 = i.uv_texcoord * _PT_Medieval_Coat_of_Arms_01_ST.xy + _PT_Medieval_Coat_of_Arms_01_ST.zw;
			float temp_output_288_0 = ( 1.0 - tex2D( _PT_Medieval_Coat_of_Arms_01, uv_PT_Medieval_Coat_of_Arms_01 ).a );
			float4 temp_cast_0 = (temp_output_288_0).xxxx;
			float4 temp_output_1_0_g60 = temp_cast_0;
			float4 color292 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
			float4 temp_output_2_0_g60 = color292;
			float temp_output_11_0_g60 = distance( temp_output_1_0_g60 , temp_output_2_0_g60 );
			float2 _Vector0 = float2(1.6,1);
			float4 lerpResult21_g60 = lerp( color293 , temp_output_1_0_g60 , saturate( ( ( temp_output_11_0_g60 - _Vector0.x ) / max( _Vector0.y , 1E-05 ) ) ));
			float4 lerpResult290 = lerp( lerpResult265 , lerpResult21_g60 , ( 1.0 - temp_output_288_0 ));
			o.Albedo = lerpResult290.rgb;
			float4 color189 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
			float3 temp_cast_2 = (( ( 1.0 - color189.r ) * _Emission )).xxx;
			o.Emission = temp_cast_2;
			float lerpResult308 = lerp( 0.0 , 0.65 , temp_output_231_0);
			float lerpResult309 = lerp( lerpResult308 , 0.65 , temp_output_232_0);
			float lerpResult310 = lerp( lerpResult309 , 0.65 , temp_output_230_0);
			o.Metallic = (( _Metallic )?( lerpResult310 ):( 0.0 ));
			float lerpResult321 = lerp( 0.0 , 0.3 , temp_output_234_0);
			float lerpResult323 = lerp( lerpResult321 , 0.3 , temp_output_233_0);
			float lerpResult325 = lerp( lerpResult323 , 0.3 , temp_output_302_0);
			float lerpResult333 = lerp( lerpResult325 , 0.7 , temp_output_231_0);
			float lerpResult337 = lerp( lerpResult333 , 0.7 , temp_output_232_0);
			float lerpResult334 = lerp( lerpResult337 , 0.7 , temp_output_230_0);
			float lerpResult315 = lerp( lerpResult334 , 0.7 , temp_output_212_0);
			float lerpResult317 = lerp( lerpResult315 , 0.3 , temp_output_200_0);
			float lerpResult319 = lerp( lerpResult317 , 0.7 , temp_output_201_0);
			float lerpResult327 = lerp( lerpResult319 , 0.4 , temp_output_214_0);
			float lerpResult329 = lerp( lerpResult327 , 0.3 , temp_output_274_0);
			float lerpResult331 = lerp( lerpResult329 , 0.1 , temp_output_272_0);
			o.Smoothness = (( _Smoothness )?( lerpResult331 ):( 0.0 ));
			o.Occlusion = (0.0 + (0.5 - 0.0) * (1.0 - 0.0) / (1.0 - 0.0));
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
2743;79;1536;986;-3045.807;-1060.718;1;True;True
Node;AmplifyShaderEditor.SamplerNode;222;-1243.432,330.0217;Inherit;True;Property;_PT_Medieval_Armors_Cloth_Mask_01;PT_Medieval_Armors_Cloth_Mask_01;22;0;Create;True;0;0;0;False;0;False;-1;e4f1621d61032d045964d463b3806afe;e4f1621d61032d045964d463b3806afe;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;37;-1331.566,-1298.976;Inherit;True;Property;_Texture;Texture;15;0;Create;True;0;0;0;False;0;False;-1;004b7cabc9421734bb88a754e99fd641;004b7cabc9421734bb88a754e99fd641;True;0;False;white;Auto;False;Object;-1;Derivative;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;33;-1181.63,-220.4949;Float;False;Property;_Color_Primary;Color_Primary;0;0;Create;True;0;0;0;False;0;False;0.1333333,0.4960412,0.9098039,0;0.1333659,0.764151,0.1610319,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;236;-1424.079,20.04883;Inherit;True;MaskingFunction;-1;;22;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;340;-1308.344,-1037.918;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;237;-1058.281,-511.0466;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;235;-1000.104,1.80209;Inherit;True;MaskingFunction;-1;;23;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;40;-782.6186,-206.4698;Float;False;Property;_Color_Secondary;Color_Secondary;2;0;Create;True;0;0;0;False;0;False;0.1137255,0.972549,0.5000383,0;0.9716981,0.1145869,0.1145869,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;296;-429.4438,-221.8035;Float;False;Property;_Color_Tertiary;Color_Tertiary;1;0;Create;True;0;0;0;False;0;False;0.1137255,0.972549,0.9014137,0;0.1137254,0.972549,0.9014137,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;220;-426.6665,420.8526;Inherit;True;Property;_PT_Medieval_Armors_Leather_Mask_01;PT_Medieval_Armors_Leather_Mask_01;21;0;Create;True;0;0;0;False;0;False;-1;9c0e067347abba2489817b3ce813c911;9c0e067347abba2489817b3ce813c911;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;295;-542.4357,4.180714;Inherit;True;MaskingFunction;-1;;44;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;251;-853.3527,-770.8041;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;238;-683.9642,-474.1678;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;44;-115.6029,-227.9229;Float;False;Property;_Color_Leather_Primary;Color_Leather_Primary;3;0;Create;True;0;0;0;False;0;False;0.6698113,0.5611847,0.3380651,0;1,1,1,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;297;-297.9817,-477.1224;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;234;-126.2831,1.917908;Inherit;True;MaskingFunction;-1;;45;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;252;-503.2435,-779.4963;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;320;-111.7204,1022.45;Inherit;False;Constant;_Float6;Float 6;26;0;Create;True;0;0;0;False;0;False;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;233;281.4805,3.283642;Inherit;True;MaskingFunction;-1;;46;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;63;247.8499,-230.9659;Float;False;Property;_Color_Leather_Secondary;Color_Leather_Secondary;6;0;Create;True;0;0;0;False;0;False;0.5566038,0.4571441,0.2546725,1;1,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;298;-180.6431,-778.0726;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;239;15.91557,-486.2087;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;321;-97.83324,1127.993;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;322;95.19873,1026.616;Inherit;False;Constant;_Float7;Float 7;26;0;Create;True;0;0;0;False;0;False;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;217;803.161,276.3485;Inherit;True;Property;_PT_Medieval_Armors_Metal_Mask_01;PT_Medieval_Armors_Metal_Mask_01;19;0;Create;True;0;0;0;False;0;False;-1;47efbf030a9bb7f428ba51b46a2fdd03;47efbf030a9bb7f428ba51b46a2fdd03;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;253;167.2368,-765.9283;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;301;597.7349,-232.2355;Float;False;Property;_Color_Leather_Tertiary;Color_Leather_Tertiary;5;0;Create;True;0;0;0;False;0;False;0.4433962,0.3422169,0.131764,1;0.4433961,0.3422168,0.1317639,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;302;662.4528,-8.856423;Inherit;True;MaskingFunction;-1;;47;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;240;308.1527,-486.4864;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;125;917.1165,-224.3813;Float;False;Property;_Color_Metal_Primary;Color_Metal_Primary;4;0;Create;True;0;0;0;False;0;False;0.6320754,0.6290939,0.6290939,0;1,0.08018861,0.08018861,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;300;702.3545,-482.7762;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;231;1023.747,-32.84896;Inherit;True;MaskingFunction;-1;;48;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.RangedFloatNode;324;320.1697,1025.228;Inherit;False;Constant;_Float8;Float 8;26;0;Create;True;0;0;0;False;0;False;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;323;167.4117,1132.159;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;254;430.987,-766.99;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;241;1044.363,-476.4036;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;299;740.6779,-762.8619;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;232;1388.143,-49.68764;Inherit;True;MaskingFunction;-1;;49;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;139;1295.29,-225.1316;Float;False;Property;_Color_Metal_Secondary;Color_Metal_Secondary;8;0;Create;True;0;0;0;False;0;False;0.4622642,0.4382788,0.4382788,0;1,0.3726414,0.3726414,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;335;932.5232,1051.787;Inherit;False;Constant;_Float13;Float 13;26;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;325;454.8757,1134.936;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;230;1806.061,-39.18302;Inherit;True;MaskingFunction;-1;;50;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;333;946.4111,1157.329;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;187;1930.878,328.9085;Inherit;True;Property;_PT_Medieval_Armors_Skin_Eye_Hair_Mask_01;PT_Medieval_Armors_Skin_Eye_Hair_Mask_01;16;0;Create;True;0;0;0;False;0;False;-1;b76fe68d69ca53f43a4e6f66d135dd90;b76fe68d69ca53f43a4e6f66d135dd90;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;147;1700.544,-233.3216;Float;False;Property;_Color_Metal_Dark;Color_Metal_Dark;7;0;Create;True;0;0;0;False;0;False;0.1603774,0.1603774,0.1603774,0;1,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;332;1139.442,1055.953;Inherit;False;Constant;_Float12;Float 12;26;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;242;1436.914,-471.6147;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;255;1074.415,-765.0511;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;336;1364.414,1054.565;Inherit;False;Constant;_Float14;Float 14;26;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;337;1211.655,1161.496;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;212;2237.833,-41.0896;Inherit;True;MaskingFunction;-1;;51;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;256;1395.98,-745.9647;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;48;2106.773,-242.5243;Float;False;Property;_Color_Hair;Color_Hair;9;0;Create;True;0;0;0;False;0;False;0.3773585,0.3152891,0.3079388,0;1,0.1074455,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;243;1867.914,-455.6147;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;314;2438.449,1051.261;Inherit;False;Constant;_Float3;Float 3;26;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;215;3498.443,450.6407;Inherit;True;Property;_PT_Medieval_Armors_Lips_Scars_Mask_01;PT_Medieval_Armors_Lips_Scars_Mask_01;18;0;Create;True;0;0;0;False;0;False;-1;9a688ecd3c6cbee4cb2a528bf72399d4;9a688ecd3c6cbee4cb2a528bf72399d4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;334;1499.119,1164.273;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;200;2714.369,-35.4164;Inherit;True;MaskingFunction;-1;;52;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;257;1839.98,-740.9647;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;244;2335.914,-463.6147;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;52;2629.302,-238.8943;Float;False;Property;_Color_Skin;Color_Skin;10;0;Create;True;0;0;0;False;0;False;0.9056604,0.6070963,0.4314703,1;0.9056604,0.6070963,0.4314702,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;315;2452.337,1156.803;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;259;2849.458,-451.2798;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;258;2340.458,-701.2798;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;198;3469.136,-234.3918;Float;False;Property;_Color_Mouth;Color_Mouth;17;0;Create;True;0;0;0;False;0;False;0.6132076,0.2053667,0.2389537,1;1,0,0.08150663,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;316;2645.368,1055.427;Inherit;False;Constant;_Float4;Float 4;26;0;Create;True;0;0;0;False;0;False;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;214;3538.411,-40.07335;Inherit;True;MaskingFunction;-1;;54;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.FunctionNode;274;3949.887,-48.47437;Inherit;True;MaskingFunction;-1;;58;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.RangedFloatNode;318;2870.34,1054.039;Inherit;False;Constant;_Float5;Float 5;26;0;Create;True;0;0;0;False;0;False;0.7;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;246;3740.914,-491.6147;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;317;2717.582,1160.969;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;201;3115.369,-61.41644;Inherit;True;MaskingFunction;-1;;57;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;260;2776.458,-742.2798;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;60;3905.914,-261.9717;Float;False;Property;_Color_Scar;Color_Scar;12;0;Create;True;0;0;0;False;0;False;0.8113208,0.2870239,0.3075845,1;0.8113207,0.2870238,0.3075844,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;304;2868.471,401.1607;Inherit;True;Property;_BlueEyeRedMouthGreenHair;BlueEye-RedMouth-GreenHair;23;0;Create;True;0;0;0;False;0;False;-1;51aad9b792a71fc49a865ef1ad0c8618;51aad9b792a71fc49a865ef1ad0c8618;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;319;3005.045,1163.747;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;262;3079.842,-834.3141;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;305;705.5397,559.4292;Inherit;False;Constant;_Float0;Float 0;26;0;Create;True;0;0;0;False;0;False;0.65;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;56;3053.307,-276.8953;Float;False;Property;_Color_Stubble;Color_Stubble;11;0;Create;True;0;0;0;False;0;False;0.7735849,0.646368,0.5582948,1;0.7735849,0.646368,0.5582948,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;275;4141.887,-476.4744;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;326;3705.588,972.3257;Inherit;False;Constant;_Float9;Float 9;26;0;Create;True;0;0;0;False;0;False;0.4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;276;3371.279,-839.3466;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;306;912.4584,563.5954;Inherit;False;Constant;_Float1;Float 1;26;0;Create;True;0;0;0;False;0;False;0.65;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;327;3719.475,1077.868;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;245;3248.914,-457.6147;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;272;4348.309,-72.43969;Inherit;True;MaskingFunction;-1;;59;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SamplerNode;287;4535.381,-1029.895;Inherit;True;Property;_PT_Medieval_Coat_of_Arms_01;PT_Medieval_Coat_of_Arms_01;20;0;Create;True;0;0;0;False;0;False;-1;d294e9544b9eca64188ea9d2482ea8a1;d294e9544b9eca64188ea9d2482ea8a1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;328;3912.507,976.4917;Inherit;False;Constant;_Float10;Float 10;26;0;Create;True;0;0;0;False;0;False;0.3;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;180;4378.174,-272.675;Float;False;Property;_Color_Eyes;Color_Eyes;13;0;Create;True;0;0;0;False;0;False;0.5283019,0.3862585,0.3862585,1;0.5283019,0.3862584,0.3862584,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;308;719.4268,664.9716;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;292;5181.601,-1224.222;Inherit;False;Constant;_Color3;Color 3;0;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;329;3984.72,1082.034;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;330;4137.478,975.1038;Inherit;False;Constant;_Float11;Float 11;26;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;288;4858.617,-955.7773;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;307;1202.43,542.2068;Inherit;False;Constant;_Float2;Float 2;26;0;Create;True;0;0;0;False;0;False;0.65;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;293;4995.601,-397.2223;Inherit;False;Constant;_Arms_Color;Arms_Color;0;0;Create;True;0;0;0;False;0;False;1,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;294;5428.939,-1198.487;Inherit;False;Constant;_Vector0;Vector 0;24;0;Create;True;0;0;0;False;0;False;1.6,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;270;4594.051,-502.4623;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;189;4922.368,234.4379;Inherit;False;Constant;_Color1;Color 1;22;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;261;3714.61,-784.3649;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;309;984.6717,669.1379;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;338;5357.888,758.401;Inherit;False;Constant;_Float15;Float 15;26;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;185;5166.992,238.7455;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;291;5491.601,-868.2223;Inherit;True;Replace Color;-1;;60;896dccb3016c847439def376a728b869;1,12,0;5;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;265;4519.051,-771.4623;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;331;4272.187,1084.812;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;289;5087.147,-871.3649;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;310;1271.498,666.3087;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;184;5116,627.8245;Float;False;Property;_Emission;Emission;14;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;290;5375.601,-513.2223;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;339;5597.888,562.401;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;313;4829.025,709.2031;Inherit;True;Property;_Smoothness;Smoothness;25;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;312;4712.064,450.8484;Inherit;True;Property;_Metallic;Metallic;24;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;183;5559.291,408.1251;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;5767.304,-306.0844;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;HNGamers/PolyTopeCustomCharacter;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;236;21;222;1
WireConnection;340;0;37;0
WireConnection;237;0;37;0
WireConnection;237;1;236;0
WireConnection;237;2;33;0
WireConnection;235;21;222;2
WireConnection;295;21;222;3
WireConnection;251;0;340;0
WireConnection;251;1;237;0
WireConnection;251;2;236;0
WireConnection;238;0;37;0
WireConnection;238;1;235;0
WireConnection;238;2;40;0
WireConnection;297;0;37;0
WireConnection;297;1;295;0
WireConnection;297;2;296;0
WireConnection;234;21;220;1
WireConnection;252;0;251;0
WireConnection;252;1;238;0
WireConnection;252;2;235;0
WireConnection;233;21;220;2
WireConnection;298;0;252;0
WireConnection;298;1;297;0
WireConnection;298;2;295;0
WireConnection;239;0;37;0
WireConnection;239;1;234;0
WireConnection;239;2;44;0
WireConnection;321;1;320;0
WireConnection;321;2;234;0
WireConnection;253;0;298;0
WireConnection;253;1;239;0
WireConnection;253;2;234;0
WireConnection;302;21;220;3
WireConnection;240;0;37;0
WireConnection;240;1;233;0
WireConnection;240;2;63;0
WireConnection;300;0;37;0
WireConnection;300;1;302;0
WireConnection;300;2;301;0
WireConnection;231;21;217;1
WireConnection;323;0;321;0
WireConnection;323;1;322;0
WireConnection;323;2;233;0
WireConnection;254;0;253;0
WireConnection;254;1;240;0
WireConnection;254;2;233;0
WireConnection;241;0;37;0
WireConnection;241;1;231;0
WireConnection;241;2;125;0
WireConnection;299;0;254;0
WireConnection;299;1;300;0
WireConnection;299;2;302;0
WireConnection;232;21;217;2
WireConnection;325;0;323;0
WireConnection;325;1;324;0
WireConnection;325;2;302;0
WireConnection;230;21;217;3
WireConnection;333;0;325;0
WireConnection;333;1;335;0
WireConnection;333;2;231;0
WireConnection;242;0;37;0
WireConnection;242;1;232;0
WireConnection;242;2;139;0
WireConnection;255;0;299;0
WireConnection;255;1;241;0
WireConnection;255;2;231;0
WireConnection;337;0;333;0
WireConnection;337;1;332;0
WireConnection;337;2;232;0
WireConnection;212;21;187;2
WireConnection;256;0;255;0
WireConnection;256;1;242;0
WireConnection;256;2;232;0
WireConnection;243;0;37;0
WireConnection;243;1;230;0
WireConnection;243;2;147;0
WireConnection;334;0;337;0
WireConnection;334;1;336;0
WireConnection;334;2;230;0
WireConnection;200;21;187;1
WireConnection;257;0;256;0
WireConnection;257;1;243;0
WireConnection;257;2;230;0
WireConnection;244;0;37;0
WireConnection;244;1;212;0
WireConnection;244;2;48;0
WireConnection;315;0;334;0
WireConnection;315;1;314;0
WireConnection;315;2;212;0
WireConnection;259;0;37;0
WireConnection;259;1;200;0
WireConnection;259;2;52;0
WireConnection;258;0;257;0
WireConnection;258;1;244;0
WireConnection;258;2;212;0
WireConnection;214;21;215;2
WireConnection;274;21;215;3
WireConnection;246;0;37;0
WireConnection;246;1;214;0
WireConnection;246;2;198;0
WireConnection;317;0;315;0
WireConnection;317;1;316;0
WireConnection;317;2;200;0
WireConnection;201;21;215;1
WireConnection;260;0;258;0
WireConnection;260;1;259;0
WireConnection;260;2;200;0
WireConnection;319;0;317;0
WireConnection;319;1;318;0
WireConnection;319;2;201;0
WireConnection;262;0;260;0
WireConnection;262;1;246;0
WireConnection;262;2;214;0
WireConnection;275;0;37;0
WireConnection;275;1;274;0
WireConnection;275;2;60;0
WireConnection;276;0;262;0
WireConnection;276;1;275;0
WireConnection;276;2;274;0
WireConnection;327;0;319;0
WireConnection;327;1;326;0
WireConnection;327;2;214;0
WireConnection;245;0;37;0
WireConnection;245;1;201;0
WireConnection;245;2;56;0
WireConnection;272;21;304;3
WireConnection;308;1;305;0
WireConnection;308;2;231;0
WireConnection;329;0;327;0
WireConnection;329;1;328;0
WireConnection;329;2;274;0
WireConnection;288;0;287;4
WireConnection;270;0;37;0
WireConnection;270;1;272;0
WireConnection;270;2;180;0
WireConnection;261;0;276;0
WireConnection;261;1;245;0
WireConnection;261;2;201;0
WireConnection;309;0;308;0
WireConnection;309;1;306;0
WireConnection;309;2;232;0
WireConnection;185;0;189;1
WireConnection;291;1;288;0
WireConnection;291;2;292;0
WireConnection;291;3;293;0
WireConnection;291;4;294;1
WireConnection;291;5;294;2
WireConnection;265;0;261;0
WireConnection;265;1;270;0
WireConnection;265;2;272;0
WireConnection;331;0;329;0
WireConnection;331;1;330;0
WireConnection;331;2;272;0
WireConnection;289;0;288;0
WireConnection;310;0;309;0
WireConnection;310;1;307;0
WireConnection;310;2;230;0
WireConnection;290;0;265;0
WireConnection;290;1;291;0
WireConnection;290;2;289;0
WireConnection;339;0;338;0
WireConnection;313;1;331;0
WireConnection;312;1;310;0
WireConnection;183;0;185;0
WireConnection;183;1;184;0
WireConnection;0;0;290;0
WireConnection;0;2;183;0
WireConnection;0;3;312;0
WireConnection;0;4;313;0
WireConnection;0;5;339;0
ASEEND*/
//CHKSM=3356512B8B92B6589B56B8E64EED45B2168BAC1A