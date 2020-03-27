Shader "Custom/BgVfx" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		_PulseMap ("Gradient Pulse Map (red channel)", 2D) = "white" {}
		_BackgroundColor ("Background Color", Color) = (1,1,1,1)
		_PulseColor ("Pulse Color", Color) = (1,1,1,1)
		_Speed ("Pulse Speed", Float) = 10
		_PulseWidth ("Pulse Width", Float) = 5


		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0

		// Add values to determine if outlining is enabled and outline color.
		[PerRendererData] _Outline("Outline", Float) = 0
		[PerRendererData] _OutlineColor("Outline Color", Color) = (1,1,1,1)
		[PerRendererData] _OutlineSize("Outline Size", int) = 1
	}

	SubShader {
		Tags { 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass {
		CGPROGRAM
			#pragma vertex SpriteVert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnitySprites.cginc"

			float _Outline;
			fixed4 _OutlineColor;
			int _OutlineSize;
			float4 _MainTex_TexelSize;

			// sampler2D _MainTex;
			sampler2D _PulseMap;
			half4 _BackgroundColor;
			half4 _PulseColor;
			float _Speed;
			float _PulseWidth;


			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;

				// // If outline is enabled and there is a pixel, try to draw an outline.
				// if (_Outline > 0 && c.a != 0) {
				// 	float totalAlpha = 1.0;

				// 	[unroll(16)]
				// 	for (int i = 1; i < _OutlineSize + 1; i++) {
				// 		fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, i * _MainTex_TexelSize.y));
				// 		fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0,i *  _MainTex_TexelSize.y));
				// 		fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(i * _MainTex_TexelSize.x, 0));
				// 		fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(i * _MainTex_TexelSize.x, 0));

				// 		totalAlpha = totalAlpha * pixelUp.a * pixelDown.a * pixelRight.a * pixelLeft.a;
				// 	}

				// 	if (totalAlpha == 0) {
				// 		c.rgba = fixed4(1, 1, 1, 1) * _OutlineColor;
				// 	}
				// }

				// c.rgb *= c.a;

				half4 bg = tex2D (_MainTex, IN.texcoord);
				half4 pulseMap = tex2D (_PulseMap, IN.texcoord);

				float modTime = fmod(_Time.x * _Speed, 1);
				float diff = abs(pulseMap.r - modTime);
				float bonusBrightness = _PulseWidth / diff;

				float3 isOverThreshold = step(bg.rgb, 0.05); //Threshold out black background
				isOverThreshold = 1 - isOverThreshold;
				float3 bgFinalColor = (bg.rgb * _BackgroundColor); // idk why there's a constant correction factor here
				float3 pulseFinalColor = (isOverThreshold * bg.rbg * bonusBrightness * pulseMap.a * _PulseColor);
				c.rgb = bgFinalColor + pulseFinalColor;
				// c.a = 1;

				// c.r = modTime * pulseMap.a;

				return c;
			}
		ENDCG
		}
	}
}


// Shader "Custom/BgVfx" {
//     Properties {
//         // _MainTex ("Main Tex", 2D) = "white" {}
//         _PulseMap ("Gradient Pulse Map (red channel)", 2D) = "white" {}
// 		_BackgroundColor ("Background Color", Color) = (1,1,1,1)
// 		_PulseColor ("Pulse Color", Color) = (1,1,1,1)
//         _Speed ("Pulse Speed", Float) = 10
//         _PulseWidth ("Pulse Width", Float) = 5
//         _BrightnessScale ("Brightness Scale", Float) = 10
//     }
//     SubShader {
//         Tags { "RenderType"="Opaque" }
//         LOD 200

//          CGPROGRAM
//          #pragma surface surf Lambert
// 		sampler2D _MainTex;
// 		sampler2D _PulseMap;
// 		half4 _BackgroundColor;
// 		half4 _PulseColor;
// 		float _Speed;
// 		float _PulseWidth;
// 		float _BrightnessScale;
// 		struct Input {
// 			float2 uv_MainTex;
//          };
//          void surf (Input IN, inout SurfaceOutput o) {
//             half4 bg = tex2D (_MainTex, IN.uv_MainTex);
//             half4 pulseMap = tex2D (_PulseMap, IN.uv_MainTex);

// 			float modTime = fmod(_Time.x * _Speed, 1);
// 			float diff = abs(pulseMap.r - modTime);
// 			float bonusBrightness = _PulseWidth / diff;

// 			float3 isOverThreshold = step(bg.rgb, 0.05); //Threshold out black background
// 			isOverThreshold = 1 - isOverThreshold;
// 			float3 bgFinalColor = (bg.rgb * _BackgroundColor * _BrightnessScale); // idk why there's a constant correction factor here
// 			float3 pulseFinalColor = (isOverThreshold * bg.rbg * bonusBrightness * pulseMap.a * _PulseColor);
// 			o.Albedo = bgFinalColor + pulseFinalColor;

// 			o.Albedo.r = 10 * modTime;
//             o.Alpha = 1;
//          }
//          ENDCG
//      } 
//      FallBack "Diffuse"


// }