using UnityEngine;
using TMPro;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject popupPanel;
    public TMP_Text questionText;
    public Button option1Button;
    public Button option2Button;
    public TMP_Text option1Text;
    public TMP_Text option2Text;
    public TMP_Text moneyText;
    public TMP_Text longTermText;
    public GameObject feedbackPanel;
    public TMP_Text feedbackText;


    public Button[] optionButtons;
    public TMP_Text totalInterestText; // Text that shows at the Goal





    [Header("Interest Settings")]
    public float annualInterestRate = 0.08f;
    public int interestYears = 5;

    private DecisionData currentDecision;
    private bool lastChoiceWasOption1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        popupPanel.SetActive(false);
        feedbackPanel.SetActive(false);
    }

    public void ShowPopup(DecisionData decision)
    {
        currentDecision = decision;
        popupPanel.SetActive(true);
        questionText.text = decision.question;
        option1Text.text = decision.option1Text;
        option2Text.text = decision.option2Text;

        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();

        option1Button.onClick.AddListener(() => {
            lastChoiceWasOption1 = true;
            GameManager.Instance.ApplyDecision(true, decision);
            ClosePopup();
            ShowFormattedFeedback(true);
            ResumePlayer();
        });

        option2Button.onClick.AddListener(() => {
            lastChoiceWasOption1 = false;
            GameManager.Instance.ApplyDecision(false, decision);
            ClosePopup();
            ShowFormattedFeedback(false);
            ResumePlayer();
        });









    }

    public void UpdateMoneyDisplay(int money, int longTermValue)
    {
        moneyText.text = $"Cash: ${money}";
        longTermText.text = $"Investments: ${longTermValue}";
    }

    public void ShowFeedback(string message, bool isEndGame = false)
    {
        feedbackText.text = message;
        feedbackPanel.SetActive(true);
        if (!isEndGame) Invoke(nameof(HideFeedback), 3f);
    }

    private void ShowFormattedFeedback(bool choseOption1)
    {
        if (currentDecision == null) return;

        int moneyImpact = choseOption1 ? currentDecision.option1MoneyImpact : currentDecision.option2MoneyImpact;
        string rawFeedback = choseOption1 ? currentDecision.option1Feedback : currentDecision.option2Feedback;

        // Replace {amount}
        string result = rawFeedback.Replace("{amount}", Mathf.Abs(moneyImpact).ToString());

        // Replace {interest} and {years} for investments on Option 1 only
        if (currentDecision.affectsLongTerm && choseOption1)
        {
            float interestValue = CalculateCompoundInterest(Mathf.Abs(moneyImpact));
            result = result.Replace("{interest}", $"${interestValue:F2}");
            result = result.Replace("{years}", interestYears.ToString());
        }

        ShowFeedback(result);
    }

    private float CalculateCompoundInterest(int principal)
    {
        return principal * Mathf.Pow(1 + annualInterestRate, interestYears);
    }

    private void ClosePopup()
    {
        popupPanel.SetActive(false);
    }

    private void ResumePlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var mover = player.GetComponent<LaneMover>();
            if (mover != null) mover.isPaused = false;
        }
    }

    private void HideFeedback() => feedbackPanel.SetActive(false);

    public void ShowTotalInterestFromLongTerm()
    {
        // Remove non-numeric chars and parse the value from longTermText ("Investments: $123")
        string numeric = longTermText.text.Replace("Investments:", "").Replace("$", "").Trim();

        if (int.TryParse(numeric, out int longTermValue))
        {
            float totalInterest = longTermValue * Mathf.Pow(1 + annualInterestRate, interestYears);
            totalInterestText.text = $"Total Interest Earned: ${totalInterest:F2}";
            totalInterestText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Failed to parse investment value from longTermText"); 
        }
    }


}
