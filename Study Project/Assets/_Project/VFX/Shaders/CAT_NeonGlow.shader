Shader "CAT/Effects/NeonGlow"
{
    Properties
    {
        _MainTex ("Texture (RGBA)", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _InnerGlowColor ("Inner Glow Color", Color) = (0,1,0,1)
        _OuterGlowColor ("Outer Glow Color", Color) = (0,0,1,1)
        _InnerGlowIntensity ("Inner Glow Intensity", Range(0,3)) = 1.0
        _OuterGlowIntensity ("Outer Glow Intensity", Range(0,3)) = 1.0
        _EmissionIntensity ("Emission Intensity", Range(0,5)) = 1.0
        
        [Space(10)]
        [Header(Flicker Effect)]
        [Toggle] _UseFlicker ("Use Flicker Effect", Float) = 1
        _NoiseTexture ("Noise Texture (R)", 2D) = "white" {}
        _FlickerSpeed ("Flicker Speed", Range(0.1, 10)) = 2.0
        _InnerFlickerMin ("Inner Flicker Min", Range(0, 1)) = 0.7
        _InnerFlickerMax ("Inner Flicker Max", Range(1, 3)) = 1.3
        _OuterFlickerMin ("Outer Flicker Min", Range(0, 1)) = 0.8
        _OuterFlickerMax ("Outer Flicker Max", Range(1, 3)) = 1.2
        _NoiseScaleInner ("Inner Noise Scale", Range(0.1, 5)) = 1.0
        _NoiseScaleOuter ("Outer Noise Scale", Range(0.1, 5)) = 0.7
        _NoiseOffsetOuter ("Outer Noise Offset", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100
        Cull Off
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // ����� ����ȭ�� ���� �����׸�
            #pragma target 3.0
            #pragma fragmentoption ARB_precision_hint_fastest
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainColor;
            float4 _InnerGlowColor;
            float4 _OuterGlowColor;
            float _InnerGlowIntensity;
            float _OuterGlowIntensity;
            float _EmissionIntensity;
            
            // ������ ȿ���� ���� ������Ƽ
            float _UseFlicker;
            sampler2D _NoiseTexture;
            float4 _NoiseTexture_ST;
            float _FlickerSpeed;
            float _InnerFlickerMin;
            float _InnerFlickerMax;
            float _OuterFlickerMin;
            float _OuterFlickerMax;
            float _NoiseScaleInner;
            float _NoiseScaleOuter;
            float _NoiseOffsetOuter;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }
            
            half4 frag (v2f i) : SV_Target
            {
                // �ؽ�ó�� �� ä���� ���������� ���ø�
                half4 tex = tex2D(_MainTex, i.uv);
                
                // R: ���� �÷� (������ �⺻ ����)
                half mainMask = tex.r;
                
                // G: InnerGlow �̹���
                half innerGlowMask = tex.g;
                
                // B: OuterGlow �̹���
                half outerGlowMask = tex.b;
                
                // A: ���� ä��
                half alpha = tex.a;
                
                // ������ ȿ�� ���
                half innerIntensity = _InnerGlowIntensity;
                half outerIntensity = _OuterGlowIntensity;
                
                if (_UseFlicker > 0.5) {
                    // ������ �ؽ�ó�� �̿��� InnerGlow ������ ���
                    float2 innerNoiseUV = i.uv * _NoiseScaleInner + float2(_Time.y * _FlickerSpeed * 0.05, _Time.y * _FlickerSpeed * 0.07);
                    float innerNoise = tex2D(_NoiseTexture, innerNoiseUV).r;
                    
                    // InnerGlow ���� ���� ����
                    innerIntensity *= lerp(_InnerFlickerMin, _InnerFlickerMax, innerNoise);
                    
                    // ������ �ؽ�ó�� �̿��� OuterGlow ������ ��� (�ణ �ٸ� �����°� ������)
                    float2 outerNoiseUV = i.uv * _NoiseScaleOuter + float2(_Time.y * _FlickerSpeed * 0.09 + _NoiseOffsetOuter, _Time.y * _FlickerSpeed * 0.03 - _NoiseOffsetOuter);
                    float outerNoise = tex2D(_NoiseTexture, outerNoiseUV).r;
                    
                    // OuterGlow ���� ���� ����
                    outerIntensity *= lerp(_OuterFlickerMin, _OuterFlickerMax, outerNoise);
                }
                
                // �� ���̾ �÷��� ������ ���� ���
                half3 mainColor = mainMask * _MainColor.rgb * _EmissionIntensity;
                half3 innerGlow = innerGlowMask * _InnerGlowColor.rgb * innerIntensity;
                half3 outerGlow = outerGlowMask * _OuterGlowColor.rgb * outerIntensity;
                
                // ���� ���� ���� (additive blending)
                half3 finalColor = mainColor + innerGlow + outerGlow;
                
                // ���ؽ� �÷��� �����Ͽ� �ν��Ͻ��� �÷� ��ȭ ����
                finalColor *= i.color.rgb;
                
                // ���� ���İ� ��� (�ؽ�ó ���� * ���� �÷� ���� * ���ؽ� ����)
                half finalAlpha = alpha * _MainColor.a * i.color.a;
                
                return half4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}