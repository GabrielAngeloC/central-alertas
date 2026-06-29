using CentralAlertas.Domain.Entities;

namespace CentralAlertas.Application.Dashboard;

public interface IDashboardViewRepository
{
    // Admin: todas as visões não excluídas, ordenadas.
    Task<List<DashboardViewConfig>> GetAllAsync(CancellationToken cancellationToken);

    // Painel: apenas visões ativas, ordenadas.
    Task<List<DashboardViewConfig>> GetActiveOrderedAsync(CancellationToken cancellationToken);

    Task<DashboardViewConfig?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<DashboardViewConfig?> GetByCategoryAsync(string category, CancellationToken cancellationToken);

    Task AddAsync(DashboardViewConfig view, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
