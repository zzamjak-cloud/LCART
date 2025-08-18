Shader "CAT/Effects/ChristmasLights" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1, 0, 0, 1)
        _Color2 ("Color 2", Color) = (0, 1, 0, 1)
        _Color3 ("Color 3", Color) = (0, 0, 1, 1)
        _Color4 ("Color 4", Color) = (1, 1, 0, 1)
        _Color5 ("Color 5", Color) = (1, 0, 1, 1)
        _Speed ("Color Change Speed", Range(0.1, 5.0)) = 1.0
        _Brightness ("Brightness", Range(0.1, 3.0)) = 1.5
    }
    
    SubShader {
        Tags { 
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float4 _Color4;
            float4 _Color5;
            float _Speed;
            float _Brightness;
            float _PatternType;
            float _PatternSpeed;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            // ���� ���� �Լ�
            float random(float2 st) {
                return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
            }
            
            // 0���� 1 ������ ���� �ֱ����� �����ķ� ��ȯ
            float sinTime(float speed, float offset) {
                return (sin(_Time.y * speed + offset) * 0.5 + 0.5);
            }
            
            // ������ ���� (0 �Ǵ� 1)
            float blinkPattern(float speed, float offset, float threshold) {
                float t = frac(_Time.y * speed + offset);
                return step(threshold, t);
            }
            
            // ���ŵ� ���� �Լ���
            
            float4 getColorByPattern(float r, float g, float b, float2 uv) {
                float time = _Time.y * _Speed;
                
                // 5���� ������ ����
                float3 color1 = _Color1.rgb;
                float3 color2 = _Color2.rgb;
                float3 color3 = _Color3.rgb;
                float3 color4 = _Color4.rgb;
                float3 color5 = _Color5.rgb;
                
                // ���� ��ȯ �ֱ� (0-5-10-...)
                float cycle = time * 0.5; // ��ȯ �ӵ� ��ȭ
                
                // �� ä�κ� ���� ����
                float3 rColor, gColor, bColor;
                
                // ���� ��ȯ ���� (5���� ���� ��ȯ)
                int colorIndex = int(fmod(cycle, 5.0));
                
                // R ä�� ���� ����
                if (colorIndex == 0) rColor = color1;
                else if (colorIndex == 1) rColor = color2;
                else if (colorIndex == 2) rColor = color3;
                else if (colorIndex == 3) rColor = color4;
                else rColor = color5;
                
                // G ä�� ���� ���� (R���� 1ĭ ��)
                colorIndex = int(fmod(cycle + 1.0, 5.0));
                if (colorIndex == 0) gColor = color1;
                else if (colorIndex == 1) gColor = color2;
                else if (colorIndex == 2) gColor = color3;
                else if (colorIndex == 3) gColor = color4;
                else gColor = color5;
                
                // B ä�� ���� ���� (G���� 1ĭ ��)
                colorIndex = int(fmod(cycle + 2.0, 5.0));
                if (colorIndex == 0) bColor = color1;
                else if (colorIndex == 1) bColor = color2;
                else if (colorIndex == 2) bColor = color3;
                else if (colorIndex == 3) bColor = color4;
                else bColor = color5;
                
                // ä�κ� ���� ����
                float rIntensity = r * _Brightness;
                float gIntensity = g * _Brightness;
                float bIntensity = b * _Brightness;
                
                // ���� ���� �ռ�
                float3 finalColor = rColor * rIntensity + gColor * gIntensity + bColor * bIntensity;
                
                return float4(finalColor, max(max(r, g), b));
            }
            
            fixed4 frag (v2f i) : SV_Target {
                // �ؽ�ó���� �� ä�� ���� ��������
                fixed4 col = tex2D(_MainTex, i.uv);
                float r = col.r;
                float g = col.g;
                float b = col.b;
                float a = col.a;
                
                // ä�κ� ���� ���� ����
                fixed4 finalColor = getColorByPattern(r, g, b, i.uv);
                
                // ���� ä�� ����
                finalColor.a *= a;
                
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}