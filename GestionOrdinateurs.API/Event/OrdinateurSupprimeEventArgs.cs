namespace GestionOrdinateurs.API.Event;

using GestionOrdinateurs.Shared.Ordinateurs;

public class OrdinateurSupprimeEventArgs : EventArgs
{
    public OrdinateurSupprimeEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }

    public Ordinateur Ordinateur { get; }
}