namespace Assets._03_Quiz.Scripts
{
	using System.Collections.Generic;

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
}
