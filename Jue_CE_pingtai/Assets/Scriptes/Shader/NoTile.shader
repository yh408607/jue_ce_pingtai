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

             //�������
            float4 hash4( float2 p ) { return frac(sin  ( float4( 1.0+dot(p,float2(37.0,17.0)), 
                                                                 2.0+dot(p,float2(11.0,47.0)),
                                                                  3.0+dot(p,float2(41.0,29.0)),
                                                                  4.0+dot(p,float2(23.0,31.0))))*103.0); }

            float4 textureNoTile( sampler2D samp,  float2 uv )
            {
                //iuv�õ���ƬԪ���ڵ�ש������ fuv��ƬԪ�ڴ�ש�ϵ�λ�ã����ã�
                float2 iuv = floor( uv );   
                float2 fuv = frac( uv );

                // �������4��������ӣ�xy����������λ�ơ�zw���������������������ͼ��ת����ֵ��0-1֮�䣩
                float4 ofa = hash4( iuv + float2(0.0,0.0) );
                float4 ofb = hash4( iuv + float2(1.0,0.0) );
                float4 ofc = hash4( iuv + float2(0.0,1.0) );
                float4 ofd = hash4( iuv + float2(1.0,1.0) );

                //ddx �� ddy���ֱ��Ӧ x, y ���ϣ�����Ļ�ռ��У����ؿ��и��ֱ����ı仯�ʡ�
                float2 dx = ddx( uv );
                float2 dy = ddy( uv );

                // ����˵zw����Ϊ0-1֮����������-0.5����sign������ġ�
                ofa.zw = sign(ofa.zw-0.5);
                ofb.zw = sign(ofb.zw-0.5);
                ofc.zw = sign(ofc.zw-0.5);
                ofd.zw = sign(ofd.zw-0.5);
                
                //��uv�ı�+-�ټ�ƫ��  ��uvƫ��������
                float2 uva = uv*ofa.zw + ofa.xy;     float2 ddxa = dx*ofa.zw;   float2 ddya = dy*ofa.zw;
                float2 uvb = uv*ofb.zw + ofb.xy;    float2 ddxb = dx*ofb.zw;    float2 ddyb = dy*ofb.zw;
                float2 uvc = uv*ofc.zw + ofc.xy;    float2 ddxc = dx*ofc.zw;   float2 ddyc = dy*ofc.zw;
                float2 uvd = uv*ofd.zw + ofd.xy;     float2 ddxd = dx*ofd.zw;   float2 ddyd = dy*ofd.zw;
                    
                // �����ֶ����ڵ��ں�
                float2 b = smoothstep(_BlendScale,1-_BlendScale,fuv);
                //��ͨ�� fuv��x���� �� �L�K�ͨI�J�ֱ��ںϣ���ͨ��y�����������������ںϡ�
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