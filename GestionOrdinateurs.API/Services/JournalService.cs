namespace GestionOrdinateurs.API.Services;

public class JournalService
{
    private static string JournalDirectory = Path.Combine(
        "Logs",
        "Reparations_Logs",
        "Ordinateurs");
    private static string JournalPath = Path.Combine(JournalDirectory, "reparations.log");

    public void Ecrire(string message)
    {
        Directory.CreateDirectory(JournalDirectory);

        using var writer = new StreamWriter(JournalPath, append: true);
        writer.WriteLine(message);
    }
}
