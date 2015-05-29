Shader "Custom/GlobalSplashShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 1.0

		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (.002, 0.03)) = .005
		_MainTexPaint ("Albedo (Paint, RGB)", 2D) = "white" {}

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		half _Glossiness;
		half _Metallic;
		float _BumpScale;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = UnpackScaleNormal (tex2D (_BumpMap, IN.uv_BumpMap), _BumpScale);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
		

		UsePass "Toon/Basic Outline/OUTLINE"

		//Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		BlendOp Add
		Name "SPLASH"

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows decal:blend 

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTexPaint;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex; 
			float3 worldPos;
			float2 uv_BumpMap;
		};
		uniform sampler2D _PaintTexture;
		uniform float _PaintScale = 0;
		
		uniform half _GlossinessPaint = 0;
		uniform half _MetallicPaint = 0;
		float _BumpScale;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			if(_PaintScale == 0) {
				o.Alpha = 0;
				return;
			}
	
			
			float textureWidth = 4096;
			float textureHeight = 4096;
			//half2 uvs = half2(
			//	((IN.worldPos.x + (sin(IN.worldPos.x + _Time.y)) * 0.1 )  * 100 / textureWidth) + 0.5,
			//	((IN.worldPos.z + (cos(IN.worldPos.z + _Time.y)) * 0.1 ) * 100 / textureHeight) + 0.5
			//);
			half2 uvs = half2(
				(IN.worldPos.x * _PaintScale / textureWidth) + 0.5,
				(IN.worldPos.z * _PaintScale / textureHeight) + 0.5
			);

			fixed4 color = tex2D(_PaintTexture, uvs);
			fixed4 c = tex2D (_MainTexPaint, IN.uv_MainTex) * color;
			
			o.Albedo = c.rgb;
			o.Normal = UnpackScaleNormal (tex2D (_BumpMap, IN.uv_BumpMap), _BumpScale);
			// Metallic and smoothness come from slider variables
			o.Metallic = _MetallicPaint;
			o.Smoothness = _GlossinessPaint;

			if(c.a < 0.4)
				o.Alpha = 0;
			else
				o.Alpha = clamp(c.a * 3, 0, 0.95);
			
			if(color.r == 1 && color.g == 1 && color.b == 1) {
				o.Alpha = 0;
				o.Metallic = 0;
				o.Smoothness = 0;
			}

			
		}
		ENDCG
	
		//Pass
		//{
		//	Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		//	Blend SrcAlpha OneMinusSrcAlpha
		//	CGPROGRAM
		//	#pragma vertex vert
		//	#pragma fragment frag
		//	#include "UnityCG.cginc"

		//	uniform sampler2D _PaintTexture;
		//	uniform float _PaintScale;
		//	struct v2f
		//	{
		//		float4 pos          : POSITION;
		//		float3 worldPos     : TEXCOORD1;
		//	};

		//	v2f vert (appdata_full v)
		//	{
		//		v2f o;
		//		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		//		o.worldPos = mul(_Object2World, v.vertex);
		//		return o;
		//	}


		//	fixed4 getColor(float3 worldPos) {
		//		float textureWidth = 4096;
		//		float textureHeight = 4096;

		//		float x = worldPos.x * _PaintScale;
		//		float y = worldPos.z * _PaintScale;
		//		//float diffX = frac(x) - 0.5;
		//		//float diffY = frac(y) - 0.5;

		//		//if(sqrt(diffX * diffX + diffY * diffY) > 0.5) {
		//		//	x = diffX < 0 ? floor(x) : ceil(x);
		//		//	y = diffY < 0 ? floor(y) : ceil(y);
		//		//} else {
		//		//	x = diffX < 0 ? ceil(x) : floor(x);
		//		//	y = diffY < 0 ? ceil(y) : floor(y);
		//		//}

		//		//x += 0.5;
		//		//y += 0.5;

		//		half2 uvs = half2(
		//			(x / textureWidth) + 0.5,
		//			(y / textureHeight) + 0.5
		//		);

		//		return tex2D(_PaintTexture, uvs);
		//	}
            
		//	half4 frag( v2f i ) : COLOR
		//	{

		//		fixed4 color = getColor(i.worldPos);

		//		if(color.r == 1 && color.g == 1 && color.b == 1)
		//			color.a = 0;
		//		else
		//			color.a = color.a;
				
		//		//color.a = sqrt(sqrt(sqrt(color.a)));
		//		//color.a = pow(color.a, 15);
		//		if(color.a < 0.2)
		//			color.a = 0;
		//		else
		//			color.a *= 3;
		//		//if(color.a == 0) {
		//		//	discard;
		//		//}

		//		return color;

		//	}
		//	ENDCG          
		//}
		
	} 
	FallBack "Diffuse"
}
