namespace GestionOrdinateurs.API.Event;

using GestionOrdinateurs.Shared.Ordinateurs;

public class OrdinateurAjouteEventArgs : EventArgs
{
    public OrdinateurAjouteEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }

    public Ordinateur Ordinateur { get; }
}