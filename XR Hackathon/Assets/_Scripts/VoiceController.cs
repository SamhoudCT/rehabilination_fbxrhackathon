using System;
using Crosstales.RTVoice;
using Facebook.WitAi;
using Facebook.WitAi.Lib;
using Oculus.Voice;
using UniRx;
using UnityEngine;

public class VoiceController : MonoBehaviour
{
    //Static cingleton instance
    public static VoiceController Instance;
    
    [HideInInspector] public ReactiveProperty<bool> voicesReady = new ReactiveProperty<bool>();

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator charAnim;
    [SerializeField] private AppVoiceExperience voiceExperience;
    [SerializeField] private PetController petController;

    private bool voiceReady;
    private ReactiveProperty<bool> isHandlingRequest = new ReactiveProperty<bool>();

    public bool askedPatientStatus;
    private bool askedExercise;
    private bool askedGame;
    private IDisposable timer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        Speaker.Instance.OnVoicesReady += SpeakerVoicesReady;
        
        voiceExperience.events.OnRequestCreated.AddListener(OnUserVoiceRequest);
        voiceExperience.events.OnResponse.AddListener(OnUserVoiceReponse);
        voiceExperience.events.OnStartListening.AddListener(OnStartListening);
        voiceExperience.events.OnStoppedListening.AddListener(OnEndListening);
        voiceExperience.events.OnStoppedListeningDueToDeactivation.AddListener(OnEndListening);
        voiceExperience.events.OnStoppedListeningDueToInactivity.AddListener(OnEndListening);
        voiceExperience.events.OnStoppedListeningDueToTimeout.AddListener(OnEndListening);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) 
            || (askedPatientStatus && !askedExercise && !askedGame && OVRInput.GetDown(OVRInput.Button.One)))
        {
            CommandFeelingGood();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)
                 || (askedPatientStatus && askedExercise && !askedGame && OVRInput.GetDown(OVRInput.Button.One)))
        {
            CommandDoExercise();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)
                 || (askedPatientStatus && askedExercise && askedGame && OVRInput.GetDown(OVRInput.Button.One)))
        {
            CommandPlayGame1();
        }
    }

    public void Speak(string text)
    {
        if(voicesReady.Value)
            Speaker.Instance.Speak(text, audioSource, Speaker.Instance.VoicesForCulture("en-US")[15], true, 1f, 1f);
    }

    public void CommandHello()
    {
        petController.React("Hi there!");
    }
    
    public void CommandReset()
    {
        askedExercise = false;
        askedPatientStatus = false;
        petController.React("Ok, I will reset everything");
        GameController.Instance.Reset();

        Observable.Timer(TimeSpan.FromMilliseconds(2000)).Subscribe(x =>
        {
            petController.StartSequences();
        }).AddTo(this);
    }

    public void CommandFeelingBad()
    {
        if (askedPatientStatus)
        {
            petController.React("I am sorry, let me try to cheer you up. Want to play a game?");
            askedExercise = true;
        }
    }
    
    public void CommandFeelingGood()
    {
        if (askedPatientStatus)
        {
            petController.React("Great! Ready for a challenge?");
            petController.animator.SetTrigger("jump");
            askedExercise = true;
        }
    }
    
    public void CommandDoExercise()
    {
        if (askedExercise)
        {
            petController.React("Faaaantastic, let’s go!");
            petController.animator.SetTrigger("jump");
            
            Observable.Timer(TimeSpan.FromMilliseconds(2000)).Subscribe(x =>
            {
                petController.React("What would you like to do today?");
                askedGame = true;
            }).AddTo(this);
        }
    }
    
    public void CommandJump()
    {
        charAnim.SetTrigger("Jump");
        petController.React("WEEEEEEE");
    }
    
    public void CommandSleep()
    {
        charAnim.SetTrigger("Die");
        petController.React("Goodnight");
    }

    public void CommandWhoAreYou()
    {
        petController.SlimShady();
    }

    public void CommandPlayGame1()
    {
        petController.React("Great, let’s go nuts!");
        petController.animator.SetTrigger("jump");
        
        Observable.Timer(TimeSpan.FromMilliseconds(1800)).Subscribe(x =>
        {
            GameController.Instance.StartGame();
        }).AddTo(this);
    }

    void OnUserVoiceRequest(WitRequest request)
    {
        isHandlingRequest.Value = true;
    }

    void OnUserVoiceReponse(WitResponseNode response)
    {
        isHandlingRequest.Value = false;
    }
    
    void OnStartListening()
    {
        Debug.Log("Listening started");
    }
    
    void OnEndListening()
    {
        Debug.Log("Listening stopped");

        if (!isHandlingRequest.Value)
            ActivateVoiceListening();
        else
        {
            isHandlingRequest.Subscribe(handlingRequest =>
            {
                if (!handlingRequest)
                    ActivateVoiceListening();
            });
        }
    }

    public void ActivateVoiceListening()
    {
        if(voiceExperience.Active)
            return;
        
        timer?.Dispose();
        timer = Observable.Timer(TimeSpan.FromMilliseconds(200)).Subscribe(x =>
        {
            if (!isHandlingRequest.Value)
                voiceExperience.Activate();
        });
    }
    
    void SpeakerVoicesReady()
    {
        Observable.NextFrame().Subscribe(x =>
        {
            voicesReady.Value = true;
        });
    }
}
