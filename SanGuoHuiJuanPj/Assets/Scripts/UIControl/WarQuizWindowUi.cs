using System;
using CorrelateLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WarQuizWindowUi : MonoBehaviour
{
    [SerializeField] private Text QuestionText;
    [SerializeField] private Answer[] Answers;
    [SerializeField] private Button ContinueButton;
    [SerializeField] private Color TextColor = Color.white;
    [SerializeField] private Color CorrectColor = Color.green;
    [SerializeField] private Color WrongColor = Color.red;
    public void Init()
    {
        ContinueButton.onClick.AddListener(() =>
        {
            Off();
            WarsUIManager.instance.PassStage();
        });
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Off()
    {
        gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(false);
    }

    public void SetQuiz(QuestTable quest,UnityAction<bool> answerAction)
    {
        QuestionText.text = quest.Question;
        SetAnswers(ans =>
        {
            ans.Button.onClick.RemoveAllListeners();
            ans.Text.color = TextColor;
            var answerText = string.Empty;
            switch (ans.Id)
            {
                case 1: answerText = quest.A;break;
                case 2: answerText = quest.B;break;
                case 3: answerText = quest.C;break;
                default: throw new ArgumentOutOfRangeException(nameof(ans), $"ans.id ={ans.Id}");
            }
            ans.Text.text = answerText;
            ans.Button.onClick.AddListener(() => OnAnswerClicked(ans,ans.Id==quest.Answer, answerAction));
        });
    }

    private void SetAnswers(UnityAction<Answer> action)
    {
        for (int i = 0; i < 3; i++)
        {
            var ans = Answers[i];
            ans.Id = i + 1;
            action(ans);
        }
    }

    private void OnAnswerClicked(Answer answer, bool isCorrect, UnityAction<bool> answerAction)
    {
        answer.Text.color = isCorrect ? CorrectColor : WrongColor;
        ContinueButton.gameObject.SetActive(true);
        answerAction?.Invoke(isCorrect);
    }

    [Serializable] private class Answer
    {
        public int Id;
        public Text Text;
        public Button Button;
    }
}