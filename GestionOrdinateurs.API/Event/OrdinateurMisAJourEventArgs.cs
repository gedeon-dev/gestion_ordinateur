namespace GestionOrdinateurs.API.Event;

using GestionOrdinateurs.Shared.Ordinateurs;

public class OrdinateurMisAJourEventArgs : EventArgs
{
    public OrdinateurMisAJourEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }

    public Ordinateur Ordinateur { get; }
}