namespace CRC.Foundation;

public interface IDatabaseProbe
{
    Task<bool> IsReadyAsync(CancellationToken cancellationToken);
}
