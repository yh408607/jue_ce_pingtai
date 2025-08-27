Shader "Custom/SDFHoleShader_Dynamic"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _HoleRadius ("Hole Radius", Range(0,0.5)) = 0.1
        _Feather ("Feather", Range(0,0.2)) = 0.05
        _HolePositions ("Hole Positions", Vector) = (0.5,0.5,0,0) // 只用xy坐标
    }
    
    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0
        
        sampler2D _MainTex;
        fixed4 _Color;
        float _HoleRadius;
        float _Feather;
        float4 _HolePositions[10]; // 最大支持10个孔洞
        
        struct Input {
            float2 uv_MainTex;
        };
        
        float createHole(float2 uv, float2 holePos) {
            float dist = distance(uv, holePos);
            return smoothstep(_HoleRadius, _HoleRadius + _Feather, dist);
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            
            float alpha = 1.0;
            // 计算所有孔洞的影响
            for(int i = 0; i < 10; i++) {
                alpha *= createHole(IN.uv_MainTex, _HolePositions[i].xy);
            }
            
            o.Albedo = c.rgb;
            o.Alpha = c.a * alpha;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}