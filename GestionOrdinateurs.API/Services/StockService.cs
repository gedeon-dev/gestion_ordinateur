namespace GestionOrdinateurs.API.Services;

public class StockService
{
    private object _verrou = new();

    public int StockSSD { get; private set; } = 10;

    public async Task ConsommerSSD()
    {
        await Task.Delay(10);
        lock (_verrou)
        {
            StockSSD--;
        }
    }
}
