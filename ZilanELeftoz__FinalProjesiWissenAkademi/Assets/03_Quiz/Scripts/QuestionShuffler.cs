using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._03_Quiz.Scripts
{
	public class QuestionShuffler
	{
		public List<int> ShuffleQuestionsWithBuffer(int questionCount, List<int> previousOrder, int bufferSize)
		{
			List<int> newOrder = Enumerable.Range(0, questionCount).ToList();
			newOrder = newOrder.OrderBy(x => UnityEngine.Random.value).ToList();

			if (previousOrder != null && previousOrder.Count > 0)
			{
				newOrder.RemoveAll(x => previousOrder.Take(bufferSize).Contains(x));
			}

			return newOrder;
		}
	}
}
