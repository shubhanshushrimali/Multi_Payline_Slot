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

    private Dictionary<Sprite, int> symbolScores;

    void Start()
    {
        InitializeSymbolScores();
        UpdateScoreText();
    }

    private void InitializeSymbolScores()
    {
        symbolScores = new Dictionary<Sprite, int>();

        foreach (Sprite symbol in spriteOptions)
        {
        
            if (symbol.name == "cherry") symbolScores[symbol] = 100;
            else if (symbol.name == "bar") symbolScores[symbol] = 50;
            else if (symbol.name == "orange") symbolScores[symbol] = 75;
            else if (symbol.name == "bell") symbolScores[symbol] = 200;
            else if (symbol.name == "seven") symbolScores[symbol] = 500;
            else if (symbol.name == "graps") symbolScores[symbol] = 500;
           
        }
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

        for (int i = 1; i < rowSprites.Length; i++)
        {
            if (rowSprites[i] == lastSprite)
            {
                matchCount++;
            }
            else
            {
                if (matchCount >= 3)
                {
                    AddScore(lastSprite, matchCount);
                }
                matchCount = 1;
                lastSprite = rowSprites[i];
            }
        }

 
        if (matchCount >= 3)
        {
            AddScore(lastSprite, matchCount);
        }
    }

    public void AddScore(Sprite symbol, int matchCount)
    {
        if (symbolScores.TryGetValue(symbol, out int baseScore))
        {
            int finalScore = baseScore;

            if (matchCount == 4)
            {
                finalScore *= 2;
            }
            else if (matchCount == 5)
            {
                finalScore *= 3;
            }

            currentScore += finalScore;
            Debug.Log($"Matched {matchCount} {symbol.name}(s). Score: {finalScore}. Total Score: {currentScore}");
            UpdateScoreText();
        }
        else
        {
            Debug.LogWarning("Symbol not found in score list.");
        }
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