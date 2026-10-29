using FluentAssertions;
using GestionOrdinateurs.API.Models;
using GestionOrdinateurs.Shared.Ordinateurs;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.Tests;

public class ReparationTest
{
    [Fact]
    public async Task CreerReparation_EnregistreLaReparationAssocieeAUnOrdinateur()
    {
        var options = new DbContextOptionsBuilder<ParcInformatiqueContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ParcInformatiqueContext(options);
        var ordinateur = new Ordinateur
        {
            Nom = "PC de test",
            Caracteristiques = new Caracteristiques(16, 512, TypeOrdinateur.Portable)
        };
        context.Ordinateurs.Add(ordinateur);
        await context.SaveChangesAsync();

        var reparation = new Reparation
        {
            OrdinateurId = ordinateur.Id,
            UnOrdinateur = ordinateur,
            TypeReparation = "Remplacement de batterie",
            DateDebut = DateTime.UtcNow,
            Statut = "En cours",
            Technicien = "Alice"
        };

        context.Reparations.Add(reparation);
        await context.SaveChangesAsync();

        var reparationEnregistree = await context.Reparations
            .AsNoTracking()
            .SingleAsync();
        reparationEnregistree.Id.Should().BeGreaterThan(0);
        reparationEnregistree.OrdinateurId.Should().Be(ordinateur.Id);
        reparationEnregistree.TypeReparation.Should().Be("Remplacement de batterie");
        reparationEnregistree.Statut.Should().Be("En cours");
        reparationEnregistree.Technicien.Should().Be("Alice");
    }
}
