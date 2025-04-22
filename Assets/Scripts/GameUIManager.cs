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

    private void ShowFormattedFeedback(bool option1)
    {
        if (currentDecision == null) return;

        int moneyImpact = option1 ? currentDecision.option1MoneyImpact : currentDecision.option2MoneyImpact;
        string rawFeedback = option1 ? currentDecision.option1Feedback : currentDecision.option2Feedback;

        string result = rawFeedback.Replace("{amount}", Mathf.Abs(moneyImpact).ToString());

        if (currentDecision.affectsLongTerm && option1 && moneyImpact > 0)
        {
            float compound = CalculateCompoundInterest(moneyImpact);
            result = result.Replace("{interest}", $"${compound:F2}");
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
}
