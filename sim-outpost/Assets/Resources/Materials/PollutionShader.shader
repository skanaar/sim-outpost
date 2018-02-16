Shader "Skanaar/GroundPollutionShader" {
    Properties {
        _GridTex ("Grid", 2D) = "white" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert fullforwardshadows
        #pragma target 3.0
        struct Input {
            float2 uv_GridTex;
            float4 vertexColor;
        };

        struct v2f {
            float4 pos : SV_POSITION;
            fixed4 color : COLOR;
        };

        void vert(inout appdata_full v, out Input o){
            UNITY_INITIALIZE_OUTPUT(Input,o);
            o.uv_GridTex = v.texcoord1.xy;
            o.vertexColor = v.color;
        }

        sampler2D _GridTex;

        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 grid = tex2D(_GridTex, IN.uv_GridTex);
            float4 c = IN.vertexColor;
            float pollute = clamp(c.r, 0, 1);
            float gridness = 1 - clamp(5*(c.a-0.3)*(c.a-0.3), 0, 1);
            float3 color = pollute*float3(1,0,0) + (1-pollute)*float3(1,1,1);
            o.Albedo = color * (1*(1-gridness) + grid.rgb*gridness);
            o.Alpha = 1;
        }
        ENDCG
    } 
    FallBack "Diffuse"
}