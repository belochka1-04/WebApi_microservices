using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KameraData.Data.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace KameraData.Data;

public partial class KameraDbContext : DbContext
{
    public KameraDbContext()
    {
    }

    public KameraDbContext(DbContextOptions<KameraDbContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<ApplicationLog> ApplicationLogs { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }
    public virtual DbSet<Proxy> Proxies { get; set; }
    public virtual DbSet<ReplacesArchive> ReplacesArchives { get; set; }
    public virtual DbSet<PartSource> PartSources { get; set; }
    public virtual DbSet<PartsPicArchive> PartsPicArchives { get; set; }
    public virtual DbSet<PartsNamesArchive> PartsNamesArchives { get; set; }

    public virtual DbSet<JobDescriptionAndNote> JobDescriptionAndNotes { get; set; }

    public virtual DbSet<JobDoc> JobDocs { get; set; }

    public virtual DbSet<JobDocsModelInfo> JobDocsModelInfos { get; set; }

    public virtual DbSet<JobStock> JobStocks { get; set; }

    public virtual DbSet<JobStocksView> JobStocksViews { get; set; }

    public virtual DbSet<Mask> Masks { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    /// <summary>
    /// //////////////////////////new struct//////////////////////////////
    /// </summary>
    public virtual DbSet<Brand> Brands { get; set; }
    public virtual DbSet<BrandModel> BrandModels { get; set; }
    public virtual DbSet<Site> Sites { get; set; }
    public virtual DbSet<Document> Documents { get; set; }
    public virtual DbSet<DocumentType> DocumentTypes { get; set; }
    public virtual DbSet<ModelPart> ModelParts { get; set; }
    public virtual DbSet<ModelDocument> ModelDocuments { get; set; }
    public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
    public virtual DbSet<PricebotTask> PricebotTasks { get; set; }
    public virtual DbSet<UniqModel> UniqModels { get; set; }
    public virtual DbSet<DocumentPdfText> DocumentPdfTexts { get; set; }
    public virtual DbSet<PartsRequest> PartsRequests { get; set; }

    public virtual DbSet<DocumentQa> DocumentQas { get; set; }
    public virtual DbSet<DocumentAnalysis> DocumentAnalysiss { get; set; }
    public virtual DbSet<DocumentGenericQa> DocumentGenericQas { get; set; }
    public virtual DbSet<GenericQuestion> GenericQuestions { get; set; }


    /// <summary>
    /// //////////////////////////new struct//////////////////////////////
    /// </summary>

    public virtual DbSet<ModelNumber> ModelNumbers { get; set; }

    public virtual DbSet<ModelsNumbersNotFound> ModelsNumbersNotFounds { get; set; }
    public virtual DbSet<Parts> Parts { get; set; }
    public virtual DbSet<PartsAndReplace> PartsAndReplaces { get; set; }

    public virtual DbSet<Response> Responses { get; set; }
    public virtual DbSet<Sources> Sources { get; set; }


    public virtual DbSet<StockCred> StockCreds { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<StopWords> StopWord { get; set; }

    public virtual DbSet<ImageText> ImageTexts { get; set; }

    public virtual DbSet<JobPic> JobPics { get; set; }
    public virtual DbSet<JobDocInfo> JobDocsInfo { get; set; }

    public virtual DbSet<UserStock> UserStocks { get; set; }

    public virtual DbSet<LevaModel> LevaModels { get; set; }

    public virtual DbSet<SharedStock> SharedStocks { get; set; }

    public virtual DbSet<OnboardingVideo> OnboardingVideos { get; set; }

    public virtual DbSet<Issue> Issues { get; set; }

    public virtual DbSet<TipsVideo> TipsVideos { get; set; }
    public virtual DbSet<TipsLink> TipsLinks { get; set; }

    public virtual DbSet<ModelTb> ModelTbs { get; set; }

    public virtual DbSet<ApiClient> ApiClients { get; set; }

    public virtual DbSet<ModelLinkHistory> ModelLinkHistories { get; set; }
    

    public virtual DbSet<GoogleSerpRaw> GoogleSerpRaws { get; set; }

    public virtual DbSet<SiteTemplate> SiteTemplates { get; set; }
    public virtual DbSet<GoogleModelRequest> GoogleModelRequests { get; set; }

    public virtual DbSet<SiteTemplateTitleRule> SiteTemplateTitleRules { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

            optionsBuilder.UseSqlServer(_connectionString);
           
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       
       

        OnModelCreatingPartial(modelBuilder);

        modelBuilder
           .UseCollation("utf8_general_ci")
           .HasCharSet("utf8");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KameraDbContext).Assembly);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
