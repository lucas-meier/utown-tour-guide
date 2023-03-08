using Microsoft.Azure.ObjectAnchors.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NUSMiniGame : MonoBehaviour, IMiniGameTransformer
{
    [SerializeField] private GameObject nusTransform;
    [SerializeField] private List<GameObject> wList;
    [SerializeField] private List<GameObject> eList;
    [SerializeField] private List<GameObject> loveList;

    private Coroutine startRoutine = null;
    private bool gameStarted;
    private GameObject w;
    private GameObject e;
    private GameObject love;
    [SerializeField] private GameObject NUS;
    private HashSet<GameObject> alignedObject = new HashSet<GameObject>();
    public void UpdateTransform(IObjectAnchorsServiceEventArgs e)
    {
        transform.SetPositionAndRotation(e.Location.Value.Position, e.Location.Value.Orientation);
        transform.localScale = e.ScaleChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        MiniGameManager.instance.RegisterMiniGame("nus", gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        gameStarted = true;
        startRoutine = StartCoroutine(StartRoutine());
    }

    IEnumerator StartRoutine()
    {
        MiniGameManager.instance.Speak("Let's make. We love N U S in a line");
        w = wList[UnityEngine.Random.Range(0, wList.Count)];
        e = eList[UnityEngine.Random.Range(0, eList.Count)];
        love = loveList[UnityEngine.Random.Range(0, loveList.Count)];
        w.SetActive(true);
        e.SetActive(true);
        love.SetActive(true);
        alignedObject.Clear();
        yield return default;
    }

    private void OnDisable()
    {
        gameStarted = false;
        if (startRoutine != null)
        {
            StopCoroutine(startRoutine);
        }
        foreach (var o in wList) o.SetActive(false);
        foreach (var o in eList) o.SetActive(false);
        foreach (var o in loveList) o.SetActive(false);
    }

    public void ManipulationEnded()
    {
        CheckInALine();
    }

    void CheckInALine()
    {
        if (gameStarted)
        {
            if (alignedObject.Contains(w))
            {
                if (!RoughlyInALine(w))
                {
                    MiniGameManager.instance.Speak("w is not aligned");
                    alignedObject.Remove(w);
                }
            }
            else if (RoughlyInALine(w))
            {
                MiniGameManager.instance.Speak("w is aligned");
                alignedObject.Add(w);
            }

            if (alignedObject.Contains(e))
            {
                if (!RoughlyInALine(e))
                {
                    MiniGameManager.instance.Speak("e is not aligned");
                    alignedObject.Remove(e);
                }
            }
            else if (RoughlyInALine(e))
            {
                MiniGameManager.instance.Speak("e is aligned");
                alignedObject.Add(e);
            }

            if (alignedObject.Contains(love))
            {
                if (!RoughlyInALine(love))
                {
                    MiniGameManager.instance.Speak("love is not aligned");
                    alignedObject.Remove(love);
                }
            }
            else if (RoughlyInALine(love))
            {
                MiniGameManager.instance.Speak("love is aligned");
                alignedObject.Add(love);
            }

            if (alignedObject.Count == 3)
            {
                StartCoroutine(EndRoutine());
            }
        }
    }

    IEnumerator EndRoutine()
    {
        MiniGameManager.instance.Speak("Yes!!! We love N U S.");
        yield return new WaitForSeconds(5.0f);
        MiniGameManager.instance.Speak("Thank you for playing");
        MiniGameManager.instance.EndMiniGame(true);
    }

    bool RoughlyInALine(GameObject o)
    {
        var vec1 = o.transform.position - NUS.transform.position;
        var vec2 = -NUS.transform.forward;
        if (Vector3.Angle(vec1, vec2) < 60.0f) return true;
        else return false;
    }
}
