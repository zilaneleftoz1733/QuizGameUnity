
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Question
{
    public string questionText;
    public string option1;
    public string option2;
    public string option3;
    public string option4;
    public int correctAnswerIndex;
    public int pointValue;
}
[System.Serializable]
public class QuestionList
{
    public List<Question> Questions;
}


public class DataReader : MonoBehaviour
{

    public TextMeshProUGUI questionTextUI;  // Sorular� g�sterecek TextMeshPro UI
    public Button[] optionButtons;         // ��klar i�in 4 adet Button
    public Button nextButton;              // Sonraki soru i�in Button
    public Button restartButton;               // Oyunu yeniden ba�latacak buton
    public TextMeshProUGUI nextButtonText; // Next butonunun metni
    public TextMeshProUGUI scoreTextUI;     // Puan� g�sterecek TextMeshPro UI
    public TextMeshProUGUI totalScoreTextUI;   // Toplam skoru g�sterecek Text
    public GameObject completePanel;              // Toplam skoru g�sterecek panel
    public GameObject questionPanel;              // soru g�sterecek panel
    public TextMeshProUGUI correctAnswersText; // Do�ru cevap say�s�n� g�sterecek Text
    public TextMeshProUGUI timeTakenText;      // Ge�en s�reyi g�sterecek Text



    private List<Question> questions;      // T�m sorular burada tutulacak
    private int currentQuestionIndex = 0;  // �u anki sorunun indeksi
    private float quizStartTime;               // Quiz ba�lang�� zaman�
    private int totalScore = 0;            // Toplam puan
    private int maxQuestions = 2;          // Sorular�n maksimum say�s�
    private int correctAnswers = 0;            // Do�ru cevap say�s�
    private string fileName = "quizQuestions.json"; // JSON dosya ad�
    private bool isAnswerCorrect = false;  // Do�ru cevap i�aretlenmi� mi?

    private List<int> questionOrder; // Sorular�n rastgele s�ralamas�
    private int bufferSize = 5;      // Tampon boyutu (�nceki 3 soru tekrar gelmez)
    void Start()
    {
        LoadQuestionsFromJSON();
        ShuffleQuestionsWithBuffer(); // Sorular� kar��t�r
        DisplayQuestion();

        quizStartTime = Time.time;

        nextButton.onClick.AddListener(NextQuestion);
        restartButton.onClick.AddListener(RestartGame); // Restart butonuna listener ekle
        SetNextButtonState(false, "Select an Answer"); // Ba�lang��ta pasif ve uyar� mesaj�
        completePanel.SetActive(false);
    }
    void LoadQuestionsFromJSON()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            QuestionList questionList = JsonUtility.FromJson<QuestionList>(json);

            questions = questionList.Questions;
        }
        else
        {
            Debug.LogError("JSON dosyas� bulunamad�: " + filePath);
        }
    }
    void ShuffleQuestionsWithBuffer()
    {
        List<int> newOrder = Enumerable.Range(0, questions.Count).ToList();
        newOrder = newOrder.OrderBy(x => Random.value).ToList();

        // Tampondaki sorular� tekrar listeye koyarak d�ng�y� �nle
        if (questionOrder != null && questionOrder.Count > 0)
        {
            newOrder.RemoveAll(x => questionOrder.Take(bufferSize).Contains(x));
        }

        // Tampon i�in eski s�ray� kaydet
        questionOrder = newOrder;
    }

    void DisplayQuestion()
    {
        if (currentQuestionIndex < maxQuestions && currentQuestionIndex < questions.Count)
        {
            Question currentQuestion = questions[questionOrder[currentQuestionIndex]];

            // Soru Metni
            questionTextUI.text = currentQuestion.questionText;

            // ��klar
            optionButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option1;
            optionButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option2;
            optionButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option3;
            optionButtons[3].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option4;

            // ��k Butonlar�na Listener Eklenmesi
            AddOptionListeners(currentQuestion);
            UpdateScoreText(currentQuestion.pointValue);

            // Next butonunu ba�lang��ta gizle
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
            int optionIndex = i; // Listener i�in ge�ici de�i�ken
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
            Debug.Log("Do�ru Cevap!");
            isAnswerCorrect = true;
            totalScore += pointValue; // Do�ru cevaba g�re puan� ekle
            correctAnswers++; // Do�ru cevap say�s�n� art�r
                              // ��k butonlar�n� devre d��� b�rak
            UpdateButtonStates(correctOption);
            SetNextButtonState(true, " Correct! Next");
        }
        else
        {
            Debug.Log("Yanl�� Cevap!");
            isAnswerCorrect = false;
            SetNextButtonState(false, "Wrong Answer! \n Select Answer"); // Next butonunu pasif b�rak
        }
    }
    void UpdateButtonStates(int correctOption)
    {
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i == correctOption)
            {
                // Do�ru ��k i�in sadece t�klamay� kapat, g�rseli koru
                optionButtons[i].interactable = false;

                // Do�ru cevaba �zel bir stil veya g�rsel eklemek isterseniz:
                //optionButtons[i].GetComponent<Image>().color = Color.green; // �rnek: ye�il arka plan
            }
            else
            {
                // Yanl�� ��klar i�in t�klamay� kapat ve g�rseli karart
                optionButtons[i].interactable = false;
                //optionButtons[i].GetComponent<Image>().color = Color.gray; // �rnek: gri arka plan
            }
        }
    }
    void EnableOptionButtons()
    {
        // T�m butonlar� varsay�lan hale getir
        foreach (Button btn in optionButtons)
        {
            btn.gameObject.SetActive(true);
            btn.interactable = true;
            btn.GetComponent<Image>().color = Color.white; // �rnek: varsay�lan beyaz arka plan
        }
    }
    void NextQuestion()
    {
        if (isAnswerCorrect) // Sadece do�ru cevap verildiyse �al���r
        {
            currentQuestionIndex++;
            DisplayQuestion();
        }
        // Yeni soru geldi�inde butonlar� yeniden etkinle�tir
        EnableOptionButtons();

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

        Debug.Log("Quiz tamamland�!");

        questionPanel.SetActive(false);
        // Skor panelini etkinle�tir
        completePanel.SetActive(true);

        // Toplam skoru g�ster
        totalScoreTextUI.text = "Final Score: " + totalScore;

        // Do�ru cevaplar� g�ster
        correctAnswersText.text = $"Correct Answers: {correctAnswers}/{maxQuestions}";

        // Ge�en s�reyi hesapla ve g�ster
        float timeTaken = Time.time - quizStartTime;
        timeTakenText.text = $"Time Taken: {timeTaken:F2} seconds";

    }
    void RestartGame()
    {
        // De�i�kenleri s�f�rla
        currentQuestionIndex = 0;
        totalScore = 0;
        correctAnswers = 0;
        quizStartTime = Time.time;

        // Sorular� yeniden kar��t�r
        ShuffleQuestionsWithBuffer();

        // Skor panelini gizle
        completePanel.SetActive(false);
        questionPanel.SetActive(true);

        // �lk soruyu g�ster
        DisplayQuestion();
    }

}