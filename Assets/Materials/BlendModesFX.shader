// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// unlit, vertex colour, alpha blended
// cull off

Shader "CodeCorsair/BlendModesFX" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}

		// This here gives hints to the shader to do pixel-time tricks
		_BlendMode("BlendMode", Range(0, 16)) = 0 // 0 is standard blend

		[HideInInspector] _BlendOp1("__op1", Float) = 0.0
		[HideInInspector] _BlendSrc1("__src1", Float) = 1.0
		[HideInInspector] _BlendDst1("__dst1", Float) = 0.0
		[HideInInspector] _BlendSrcAlpha1("__src_alpha1", Float) = 1.0
		[HideInInspector] _BlendDstAlpha1("__dst_alpha1", Float) = 0.0

		[HideInInspector] _BlendOp2("__op2", Float) = 0.0
		[HideInInspector] _BlendSrc2("__src2", Float) = 1.0
		[HideInInspector] _BlendDst2("__dst2", Float) = 0.0
		[HideInInspector] _BlendSrcAlpha2("__src_alpha2", Float) = 1.0
		[HideInInspector] _BlendDstAlpha2("__dst_alpha2", Float) = 0.0
	}
	
	SubShader
	{		
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		ZWrite Off Lighting Off Cull Off Fog{ Mode Off }
		LOD 110

		Pass
		{
			BlendOp[_BlendOp1]
			Blend[_BlendSrc1][_BlendDst1],[_BlendSrcAlpha1][_BlendDstAlpha1]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _BlendMode;

			struct vin
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};

			v2f vert(vin v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{	
				float4 color = tex2D(_MainTex, i.texcoord);

				if (_BlendMode == 1) // Darken
				{
					color.rgb = lerp(float3(1, 1, 1), color.rgb, color.a);
				}
				else if (_BlendMode == 2) // Multiply
				{
					color.rgb *= color.a;
				}
				else if (_BlendMode == 3) // Color Burn
				{
					color.rgb = 1.0 - (1.0 / max(0.001, color.rgb * color.a + 1.0 - color.a)); // max to avoid infinity
				}
				else if (_BlendMode == 4) // Linear Burn
				{
					color.rgb = (color.rgb - 1.0) * color.a;
				}
				else if (_BlendMode == 5) // Lighten
				{
					color.rgb = lerp(float3(0, 0, 0), color.rgb, color.a);
				}
				else if (_BlendMode == 6) // Screen
				{
					color.rgb *= color.a;
				}
				else if (_BlendMode == 7) // Color Dodge
				{
					color.rgb = 1.0 / max(0.01, (1.0 - color.rgb * color.a));
				}
				else if (_BlendMode == 8) // Linear Dodge
				{
					// Do nothing
				}
				else if (_BlendMode == 9) // Overlay
				{
					color.rgb *= color.a;

					float3 desiredValue = (4.0 * color.rgb - 1.0) / (2.0 - 4.0 * color.rgb);
					float3 backgroundValue = (1.0 - color.a) / ((2.0 - 4.0 * color.rgb) * max(0.001, color.a));

					color.rgb = desiredValue + backgroundValue;
				}
				else if (_BlendMode == 10) // Soft Light
				{
					float3 desiredValue = 2.0 * color.rgb * color.a / (1.0 - 2.0 * color.rgb * color.a);
					float3 backgroundValue = (1.0 - color.a) / ((1.0 - 2.0 * color.rgb * color.a) * max(0.001, color.a));

					color.rgb = desiredValue + backgroundValue;
				}
				else if (_BlendMode == 11) // Hard Light
				{
					float3 numerator = (2.0 * color.rgb * color.rgb - color.rgb) * (color.a);
					float3 denominator = max(0.001, (4.0 * color.rgb - 4.0 * color.rgb * color.rgb) * (color.a) + 1.0 - color.a);
					color.rgb = numerator / denominator;
				}
				else if (_BlendMode == 12) // Vivid Light
				{
					color.rgb *= color.a;
					color.rgb = color.rgb >= float3(0.5, 0.5, 0.5) ? (1.0 / max(0.0001, 2.0 - 2.0 * color.rgb)) : float3(1.0, 1.0, 1.0);
				}
				else if (_BlendMode == 13) // Linear Light
				{
					color.rgb = (2 * color.rgb - 1.0) * color.a;
				}

				return color;
			}
			
			ENDCG
		}

		Pass
		{
			BlendOp[_BlendOp2]
			Blend[_BlendSrc2][_BlendDst2],[_BlendSrcAlpha2][_BlendDstAlpha2]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _BlendMode;

			struct vin
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};

			v2f vert(vin v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{	
				float4 color = tex2D(_MainTex, i.texcoord);
				color.rgb *= i.color.rgb;
				color.a *= i.color.a;
				
				if (_BlendMode == 9) // Overlay
				{
					color.rgb *= color.a; // For alpha blending
					float3 value = (2.0 - 4.0 * color.rgb);
					color.rgb = value * max(0.001, color.a);
				}
				else if(_BlendMode == 10) // Soft Light
				{
					color.rgb = (1.0 - 2.0 * color.rgb * color.a) * max(0.001, color.a);
				}
				else if (_BlendMode == 11) // Hard Light
				{
					color.rgb = max(0.001, (4.0 * color.rgb - 4.0 * color.rgb * color.rgb) * (color.a) + 1.0 - color.a); // max because 0 goes to infinity
				}
				else if (_BlendMode == 12) // Vivid Light
				{
					//color.rgb *= color.a;
					color.rgb = color.rgb < 0.5 ? (color.a - color.a / max(0.0001, 2.0 * color.rgb)) : float3(0.0, 0.0, 0.0);
				}
				else if (_BlendMode == 13) // Linear Light
				{
					
				}

				return color;
			}
			
			ENDCG
		}
	}
}
