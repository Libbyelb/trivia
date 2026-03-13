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
            Answers = new List<string>(response.Results[i].IncorrectAnswers).Append(response.Results[i].CorrectAnswer).ToList(),
            CorrectAnswers = new List<string> { hashcode(response.Results[i].CorrectAnswer).ToString() }
        });
    }
    var stringpfquestions = JsonConvert.SerializeObject(Cleangquestions);
    return stringpfquestions;
}   

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

static int hashcode(string input)
{
            int hashCode = input.GetHashCode();
            return hashCode;
}


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


app.MapPost("/Answers", (bool answers) =>
{
    Console.WriteLine($"Received answer: {answers}");
    if (answers)
    {
        return Results.Ok("Correct!");
    }
    return Results.Created($"/Questions/", answers);
}).WithName("PostAnswer")
.WithOpenApi();
app.Run();

