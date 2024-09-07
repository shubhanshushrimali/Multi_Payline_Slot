using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spinner : MonoBehaviour
{
    public float reducer;
    public float multiplier = 0;
    private bool round1 = false;
    public bool isSpinning = false;
    public GameObject Button;
    public GameObject scoretext;

    void Start()
    {
        reducer = Random.Range(0.01f, 0.5f);
    }

    void FixedUpdate()
    {
        if (!isSpinning) return;

        if (multiplier > 0)
            transform.Rotate(Vector3.forward, 1 * multiplier);
        else
            isSpinning = false;

        if (multiplier < 20 && !round1)
            multiplier += 0.1f;
        else
            round1 = true;

        if (round1 && multiplier > 0)
            multiplier -= reducer;
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }

    public void Spin()
    {
        if (isSpinning) return;

        ResetWheel();
        isSpinning = true;
        Button.SetActive(false);
        scoretext.SetActive(true);
    }

    private void ResetWheel()
    {
        multiplier = 1;
        reducer = Random.Range(0.01f, 0.5f);
        round1 = false;
        isSpinning = true;
    }
}