var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/Questions", () =>
{
    var Question = new
    {
        Id = 2,
        Text = $"Question 2",
        Answer = $"Answer 2"
    };
    return Question;
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

