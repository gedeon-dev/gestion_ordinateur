namespace GestionOrdinateurs.API.Services;

using GestionOrdinateurs.Shared.Ordinateurs;
using GestionOrdinateurs.API.Services.Interfaces;
using GestionOrdinateurs.API.Models;
using Microsoft.EntityFrameworkCore;
using GestionOrdinateurs.API.Event;
using System.Globalization;


public class OrdinateurService : IOrdinateurService
{
    private ParcInformatiqueContext _context;
    private JournalService _journalService;
    public event EventHandler<OrdinateurAjouteEventArgs>? OrdinateurAjoute;
    public event EventHandler<OrdinateurSupprimeEventArgs>? OrdinateurSupprime;
    public event EventHandler<OrdinateurMisAJourEventArgs>? OrdinateurMisAJour;


    public OrdinateurService(
        ParcInformatiqueContext context,
        ILogger<OrdinateurEventSubscriber> eventLogger,
        JournalService journalService)
    {
        _context = context;
        _journalService = journalService;
        OrdinateurEventSubscriber handler = new OrdinateurEventSubscriber(eventLogger);
        OrdinateurAjoute += handler.AfficherMessageAjout;
        OrdinateurSupprime += handler.AfficherMessageSuppression;
        OrdinateurMisAJour += handler.AfficherMessageMiseAJour;
    }

    public Task<List<Ordinateur>> GetAllAsync()
    {
        return _context.Ordinateurs.ToListAsync();
    }

    public Task<Ordinateur?> GetByIdAsync(int id)
    {
        return _context.Ordinateurs.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Ordinateur> CreateOrdinateurAsync(Ordinateur ordinateur)
    {
        _context.Ordinateurs.Add(ordinateur);
        await _context.SaveChangesAsync();
        OrdinateurAjoute?.Invoke(this, new OrdinateurAjouteEventArgs(ordinateur));
        EcrireAction("ajouté", ordinateur);
        return ordinateur;
    }

    public async Task<Ordinateur?> UpdateOrdinateurAsync(int id, Ordinateur updatedOrdinateur)
    {
        Ordinateur? existingOrdinateur = await GetByIdAsync(id);
        if (existingOrdinateur == null)
            return null;

        existingOrdinateur.Nom = updatedOrdinateur.Nom;
        existingOrdinateur.Caracteristiques = updatedOrdinateur.Caracteristiques;
        existingOrdinateur.Etat = updatedOrdinateur.Etat;
        existingOrdinateur.Prix = updatedOrdinateur.Prix;
        existingOrdinateur.Stock = updatedOrdinateur.Stock;
        existingOrdinateur.Vendu = updatedOrdinateur.Vendu;

        await _context.SaveChangesAsync();
        OrdinateurMisAJour?.Invoke(this, new OrdinateurMisAJourEventArgs(existingOrdinateur));
        EcrireAction("modifié", existingOrdinateur);

        return existingOrdinateur;
    }

    public async Task<bool> DeleteOrdinateurAsync(int id)
    {
        Ordinateur? ordinateur = await GetByIdAsync(id);
        if (ordinateur == null)
            return false;

        _context.Ordinateurs.Remove(ordinateur);
        await _context.SaveChangesAsync();
        OrdinateurSupprime?.Invoke(this, new OrdinateurSupprimeEventArgs(ordinateur));
        EcrireAction("supprimé", ordinateur);
        return true;
    }

    public async Task<List<Ordinateur>> GetOrdinateursSortedByPriceAsync()
    {
        // SQLite's decimal sort collation depends on the current culture.
        var ordinateurs = await _context.Ordinateurs.ToListAsync();
        return ordinateurs.OrderBy(o => o.Prix).ToList();
    }

    public Task<List<Ordinateur>> GetDisponiblesAsync()
    {
        return _context.Ordinateurs
            .Where(o => o.Stock > 0)
            .ToListAsync();
    }

    public string Diagnostiquer(Ordinateur ordinateur)
    {
        var diagnostic = ordinateur switch
        {
            { Etat: EtatOrdinateur.HorsService } => "Remplacement",
            {
                Etat: EtatOrdinateur.Lent,
                Caracteristiques: { RamGo: < 8 }
            } => "Ajouter RAM",
            _ => "Aucune action"
        };

        EcrireAction($"diagnostic effectué ({diagnostic}) sur", ordinateur);
        return diagnostic;
    }

    public (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations)
        CalculerStatistiques()
    {
        var ordinateurs = _context.Ordinateurs.ToList();

        return (
            TotalOrdinateur: ordinateurs.Count,
            OrdiDisponibles: ordinateurs.Count(ordinateur => ordinateur.Stock > 0),
            ValeurStock: ordinateurs.Sum(ordinateur => ordinateur.Prix * ordinateur.Stock),
            NbReparations: _context.Reparations.Count());
    }

    private void EcrireAction(string action, Ordinateur ordinateur)
    {
        var dateDuJour = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        _journalService.Ecrire($"{dateDuJour} - Ordinateur {ordinateur.Id} ({ordinateur.Nom}) {action}");
    }
}