using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorDelay : MonoBehaviour
{
    private RuntimeAnimatorController _animatorController;

    void Start()
    {
        _animatorController = GetComponent<Animator>().runtimeAnimatorController;
        GetComponent<Animator>().runtimeAnimatorController = null;
        StartCoroutine(_reloadStoryObjects());
    }

    private IEnumerator _reloadStoryObjects()
    {
        yield return new WaitForSeconds(0.2f);
        GetComponent<Animator>().runtimeAnimatorController = _animatorController;
    }
}
