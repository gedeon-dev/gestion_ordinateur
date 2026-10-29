using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.Shared.Ordinateurs;

namespace GestionOrdinateurs.API.Services.Interfaces;

public interface IReparationService
{
    Task SimulerReparationAsync(Ordinateur ordinateur);
    Task<List<Reparation>> GetAllAsync();
    Task<List<ReparationService.HistoriqueReparationRecord>> GetHistoriqueReparationsAsync();
}
