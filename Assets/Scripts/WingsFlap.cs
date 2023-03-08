using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingsFlap : MonoBehaviour
{
    [SerializeField] private ElephantMiniGame elephantGame;
    [SerializeField] private float deltaZ;
    [SerializeField] private float frequency;
    private int countI = 0;
    private Quaternion startRotation;
    // Start is called before the first frame update
    void Start()
    {
        startRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (elephantGame.gameStarted)
        {
            countI++;
            transform.localRotation = Quaternion.Euler(0, 0, deltaZ * Mathf.Abs(Mathf.Sin(frequency * countI))) * startRotation;
        }
    }
}
