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

    public TextMeshProUGUI scoreText;
    public GameObject bonusButton;  

    private Dictionary<Sprite, int> symbolScores;

    void Start()
    {
        InitializeSymbolScores();
        UpdateScoreText(0);
        bonusButton.SetActive(false);  
    }

    private void InitializeSymbolScores()
    {
        symbolScores = new Dictionary<Sprite, int>();

        foreach (Sprite symbol in spriteOptions)
        {
            if (symbol.name == "cherry") symbolScores[symbol] = 100;
            else if (symbol.name == "grapes") symbolScores[symbol] = 150;
            else if (symbol.name == "orange") symbolScores[symbol] = 125;
            else if (symbol.name == "banana") symbolScores[symbol] = 175;
            else if (symbol.name == "bell") symbolScores[symbol] = 200; 
            else if (symbol.name == "seven") symbolScores[symbol] = 500;
            else if (symbol.name == "bar") symbolScores[symbol] = 700;
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

        Sprite lastSprite = null;

        while (elapsedTime < duration)
        {
            Sprite selectedSprite;
            float chance = Random.Range(0f, 1f);

            if (lastSprite != null && chance < 0.6f)
            {
                selectedSprite = lastSprite;
            }
            else
            {
                selectedSprite = GetBiasedRandomSprite();
            }

            spriteRenderer.sprite = selectedSprite;
            lastSprite = selectedSprite;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private Sprite GetBiasedRandomSprite()
    {
        float[] weights = new float[spriteOptions.Length];

        for (int i = 0; i < spriteOptions.Length; i++)
        {
            switch (spriteOptions[i].name)
            {
                case "cherry": weights[i] = 0.3f; break;
                case "grapes": weights[i] = 0.25f; break;
                case "orange": weights[i] = 0.15f; break;
                case "banana": weights[i] = 0.1f; break;
                case "bell": weights[i] = 0.2f; break;
                case "seven": weights[i] = 0.05f; break;
                case "bar": weights[i] = 0.1f; break;
            }
        }

        float totalWeight = 0f;
        foreach (float weight in weights)
        {
            totalWeight += weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;
        for (int i = 0; i < spriteOptions.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight)
            {
                return spriteOptions[i];
            }
        }

        return spriteOptions[0];
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

        int newScore = CheckRows(slots);
        UpdateScoreText(newScore);
    }

    private int CheckRows(GameObject[,] slots)
    {
        int totalScore = 0;

        for (int row = 0; row < 3; row++)
        {
            Sprite[] rowSprites = new Sprite[5];
            for (int col = 0; col < 5; col++)
            {
                rowSprites[col] = slots[row, col].GetComponent<SpriteRenderer>().sprite;
            }

            totalScore += CalculateRowScore(rowSprites);
        }

        return totalScore;
    }

    private int CalculateRowScore(Sprite[] rowSprites)
    {
        Dictionary<Sprite, int> matchCounts = new Dictionary<Sprite, int>();

        foreach (Sprite sprite in rowSprites)
        {
            if (matchCounts.ContainsKey(sprite))
            {
                matchCounts[sprite]++;
            }
            else
            {
                matchCounts[sprite] = 1;
            }
        }

        int rowScore = 0;
        bool bonusConditionMet = false;  

        foreach (var match in matchCounts)
        {
            int matchCount = match.Value;
            Sprite symbol = match.Key;

            if (matchCount >= 3 && symbolScores.TryGetValue(symbol, out int baseScore))
            {
                if (matchCount == 4)
                {
                    rowScore += baseScore * 2;
                }
                else if (matchCount == 5)
                {
                    rowScore += baseScore * 3;
                }
                else
                {
                    rowScore += baseScore;
                }

           
                if (symbol.name == "bell" && matchCount >= 3)
                {
                    bonusConditionMet = true;
                }
            }
        }

    
        if (bonusConditionMet)
        {
            ActivateBonusButton();
        }

        return rowScore;
    }

    private void ActivateBonusButton()
    {
        bonusButton.SetActive(true); 
    }

    private void UpdateScoreText(int newScore)
    {
        scoreText.text = "Score: " + newScore;
    }

    public void ResetScore()
    {
        UpdateScoreText(0);
    }

    public void BnousRound()
    {
        SceneManager.LoadScene(1); 
    }
}