using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Financial State
    [Header("Financial Settings")]
    public int startingMoney = 1000;
    public int currentMoney;
    public int longTermSavings;

    // Game Progress
    [Header("Game Progress")]
    [SerializeField] private int decisionsMade;
    private const int TOTAL_DECISIONS = 20;

    // Debugging
    [Header("Debug")]
    [SerializeField] private bool logTransactions = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        currentMoney = startingMoney;
        longTermSavings = 0;
        decisionsMade = 0;

        if (logTransactions) Debug.Log("Game initialized with starting money: $" + startingMoney);
    }

    public void ApplyDecision(bool choseOption1, DecisionData decision)
    {
        if (decision == null)
        {
            Debug.LogError("Decision data is null!");
            return;
        }

        int impact = choseOption1 ? decision.option1MoneyImpact : decision.option2MoneyImpact;

        // Handle financial transaction
        if (!ProcessTransaction(impact, choseOption1, decision))
        {
            return; // Transaction failed (insufficient funds)
        }

        // Update game state
        decisionsMade++;
        UpdateGameUI();

        // Check for game completion
        if (decisionsMade >= TOTAL_DECISIONS)
        {
            ShowEndGameSummary();
        }
    }

    private bool ProcessTransaction(int amount, bool isOption1, DecisionData decision)
    {
        // Block spending if insufficient funds (unless it's income)
        if (amount < 0 && Mathf.Abs(amount) > currentMoney)
        {
            string feedback = isOption1 ? decision.option1Feedback : decision.option2Feedback;
            GameUIManager.Instance.ShowFeedback("Not enough money! " + feedback);
            return false;
        }

        currentMoney += amount;

        // Handle long-term investments
        if (decision.affectsLongTerm && isOption1)
        {
            longTermSavings += Mathf.Abs(amount);
            if (logTransactions) Debug.Log($"Invested ${Mathf.Abs(amount)} (Total savings: ${longTermSavings})");
        }

        if (logTransactions)
        {
            Debug.Log($"Transaction: {(amount > 0 ? "+" : "")}{amount} | New balance: ${currentMoney}");
        }

        return true;
    }

    private void UpdateGameUI()
    {
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateMoneyDisplay(currentMoney, longTermSavings);

            // Show appropriate feedback based on last decision
            string feedback = decisionsMade switch
            {
                5 => "Good start! Keep making smart choices.",
                10 => "Halfway there! Check your progress.",
                15 => "Final stretch! How's your budget looking?",
                _ => ""
            };

            if (!string.IsNullOrEmpty(feedback))
            {
                GameUIManager.Instance.ShowFeedback(feedback);
            }
        }
    }

    private void ShowEndGameSummary()
    {
        // Calculate projected growth with compound interest
        float interestRate = 0.07f; // 7% annual return
        int projectedGrowth = (int)(longTermSavings * (1 + interestRate));

        string summary = $"<b>Final Results</b>\n" +
                        $"Cash on Hand: ${currentMoney}\n" +
                        $"Investments: ${longTermSavings}\n" +
                        $"Projected 1-Year Growth: ${projectedGrowth}\n\n" +
                        $"<i>Total Decisions Made: {decisionsMade}</i>";

        GameUIManager.Instance.ShowFeedback(summary, isEndGame: true);

        if (logTransactions) Debug.Log("Game completed! " + summary.Replace("<b>", "").Replace("</b>", ""));
    }

    // Helper method for debugging/cheats
    public void AddFunds(int amount)
    {
        currentMoney += amount;
        UpdateGameUI();
        Debug.Log($"Cheat: Added ${amount}. New balance: ${currentMoney}");
    }
}