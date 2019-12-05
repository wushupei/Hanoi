Shader "MyShader/Shader05"
{
	Properties
	{
		  _MainTex("MainTex", 2D) = "" {}
	      _SelfColor("SelfDiffuseColor",Color) = (1,1,1,1)
	}
	SubShader
	{
	     Pass
		 {
			Tags{"LightMode" = "ForwardBase"}
			CGPROGRAM
			#include "Lighting.cginc" 
			#pragma vertex vertexFun
			#pragma fragment fragmentFun
			sampler2D _MainTex;
			fixed4 _SelfColor;
			fixed _Specular;
			struct v2f
			{
			    fixed4 clipPos : SV_POSITION;
			    fixed3 worldPos : COLOR1;
			    fixed3 worldNormal : COLOR0;
				fixed2 uv : TEXCOORD0;
			};
			v2f vertexFun(appdata_full v)
			{				
			    v2f vf;
			    vf.clipPos = UnityObjectToClipPos(v.vertex);
			    vf.worldPos = mul(v.vertex,unity_WorldToObject); 
			    vf.worldNormal = mul(v.normal,(fixed3x3)unity_WorldToObject);
				vf.uv= v.texcoord.xy;
			    return vf;
			}
			//逐像素计算
			fixed4 fragmentFun(v2f vf) :SV_Target
			{
				//获取光源方向和法线方向并归一化(光源方向是指顶点到光源的方向)
				fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 normalDir = normalize(vf.worldNormal);

				//获取环境光
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				//获取漫反射
				fixed3 diffuse = _LightColor0.rgb* dot(normalDir,lightDir)*0.5 + 0.5; //使用半兰伯特获取漫反射
				diffuse = diffuse * _SelfColor.rgb; //融合自身光
				//获取高光(镜面反射)
				fixed3 reflectDir = normalize(reflect(-lightDir,normalDir)); //获取反射光方向
				fixed3 cameraDir = normalize(_WorldSpaceCameraPos.xyz - vf.worldPos.xyz); //获取摄像机方向(摄像机-顶点)
				fixed3 specular = _LightColor0.rgb*pow(dot(reflectDir,cameraDir)*0.5 + 0.5, _Specular); //根据公式计算高光(半兰伯特)
				//最终光照效果
				fixed3 color = tex2D(_MainTex, vf.uv) *( diffuse + ambient + specular);
				return fixed4(color,1); //最终的光照效果返回系统
			}
			ENDCG
		 }
	}
	Fallback "VertexLit"
}
