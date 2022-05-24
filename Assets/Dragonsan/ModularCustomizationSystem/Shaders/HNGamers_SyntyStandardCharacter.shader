// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "HNGamers_SyntyStandardCharacter"
{
	Properties
	{
		_leatherMask("leatherMask", 2D) = "white" {}
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
		_Color_Medical("Color_Medical", Color) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _OriginalTexture;
		uniform float4 _OriginalTexture_ST;
		uniform float4 _Color_Primary;
		uniform sampler2D _primaryMask;
		SamplerState sampler_primaryMask;
		uniform float4 _primaryMask_ST;
		uniform float4 _Color_Secondary;
		uniform float4 _Color_Tertiary;
		uniform float4 _Color_Leather_Primary;
		uniform sampler2D _leatherMask;
		SamplerState sampler_leatherMask;
		uniform float4 _leatherMask_ST;
		uniform float4 _Color_Leather_Secondary;
		uniform float4 _Color_Leather_Tertiary;
		uniform float4 _Color_Metal_Primary;
		uniform sampler2D _metalmask;
		SamplerState sampler_metalmask;
		uniform float4 _metalmask_ST;
		uniform float4 _Color_Metal_Secondary;
		uniform float4 _Color_Metal_Dark;
		uniform float4 _Color_Skin;
		uniform sampler2D _skinhairMask;
		SamplerState sampler_skinhairMask;
		uniform float4 _skinhairMask_ST;
		uniform float4 _Color_Hair;
		uniform float4 _Color_Stubble;
		uniform sampler2D _colorMasks;
		uniform float4 _colorMasks_ST;
		uniform float4 _Color_Medical;
		uniform sampler2D _TextureSample0;
		SamplerState sampler_TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float4 _Color2;
		uniform float4 _Color1;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_OriginalTexture = i.uv_texcoord * _OriginalTexture_ST.xy + _OriginalTexture_ST.zw;
			float4 tex2DNode7 = tex2D( _OriginalTexture, uv_OriginalTexture );
			float decodeFloatRGBA11 = DecodeFloatRGBA( tex2DNode7 );
			float2 uv_primaryMask = i.uv_texcoord * _primaryMask_ST.xy + _primaryMask_ST.zw;
			float4 tex2DNode3 = tex2D( _primaryMask, uv_primaryMask );
			float temp_output_25_0_g2 = 0.0;
			float temp_output_22_0_g2 = step( tex2DNode3.r , temp_output_25_0_g2 );
			float temp_output_17_0 = ( temp_output_22_0_g2 - 0.0 );
			float4 lerpResult18 = lerp( float4( 0,0,0,0 ) , ( decodeFloatRGBA11 * _Color_Primary * temp_output_17_0 ) , temp_output_17_0);
			float temp_output_25_0_g3 = 0.0;
			float temp_output_22_0_g3 = step( tex2DNode3.b , temp_output_25_0_g3 );
			float temp_output_19_0 = ( temp_output_22_0_g3 - 0.0 );
			float4 lerpResult22 = lerp( lerpResult18 , ( decodeFloatRGBA11 * _Color_Secondary * temp_output_19_0 ) , temp_output_19_0);
			float temp_output_25_0_g4 = 0.0;
			float temp_output_22_0_g4 = step( tex2DNode3.g , temp_output_25_0_g4 );
			float temp_output_26_0 = ( temp_output_22_0_g4 - 0.0 );
			float4 lerpResult23 = lerp( lerpResult22 , ( decodeFloatRGBA11 * _Color_Tertiary * temp_output_26_0 ) , temp_output_26_0);
			float2 uv_leatherMask = i.uv_texcoord * _leatherMask_ST.xy + _leatherMask_ST.zw;
			float4 tex2DNode1 = tex2D( _leatherMask, uv_leatherMask );
			float temp_output_25_0_g5 = 0.0;
			float temp_output_22_0_g5 = step( tex2DNode1.r , temp_output_25_0_g5 );
			float temp_output_30_0 = ( temp_output_22_0_g5 - 0.0 );
			float4 lerpResult27 = lerp( lerpResult23 , ( decodeFloatRGBA11 * _Color_Leather_Primary * temp_output_30_0 ) , temp_output_30_0);
			float temp_output_25_0_g6 = 0.0;
			float temp_output_22_0_g6 = step( tex2DNode1.g , temp_output_25_0_g6 );
			float temp_output_34_0 = ( temp_output_22_0_g6 - 0.0 );
			float4 lerpResult31 = lerp( lerpResult27 , ( decodeFloatRGBA11 * _Color_Leather_Secondary * temp_output_34_0 ) , temp_output_34_0);
			float temp_output_25_0_g7 = 0.0;
			float temp_output_22_0_g7 = step( tex2DNode1.b , temp_output_25_0_g7 );
			float temp_output_38_0 = ( temp_output_22_0_g7 - 0.0 );
			float4 lerpResult35 = lerp( lerpResult31 , ( decodeFloatRGBA11 * _Color_Leather_Tertiary * temp_output_38_0 ) , temp_output_38_0);
			float2 uv_metalmask = i.uv_texcoord * _metalmask_ST.xy + _metalmask_ST.zw;
			float4 tex2DNode2 = tex2D( _metalmask, uv_metalmask );
			float temp_output_25_0_g8 = 0.0;
			float temp_output_22_0_g8 = step( tex2DNode2.r , temp_output_25_0_g8 );
			float temp_output_42_0 = ( temp_output_22_0_g8 - 0.0 );
			float4 lerpResult39 = lerp( lerpResult35 , ( decodeFloatRGBA11 * _Color_Metal_Primary * temp_output_42_0 ) , temp_output_42_0);
			float temp_output_25_0_g9 = 0.0;
			float temp_output_22_0_g9 = step( tex2DNode2.g , temp_output_25_0_g9 );
			float temp_output_46_0 = ( temp_output_22_0_g9 - 0.0 );
			float4 lerpResult43 = lerp( lerpResult39 , ( decodeFloatRGBA11 * _Color_Metal_Secondary * temp_output_46_0 ) , temp_output_46_0);
			float temp_output_25_0_g10 = 0.0;
			float temp_output_22_0_g10 = step( tex2DNode2.b , temp_output_25_0_g10 );
			float temp_output_50_0 = ( temp_output_22_0_g10 - 0.0 );
			float4 lerpResult47 = lerp( lerpResult43 , ( decodeFloatRGBA11 * _Color_Metal_Dark * temp_output_50_0 ) , temp_output_50_0);
			float2 uv_skinhairMask = i.uv_texcoord * _skinhairMask_ST.xy + _skinhairMask_ST.zw;
			float4 tex2DNode5 = tex2D( _skinhairMask, uv_skinhairMask );
			float temp_output_25_0_g23 = 0.0;
			float temp_output_22_0_g23 = step( tex2DNode5.r , temp_output_25_0_g23 );
			float temp_output_54_0 = ( temp_output_22_0_g23 - 0.0 );
			float4 lerpResult51 = lerp( lerpResult47 , ( decodeFloatRGBA11 * _Color_Skin * temp_output_54_0 ) , temp_output_54_0);
			float temp_output_25_0_g24 = 0.0;
			float temp_output_22_0_g24 = step( tex2DNode5.g , temp_output_25_0_g24 );
			float temp_output_58_0 = ( temp_output_22_0_g24 - 0.0 );
			float4 lerpResult55 = lerp( lerpResult51 , ( decodeFloatRGBA11 * _Color_Hair * temp_output_58_0 ) , temp_output_58_0);
			float temp_output_25_0_g27 = 0.0;
			float temp_output_22_0_g27 = step( tex2DNode5.b , temp_output_25_0_g27 );
			float temp_output_62_0 = ( temp_output_22_0_g27 - 0.0 );
			float4 lerpResult59 = lerp( lerpResult55 , ( decodeFloatRGBA11 * _Color_Stubble * temp_output_62_0 ) , temp_output_62_0);
			float2 uv_colorMasks = i.uv_texcoord * _colorMasks_ST.xy + _colorMasks_ST.zw;
			float4 lerpResult76 = lerp( tex2DNode7 , lerpResult59 , tex2D( _colorMasks, uv_colorMasks ));
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float4 tex2DNode89 = tex2D( _TextureSample0, uv_TextureSample0 );
			float temp_output_25_0_g22 = 0.0;
			float temp_output_22_0_g22 = step( tex2DNode89.r , temp_output_25_0_g22 );
			float temp_output_77_0 = ( temp_output_22_0_g22 - 0.0 );
			float4 lerpResult79 = lerp( float4( 0,0,0,0 ) , ( decodeFloatRGBA11 * _Color_Medical * temp_output_77_0 ) , temp_output_77_0);
			float temp_output_25_0_g25 = 0.0;
			float temp_output_22_0_g25 = step( tex2DNode89.b , temp_output_25_0_g25 );
			float temp_output_85_0 = ( temp_output_22_0_g25 - 0.0 );
			float4 lerpResult87 = lerp( lerpResult79 , ( decodeFloatRGBA11 * _Color2 * temp_output_85_0 ) , temp_output_85_0);
			float temp_output_25_0_g26 = 0.0;
			float temp_output_22_0_g26 = step( tex2DNode89.g , temp_output_25_0_g26 );
			float temp_output_85_28 = ( temp_output_25_0_g25 + 0.2 );
			float4 lerpResult83 = lerp( lerpResult87 , ( decodeFloatRGBA11 * _Color1 * ( temp_output_22_0_g26 - 0.0 ) ) , temp_output_85_28);
			float4 lerpResult90 = lerp( lerpResult76 , ( lerpResult83 + lerpResult83 + lerpResult83 ) , temp_output_85_28);
			o.Albedo = lerpResult90.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
0;0;2560;1579;410.3672;2248.263;1.153707;True;True
Node;AmplifyShaderEditor.SamplerNode;7;-2476.411,-180.0257;Inherit;True;Property;_OriginalTexture;OriginalTexture;5;0;Create;True;0;0;False;0;False;-1;3391e5a0d7f698a4b97dea8e8b5e894c;3391e5a0d7f698a4b97dea8e8b5e894c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-1004.295,855.2948;Inherit;True;Property;_primaryMask;primaryMask;2;0;Create;True;0;0;False;0;False;-1;7812ab1b62b0b0946b745c8f858113e5;7812ab1b62b0b0946b745c8f858113e5;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;17;-1537.812,418.0414;Inherit;True;MaskingFunction;-1;;2;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.DecodeFloatRGBAHlpNode;11;1173.725,-607.6219;Inherit;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;13;-1561.718,195.4911;Inherit;False;Property;_Color_Primary;Color_Primary;17;0;Create;True;0;0;False;0;False;0,0.1698678,1,0;0.6078432,0.6078432,0.6078432,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;19;-1132.836,431.679;Inherit;True;MaskingFunction;-1;;3;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1336.812,156.0414;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;20;-1120,240;Inherit;False;Property;_Color_Secondary;Color_Secondary;14;0;Create;True;0;0;False;0;False;0.8983961,1,0.0990566,0;0.6078432,0.6078432,0.6078432,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;24;-702.3935,237.952;Inherit;False;Property;_Color_Tertiary;Color_Tertiary;15;0;Create;True;0;0;False;0;False;0.9339623,0,0,0;0.6078432,0.6078432,0.6078432,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;26;-658.2296,423.6309;Inherit;True;MaskingFunction;-1;;4;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SamplerNode;1;90.32063,780.6279;Inherit;True;Property;_leatherMask;leatherMask;0;0;Create;True;0;0;False;0;False;-1;72277d89d83be214da4f37fe6fb8bf7b;72277d89d83be214da4f37fe6fb8bf7b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-899.8356,200.6789;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;18;-1363.812,-80.95856;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-475.2291,198.6308;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;30;-183.4074,428.2433;Inherit;True;MaskingFunction;-1;;5;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;29;-205.5713,252.5644;Inherit;False;Property;_Color_Leather_Primary;Color_Leather_Primary;21;0;Create;True;0;0;False;0;False;0.6037736,0.2933429,0.2933429,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;22;-948,-79;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;23;-546.3935,-66.04803;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;33;274.0823,243.8959;Inherit;False;Property;_Color_Leather_Secondary;Color_Leather_Secondary;13;0;Create;True;0;0;False;0;False;0.764151,0.1622019,0.1622019,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;34;296.2462,419.575;Inherit;True;MaskingFunction;-1;;6;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;14.59301,213.2431;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;494.2467,204.5746;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;27;23.42867,-56.43567;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;38;819.2415,448.4698;Inherit;True;MaskingFunction;-1;;7;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SamplerNode;2;1750.514,884.5279;Inherit;True;Property;_metalmask;metalmask;1;0;Create;True;0;0;False;0;False;-1;9f2a5941ad53e8640a5d39d1835cef71;9f2a5941ad53e8640a5d39d1835cef71;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;37;797.0776,272.7907;Inherit;False;Property;_Color_Leather_Tertiary;Color_Leather_Tertiary;12;0;Create;True;0;0;False;0;False;0.764151,0.1622019,0.1622019,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;42;1410.048,493.0276;Inherit;True;MaskingFunction;-1;;8;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;41;1481.884,284.3486;Inherit;False;Property;_Color_Metal_Primary;Color_Metal_Primary;11;0;Create;True;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;1017.242,233.4694;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;31;482.0822,-44.10426;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;1702.049,245.0275;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;46;2053.049,474.4749;Inherit;True;MaskingFunction;-1;;9;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;35;1005.077,-15.20952;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;45;2030.884,298.796;Inherit;False;Property;_Color_Metal_Secondary;Color_Metal_Secondary;10;0;Create;True;0;0;False;0;False;0.764151,0.1622019,0.1622019,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;3764.175,859.3085;Inherit;True;Property;_skinhairMask;skinhairMask;4;0;Create;True;0;0;False;0;False;-1;c8320301f0bc56a499b33e078298a52f;c8320301f0bc56a499b33e078298a52f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;2251.05,259.4748;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;49;2516.316,310.3539;Inherit;False;Property;_Color_Metal_Dark;Color_Metal_Dark;9;0;Create;True;0;0;False;0;False;0.7924528,0.06354576,0.06354576,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;89;717.1438,-1327.472;Inherit;True;Property;_TextureSample0;Texture Sample 0;20;0;Create;True;0;0;False;0;False;-1;ea89a3aa54bb30e4c82b012a1a158672;ea89a3aa54bb30e4c82b012a1a158672;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;50;2538.481,486.0329;Inherit;True;MaskingFunction;-1;;10;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;39;1689.884,-3.651451;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;2736.482,271.0327;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;77;-90.72515,-1520.931;Inherit;True;MaskingFunction;-1;;22;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;43;2238.884,10.79588;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;52;3208.709,301.8019;Inherit;False;Property;_Color_Skin;Color_Skin;8;0;Create;True;0;0;False;0;False;1,0.8726415,0.8726415,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;54;3230.874,477.4809;Inherit;True;MaskingFunction;-1;;23;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;80;-112.8903,-1696.61;Inherit;False;Property;_Color_Medical;Color_Medical;22;0;Create;True;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;56;3760.708,321.3077;Inherit;False;Property;_Color_Hair;Color_Hair;7;0;Create;True;0;0;False;0;False;1,0.8726415,0.8726415,0;0.6078432,0.6078432,0.6078432,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;88;1318.737,-1800.652;Inherit;False;Property;_Color2;Color 2;19;0;Create;True;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;85;1340.902,-1624.973;Inherit;True;MaskingFunction;-1;;25;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.LerpOp;47;2724.316,22.35382;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;151.5645,-1761.983;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;58;3782.873,496.9867;Inherit;True;MaskingFunction;-1;;24;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;3428.875,262.4807;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;79;95.10992,-1984.61;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;60;4251.75,340.8899;Inherit;False;Property;_Color_Stubble;Color_Stubble;16;0;Create;True;0;0;False;0;False;1,0.8726415,0.8726415,0;0.6078432,0.6078432,0.6078432,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;62;4273.915,516.5689;Inherit;True;MaskingFunction;-1;;27;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.ColorNode;84;528.0123,-1900.533;Inherit;False;Property;_Color1;Color 1;18;0;Create;True;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;1538.903,-1839.973;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;51;3416.709,13.80192;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;81;550.1774,-1724.854;Inherit;True;MaskingFunction;-1;;26;4ce6806502d2cfd4e8b2f6af1aa73ecc;0;3;21;FLOAT;0;False;30;FLOAT;0;False;25;FLOAT;0;False;3;FLOAT;0;FLOAT;32;FLOAT;28
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;3980.873,281.9865;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;4471.916,301.5688;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;748.1785,-1939.854;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;87;721.7371,-2186.652;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;55;3968.707,33.30767;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;74;-1431.828,-555.2808;Inherit;True;Property;_colorMasks;colorMasks;6;0;Create;True;0;0;False;0;False;-1;7a76017a04248954cbb6d454bfa3900e;7a76017a04248954cbb6d454bfa3900e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;59;4459.75,52.88976;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;83;1641.012,-2188.533;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;94;1976.951,-1880.634;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;76;2440.2,-1237.084;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;90;2442.779,-1794.977;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;972.7799,842.138;Inherit;True;Property;_shapeMask;shapeMask;3;0;Create;True;0;0;False;0;False;-1;ea89a3aa54bb30e4c82b012a1a158672;ea89a3aa54bb30e4c82b012a1a158672;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;5387.337,-1058.624;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;HNGamers_SyntyStandardCharacter;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;21;3;1
WireConnection;11;0;7;0
WireConnection;19;21;3;3
WireConnection;15;0;11;0
WireConnection;15;1;13;0
WireConnection;15;2;17;0
WireConnection;26;21;3;2
WireConnection;21;0;11;0
WireConnection;21;1;20;0
WireConnection;21;2;19;0
WireConnection;18;1;15;0
WireConnection;18;2;17;0
WireConnection;25;0;11;0
WireConnection;25;1;24;0
WireConnection;25;2;26;0
WireConnection;30;21;1;1
WireConnection;22;0;18;0
WireConnection;22;1;21;0
WireConnection;22;2;19;0
WireConnection;23;0;22;0
WireConnection;23;1;25;0
WireConnection;23;2;26;0
WireConnection;34;21;1;2
WireConnection;28;0;11;0
WireConnection;28;1;29;0
WireConnection;28;2;30;0
WireConnection;32;0;11;0
WireConnection;32;1;33;0
WireConnection;32;2;34;0
WireConnection;27;0;23;0
WireConnection;27;1;28;0
WireConnection;27;2;30;0
WireConnection;38;21;1;3
WireConnection;42;21;2;1
WireConnection;36;0;11;0
WireConnection;36;1;37;0
WireConnection;36;2;38;0
WireConnection;31;0;27;0
WireConnection;31;1;32;0
WireConnection;31;2;34;0
WireConnection;40;0;11;0
WireConnection;40;1;41;0
WireConnection;40;2;42;0
WireConnection;46;21;2;2
WireConnection;35;0;31;0
WireConnection;35;1;36;0
WireConnection;35;2;38;0
WireConnection;44;0;11;0
WireConnection;44;1;45;0
WireConnection;44;2;46;0
WireConnection;50;21;2;3
WireConnection;39;0;35;0
WireConnection;39;1;40;0
WireConnection;39;2;42;0
WireConnection;48;0;11;0
WireConnection;48;1;49;0
WireConnection;48;2;50;0
WireConnection;77;21;89;1
WireConnection;43;0;39;0
WireConnection;43;1;44;0
WireConnection;43;2;46;0
WireConnection;54;21;5;1
WireConnection;85;21;89;3
WireConnection;47;0;43;0
WireConnection;47;1;48;0
WireConnection;47;2;50;0
WireConnection;78;0;11;0
WireConnection;78;1;80;0
WireConnection;78;2;77;0
WireConnection;58;21;5;2
WireConnection;53;0;11;0
WireConnection;53;1;52;0
WireConnection;53;2;54;0
WireConnection;79;1;78;0
WireConnection;79;2;77;0
WireConnection;62;21;5;3
WireConnection;86;0;11;0
WireConnection;86;1;88;0
WireConnection;86;2;85;0
WireConnection;51;0;47;0
WireConnection;51;1;53;0
WireConnection;51;2;54;0
WireConnection;81;21;89;2
WireConnection;57;0;11;0
WireConnection;57;1;56;0
WireConnection;57;2;58;0
WireConnection;61;0;11;0
WireConnection;61;1;60;0
WireConnection;61;2;62;0
WireConnection;82;0;11;0
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
WireConnection;94;0;83;0
WireConnection;94;1;83;0
WireConnection;94;2;83;0
WireConnection;76;0;7;0
WireConnection;76;1;59;0
WireConnection;76;2;74;0
WireConnection;90;0;76;0
WireConnection;90;1;94;0
WireConnection;90;2;85;28
WireConnection;0;0;90;0
ASEEND*/
//CHKSM=856E26651F2444F96B02F46B6F356885A729AA2C