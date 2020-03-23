Shader "Custom/DumpTex"
{
    Properties
	{

		[NoScaleOffset]_MainTex("Tex1 (RGB)", 2D) = "white" {}
		_MainTexUV("UV Set",vector) = (1,1,0,0)

		[NoScaleOffset]_MainTex2 ("Tex2 (RGB)", 2D) = "white" {}
		_MainTex2UV("UV Set",vector) = (1,1,0,0)
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
       
        #pragma surface surf Kim


			float4 LightingKim(SurfaceOutput s, float3 lightDir, float atten)
		{
			float NdotL = dot(s.Normal,lightDir);//*0.5 + 0.5;




			//////////////////////////////////////-- 라이팅 경계 계산

			float SetCAtten = NdotL;

			if (SetCAtten < 0.1) {

				SetCAtten = 0;

			}
			else {

				SetCAtten = 1;

			}


			//////////////////////////////////////--계산종료




			/////////////////////////////////////-- 셰도우 계산

			float ShadowSet = NdotL;

			ShadowSet = (ShadowSet * (SetCAtten*0.8)) * _LightColor0 * atten;

			////////////////////////////////////-- 셰도우 계산종료








			float4 c;


			//c.rgb = ShadowSet; // ShadowSet*atten;
			c.rgb = s.Albedo *ShadowSet;
			c.a = s.Alpha;

			return c;
		}


       
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _MainTex2;
		
		float4 _MainTexUV;
		float4 _MainTex2UV;

        struct Input
        {
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
        };

       
 

      
        UNITY_INSTANCING_BUFFER_START(Props)
            
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            
			float2 topUV = float2(IN.worldPos.x, IN.worldPos.z);
			float2 frontUV = float2 (IN.worldPos.x, IN.worldPos.y);
			float2 sideUV = float2 (IN.worldPos.z, IN.worldPos.y);


			fixed4 topTex = tex2D(_MainTex, topUV * _MainTexUV.xy + _MainTexUV.zw);
			fixed4 frontTex = tex2D(_MainTex2, frontUV * _MainTex2UV.xy + _MainTex2UV.zw);
			fixed4 sideTex = tex2D (_MainTex2, sideUV * _MainTex2UV.xy + _MainTex2UV.zw);



			//o.Albedo = topTex.rgb;
            o.Albedo = lerp(topTex, frontTex, abs( IN.worldNormal.z) );
			o.Albedo = lerp(o.Albedo, sideTex, abs( IN.worldNormal.x) );
          
           
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
