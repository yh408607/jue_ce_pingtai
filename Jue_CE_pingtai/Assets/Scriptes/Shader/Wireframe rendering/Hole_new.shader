Shader "Custom/SDFHoleShader_WebGL"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _HoleRadius ("Hole Radius", Range(0,0.5)) = 0.5
        _Feather ("Feather", Range(0,0.2)) = 0.05
        _HolePosition1 ("Hole Position 1", Vector) = (0.5,0.5,0,0)
        // _HolePosition2 ("Hole Position 2", Vector) = (0.3,0.3,0,0)
        // _HolePosition3 ("Hole Position 3", Vector) = (0.7,0.3,0,0)
    }
    
    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent"
            "IgnoreProjector"="True"  // 这对WebGL很重要
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off  // 透明物体通常需要关闭深度写入
        
        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0  // 使用较低的特性级别确保WebGL兼容
        //#pragma surface surf Standard alphatest:_Cutoff
        
        sampler2D _MainTex;
        fixed4 _Color;
        float _HoleRadius;
        float _Feather;
        float4 _HolePosition1;
        // float4 _HolePosition2;
        // float4 _HolePosition3;
        
        struct Input {
            float2 uv_MainTex;
            //float4 screenPos;
        };
        
        float createHole(float2 uv, float2 holePos) {
            float dist = distance(uv, holePos.xy);
            // 平滑过渡
            return smoothstep(_HoleRadius, _HoleRadius + _Feather, dist);
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            
            // 计算所有孔洞的alpha
            float alpha = 1.0;
            alpha *= createHole(IN.uv_MainTex, _HolePosition1);
            // alpha *= createHole(IN.uv_MainTex, _HolePosition2);
            // alpha *= createHole(IN.uv_MainTex, _HolePosition3);
            
            o.Albedo = c.rgb;
            o.Alpha = c.a * alpha;
            o.Metallic = 0;
            o.Smoothness = 0;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"  // WebGL备用方案
}