Shader "Skanaar/WaterShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Bumpmap", 2D) = "white" {}
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard vertex:vert fullforwardshadows alpha
        #pragma target 3.0
        struct Input {
            float2 uv_MainTex;
            float4 vertexColor;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            fixed4 color : COLOR;
        };

        void vert(inout appdata_full v, out Input o){
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.uv_MainTex = v.texcoord.xy;
            o.vertexColor = v.color;
        }

        sampler2D _MainTex;
        sampler2D _BumpMap;

        void surf(Input IN, inout SurfaceOutputStandard o) {
            o.Albedo = float3(0.1,0.15,1);
            o.Metallic = 0;
            o.Smoothness = 0.5;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            o.Alpha = IN.vertexColor.a;
        }
        ENDCG
    } 
    FallBack "Diffuse"
}