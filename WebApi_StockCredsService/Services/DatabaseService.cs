using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;
using SharedMicroserviceLibrary.Middleware;

namespace WebApi_StockCredsService.Services
{
    public class DatabaseService : IDatabaseService, IDatabaseHealthCheck
    {
        private readonly KameraDbContext _dbContext;
        private readonly NLog.ILogger _logger;

        public DatabaseService(KameraDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<bool> IsDatabaseHealthyAsync()
        {
            try
            {
                return await _dbContext.Database.CanConnectAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка проверки подключения к базе данных");
                return false;
            }
        }

        #region stockCreds
        public async Task<List<StockCred>> GetStockCreds()
        {
            try
            {
                var result = await _dbContext.StockCreds.ToListAsync();
                return result;
            }
            catch (DbUpdateException ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка при получении stock creds): {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Необработанная ошибка при получении stock creds): {ex.Message}");
                throw;
            }
        }

        public async Task<List<StockCred>> ClaimDueStockCreds(string workerId, int batchSize, int leaseSeconds)
        {
            var safeBatchSize = Math.Clamp(batchSize, 1, 100);
            var safeLeaseSeconds = Math.Clamp(leaseSeconds, 60, 3600);
            var lockId = Guid.NewGuid();

            var batchSizeParam = new SqlParameter("@batchSize", safeBatchSize);
            var leaseSecondsParam = new SqlParameter("@leaseSeconds", safeLeaseSeconds);
            var lockIdParam = new SqlParameter("@lockId", lockId);
            var workerIdParam = new SqlParameter("@workerId", workerId);

            return await _dbContext.StockCreds
                .FromSqlRaw("""
DECLARE @claimed TABLE (id int NOT NULL PRIMARY KEY);

UPDATE TOP (@batchSize) dbo.stock_creds WITH (ROWLOCK, READPAST, UPDLOCK)
SET
    sync_lock_id = @lockId,
    sync_locked_by = @workerId,
    sync_lease_until = DATEADD(second, @leaseSeconds, SYSUTCDATETIME()),
    sync_attempt_count = ISNULL(sync_attempt_count, 0) + 1,
    sync_last_error = NULL
OUTPUT INSERTED.id INTO @claimed(id)
WHERE
    LOWER(sync_switch) = 'on'
    AND doc_types_id IS NOT NULL
    AND (
        update_time IS NULL
        OR DATEADD(second, TRY_CONVERT(int, sync_freq), update_time) < GETDATE()
    )
    AND (
        sync_lease_until IS NULL
        OR sync_lease_until < SYSUTCDATETIME()
    );

SELECT sc.*
FROM dbo.stock_creds AS sc
INNER JOIN @claimed AS c ON c.id = sc.id
ORDER BY sc.id;
""", batchSizeParam, leaseSecondsParam, lockIdParam, workerIdParam)
                .ToListAsync();
        }

        public async Task ReleaseStockCredLease(int stockCredId, string workerId)
        {
            var stockCredIdParam = new SqlParameter("@stockCredId", stockCredId);
            var workerIdParam = new SqlParameter("@workerId", workerId);

            await _dbContext.Database.ExecuteSqlRawAsync("""
UPDATE dbo.stock_creds
SET
    sync_lock_id = NULL,
    sync_locked_by = NULL,
    sync_lease_until = NULL
WHERE id = @stockCredId
  AND sync_locked_by = @workerId;
""", stockCredIdParam, workerIdParam);
        }

        public async Task<bool> UpdateSyncLease(int id, StockSyncLeaseRequest request, bool finish)
        {
            var seconds = Math.Clamp(request.LeaseSeconds, 60, 3600);
            var retry = Math.Clamp(request.RetryDelaySeconds, 1, 3600);
            var error = request.Error is { Length: > 1000 } ? request.Error[..1000] : request.Error;
            if (!finish)
                return await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
UPDATE dbo.stock_creds SET sync_lease_until=DATEADD(second,{seconds},SYSUTCDATETIME())
WHERE id={id} AND sync_locked_by={request.WorkerId} AND sync_lock_id={request.LockId}
AND sync_lease_until > SYSUTCDATETIME();
""") == 1;
            if (request.Success)
                return await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
UPDATE dbo.stock_creds SET sync_lock_id=NULL,sync_locked_by=NULL,sync_lease_until=NULL,sync_last_error=NULL
WHERE id={id} AND sync_locked_by={request.WorkerId} AND sync_lock_id={request.LockId}
AND sync_lease_until > SYSUTCDATETIME();
""") == 1;
            return await _dbContext.Database.ExecuteSqlInterpolatedAsync($"""
UPDATE dbo.stock_creds SET sync_lock_id=NULL,sync_locked_by=NULL,
sync_lease_until=DATEADD(second,{retry},SYSUTCDATETIME()),sync_last_error={error}
WHERE id={id} AND sync_locked_by={request.WorkerId} AND sync_lock_id={request.LockId}
AND sync_lease_until > SYSUTCDATETIME();
""") == 1;
        }

        public Task UpdateSyncState(int id, StockSyncStateRequest request) =>
            _dbContext.StockCreds.Where(stock => stock.Id == id).ExecuteUpdateAsync(setters => setters
                .SetProperty(stock => stock.UpdateTime, request.UpdateTime)
                .SetProperty(stock => stock.RecordsCount, request.RecordsCount)
                .SetProperty(stock => stock.SyncSwitch, request.SyncSwitch));

        public async Task UpdateStockCreds(StockCred stockCred)
        {
            try
            {
                _dbContext.StockCreds.Update(stockCred);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка: + {ex}");
            }
        }

        #endregion
    }
}
