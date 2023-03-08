using Microsoft.Azure.ObjectAnchors.Unity;
using Microsoft.MixedReality.OpenXR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedMiniGame : MonoBehaviour, IMiniGameTransformer
{
    internal bool gameStarted = false;
    private Coroutine flashRoutine = null;
    [SerializeField] private Vector3 wateringcanPosition;
    [SerializeField] private GameObject seedTransform;
    [SerializeField] private MeshRenderer seed;
    [SerializeField] private MeshRenderer tree1;
    [SerializeField] private MeshRenderer tree2;
    [SerializeField] private MeshRenderer tree3;
    [SerializeField] private MeshRenderer tree4;
    [SerializeField] private BoxCollider wateringcanCollider;
    [SerializeField] private BoxCollider seedCollider;
    private int waterCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        MiniGameManager.instance.RegisterMiniGame("seed", gameObject);
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
        MiniGameManager.instance.Speak("Here is a seed. Let's water it");
        waterCount = 0;
        wateringcanPosition = wateringcanCollider.transform.localPosition;
        seed.enabled = true;
        tree1.enabled = false;
        tree2.enabled = false;
        tree3.enabled = false;
        tree4.enabled = false;
        yield return default;
    }

    public void UpdateTransform(IObjectAnchorsServiceEventArgs e)
    {
        transform.position = e.Location.Value.Position;
        seedTransform.transform.SetPositionAndRotation(e.Location.Value.Position, e.Location.Value.Orientation);
        seedTransform.transform.localScale = e.ScaleChange;
    }

    /*    public void ManipulationEnded()
        {
            if (gameStarted && wateringcanCollider.bounds.Intersects(seedCollider.bounds))
            {
                MiniGameManager.instance.Speak("seed is growing");
                wateringcanCollider.transform.localPosition = wateringcanPosition;
                waterCount++;
            }

            if (waterCount == 3)
            {
                StartCoroutine(EndGameRoutine());
            }
        }
    */

    public void ManipulationEnded()
    {
        if (gameStarted && wateringcanCollider.bounds.Intersects(seedCollider.bounds))
        {
//            MiniGameManager.instance.Speak("seed is growing");
            wateringcanCollider.transform.localPosition = wateringcanPosition;
            waterCount++;
        }

        if (waterCount == 1)
        {
            MiniGameManager.instance.Speak("the seed is growing into a sapling");
            seed.enabled = false;
            tree1.transform.localScale = tree1.transform.localScale;
            tree1.enabled = true;
            StartCoroutine(TreeGrow(tree1.gameObject));
        }

        else if (waterCount == 2)
        {
            MiniGameManager.instance.Speak("the sapling is growing into a tree");
            // StartCoroutine(TreeGrow(tree1.gameObject));
            StartCoroutine(TreeGrowWrapper(tree1, tree2));
        }

        else if (waterCount == 3)
        {
            MiniGameManager.instance.Speak("the tree is growing into a big tree");
            StartCoroutine(TreeGrowWrapper(tree2, tree3));
        }

        else if (waterCount == 4)
        {
            MiniGameManager.instance.Speak("the big tree bears fruits");
            tree3.enabled = false;
            tree4.enabled = true;
        }

        else if (waterCount == 5)
        {
            StartCoroutine(EndGameRoutine());
        }
    }

    IEnumerator EndGameRoutine()
    {

        MiniGameManager.instance.Speak("Education is like watering a tree. It's an investment for the future. N U S Giving provides opportunities for every young student.");
        yield return new WaitForSeconds(8.0f);
        MiniGameManager.instance.EndMiniGame(true);
        yield return default;
    }

    IEnumerator TreeGrow(GameObject o)
    {
        var scale = o.transform.localScale;
        var startTime = Time.time;
        while (Time.time - startTime < 5.0f)
        {
            var deltaTime = Time.time - startTime;
            var deltaScale = 1.0f + deltaTime / 5.0f;
            o.transform.localScale = scale * deltaScale;
            yield return null;
        }
        yield return default;
    }

    IEnumerator TreeGrowWrapper(MeshRenderer o, MeshRenderer o_ob)
    {
        yield return StartCoroutine(TreeGrow(o.gameObject));
        o.enabled = false;
        o_ob.enabled = true;
        yield return new WaitForSeconds(5.0f);
    }
}
