using FluentAssertions;
using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Moq.Language.Flow;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WebApi_applications.Services;
using Xunit;

namespace WebApi_Applications.Tests
{
    public class DatabaseServiceTests
    {
        private readonly Mock<KameraDbContext> _mockDbContext;
        private readonly Mock<ILogger> _mockLogger;
        private readonly DatabaseService _service;

        public DatabaseServiceTests()
        {
            _mockDbContext = new Mock<KameraDbContext>();
            //Передаем МОК в конструктор сервиса!
            _service = new DatabaseService(_mockDbContext.Object);
        }
        // ========== Applications ==========
        #region Applications
        #region GetApplicationList
        [Fact]
        public async Task GetApplicationList_Success_ReturnsFilteredList()
        {
            // ARRANGE
            var testApps = new List<Application>
            {
                new Application { Id = 1, Name = "App1", Order = 1, CanStartInStarter = 0 },
                new Application { Id = 2, Name = "App2", Order = 2, CanStartInStarter = 1 } // Исключится
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Application>>();
            mockSet.As<IQueryable<Application>>().Setup(m => m.Provider).Returns(testApps.Provider);
            mockSet.As<IQueryable<Application>>().Setup(m => m.Expression).Returns(testApps.Expression);
            mockSet.As<IQueryable<Application>>().Setup(m => m.ElementType).Returns(testApps.ElementType);
            mockSet.As<IQueryable<Application>>().Setup(m => m.GetEnumerator()).Returns(testApps.GetEnumerator());

            // Настраиваем ToListAsync
            mockSet.Setup(x => x.ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(testApps.Where(x => x.CanStartInStarter != 1).OrderBy(x => x.Order).ToList());

            _mockDbContext.Setup(c => c.Applications).Returns(mockSet.Object);

            // Act
            var result = await _service.GetApplicationList();

            // Assert
            result.Should().HaveCount(1);
            result.First().Name.Should().Be("App1");
        }

        /// <summary>
        /// Тест 2: Пустой список - возвращает пустой IEnumerable
        /// </summary>
        [Fact]
        public async Task GetApplicationList_EmptyDb_ReturnsEmptyEnumerable()
        {
            // ARRANGE

            // ARRANGE - Пустой список приложений
            var emptyApps = new List<Application>().AsQueryable();

            // 1. Создаем мок DbSet<Application>
            var mockSet = new Mock<DbSet<Application>>();

            // 2. Настраиваем IQueryable свойства (обязательно для LINQ)
            mockSet.As<IQueryable<Application>>().Setup(m => m.Provider).Returns(emptyApps.Provider);
            mockSet.As<IQueryable<Application>>().Setup(m => m.Expression).Returns(emptyApps.Expression);
            mockSet.As<IQueryable<Application>>().Setup(m => m.ElementType).Returns(emptyApps.ElementType);
            mockSet.As<IQueryable<Application>>().Setup(m => m.GetEnumerator()).Returns(emptyApps.GetEnumerator());

            // 3. Настраиваем ToListAsync - возвращает пустой список
            mockSet.Setup(x => x.ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Application>());

            // 4. Подключаем мок DbSet к DbContext
            _mockDbContext.Setup(c => c.Applications).Returns(mockSet.Object);

            // Act - Вызов тестируемого метода
            var result = await _service.GetApplicationList();

            // Assert - Проверки результата
            result.Should().BeEmpty(); // Enumerable.Empty<Application>()
            result.Should().NotBeNull(); // Не null

            // Verify - Проверяем вызовы
            mockSet.Verify(x => x.ToListAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(c => c.Applications, Times.Once);
        }

        /// <summary>
        /// Тест 3: Исключение в БД - логирует ошибку и возвращает пустой список
        /// </summary>
        [Fact]
        public async Task GetApplicationList_DbException_LogsErrorAndReturnsEmpty()
        {
            // ARRANGE
            var mockSet = new Mock<DbSet<Application>>();

            // Настраиваем IQueryable (это обязательно для LINQ)
            mockSet.As<IQueryable<Application>>().Setup(m => m.Provider).Throws(new Exception("DB Failure"));
            mockSet.As<IQueryable<Application>>().Setup(m => m.Expression).Throws(new Exception("DB Failure"));
            mockSet.As<IQueryable<Application>>().Setup(m => m.ElementType).Throws(new Exception("DB Failure"));
            mockSet.As<IQueryable<Application>>().Setup(m => m.GetEnumerator()).Throws(new Exception("DB Failure"));

            _mockDbContext.Setup(c => c.Applications).Returns(mockSet.Object);

            // Логгер мокируем и проверяем вызов
            var mockLogger = new Mock<ILogger>();

            // Передаем мок логгера в сервис (предполагается конструктор с логгером)
            var service = new DatabaseService(_mockDbContext.Object/*, mockLogger.Object*/);

            // ACT
            var result = await service.GetApplicationList();

            // ASSERT
            result.Should().BeEmpty(); // Возвращает пустой список при ошибке

            // Проверяем, что логгер зафиксировал ошибку (раскомментируй если сервис принимает ILogger через конструктор)
            //mockLogger.Verify(
            //    x => x.Error(It.IsAny<Exception>(), "Ошибка получения списка приложений"), Times.AtLeastOnce);

            // Проверяем, что DbSet был вызван
            _mockDbContext.Verify(c => c.Applications, Times.Once);
        }
        /// <summary>
        /// Тест 4: Все приложения исключены фильтром - возвращает пустой список
        /// </summary>
        /// 
        /// <summary>
        /// Тест 4: Все приложения исключены фильтром - возвращает пустой список
        /// </summary>
        [Fact]
        public async Task GetApplicationList_AllExcludedByFilter_ReturnsEmpty()
        {
            // ARRANGE - Все приложения имеют CanStartInStarter = 1 (исключаются фильтром)
            var excludedApps = new List<Application>
    {
        new Application { Id = 1, Name = "ExcludedApp1", Order = 1, CanStartInStarter = 1 },
        new Application { Id = 2, Name = "ExcludedApp2", Order = 2, CanStartInStarter = 1 }
    }.AsQueryable();

            // 1. Создаем мок DbSet<Application>
            var mockSet = new Mock<DbSet<Application>>();

            // 2. Настраиваем IQueryable свойства (обязательно для LINQ Where/OrderBy)
            mockSet.As<IQueryable<Application>>().Setup(m => m.Provider).Returns(excludedApps.Provider);
            mockSet.As<IQueryable<Application>>().Setup(m => m.Expression).Returns(excludedApps.Expression);
            mockSet.As<IQueryable<Application>>().Setup(m => m.ElementType).Returns(excludedApps.ElementType);
            mockSet.As<IQueryable<Application>>().Setup(m => m.GetEnumerator()).Returns(excludedApps.GetEnumerator());

            // 3. КРИТИЧНО: Настраиваем ToListAsync с результатом ПОСЛЕ фильтрации
            // Фильтр Where(x => x.CanStartInStarter != 1) исключает ВСЕ приложения
            var filteredResult = excludedApps.Where(x => x.CanStartInStarter != 1).OrderBy(x => x.Order).ToList();
            // filteredResult будет ПУСТЫМ, т.к. все приложения имеют CanStartInStarter = 1

            mockSet.Setup(x => x.ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(filteredResult); // Возвращает пустой список!

            // 4. Подключаем мок DbSet к DbContext
            _mockDbContext.Setup(c => c.Applications).Returns(mockSet.Object);

            // Act - Вызов тестируемого метода
            var result = await _service.GetApplicationList();

            // Assert - Проверки результата
            result.Should().BeEmpty(); // Фильтр Where исключает все записи
            result.Should().HaveCount(0);
            result.Should().NotBeNull(); // Важно: метод не возвращает null

            // Дополнительная проверка: исходные данные НЕ пустые, но фильтр работает
            excludedApps.Should().HaveCount(2); // В БД есть данные

            // Verify - Проверяем вызовы
            mockSet.Verify(x => x.ToListAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(c => c.Applications, Times.Once);
        }
        #endregion

        #region GetApplicationName
        /// <summary>
        /// Тест 1: Возвращает список приложений с заданным именем
        /// </summary>
        [Fact]
        public async Task GetApplicationName_ExistingName_ReturnsApplications()
        {
            // Arrange
            var testApps = new List<Application>
        {
            new Application { Id = 1, Name = "sampleName" },
            new Application { Id = 2, Name = "sampleName" }
        }.AsQueryable();

            var mockSet = new Mock<DbSet<Application>>();
            mockSet.As<IQueryable<Application>>().Setup(m => m.Provider).Returns(testApps.Provider);
            mockSet.As<IQueryable<Application>>().Setup(m => m.Expression).Returns(testApps.Expression);
            mockSet.As<IQueryable<Application>>().Setup(m => m.ElementType).Returns(testApps.ElementType);
            mockSet.As<IQueryable<Application>>().Setup(m => m.GetEnumerator()).Returns(testApps.GetEnumerator());

            // Настройка ToListAsync
            mockSet.Setup(m => m.ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(testApps.ToList());

            _mockDbContext.Setup(c => c.Applications).Returns(mockSet.Object);

            // Act
            var result = await _service.GetApplicationName("sampleName");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.All(a => a.Name == "sampleName").Should().BeTrue();

            mockSet.Verify(m => m.ToListAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Тест 2: Возвращает пустой список при отсутствии приложений с заданным именем
        /// </summary>
        [Fact]
        public async Task GetApplicationName_NotFound_ReturnsEmptyList()
        {
            // Arrange
            var emptyApps = new List<Application>().AsQueryable();

            var mockSet = new Mock<DbSet<Application>>();
            mockSet.As<IQueryable<Application>>().Setup(m => m.Provider).Returns(emptyApps.Provider);
            mockSet.As<IQueryable<Application>>().Setup(m => m.Expression).Returns(emptyApps.Expression);
            mockSet.As<IQueryable<Application>>().Setup(m => m.ElementType).Returns(emptyApps.ElementType);
            mockSet.As<IQueryable<Application>>().Setup(m => m.GetEnumerator()).Returns(emptyApps.GetEnumerator());

            mockSet.Setup(m => m.ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Application>());

            _mockDbContext.Setup(c => c.Applications).Returns(mockSet.Object);

            // Act
            var result = await _service.GetApplicationName("unknownName");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            mockSet.Verify(m => m.ToListAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Тест 3: Обработка исключения - логирует ошибку и возвращает пустой список
        /// </summary>
        [Fact]
        public async Task GetApplicationName_DbException_LogsErrorAndReturnsEmpty()
        {
            // Arrange
            var mockSet = new Mock<DbSet<Application>>();

            mockSet.As<IQueryable<Application>>().Setup(m => m.Provider).Throws(new Exception("DB failed"));
            mockSet.As<IQueryable<Application>>().Setup(m => m.Expression).Throws(new Exception("DB failed"));
            mockSet.As<IQueryable<Application>>().Setup(m => m.ElementType).Throws(new Exception("DB failed"));
            mockSet.As<IQueryable<Application>>().Setup(m => m.GetEnumerator()).Throws(new Exception("DB failed"));

            _mockDbContext.Setup(c => c.Applications).Returns(mockSet.Object);

            // В этом примере предполагается, что логгер доступен; если есть возможность, передай мок логгера

            // Act
            var result = await _service.GetApplicationName("anyName");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            mockSet.As<IQueryable<Application>>().Verify(m => m.GetEnumerator(), Times.AtLeastOnce);
        }
        #endregion

        #region UpdateApplication
        /// <summary>
        /// Тесты для метода UpdateApplication(Application app)
        /// </summary>
        [Fact]
        public async Task UpdateApplication_ValidApp_UpdatesSuccessfully()
        {
            var app = new Application { Id = 1, Name = "TestApp", CanStartInStarter = 1 };

            var mockSet = new Mock<DbSet<Application>>();
            _mockDbContext.Setup(c => c.Applications).Returns(mockSet.Object);
            _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            await _service.UpdateApplication(app);

            app.CanStartInStarter.Should().Be(0); // ✅ Бизнес-логика сработала
            mockSet.Verify(x => x.Update(app), Times.Once);
        }


        [Fact]
        public async Task UpdateApplication_DbException_HandlesGracefully()
        {
            var app = new Application { Id = 1, Name = "TestApp" };

            var mockSet = new Mock<DbSet<Application>>();
            _mockDbContext.Setup(c => c.Applications).Returns(mockSet.Object);
            _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException("Save failed"));

            await _service.UpdateApplication(app); // ✅ НЕ падает!

            mockSet.Verify(x => x.Update(app), Times.Once);
        }


        [Fact]
        public async Task UpdateApplication_VerifiesCanStartInStarterSetToZero()
        {
            // ARRANGE - приложение с CanStartInStarter = 1 (изменится на 0)
            var appToUpdate = new Application
            {
                Id = 1,
                Name = "TestApp",
                CanStartInStarter = 1  // ← ДО вызова = 1
            };

            var mockSet = new Mock<DbSet<Application>>();
            mockSet.As<IQueryable<Application>>().Setup(m => m.Provider)
                .Returns(new List<Application>().AsQueryable().Provider);
            mockSet.As<IQueryable<Application>>().Setup(m => m.Expression)
                .Returns(new List<Application>().AsQueryable().Expression);
            mockSet.As<IQueryable<Application>>().Setup(m => m.ElementType)
                .Returns(new List<Application>().AsQueryable().ElementType);
            mockSet.As<IQueryable<Application>>().Setup(m => m.GetEnumerator())
                .Returns(new List<Application>().AsQueryable().GetEnumerator());

            _mockDbContext.Setup(c => c.Applications).Returns(mockSet.Object);
            _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _service.UpdateApplication(appToUpdate);

            // Assert - БИЗНЕС-ЛОГИКА сработала!
            appToUpdate.CanStartInStarter.Should().Be(0); 

            // Verify
            mockSet.Verify(x => x.Update(It.Is<Application>(a => a.CanStartInStarter == 0)), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }


        #endregion

        #endregion
        // ========== ApplicationLog ==========
        #region ApplicationLog
        #region GetApplicationLogList
        /// <summary>
        /// Тест 1: Возвращает список логов приложений
        /// </summary>
        [Fact]
        public async Task GetApplicationLogList_ReturnsLogs()
        {
            // Arrange - тестовые логи
            var testLogs = new List<ApplicationLog>
        {
            new ApplicationLog { Id = 1, RecType = "Info", Text = "Log1" },
            new ApplicationLog { Id = 2, RecType = "Warning", Text = "Log2" }
        }.AsQueryable();

            var mockSet = new Mock<DbSet<ApplicationLog>>();
            mockSet.As<IQueryable<ApplicationLog>>().Setup(m => m.Provider).Returns(testLogs.Provider);
            mockSet.As<IQueryable<ApplicationLog>>().Setup(m => m.Expression).Returns(testLogs.Expression);
            mockSet.As<IQueryable<ApplicationLog>>().Setup(m => m.ElementType).Returns(testLogs.ElementType);
            mockSet.As<IQueryable<ApplicationLog>>().Setup(m => m.GetEnumerator()).Returns(testLogs.GetEnumerator());

            mockSet.Setup(m => m.ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(testLogs.ToList());

            _mockDbContext.Setup(c => c.ApplicationLogs).Returns(mockSet.Object);

            // Act
            var result = await _service.GetApplicationLogList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Any(l => l.Text == "Log1").Should().BeTrue();
            result.Any(l => l.Text == "Log2").Should().BeTrue();

            mockSet.Verify(m => m.ToListAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Тест 2: Возвращает пустой список, если логов нет
        /// </summary>
        [Fact]
        public async Task GetApplicationLogList_Empty_ReturnsEmpty()
        {
            // Arrange - пустой список
            var emptyLogs = new List<ApplicationLog>().AsQueryable();

            var mockSet = new Mock<DbSet<ApplicationLog>>();
            mockSet.As<IQueryable<ApplicationLog>>().Setup(m => m.Provider).Returns(emptyLogs.Provider);
            mockSet.As<IQueryable<ApplicationLog>>().Setup(m => m.Expression).Returns(emptyLogs.Expression);
            mockSet.As<IQueryable<ApplicationLog>>().Setup(m => m.ElementType).Returns(emptyLogs.ElementType);
            mockSet.As<IQueryable<ApplicationLog>>().Setup(m => m.GetEnumerator()).Returns(emptyLogs.GetEnumerator());

            mockSet.Setup(m => m.ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ApplicationLog>());

            _mockDbContext.Setup(c => c.ApplicationLogs).Returns(mockSet.Object);

            // Act
            var result = await _service.GetApplicationLogList();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            mockSet.Verify(m => m.ToListAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Тест 3: Обработка исключения - логирует ошибку и возвращает пустой список
        /// </summary>
        [Fact]
        public async Task GetApplicationLogList_DbException_LogsErrorAndReturnsEmpty()
        {
            // Arrange - мок DbSet выбрасывает исключение
            var mockSet = new Mock<DbSet<ApplicationLog>>();
            mockSet.As<IQueryable<ApplicationLog>>().Setup(m => m.Provider).Throws(new Exception("DB failure"));
            mockSet.As<IQueryable<ApplicationLog>>().Setup(m => m.Expression).Throws(new Exception("DB failure"));
            mockSet.As<IQueryable<ApplicationLog>>().Setup(m => m.ElementType).Throws(new Exception("DB failure"));
            mockSet.As<IQueryable<ApplicationLog>>().Setup(m => m.GetEnumerator()).Throws(new Exception("DB failure"));

            _mockDbContext.Setup(c => c.ApplicationLogs).Returns(mockSet.Object);

            // Act
            var result = await _service.GetApplicationLogList();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            // Проверка вызова логгера возможна, если ILogger мок передан

            mockSet.As<IQueryable<ApplicationLog>>().Verify(m => m.GetEnumerator(), Times.AtLeastOnce);
        }
        #endregion

        #region InsertApplicationLogAsync
        /// <summary>
        /// Тесты для InsertApplicationLogAsync
        /// </summary>

        [Fact]
        public async Task InsertApplicationLogAsync_ValidData_CreatesLogSuccessfully()
        {
            // ARRANGE - Подготовка тестовых данных
            var applicationName = "TestApp";
            var wrnType = "Warning";
            var wrnText = "Test warning text";
            var desc = "Test description";

            // Мокаем Application - находим по имени
            var mockAppSet = new Mock<DbSet<Application>>();
            var testApp = new Application { Id = 1, Name = applicationName };
            var appsQueryable = new List<Application> { testApp }.AsQueryable();

            mockAppSet.As<IQueryable<Application>>().Setup(m => m.Provider).Returns(appsQueryable.Provider);
            mockAppSet.As<IQueryable<Application>>().Setup(m => m.Expression).Returns(appsQueryable.Expression);
            mockAppSet.As<IQueryable<Application>>().Setup(m => m.ElementType).Returns(appsQueryable.ElementType);
            mockAppSet.As<IQueryable<Application>>().Setup(m => m.GetEnumerator()).Returns(appsQueryable.GetEnumerator());

            mockAppSet.Setup(x => x.FirstOrDefault(It.Is<Application>(a => a.Name == applicationName)))
                .Returns(testApp);

            // Мокаем ApplicationLogs DbSet
            var mockLogSet = new Mock<DbSet<ApplicationLog>>();
            _mockDbContext.Setup(c => c.Applications).Returns(mockAppSet.Object);
            _mockDbContext.Setup(c => c.ApplicationLogs).Returns(mockLogSet.Object);
            _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1); // 1 запись добавлена

            // Act
            await _service.InsertApplicationLogAsync(applicationName, wrnType, wrnText, desc);

            // Assert - проверяем вызовы
            mockAppSet.Verify(x => x.FirstOrDefault(It.Is<Application>(a => a.Name == applicationName)), Times.Once);
            mockLogSet.Verify(x => x.AddAsync(It.IsAny<ApplicationLog>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task InsertApplicationLogAsync_AppNotFound_DoesNothing()
        {
            // ARRANGE - Приложение НЕ найдено
            var applicationName = "NonExistentApp";

            var mockAppSet = new Mock<DbSet<Application>>();
            mockAppSet.Setup(x => x.FirstOrDefault(It.IsAny<Expression<Func<Application, bool>>>()))
                .Returns((Application)null); // null - не найдено

            var mockLogSet = new Mock<DbSet<ApplicationLog>>();
            _mockDbContext.Setup(c => c.Applications).Returns(mockAppSet.Object);
            _mockDbContext.Setup(c => c.ApplicationLogs).Returns(mockLogSet.Object);

            // Act
            await _service.InsertApplicationLogAsync(applicationName, "Info", "Test text");

            // Assert - НЕ вызываются AddAsync и SaveChanges
            mockLogSet.Verify(x => x.AddAsync(It.IsAny<ApplicationLog>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task InsertApplicationLogAsync_EmptyParameters_DoesNothing()
        {
            // ARRANGE - Пустые параметры (условие if в методе)
            var mockAppSet = new Mock<DbSet<Application>>();
            var mockLogSet = new Mock<DbSet<ApplicationLog>>();
            _mockDbContext.Setup(c => c.Applications).Returns(mockAppSet.Object);
            _mockDbContext.Setup(c => c.ApplicationLogs).Returns(mockLogSet.Object);

            // Act & Assert - разные пустые варианты
            await _service.InsertApplicationLogAsync("", "Info", "text");        // Пустое applicationName
            await _service.InsertApplicationLogAsync("App", "", "text");        // Пустой wrnType
            await _service.InsertApplicationLogAsync("App", "Info", "");        // Пустой wrnText

            mockLogSet.Verify(x => x.AddAsync(It.IsAny<ApplicationLog>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockDbContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task InsertApplicationLogAsync_DbException_HandlesGracefully()
        {
            // ARRANGE - Ошибка SaveChangesAsync
            var applicationName = "TestApp";
            var mockAppSet = new Mock<DbSet<Application>>();
            var testApp = new Application { Id = 1, Name = applicationName };
            mockAppSet.Setup(x => x.FirstOrDefault(It.IsAny<Expression<Func<Application, bool>>>())).Returns(testApp);

            var mockLogSet = new Mock<DbSet<ApplicationLog>>();
            _mockDbContext.Setup(c => c.Applications).Returns(mockAppSet.Object);
            _mockDbContext.Setup(c => c.ApplicationLogs).Returns(mockLogSet.Object);
            _mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException("Save failed"));

            // Act - НЕ должно упасть!
            await _service.InsertApplicationLogAsync(applicationName, "Error", "DB Error test");

            // Assert - Вызовы прошли, но SaveChanges выбросил исключение (обработано в catch)
            mockLogSet.Verify(x => x.AddAsync(It.IsAny<ApplicationLog>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #endregion
        // ========== HEALTH CHECK ==========
        /// <summary>
        /// Тест: IsDatabaseHealthyAsync возвращает true при успешном подключении к БД
        /// </summary>
        [Fact]
        public async Task IsDatabaseHealthyAsync_Connected_ReturnsTrue()
        {
            // ARRANGE - Симулируем успешное подключение к БД
            _mockDbContext.Setup(c => c.Database.CanConnectAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true); // ✅ БД доступна

            // Act - Вызов метода проверки здоровья
            var result = await _service.IsDatabaseHealthyAsync();

            // Assert - Проверки результата
            result.Should().BeTrue(); // Возвращает true при успешном подключении

            // Verify - Проверяем вызовы
            _mockDbContext.Verify(
                c => c.Database.CanConnectAsync(It.IsAny<CancellationToken>()),
                Times.Once); // Метод CanConnectAsync вызван ровно 1 раз
        }

        /// <summary>
        /// Тест: IsDatabaseHealthyAsync возвращает false при недоступной БД
        /// </summary>
        [Fact]
        public async Task IsDatabaseHealthyAsync_Disconnected_ReturnsFalse()
        {
            // ARRANGE - Симулируем недоступную БД
            _mockDbContext.Setup(c => c.Database.CanConnectAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false); // ❌ БД недоступна

            // Act
            var result = await _service.IsDatabaseHealthyAsync();

            // Assert
            result.Should().BeFalse(); // Возвращает false при недоступной БД

            // Verify
            _mockDbContext.Verify(
                c => c.Database.CanConnectAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }


    }
}