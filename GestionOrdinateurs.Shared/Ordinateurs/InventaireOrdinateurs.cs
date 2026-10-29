using System.Collections;

namespace GestionOrdinateurs.Shared.Ordinateurs;

public class InventaireOrdinateurs : IReadOnlyCollection<Ordinateur>
{
    private readonly List<Ordinateur> _ordinateurs = new();

    public int Count => _ordinateurs.Count;

    public List<Ordinateur> Ordinateurs
    {
        get
        {
            return _ordinateurs;
        }
        set
        {
            _ordinateurs.Clear();
            if (value != null)
                _ordinateurs.AddRange(value);
        }
    }

    public Ordinateur? this[int index] =>
        _ordinateurs.FirstOrDefault(ordinateur => ordinateur.Id == index);

    public Ordinateur? this[string reference] =>
        _ordinateurs.FirstOrDefault(ordinateur =>
            string.Equals(ordinateur.Nom, reference, StringComparison.OrdinalIgnoreCase));

    public void Add(Ordinateur ordinateur)
    {
        if (ordinateur != null)
            _ordinateurs.Add(ordinateur);
    }

    public IEnumerator<Ordinateur> GetEnumerator() => _ordinateurs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
