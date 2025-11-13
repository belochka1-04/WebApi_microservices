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
using WebApi_JobService.Services;

namespace WebApi_JobService.Services
{
    public class DatabaseService : IDatabaseService, IDatabaseHealthCheck
    {
        private readonly KameraDbContext _dbContext;
        private readonly NLog.ILogger _logger;
        private readonly UserServiceClient _userServiceClient;

        public DatabaseService(KameraDbContext dbContext, UserServiceClient userServiceClient)
        {
            _dbContext = dbContext;
            _userServiceClient = userServiceClient;
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

        #region job_description_and_notes
        public async Task<IEnumerable<JobDescriptionAndNote>> GetJobsAsync()
        {
            try
            {
                return await _dbContext.JobDescriptionAndNotes
                    .Where(j => j.Status == "0")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error("Ошибка при получении заданий: " + ex);
                return new List<JobDescriptionAndNote>();
            }
        }

        public async Task<IEnumerable<JobDescriptionAndNote>> GetJandD(int jobID)
        {
            try
            {
                return await _dbContext.JobDescriptionAndNotes
                    .Where(j => j.JobId == jobID)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + "джоб ид:" + jobID + " - " + ex);
                return new List<JobDescriptionAndNote>();
            }
        }

        public async Task UpdateDescriptionAndNotesStatusAsync(int jobId, int status)
        {
            try
            {
                var jobDescriptions = await _dbContext.JobDescriptionAndNotes
                    .Where(j => j.JobId == jobId)
                    .ToListAsync();

                foreach (var jobDescription in jobDescriptions)
                {
                    jobDescription.Status = status.ToString();
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + ex);
            }
        }
        public async Task UpdateDescriptionAndNotesDescriptionAsync(int jobId, string fulltext)
        {
            try
            {
                var jobDescriptions = await _dbContext.JobDescriptionAndNotes
                    .Where(j => j.JobId == jobId)
                    .ToListAsync();

                foreach (var jobDescription in jobDescriptions)
                {
                    jobDescription.JobDescription = fulltext;
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + ex);
            }
        }

        public async Task UpdateDescriptionAndNotesPicCountAsync(int jobId, int pic_count)
        {
            try
            {
                var jobDescriptions = await _dbContext.JobDescriptionAndNotes
                    .Where(j => j.JobId == jobId)
                    .ToListAsync();

                foreach (var jobDescription in jobDescriptions)
                {
                    jobDescription.PicRecon = pic_count;
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + ex);
            }
        }
        public async Task InsertDescriptionAndNotesDescriptionAsync(int jobId, string fulltext)
        {
            try
            {
                var jobDescription = new JobDescriptionAndNote
                {
                    JobId = jobId,
                    JobDescription = fulltext.Replace('\'', ' '),
                    Status = "0" // или любое другое значение по умолчанию
                };

                await _dbContext.JobDescriptionAndNotes.AddAsync(jobDescription);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + ex);
            }
        }
        #endregion

        #region jobs
        public async Task<Job> GetJobByIdAsync(int jobId)
        {
            try
            {
                var job = await _dbContext.Jobs.Include(x => x.User)
                    .Where(j => j.Id == jobId)
                    .Select(j => new Job
                    {
                        Id = j.Id,
                        UserId = j.UserId,
                        JobNumber = j.JobNumber,
                        JobLink = j.JobLink,
                        Token = j.Token,
                        UpdateRequestStatus = j.UpdateRequestStatus,
                        User = j.User,
                    })
                    .FirstOrDefaultAsync();

                if (job == null)
                {
                    throw new Exception("Job not found.");
                }

                return job;
            }
            catch (Exception ex)
            {
                _logger.Error("Ошибка при получении заданий: " + ex);
                return new Job(); // Возвращаем новый экземпляр Job в случае ошибки
            }
        }
        public async Task<IEnumerable<Job>> GetJobByLink(string jobLink)
        {
            try
            {
                var jobs = await _dbContext.Jobs
                    .Where(j => j.JobLink == jobLink)
                    .Select(j => new Job
                    {
                        Id = j.Id,
                        UserId = j.UserId,
                        JobNumber = j.JobNumber
                    })
                    .ToListAsync();

                return jobs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                _logger.Error("Ошибка:" + "джоб линк:" + jobLink + " - " + ex);
                return new List<Job>();
            }
        }

        public async Task<Job> AddJobByTgAsync(int Id)
        {
            try
            {
               // var userId = _dbContext.Users.Where(x => x.TelegramId == Id).FirstOrDefault(); заменяем на работу с сервисом UserService
                var userId = await _userServiceClient.GetUserByTelegramIdAsync(Id); // Внешний http/rpc-клиент

                if (userId == null || userId.Id < 1)
                {
                    throw new Exception($"Пользователь с Telegram ID {Id} не найден.");
                }
                // Находим максимальный JobNumber
                var maxJobNumber = await _dbContext.Jobs
                    .MaxAsync(j => Convert.ToInt32(j.JobNumber)); // Получаем максимальный номер или 0, если нет заданий

                // Создаем новый объект Job
                var newJob = new Job
                {
                    UserId = userId.Id,
                    JobNumber = (maxJobNumber + 1).ToString(), // Уникальный номер задания
                    JobLink = string.Empty, // Пустая строка для JobLink
                    UpdateRequestStatus = string.Empty // Пустая строка для UpdateRequestStatus
                };

                // Добавляем объект в контекст
                await _dbContext.Jobs.AddAsync(newJob);

                // Сохраняем изменения в базе данных
                await _dbContext.SaveChangesAsync();

                return newJob; // Возвращаем добавленный объект
            }
            catch (Exception ex)
            {
                _logger.Error("Ошибка при добавлении задания: " + ex);
                throw; // Пробрасываем исключение дальше
            }
        }

        #endregion

    }
}
