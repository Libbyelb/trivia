using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json;
using Backend.Models;

var builder = WebApplication.CreateBuilder(args);
var client = new HttpClient();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows cross-origin requests from the frontend
//-- CORS configuration --
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod());
});
var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors("frontend");
//--
  static List<T> Shuffle<T>(List<T> list) {
    Random random = new Random();
    int n = list.Count;

    // Start from the end and swap elements with a random one
    for (int i = n - 1; i > 0; i--) {
      int j = random.Next(0, i + 1);
      T temp = list[i];
      list[i] = list[j];
      list[j] = temp;
    }
    return list;
  }
static string Cleaningquestion(TriviaApiResponse response)
{
    var Cleangquestions = new List<CleanTriviaQuestion>();
    for (int i = 0; i < response.Results.Count; i++)
    {
        Cleangquestions.Add(new CleanTriviaQuestion
        {
            Type = response.Results[i].Type,
            Difficulty = response.Results[i].Difficulty,
            Category = response.Results[i].Category,
            Question = System.Net.WebUtility.HtmlDecode(response.Results[i].Question),
            Answers = Shuffle(new List<string>(response.Results[i].IncorrectAnswers).Append(response.Results[i].CorrectAnswer).ToList()),
            CorrectAnswers = new List<string> { hashcode(response.Results[i].CorrectAnswer + response.Results[i].Question).ToString()  }
        });
    }
    var stringpfquestions = JsonConvert.SerializeObject(Cleangquestions);
    return stringpfquestions;
}   
static int hashcode(string input)
{
            int hashCode = input.GetHashCode();
            return hashCode;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/Questions", () =>
{
    var endpoint = new Uri("https://opentdb.com/api.php?amount=3");
    var result = client.GetAsync(endpoint).Result;
    var json = result.Content.ReadAsStringAsync().Result;
    var json2 = System.Text.Json.JsonSerializer.Deserialize<TriviaApiResponse>(json);
    return Cleaningquestion(json2);
})
.WithName("GetQuestions")
.WithOpenApi();


app.MapPost("/Answers", (AnswerSubmission submission) =>
{
    bool[] bools = new bool[submission.Questions.Length];
    for (int i = 0; i < submission.Questions.Length; i++)
    {
        Console.WriteLine($"code: {hashcode(submission.Questions[i])}");
        Console.WriteLine($"Answer: {submission.Answers[i]}");
        if (hashcode(submission.Answers[i] + submission.Questions[i]).ToString() == submission.CorrectAnswers[i])
        {
            Console.WriteLine("Correct answer!");
            bools[i] = true;
        }
        else
        {
            Console.WriteLine("Incorrect answer.");
            bools[i] = false;
        }
  }

    return Results.Ok(bools);
}).WithName("PostAnswer")
.WithOpenApi();
app.Run();

