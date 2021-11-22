using System;
using System.Collections.Generic;
using Pixelplacement;
using Pixelplacement.TweenSystem;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    //Inspector variables
    [SerializeField] private TMP_Text debugText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image timerProgress;
    [SerializeField] private PetController pet;
    [SerializeField] private GameObject trees;
    [SerializeField] private GameObject bucket;
    [SerializeField] private GameObject coconutPrefab;
    [SerializeField] private Collider lefthandTrigger;
    [SerializeField] private Collider righthandTrigger;
    [SerializeField] private List<Transform> gamePositions;

    //Private variables
    private bool touchingLeft, touchingRight, holdingCoconut;
    private Transform controllerToFollow;
    private GameObject coconut;
    private int score = 0;
    private IDisposable posSwitchTimer;
    private TweenBase timerTween;

    public void Awake()
    {
        //Disable all the game elements.
        trees.SetActive(false);
        bucket.SetActive(false);
        timerProgress.fillAmount = 0;

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SubscribeTriggers();
    }

    private void Update()
    {
        if (touchingRight && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            debugText.text = "Spawn coconut on right hand";
            controllerToFollow = righthandTrigger.transform;
            CreatAndFollowCoconut();
        }
        
        if (touchingLeft && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            debugText.text = "Spawn coconut on left hand";
            controllerToFollow = lefthandTrigger.transform;
            CreatAndFollowCoconut();
        }

        if (holdingCoconut && coconut != null)
        {
            coconut.transform.position = controllerToFollow.transform.position;
            
            if(controllerToFollow == lefthandTrigger.transform && OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch)
               || controllerToFollow == righthandTrigger.transform && OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {
                debugText.text = "Drop the coconut!";
                DropCoconut();
            }
        }
    }

    public void StartGame()
    {
        int i = 0;
        Tween.Position(pet.transform, gamePositions[i].position, 0.4f, 0.0f, Tween.EaseLinear, Tween.LoopType.None, null, () =>
        {
            //Enable all the game elements.
            trees.SetActive(true);
            bucket.SetActive(true);
            timerProgress.fillAmount = 0;

            pet.React("Try to reach for the coconuts with your controllers.");
            Observable.Timer(TimeSpan.FromMilliseconds(2800)).Subscribe(x =>
            {
                pet.React("When you touch them, hold the trigger to take a coconut from the tree.");
                
                Observable.Timer(TimeSpan.FromMilliseconds(3200)).Subscribe(y =>
                {
                    pet.React("Try to throw as many coconuts as you can, into my bucket!");

                    timerTween?.Stop();
                    timerTween = Tween.Value(0f, 1f, f => timerProgress.fillAmount = f, 60f, 0);
                    Observable.Timer(TimeSpan.FromSeconds(60.5f)).Subscribe(x =>
                    {
                        Endgame();
                    });
                    
                    //Start interval timer to reposition the pet
                    posSwitchTimer?.Dispose();
                    posSwitchTimer = Observable.Interval(TimeSpan.FromSeconds(12)).Subscribe(time =>
                    {
                        i++;
                        if (i > gamePositions.Count - 1) i = 0;
                        
                        Tween.Position(pet.transform, gamePositions[i].position, 0.2f, 0.0f, Tween.EaseLinear);
                    });
                }).AddTo(this);
            }).AddTo(this);
        });
    }

    public void AddScore()
    {
        //Play bucket sound
        AudioController.Instance.PlayBucketSound();
        
        score++;
        scoreText.text = score + "x";

        if (score == 1)
            pet.React("Yes, well done! Keep going!");
        
        if (score == 5)
            pet.React("Wooohooo! Your are doing great!");
        
        if(score == 10)
            pet.React("Amazing!");
    }

    private void Endgame()
    {
        pet.React("Yeeaaahhhh!");
        Observable.Timer(TimeSpan.FromSeconds(4.5f)).Subscribe(x =>
        {
            pet.React("Well done champ! That’s even more than yesterday!");
        });
        
        //Play victory sound
        AudioController.Instance.PlayVictorySound();

        //Play cheering animation
        pet.animator.SetBool("cheering", true);
        pet.animator.SetTrigger("cheer");

        timerTween?.Finish();
        Observable.Timer(TimeSpan.FromMilliseconds(200)).Subscribe(x =>
        {
            bucket.SetActive(false);
        });
    }

    public void Reset()
    {
        timerTween?.Stop();
        posSwitchTimer?.Dispose();
        score = 0;
        scoreText.text = score + "x";
        
        //Disable all the game elements.
        trees.SetActive(false);
        bucket.SetActive(false);
        timerProgress.fillAmount = 0;
    }

    private void CreatAndFollowCoconut()
    {
        DropCoconut(false);  //Drop if already holding;
        
        AudioController.Instance.PlayGrabSound();
        
        coconut = Instantiate(coconutPrefab, controllerToFollow.position, Quaternion.identity);
        holdingCoconut = true;
    }

    private void DropCoconut(bool playSound = true)
    {
        if (coconut == null) return;
        else
        {
            Rigidbody rigidbody = coconut.GetComponent<Rigidbody>();
            rigidbody.useGravity = true;
            
            if (controllerToFollow == lefthandTrigger.transform)
            {
                rigidbody.velocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
                rigidbody.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.LTouch);
            }
            else if (controllerToFollow == righthandTrigger.transform)
            {
                rigidbody.velocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
                rigidbody.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
            }

            rigidbody.AddForce(controllerToFollow.forward * 1.5f, ForceMode.Impulse);
            
            //Play throwing sound
            if(rigidbody.velocity.magnitude > 0.1 && playSound)
                AudioController.Instance.PlayThrowSound();

            holdingCoconut = false;
        }
    }

    private void SubscribeTriggers()
    {
        lefthandTrigger.OnTriggerEnterAsObservable().Subscribe(collider =>
        {
            if (collider.CompareTag("TreeLeft") || collider.CompareTag("TreeRight"))
            {
                touchingLeft = true;
                debugText.text = "Touching coconut with left hand";
            }
        }).AddTo(this);
        
        lefthandTrigger.OnTriggerExitAsObservable().Subscribe(collider =>
        {
            if (collider.CompareTag("TreeLeft") || collider.CompareTag("TreeRight"))
                touchingLeft = false;
        }).AddTo(this);
        
        righthandTrigger.OnTriggerEnterAsObservable().Subscribe(collider =>
        {
            if (collider.CompareTag("TreeLeft") || collider.CompareTag("TreeRight"))
            {
                touchingRight = true;
                debugText.text = "Touching coconut with right hand";
            }
        }).AddTo(this);
        
        righthandTrigger.OnTriggerExitAsObservable().Subscribe(collider =>
        {
            if (collider.CompareTag("TreeLeft") || collider.CompareTag("TreeRight"))
                touchingRight = false;
        }).AddTo(this);
    }
}
