using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MonitoradorLogs.Data;
using MonitoradorLogs.Models;

public class LogUpdater : IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<LogUpdater> _logger;
    private readonly List<Log> _logs = new List<Log>();
    private readonly Task _updateTask;
    private readonly CancellationTokenSource _cancellationTokenSource;
    public DateTime ult_atualizacao;

    public LogUpdater(IServiceProvider serviceProvider, ILogger<LogUpdater> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();

        // Inicialize o Task no construtor
        _updateTask = Task.Run(() => AtualizaLogsPeriodicamente(_cancellationTokenSource.Token));
    }

    public IReadOnlyList<Log> Logs => _logs;

    private async Task AtualizaLogsPeriodicamente(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                try
                {
                    var logsFromDb = await context.Log.ToListAsync(cancellationToken);
                    _logger.LogInformation("Logs recuperados com sucesso, quantidade: " + logsFromDb.Count);

                    lock (_logs) // Usar lock para evitar problemas de concorrência
                    {
                        _logger.LogInformation("Atualizando logs");
                        _logs.Clear();
                        _logs.AddRange(logsFromDb);
                        _logger.LogInformation("Logs atualizados, quantidade: " + _logs.Count);
                        ult_atualizacao = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar logs");
                }
            }

            await Task.Delay(30000, cancellationToken); // Espera por 30 segundos
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _updateTask.Wait(); // Opcional: espere que a tarefa seja concluída
        _cancellationTokenSource.Dispose();
    }
}
