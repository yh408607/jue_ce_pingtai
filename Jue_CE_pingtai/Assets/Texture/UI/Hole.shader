Shader "Custom/SDFHoleShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _HoleCount ("Hole Count", Int) = 0
        _HoleRadius ("Hole Radius", Range(0,1)) = 0.2
        _Feather ("Feather", Range(0,0.1)) = 0.02
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        //LOD 200

        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        
        Blend SrcAlpha OneMinusSrcAlpha
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
            float4 _HoleData;
            
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // 计算到孔洞中心的距离
                float dist = distance(i.uv, _HoleData.xy);
                // 简单挖洞效果
                float hole = step(dist, _HoleData.z);
                
                return fixed4(col.rgb, 1.0 - hole);
            }
            ENDCG
        }

    }
    //FallBack "Diffuse"
}


