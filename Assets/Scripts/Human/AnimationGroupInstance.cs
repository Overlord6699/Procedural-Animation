using System.Linq;
using UnityEngine;

public class AnimationGroupInstance
{
    private readonly Transform target;
    private readonly AnimationInstance[] instances;
    private readonly Vector3 originalPosition;
    private readonly Vector3 originalRotation;

    public AnimationGroupInstance(Transform target, ObjectProceduralAnimationGroup animation)
    {
        this.target = target;
        originalPosition = target.localPosition;
        originalRotation = target.eulerAngles;
        instances = animation.Animations
            .Select(a => new AnimationInstance(a))
            .ToArray();
    }

    public void Update(float t)
    {
        var currentOffset = Vector3.zero;
        var currentRotation = Vector3.zero;

        foreach (var animationInstance in instances)
        {
            animationInstance.Update(t, ref currentOffset, ref currentRotation);
        }

        target.localPosition = originalPosition + currentOffset;
        target.localRotation = Quaternion.Euler(originalRotation + currentRotation);
    }
}