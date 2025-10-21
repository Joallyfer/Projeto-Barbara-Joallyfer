using Joallyfer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Joallyfer; 

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();
var app = builder.Build();

app.MapPost("/consumo/cadastrar", ([FromBody] Consumo consumo, AppDataContext ctx) =>
{
    if (consumo.Mes < 1 || consumo.Mes > 12)
        return Results.BadRequest("Mês inválido");
    if (consumo.Ano < 2000)
        return Results.BadRequest("Ano inválido");
    if (consumo.M3Consumidos <= 0)
        return Results.BadRequest("Consumo deve ser maior que zero");

    var duplicado = ctx.Consumos.FirstOrDefault(x => x.Cpf == consumo.Cpf && x.Mes == consumo.Mes && x.Ano == consumo.Ano);
    if (duplicado != null)
        return Results.BadRequest("Já existe leitura para esse CPF nesse mês");

    // Cálculos
    consumo.ConsumoFaturado = consumo.M3Consumidos;
    consumo.Tarifa = 5.0;
    consumo.ValorAgua = consumo.M3Consumidos * consumo.Tarifa;

    if (consumo.Bandeira.ToLower().Contains("amarela"))
        consumo.AdicionalBandeira = consumo.M3Consumidos * 1.1;
    else if (consumo.Bandeira.ToLower().Contains("vermelha"))
        consumo.AdicionalBandeira = consumo.M3Consumidos * 1.2;
    else
        consumo.AdicionalBandeira = 0;

    consumo.TaxaEsgoto = consumo.PossuiEsgoto ? consumo.ValorAgua * 0.80 : 0;
    consumo.Total = consumo.ValorAgua + consumo.AdicionalBandeira + consumo.TaxaEsgoto;

    ctx.Consumos.Add(consumo);
    ctx.SaveChanges();

    return Results.Created($"/consumo/{consumo.Id}", consumo);
});

app.MapGet("/consumo/listar", (AppDataContext ctx) =>
{
    var lista = ctx.Consumos.ToList();
    if (lista.Count == 0)
        return Results.NotFound("Nenhum consumo cadastrado.");
    return Results.Ok(lista);


});

    app.MapGet("/consumo/buscar/{cpf}/{mes}/{ano}", 
    (string cpf, int mes, int ano, AppDataContext ctx) =>
{
    var consumo = ctx.Consumos.FirstOrDefault(x => x.Cpf == cpf && x.Mes == mes && x.Ano == ano);

    if (consumo == null)
    {
        return Results.NotFound("Leitura não encontrada");
    }

    return Results.Ok(consumo);
});

app.MapDelete("/consumo/remover/{cpf}/{mes}/{ano}", 
    (string cpf, int mes, int ano, AppDataContext ctx) =>
{
    var consumo = ctx.Consumos.FirstOrDefault(x => x.Cpf == cpf && x.Mes == mes && x.Ano == ano);

    if (consumo == null)
    {
        return Results.NotFound("Leitura não encontrada");
    }

    ctx.Consumos.Remove(consumo);
    ctx.SaveChanges();

    return Results.Ok("Leitura removida com sucesso");

   
});

 app.Run();




















