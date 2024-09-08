using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelAnimations : MonoBehaviour
{
    public static AngelAnimations Instance;

    private Animator _animator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        _animator = GetComponent<Animator>();
    }

    public void SetClickTrigger(string triggerName)
    {
        _animator.SetTrigger(triggerName);
    }
}
