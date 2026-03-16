using System.Text.RegularExpressions;
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
static string CleanQuestions(TriviaApiResponse response)
{
    var cleanQuestions = new List<CleanTriviaQuestion>();
    for (int i = 0; i < response.Results.Count; i++)
    {
        cleanQuestions.Add(new CleanTriviaQuestion
        {
            Type = response.Results[i].Type,
            Difficulty = response.Results[i].Difficulty,
            Category = response.Results[i].Category,
            Question = System.Net.WebUtility.HtmlDecode(response.Results[i].Question),
            Answers = Shuffle(new List<string>(response.Results[i].IncorrectAnswers).Append(response.Results[i].CorrectAnswer).ToList()),
            CorrectAnswers = new List<string>
            {
                ComputeHashCode($"{NormalizeForHash(response.Results[i].CorrectAnswer)}|{NormalizeForHash(response.Results[i].Question)}").ToString()
            }
        });
    }
    var questionsJson = JsonConvert.SerializeObject(cleanQuestions);
    return questionsJson;
}   
static int ComputeHashCode(string input)
{
            int hashCode = input.GetHashCode();
            return hashCode;
}
static string NormalizeForHash(string input)
{
    var decoded = System.Net.WebUtility.HtmlDecode(input ?? string.Empty);
    var collapsedWhitespace = Regex.Replace(decoded.Trim(), @"\s+", " ");
    return collapsedWhitespace.ToUpperInvariant();
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
    var triviaResponse = System.Text.Json.JsonSerializer.Deserialize<TriviaApiResponse>(json);
    return CleanQuestions(triviaResponse);
})
.WithName("GetQuestions")
.WithOpenApi();


app.MapPost("/Answers", (AnswerSubmission submission) =>
{


    bool[] answerResults = new bool[submission.Questions.Length];
    for (int i = 0; i < submission.Questions.Length; i++)
    {
        var submittedCode = ComputeHashCode($"{NormalizeForHash(submission.Answers[i])}|{NormalizeForHash(submission.Questions[i])}").ToString();
        if (submittedCode == submission.CorrectAnswers[i])
        {
            answerResults[i] = true;
        }
        else
        {
            answerResults[i] = false;
        }
  }

    return Results.Ok(answerResults);
}).WithName("PostAnswer")
.WithOpenApi();
app.Run();

