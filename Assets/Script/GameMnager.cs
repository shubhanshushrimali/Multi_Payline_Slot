using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMnager : MonoBehaviour
{
    public Sprite[] spriteOptions;
    public GameObject[] spriteRenderers;
    public float minSpinTime = 4f;
    public float maxSpinTime = 6f;

    public int currentScore = 0;
    public TextMeshProUGUI scoreText;

    public int fiveInRowScore = 100;
    public int fourInRowScore = 50;
    public int threeInRowScore = 25;

    void Start()
    {
        UpdateScoreText();
    }

    public void StartSpin()
    {
        StartCoroutine(SpinAllSprites());
    }

    private IEnumerator SpinAllSprites()
    {
        float spinDuration = Random.Range(minSpinTime, maxSpinTime);

        foreach (GameObject spriteObject in spriteRenderers)
        {
            StartCoroutine(SpinSprite(spriteObject, spinDuration));
        }

        yield return new WaitForSeconds(spinDuration);
        CalculateScore();
    }

    private IEnumerator SpinSprite(GameObject spriteObject, float duration)
    {
        SpriteRenderer spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            spriteRenderer.sprite = spriteOptions[Random.Range(0, spriteOptions.Length)];
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void CalculateScore()
    {
        GameObject[,] slots = new GameObject[3, 5];
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                slots[row, col] = spriteRenderers[row * 5 + col];
            }
        }

        CheckRows(slots);
    }

    private void CheckRows(GameObject[,] slots)
    {
        for (int row = 0; row < 3; row++)
        {
            Sprite[] rowSprites = new Sprite[5];
            for (int col = 0; col < 5; col++)
            {
                rowSprites[col] = slots[row, col].GetComponent<SpriteRenderer>().sprite;
            }

            CheckRow(rowSprites);
        }
    }

    private void CheckRow(Sprite[] rowSprites)
    {
        int matchCount = 1;
        Sprite lastSprite = rowSprites[0];
        int maxMatchCount = 1;

        for (int i = 1; i < rowSprites.Length; i++)
        {
            if (rowSprites[i] == lastSprite)
            {
                matchCount++;
            }
            else
            {
                if (matchCount > maxMatchCount)
                {
                    maxMatchCount = matchCount;
                }
                matchCount = 1;
                lastSprite = rowSprites[i];
            }
        }

        if (matchCount > maxMatchCount)
        {
            maxMatchCount = matchCount;
        }

        if (maxMatchCount >= 5)
        {
            AddScore(fiveInRowScore);
        }
        else if (maxMatchCount == 4)
        {
            AddScore(fourInRowScore);
        }
        else if (maxMatchCount == 3)
        {
            AddScore(threeInRowScore);
        }
    }

    public void AddScore(int score)
    {
        currentScore += score;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreText();
    }
}