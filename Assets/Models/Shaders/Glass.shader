Shader "Unlit/Glass"
{
    //Properties
    Properties {
        _MainTex ("Main Tex", 2D) = "white" {}  //������������
        _Cubemap ("EM", Cube) = "_Skybox" {}
        _BumpMap ("Bump Map", 2D) = "bump" {}  //������������
        //control the distortion of refraction
        _Distortion ("Distortion", range(0, 100)) = 10
        _RefractAmount ("Refract Amount", Range(0.0, 1.0)) = 1.0
    }
    
    SubShader {
        //Queue must be transparent, opaque objects will be drawn before
        Tags { "Queue"="Transparent" "RenderType"="Opaque" }
 
        //define a pass to grab the screen behind the object,
        //see the result by using "_RefractionTex"
        GrabPass {"_RefractionTex"}
 
        Pass {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
 
            //Properties
            sampler2D _MainTex;
            float4 _MainTex_ST;
            samplerCUBE _Cubemap;
            sampler2D _BumpMap;
            float4 _BumpMap_ST;
            float _Distortion;
            fixed _RefractAmount;
            //remember to add:
            sampler2D _RefractionTex;
            //get the texel size:
            float4 _RefractionTex_TexelSize; 
            
            struct a2v {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                float4 tangent : TANGENT;
            };
 
            struct v2f {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 TtoW0 : TEXCOORD1;
                float4 TtoW1 : TEXCOORD2;
                float4 TtoW2 : TEXCOORD3;
                float4 srcPos : TEXCOORD4;
            };
 
            v2f vert(a2v v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
 
                //ץȡ��Ļͼ��Ĳ�������
                o.srcPos = ComputeGrabScreenPos(o.pos);
                o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv.zw = TRANSFORM_TEX(v.texcoord, _BumpMap);
 
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal).xyz;
                float3 worldTangent = UnityObjectToWorldNormal(v.tangent).xyz;
                float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
 
                //�������߿ռ� -> ����ռ�ľ���ֻ��Ҫ3x3
                //���аڷ�
                o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
                o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
                o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);
 
                return o;
            }
 
            fixed4 frag(v2f i) :SV_Target {
                float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
                //���������Ҫ������
                fixed3 worldlightDir = normalize(UnityWorldSpaceLightDir(worldPos));
                fixed3 worldviewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                //���������+���룬�õ����߷���
                fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
                
                //����������Ҫģ����ǲ���������Ч������˲��������ֳ���ķ��������ƫ�ƣ�
                //bump.xy *= _BumpScale;
                //bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
 
                //��ʼʵ������Ч��
                float2 offset = bump.xy * _Distortion * _RefractionTex_TexelSize.xy;
                i.srcPos.xy = offset + i.srcPos.xy;
                //�����õ������䡱��ɫ:
                fixed3 refractColor = tex2D(_RefractionTex, i.srcPos.xy/i.srcPos.w).rgb;
 
                bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
 
                //��ʼ����ӳ�䣺
                fixed3 reflectDir = reflect(-worldviewDir, bump);
                fixed4 texColor = tex2D(_MainTex, i.uv.xy);
                fixed3 reflectColor = texCUBE(_Cubemap, reflectDir).rgb * texColor.rgb;
 
                fixed3 finalColor = reflectColor * (1 - _RefractAmount) + refractColor * _RefractAmount;
                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    FallBack  "Diffuse"
}
