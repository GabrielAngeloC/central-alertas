using CentralAlertas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CentralAlertas.Infrastructure.Persistence;

// Cria as visões iniciais do painel se a tabela estiver vazia.
public class DashboardViewSeeder
{
    private readonly CentralAlertasDbContext _dbContext;

    public DashboardViewSeeder(CentralAlertasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.DashboardViews.AnyAsync(cancellationToken);
        if (exists)
            return;

        var views = new[]
        {
            new DashboardViewConfig("pedidos", "Pedidos & Notas", 1),
            new DashboardViewConfig("faturamento", "Faturamento", 2),
            new DashboardViewConfig("cotacao", "Cotação de frete", 3),
            new DashboardViewConfig("infraestrutura", "Internet & Infra", 4),
            new DashboardViewConfig("estoque", "Estoque", 5),
        };

        await _dbContext.DashboardViews.AddRangeAsync(views, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
