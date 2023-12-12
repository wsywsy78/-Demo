Shader "Custom/StencilMask"
{
    Properties
    {
		_RefValue("Stencil RefValue", int) = 1
        _MainTex("Main Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Portal" "Queue"="Geometry-1"}
        //Tags { "RenderType"="Mask" "Queue"="Geometry-1"}
        LOD 100

        Stencil{
            Ref [_RefValue]
			Comp Always
            Pass Replace
        }
        ZWrite Off

        ColorMask 0

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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			uniform float4 _Colour;

			fixed4 frag(v2f i) : SV_Target
			{
				return _Colour;
			}
		ENDCG
        }
    }
}
