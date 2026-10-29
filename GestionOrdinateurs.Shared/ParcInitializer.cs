namespace GestionOrdinateurs.Shared;

using GestionOrdinateurs.Shared.Ordinateurs;

public static class ParcInitializer
{
    public static List<Ordinateur> InitializeParc()
    {
        return new List<Ordinateur>
        {
            new Ordinateur { Id = 1, Nom = "PC Bureau 1", Caracteristiques = new(16, 512, TypeOrdinateur.Fixe), Etat = EtatOrdinateur.EnService },
            new Ordinateur { Id = 2, Nom = "PC Bureau 2", Caracteristiques = new(8, 256, TypeOrdinateur.Fixe), Etat = EtatOrdinateur.EnMaintenance },
            new Ordinateur { Id = 3, Nom = "Laptop 1", Caracteristiques = new(16, 512, TypeOrdinateur.Portable), Etat = EtatOrdinateur.EnService },
            new Ordinateur { Id = 4, Nom = "Laptop 2", Caracteristiques = new(8, 256, TypeOrdinateur.Portable), Etat = EtatOrdinateur.HorsService },
            new Ordinateur { Id = 5, Nom = "PC Bureau 3", Caracteristiques = new(32, 1024, TypeOrdinateur.Fixe), Etat = EtatOrdinateur.EnService }
        };
    }
}