using GestionOrdinateurs.API.Models;
using GestionOrdinateurs.API.Services.Interfaces;
using GestionOrdinateurs.Shared.Ordinateurs;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace GestionOrdinateurs.API.Services;

public class ReparationService : IReparationService
{
    public record HistoriqueReparationRecord(
        int OrdinateurId,
        string NomOrdinateur,
        string Technicien,
        string TypeReparation,
        DateTime DateIntervention
    );

    private ParcInformatiqueContext _context;
    private JournalService _journalService;

    public ReparationService(ParcInformatiqueContext context, JournalService journalService)
    {
        _context = context;
        _journalService = journalService;
    }

    public Task<List<Reparation>> GetAllAsync()
    {
        return _context.Reparations
            .Include(reparation => reparation.UnOrdinateur)
            .ToListAsync();
    }

    public Task<List<HistoriqueReparationRecord>> GetHistoriqueReparationsAsync()
    {
        return _context.Reparations
            .Select(reparation => new HistoriqueReparationRecord(
                reparation.OrdinateurId,
                reparation.UnOrdinateur.Nom,
                reparation.Technicien,
                reparation.TypeReparation,
                reparation.DateDebut))
            .ToListAsync();
    }

    public async Task SimulerReparationAsync(Ordinateur ordinateur)
    {
        var reparation = new Reparation
        {
            OrdinateurId = ordinateur.Id,
            TypeReparation = "Simulation",
            DateDebut = DateTime.UtcNow,
            Statut = "En cours"
        };

        _context.Reparations.Add(reparation);
        await _context.SaveChangesAsync();

        await Task.Delay(2000);

        reparation.Statut = "Terminée";
        await _context.SaveChangesAsync();

        var dateDuJour = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        _journalService.Ecrire($"{dateDuJour} - {ordinateur.Nom} réparé");
    }
}
