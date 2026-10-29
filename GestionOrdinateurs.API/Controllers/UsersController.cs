using GestionOrdinateurs.API.Models;
using GestionOrdinateurs.Shared.Authentification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionOrdinateurs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ParcInformatiqueContext _db;

    public UsersController(ParcInformatiqueContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        return Ok(await _db.Users.ToListAsync());
    }

    [HttpGet("{login}")]
    public async Task<ActionResult<User>> GetByLogin(string login)
    {
        var user = await _db.Users.SingleOrDefaultAsync(user => user.Login == login);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
