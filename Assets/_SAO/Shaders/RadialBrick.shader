Shader "Custom/RadialBrickAltHeightURP"
{
    Properties
    {
        _BrickColor("Brick Color", Color) = (1, 0.9, 0.85, 1)
        _MortarColor("Mortar Color", Color) = (0.2, 0.2, 0.2, 1)

        _AngleScale("Bricks Per Ring", Float) = 64

        _RingHeightEven("Ring Height Even", Float) = 0.02
        _RingHeightOdd("Ring Height Odd", Float) = 0.06

        _MortarSize("Mortar Thickness (X=Angle, Y=Radius)", Vector) = (0.05, 0.05, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        LOD 100

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _BrickColor;
            float4 _MortarColor;

            float _AngleScale;
            float _RingHeightEven;
            float _RingHeightOdd;

            float4 _MortarSize;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 centeredUV = IN.uv - 0.5;

                float radius = length(centeredUV);
                float angle = atan2(centeredUV.y, centeredUV.x);
                angle = angle / (2.0 * 3.14159265) + 0.5;

                // Simulate stepping through rings to find index
                float acc = 0;
                float ringIndex = 0;
                [loop]
                for (int i = 0; i < 64; i++)
                {
                    float height = (fmod(i, 2.0) == 0.0) ? _RingHeightEven : _RingHeightOdd;
                    acc += height;

                    if (radius <= acc)
                    {
                        ringIndex = i;
                        break;
                    }
                }

                float isEvenRing = fmod(ringIndex, 2.0) == 0.0;
                float ringHeight = isEvenRing ? _RingHeightEven : _RingHeightOdd;

                // Local position inside the current ring (0–1)
                float ringStart = acc - ringHeight;
                float localRadiusInRing = (radius - ringStart) / ringHeight;

                // Offset bricks every other ring
                float rowOffset = fmod(ringIndex, 2.0) * 0.5;
                float scaledAngle = angle * _AngleScale + rowOffset;

                float2 brickUV = float2(scaledAngle, localRadiusInRing);
                float2 cellUV = frac(brickUV);

                float2 mortar = step(_MortarSize.xy, cellUV);
                float mask = mortar.x * mortar.y;

                float4 finalColor = lerp(_MortarColor, _BrickColor, mask);
                return finalColor;
            }

            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
