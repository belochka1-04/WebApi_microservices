using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;
using SharedMicroserviceLibrary.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using WebApi_ImageJobService.Services;

namespace WebApi_ImageJobService.Services
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

        #region ImageText
        public async Task SaveTextToBDAsync(string text, string folderName, string fileName)
        {
            try
            {
                // Создаем новый объект ImageText
                var imageText = new ImageText
                {
                    folder = folderName,
                    file = fileName,
                    text = text.Replace('\'', ' ') // Заменяем одинарные кавычки
                };

                // Добавляем объект в контекст
                await _dbContext.ImageTexts.AddAsync(imageText);

                // Сохраняем изменения в базе данных
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка: папка: {folderName} - {fileName} {ex}");
            }
        }

        public async Task InsertImageFile(string folder_name, string file_name, string ocr_text, int is_rating_plate_percentage, decimal completion_cost, int image_tokens = 0, string brand = "NULL", string model = "NULL", string serial_number = "NULL")
        {
            try
            {
                // Создаем новый объект JobPic
                var req = new JobPic
                {
                    FileName = file_name,
                    ImageToken = image_tokens,
                    Brand = brand,
                    Model = model,
                    SerialNumber = serial_number,
                    FolderName = folder_name,
                    OcrText = ocr_text,
                    IsRating = is_rating_plate_percentage,
                    CompletionCosts = completion_cost
                };

                // Добавляем объект в контекст
                await _dbContext.JobPics.AddAsync(req);

                // Сохраняем изменения в базе данных
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка: папка: {folder_name} - {file_name} {ex}");
            }
        }

        public async Task UpdateJobPic(JobPic pic)
        {
            try
            {
                _dbContext.JobPics.Update(pic);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error($"Ошибка: + {ex}");
            }
        }

        public async Task<IEnumerable<JobPic>> GetJobPicList(string fileName)
        {
            try
            {
                // Получаем список приложений из базы данных
                return await _dbContext.JobPics
                   .Where(x => x.FileName == fileName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Logger logger = LogManager.GetCurrentClassLogger();
                logger.Error("Ошибка: не получилось найти перечень файлов - " + ex);
                return Enumerable.Empty<JobPic>();
            }
        }
        #endregion
    }
}
