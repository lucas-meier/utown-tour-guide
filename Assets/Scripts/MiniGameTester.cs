using System.Collections;
using UnityEngine;

public class MiniGameTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Test());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator Test()
    {
        yield return new WaitForSeconds(2.0f);
        MiniGameManager.instance.activeMiniGame = gameObject;
        MiniGameManager.instance.Speak("Hello. Here is a play test program. You can experience the mini games. But there are more features in the real application, like geo location and object recognition.");
        yield return new WaitForSeconds(15.0f);
        while (MiniGameManager.instance.miniGames.Count != 3)
        {
            yield return null;
        }
        MiniGameManager.instance.Speak("Ready to get started");
        yield return new WaitForSeconds(5.0f);
        yield return playTest("nus");
        yield return playTest("elephant");
        yield return playTest("seed");

        MiniGameManager.instance.Speak("Thank you for play testing. Now you can exit the application.");
        MiniGameManager.instance.activeMiniGame = null;
        yield return default;
    }

    IEnumerator playTest(string gameName)
    {
        var miniGame = MiniGameManager.instance.miniGames[gameName];
        MiniGameManager.instance.activeMiniGame = miniGame;
        miniGame.SetActive(true);
        SetPositionAndRotation(ref miniGame, gameName);
        while (miniGame.activeSelf)
        {
            yield return null;
        }
        MiniGameManager.instance.activeMiniGame = gameObject;
        yield return new WaitForSeconds(5.0f);
        yield return default;
    }

    void SetPositionAndRotation(ref GameObject miniGame, string gameName)
    {
        var cameraDirection = Camera.main.transform.forward;
        cameraDirection.y = 0;
        cameraDirection = cameraDirection.normalized;
        miniGame.transform.position = Camera.main.transform.position + cameraDirection * 2.0f - new Vector3(0, 1.0f, 0);
        if (gameName == "nus")
        {
            miniGame.transform.rotation = Quaternion.LookRotation(cameraDirection, Vector3.up);
            miniGame.transform.rotation *= Quaternion.Euler(-90, 0, 0);
        }
    }
}
