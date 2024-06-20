// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Unlit/StarShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Blend SrcAlpha One
        ZTest Off
        Tags { "Queue" = "Background" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color[1023];
            float _Size[1023];

            //UNITY_INSTANCING_BUFFER_START(Props)
            //    UNITY_DEFINE_INSTANCED_PROP(float, _Size)
            //    #define _Size_arr Props

            //    UNITY_DEFINE_INSTANCED_PROP(float, _Color)
            //    #define _Color_arr Props

            //UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v, uint instanceID: SV_InstanceID)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex * _Size[instanceID]);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = float4(1, 1, 1, 1);
                UNITY_TRANSFER_FOG(o,o.vertex);

                #ifdef UNITY_INSTANCING_ENABLED
                    o.color = _Color[instanceID];
                #endif
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // UNITY_SETUP_INSTANCE_ID(i);
                float distance_from_center = length((2 * i.uv) - 1);
                float inverse_dist = saturate((0.2 / distance_from_center) - 0.2);
                float4 col = i.color;
                col.a = inverse_dist;
                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
