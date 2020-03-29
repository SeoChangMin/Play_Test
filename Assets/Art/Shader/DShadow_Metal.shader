Shader "Custom/DShadow_Metal"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("BumpMap (RGB)", 2D) = "white" {}

		_RefMask("RefMask" ,2D) = "black"{}
		_RefPower("Ref Power", Range(0,1)) = 1
		
       
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
      
        #pragma surface surf Kim fullforwardshadows

			#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _RefMask;


		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_RefMask;

			float3 worldRefl;
			INTERNAL_DATA
		};


		float4 _Color;
		float _RefPower;
		




		float4 LightingKim(SurfaceOutput s, float3 lightDir, float atten)
		{
			float NdotL = dot(s.Normal, lightDir);//*0.5 + 0.5;




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

			float ShadowSet = NdotL*1.2;

			ShadowSet = (ShadowSet * (SetCAtten*0.8)) * atten;

			/*
			if (ShadowSet >= 1) {
			
				ShadowSet = 1;

			}
			if(ShadowSet <= 0){
			
				ShadowSet = 0;

			}  
			*/

			
			
			
			//SColor = SColor * _ShadowColor;

			//float4 ShadowSetEnd = ShadowSet + SColor.rgba;

			////////////////////////////////////-- 셰도우 계산종료








			float4 c;


			//c.rgb = ShadowSet; // ShadowSet*atten *_LightColor0;
			c.rgb = s.Albedo * ShadowSet * _LightColor0;


			c.a = s.Alpha;

			return c;
		}



       
        UNITY_INSTANCING_BUFFER_START(Props)
            
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			float4 RefMask = tex2D(_RefMask, IN.uv_RefMask)*_RefPower;


			

           
			
			o.Albedo = c.rgb;


			////////////////////////리플렉션 계산


			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

			float3 rv = WorldReflectionVector(IN, o.Normal).xyz;
			float3 Ref = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, rv).rgb * unity_SpecCube0_HDR.r;

			o.Emission = Ref.rgb*RefMask.rgb;


			////////////////////////종료


		    o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
