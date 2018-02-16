Shader "Skanaar/GroundShader" {
    Properties {
        _MainTex ("Ground", 2D) = "white" {}
        _GridTex ("Grid", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert fullforwardshadows
        #pragma target 3.0
        struct Input {
            float2 uv_MainTex;
            float2 uv_GridTex;
            float4 vertexColor;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            fixed4 color : COLOR;
        };

        void vert(inout appdata_full v, out Input o){
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.uv_MainTex = v.texcoord.xy;
            o.uv_GridTex = v.texcoord1.xy;
            o.vertexColor = v.color;
        }

        sampler2D _MainTex;
        sampler2D _GridTex;

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 ground = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 grid = tex2D(_GridTex, IN.uv_GridTex);
            float4 c = IN.vertexColor;
            float gridness = 1 - clamp(5*(c.a-0.3)*(c.a-0.3), 0, 1);
            float3 color = c.a*float3(0.5,0.5,0.5) + (1-c.a)*float3(0.2,1,0.1);
            o.Albedo = color * (ground.rgb*(1-gridness) + grid.rgb*gridness);
            o.Alpha = 1;
        }
        ENDCG
    } 
    FallBack "Diffuse"
}