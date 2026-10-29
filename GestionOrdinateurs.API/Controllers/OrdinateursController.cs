namespace GestionOrdinateurs.API.Controllers;

using GestionOrdinateurs.Shared.Ordinateurs;
using GestionOrdinateurs.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdinateursController : ControllerBase
{
    private IOrdinateurService _ordinateurService;
    private ILogger<OrdinateursController> _logger;
    private IConfiguration _configuration;

    public OrdinateursController(IOrdinateurService ordinateurService, ILogger<OrdinateursController> logger, IConfiguration configuration)
    {
        _ordinateurService = ordinateurService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        List<Ordinateur> ordinateurs = await _ordinateurService.GetAllAsync();
        _logger.LogInformation("Récupération de tous les ordinateurs");
        return Ok(ordinateurs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        Ordinateur? ordinateur = await _ordinateurService.GetByIdAsync(id);
        if (ordinateur == null)
        {
            _logger.LogWarning($"Ordinateur avec l'ID {id} non trouvé");
            return NotFound();
        }

        _logger.LogInformation($"Récupération de l'ordinateur avec l'ID {id}");
        return Ok(ordinateur);
    }

    [HttpGet("inventaire/{nom}")]
    public async Task<IActionResult> Chercher(string nom)
    {
        var inventaire = new InventaireOrdinateurs
        {
            Ordinateurs = await _ordinateurService.GetAllAsync()
        };

        var ordinateur = inventaire[nom];
        if (ordinateur == null)
            return NotFound();

        return Ok(ordinateur);
    }

    [HttpGet("{id}/diagnostic")]
    public async Task<IActionResult> GetDiagnostic(int id)
    {
        Ordinateur? ordinateur = await _ordinateurService.GetByIdAsync(id);
        if (ordinateur == null)
        {
            _logger.LogWarning("Ordinateur avec l'ID {Id} non trouvé pour diagnostic", id);
            return NotFound();
        }

        var diagnostic = _ordinateurService.Diagnostiquer(ordinateur);
        return Ok(diagnostic);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] Ordinateur ordinateur)
    {
        if (ordinateur == null)
        {
            _logger.LogWarning("Tentative de création d'un ordinateur avec des données nulles");
            return BadRequest();
        }

        var createdOrdinateur = await _ordinateurService.CreateOrdinateurAsync(ordinateur);
        _logger.LogInformation($"Ordinateur créé avec l'ID {createdOrdinateur.Id}");
        return CreatedAtAction(nameof(GetById), new { id = createdOrdinateur.Id }, createdOrdinateur);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] Ordinateur ordinateur)
    {
        if (ordinateur == null || ordinateur.Id != id)
        {
            _logger.LogWarning("Tentative de mise à jour d'un ordinateur avec des données invalides");
            return BadRequest();
        }

        var updatedOrdinateur = await _ordinateurService.UpdateOrdinateurAsync(id, ordinateur);
        if (updatedOrdinateur == null)
        {
            _logger.LogWarning($"Ordinateur avec l'ID {id} non trouvé");
            return NotFound();
        }

        _logger.LogInformation($"Ordinateur mis à jour avec l'ID {updatedOrdinateur.Id}");
        return Ok(updatedOrdinateur);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _ordinateurService.DeleteOrdinateurAsync(id);
        if (!deleted)
        {
            _logger.LogWarning($"Ordinateur avec l'ID {id} non trouvé");
            return NotFound();
        }

        _logger.LogInformation($"Ordinateur supprimé avec l'ID {id}");
        return NoContent();
    }

    /*[HttpGet("prix")]
    public IActionResult GetByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        var resultat = _ordinateurService.GetOrdinateursByPriceRange(minPrice, maxPrice).ToList();
        _logger.LogInformation($"Récupération des ordinateurs dans la plage de prix {minPrice} - {maxPrice}");
        return Ok(resultat);
    }*/

    [HttpGet("prix/tri")]
    public async Task<IActionResult> GetSortedByPrice()
    {
        var resultat = await _ordinateurService.GetOrdinateursSortedByPriceAsync();
        _logger.LogInformation("Récupération des ordinateurs triés par prix");
        return Ok(resultat);
    }

    [HttpGet("disponibles")]
    public async Task<IActionResult> GetDisponibles()
    {
        var resultat = await _ordinateurService.GetDisponiblesAsync();
        _logger.LogInformation("Récupération des ordinateurs ayant du stock");
        return Ok(resultat);
    }

    [HttpGet("statistiques")]
    public IActionResult GetStatistiques()
    {
        var (total, enStock, valeurStock, nbReparations) =
            _ordinateurService.CalculerStatistiques();

        return Ok(new
        {
            TotalOrdinateurs = total,
            OrdinateursEnStock = enStock,
            ValeurStock = valeurStock,
            NombreReparations = nbReparations
        });
    }

    [HttpGet("config")]
    public IActionResult GetConfig()
    {
        var config = new
        {
            ApplicationName = _configuration["Application:Name"],
            ApplicationVersion = _configuration["Application:Version"]
        };
        return Ok(config);
    }
}