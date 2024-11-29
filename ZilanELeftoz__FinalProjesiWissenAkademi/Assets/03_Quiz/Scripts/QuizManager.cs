using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._03_Quiz.Scripts
{
	public class QuizManager : MonoBehaviour
	{
		public TextMeshProUGUI questionTextUI;
		public Button[] optionButtons;
		public Button nextButton;
		public Button restartButton;
		public TextMeshProUGUI nextButtonText;
		public TextMeshProUGUI scoreTextUI;
		public TextMeshProUGUI totalScoreTextUI;
		public GameObject completePanel;
		public GameObject questionPanel;
		public TextMeshProUGUI correctAnswersText;
		public TextMeshProUGUI timeTakenText;

		private List<Question> questions;
		private List<int> questionOrder;
		private int currentQuestionIndex = 0;
		private float quizStartTime;
		private int totalScore = 0;
		private int correctAnswers = 0;
		private int maxQuestions = 2;
		private bool isAnswerCorrect = false;
		private int bufferSize = 3;

		private QuestionLoader questionLoader;
		private QuestionShuffler questionShuffler;



		void Start()
		{
			questionLoader = new QuestionLoader();
			questionShuffler = new QuestionShuffler();

			questions = questionLoader.LoadQuestions();
			questionOrder = questionShuffler.ShuffleQuestionsWithBuffer(questions.Count, null, bufferSize);

			quizStartTime = Time.time;

			nextButton.onClick.AddListener(NextQuestion);
			restartButton.onClick.AddListener(RestartGame);

			SetNextButtonState(false, "Select an Answer");
			completePanel.SetActive(false);

			DisplayQuestion();
		}

		void DisplayQuestion()
		{
			if (currentQuestionIndex < maxQuestions && currentQuestionIndex < questions.Count)
			{
				Question currentQuestion = questions[questionOrder[currentQuestionIndex]];

				questionTextUI.text = currentQuestion.questionText;

				optionButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option1;
				optionButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option2;
				optionButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option3;
				optionButtons[3].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option4;

				AddOptionListeners(currentQuestion);
				UpdateScoreText(currentQuestion.pointValue);
				SetNextButtonState(false, "Please select an answer");
			}
			else
			{
				EndQuiz();
			}
		}

		void AddOptionListeners(Question question)
		{
			for (int i = 0; i < optionButtons.Length; i++)
			{
				int optionIndex = i;
				optionButtons[i].onClick.RemoveAllListeners();

				optionButtons[i].onClick.AddListener(() =>
				{
					CheckAnswer(optionIndex, question.correctAnswerIndex - 1, question.pointValue);
				});
			}
		}

		void CheckAnswer(int selectedOption, int correctOption, int pointValue)
		{
			if (selectedOption == correctOption)
			{
				Debug.Log("Doğru Cevap!");
				isAnswerCorrect = true;
				totalScore += pointValue; // Doğru cevaba göre puanı ekle
				correctAnswers++; // Doğru cevap sayısını artır
								  // Şık butonlarını devre dışı bırak
				UpdateButtonStates(correctOption);
				SetNextButtonState(true, "Correct! Next");
			}
			else
			{
				Debug.Log("Yanlış Cevap!");
				isAnswerCorrect = false;
				SetNextButtonState(false, "Wrong Answer! \n Select Answer"); 
			}
		}
		void UpdateButtonStates(int correctOption)
		{
			for (int i = 0; i < optionButtons.Length; i++)
			{
				if (i == correctOption)
				{
					optionButtons[i].interactable = false;

				}
				else
				{
					
					optionButtons[i].interactable = false;
			
				}
			}
		}

		void NextQuestion()
		{
			if (isAnswerCorrect)
			{
				currentQuestionIndex++;
				DisplayQuestion();
			}
			EnableOptionButtons();

		}

		void EnableOptionButtons()
		{
			foreach (Button btn in optionButtons)
			{
				btn.gameObject.SetActive(true);
				btn.interactable = true;
				btn.GetComponent<Image>().color = Color.white;
			}
		}
		void SetNextButtonState(bool interactable, string buttonText)
		{
			nextButton.interactable = interactable;
			nextButtonText.text = buttonText;
		}

		void UpdateScoreText(int pointValue)
		{
			scoreTextUI.text = pointValue.ToString();
		}

		void EndQuiz()
		{
			questionPanel.SetActive(false);
			completePanel.SetActive(true);

			totalScoreTextUI.text = "Final Score: " + totalScore;
			correctAnswersText.text = $"Correct Answers: {correctAnswers}/{maxQuestions}";
			timeTakenText.text = $"Time Taken: {Time.time - quizStartTime:F2} seconds";
		}

		void RestartGame()
		{
			currentQuestionIndex = 0;
			totalScore = 0;
			correctAnswers = 0;
			quizStartTime = Time.time;

			questionOrder = questionShuffler.ShuffleQuestionsWithBuffer(questions.Count, questionOrder, bufferSize);

			completePanel.SetActive(false);
			questionPanel.SetActive(true);

			DisplayQuestion();
		}
	}
}
