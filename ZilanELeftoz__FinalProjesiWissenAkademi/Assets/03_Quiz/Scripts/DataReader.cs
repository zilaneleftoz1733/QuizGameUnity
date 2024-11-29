
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

    public TextMeshProUGUI questionTextUI;  // Sorularý gösterecek TextMeshPro UI
    public Button[] optionButtons;         // Þýklar için 4 adet Button
    public Button nextButton;              // Sonraki soru için Button
    public Button restartButton;               // Oyunu yeniden baþlatacak buton
    public TextMeshProUGUI nextButtonText; // Next butonunun metni
    public TextMeshProUGUI scoreTextUI;     // Puaný gösterecek TextMeshPro UI
    public TextMeshProUGUI totalScoreTextUI;   // Toplam skoru gösterecek Text
    public GameObject completePanel;              // Toplam skoru gösterecek panel
    public GameObject questionPanel;              // soru gösterecek panel
    public TextMeshProUGUI correctAnswersText; // Doðru cevap sayýsýný gösterecek Text
    public TextMeshProUGUI timeTakenText;      // Geçen süreyi gösterecek Text



    private List<Question> questions;      // Tüm sorular burada tutulacak
    private int currentQuestionIndex = 0;  // Þu anki sorunun indeksi
    private float quizStartTime;               // Quiz baþlangýç zamaný
    private int totalScore = 0;            // Toplam puan
    private int maxQuestions = 2;          // Sorularýn maksimum sayýsý
    private int correctAnswers = 0;            // Doðru cevap sayýsý
    private string fileName = "quizQuestions.json"; // JSON dosya adý
    private bool isAnswerCorrect = false;  // Doðru cevap iþaretlenmiþ mi?

    private List<int> questionOrder; // Sorularýn rastgele sýralamasý
    private int bufferSize = 5;      // Tampon boyutu (önceki 3 soru tekrar gelmez)
    void Start()
    {
        LoadQuestionsFromJSON();
        ShuffleQuestionsWithBuffer(); // Sorularý karýþtýr
        DisplayQuestion();

        quizStartTime = Time.time;

        nextButton.onClick.AddListener(NextQuestion);
        restartButton.onClick.AddListener(RestartGame); // Restart butonuna listener ekle
        SetNextButtonState(false, "Select an Answer"); // Baþlangýçta pasif ve uyarý mesajý
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
            Debug.LogError("JSON dosyasý bulunamadý: " + filePath);
        }
    }
    void ShuffleQuestionsWithBuffer()
    {
        List<int> newOrder = Enumerable.Range(0, questions.Count).ToList();
        newOrder = newOrder.OrderBy(x => Random.value).ToList();

        // Tampondaki sorularý tekrar listeye koyarak döngüyü önle
        if (questionOrder != null && questionOrder.Count > 0)
        {
            newOrder.RemoveAll(x => questionOrder.Take(bufferSize).Contains(x));
        }

        // Tampon için eski sýrayý kaydet
        questionOrder = newOrder;
    }

    void DisplayQuestion()
    {
        if (currentQuestionIndex < maxQuestions && currentQuestionIndex < questions.Count)
        {
            Question currentQuestion = questions[questionOrder[currentQuestionIndex]];

            // Soru Metni
            questionTextUI.text = currentQuestion.questionText;

            // Þýklar
            optionButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option1;
            optionButtons[1].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option2;
            optionButtons[2].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option3;
            optionButtons[3].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.option4;

            // Þýk Butonlarýna Listener Eklenmesi
            AddOptionListeners(currentQuestion);
            UpdateScoreText(currentQuestion.pointValue);

            // Next butonunu baþlangýçta gizle
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
            int optionIndex = i; // Listener için geçici deðiþken
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
            Debug.Log("Doðru Cevap!");
            isAnswerCorrect = true;
            totalScore += pointValue; // Doðru cevaba göre puaný ekle
            correctAnswers++; // Doðru cevap sayýsýný artýr
                              // Þýk butonlarýný devre dýþý býrak
            UpdateButtonStates(correctOption);
            SetNextButtonState(true, " Correct! Next");
        }
        else
        {
            Debug.Log("Yanlýþ Cevap!");
            isAnswerCorrect = false;
            SetNextButtonState(false, "Wrong Answer! \n Select Answer"); // Next butonunu pasif býrak
        }
    }
    void UpdateButtonStates(int correctOption)
    {
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i == correctOption)
            {
                // Doðru þýk için sadece týklamayý kapat, görseli koru
                optionButtons[i].interactable = false;

                // Doðru cevaba özel bir stil veya görsel eklemek isterseniz:
                //optionButtons[i].GetComponent<Image>().color = Color.green; // Örnek: yeþil arka plan
            }
            else
            {
                // Yanlýþ þýklar için týklamayý kapat ve görseli karart
                optionButtons[i].interactable = false;
                //optionButtons[i].GetComponent<Image>().color = Color.gray; // Örnek: gri arka plan
            }
        }
    }
    void EnableOptionButtons()
    {
        // Tüm butonlarý varsayýlan hale getir
        foreach (Button btn in optionButtons)
        {
            btn.gameObject.SetActive(true);
            btn.interactable = true;
            btn.GetComponent<Image>().color = Color.white; // Örnek: varsayýlan beyaz arka plan
        }
    }
    void NextQuestion()
    {
        if (isAnswerCorrect) // Sadece doðru cevap verildiyse çalýþýr
        {
            currentQuestionIndex++;
            DisplayQuestion();
        }
        // Yeni soru geldiðinde butonlarý yeniden etkinleþtir
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

        Debug.Log("Quiz tamamlandý!");

        questionPanel.SetActive(false);
        // Skor panelini etkinleþtir
        completePanel.SetActive(true);

        // Toplam skoru göster
        totalScoreTextUI.text = "Final Score: " + totalScore;

        // Doðru cevaplarý göster
        correctAnswersText.text = $"Correct Answers: {correctAnswers}/{maxQuestions}";

        // Geçen süreyi hesapla ve göster
        float timeTaken = Time.time - quizStartTime;
        timeTakenText.text = $"Time Taken: {timeTaken:F2} seconds";

    }
    void RestartGame()
    {
        // Deðiþkenleri sýfýrla
        currentQuestionIndex = 0;
        totalScore = 0;
        correctAnswers = 0;
        quizStartTime = Time.time;

        // Sorularý yeniden karýþtýr
        ShuffleQuestionsWithBuffer();

        // Skor panelini gizle
        completePanel.SetActive(false);
        questionPanel.SetActive(true);

        // Ýlk soruyu göster
        DisplayQuestion();
    }

}