var builder = WebApplication.CreateBuilder(args);

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

// app.MapGet("/test", async (IHttpClientFactory factory) =>
// {
//     var client = factory.CreateClient();
//     var response = await client.GetAsync("http://localhost:5210/external-post");

//     if (!response.IsSuccessStatusCode)
//         return Results.StatusCode((int)response.StatusCode);

//     var json = await response.Content.ReadAsStringAsync();
//     return Results.Content(json, "application/json");
// })
// .WithName("Gettest")
// .WithOpenApi();
app.Run();

