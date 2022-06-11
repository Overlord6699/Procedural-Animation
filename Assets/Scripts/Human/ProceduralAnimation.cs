using System;
using UnityEngine;
using UnityEngine.Pool;

public class ProceduralAnimation : MonoBehaviour
{
    [SerializeField] 
    private float loopTime = 1;

    [SerializeField] 
    private ProceduralAnimationDefinition animation;

    private AnimationGroupInstance[] groups;

    private void Awake()
    {
        var tmp = ListPool<AnimationGroupInstance>.Get();

        foreach (var group in animation.Groups)
        {
            var pathElements = group.TargetPath.Split("\\");
            var target = transform;

            foreach (var pathElement in pathElements)
            {
                target = target.Find(pathElement);
                if (target == null) break;
            }

            if (target != null)
                tmp.Add(new AnimationGroupInstance(target, group));

            groups = tmp.ToArray();

            ListPool<AnimationGroupInstance>.Release(tmp);
        }
    }

    private void Update()
    {
        var t = (Time.time % loopTime) / loopTime;
        foreach (var groupInstance in groups)
        {
            groupInstance.Update(t);
        }
    }
}