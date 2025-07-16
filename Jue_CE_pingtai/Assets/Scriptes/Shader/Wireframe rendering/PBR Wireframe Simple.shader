Shader "Custom/PBR Wireframe Simple"
{
    Properties
    {
        // PBR 属性
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}
        _MetallicGlossMap("Metallic", 2D) = "white" {}
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _BumpMap("Normal Map", 2D) = "bump" {}
        _BumpScale("Normal Scale", Float) = 1.0
        
        // 线框属性
        _WireColor("Wire Color", Color) = (0,0,0,1)
        [Toggle]_WireOnly("Wire Only", Float) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.0
            #include "UnityCG.cginc"
            #include "UnityStandardUtils.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2g
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 tangent : TEXCOORD2;
            };
            
            struct g2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 tangent : TEXCOORD2;
                float3 dist : TEXCOORD3;
            };
            
            // PBR 属性
            sampler2D _MainTex;
            sampler2D _MetallicGlossMap;
            sampler2D _BumpMap;
            half _Glossiness;
            half _Metallic;
            fixed4 _Color;
            half _BumpScale;
            
            // 线框属性
            fixed4 _WireColor;
            float _WireOnly;
            
            v2g vert(appdata v)
            {
                v2g o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
                return o;
            }
            
            [maxvertexcount(3)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
            {
                // 计算三角形在屏幕空间中的边长
                float2 p0 = IN[0].vertex.xy / IN[0].vertex.w;
                float2 p1 = IN[1].vertex.xy / IN[1].vertex.w;
                float2 p2 = IN[2].vertex.xy / IN[2].vertex.w;
                
                float2 edge0 = p2 - p1;
                float2 edge1 = p2 - p0;
                float2 edge2 = p1 - p0;
                
                float area = abs(edge1.x * edge2.y - edge1.y * edge2.x);
                
                g2f o;
                
                for(int i = 0; i < 3; i++)
                {
                    o.pos = IN[i].vertex;
                    o.uv = IN[i].uv;
                    o.normal = IN[i].normal;
                    o.tangent = IN[i].tangent;
                    
                    // 计算每个顶点到对边的距离
                    if(i == 0) o.dist = float3(area / length(edge0), 0, 0);
                    else if(i == 1) o.dist = float3(0, area / length(edge1), 0);
                    else o.dist = float3(0, 0, area / length(edge2));
                    
                    triStream.Append(o);
                }
            }
            
            float4 frag(g2f i) : SV_Target
            {
                // 标准PBR计算
                fixed4 c = tex2D(_MainTex, i.uv) * _Color;
                fixed4 metallicGloss = tex2D(_MetallicGlossMap, i.uv);
                half metallic = metallicGloss.r * _Metallic;
                half smoothness = metallicGloss.a * _Glossiness;
                
                // 法线计算
                half3 normal = i.normal;
                if(_BumpScale != 0.0)
                {
                    half3 tangentNormal = UnpackScaleNormal(tex2D(_BumpMap, i.uv), _BumpScale);
                    half3 tangent = normalize(i.tangent.xyz);
                    half3 bitangent = cross(normal, tangent) * i.tangent.w;
                    normal = normalize(tangentNormal.x * tangent + tangentNormal.y * bitangent + tangentNormal.z * normal);
                }
                
                // 线框计算 - 使用固定宽度0.008
                float minDist = min(i.dist.x, min(i.dist.y, i.dist.z));
                float wire = smoothstep(0.0009, 0.0009 + 0.0009, minDist);
                
                if(_WireOnly > 0.5)
                {
                    return _WireColor * (1 - wire);
                }
                else
                {
                    return lerp(_WireColor, c, wire);
                }
            }
            ENDCG
        }
    }
    FallBack "Standard"
}