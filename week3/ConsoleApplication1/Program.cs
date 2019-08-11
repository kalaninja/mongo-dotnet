using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args)
		{
			MainAsync().Wait();

			Console.WriteLine("Press any key");
			Console.ReadLine();
		}

		private async static Task MainAsync()
		{
			var client = new MongoClient();
			var db = client.GetDatabase("school");

			var studentsCollection = db.GetCollection<Student>("students");
			var students = await studentsCollection.Find(new BsonDocument()).ToListAsync();

			foreach (var student in students)
			{
				var score = student.Scores.Where(x => x.Type.ToLower() == "homework").OrderBy(x => x.ScoreValue).First();
				student.Scores.Remove(score);

				studentsCollection.ReplaceOne(x => x.Id == student.Id, student);
			}
		}

		private class Student
		{
			public int Id { get; set; }

			[BsonElement("name")]
			public string Name { get; set; }

			[BsonElement("scores")]
			public IList<Score> Scores { get; set; }
		}

		private class Score
		{
			[BsonElement("type")]
			public string Type { get; set; }

			[BsonElement("score")]
			public double ScoreValue { get; set; }
		}
	}
}
