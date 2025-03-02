using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using WorldCities.Server.Data;
using WorldCities.Server.Data.GraphQL;
using WorldCities.Server.Data.Models;

var angularPolicy = "AngularPolicy";

var builder = WebApplication.CreateBuilder(args);

//var varHttpClient = builder.c.UseUrls();
string dbConnectionString = builder.Configuration.GetConnectionString(Environment.MachineName + "--Connection") ??
    builder.Configuration.GetConnectionString("DefaultConnection");

#if DEBUG
Console.WriteLine($"The database connection string is :{dbConnectionString}");
#endif

// Adds Serilog support
builder.Host.UseSerilog((ctx , lc) => lc
     .ReadFrom.Configuration(ctx.Configuration)
     .WriteTo.MSSqlServer(connectionString: dbConnectionString ,
         restrictedToMinimumLevel: LogEventLevel.Information ,
         sinkOptions: new MSSqlServerSinkOptions {
             TableName = "LogEvents" ,
             AutoCreateSqlTable = true
         }
     )
    .WriteTo.Console());

builder.Services.AddCors(options =>
 options.AddPolicy(name: angularPolicy ,
    _configuration => {
        _configuration.AllowAnyHeader();
        _configuration.AllowAnyMethod();
        if (builder.Configuration.GetValue<bool>("cors:enabled")) {
            if (builder.Configuration.GetValue<bool>("cors:allowAnyOrigin")) {
                _configuration.AllowAnyOrigin();
            }
            else {
                List<string> origins = new List<string>();
                builder.Configuration.GetSection("cors:origins").Bind(origins);
                foreach (var origin in origins) {
                    _configuration.WithOrigins(origin.ToString());
                }
            }

        }
    }));


// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddGraphQLServer()
         .AddAuthorization()
         .AddQueryType<Query>()
         .AddMutationType<Mutation>()
         .AddFiltering()
         .AddSorting();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
//NOTE: The "DefaultConnection" value must be specified on a single line , otherwise it
//won’t work.
//200 Data Model with Entity Framework Core
     options.UseSqlServer(dbConnectionString)
);
// Add ASP.NET Core Identity support
builder.Services.AddIdentity<ApplicationUser , IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
 .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<JwtHandler>();
// Add Authentication services & middlewares
builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        RequireExpirationTime = true ,
        ValidateIssuer = true ,
        ValidateAudience = true ,
        ValidateLifetime = true ,
        ValidateIssuerSigningKey = true ,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ,
        ValidAudience = builder.Configuration["JwtSettings:Audience"] ,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.
        GetBytes(builder.Configuration["JwtSettings:SecurityKey"]!))
    };
}
);

var app = builder.Build();

app.UseSerilogRequestLogging();


app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}
else {
    app.UseExceptionHandler("/Error");
    app.MapGet("/Error" , () => Results.Problem());
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors(angularPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL("/api/graphql");

app.MapMethods("/api/heartbeat" , new[] { "HEAD" } ,
 () => Results.Ok());

app.MapFallbackToFile("/index.html");

//

app.Run();
