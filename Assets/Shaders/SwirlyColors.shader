Shader "Custom/SwirlyColors" {
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Transparency("Transparency", Range(0.0, 1.0)) = 0.5
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		LOD 200
		CGPROGRAM

#pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert
#pragma target 3.0

		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
		float3 localPos;
		float3 worldPos;
	};

	half _Glossiness;
	half _Metallic;
	fixed4 _Color;
	half _Transparency;

	void vert(inout appdata_full v, out Input o) {
		UNITY_INITIALIZE_OUTPUT(Input, o);
		o.localPos = v.vertex.xyz;
	}

	void surf(Input IN, inout SurfaceOutputStandard o)
	{
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

		c.r = (1 + sin(_Time.y + IN.worldPos.x * 1000)) / 2;
		c.g = (1 + cos(_Time.y + IN.worldPos.y * 1000)) / 2;
		c.b = (c.r + c.b) / 2;

		o.Albedo = c.rgb;
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = _Transparency;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
