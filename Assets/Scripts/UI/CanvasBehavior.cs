using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasBehavior : MonoBehaviour
{
    [SerializeField] private GameObject _fadeToBlack;
    private Animator _anim;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Sets the fade to black image to on/off, 1/0. Called by animation event
    /// </summary>
    /// <param name="input"></param> om = 1, off = 0
    public void EnableFTB(int input)
    {
        if(input == 1) 
        {
            _fadeToBlack.SetActive(true);
        }
        else
        {
            _fadeToBlack.SetActive(false);
        }
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Q))
    //    {
    //        _anim.SetTrigger("FadeToBlack");
    //    }
    //}
}
