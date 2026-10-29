using FluentAssertions;
using GestionOrdinateurs.API.Event;
using GestionOrdinateurs.API.Models;
using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.Shared.Ordinateurs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace GestionOrdinateurs.Tests;

public class OrdinateurServiceTest
{
    private static ParcInformatiqueContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ParcInformatiqueContext(options);
    }

    private static OrdinateurService CreateService(ParcInformatiqueContext context)
    {
        return new OrdinateurService(
            context,
            NullLogger<OrdinateurEventSubscriber>.Instance,
            new JournalService());
    }

    [Fact]
    public async Task CreateOrdinateur_AjouteUnOrdinateur()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var beforeCount = (await service.GetAllAsync()).Count;

        var pc = new Ordinateur
        {
            Id = 100,
            Nom = "PC-Test",
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable),
            Etat = EtatOrdinateur.EnService,
            Prix = 1299.99m,
            Stock = 5
        };

        await service.CreateOrdinateurAsync(pc);

        var ordinateurs = await service.GetAllAsync();
        ordinateurs.Should().HaveCount(beforeCount + 1);
        var createdOrdinateur = ordinateurs.Should().ContainSingle().Subject;
        createdOrdinateur.Id.Should().Be(100);
        createdOrdinateur.Nom.Should().Be("PC-Test");
        createdOrdinateur.Caracteristiques.RamGo.Should().Be(16);
        createdOrdinateur.Caracteristiques.StockageGo.Should().Be(512);
        createdOrdinateur.Caracteristiques.TypeOrdi.Should().Be(TypeOrdinateur.Portable);
        createdOrdinateur.Etat.Should().Be(EtatOrdinateur.EnService);
        createdOrdinateur.Prix.Should().Be(1299.99m);
        createdOrdinateur.Stock.Should().Be(5);
    }

    [Fact]
    public async Task UpdateOrdinateur_MetAJourTousLesAttributsModifiables()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        context.Ordinateurs.Add(new Ordinateur
        {
            Id = 101,
            Nom = "Ancien PC",
            Caracteristiques = new Caracteristiques(8, 256, TypeOrdinateur.Fixe),
            Etat = EtatOrdinateur.EnMaintenance,
            Prix = 500m,
            Stock = 1
        });
        await context.SaveChangesAsync();

        var updatedInput = new Ordinateur
        {
            Id = 101,
            Nom = "Nouveau PC",
            Caracteristiques = new Caracteristiques(32, 1024, TypeOrdinateur.Server),
            Etat = EtatOrdinateur.EnService,
            Prix = 1899.99m,
            Stock = 7
        };

        var updatedOrdinateur = await service.UpdateOrdinateurAsync(101, updatedInput);

        updatedOrdinateur.Should().NotBeNull();
        updatedOrdinateur!.Id.Should().Be(101);
        updatedOrdinateur.Nom.Should().Be("Nouveau PC");
        updatedOrdinateur.Caracteristiques.RamGo.Should().Be(32);
        updatedOrdinateur.Caracteristiques.StockageGo.Should().Be(1024);
        updatedOrdinateur.Caracteristiques.TypeOrdi.Should().Be(TypeOrdinateur.Server);
        updatedOrdinateur.Etat.Should().Be(EtatOrdinateur.EnService);
        updatedOrdinateur.Prix.Should().Be(1899.99m);
        updatedOrdinateur.Stock.Should().Be(7);
    }

    [Fact]
    public async Task GetDisponibles_RetourneUniquementLesOrdinateursAvecDuStock()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        context.Ordinateurs.AddRange(
            new Ordinateur { Id = 1, Nom = "PC disponible", Caracteristiques = new(8, 256, TypeOrdinateur.Fixe), Etat = EtatOrdinateur.EnService, Stock = 3 },
            new Ordinateur { Id = 2, Nom = "PC sans stock", Caracteristiques = new(16, 512, TypeOrdinateur.Portable), Etat = EtatOrdinateur.EnService, Stock = 0 },
            new Ordinateur { Id = 3, Nom = "PC stock négatif", Caracteristiques = new(32, 1024, TypeOrdinateur.Server), Etat = EtatOrdinateur.HorsService, Stock = -1 });
        await context.SaveChangesAsync();

        var resultat = await service.GetDisponiblesAsync();

        resultat.Should().ContainSingle().Which.Stock.Should().BeGreaterThan(0);
        resultat[0].Id.Should().Be(1);
    }
}