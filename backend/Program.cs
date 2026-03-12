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
    var Cleangquestion = "cleaned question";
    var Cleangquestions = new List<CleanTriviaQuestion>();
    for (int i = 0; i < response.Results.Count; i++)
    {
        Console.WriteLine($"Cleaning question: {response.Results[i].Question}");
        Cleangquestions.Add(new CleanTriviaQuestion
        {
            Type = response.Results[i].Type,
            Difficulty = response.Results[i].Difficulty,
            Category = response.Results[i].Category,
            Question = System.Net.WebUtility.HtmlDecode(response.Results[i].Question)
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

app.MapGet("/Questions", () =>
{
    var endpoint = new Uri("https://opentdb.com/api.php?amount=3");
    var result = client.GetAsync(endpoint).Result;
    var json = result.Content.ReadAsStringAsync().Result;
    var json2 = JsonConvert.DeserializeObject<TriviaApiResponse>(json);
    Console.WriteLine("Fetched questions from Open Trivia DB");
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

