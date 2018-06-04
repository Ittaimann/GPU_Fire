Shader "Custom/Particle" {

	SubShader {
		Pass {
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		Blend SrcAlpha one

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 5.0

		struct Particle{
			float3 position;
			float3 SpawnPos;
			float3 velocity;
			float life;
			float3 Normal;
		};
		
		struct PS_INPUT{
			float4 position : SV_POSITION;
			float4 color : COLOR;
			float life : LIFE;
		};
		// particles' data
		StructuredBuffer<Particle> particleBuffer;
		

		PS_INPUT vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
		{
			PS_INPUT o = (PS_INPUT)0;

			// Color
			float life = particleBuffer[instance_id].life;
			float lerpVal = life * 0.25f;
			//o.color = fixed4(sin(particleBuffer[instance_id].position.x), sin(particleBuffer[instance_id].position.y),sin(particleBuffer[instance_id].position.z), lerpVal);
			// Position
			 //fixed4(1.0f - lerpVal+0.1, lerpVal+0.1, 1.0f, lerpVal);
			o.color = fixed4(lerpVal,sin(particleBuffer[instance_id].position.y*10),0,1);
			o.position = UnityObjectToClipPos(float4(particleBuffer[instance_id].position, 1.0f));
		
			o.color.a=saturate(dot(particleBuffer[instance_id].Normal,normalize(_WorldSpaceCameraPos-particleBuffer[instance_id].position)));

			return o;
		}

		float4 frag(PS_INPUT i) : COLOR
		{
			return i.color;
		}


		ENDCG
		}
	}
	FallBack Off
}