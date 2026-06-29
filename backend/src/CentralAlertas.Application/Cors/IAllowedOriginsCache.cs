namespace CentralAlertas.Application.Cors;

// Mantém em memória o conjunto de origens permitidas (config + banco) usado
// pela política de CORS. Atualizado no startup e após cada alteração do CRUD.
public interface IAllowedOriginsCache
{
    bool IsAllowed(string origin);

    Task RefreshAsync(CancellationToken cancellationToken = default);
}
