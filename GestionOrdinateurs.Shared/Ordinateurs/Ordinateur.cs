using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionOrdinateurs.Shared.Ordinateurs;

[Table("Ordinateur")]
public class Ordinateur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public EtatOrdinateur Etat { get; set; }
    [DefaultValue(0)]
    public int Stock { get; set; }

    [DefaultValue(0)]
    public decimal Prix { get; set; }

    public bool Vendu { get; set; } = false;

    public Caracteristiques Caracteristiques { get; set; } = new(0, 0, TypeOrdinateur.Portable);
    public List<Reparation> Reparations { get; set; } = new();

    public virtual string GetDetails()
    {
        return $"ID: {Id}, Nom: {Nom}, Type: {Caracteristiques.TypeOrdi}, RAM: {Caracteristiques.RamGo}GB, Stockage: {Caracteristiques.StockageGo}GB, Etat: {Etat}, Stock: {Stock}";
    }

    public void SetEtat(EtatOrdinateur etat)
    {
        Etat = etat;
    }
}