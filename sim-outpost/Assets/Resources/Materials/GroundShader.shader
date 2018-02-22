Shader "Skanaar/GroundShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _DetailTex ("Detail", 2D) = "white" {}
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
            o.uv_GridTex = v.texcoord2.xy;
            o.vertexColor = v.color;
        }

        sampler2D _MainTex;
        sampler2D _DetailTex;
        sampler2D _GridTex;

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o) {
            float3 ground = tex2D(_MainTex, IN.uv_MainTex).rgb;
            float3 detail = tex2D(_DetailTex, 0.25*IN.uv_GridTex).rgb;
            fixed4 grid = tex2D(_GridTex, IN.uv_GridTex);
            float4 c = IN.vertexColor;
            float unbuildable = clamp(10*(c.a-0.3f)*(c.a-0.3f), 0, 1);
            float gridness = 1 - unbuildable;
            o.Albedo = detail*ground*(float3(1,1,1)*(1-gridness) + grid.rgb*gridness);
            o.Alpha = 1;
        }
        ENDCG
    } 
    FallBack "Diffuse"
}