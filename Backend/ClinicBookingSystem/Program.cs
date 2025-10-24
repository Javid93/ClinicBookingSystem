using ClinicBookingSystem.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddControllers();

//Add DbContext with MySql
builder.Services.AddDbContext<ClinicContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));


// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.CustomSchemaIds(type => type.Name.Replace("DTO", ""));
    c.SchemaFilter<ClinicBookingSystem.Filters.PatientDtoExampleFilter>();
    c.SchemaFilter<ClinicBookingSystem.Filters.AppointmentDtoExampleFilter>();
    c.SchemaFilter<ClinicBookingSystem.Filters.ClinicDtoExampleFilter>();
    c.SchemaFilter<ClinicBookingSystem.Filters.DoctorDtoExampleFilter>();
    c.SchemaFilter<ClinicBookingSystem.Filters.SpecialityDtoExampleFilter>();


    //For XML comments
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "ClinicBookingSystem.xml");
    c.IncludeXmlComments(xmlPath);

});





//Add CORS policy for future frontend access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5221", // Swagger backend
            "http://localhost:5173" //React frontend
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();


//Create DB and apply migrations if not already eksist
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ClinicContext>();
    dbContext.Database.Migrate();
}

//Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinic API V1");
        c.RoutePrefix = "doc";
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
