Shader "Custom/SingleDynamicHole"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _HolePos ("Hole Position", Vector) = (0.5,0.5,0,0)
        _HoleRadius ("Hole Radius", Range(0,0.5)) = 0.1
        _Feather ("Feather Width", Range(0,0.2)) = 0.05
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
        #pragma target 2.0  // 更低的目标确保兼容性
        
        sampler2D _MainTex;
        fixed4 _Color;
        float2 _HolePos;
        float _HoleRadius;
        float _Feather;
        
        struct Input {
            float2 uv_MainTex;
        };
        
        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            
            // 计算到孔洞中心的距离
            float dist = distance(IN.uv_MainTex, _HolePos);
            
            // 计算透明度（带羽化边缘）
            float holeAlpha = smoothstep(_HoleRadius, _HoleRadius + _Feather, dist);
            
            o.Albedo = texColor.rgb;
            o.Alpha = texColor.a * holeAlpha;
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}