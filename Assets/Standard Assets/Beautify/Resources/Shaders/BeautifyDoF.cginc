	// Copyright 2016 Kronnect - All Rights Reserved.
	// Based on research "Efficiently Simulating the Bokeh of Polygonal Apertures in a Post-Process Depth of Field Shader" L. McIntosh, B. E. Riecke and S. DiPaola
	
	#include "UnityCG.cginc"

	uniform sampler2D_float _CameraDepthTexture;
	uniform float4    _CameraDepthTexture_TexelSize;
	uniform sampler2D _MainTex;
	uniform float4    _MainTex_TexelSize;
	uniform float4    _MainTex_ST;
	uniform float4	  _BokehData;
	uniform float4    _BokehData2;
	
    struct appdata {
    	float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
    };
    
	struct v2f {
	    float4 pos : SV_POSITION;
	    float2 uv: TEXCOORD0;
		float2 depthUV : TEXCOORD1;	    		
	};

	v2f vert(appdata v) {
    	v2f o;
    	o.pos = UnityObjectToClipPos(v.vertex);
    	o.uv  = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);
		o.depthUV = o.uv;
		
    	#if UNITY_UV_STARTS_AT_TOP
    	if (_MainTex_TexelSize.y < 0) {
	        // Depth texture is inverted WRT the main texture
    	    o.depthUV.y = 1.0 - o.depthUV.y;
    	}
    	#endif  	
    	return o;
	}

	float getCoc(float2 uv) {
		float depth  = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, uv)));
		float xd     = abs(depth - _BokehData.x) - _BokehData2.x * (depth < _BokehData.x);
		return 0.5 * _BokehData.y * xd/depth;	// radius of CoC
	}
				
	float4 fragCoC (v2f i) : SV_Target {
		float4 p      = tex2D(_MainTex, i.uv);
		#if UNITY_COLORSPACE_GAMMA
		p.rgb = GammaToLinearSpace(p.rgb);
		#endif
   		return float4(p.rgb, getCoc(i.depthUV));
   	}	
	
	float4 fragCoCDebug (v2f i) : SV_Target {
		float4 pixel  = tex2D(_MainTex, i.uv);
		float  CoC    = getCoc(i.depthUV);
		pixel.a       = min(CoC, pixel.a);
		return pixel.aaaa;
   	}
		
	float4 fragBlur (v2f i): SV_Target {
		float4 sum     = tex2D(_MainTex, i.uv );
   		
		int samples    = ceil(sum.a);
		float  w       = 1.0;
		float4 dir     = float4(_BokehData.zw * _MainTex_TexelSize.xy, 0, 0);
		       dir    *= max(1.0, (float)samples / _BokehData2.y);
		float  jitter  = dot(float2(2.4084507, 3.2535211), i.uv * _MainTex_TexelSize.zw);
		float2 disp0   = dir.xy * (frac(jitter) + 0.5);
		float4 disp1   = float4(i.uv + disp0, 0, 0);
		float4 disp2   = float4(i.uv - disp0, 0, 0);

		int sampleCount = (int)min(_BokehData2.y, samples);
		for (int k=1;k<sampleCount;k++, disp1 += dir, disp2 -= dir) {
			float4 pixel1 = tex2Dlod(_MainTex, disp1);
			if (pixel1.a>k) {
				sum += pixel1;
				w ++;
			}
			float4 pixel2 = tex2Dlod(_MainTex, disp2);
			if (pixel2.a>k) {
				sum += pixel2;
				w ++;
			}
		}
		sum /= w;
		return sum + _BokehData2.w * max(sum.g - _BokehData2.z, 0);
	}

