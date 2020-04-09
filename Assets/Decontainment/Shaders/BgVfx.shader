Shader "Custom/BgVfx" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		_PulseMap ("Gradient Pulse Map (red channel)", 2D) = "white" {}
		_MaxMap ("Max Map (red channel)", 2D) = "white" {}
		_BackgroundColor ("Background Color", Color) = (1,1,1,1)
		_PulseColor ("Pulse Color", Color) = (1,1,1,1)
		_Speed ("Pulse Speed", Float) = 10
		_Offset ("Pulse Offest", Float) = 0
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
			sampler2D _MaxMap;
			half4 _BackgroundColor;
			half4 _PulseColor;
			float _Speed;
			float _Offset;
			float _PulseWidth;


			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 bg = SampleSpriteTexture(IN.texcoord);
				half4 pulseMap = tex2D(_PulseMap, IN.texcoord);
				half4 maxMap = tex2D(_MaxMap, IN.texcoord);

				float modTime = fmod(_Time.x * _Speed + _Offset, maxMap.r);
				float normDiff = abs(pulseMap.r - modTime) / maxMap.r;
				normDiff = normDiff > .5 ? 1 - normDiff : normDiff;
				float bonusBrightness = _PulseWidth / normDiff;

				bool isOverThreshold = step(_BackgroundColor, bg.rgb); //Threshold out black background
				float3 pulseFinalColor = isOverThreshold * bg.rgb * bonusBrightness * pulseMap.a * _PulseColor;
				fixed4 finalColor = fixed4(bg * IN.color + pulseFinalColor, bg.a);

				return finalColor;
			}
		ENDCG
		}
	}
}