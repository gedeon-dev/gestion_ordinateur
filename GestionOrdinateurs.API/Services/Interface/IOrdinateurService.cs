namespace GestionOrdinateurs.API.Services.Interfaces;

using GestionOrdinateurs.Shared.Ordinateurs;
using GestionOrdinateurs.Shared;
using GestionOrdinateurs.API.Event;

public interface IOrdinateurService
{
    Task<List<Ordinateur>> GetAllAsync();
    Task<Ordinateur?> GetByIdAsync(int id);
    Task<Ordinateur> CreateOrdinateurAsync(Ordinateur ordinateur);
    Task<Ordinateur?> UpdateOrdinateurAsync(int id, Ordinateur ordinateur);
    Task<bool> DeleteOrdinateurAsync(int id);
    Task<List<Ordinateur>> GetOrdinateursSortedByPriceAsync();
    Task<List<Ordinateur>> GetDisponiblesAsync();
    string Diagnostiquer(Ordinateur ordinateur);
    (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques();

    event EventHandler<OrdinateurAjouteEventArgs> OrdinateurAjoute;
    event EventHandler<OrdinateurSupprimeEventArgs> OrdinateurSupprime;
    event EventHandler<OrdinateurMisAJourEventArgs> OrdinateurMisAJour;
}