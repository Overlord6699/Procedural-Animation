using System;
using UnityEngine;

public class AnimationInstance
{
    private readonly ObjectProceduralAnimation animation;

    public AnimationInstance(ObjectProceduralAnimation animation)
    {
        this.animation = animation;
    }

    public void Update(float t, ref Vector3 currentOffset, ref Vector3 currentRotation)
    {
        var curveValue = animation.Evaluate(t);
        switch (animation.PropertyType)
        {
            case PropertyType.LocalTransformX:
                currentOffset.x += curveValue;
                break;
            case PropertyType.LocalTransformY:
                currentOffset.y += curveValue;
                break;
            case PropertyType.LocalTransformZ:
                currentOffset.z += curveValue;
                break;
            case PropertyType.LocalRotationX:
                currentRotation.x += curveValue;
                break;
            case PropertyType.LocalRotationY:
                currentRotation.y += curveValue;
                break;
            case PropertyType.LocalRotationZ:
                currentRotation.z += curveValue;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}