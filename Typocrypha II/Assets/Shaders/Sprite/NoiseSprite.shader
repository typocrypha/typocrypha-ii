Shader "Custom/NoiseSprite"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] _Animated("Animated?", Float) = 0
        _AnimationSpeed("Animation Speed",float) = 1
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
        _NoiseScale("Noise Scale",float) = 1
        _NoiseStrength("Noise Strength", Range(0.0, 1.0)) = 0.5
        _ROffset("R Offset",float) = 1
        _GOffset("G Offset",float) = 1
        _BOffset("B Offset",float) = 1
        _Width("Width",float) = 1
        _Height("Height",float) = 1
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

                float _Width;
                float _Height;
                float _NoiseScale;
                float _NoiseStrength;
                float _ROffset;
                float _GOffset;
                float _BOffset;
                float _Animated;
                float _AnimationSpeed;
                //
                // Description : Array and textureless GLSL 2D simplex noise function.
                //      Author : Ian McEwan, Ashima Arts.
                //  Maintainer : stegu
                //     Lastmod : 20110822 (ijm)
                //     License : Copyright (C) 2011 Ashima Arts. All rights reserved.
                //               Distributed under the MIT License. See LICENSE file.
                //               https://github.com/ashima/webgl-noise
                //               https://github.com/stegu/webgl-noise
                // 

                float3 mod289(float3 x) {
                   return x - floor(x * (1.0 / 289.0)) * 289.0;
                }

                float2 mod289(float2 x) {
                    return x - floor(x * (1.0 / 289.0)) * 289.0;
                }

                float3 permute(float3 x) {
                    return mod289(((x * 34.0) + 10.0) * x);
                }

                float snoise(float2 v)
                {
                    const float4 C = float4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
                        0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
                        -0.577350269189626,  // -1.0 + 2.0 * C.x
                        0.024390243902439); // 1.0 / 41.0
                     // First corner
                    float2 i = floor(v + dot(v, C.yy));
                    float2 x0 = v - i + dot(i, C.xx);

                    // Other corners
                    float2 i1;
                    //i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
                    //i1.y = 1.0 - i1.x;
                    i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
                    // x0 = x0 - 0.0 + 0.0 * C.xx ;
                    // x1 = x0 - i1 + 1.0 * C.xx ;
                    // x2 = x0 - 1.0 + 2.0 * C.xx ;
                    float4 x12 = x0.xyxy + C.xxzz;
                    x12.xy -= i1;

                    // Permutations
                    i = mod289(i); // Avoid truncation effects in permutation
                    float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0))
                        + i.x + float3(0.0, i1.x, 1.0));

                    float3 m = max(0.5 - float3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
                    m = m * m;
                    m = m * m;

                    // Gradients: 41 points uniformly over a line, mapped onto a diamond.
                    // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

                    float3 x = 2.0 * frac(p * C.www) - 1.0;
                    float3 h = abs(x) - 0.5;
                    float3 ox = floor(x + 0.5);
                    float3 a0 = x - ox;

                    // Normalise gradients implicitly by scaling m
                    // Approximation of: m *= inversesqrt( a0*a0 + h*h );
                    m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);

                    // Compute final noise value at P
                    float3 g;
                    g.x = a0.x * x0.x + h.x * x0.y;
                    g.yz = a0.yz * x12.xz + h.yz * x12.yw;
                    return 130.0 * dot(m, g);
                }
                v2f vert(appdata_t IN) {
                    return SpriteVert(IN);
                }

                fixed4 frag(v2f IN) : SV_Target{

                    float2 screenPos = ComputeScreenPos(IN.vertex);
                    float2 noisePos = _NoiseScale * screenPos;
                    float sn = snoise(noisePos) + 1;

                    float2 Rs = noisePos + float2(_ROffset, _ROffset);
                    float2 Gs = noisePos + float2(_GOffset, _GOffset);
                    float2 Bs = noisePos + float2(_BOffset, _BOffset);

                    float2 animOffset = round(sin(_Time.y * _AnimationSpeed * sn) * 0.5 + 0.5); 

                    float R = snoise(lerp(Rs, Rs * animOffset, _Animated)); // snoise(lerp(Rs, Rs + animOffsetR, _Animated));
                    float G = snoise(lerp(Gs, Gs * animOffset, _Animated)); // snoise(lerp(Gs, Gs + animOffsetG, _Animated));
                    float B = snoise(lerp(Bs, Bs * animOffset, _Animated)); // snoise(lerp(Bs, Bs - animOffsetB, _Animated));

                    fixed4 col = SpriteFrag(IN);

                    return lerp(col, fixed4(R, G, B, col.a), _NoiseStrength);
                }
            ENDCG
            }
        }
}
