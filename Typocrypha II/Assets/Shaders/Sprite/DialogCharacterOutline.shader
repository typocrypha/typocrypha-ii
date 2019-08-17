Shader "Custom/DialogCharacterOutline"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
		_BodyTex("Body", 2D) = "white" {}
		_ClothesTex("Clothes", 2D) = "white" {}
		_HairTex("Hair", 2D) = "white" {}
		_OutlineSize("Outline Size", float) = 0
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_FadeAmount("Fade Amount", float) = 0
		_FadeColor("Fade Color", Color) = (0,0,0,1)
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnitySprites.cginc"

				sampler2D _BodyTex;
				sampler2D _ClothesTex;
				sampler2D _HairTex;
				float _OutlineSize;
				fixed4 _OutlineColor;
				float _FadeAmount;
				fixed4 _FadeColor;

				fixed4 composite(float2 uv) {
					return tex2D(_BodyTex, uv) + tex2D(_ClothesTex, uv) + tex2D(_HairTex, uv);
				}

				v2f vert(appdata_t IN) {
					return SpriteVert(IN);
				}

				fixed4 frag(v2f IN) : SV_Target {
					fixed4 c = tex2D(_MainTex, IN.texcoord);
					float2 x_dist = float2 (_OutlineSize, 0);
					float2 y_dist = float2 (0, _OutlineSize);
					if (composite(IN.texcoord).a < 0.1) {
						if ((composite(IN.texcoord + x_dist).a > 0) ||
							(composite(IN.texcoord - x_dist).a > 0) ||
							(composite(IN.texcoord + y_dist).a > 0) ||
							(composite(IN.texcoord - y_dist).a > 0)) 
							{
								c.rgb = _OutlineColor.rgb * (1 - _FadeAmount) + (_FadeAmount * _FadeColor.rgb);
								c.a = 1;
							}
							
					}
					return c;
				}
			ENDCG
			}
		}
}
