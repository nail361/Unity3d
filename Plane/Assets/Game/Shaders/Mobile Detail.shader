Shader "Mobile/DiffuseColored" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		
		Pass{
			
			Material{
				Diffuse [_Color]
			}
			
			Lighting On
			Fog { Mode Off }
			SetTexture [_MainTex] { combine texture * primary Double, texture * primary}
		}

	} 
	FallBack "VertexLit", 2
}
