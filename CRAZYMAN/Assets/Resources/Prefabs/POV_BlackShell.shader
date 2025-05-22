Shader "POV/BlackShell"
{
    Properties
    {
        _Alpha ("Darkness", Range(0, 1)) = 0.9
    }
    SubShader
    {
        Tags { "Queue"="Overlay+100" "RenderType"="Transparent" }
        Cull Front
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _Alpha;

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 camForward = _WorldSpaceCameraPos + float3(0, 0, 1); // fake forward
                float3 dir = normalize(i.worldPos - _WorldSpaceCameraPos);

                float fade = smoothstep(0.5, 0.95, dot(dir, normalize(_WorldSpaceCameraPos - i.worldPos)) * -1);
                float finalAlpha = fade * _Alpha;

                return float4(0, 0, 0, finalAlpha);
            }
            ENDCG
        }
    }
}
