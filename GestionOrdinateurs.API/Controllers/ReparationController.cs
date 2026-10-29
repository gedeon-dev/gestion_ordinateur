using GestionOrdinateurs.API.Services.Interfaces;
using GestionOrdinateurs.API.Services;
using GestionOrdinateurs.Shared.Ordinateurs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionOrdinateurs.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ReparationController : ControllerBase
{
    private IReparationService _reparationService;
    private IOrdinateurService _ordinateurService;
    private StockService _stockService;
    private ILogger<ReparationController> _logger;

    public ReparationController(
        IReparationService reparationService,
        IOrdinateurService ordinateurService,
        StockService stockService,
        ILogger<ReparationController> logger)
    {
        _reparationService = reparationService;
        _ordinateurService = ordinateurService;
        _stockService = stockService;
        _logger = logger;
    }

    [HttpPost("SimulerReparation")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SimulerReparation()
    {
        List<Ordinateur> listOrdinateurs = await _ordinateurService.GetAllAsync();
        if (listOrdinateurs.Count < 3)
        {
            return BadRequest("Il faut au moins trois ordinateurs pour lancer la simulation.");
        }

        foreach (var ordinateur in listOrdinateurs.Take(3))
        {
            await _reparationService.SimulerReparationAsync(ordinateur);
        }

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<List<Reparation>>> GetAll()
    {
        var reparations = await _reparationService.GetAllAsync();
        return Ok(reparations);
    }

    [HttpGet("historique")]
    public async Task<ActionResult<List<ReparationService.HistoriqueReparationRecord>>> GetHistorique()
    {
        var historique = await _reparationService.GetHistoriqueReparationsAsync();
        return Ok(historique);
    }

    [HttpGet("GetStockSSD")]
    public async Task<IActionResult> GetStockSSD()
    {
        await Task.WhenAll(
            Enumerable.Range(0, 10)
                .Select(_ => _stockService.ConsommerSSD()));

        _logger.LogInformation("Stock SSD restant : {StockSSD}", _stockService.StockSSD);
        return Ok(_stockService.StockSSD);
    }
}
