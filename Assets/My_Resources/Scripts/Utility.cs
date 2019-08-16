using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static List<GameObject> SamplesPlacement(GameObject pf, float radius, int amount, Shapes shape, Transform parent = null)
    {
        List<GameObject> samples = new List<GameObject>(amount);

        if (shape is Shapes.wall)
        {
            for(int i = 0; i < amount; i++)
            {
                float x_Pos = -radius + i * radius / amount * 2;
                var pos = new Vector3(x_Pos, parent.position.y, 1);

                GameObject obj = Object.Instantiate(pf, pos + parent.position, Quaternion.identity, parent) as GameObject;
                samples.Add(obj);
            }
        }
        else if(shape is Shapes.circule)
        {
            for(int i = 0; i < amount; i++)
            {
                float angle = i * Mathf.PI * 2 / amount;
                Vector3 position = new Vector3(Mathf.Cos(angle), parent.position.y, Mathf.Sin(angle)) * radius;

                GameObject obj = Object.Instantiate(pf, position + parent.position, Quaternion.identity, parent) as GameObject;
                samples.Add(obj);
            }
        }
        else if(shape is Shapes.spiral)
        {
            float multiplier = 3f;
            for(int i = 0; i < amount; ++i)
            {
                Spiral pos = GetSpiral(i);
                GameObject obj = Object.Instantiate(pf,new Vector3(pos.x * multiplier + parent.transform.position.x, 0, pos.z * multiplier + parent.transform.position.z), parent.transform.rotation, parent);
                samples.Add(obj);
            }
        }
        return samples;
    }
    private static Spiral GetSpiral(int n)
    {
        int x = 0, z = 0;
        if (--n >= 0)
        {
            int v = (int)Mathf.Floor(Mathf.Sqrt(n + 0.25f) - 0.5f);
            int spiralBaseIndex = v * (v + 1);
            int flipFlop = ((v & 1) << 1) - 1;
            int offset = flipFlop * ((v + 1) >> 1);
            x += offset; z += offset;

            int cornerIndex = spiralBaseIndex + (v + 1);
            if (n < cornerIndex)
            {
                x -= flipFlop * (n - spiralBaseIndex + 1);
            }
            else
            {
                x -= flipFlop * (v + 1);
                z -= flipFlop * (n - cornerIndex + 1);
            }
        }

        Spiral result = new Spiral();
        result.x = x;
        result.z = z;

        return result;
    }

    public enum Shapes
    {
        wall,
        circule,
        spiral
    }
    public enum SpectumToUse
    {
        Hz128,
        Hz256,
        Hz512,
        Hz1024,
        Hz2048,
        Hz4096
    }
    public struct Spiral
    {
        public float x;
        public float z;
    }
}
