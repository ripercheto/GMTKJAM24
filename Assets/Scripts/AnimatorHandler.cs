using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class AnimatorHandler
{
    IEnumerable AnimatorParameters
    {
        get
        {
            if (animator == null)
            {
                yield break;
            }

            for (var i = 0; i < animator.parameterCount; i++)
            {
                var param = animator.GetParameter(i);
                yield return new ValueDropdownItem<int>(param.name, param.nameHash);
            }
        }
    }

    public Animator animator;
    [ValueDropdown(nameof(AnimatorParameters))]
    public int animatorParameter;

    public bool HasAnimator => animator != null;

    public void SetBool(bool value)
    {
        animator.SetBool(animatorParameter, value);
    }

    public void SetTrigger()
    {
        animator.SetTrigger(animatorParameter);
    }
    
    public void SetFloat(float value)
    {
        animator.SetFloat(animatorParameter, value);
    }
}