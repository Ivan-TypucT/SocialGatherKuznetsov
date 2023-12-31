﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialGatherKuznetsov.Data;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using SocialGatherKuznetsov.Models;
/*
using (var context = new SocialGatherKuznetsov2Context(new DbContextOptions<SocialGatherKuznetsov2Context>()))
{
    // Создаем экземпляр Guest
    var pete = new Guest()
    {
        UserId = "pete123",
    };

    // Получаем нужный Card (pol)
    var pol = context.Card.FirstOrDefault(c => c.Name == "pol");

    if (pol != null)
    {
        // Добавляем Pete в GuestsList
        pol.GuestsList.Add(pete);

        // Сохраняем изменения в БД
        context.SaveChanges();
    }
}
using (var context = new SocialGatherKuznetsov2Context(new DbContextOptions<SocialGatherKuznetsov2Context>()))
{
    // Получаем нужный Card (pol)
    var pol = context.Card.FirstOrDefault(c => c.Name == "pol");

    if (pol != null)
    {
        // Извлекаем список гостей (GuestsList) у Card (pol)
        var guestList = pol.GuestsList.ToList();

        // Используем guestList по своему усмотрению
    }
}*/

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<SocialGatherKuznetsovContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SocialGatherKuznetsovContext") ?? throw new InvalidOperationException("Connection string 'SocialGatherKuznetsovContext' not found.")));
builder.Services.AddDbContext<SocialGatherKuznetsov2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SocialGatherKuznetsov2Context") ?? throw new InvalidOperationException("Connection string 'SocialGatherKuznetsov2Context' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
           
            ValidateIssuer = true,
           
            ValidIssuer = AuthOptions.ISSUER,
     
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            
            ValidateLifetime = true,
            
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            
            ValidateIssuerSigningKey = true,
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/");

app.Run();
public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // �������� ������
    public const string AUDIENCE = "MyAuthClient"; // ����������� ������
    const string KEY = "KeyShKeyShKeyShKeyShKeyShKeyShKeyShKeyShKeySh";   // ���� ��� ��������
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}