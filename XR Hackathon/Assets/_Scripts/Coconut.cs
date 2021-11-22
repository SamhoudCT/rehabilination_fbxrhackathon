using System;
using UnityEngine;

public class Coconut : MonoBehaviour
{
    private bool collected = false;

    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        
        if (other.CompareTag("Bucket"))
        {
            collected = true;
            GameController.Instance.AddScore();
            gameObject.SetActive(false);
        }
    }
}
