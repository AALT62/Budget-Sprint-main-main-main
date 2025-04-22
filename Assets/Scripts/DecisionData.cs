using UnityEngine;

[CreateAssetMenu(fileName = "NewDecision", menuName = "Budgeting Game/Decision")]
public class DecisionData : ScriptableObject
{
    public string decisionID;
    [TextArea] public string question;
    public string option1Text;
    public string option2Text;
    public int option1MoneyImpact;
    public int option2MoneyImpact;
    [TextArea] public string option1Feedback;
    [TextArea] public string option2Feedback;
    public bool affectsLongTerm; // For IRA/investments
}