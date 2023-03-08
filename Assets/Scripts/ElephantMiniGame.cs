using Microsoft.Azure.ObjectAnchors.Unity;
using Microsoft.MixedReality.OpenXR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElephantMiniGame : MonoBehaviour, IMiniGameTransformer
{
    internal bool gameStarted = false;
    private Coroutine flashRoutine = null;
    [SerializeField] private Vector3 bananaPosition;
    [SerializeField] private GameObject elephantTransform;
    [SerializeField] private MeshRenderer elephant;
    [SerializeField] private BoxCollider bananaCollider;
    [SerializeField] private BoxCollider elephantMouse;
    private int feedCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        MiniGameManager.instance.RegisterMiniGame("elephant", gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        flashRoutine = StartCoroutine(StartRoutine());
        gameStarted = true;
    }

    private void OnDisable()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        gameStarted = false;
    }

    IEnumerator StartRoutine()
    {
        MiniGameManager.instance.Speak("Here is an elephant. Let's feed some bananas to it for three times");
        feedCount = 0;
        bananaPosition = bananaCollider.transform.localPosition;
        yield return default;
    }

    public void UpdateTransform(IObjectAnchorsServiceEventArgs e)
    {
        transform.position = e.Location.Value.Position;
        elephantTransform.transform.SetPositionAndRotation(e.Location.Value.Position, e.Location.Value.Orientation);
        elephantTransform.transform.localScale = e.ScaleChange;
    }

    public void ManipulationEnded()
    {
        if (gameStarted && bananaCollider.bounds.Intersects(elephantMouse.bounds))
        { 
            MiniGameManager.instance.Speak("elephant is happy");
            bananaCollider.transform.localPosition = bananaPosition;
            feedCount++;
        }

        if (feedCount == 3)
        {
            StartCoroutine(EndGameRoutine());
        }
    }

    IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        MiniGameManager.instance.Speak("Thank you for feeding elephant");
        MiniGameManager.instance.EndMiniGame(true);
        yield return default;
    }
}
