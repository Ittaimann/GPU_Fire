Shader "Custom/Particle" {

	SubShader {
		Pass {
		Tags{ "RenderType" = "Transparent"  }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha

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
			//float3 worldNormal;
		};
		// particles' data
		StructuredBuffer<Particle> particleBuffer;
		

		PS_INPUT vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
		{
			PS_INPUT o = (PS_INPUT)0;

			// Color
			float life = particleBuffer[instance_id].life;
			float lerpVal = life * 0.5f;
			//o.color = fixed4(sin(particleBuffer[instance_id].position.x), sin(particleBuffer[instance_id].position.y),sin(particleBuffer[instance_id].position.z), lerpVal);
			// Position
			 //fixed4(1.0f - lerpVal+0.1, lerpVal+0.1, 1.0f, lerpVal);
			o.color = fixed4(abs(particleBuffer[instance_id].velocity.z)+abs(particleBuffer[instance_id].velocity.x) +saturate(lerpVal-.1)  ,saturate(lerpVal-.5) ,  0  ,lerpVal*.9);
			o.position = UnityObjectToClipPos(float4(particleBuffer[instance_id].position, 1.0f));
		
			
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