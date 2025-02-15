using UnityEngine;

[System.Serializable]
public struct Boundary
{
    public float minValue;
    public float maxValue;

    public Boundary(float min, float max)
    {
        minValue = min;
        maxValue = max;
    }
}
