using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using NLog;
using SharedMicroserviceLibrary.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebApi_document.Services;

namespace WebApi_document.Services
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

        #region Document
        public async Task<KameraData.Data.Models.Document> GetDocByIdAsync(int ID)
        {
            try
            {
                if (_dbContext.Documents.Any(j => j.Id == ID))
                {
                    return await _dbContext.Documents.Where(j => j.Id == ID).FirstOrDefaultAsync();

                }
                else
                    return new KameraData.Data.Models.Document();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + "документ ид:" + ID + " - " + ex);
                return new KameraData.Data.Models.Document();
            }
        }
        #endregion

        #region DocumentPdfText
        public async Task<DocumentPdfText> GetDocPdfByIdAsync(int docID)
        {
            try
            {
                if (_dbContext.DocumentPdfTexts.Any(j => j.DocumentId == docID))
                {
                    return await _dbContext.DocumentPdfTexts.Where(j => j.DocumentId == docID).FirstOrDefaultAsync();

                }
                else
                    return new DocumentPdfText();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + "документ ид:" + docID + " - " + ex);
                return new DocumentPdfText();
            }
        }
        #endregion
    }
}
