using System;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using UniRx;
using UnityEngine;

public class ExplainScene : MonoBehaviour
{
    [SerializeField] private Transform pet;
    [SerializeField] private Animator petAnimator;
    [SerializeField] private Transform startPos, startJumpPos, endJumpPos;
    
    private Vector3 lookat;

    // Start is called before the first frame update
    void Start()
    {
        pet.position = startPos.position;
        lookat = startJumpPos.position;
        
        petAnimator.SetBool("running", true);
        Tween.Position(pet, startJumpPos.position, 3.0f, 0.0f, Tween.EaseLinear, Tween.LoopType.None, null, () =>
        {
            lookat = endJumpPos.position;
            petAnimator.SetTrigger("jump");
            petAnimator.SetBool("running", false);
            
            Tween.Position(pet, endJumpPos.position, 0.8f, 0.1f, Tween.EaseLinear, Tween.LoopType.None, null, () =>
            {
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(x =>
                {
                    petAnimator.SetTrigger("jump");
                });
                
                Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(x =>
                {
                    petAnimator.SetBool("cheering", true);
                    petAnimator.SetTrigger("cheer");
                });
            });
        });
    }

    // Update is called once per frame
    void Update()
    {
        lookat.y = transform.position.y;
        transform.LookAt(lookat);
    }
}
