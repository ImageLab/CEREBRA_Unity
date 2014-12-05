Shader "IntensityShader" {
	Properties {
		_MainTex ("Texture Image", 2D) = "white" {}
	}
    
    SubShader {
		Tags { "RenderType" = "Opaque" }
		
		CGPROGRAM
		
		#pragma surface surf Lambert
		
		struct Input {
	  		float2 uv_MainTex;
		};
		
		sampler2D _MainTex;
		//float _MyFloat;
		//float _myfirst;
		//float _mysecond;
		
		void surf (Input inp, inout SurfaceOutput o) {
			
			//inp.uv_MainTex.y = _MyFloat;
	  		o.Albedo = tex2D(_MainTex, inp.uv_MainTex).rgb;
		}
		
		ENDCG
	}
	
	SubShader {
		Pass {
			
		    CGPROGRAM
		    
		    #pragma vertex vert
		    #pragma fragment frag
		    #pragma target 3.0

		    #include "UnityCG.cginc"
		    
		    uniform sampler2D _MainTex;
		    
		    struct vertexInput {
		        float4 vertex : POSITION;
		        float4 texcoord0 : TEXCOORD0;
		    };

		    struct fragmentInput{
		        float4 position : SV_POSITION;
		        float4 texcoord0 : TEXCOORD0;
		    };

		    fragmentInput vert(vertexInput i){
		        fragmentInput o;
		        o.position = mul (UNITY_MATRIX_MVP, i.vertex);
		        o.texcoord0 = i.texcoord0;
		        return o;
		    }
		    float4 frag(fragmentInput i) : COLOR {
		    	
		        return tex2D(_MainTex, i.texcoord0.xy);
		    }
		    
		    ENDCG
		}
	}
}
