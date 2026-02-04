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

        #region DocumentQa

        public async Task<DocumentQa?> GetDocumentQaByIdAsync(int id)
        {
            return await _dbContext.DocumentQas
                .AsNoTracking()
                .OrderByDescending(qa => qa.Id)
                .FirstOrDefaultAsync(qa => qa.Id == id);
        }
        public async Task<DocumentQa> CreateDocumentQaAsync(int analysisId, string question)
        {
            var qa = new DocumentQa
            {
                AnalysisId = analysisId,
                Question = question,
                Status = 0, // начальный статус
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.DocumentQas.Add(qa);
            await _dbContext.SaveChangesAsync();
            return qa;
        }

        public async Task<List<DocumentQa>> GetDocumentQasByAnalysisIdAsync(int analysisId)
        {
            return await _dbContext.DocumentQas
                .AsNoTracking()
                .Where(qa => qa.AnalysisId == analysisId)
                .OrderByDescending(qa => qa.Id)
                .ToListAsync();
        }

        public async Task UpdateDocumentQaStatusAsync(int id, int status)
        {
            var qa = await _dbContext.DocumentQas.FindAsync(id);
            if (qa != null)
            {
                qa.Status = status;
               
                await _dbContext.SaveChangesAsync();
            }
        }
        #endregion

        #region DocumentAnalysis

        public async Task<DocumentAnalysis?> GetDocumentAnalysisByIdAsync(int id)
        {
            return await _dbContext.DocumentAnalysiss
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<DocumentAnalysis?> GetDocumentAnalysisWithIncludesAsync(int id)
        {
            return await _dbContext.DocumentAnalysiss
                .AsNoTracking()
                .Include(a => a.DocumentGenericQas)
                .Include(a => a.DocumentQas)
                .Where(a => a.Id == id)
                .OrderByDescending(a => a.DocumentId)
                .FirstOrDefaultAsync();
        }

        public async Task<DocumentAnalysis?> GetDocumentAnalysisByDocumentIdAsync(int documentId)
        {
            return await _dbContext.DocumentAnalysiss
                .AsNoTracking()
                .Where(a => a.DocumentId == documentId)
                .OrderByDescending(a => a.DocumentId)
                .FirstOrDefaultAsync();
        }

        public async Task<DocumentAnalysis> CreateDocumentAnalysisAsync(int documentId)
        {
            var analysis = new DocumentAnalysis
            {
                DocumentId = documentId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.DocumentAnalysiss.Add(analysis);
            await _dbContext.SaveChangesAsync();
            return analysis;
        }

        public async Task<List<DocumentGenericQa>> GetDocumentGenericQasByAnalysisIdAsync(int analysisId)
        {
            return await _dbContext.DocumentGenericQas
                .AsNoTracking()
                .Where(gqa => gqa.AnalysisId == analysisId)
                .ToListAsync();
        }
        #endregion
    }
}
