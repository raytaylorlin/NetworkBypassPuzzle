Shader "Custom/Noise/Pseudo Random Animation"
{
    Properties
    {
        _Factor1 ("Factor 1", float) = 1
        _Factor2 ("Factor 2", float) = 1
        _Factor3 ("Factor 3", float) = 1
        _Speed ("Speed", float) = 1
        _Color ("Color Tint", Color) = (1, 1, 1, 1)
    }
 
    SubShader
    {
        Tags { "RenderType"="Opaque" }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
           
            #include "UnityCG.cginc"
           
            float _Factor1;
            float _Factor2;
            float _Factor3;
            float _Speed;
            fixed4 _Color;
 
            float noise(half2 uv)
            {
                float t = _Speed * _Time.y;
                return frac(sin(dot(uv, float2(t * _Factor1, t * _Factor2))) * t * _Factor3);
            }
 
            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = noise(i.uv);
                col.rgb *= _Color.rgb;
                return col;
            }
            ENDCG
        }
    }
}