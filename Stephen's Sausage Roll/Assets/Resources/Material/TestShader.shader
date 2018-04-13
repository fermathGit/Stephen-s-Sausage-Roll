Shader "Unlit/TestShader"
{
	Properties
	{
		_MainTex("MainTex",2D) = "white"{}
		_BumpTex("BumpTex",2D) = ""{}
		_Num1("Num1",Range(1,100)) = 5
		_Num2("Num2",Range(0.1,0.9)) = 0.5
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0
		sampler2D _MainTex;
		sampler2D _BumpTex;
		float _Num1;
		float _Num2;
		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpTex;
			float3 worldPos;
		};
		void surf(Input IN, inout SurfaceOutput o) {
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
			_Num2 += 0.01;
			float v = frac((IN.worldPos.y + IN.worldPos.z*0.1)*_Num1) - _Num2;
			clip(v);
		}
		ENDCG
	}
}
