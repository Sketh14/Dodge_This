// Original : https://www.shadertoy.com/view/st3fDf
// Texture Wrap Mode should be bilinear

Shader "Unlit/RainUnlitShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed", Float) = 0.2
        _Intensity ("Intensity", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // #pragma target 5.0
            fixed4 _Color;
            float _Speed;
            float _Intensity;

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.vertex.y = -o.vertex.y;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            //https://discussions.unity.com/t/2d-3d-4d-optimised-perlin-noise-cg-hlsl-library-cginc/523441
            fixed noise(fixed st)
            {
                return frac(sin(dot(st, float2(12.9898, 78.233))) * 43758.5453);
            }

            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 q = i.vertex.xy/_ScreenParams.xy;
                fixed2 p = 2.0 * q - 1.0;
                p.x *= _ScreenParams.x/_ScreenParams.y;
                // p.x /= 2;
                // p.y *= _ScreenParams.y/_ScreenParams.x;
                // p.x = 1 - p.x;
                // p.y = 1 - p.y;
                
                fixed3 col = fixed3(0.0, 0.0, 0.0);
                // fixed3 col = _Color.xyz;

                //Rain                
                fixed timeVal = _Time * _Speed;
                fixed2 st = p * fixed2(0.5, -0.01) + fixed2(timeVal , timeVal);          //For Editor
                // fixed2 st = p * fixed2(0.5, 0.01) + fixed2(timeVal , timeVal);
                // fixed2 st = p * fixed2(0.01, 0.5) + fixed2(timeVal , timeVal);

                // fixed f = floor(fmod(_Time/9.0, 2.0));
                fixed f = 1.0;
                f = tex2D(_MainTex, st).y * tex2D(_MainTex, st * 0.4).x * 1.2;

                // fixed f = noise(st * 200.5) * noise(st * 125.5);
                f = clamp(pow(abs(f), 23.0) * 8.0, 0.0, (1 - q.y) * _Intensity);             // Decide the intensity of the visibility          //For Editor
                // f = clamp(pow(abs(f), 23.0) * 8.0, 0.0, q.y * _Intensity);             // Decide the intensity of the visibility
                // col += f;
                col.x += f; col.y += f; col.z += f;

                fixed4 finalCol = fixed4(col, 1.0);
                // fixed4 finalCol = tex2D(_MainTex, i.uv);
                // fixed4 finalCol = _Color;
                return finalCol;
            }
            /**/

            /*
            float4 frag (v2f i) : SV_Target
            {
                float2 q = i.vertex.xy/_ScreenParams.xy;
                // float2 p = -1.0 + 2.0*q;
                float2 p = q;
                p.x *= _ScreenParams.x/_ScreenParams.y;         //aspect ratio

                float travelTime = (_Time * 0.2) + 0.1;
                // float travelTime = (_Time * 0.2);
	
                float2 tiling = float2(1., .01);
                float2 offset = float2(travelTime * 0.5 + p.x * 0.2, travelTime * 0.2);
	
                float2 st = p * tiling + offset;
    
                float rain = 0.1;  
                float f = noise(st * 200.5) * noise(st * 125.5);  
                // float f = noise(st);
   	            f = clamp(pow(abs(f), 15.0) * 1.5 * (rain * rain * 125.0), 0.0, 0.25);
                
                float3 col = float3(1.0, 1.0, 1.0) * f;

                float4 finalCol = float4(col, 1.0);
                // float4 finalCol = tex2D(_MainTex, i.uv);
                // float4 finalCol = _Color;
                return finalCol;
            }
            */
            ENDCG
        }
    }
}
