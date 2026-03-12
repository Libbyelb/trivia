using System.Net.Http;


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



//

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
    Console.WriteLine("Fetched questions from Open Trivia DB");
    return json;
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

