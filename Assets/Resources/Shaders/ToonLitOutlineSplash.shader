Shader "Custom/Toon Lit Outline Splash" {
	Properties {
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (.002, 0.03)) = .005
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {} 

		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_BumpScale("Scale", Float) = 1.0
		_MainTexPaint ("Albedo (Paint, RGB)", 2D) = "white" {}
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		UsePass "Toon/Lit/FORWARD"
		UsePass "Toon/Basic Outline/OUTLINE"

		Blend SrcAlpha OneMinusSrcAlpha
		BlendOp Add

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

	} 
	
	Fallback "Toon/Lit"
}
