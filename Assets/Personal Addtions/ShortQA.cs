using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
public class ShortQA : MonoBehaviour
{
    [SerializeField] private TMP_InputField FieldOne;
    [SerializeField] private TMP_Dropdown FieldTwo;
    [SerializeField] private Slider FieldThree;
    [SerializeField] private Scribe ServerPoint;
    [SerializeField] private Button Submit;
    private string AnswerOne = "Null";
    private string AnswerTwo = "Monday";
    private int AnswerThree = 0;
    void Awake()
    {
        FieldOne.onValueChanged.AddListener(UpdateValueOne);
        FieldTwo.onValueChanged.AddListener(UpdateValueTwo);
        FieldThree.onValueChanged.AddListener(UpdateValueThree);
        Submit.onClick.AddListener(SumbitAnswers);
        
    }
    private void UpdateValueOne(string newValue)
    {
        AnswerOne = newValue;
    }
    private void UpdateValueTwo(int newValue)
    {
        AnswerTwo = FieldTwo.options[newValue].text;
    }
    private void UpdateValueThree(float newValue)
    {
        AnswerThree = (int)newValue;
    }
    private void SumbitAnswers()
    {
        QAresponses answers = new QAresponses(AnswerOne,AnswerTwo,AnswerThree);
        ServerPoint.NoteDownRpc(answers);
    }
    


}
