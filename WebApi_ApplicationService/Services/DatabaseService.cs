using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using NLog;
using SharedMicroserviceLibrary.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi_applications.Services;

namespace WebApi_applications.Services
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

        #region Application

        public async Task<IEnumerable<Application>> GetApplicationList()
        {
            try
            {
                return await _dbContext.Applications
                    .Where(x => x.CanStartInStarter != 1)
                    .OrderBy(x => x.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка получения списка приложений");
                return Enumerable.Empty<Application>();
            }
        }

        public async Task<IEnumerable<Application>> GetApplicationName(string name)
        {
            try
            {
                return await _dbContext.Applications
                    .Where(app => app.Name == name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка получения приложения по имени");
                return Enumerable.Empty<Application>();
            }
        }

        public async Task UpdateApplication(Application app)
        {
            try
            {
                app.CanStartInStarter = 0;
                _dbContext.Applications.Update(app);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка обновления приложения");
            }
        }

        #endregion

        #region ApplicationLog
        public async Task<IEnumerable<KameraData.Data.Models.ApplicationLog>> GetApplicationLogList()
        {
            try
            {
                // Получаем список приложений из базы данных
                return await _dbContext.ApplicationLogs.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error("Ошибка: полчить ошибки " + ex);
                return Enumerable.Empty<ApplicationLog>();
            }
        }

        public async Task InsertApplicationLogAsync(string applicationName, string wrnType, string wrnText, string desc = "")
        {
            try
            {
                if (wrnType.Length > 0 && applicationName.Length > 0 && wrnText.Length > 0)
                {
                    var app = _dbContext.Applications.FirstOrDefault(x => x.Name == applicationName);
                    if (app != null)
                    {
                        var log = new ApplicationLog
                        {
                            ApplicationId = app.Id,
                            RecDate = DateTime.Now,
                            RecType = wrnType,
                            Text = wrnText,
                            Description = desc
                        };

                        // Добавляем объект в контекст
                        await _dbContext.ApplicationLogs.AddAsync(log);
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка:не удалось создать запись об ошибке {ex}");
            }
        }
        #endregion
    }
}
