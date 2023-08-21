using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebChatApp.Data;
using WebChatApp.Data.Entities;
using WebChatApp.Hubs;
using WebChatApp.IdentityServer;
using WebChatApp.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
var mvcBuilder = builder.Services.AddRazorPages(opts =>
{
    opts.Conventions.AddAreaFolderRouteModelConvention("Idenity", "/Account/", model =>
    {
        foreach (var selector in model.Selectors)
        {
            var atributeRouteModel = selector.AttributeRouteModel;
            atributeRouteModel.Order = -1;
            atributeRouteModel.Template = atributeRouteModel.Template.Remove(0, "Identity".Length);
        }
    });
});

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}



builder.Services.AddDbContext<ManageAppDbContext>(
    options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
);


builder.Services
    .AddIdentity<ManageUser, IdentityRole>()
    .AddEntityFrameworkStores<ManageAppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication()
            .AddLocalApi("Bearer", option =>
            {
                option.ExpectedScope = "api.WebApp";
            });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Bearer", policy =>  // thêm một cái chính sách
    {
        policy.AddAuthenticationSchemes("Bearer");
        policy.RequireAuthenticatedUser();
    });
});

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web app sapce api", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(configuration["AuthorityUrl"] + "/connect/authorize"),
                    Scopes = new Dictionary<string, string> { { "api.WebApp", "WebApp Api" } }
                }
            },
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme,Id= "Bearer"}
                },
                new List<string>{"api.WebApp"}
            }
        });
    }
    );

builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

}).AddInMemoryApiResources(Config.Apis)
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryIdentityResources(Config.Ids)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddAspNetIdentity<ManageUser>()
    .AddDeveloperSigningCredential();

// Remove this line to avoid duplication
// builder.Services.AddRazorPages();
builder.Services.AddTransient<IEmailSender, EmailSenderService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapRazorPages();
    endpoints.MapHub<ChatHub>("/chatHub");
});

app.UseIdentityServer();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.OAuthClientId("swagger");
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web app space api v1");
});

app.Run();
