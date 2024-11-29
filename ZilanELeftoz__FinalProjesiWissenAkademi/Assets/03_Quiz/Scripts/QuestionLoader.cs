using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets._03_Quiz.Scripts
{
	public class QuestionLoader
	{
		public string fileName = "quizQuestions.json";

		public List<Question> LoadQuestions()
		{
			string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

			if (File.Exists(filePath))
			{
				string json = File.ReadAllText(filePath);
				QuestionList questionList = JsonUtility.FromJson<QuestionList>(json);
				return questionList.Questions;
			}
			else
			{
				Debug.LogError("JSON file not found: " + filePath);
				return new List<Question>();
			}
		}
	}
}

