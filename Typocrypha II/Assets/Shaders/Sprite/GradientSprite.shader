Shader "Custom/GradientSprite"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
        _ColorA("Color 1", Color) = (1, 1, 1, 1)
        _ColorB("Color 2", Color) = (1, 1, 1, 1)
        [Enum(Horizontal,0,Vertical,1)] _Direction("Gradient Direction", Float) = 0
        _OffsetA("Color 1 Offset", Range(0.0, 1.0)) = 0.0
        _OffsetB("Color 2 Offset", Range(0.0, 1.0)) = 1.0
        [IntRange] _Steps("Steps", Range(1, 25)) = 1

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
            Blend SrcAlpha OneMinusSrcAlpha

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
                fixed4 _ColorA;
                float _OffsetA;
                fixed4 _ColorB;
                float _OffsetB;
                float _Direction;
                int _Steps;

                v2f vert(appdata_t IN) {
                    return SpriteVert(IN);
                }

                fixed4 frag(v2f IN) : SV_Target{

                    // get val (u or v value depending on horizontal or vertical)
                    float val = lerp(IN.texcoord.x, 1 - IN.texcoord.y, _Direction); 

                    val = (val - _OffsetA) / (_OffsetB - _OffsetA); // lerp with offset

                    float stepInv = 1.0f / _Steps; // step it
                    float newVal = round(val * _Steps) * stepInv;
                    newVal = lerp(newVal, val, floor(stepInv));

                    fixed4 col = lerp(_ColorA, _ColorB, newVal);
                    return col;
                }
            ENDCG
            }
        }
}
