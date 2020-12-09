
using UnityEngine;

public class Utils
{
    public static int GenerateHeight(int maxHeight,int minHeight, int octaves, float smooth, float persistance, float x, float y)
    {
        float height = ScaleFunctionResult(minHeight, maxHeight, 0, 1,FractalBrownianMotion(x * smooth, y * smooth, octaves, persistance));
        return (int)height;
    }
    public static int GenerateHeight(float x, float y)
    {
        int maxHeight = 150;
        int octaves = 4;
        float smooth = 0.01f;
        float persistance = 0.5f;
        float height = ScaleFunctionResult(0, maxHeight, 0, 1, FractalBrownianMotion(x * smooth, y * smooth, octaves, persistance));
        return (int)height;
    }

    static float ScaleFunctionResult(float newMin, float newMax, float originMin, float originMax, float value)
    {
        return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(originMin, originMax, value));
    }
    static float FractalBrownianMotion(float x, float y, int octaves, float persistance)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 1;
        float offset = 64_000;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise((x+offset) * frequency, (y+offset) * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistance;
            frequency *= 2;
        }
        return total/maxValue;
       
    }

    public static  float FractalBrownianMotion_3D(float x, float y, float z, int octaves, float persistance, float smooth)
    {
        float XY = FractalBrownianMotion(x*smooth, y*smooth, octaves, persistance);
        float YZ = FractalBrownianMotion(y*smooth, z*smooth, octaves, persistance);
        float ZX = FractalBrownianMotion(z*smooth, x*smooth, octaves, persistance);

        float YX = FractalBrownianMotion(y * smooth, x * smooth, octaves, persistance);
        float ZY = FractalBrownianMotion(z * smooth, y * smooth, octaves, persistance);
        float XZ = FractalBrownianMotion(x * smooth, z * smooth, octaves, persistance);
        return (XY + YZ + ZX + YX + ZY + XZ) / 6;
    }
}
