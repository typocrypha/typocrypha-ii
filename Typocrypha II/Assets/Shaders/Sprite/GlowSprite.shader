Shader "Custom/GlowSprite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Tint",Color)=(1,1,1,1)
	}
	SubShader
	{
		Tags
		{
			"Queue"="Transparent" 
			"IgnoreProjector"="True"
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
		}

		// No culling or depth
		Cull Off 
		ZWrite Off 
		ZTest Always
		Lighting Off
		Blend SrcAlpha One
		BlendOp Add

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f vert (float4 vertex : POSITION, float4 color : COLOR, float2 uv : TEXCOORD0, out float4 outpos : SV_POSITION)
			{
				v2f o;
				o.uv = uv;
				o.color = color;
				outpos = UnityObjectToClipPos(vertex);
				return o;
			}
			
			sampler2D _MainTex;
			fixed4 _Color;

			fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
			{
				// create glitch distortion, which flairs at certain time points
				float bound = floor(screenPos.y - (screenPos.y % 21));
				float xbound = floor(screenPos.x - (screenPos.x % 21));
				bool glitch = _Time.g % 4 < (sin(xbound) *0.5 + 0.5) * 0.15;
				glitch = glitch || _Time.g % 9 < (sin(bound) *0.5 + 0.5) * 0.2;
				if (glitch) {
					float shift = sin(bound)/128;
					if (i.uv.y > .50) shift *= 3;
					i.uv.x += shift/4;
				}
				fixed4 c = tex2D (_MainTex, i.uv);
				c.a *= (1 - 0.1*glitch);
				c.a -= 0.25;
				// apply staticy grain
				//float statc = 0.5 + ((screenPos.y) % 4)/4;
				//c.rgb = c.rgb * statc;
				return c;
			}
			ENDCG
		}
	}
}