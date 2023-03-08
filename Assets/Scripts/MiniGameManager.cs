using Microsoft.Azure.ObjectAnchors.Unity;
using Microsoft.Azure.ObjectAnchors.Unity.Sample;
using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public static MiniGameManager instance;

    // cached handle to object tracking service
    private ObjectTracker _objectTracker;
    private TrackableObjectMenu _trackableObjectMenu;

    internal Dictionary<string, GameObject> miniGames = new Dictionary<string, GameObject>();
    internal GameObject activeMiniGame = null;
    internal HashSet<GameObject> finishedMiniGames = new HashSet<GameObject>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        _trackableObjectMenu = FindObjectOfType<TrackableObjectMenu>();
        _objectTracker = ObjectTracker.Instance;
        _objectTracker.OnObjectAddedOrUpdated += StartMiniGame;
        _objectTracker.OnObjectRemoved += EndMiniGame;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        _objectTracker.OnObjectAddedOrUpdated -= StartMiniGame;
        _objectTracker.OnObjectRemoved -= EndMiniGame;
    }

    public void RegisterMiniGame(string name, GameObject miniGame)
    {
        miniGames[name] = miniGame;
        miniGame.SetActive(false);
    }

    public void StartMiniGame(IObjectAnchorsServiceEventArgs e)
    {
        if (miniGames.TryGetValue(modelNameFromArg(e), out var miniGame))
        {
            if (finishedMiniGames.Contains(miniGame)) return;
            if (activeMiniGame == null)
            {
                activeMiniGame = miniGame;
                activeMiniGame.SetActive(true);
            }
            if (activeMiniGame == miniGame)
            {
                activeMiniGame.GetComponent<IMiniGameTransformer>().UpdateTransform(e);
            }
        }
    }

    private string modelNameFromArg(IObjectAnchorsServiceEventArgs e)
    {
        var filePath = Microsoft.Azure.ObjectAnchors.Unity.Sample.TrackableObjectDataLoader.Instance.TrackableObjectDataFromId(e.ModelId).ModelFilePath;
        return Path.GetFileNameWithoutExtension(filePath);
    }

    public void EndMiniGame(IObjectAnchorsServiceEventArgs e)
    {
        EndMiniGame(modelNameFromArg(e));
    }

    public void EndMiniGame(string name, bool endNormaly = false)
    {
        if (miniGames.TryGetValue(name, out var miniGame))
        {
            if (activeMiniGame == miniGame)
            {
                EndMiniGame(endNormaly);
            }
        }
    }

    public void EndMiniGame(bool endNormaly = false)
    {
        activeMiniGame.SetActive(false);
        if (endNormaly)
        {
            StartCoroutine(MinigameReadyAfterSeconds(activeMiniGame, 60));
        }
        activeMiniGame = null;
    }

    public void Speak(string text)
    {
        _trackableObjectMenu.Speak(text);
    }

    IEnumerator MinigameReadyAfterSeconds(GameObject miniGame, float seconds)
    {
        finishedMiniGames.Add(miniGame);
        yield return new WaitForSeconds(seconds);
        finishedMiniGames.Remove(miniGame);
        yield return default;
    }
}
