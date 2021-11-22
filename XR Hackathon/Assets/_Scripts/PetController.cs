using System;
using Pixelplacement;
using TMPro;
using UniRx;
using UnityEngine;

public class PetController : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private TMP_Text interactionText;
    [SerializeField] private VoiceController voiceController;
    [SerializeField] private GameObject textBalloon;
    [SerializeField] private GameObject vrHeadset;
    [SerializeField] private Transform startPos, startJumpPos, endJumpPos, endPos;

    private Vector3 lookat;
    
    void Start()
    {
        transform.position = startPos.position;
        lookat = startJumpPos.position;
        
        animator.SetBool("running", true);
        Tween.Position(transform, startJumpPos.position, 3.0f, 0.0f, Tween.EaseLinear, Tween.LoopType.None, null, () =>
        {
            lookat = endJumpPos.position;
            animator.SetTrigger("jump");
            animator.SetBool("running", false);
            
            Tween.Position(transform, endJumpPos.position, 0.9f, 0.1f, Tween.EaseLinear, Tween.LoopType.None, null, () =>
            {
                lookat = endPos.position;
                animator.SetBool("running", true);
                Tween.Position(transform, endPos.position, 2.0f, 0.0f, Tween.EaseLinear, Tween.LoopType.None, null, () =>
                {
                    animator.SetBool("running", false);
                    lookat = vrHeadset.transform.position;
                    
                    if(voiceController.voicesReady.Value)
                        StartSequences();
                    else
                        voiceController.voicesReady.Subscribe(x =>
                        {
                            if (x)
                            {
                                StartSequences();
                            }
                        }).AddTo(this);
                });

            });
        });
    }

    void Update()
    {
        lookat.y = transform.position.y;
        transform.LookAt(lookat);
    }

    public void StartSequences()
    {
        React("Good morning champ!");

        Observable.Timer(TimeSpan.FromMilliseconds(2500)).Subscribe(x =>
        {
            React("How are you feeling?");
            voiceController.askedPatientStatus = true;
            voiceController.ActivateVoiceListening();
        });
    }

    public void React(string message)
    {
        textBalloon.SetActive(false);
        Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(x =>
        {
            textBalloon.SetActive(true);
            UpdateTextBalloon(message);
            TextToSpeech(message);
        }).AddTo(this);
    }

    void UpdateTextBalloon(string textMessage)
    {
        interactionText.text = textMessage;
    }

    void TextToSpeech(string message)
    {
        voiceController.Speak(message);
    }
    
    public void SlimShady()
    {
        React("Hi! My name is...");
        Observable.Timer(TimeSpan.FromMilliseconds(1800)).Subscribe(x =>
        {
            React("What?, my name is...");
            Observable.Timer(TimeSpan.FromMilliseconds(1800)).Subscribe(x =>
            {
                React("Who?, my name is...");
                Observable.Timer(TimeSpan.FromMilliseconds(1800)).Subscribe(x =>
                {
                    React("Chika-chika Reco-Very");
                }).AddTo(this);
            }).AddTo(this);
        }).AddTo(this);
    }
}
