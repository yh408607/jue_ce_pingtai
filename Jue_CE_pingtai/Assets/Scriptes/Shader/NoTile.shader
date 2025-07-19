Shader "Custom/NoTile"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlendScale("BlendScale",Range(0,0.5))=0.25
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            float _BlendScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

             //制造随机
            float4 hash4( float2 p ) { return frac(sin  ( float4( 1.0+dot(p,float2(37.0,17.0)), 
                                                                 2.0+dot(p,float2(11.0,47.0)),
                                                                  3.0+dot(p,float2(41.0,29.0)),
                                                                  4.0+dot(p,float2(23.0,31.0))))*103.0); }

            float4 textureNoTile( sampler2D samp,  float2 uv )
            {
                //iuv得到该片元所在的砖块坐标 fuv该片元在此砖上的位置（常用）
                float2 iuv = floor( uv );   
                float2 fuv = frac( uv );

                // 制造随机4个随机因子，xy分量用来做位移、zw分量用来做横向、纵向的贴图翻转（固值区0-1之间）
                float4 ofa = hash4( iuv + float2(0.0,0.0) );
                float4 ofb = hash4( iuv + float2(1.0,0.0) );
                float4 ofc = hash4( iuv + float2(0.0,1.0) );
                float4 ofd = hash4( iuv + float2(1.0,1.0) );

                //ddx 和 ddy，分别对应 x, y 轴上，在屏幕空间中，像素块中各种变量的变化率。
                float2 dx = ddx( uv );
                float2 dy = ddy( uv );

                // 上面说zw分量为0-1之间的随机数，-0.5后用sign求随机的±
                ofa.zw = sign(ofa.zw-0.5);
                ofb.zw = sign(ofb.zw-0.5);
                ofc.zw = sign(ofc.zw-0.5);
                ofd.zw = sign(ofd.zw-0.5);
                
                //对uv改变+-再加偏移  对uv偏导数区±
                float2 uva = uv*ofa.zw + ofa.xy;     float2 ddxa = dx*ofa.zw;   float2 ddya = dy*ofa.zw;
                float2 uvb = uv*ofb.zw + ofb.xy;    float2 ddxb = dx*ofb.zw;    float2 ddyb = dy*ofb.zw;
                float2 uvc = uv*ofc.zw + ofc.xy;    float2 ddxc = dx*ofc.zw;   float2 ddyc = dy*ofc.zw;
                float2 uvd = uv*ofd.zw + ofd.xy;     float2 ddxd = dx*ofd.zw;   float2 ddyd = dy*ofd.zw;
                    
                // 设置手动调节的融合
                float2 b = smoothstep(_BlendScale,1-_BlendScale,fuv);
                //先通过 fuv的x分量 将 ↙↘和↖↗分别融合，再通过y分量将上下两部分融合。
                return lerp( lerp( tex2D( samp, uva, ddxa, ddya ), 
                                tex2D( samp, uvb, ddxb, ddyb ), b.x ), 
                            lerp( tex2D( samp, uvc, ddxc, ddyc ),
                                tex2D( samp, uvd, ddxd, ddyd ), b.x), b.y );
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = textureNoTile(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}