namespace GestionOrdinateurs.API.Event;


public class OrdinateurEventSubscriber
{
    private ILogger<OrdinateurEventSubscriber> _logger;

    public OrdinateurEventSubscriber(ILogger<OrdinateurEventSubscriber> logger)
    {
        _logger = logger;
    }

    public void AfficherMessageAjout(object? sender, OrdinateurAjouteEventArgs e)
    {
        _logger.LogInformation("[Événement] Ordinateur ajouté : {Nom} (ID {Id})", e.Ordinateur.Nom, e.Ordinateur.Id);
    }

    public void AfficherMessageSuppression(object? sender, OrdinateurSupprimeEventArgs e)
    {
        _logger.LogInformation("[Événement] Ordinateur supprimé : ID {Id}", e.Ordinateur.Id);
    }

    public void AfficherMessageMiseAJour(object? sender, OrdinateurMisAJourEventArgs e)
    {
        _logger.LogInformation("[Événement] Ordinateur mis à jour : {Nom} (ID {Id})", e.Ordinateur.Nom, e.Ordinateur.Id);
    }
}