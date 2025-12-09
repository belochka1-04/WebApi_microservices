using FluentAssertions;
using KameraData.Data;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;

namespace WebApi_Applications.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //  ConfigureServices
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                    typeof(DbContextOptions<KameraDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Только InMemory БД
                services.AddDbContext<KameraDbContext>(options =>
                    options.UseInMemoryDatabase("IntegrationTestDb"));
            });
        }
    }

    // Тесты с ИЗОЛИРОВАННЫМ состоянием БД
    public class ApplicationControllerIntegrationTests :
        IClassFixture<CustomWebApplicationFactory>,
        IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ApplicationControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        
        public async Task InitializeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KameraDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Applications для тестов
            context.Applications.AddRange(
                new Application { Id = 1, Name = "TestApp1", CanStartInStarter = 0, Order = 1 },
                new Application { Id = 2, Name = "TestApp2", CanStartInStarter = 0, Order = 2 }
            );

            //  ApplicationLog для тестов
            context.ApplicationLogs.AddRange(
    new ApplicationLog
    {
        Id = 1,
        ApplicationId = 1,
        RecDate = DateTime.Now.AddHours(-1),
        RecType = "ERROR",
        Text = "Test Error 1",
        Description = "Desc 1"
    },
    new ApplicationLog
    {
        Id = 2,
        ApplicationId = 1,
        RecDate = DateTime.Now.AddMinutes(-30),
        RecType = "WARN",
        Text = "Test Warning",
        Description = ""
    }
);

            await context.SaveChangesAsync();
        }


        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetApplicationList_ReturnsOkAndList()
        {
            var response = await _client.GetAsync("/api/application/get");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var apps = await response.Content.ReadFromJsonAsync<List<Application>>();
            apps.Should().NotBeNull();
            apps.Should().HaveCount(2); // ✅ TestApp1 + TestApp2
        }

        [Fact]
        public async Task GetApplicationName_ReturnsSingleApp()
        {
            var response = await _client.GetAsync("/api/application/getapp/TestApp1"); // ✅ Фикс
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var app = await response.Content.ReadFromJsonAsync<Application>();
            app.Should().NotBeNull();
            app.Name.Should().Be("TestApp1"); // Фикс
        }

        [Fact]
        public async Task UpdateApplication_ReturnsNoContent()
        {
            // Arrange - БД с TestApp1 (CanStartInStarter = 0)

            var appToUpdate = new Application
            {
                Id = 1,
                Name = "UpdatedName",  // Меняем имя
                CanStartInStarter = 1, // ❌ Отправляем 1
                Order = 10
            };

            var response = await _client.PutAsJsonAsync("/api/application/update/1", appToUpdate);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // ✅ Assert - проверяем бизнес-логику метода
            var getResponse = await _client.GetAsync("/api/application/getapp/UpdatedName");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var app = await getResponse.Content.ReadFromJsonAsync<Application>();
            app.Should().NotBeNull();
            app.Name.Should().Be("UpdatedName");           // ✅ Имя обновилось
            app.CanStartInStarter.Should().Be(0);          // ✅ Метод принудительно поставил 0
            app.Order.Should().Be(10);                     // ✅ Order обновился
        }

        [Fact]
        public async Task GetApplicationList_FiltersCanStartInStarter()
        {
            // Arrange - создаем app с CanStartInStarter = 1 (не должен попасть в список)
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KameraDbContext>();
            context.Applications.Add(new Application
            {
                Id = 3,
                Name = "StarterApp",
                CanStartInStarter = 1,
                Order = 0
            });
            await context.SaveChangesAsync();

            var response = await _client.GetAsync("/api/application/get");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var apps = await response.Content.ReadFromJsonAsync<List<Application>>();
            apps.Should().HaveCount(2); // Только TestApp1 + TestApp2 (не StarterApp!)
            apps.Should().NotContain(a => a.Name == "StarterApp");
        }

        [Fact]
        public async Task GetApplicationName_NotFound_ReturnsEmpty()
        {
            var response = await _client.GetAsync("/api/application/getapp/NonExistent");
            response.StatusCode.Should().Be(HttpStatusCode.OK); // ✅ Метод возвращает Empty, не 404

            var apps = await response.Content.ReadFromJsonAsync<List<Application>>();
            apps.Should().BeEmpty(); // ✅ Graceful handling
        }

        [Fact]
        public async Task GetApplicationLogList_WithData_ReturnsOkAndList()
        {
            // Act
            var response = await _client.GetAsync("/api/applicationlog/get");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert
            var logs = await response.Content.ReadFromJsonAsync<List<ApplicationLog>>();
            logs.Should().NotBeNull();
            logs.Should().HaveCount(2);
            logs.Should().Contain(l => l.RecType == "ERROR");
            logs.Should().Contain(l => l.RecType == "WARN");
        }

        [Fact]
        public async Task GetApplicationLogList_Empty_ReturnsNoContent()
        {
            // Arrange - чистим логи
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KameraDbContext>();
            context.ApplicationLogs.RemoveRange(context.ApplicationLogs);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/applicationlog/get");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent); // ✅ 204 из контроллера
        }

        [Fact]
        public async Task InsertApplicationLog_ValidData_ReturnsNoContent()
        {
            // Act
            var log = new ApplicationLog
            {
                RecType = "CRITICAL",
                Text = "New Critical Error",
                Description = "Test Description"
            };

            var response = await _client.PutAsJsonAsync("/api/applicationlog/insert/TestApp1", log);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent); // ✅ 204

            // Assert - проверяем создание
            var logsResponse = await _client.GetAsync("/api/applicationlog/get");
            var logs = await logsResponse.Content.ReadFromJsonAsync<List<ApplicationLog>>();
            logs.Should().Contain(l => l.Text == "New Critical Error");
            logs.Should().Contain(l => l.RecType == "CRITICAL");
            logs.Should().Contain(l => l.ApplicationId == 1); // TestApp1.Id
        }

        [Fact]
        public async Task InsertApplicationLog_InvalidData_ReturnsBadRequest()
        {
            // Act - пустые поля (валидация контроллера)
            var invalidLog = new ApplicationLog
            {
                RecType = "",      // ❌ Пустой
                Text = "",         // ❌ Пустой
                Description = "Desc"
            };

            var response = await _client.PutAsJsonAsync("/api/applicationlog/insert/TestApp1", invalidLog);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest); // 400

            var errorContent = await response.Content.ReadAsStringAsync();
            errorContent.Should().Contain("cannot be null"); // Из контроллера
        }

        [Fact]
        public async Task InsertApplicationLog_NonExistentApp_ReturnsNoContent()
        {
            // Act - несуществующее приложение (метод Insert игнорирует)
            var log = new ApplicationLog
            {
                RecType = "ERROR",
                Text = "NonExistent App Error"
            };

            var response = await _client.PutAsJsonAsync("/api/applicationlog/insert/NonExistentApp", log);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent); // ✅ 204 (graceful)

            // Assert - лог НЕ создался (логика сервиса)
            var logsResponse = await _client.GetAsync("/api/applicationlog/get");
            var logs = await logsResponse.Content.ReadFromJsonAsync<List<ApplicationLog>>();
            logs.Should().NotContain(l => l.Text == "NonExistent App Error");
        }

        [Fact]
        public async Task InsertApplicationLog_EmptyApplicationName_ReturnsBadRequest()
        {
            // Act - пустое имя приложения
            var log = new ApplicationLog
            {
                RecType = "ERROR",
                Text = "Empty App Error"
            };

            var response = await _client.PutAsJsonAsync("/api/applicationlog/insert/", log); // 
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest); // 400 из контроллера
        }


    }
}
