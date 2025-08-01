Shader "Custom/PBR Transparent"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _MetallicGlossMap("Metallic (R) Smoothness (A)", 2D) = "white" {}
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _BumpMap("Normal Map", 2D) = "bump" {}
        _BumpScale("Normal Scale", Float) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}
        _OcclusionStrength("Occlusion Strength", Range(0.0, 1.0)) = 1.0
        _EmissionMap("Emission", 2D) = "black" {}
        [HDR]_EmissionColor("Emission Color", Color) = (0,0,0,1)

        _HoleCount("Hole Count", Int) = 0
        _HoleRadius("Hole Radius", Range(0,1)) = 0.2
        _Feather("Feather", Range(0,0.1)) = 0.02
        
        // ͸���ȿ�������
        _Alpha("Alpha", Range(0,1)) = 1.0
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
        [Enum(Off,0,On,1)] _AlphaTest("Alpha Test", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Src Blend", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Dst Blend", Float) = 0
        [Enum(Off,0,On,1)] _ZWrite("Z Write", Float) = 1
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        
        Blend [_SrcBlend] [_DstBlend]
        ZWrite [_ZWrite]
        Cull Back
        
        CGPROGRAM
        // ʹ�� Standard ����ģ�ͣ�������͸���Ȼ��
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0
        
        // ����͸���Ȳ���
        #pragma shader_feature _ALPHATEST_ON
        
        sampler2D _MainTex;
        sampler2D _MetallicGlossMap;
        sampler2D _BumpMap;
        sampler2D _OcclusionMap;
        sampler2D _EmissionMap;
        
        struct Input
        {
            float2 uv_MainTex;
        };

        struct v2f {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };
        
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _BumpScale;
        float _OcclusionStrength;
        fixed4 _EmissionColor;
        
        // ͸���ȿ��Ʊ���
        float _Alpha;
        float _Cutoff;

        float4 _HoleData;
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // ������ɫ
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            
            // �����Ⱥ͹⻬��
            fixed4 metallicGloss = tex2D(_MetallicGlossMap, IN.uv_MainTex);
            o.Metallic = metallicGloss.r * _Metallic;
            o.Smoothness = metallicGloss.a * _Glossiness;
            
            // ������ͼ
            o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
            
            // �������ڱ�
            fixed occ = tex2D(_OcclusionMap, IN.uv_MainTex).r;
            o.Occlusion = LerpOneTo(occ, _OcclusionStrength);
            
            // �Է���
            fixed3 emission = tex2D(_EmissionMap, IN.uv_MainTex).rgb * _EmissionColor.rgb;
            o.Emission = emission;
            
            // ͸���ȿ���
            o.Alpha = c.a * _Alpha;
            
            // ͸���Ȳ���
            #if _ALPHATEST_ON
                clip(o.Alpha - _Cutoff);
            #endif
        }


        fixed4 frag(v2f i) : SV_Target{
              fixed4 col = tex2D(_MainTex, i.uv);

            // ���㵽�׶����ĵľ���
            float dist = distance(i.uv, _HoleData.xy);
            // ���ڶ�Ч��
            float hole = step(dist, _HoleData.z);

            return fixed4(col.rgb, 1.0 - hole);
        }

        ENDCG
    }
   //  FallBack "Transparent"
   // CustomEditor "UnityEditor.ShaderGUI"
  // CustomEditor "PBRTransparentShaderGUI"
}