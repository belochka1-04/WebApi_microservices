using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Document = KameraData.Data.Models.Document;

namespace KameraData.Data
{
    public class ModelsCreateing
    {


        public void OnModelMySqlCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                           .UseCollation("utf8_general_ci")
                           .HasCharSet("utf8");

            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("applications");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.FileName)
                    .HasMaxLength(500)
                    .HasColumnName("file_name");
                entity.Property(e => e.LastRunDate)
                    .HasDefaultValueSql("'0000-00-00 00:00:00'")
                    .HasColumnType("datetime")
                    .HasColumnName("last_run_date");
                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .HasColumnName("name");
                entity.Property(e => e.ProcessName)
                    .HasMaxLength(500)
                    .HasColumnName("process_name");
                entity.Property(e => e.Order)
                    .HasColumnType("int(11)")
                    .HasColumnName("order");
                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)")
                    .HasColumnName("status");
                entity.Property(e => e.CanStartInStarter)
                     .HasDefaultValueSql("'0'")
                     .HasColumnType("int(11)")
                     .HasColumnName("CanStartInStarter");
            });

            modelBuilder.Entity<PartsNamesArchive>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("parts_names_archive");

                entity.HasIndex(e => e.PartsAndReplacesId, "parts_and_replaces_id");
                entity.HasIndex(e => e.PartsSourcesId, "parts_sources_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.PartsAndReplacesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("parts_and_replaces_id");

                entity.Property(e => e.PartName)
                    .HasMaxLength(255)
                    .IsRequired()
                    .HasColumnName("partname");

                entity.Property(e => e.PartsSourcesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("parts_sources_id");

                entity.Property(e => e.AttemptCounter)
                    .HasColumnType("int(11)")
                    .HasDefaultValue(0)
                    .HasColumnName("attempt_counter");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<PartsPicArchive>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("parts_pic_archive");

                entity.HasIndex(e => e.PartsAndReplacesId, "parts_and_replaces_id");
                entity.HasIndex(e => e.PartsSourcesId, "parts_sources_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.PartsAndReplacesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("parts_and_replaces_id");

                entity.Property(e => e.LocalPath)
                    .HasMaxLength(255)
                    .IsRequired()
                    .HasColumnName("local_path");

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("link");

                entity.Property(e => e.PartsSourcesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("parts_sources_id");

                entity.Property(e => e.AttemptCounter)
                    .HasColumnType("int(11)")
                    .HasDefaultValue(0)
                    .HasColumnName("attempt_counter");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");


            });

            modelBuilder.Entity<PartSource>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("parts_sources");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.SourceName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_name");

                entity.Property(e => e.Confidence)
                   .HasColumnName("confidence");

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("link");
                entity.Property(e => e.DataType)
                                .IsRequired()
                                .HasColumnType("text")
                                .HasColumnName("data_type");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<ReplacesArchive>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("replaces_archive");

                entity.HasIndex(e => e.PartsAndReplacesId, "parts_and_replaces_id");
                entity.HasIndex(e => e.PartsSourcesId, "parts_sources_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.PartsAndReplacesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("parts_and_replaces_id");

                entity.Property(e => e.ReplaceNumber)
                    .IsRequired()
                    .HasMaxLength(255) // Установите максимальную длину в зависимости от ваших требований
                    .HasColumnName("replace_number");

                entity.Property(e => e.PartsSourcesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("parts_sources_id");

                entity.Property(e => e.AttemptCounter)
                    .HasColumnType("int(11)")
                    .HasDefaultValue(0)
                    .HasColumnName("attempt_counter");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<ApplicationLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("application_logs");

                entity.HasIndex(e => e.ApplicationId, "application_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.ApplicationId)
                    .HasColumnType("int(11)")
                    .HasColumnName("application_id");
                entity.Property(e => e.Description)
                    .HasMaxLength(5000)
                    .HasDefaultValueSql("''")
                    .HasColumnName("description");


                entity.HasOne(d => d.Application).WithMany(p => p.ApplicationLogs)
                    .HasForeignKey(d => d.ApplicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_application_logs_applications");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity
                    .ToTable("jobs")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasIndex(e => new { e.UserId, e.JobNumber, e.JobLink }, "sost_unik").IsUnique();

                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.JobLink)
                    .HasMaxLength(57)
                    .HasColumnName("job_link");
                entity.Property(e => e.JobNumber)
                    .HasMaxLength(555)
                    .HasColumnName("job_number");
                entity.Property(e => e.Token)
                    .HasMaxLength(555)
                    .HasColumnName("token");
                entity.Property(e => e.UpdateRequestStatus)
                    .HasMaxLength(555)
                    .HasColumnName("update_request_status");
                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_id");

                entity.HasOne(d => d.User).WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("jobs_ibfk_1");
            });

            modelBuilder.Entity<JobDescriptionAndNote>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity
                    .ToTable("job_description_and_notes")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasIndex(e => e.JobId, "job_id").IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.JobDescription)
                    .HasColumnType("text")
                    .HasColumnName("job_description");
                entity.Property(e => e.JobId)
                    .HasColumnType("int(11)")
                    .HasColumnName("job_id");
                entity.Property(e => e.JobNotes)
                    .HasColumnType("text")
                    .HasColumnName("job_notes");
                entity.Property(e => e.PicCounter)
                    .HasColumnType("int(11)")
                    .HasColumnName("pic_counter");
                entity.Property(e => e.Status)
                    .HasColumnType("enum('1','0')")
                    .HasColumnName("status");

                entity.HasOne(d => d.Job).WithOne(p => p.JobDescriptionAndNote)
                    .HasForeignKey<JobDescriptionAndNote>(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("job_desc");
            });

            modelBuilder.Entity<JobPic>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("job_pics_test_100");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.FolderName)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''")
                    .HasColumnName("folder_name");
                entity.Property(e => e.FileName)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''")
                    .HasColumnName("file_name");
                entity.Property(e => e.OcrText)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("''")
                    .HasColumnName("ocr_text");
                entity.Property(e => e.IsRating)
                    .HasColumnType("int(11)")
                    .HasColumnName("is_rating_plate_percentage");
                entity.Property(e => e.CompletionCosts)
                            .HasColumnType("decimal(18,6)")
                            .HasColumnName("completion_cost");
                entity.Property(e => e.ImageToken)
                   .HasColumnType("int(11)")
                   .HasColumnName("image_tokens");
                entity.Property(e => e.Brand)
                  .HasMaxLength(255)
                  .HasDefaultValueSql("''")
                  .HasColumnName("brand");
                entity.Property(e => e.Model)
               .HasMaxLength(255)
               .HasDefaultValueSql("''")
               .HasColumnName("model");
                entity.Property(e => e.SerialNumber)
              .HasMaxLength(255)
              .HasDefaultValueSql("''")
              .HasColumnName("serial_number");

            });

            modelBuilder.Entity<Proxy>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("proxy_table");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.Type)
                    .HasDefaultValueSql("''")
                    .HasColumnName("Type");
                entity.Property(e => e.IP)
                    .HasDefaultValueSql("''")
                    .HasColumnName("IP");
                entity.Property(e => e.Port)
                    .HasColumnType("int(11)")
                    .HasColumnName("Port");
                entity.Property(e => e.Login)
                   .HasDefaultValueSql("''")
                   .HasColumnName("Login");
                entity.Property(e => e.Password)
                            .HasColumnName("Password");
                entity.Property(e => e.IsActive)
                   .HasColumnType("tinyint(1)")
                   .HasColumnName("IsActive");
            });

            modelBuilder.Entity<JobDoc>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity
                    .ToTable("job_docs")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasIndex(e => e.JobId, "job_id");

                entity.HasIndex(e => e.ModelsId, "models_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                //entity.Property(e => e.CategoryName)
                //    .HasMaxLength(100)
                //    .HasColumnName("category_name");
                //entity.Property(e => e.Confidence)
                //    .HasColumnType("enum('1','2','3','4','5')")
                //    .HasColumnName("confidence");

                entity.Property(e => e.JobId)
                    .HasColumnType("int(11)")
                    .HasColumnName("job_id");
                entity.Property(e => e.ModelsId)
                    .HasColumnType("int(11)")
                    .HasColumnName("models_id");


                entity.HasOne(d => d.Job).WithMany(p => p.JobDocs)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("job_docs_ibfk_1");

                entity.HasOne(d => d.Models).WithMany(p => p.JobDocs)
                    .HasForeignKey(d => d.ModelsId)
                    .HasConstraintName("job_docs_ibfk_2");
            });

            modelBuilder.Entity<JobDocsModelInfo>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("job_docs_model_info");

                entity.Property(e => e.Brand)
                    .HasMaxLength(100)
                    .HasColumnName("brand")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.CategoriesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("categories_id");
                entity.Property(e => e.DateModel).HasColumnName("date_model");
                entity.Property(e => e.FileName)
                    .HasMaxLength(100)
                    .HasColumnName("file_name")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.FileTitle)
                    .HasMaxLength(100)
                    .HasColumnName("file_title")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.Jdbrand)
                    .HasMaxLength(100)
                    .HasColumnName("jdbrand")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.JdcategoriesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("jdcategories_id");
                entity.Property(e => e.JddateModel).HasColumnName("jddate_model");
                entity.Property(e => e.JdfileName)
                    .HasMaxLength(100)
                    .HasColumnName("jdfile_name")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.JdfileTitle)
                    .HasMaxLength(100)
                    .HasColumnName("jdfile_title")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.JdlocalPath)
                    .HasMaxLength(200)
                    .HasColumnName("jdlocal_path")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.Jdmodel)
                    .HasMaxLength(40)
                    .HasColumnName("jdmodel")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.JdmodelConfidence)
                    .HasMaxLength(1)
                    .HasColumnName("jdmodel_confidence")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.JdmodelVersion)
                    .HasMaxLength(100)
                    .HasColumnName("jdmodel_version")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.JdsourceId)
                    .HasColumnType("int(11)")
                    .HasColumnName("jdsource_id");
                entity.Property(e => e.JdwebLink)
                    .HasMaxLength(200)
                    .HasColumnName("jdweb_link")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.JobDescription)
                    .HasColumnType("mediumtext")
                    .HasColumnName("job_description")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.JobDocModelId)
                    .HasColumnType("int(11)")
                    .HasColumnName("job_doc_model_id");
                entity.Property(e => e.JobId)
                    .HasColumnType("int(11)")
                    .HasColumnName("job_id");
                entity.Property(e => e.JobNotes)
                    .HasColumnType("mediumtext")
                    .HasColumnName("job_notes")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.LocalPath)
                    .HasMaxLength(200)
                    .HasColumnName("local_path")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.Model)
                    .HasMaxLength(40)
                    .HasColumnName("model")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.ModelConfidence)
                    .HasMaxLength(1)
                    .HasColumnName("model_confidence")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.ModelId)
                    .HasColumnType("int(11)")
                    .HasColumnName("model_id");
                entity.Property(e => e.ModelNumber)
                    .HasMaxLength(100)
                    .HasColumnName("model_number");
                entity.Property(e => e.ModelVersion)
                    .HasMaxLength(100)
                    .HasColumnName("model_version")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.ModelsNumberId)
                    .HasColumnType("int(11)")
                    .HasColumnName("models_number_id");
                entity.Property(e => e.NotFountModel)
                    .HasMaxLength(100)
                    .HasColumnName("not_fount_model");
                entity.Property(e => e.ResponseText)
                    .HasColumnType("mediumtext")
                    .HasColumnName("response_text");
                entity.Property(e => e.SourceId)
                    .HasColumnType("int(11)")
                    .HasColumnName("source_id");
                entity.Property(e => e.Token)
                    .HasColumnType("text")
                    .HasColumnName("token")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.WebLink)
                    .HasMaxLength(200)
                    .HasColumnName("web_link")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
            });

            modelBuilder.Entity<JobStock>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity
                    .ToTable("job_stocks")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasIndex(e => e.JobId, "job_id");

                entity.HasIndex(e => e.UserStockId, "user_stock_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.JobId)
                    .HasColumnType("int(11)")
                    .HasColumnName("job_id");
                entity.Property(e => e.MatchPartNumber)
                    .HasMaxLength(30)
                    .HasColumnName("match_part_number");
                entity.Property(e => e.UserStockId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_stock_id");

                entity.HasOne(d => d.Job).WithMany(p => p.JobStocks)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("job_stocks_ibfk_1");

                entity.HasOne(d => d.UserStock).WithMany(p => p.JobStocks)
                    .HasForeignKey(d => d.UserStockId)
                    .HasConstraintName("job_stocks_ibfk_2");
            });

            modelBuilder.Entity<JobStocksView>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("job_stocks_view");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.JobDescription)
                    .HasColumnType("mediumtext")
                    .HasColumnName("job_description")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.JobId)
                    .HasColumnType("int(11)")
                    .HasColumnName("job_id");
                entity.Property(e => e.JobNotes)
                    .HasColumnType("mediumtext")
                    .HasColumnName("job_notes")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.MainPartNumber)
                    .HasMaxLength(20)
                    .HasColumnName("main_part_number")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.MatchPartNumber)
                    .HasMaxLength(30)
                    .HasColumnName("match_part_number")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.MnJobId)
                    .HasColumnType("int(11)")
                    .HasColumnName("mn_job_id");
                entity.Property(e => e.ModelNumber)
                    .HasMaxLength(100)
                    .HasColumnName("model_number");
                entity.Property(e => e.NotFountModel)
                    .HasMaxLength(100)
                    .HasColumnName("not_fount_model");
                entity.Property(e => e.PartId)
                    .HasColumnType("int(11)")
                    .HasColumnName("part_id");
                entity.Property(e => e.PartName)
                    .HasMaxLength(70)
                    .HasColumnName("part_name")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.PartNumber)
                    .HasMaxLength(50)
                    .HasColumnName("part_number")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.PartsAndReplacesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("parts_and_replaces_id");
                entity.Property(e => e.Replaces)
                    .HasColumnName("replaces")
                    .UseCollation("utf8mb4_bin")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.ResponseText)
                    .HasColumnType("mediumtext")
                    .HasColumnName("response_text");
                entity.Property(e => e.StockName)
                    .HasMaxLength(30)
                    .HasColumnName("stock_name")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.Token)
                    .HasColumnType("text")
                    .HasColumnName("token")
                    .UseCollation("utf8mb4_unicode_ci")
                    .HasCharSet("utf8mb4");
                entity.Property(e => e.UserStockId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_stock_id");
            });

            modelBuilder.Entity<Mask>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.mask)
                    .HasColumnType("text")
                    .HasColumnName("mask");
                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_id");


            });

            modelBuilder.Entity<Model>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity
                    .ToTable("models")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasIndex(e => e.CategoriesId, "categories_id");

                entity.HasIndex(e => e.model, "model");

                entity.HasIndex(e => e.SourceId, "source_id");

                entity.HasIndex(e => e.Token, "token").IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.Brand)
                    .HasMaxLength(100)
                    .HasColumnName("brand");
                entity.Property(e => e.CategoriesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("categories_id");
                entity.Property(e => e.CleanedModel)
                    .HasMaxLength(40)
                    .HasColumnName("cleaned_model");
                entity.Property(e => e.Confidence)
                    .HasMaxLength(1)
                    .HasColumnName("confidence");
                entity.Property(e => e.DateModel).HasColumnName("date_model");
                entity.Property(e => e.FileName)
                    .HasMaxLength(100)
                    .HasColumnName("file_name");
                entity.Property(e => e.FileTitle)
                    .HasMaxLength(100)
                    .HasColumnName("file_title");
                entity.Property(e => e.LocalPath)
                    .HasMaxLength(200)
                    .HasColumnName("local_path");
                entity.Property(e => e.model)
                    .HasMaxLength(40)
                    .HasColumnName("model");
                //entity.Property(e => e.OldWeblink)
                //    .HasMaxLength(200)
                //    .HasColumnName("old_weblink");
                entity.Property(e => e.SourceId)
                    .HasColumnType("int(11)")
                    .HasColumnName("source_id");
                entity.Property(e => e.Token)
                    .HasMaxLength(6)
                    .HasColumnName("token");
                entity.Property(e => e.Version)
                    .HasMaxLength(100)
                    .HasColumnName("version");
                entity.Property(e => e.WebLink)
                    .HasMaxLength(200)
                    .HasColumnName("web_link");
            });

            modelBuilder.Entity<ModelNumber>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("model_numbers");

                entity.HasIndex(e => e.JobId, "job_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.Confirmed)
                    .HasColumnType("enum('-1','0','1','2','3','4')")
                    .HasColumnName("confirmed");
                entity.Property(e => e.GotForWorkAt)
                    .HasColumnType("datetime")
                    .HasColumnName("got_for_work_at");
                entity.Property(e => e.JobId)
                    .HasColumnType("int(11)")
                    .HasColumnName("job_id");
                entity.Property(e => e.ModelNumber1)
                    .HasMaxLength(100)
                    .HasColumnName("model_number");
                entity.Property(e => e.DocCounter)
                    .HasColumnType("int(11)")
                    .HasColumnName("doc_counter");
                entity.HasOne(d => d.Job).WithMany(p => p.ModelNumbers)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("model_numbers_ibfk_1");
            });

            modelBuilder.Entity<Sources>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("sources");

                //entity.HasIndex(e => e.JobId, "job_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.SourceName)
                    .HasMaxLength(100)
                    .HasColumnName("source_name");
                entity.Property(e => e.Confidence)
                    .HasColumnType("enum('1','2','3','4', '5')")
                    .HasColumnName("confidence");
                entity.Property(e => e.DataTypes)
                    .HasColumnType("json")
                    .HasColumnName("data_types");
                entity.Property(e => e.FolderPath)
                    .HasMaxLength(500)
                    .HasColumnName("folder_path");
            });

            modelBuilder.Entity<ModelsNumbersNotFound>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.ToTable("models_numbers_not_found");

                entity.HasIndex(e => e.JobId, "job_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.JobId)
                    .HasColumnType("int(11)")
                    .HasColumnName("job_id");
                entity.Property(e => e.ModelNumber)
                    .HasMaxLength(100)
                    .HasColumnName("model_number");

                entity.HasOne(d => d.Job).WithMany(p => p.ModelsNumbersNotFounds)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("models_numbers_not_found_ibfk_1");
            });

            modelBuilder.Entity<PartsAndReplace>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity
                    .ToTable("parts_and_replaces")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.GotForWorkAt)
                    .HasColumnType("datetime")
                    .HasColumnName("got_for_work_at");
                entity.Property(e => e.MainPartNumber)
                    .HasMaxLength(20)
                    .HasColumnName("main_part_number");
                entity.Property(e => e.PartName)
                    .HasMaxLength(70)
                    .HasColumnName("part_name");
                entity.Property(e => e.PicBase64).
                    HasColumnName("pic_base64");
                entity.Property(e => e.Replaces)
                    .HasColumnType("json")
                    .HasColumnName("replaces");
                entity.Property(e => e.Status)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("enum('0','1','2','3')")
                    .HasColumnName("status");
                entity.Property(e => e.Brand).
                    HasColumnName("brand");
                entity.Property(e => e.Pic).
                   HasColumnName("pic");
                entity.Property(e => e.PicLink).
                  HasColumnName("pic_link");
            });

            modelBuilder.Entity<JobDocInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity
                    .ToTable("job_docs_info")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.SourceId)
                     .HasColumnType("int(11)")
                    .HasColumnName("source_id");
                entity.Property(e => e.JobId)
                      .HasColumnType("int(11)")
                     .HasColumnName("job_id");
                entity.Property(e => e.ModelsNumberId)
                      .HasColumnType("int(11)")
                     .HasColumnName("models_number_id");
                entity.Property(e => e.ModelId)
                      .HasColumnType("int(11)")
                     .HasColumnName("model_id");
                entity.Property(e => e.LocalPath)
                    .HasMaxLength(200)
                    .HasColumnName("local_path");

            });

            modelBuilder.Entity<Parts>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity
                    .ToTable("parts")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.PartName)
                    .HasMaxLength(30)
                    .HasColumnName("part_name");
                entity.Property(e => e.PartNumber)
                    .HasMaxLength(30)
                    .HasColumnName("part_number");
                entity.Property(e => e.IdModel)
                    .HasColumnType("int(11)")
                    .HasColumnName("id_model");

            });

            modelBuilder.Entity<StopWords>(entity =>
            {

                entity
                    .ToTable("Stop_words")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");
                entity.Property(e => e.Id)
                   .HasColumnType("int(11)")
                   .HasColumnName("id");
                entity.Property(e => e.Word)
                    .HasMaxLength(30)
                    .HasColumnName("word");
                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_id");

            });

            modelBuilder.Entity<ImageText>(entity =>
            {

                entity
                    .ToTable("image_text")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");
                entity.Property(e => e.Id)
                   .HasColumnType("int(11)")
                   .HasColumnName("id");
                entity.Property(e => e.text)
                    .HasMaxLength(4000)
                    .HasColumnName("text");
                entity.Property(e => e.file)
                    .HasMaxLength(4000)
                    .HasColumnName("file");
                entity.Property(e => e.folder)
                   .HasMaxLength(4000)
                   .HasColumnName("folder");

            });

            modelBuilder.Entity<Response>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity.HasIndex(e => e.CrmId, "fk_responses_crm");

                entity.HasIndex(e => e.JobId, "job_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.AttemptCount)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)")
                    .HasColumnName("attempt_count");
                entity.Property(e => e.CrmId)
                    .HasColumnType("int(11)")
                    .HasColumnName("CRM_id");
                entity.Property(e => e.JobId)
                    .HasColumnType("int(11)")
                    .HasColumnName("job_id");
                entity.Property(e => e.JobLink)
                    .HasMaxLength(255)
                    .HasColumnName("job_link");
                entity.Property(e => e.ResponseText)
                    .HasColumnType("text")
                    .HasColumnName("response_text");
                entity.Property(e => e.Status)
                    .HasColumnType("enum('0','1','2','5')")
                    .HasColumnName("status");
                entity.Property(e => e.TgStatus)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("enum('0','5','1','2')")
                    .HasColumnName("TG-status");
                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_id");
                entity.Property(e => e.WhatsappStatus)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("enum('0','5','1','2')")
                    .HasColumnName("Whatsapp-status");

                entity.HasOne(d => d.Job).WithMany(p => p.Responses)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Responses_ibfk_1");
            });

            modelBuilder.Entity<StockCred>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity
                    .ToTable("stock_creds")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasIndex(e => e.ContactTypesId, "contact_types_id");

                entity.HasIndex(e => e.DocTypesId, "doc_types_id");

                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.ContactAddress)
                    .HasMaxLength(30)
                    .HasColumnName("contact_address");
                entity.Property(e => e.ContactTypesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("contact_types_id");
                entity.Property(e => e.DocTypesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("doc_types_id");
                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .HasDefaultValueSql("'partsbot@gmail.com'")
                    .HasColumnName("email");
                entity.Property(e => e.ExcelColumn)
                    .HasMaxLength(2)
                    .HasColumnName("excel_column");
                entity.Property(e => e.FileName)
                    .HasMaxLength(300)
                    .HasColumnName("file_name");
                entity.Property(e => e.Partmanager)
                    .HasMaxLength(40)
                    .HasColumnName("partmanager");
                entity.Property(e => e.RecordsCount)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)")
                    .HasColumnName("records_count");
                entity.Property(e => e.ReplaceRecords)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("int(11)")
                    .HasColumnName("replace_records");
                entity.Property(e => e.StockLink)
                    .HasMaxLength(300)
                    .HasColumnName("stock_link");
                entity.Property(e => e.StockName)
                    .HasMaxLength(30)
                    .HasColumnName("stock_name");
                entity.Property(e => e.SyncFreq)
                    .HasMaxLength(5)
                    .HasDefaultValueSql("'60'")
                    .HasColumnName("sync_freq");
                entity.Property(e => e.SyncSwitch)
                    .HasDefaultValueSql("'ON'")
                    .HasColumnType("enum('ON','OFF')")
                    .HasColumnName("sync_switch");
                entity.Property(e => e.UpdateStatus)
                    .HasColumnType("enum('Linked','Unlinked')")
                    .HasColumnName("update_status");
                entity.Property(e => e.UpdateTime)
                    .HasColumnType("timestamp")
                    .HasColumnName("update_time");
                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_id");

                entity.HasOne(d => d.User).WithMany(p => p.StockCreds)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("stock_creds_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity
                    .ToTable("users")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasIndex(e => e.CrmId, "CRM_id");

                entity.HasIndex(e => e.TelegramId, "telegram_id").IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.AllHistory)
                    .HasMaxLength(255)
                    .HasColumnName("all_history");
                entity.Property(e => e.ChatId)
                    .HasColumnType("int(11)")
                    .HasColumnName("chat_id");
                entity.Property(e => e.Code)
                    .HasDefaultValueSql("'123'")
                    .HasColumnType("int(11)")
                    .HasColumnName("code");
                entity.Property(e => e.CrmId)
                    .HasColumnType("int(11)")
                    .HasColumnName("CRM_id");
                entity.Property(e => e.CrmLogin)
                    .HasMaxLength(30)
                    .HasColumnName("CRM_login");
                entity.Property(e => e.CrmPassword)
                    .HasMaxLength(30)
                    .HasColumnName("CRM_password");
                entity.Property(e => e.Login)
                    .HasMaxLength(30)
                    .HasColumnName("login");
                entity.Property(e => e.Password)
                    .HasMaxLength(30)
                    .HasColumnName("password");
                entity.Property(e => e.PrefersCrm)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("enum('1','0')")
                    .HasColumnName("prefers_crm");
                entity.Property(e => e.PrefersTelegram)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("enum('1','0')")
                    .HasColumnName("prefers_telegram");
                entity.Property(e => e.PrefersWhatsapp)
                    .HasDefaultValueSql("'0'")
                    .HasColumnType("enum('1','0')")
                    .HasColumnName("prefers_whatsapp");
                entity.Property(e => e.Proxy)
                    .HasMaxLength(60)
                    .HasColumnName("proxy");
                entity.Property(e => e.SyncFreq)
                    .HasMaxLength(5)
                    .HasDefaultValueSql("'10'")
                    .HasColumnName("sync_freq");
                entity.Property(e => e.SyncSwitch)
                    .HasDefaultValueSql("'ON'")
                    .HasColumnType("enum('ON','OFF')")
                    .HasColumnName("sync_switch");
                entity.Property(e => e.TelegramId)
                    .HasColumnType("int(11)")
                    .HasColumnName("telegram_id");
                entity.Property(e => e.TelegramState)
                    .HasColumnType("enum('Waiting','SendingStocks','SendingJob')")
                    .HasColumnName("telegram_state");
                entity.Property(e => e.UpdateStatus)
                    .HasColumnType("enum('Linked','Unlinked')")
                    .HasColumnName("update_status");
                entity.Property(e => e.UpdateTime)
                    .HasColumnType("bigint(20)")
                    .HasColumnName("update_time");
                entity.Property(e => e.DiagramProbability)
                    .HasColumnType("int(11)")
                    .HasColumnName("diagram_probability");
                entity.Property(e => e.ManualProbability)
                    .HasColumnType("int(11)")
                    .HasColumnName("manual_probability");
            });

            modelBuilder.Entity<UserStock>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");

                entity
                    .ToTable("user_stocks")
                    .HasCharSet("utf8mb4")
                    .UseCollation("utf8mb4_unicode_ci");

                entity.HasIndex(e => e.PartsAndReplacesId, "parts_and_replaces_id");

                entity.HasIndex(e => e.StockId, "stock_id");

                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
                entity.Property(e => e.PartNumber)
                    .HasMaxLength(50)
                    .HasColumnName("part_number");
                entity.Property(e => e.PartsAndReplacesId)
                    .HasColumnType("int(11)")
                    .HasColumnName("parts_and_replaces_id");
                entity.Property(e => e.StockId)
                    .HasColumnType("int(11)")
                    .HasColumnName("stock_id");
                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_id");

                entity.HasOne(d => d.PartsAndReplaces).WithMany(p => p.UserStocks)
                    .HasForeignKey(d => d.PartsAndReplacesId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("user_stocks_ibfk_3");

                entity.HasOne(d => d.Stock).WithMany(p => p.UserStocks)
                    .HasForeignKey(d => d.StockId)
                    .HasConstraintName("user_stocks_ibfk_2");

                entity.HasOne(d => d.User).WithMany(p => p.UserStocks)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_stocks_ibfk_1");
            });


        }

        public void OnModelMsSqlCreating(ModelBuilder modelBuilder)
        {

            modelBuilder
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");

            modelBuilder.Entity<Application>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_applications_id");

                entity.ToTable("applications");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.FileName)
                    .HasMaxLength(500)
                    .HasColumnName("file_name")
                    .IsRequired(false); // Указывает, что поле может быть NULL

                entity.Property(e => e.LastRunDate)
                    .HasDefaultValueSql("NULL") // Устанавливает значение по умолчанию как NULL
                    .HasColumnType("datetime") // Используем datetime2 для MSSQL
                    .HasColumnName("last_run_date")
                    .IsRequired(false); // Указывает, что поле может быть NULL

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .HasColumnName("name")
                    .IsRequired(false); // Указывает, что поле может быть NULL

                entity.Property(e => e.ProcessName)
                    .HasMaxLength(500)
                    .HasColumnName("process_name")
                    .IsRequired(false); // Указывает, что поле может быть NULL

                entity.Property(e => e.Order)
                    .HasColumnType("int")
                    .HasColumnName("order")
                    .IsRequired(true); // Указывает, что поле может быть NULL

                entity.Property(e => e.Status)
                    .HasDefaultValue(0) // Устанавливает значение по умолчанию как 0
                    .HasColumnType("int")
                    .HasColumnName("status")
                    .IsRequired(false); // Указывает, что поле может быть NULL

                entity.Property(e => e.Path)
                      .HasColumnName("path")
                      .IsRequired(false); // Указывает, что поле может быть NULL

                entity.Property(e => e.Caption)
                   .HasColumnName("caption")
                   .IsRequired(false); // Указывает, что поле может быть NULL

                entity.Property(e => e.CanStartInStarter)
                  .HasColumnType("int")
                  .HasColumnName("CanStartInStarter")
                  .IsRequired(false); // Указывает, что поле может быть NULL
            });

            modelBuilder.Entity<ApplicationLog>(entity =>
            {
                entity.ToTable("ApplicationLog");

                entity.Property(e => e.Id)
                    .HasColumnName("Id");

                entity.Property(e => e.ApplicationId)
                    .HasColumnName("ApplicationId");

                entity.Property(e => e.RecDate)
                    .HasColumnType("datetime")
                    .HasColumnName("RecDate");

                entity.Property(e => e.RecType)
                    .HasColumnName("RecType");

                entity.Property(e => e.Text)
                    .HasColumnName("Text");

                entity.Property(e => e.Description)
                    .HasColumnName("Description");


            });


            modelBuilder.Entity<PartsNamesArchive>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_parts_names_archive");

                entity.ToTable("parts_names_archive");

                entity.HasIndex(e => e.PartsAndReplacesId, "parts_and_replaces_id");
                entity.HasIndex(e => e.PartsSourcesId, "parts_sources_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.PartsAndReplacesId)
                    .HasColumnType("int")
                    .HasColumnName("parts_and_replaces_id")
                    .IsRequired();

                entity.Property(e => e.PartName)
                    .HasMaxLength(255)
                    .IsRequired()
                    .HasColumnName("partname");

                entity.Property(e => e.PartsSourcesId)
                    .HasColumnType("int")
                    .HasColumnName("parts_sources_id")
                    .IsRequired();

                entity.Property(e => e.AttemptCounter)
                    .HasColumnType("int")
                    .HasDefaultValue(0)
                    .HasColumnName("attempt_counter");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()") // Используем GETDATE() для MSSQL
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()") // Устанавливаем значение по умолчанию как GETDATE()
                    .HasColumnName("updated_at");


            });

            modelBuilder.Entity<PartsPicArchive>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_parts_pic_archive");

                entity.ToTable("parts_pic_archive");

                entity.HasIndex(e => e.PartsAndReplacesId, "parts_and_replaces_id");
                entity.HasIndex(e => e.PartsSourcesId, "parts_sources_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd() // Указывает, что это поле будет автоинкрементироваться
                    .HasColumnName("id");

                entity.Property(e => e.PartsAndReplacesId)
                    .HasColumnType("int")
                    .HasColumnName("parts_and_replaces_id")
                    .IsRequired();

                entity.Property(e => e.LocalPath)
                    .HasMaxLength(255)
                    .IsRequired()
                    .HasColumnName("local_path");

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)") // Используем nvarchar(max) вместо text
                    .HasColumnName("link");

                entity.Property(e => e.PartsSourcesId)
                    .HasColumnType("int")
                    .HasColumnName("parts_sources_id")
                    .IsRequired();

                entity.Property(e => e.AttemptCounter)
                    .HasColumnType("int")
                    .HasDefaultValue(0)
                    .HasColumnName("attempt_counter");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()") // Используем GETDATE() для MSSQL
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()") // Устанавливаем значение по умолчанию как GETDATE()
                    .HasColumnName("updated_at");
            });

            modelBuilder.Entity<PartSource>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_parts_sources");

                entity.ToTable("parts_sources");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd() // Указывает, что это поле будет автоинкрементироваться
                    .HasColumnName("id");

                entity.Property(e => e.SourceName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("source_name");

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)") // Используем nvarchar(max) вместо text
                    .HasColumnName("link");

                entity.Property(e => e.DataType)
                    .IsRequired()
                    .HasMaxLength(50) // Указываем максимальную длину для data_type
                    .HasColumnName("data_type");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()") // Используем GETDATE() для MSSQL
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()") // Устанавливаем значение по умолчанию как GETDATE()
                    .HasColumnName("updated_at");

                entity.Property(e => e.Status)
                     .HasColumnName("status");


            });

            modelBuilder.Entity<ReplacesArchive>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_replaces_archive");

                entity.ToTable("replaces_archive");

                entity.HasIndex(e => e.PartsAndReplacesId, "parts_and_replaces_id");
                entity.HasIndex(e => e.PartsSourcesId, "parts_sources_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd() // Указывает, что это поле будет автоинкрементироваться
                    .HasColumnName("id");

                entity.Property(e => e.PartsAndReplacesId)
                    .HasColumnType("int")
                    .HasColumnName("parts_and_replaces_id")
                    .IsRequired();

                entity.Property(e => e.ReplaceNumber)
                    .IsRequired()
                    .HasMaxLength(255) // Установите максимальную длину в зависимости от ваших требований
                    .HasColumnName("replace_number");

                entity.Property(e => e.PartsSourcesId)
                    .HasColumnType("int")
                    .HasColumnName("parts_sources_id")
                    .IsRequired();

                entity.Property(e => e.AttemptCounter)
                    .HasColumnType("int")
                    .HasDefaultValue(0) // Устанавливаем значение по умолчанию как 0
                    .HasColumnName("attempt_counter");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()") // Используем GETDATE() для MSSQL
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()") // Устанавливаем значение по умолчанию как GETDATE()
                    .HasColumnName("updated_at");


            });

            //modelBuilder.Entity<ApplicationLog>(entity =>
            //{
            //    entity.HasKey(e => e.Id).HasName("PK_application_logs_id");

            //    entity.ToTable("application_logs");

            //    entity.HasIndex(e => e.ApplicationId, "application_id");

            //    entity.Property(e => e.Id)
            //        .HasColumnType("int")
            //        .ValueGeneratedOnAdd() // Указывает, что это поле будет автоинкрементироваться
            //        .HasColumnName("id");

            //    entity.Property(e => e.ApplicationId)
            //        .HasColumnType("int")
            //        .HasColumnName("application_id")
            //        .IsRequired(); // Указывает, что поле обязательно

            //    entity.Property(e => e.Description)
            //        .HasMaxLength(5000)
            //        .HasColumnName("description");

            //    entity.Property(e => e.Finish)
            //        .HasColumnType("datetime2") // Используем datetime2 для MSSQL
            //        .HasColumnName("finish")
            //        .IsRequired(); // Указывает, что поле обязательно

            //    entity.Property(e => e.Start)
            //        .HasColumnType("datetime2") // Используем datetime2 для MSSQL
            //        .HasColumnName("start")
            //        .IsRequired(); // Указывает, что поле обязательно

            //    entity.HasOne(d => d.Application) // Укажите соответствующую сущность
            //        .WithMany(p => p.ApplicationLogs) // Укажите, как будет выглядеть связь
            //        .HasForeignKey(d => d.ApplicationId)
            //        .OnDelete(DeleteBehavior.ClientSetNull) // Укажите поведение при удалении
            //        .HasConstraintName("FK_application_logs_applications");
            //});

            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_jobs_id");

                entity.ToTable("jobs");

                entity.HasIndex(e => new { e.UserId, e.JobNumber, e.JobLink }, "sost_unik").IsUnique();
                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd() // Указывает, что это поле будет автоинкрементироваться
                    .HasColumnName("id");

                entity.Property(e => e.JobLink)
                    .HasMaxLength(57)
                    .HasColumnName("job_link");

                entity.Property(e => e.JobNumber)
                    .HasMaxLength(555)
                    .HasColumnName("job_number");

                entity.Property(e => e.Token)
                    .HasMaxLength(555)
                    .HasColumnName("token");

                entity.Property(e => e.UpdateRequestStatus)
                    .HasMaxLength(555)
                    .HasColumnName("update_request_status");


                entity.Property(e => e.UserId)
                    .HasColumnType("int")
                    .HasColumnName("user_id")
                    .IsRequired(); // Указывает, что поле обязательно

                entity.HasOne(d => d.User) // Укажите соответствующую сущность
                    .WithMany(p => p.Jobs) // Укажите, как будет выглядеть связь
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull) // Укажите поведение при удалении
                    .HasConstraintName("jobs_ibfk_1");

                entity.HasOne(d => d.User).WithMany(p => p.Jobs)
                   .HasForeignKey(d => d.UserId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("jobs_ibfk_1");
            });

            modelBuilder.Entity<Job>()
            .ToTable(tb => tb.UseSqlOutputClause(false));

            modelBuilder.Entity<JobDescriptionAndNote>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_job_description_and_notes_id");

                entity.ToTable("job_description_and_notes");

                entity.HasIndex(e => e.JobId, "job_id").IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd() // Указывает, что это поле будет автоинкрементироваться
                    .HasColumnName("id");

                entity.Property(e => e.JobDescription)
                    .HasColumnType("nvarchar(max)") // Используем nvarchar(max) вместо text
                    .HasColumnName("job_description")
                    .IsRequired(false); // Указывает, что поле может быть NULL

                entity.Property(e => e.JobId)
                    .HasColumnType("int")
                    .HasColumnName("job_id"); // Указывает, что поле обязательно

                entity.Property(e => e.JobNotes)
                    .HasColumnType("nvarchar(max)") // Используем nvarchar(max) вместо text
                    .HasColumnName("job_notes")
                    .IsRequired(false); // Указывает, что поле может быть NULL

                entity.Property(e => e.PicCounter)
                    .HasColumnType("int")
                    .HasColumnName("pic_counter")
                    .IsRequired(false); // Указывает, что поле может быть NULL

                entity.Property(e => e.Status)
                    .HasColumnType("nvarchar(1)") // Используем nvarchar(1) вместо enum
                    .HasColumnName("status")
                    .IsRequired(false); // Указывает, что поле может быть NULL
                ///---------------------------
                entity.Property(e => e.Brand)
                   .HasColumnName("brand"); // Указывает, что поле может быть NULL
                entity.Property(e => e.Model)
                   .HasColumnName("model"); // Указывает, что поле может быть NULL
                entity.Property(e => e.SerialNumber)
                   .HasColumnName("serial_number"); // Указывает, что поле может быть NULL
                entity.Property(e => e.OcrText)
                   .HasColumnName("ocr_text"); // Указывает, что поле может быть NULL
                entity.Property(e => e.IsRatingPlatePercentage)
                   .HasColumnName("is_rating_plate_percentage"); // Указывает, что поле может быть NULL
                entity.Property(e => e.CompletionCost)
                   .HasColumnName("completion_cost"); // Указывает, что поле может быть NULL
                entity.Property(e => e.ImageTokens)
                   .HasColumnName("image_tokens"); // Указывает, что поле может быть NULL
                entity.Property(e => e.StickerLink)
                   .HasColumnName("sticker_link"); // Указывает, что поле может быть NULL
                entity.Property(e => e.OriginalBrand)
                .HasColumnName("original_brand"); // Указывает, что поле может быть NULL

                entity.HasOne(d => d.Job) // Укажите соответствующую сущность
                    .WithOne(p => p.JobDescriptionAndNote) // Укажите, как будет выглядеть связь
                    .HasForeignKey<JobDescriptionAndNote>(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull) // Укажите поведение при удалении
                    .HasConstraintName("job_desc");
            });

            modelBuilder.Entity<JobPic>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_job_pics_test_100");

                entity.ToTable("job_pics_test_100");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd() // Указывает, что это поле будет автоинкрементироваться
                    .HasColumnName("id");

                entity.Property(e => e.FolderName)
                    .HasMaxLength(255)
                    .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                    .HasColumnName("folder_name");

                entity.Property(e => e.FileName)
                    .HasMaxLength(255)
                    .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                    .HasColumnName("file_name");

                entity.Property(e => e.OcrText)
                    .HasMaxLength(255)
                    .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                    .HasColumnName("ocr_text");

                entity.Property(e => e.IsRating)
                    .HasColumnType("int")
                    .HasColumnName("is_rating_plate_percentage");

                entity.Property(e => e.CompletionCosts)
                    .HasColumnType("decimal(18,6)")
                    .HasColumnName("completion_cost");

                entity.Property(e => e.ImageToken)
                    .HasColumnType("int")
                    .HasColumnName("image_tokens")
                    .HasDefaultValue(0); // Устанавливаем значение по умолчанию как 0

                entity.Property(e => e.Brand)
                    .HasMaxLength(255)
                    .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                    .HasColumnName("brand");

                entity.Property(e => e.Model)
                    .HasMaxLength(255)
                    .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                    .HasColumnName("model");

                entity.Property(e => e.SerialNumber)
                    .HasMaxLength(255)
                    .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                    .HasColumnName("serial_number");
            });

            modelBuilder.Entity<Proxy>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_proxy_table_ID");

                entity.ToTable("proxy_table");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .ValueGeneratedOnAdd() // Указывает, что это поле будет автоинкрементироваться
                    .HasColumnName("ID");

                entity.Property(e => e.Type)
                    .HasColumnType("nvarchar(max)") // Используем nvarchar(max) для поддержки длинных строк
                    .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                    .HasColumnName("Type");

                entity.Property(e => e.IP)
                    .HasColumnType("nvarchar(max)") // Используем nvarchar(max) для поддержки длинных строк
                    .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                    .HasColumnName("IP");

                entity.Property(e => e.Port)
                    .HasColumnType("int") // Убираем (11), так как в MSSQL это не требуется
                    .HasColumnName("Port")
                    .HasDefaultValue(0); // Устанавливаем значение по умолчанию как 0

                entity.Property(e => e.Login)
                    .HasColumnType("nvarchar(max)") // Используем nvarchar(max) для поддержки длинных строк
                    .HasDefaultValue("") // Устанавливаем значение по умолчанию как пустую строку
                    .HasColumnName("Login");

                entity.Property(e => e.Password)
                    .HasColumnType("nvarchar(max)") // Используем nvarchar(max) для поддержки длинных строк
                    .HasColumnName("Password");

                entity.Property(e => e.IsActive)
                    .HasColumnType("TINYINT") // Используем smallint вместо tinyint
                    .HasColumnName("IsActive")
                    .HasDefaultValue(1); // Устанавливаем значение по умолчанию как 1
            });

            modelBuilder.Entity<JobDoc>(entity =>
            {
                entity.ToTable("job_docs");

                entity.HasKey(e => e.Id)
                    .HasName("PK_job_docs");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.JobId)
                    .HasColumnName("job_id")
                    .IsRequired();

                entity.Property(e => e.ModelsId)
                    .HasColumnName("models_id")
                    .IsRequired();

                entity.Property(e => e.CleanedModel)
                    .HasColumnName("cleaned_model")
                    ; // Учитывая, что это строка, ограничиваем до 1 символа

                //entity.Property(e => e.CategoryName)
                //    .HasColumnName("category_name")
                //    .HasMaxLength(100);

                entity.Property(e => e.DocumentType)
                    .HasColumnName("DocumentType")
                    .HasMaxLength(255);

                entity.Property(e => e.Site)
                   .HasColumnName("Site");

                entity.Property(e => e.Part_counter)
                   .HasColumnName("Part_counter")
                   ;

                entity.Property(e => e.Status)
                   .HasColumnName("Status")
                   ;

                entity.Property(e => e.siteState)
                  .HasColumnName("siteState")
                  ;
                entity.Property(e => e.docState)
                  .HasColumnName("docState")
                  ;
                entity.Property(e => e.partCountState)
                  .HasColumnName("partCountState")
                  ;
                entity.Property(e => e.Document_Id)
                               .HasColumnName("Document_Id")
                               ;

                entity.Property(e => e.CPСountState)
                              .HasColumnName("CPСountState")
                              ;
                entity.Property(e => e.clicked)
                              .HasColumnName("clicked")
                              ;
                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobDocs) // Предполагается, что в Job есть коллекция JobDocs
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_job_docs_jobs");

                // Установка ограничения CHECK для Confidence
                //entity.HasCheckConstraint("CK_job_docs_confidence",
                //    "[confidence] IN ('1', '2', '3', '4', '5')");
            });
            modelBuilder.Entity<JobDocsModelInfo>(entity =>
            {
                entity.HasNoKey()
                    .ToView("job_docs_model_info");

                entity.Property(e => e.Brand)
                    .HasMaxLength(100)
                    .HasColumnName("brand");

                entity.Property(e => e.CategoriesId)
                    .HasColumnType("int")
                    .HasColumnName("categories_id");

                entity.Property(e => e.DateModel)
                    .HasColumnName("date_model");

                entity.Property(e => e.FileName)
                    .HasMaxLength(100)
                    .HasColumnName("file_name");

                entity.Property(e => e.FileTitle)
                    .HasMaxLength(100)
                    .HasColumnName("file_title");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .HasColumnName("id");

                entity.Property(e => e.Jdbrand)
                    .HasMaxLength(100)
                    .HasColumnName("jdbrand");

                entity.Property(e => e.JdcategoriesId)
                    .HasColumnType("int")
                    .HasColumnName("jdcategories_id");

                entity.Property(e => e.JddateModel)
                    .HasColumnName("jddate_model");

                entity.Property(e => e.JdfileName)
                    .HasMaxLength(100)
                    .HasColumnName("jdfile_name");

                entity.Property(e => e.JdfileTitle)
                    .HasMaxLength(100)
                    .HasColumnName("jdfile_title");

                entity.Property(e => e.JdlocalPath)
                    .HasMaxLength(200)
                    .HasColumnName("jdlocal_path");

                entity.Property(e => e.Jdmodel)
                    .HasMaxLength(40)
                    .HasColumnName("jdmodel");

                entity.Property(e => e.JdmodelConfidence)
                    .HasMaxLength(1)
                    .HasColumnName("jdmodel_confidence");

                entity.Property(e => e.JdmodelVersion)
                    .HasMaxLength(100)
                    .HasColumnName("jdmodel_version");

                entity.Property(e => e.JdsourceId)
                    .HasColumnType("int")
                    .HasColumnName("jdsource_id");

                entity.Property(e => e.JdwebLink)
                    .HasMaxLength(200)
                    .HasColumnName("jdweb_link");

                entity.Property(e => e.JobDescription)
                    .HasColumnType("nvarchar(max)") // Используем nvarchar(max) вместо mediumtext
                    .HasColumnName("job_description");

                entity.Property(e => e.JobDocModelId)
                    .HasColumnType("int")
                    .HasColumnName("job_doc_model_id");

                entity.Property(e => e.JobId)
                    .HasColumnType("int")
                    .HasColumnName("job_id");

                entity.Property(e => e.JobNotes)
                    .HasColumnType("nvarchar(max)") // Используем nvarchar(max) вместо mediumtext
                    .HasColumnName("job_notes");

                entity.Property(e => e.LocalPath)
                    .HasMaxLength(200)
                    .HasColumnName("local_path");

                entity.Property(e => e.Model)
                    .HasMaxLength(40)
                    .HasColumnName("model");

                entity.Property(e => e.ModelConfidence)
                    .HasMaxLength(1)
                    .HasColumnName("model_confidence");

                entity.Property(e => e.ModelId)
                    .HasColumnType("int")
                    .HasColumnName("model_id");

                entity.Property(e => e.ModelNumber)
                    .HasMaxLength(100)
                    .HasColumnName("model_number");

                entity.Property(e => e.ModelVersion)
                    .HasMaxLength(100)
                    .HasColumnName("model_version");

                entity.Property(e => e.ModelsNumberId)
                    .HasColumnType("int")
                    .HasColumnName("models_number_id");

                entity.Property(e => e.NotFountModel)
                    .HasMaxLength(100).HasColumnName("not_fount_model");

                entity.Property(e => e.ResponseText)
                    .HasColumnType("nvarchar(max)") // Используем nvarchar(max) вместо mediumtext
                    .HasColumnName("response_text");

                entity.Property(e => e.SourceId)
                    .HasColumnType("int")
                    .HasColumnName("source_id");

                entity.Property(e => e.Token)
                    .HasColumnType("nvarchar(max)") // Используем nvarchar(max) вместо text
                    .HasColumnName("token");

                entity.Property(e => e.WebLink)
                    .HasMaxLength(200)
                    .HasColumnName("web_link");
            });

            modelBuilder.Entity<JobStock>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_job_stocks_id");

                entity
                    .ToTable("job_stocks");

                entity.HasIndex(e => e.JobId, "IX_JobId");
                entity.HasIndex(e => e.UserStockId, "IX_UserStockId");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.JobId)
                    .IsRequired()
                    .HasColumnName("job_id");

                entity.Property(e => e.MatchPartNumber)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("match_part_number");

                entity.Property(e => e.UserStockId)
                    .IsRequired()
                    .HasColumnName("user_stock_id");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobStocks)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_JobStocks_Job");

                entity.HasOne(d => d.UserStock)
                    .WithMany(p => p.JobStocks)
                    .HasForeignKey(d => d.UserStockId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_JobStocks_UserStock");
            });

            modelBuilder.Entity<JobStocksView>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToView("job_stocks_view");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .HasColumnName("id");

                entity.Property(e => e.JobDescription)
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("job_description");

                entity.Property(e => e.JobId)
                    .HasColumnType("int")
                    .HasColumnName("job_id");

                entity.Property(e => e.JobNotes)
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("job_notes");

                entity.Property(e => e.MainPartNumber)
                    .HasMaxLength(20)
                    .HasColumnName("main_part_number");

                entity.Property(e => e.MatchPartNumber)
                    .HasMaxLength(30)
                    .HasColumnName("match_part_number");

                entity.Property(e => e.MnJobId)
                    .HasColumnType("int")
                    .HasColumnName("mn_job_id");

                entity.Property(e => e.ModelNumber)
                    .HasMaxLength(100)
                    .HasColumnName("model_number");

                entity.Property(e => e.NotFountModel)
                    .HasMaxLength(100)
                    .HasColumnName("not_fount_model");

                entity.Property(e => e.PartId)
                    .HasColumnType("int")
                    .HasColumnName("part_id");

                entity.Property(e => e.PartName)
                    .HasMaxLength(70)
                    .HasColumnName("part_name");

                entity.Property(e => e.PartNumber)
                    .HasMaxLength(50)
                    .HasColumnName("part_number");

                entity.Property(e => e.PartsAndReplacesId)
                    .HasColumnType("int")
                    .HasColumnName("parts_and_replaces_id");

                entity.Property(e => e.Replaces)
                    .HasColumnName("replaces");

                entity.Property(e => e.ResponseText)
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("response_text");

                entity.Property(e => e.StockName)
                    .HasMaxLength(30)
                    .HasColumnName("stock_name");

                entity.Property(e => e.Token)
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("token");

                entity.Property(e => e.UserStockId)
                    .HasColumnType("int")
                    .HasColumnName("user_stock_id");
            });

            modelBuilder.Entity<Mask>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Masks_id");

                entity.HasIndex(e => e.UserId, "IX_UserId");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.mask) // Обратите внимание на правильное имя свойства, если оно отличается от "mask"
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("mask");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<NModel>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Model");

                entity.ToTable("Model");


                entity.Property(e => e.Id)
                     .ValueGeneratedOnAdd()
                     .HasColumnName("Id");

                entity.Property(e => e.model)
                    .HasMaxLength(150) // Изменено на 150 в соответствии со структурой таблицы
                    .HasColumnName("Title")
                    .HasDefaultValue(null);

                entity.Property(e => e.WebLink)
                    .HasMaxLength(255) // Изменено на 255 в соответствии со структурой таблицы
                    .HasColumnName("Link")
                    .HasDefaultValue(null);

                entity.Property(e => e.CleanedModel)
                    .HasMaxLength(255) // Изменено на 255 в соответствии со структурой таблицы
                    .HasColumnName("cleaned_model");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("SiteId");

                entity.Property(e => e.Token)
                    .HasMaxLength(6)
                    .HasColumnName("token")
                    .HasDefaultValue(null);

            });

            modelBuilder.Entity<BrandModel>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_BrandModel");

                entity.ToTable("BrandModel");

                entity.HasIndex(e => e.BrandId, "IX_BrandId");
                entity.HasIndex(e => e.SiteId, "IX_SiteId");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("Id");

                entity.Property(e => e.Brand)
                    .HasMaxLength(150) // Длина поля соответствует структуре таблицы
                    .HasColumnName("Code")
                    .HasDefaultValue(null);

                entity.Property(e => e.Cnt)
                    .HasColumnName("Cnt")
                    .HasDefaultValue(0); // Установлено значение по умолчанию

                entity.Property(e => e.BrandId)
                    .HasColumnName("BrandId")
                    .HasDefaultValue(null); // Установлено значение по умолчанию

                entity.Property(e => e.SiteId)
                    .HasColumnName("SiteId")
                    .HasDefaultValue(null); // Установлено значение по умолчанию

                // Настройка внешнего ключа (если необходимо)
                entity.HasOne<Site>() // Предполагается, что есть класс Site
                    .WithMany() // Укажите, как связаны модели
                    .HasForeignKey(e => e.SiteId)
                    .OnDelete(DeleteBehavior.Restrict); // Укажите поведение при удалении
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Brand");

                entity.ToTable("Brand");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("Id");

                entity.Property(e => e.Title)
                    .HasMaxLength(50) // Длина поля соответствует структуре таблицы
                    .HasColumnName("Title")
                    .HasDefaultValue(null); // Установлено значение по умолчанию
            });

            modelBuilder.Entity<Site>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Site");

                entity.ToTable("Site");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("Id");

                entity.Property(e => e.Title)
                    .HasMaxLength(50) // Длина поля соответствует структуре таблицы
                    .HasColumnName("Title")
                    .HasDefaultValue(null); // Установлено значение по умолчанию

                entity.Property(e => e.confidence)
                    .HasColumnName("Confidence")
                    .HasDefaultValue(null); // Установлено значение по умолчанию

                entity.Property(e => e.DataTypes)
                  .HasColumnType("nvarchar(max)")
                  .HasColumnName("data_types")
                  .HasDefaultValue(null);

                entity.Property(e => e.FolderPath)
                    .HasMaxLength(500)
                    .HasColumnName("folder_path")
                    .HasDefaultValue(null);

                entity.Property(e => e.Link_template)
                   .HasColumnName("Link_template");

            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Document");

                entity.ToTable("Document");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("Id");

                entity.Property(e => e.Title)
                    .HasMaxLength(100) // Максимальная длина в соответствии с структурой таблицы
                    .HasColumnName("Title")
                    .HasDefaultValue(null); // Установлено значение по умолчанию

                entity.Property(e => e.Link)
                    .HasMaxLength(255) // Максимальная длина в соответствии с структурой таблицы
                    .HasColumnName("Link")
                    .HasDefaultValue(null); // Установлено значение по умолчанию

                //entity.Property(e => e.ModelId)
                //    .HasColumnName("ModelId")
                //    .HasDefaultValue(null); // Установлено значение по умолчанию

                entity.Property(e => e.DocumentTypeId)
                    .HasColumnName("DocumentTypeId")
                    .HasDefaultValue(null); // Установлено значение по умолчанию

                entity.Property(e => e.SiteId)
                    .HasColumnName("SiteId")
                    .HasDefaultValue(null); // Установлено значение по умолчанию

                entity.Property(e => e.DateModel)
                    .HasColumnType("datetime") // Установите тип столбца в соответствии с базой данных
                    .HasColumnName("date_model")
                    .HasDefaultValue(null); // Установлено значение по умолчанию

                entity.Property(e => e.Version)
                    .HasMaxLength(100) // Максимальная длина в соответствии с структурой таблицы
                    .HasColumnName("version")
                    .HasDefaultValue(null); // Установлено значение по умолчанию
                                            // Настройка внешних ключей
                entity.Property(e => e.LocalPath)
                                .HasColumnName("LocalPath"); // Установлено значение по умолчанию
                                                             // Настройка внешних ключей
                entity.Property(e => e.ModifiedAt)
                                .HasColumnName("ModifiedAt"); // Установлено значение по умолчанию
                                                              // Настройка внешних ключей
                entity.Property(e => e.Words)
                                .HasColumnName("Words"); // Установлено значение по умолчанию
                                                         // Настройка внешних ключей

                entity.Property(e => e.token)
                                .HasColumnName("token"); // Установлено значение по умолчанию

                entity.HasOne(d => d.DocumentType)
                    .WithMany() // Укажите, как связаны модели
                    .HasForeignKey(d => d.DocumentTypeId) // Укажите внешний ключ
                    .OnDelete(DeleteBehavior.Restrict); // Укажите поведение при удалении


            });

            modelBuilder.Entity<Document>()
            .ToTable(tb => tb.UseSqlOutputClause(false));

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_DocumentType");

                entity.ToTable("DocumentType");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("Id");

                entity.Property(e => e.Title)
                    .HasMaxLength(50) // Максимальная длина в соответствии с структурой таблицы
                    .HasColumnName("Title")
                    .HasDefaultValue(null); // Установлено значение по умолчанию
            });

            modelBuilder.Entity<ModelPart>(entity =>
            {
                entity.ToTable("ModelPart");

                // Настройка первичного ключа
                entity.HasKey(e => e.Id);

                // Настройка свойств
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd(); // Автоинкремент

                entity.Property(e => e.ModelId)
                    .IsRequired(); // Обязательное поле

                entity.Property(e => e.PartId)
                    .IsRequired(); // Обязательное поле

                // Настройка внешнего ключа для ModelId
                entity.HasOne(e => e.Model)
                    .WithMany() // Если у модели есть коллекция ModelParts, укажите здесь
                    .HasForeignKey(e => e.ModelId);

                // Настройка внешнего ключа для PartId
                entity.HasOne(e => e.Part)
                    .WithMany() // Если у детали есть коллекция ModelParts, укажите здесь
                    .HasForeignKey(e => e.PartId)
                    ; // Поведение при удалении
            });

            modelBuilder.Entity<ModelDocument>(entity =>
            {
                entity.ToTable("ModelDocument");

                // Настройка первичного ключа
                entity.HasKey(e => e.Id);

                // Настройка внешнего ключа для ModelId
                entity.HasOne(e => e.Model)
                      .WithMany() // Укажите навигационное свойство в классе Model, если оно есть
                      .HasForeignKey(e => e.ModelId)
                      .OnDelete(DeleteBehavior.Restrict); // Задайте поведение при удалении

                // Настройка внешнего ключа для DocumentId
                entity.HasOne(e => e.Document)
                      .WithMany() // Укажите навигационное свойство в классе Document, если оно есть
                      .HasForeignKey(e => e.DocumentId)
                      .OnDelete(DeleteBehavior.Restrict); // Задайте поведение при удалении
            });

            modelBuilder.Entity<Parts>(entity =>
            {
                entity.ToTable("Part");

                // Настройка первичного ключа
                entity.HasKey(e => e.Id);

                // Настройка свойств
                entity.Property(e => e.PartNumber)
                    .HasColumnName("Title")
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Link)
                    .HasColumnName("Link")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.PartName)
                    .HasColumnName("Description")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Note)
                    .HasColumnName("Note")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.SiteId)
                    .HasColumnName("SiteId");

                // Настройка навигационного свойства
                entity.HasOne(d => d.Site)
                    .WithMany() // Замените пустые круглые скобки на необходимое количество зависимых действий (если у вас есть коллекция Site)
                    .HasForeignKey(d => d.SiteId)
                    .OnDelete(DeleteBehavior.Restrict); // Установите способ удаления в зависимости от вашего контекста
            });



            modelBuilder.Entity<Model>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_models_id");

                entity.ToView("models");

                entity.HasIndex(e => e.CategoriesId, "IX_CategoriesId");
                entity.HasIndex(e => e.model, "IX_Model");
                entity.HasIndex(e => e.SourceId, "IX_SourceId");
                entity.HasIndex(e => e.Token).IsUnique().HasDatabaseName("models$token");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Brand)
                    .HasMaxLength(100)
                    .HasColumnName("brand")
                    .HasDefaultValue(null);

                entity.Property(e => e.CategoriesId)
                    .IsRequired()
                    .HasColumnName("categories_id");

                entity.Property(e => e.CleanedModel)
                    .HasMaxLength(40)
                    .HasColumnName("cleaned_model");

                entity.Property(e => e.Confidence)
                    .HasMaxLength(1)
                    .HasColumnName("confidence")
                    .IsRequired();

                entity.Property(e => e.DateModel)
                    .HasColumnType("date")
                    .HasColumnName("date_model")
                    .HasDefaultValue(null);

                entity.Property(e => e.FileName)
                    .HasMaxLength(100)
                    .HasColumnName("file_names")
                    .HasDefaultValue(null);

                entity.Property(e => e.FileTitle)
                    .HasMaxLength(100)
                    .HasColumnName("file_title")
                    .HasDefaultValue(null);

                entity.Property(e => e.LocalPath)
                    .HasMaxLength(200)
                    .HasColumnName("local_path")
                    .HasDefaultValue(null);

                entity.Property(e => e.model)
                    .HasMaxLength(40)
                    .HasColumnName("model")
                    .IsRequired();



                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id");

                entity.Property(e => e.Token)
                    .HasMaxLength(6)
                    .HasColumnName("token")
                    .HasDefaultValue(null);

                entity.Property(e => e.Version)
                    .HasMaxLength(100)
                    .HasColumnName("version")
                    .HasDefaultValue(null);

                entity.Property(e => e.WebLink)
                    .HasMaxLength(200)
                    .HasColumnName("web_link")
                    .HasDefaultValue(null);

            });

            modelBuilder.Entity<ModelNumber>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_model_numbers_id");

                entity.ToTable("model_numbers");

                entity.HasIndex(e => e.JobId, "IX_JobId");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Confirmed)
                    .HasMaxLength(2)
                    .HasColumnName("confirmed")
                    .IsRequired();

                entity.Property(e => e.GotForWorkAt)
                    .HasColumnType("datetime2")
                    .HasColumnName("got_for_work_at");

                entity.Property(e => e.SerialNumber)
                   .HasColumnName("serial_number");

                entity.Property(e => e.BrandId)
                               .HasColumnName("brand_id");

                entity.Property(e => e.Brand)
                  .HasColumnName("brand");


                entity.Property(e => e.JobId)
                    .IsRequired()
                    .HasColumnName("job_id");

                entity.Property(e => e.ModelNumber1)
                    .HasMaxLength(100)
                    .HasColumnName("model_number")
                    .IsRequired();

                entity.Property(e => e.DocCounter)
                   .HasColumnName("doc_counter");

                entity.Property(e => e.jdConfirmed)
                  .HasColumnName("jd_confirmed");

                entity.Property(e => e.CleanedModel)
                   .HasColumnName("cleaned_model")
                   ;



                entity.HasOne(d => d.Job)
                            .WithMany(p => p.ModelNumbers)
                            .HasForeignKey(d => d.JobId)
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_model_numbers_Job");
            });

            modelBuilder.Entity<ModelNumber>()
       .ToTable(tb => tb.UseSqlOutputClause(false));

            modelBuilder.Entity<Sources>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_sources_id");

                entity.ToTable("sources");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.SourceName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("source_name");

                entity.Property(e => e.Confidence)
                    .IsRequired()
                    .HasMaxLength(1)
                    .HasColumnName("confidence");

                entity.Property(e => e.DataTypes)
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("data_types")
                    .HasDefaultValue(null);

                entity.Property(e => e.FolderPath)
                    .HasMaxLength(500)
                    .HasColumnName("folder_path")
                    .HasDefaultValue(null);
            });

            modelBuilder.Entity<ModelsNumbersNotFound>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_models_numbers_not_found_id");

                entity.ToTable("models_numbers_not_found");

                entity.HasIndex(e => e.JobId, "IX_JobId");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.JobId)
                    .IsRequired()
                    .HasColumnName("job_id");

                entity.Property(e => e.ModelNumber)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("model_number");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.ModelsNumbersNotFounds)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_models_numbers_not_found_Job");
            });

            modelBuilder.Entity<PartsAndReplace>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_parts_and_replaces_id");

                entity.ToTable("parts_and_replaces");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.GotForWorkAt)
                    .HasColumnType("datetime")
                    .HasColumnName("got_for_work_at");

                entity.Property(e => e.MainPartNumber)
                    .HasMaxLength(20)
                    .HasColumnName("main_part_number");

                entity.Property(e => e.PartName)
                    .HasMaxLength(70)
                    .HasColumnName("part_name");

                entity.Property(e => e.PicBase64)
                    .HasColumnName("pic_base64");

                entity.Property(e => e.Replaces)
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("replaces");

                entity.Property(e => e.Status)
                    .HasDefaultValue("0")
                    .HasColumnName("status");

                entity.Property(e => e.Brand).
                   HasColumnName("brand");
                entity.Property(e => e.Pic).
                   HasColumnName("pic");
                entity.Property(e => e.PicLink).
                  HasColumnName("pic_link");

                entity.Property(e => e.PhotoStatus).
             HasColumnName("Photo_status");

                entity.Property(e => e.DateUpdate).
          HasColumnName("date_update");
            });

            modelBuilder.Entity<JobDocInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_job_docs_info_Id");

                entity.ToTable("job_docs_info");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.SourceId)
                    .IsRequired()
                    .HasColumnName("source_id");

                entity.Property(e => e.JobId)
                    .IsRequired()
                    .HasColumnName("job_id");

                entity.Property(e => e.ModelsNumberId)
                    .IsRequired()
                    .HasColumnName("models_number_id");

                entity.Property(e => e.ModelId)
                    .HasColumnName("model_id");

                entity.Property(e => e.LocalPath)
                    .HasMaxLength(200)
                    .HasColumnName("local_path")
                    .HasDefaultValue(null);
            });

            //modelBuilder.Entity<Parts>(entity =>
            //{
            //    entity.HasKey(e => e.Id).HasName("PK_parts_id");

            //    entity.ToTable("parts");

            //    entity.Property(e => e.Id)
            //        .ValueGeneratedOnAdd()
            //        .HasColumnName("id");

            //    entity.Property(e => e.PartName)
            //        .HasMaxLength(30)
            //        .HasColumnName("part_name")
            //        .HasDefaultValue(null);

            //    entity.Property(e => e.PartNumber)
            //        .IsRequired()
            //        .HasMaxLength(30)
            //        .HasColumnName("part_number");

            //    entity.Property(e => e.IdModel)
            //        .HasColumnName("id_model")
            //        .HasDefaultValue(null);
            //});

            modelBuilder.Entity<StopWords>(entity =>
            {
                entity.ToTable("Stop_words");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Word)
                    .IsRequired() // Добавлено, если слово обязательно
                    .HasMaxLength(30)
                    .HasColumnName("word");

                entity.Property(e => e.UserId)
                    .IsRequired() // Добавлено, если user_id обязательно
                    .HasColumnName("user_id");
            });

            modelBuilder.Entity<ImageText>(entity =>
            {
                entity.ToTable("image_text");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.text)  // Изменено на Text с заглавной буквы для соответствия стилю C#
                    .IsRequired() // Добавлено, если поле обязательно
                    .HasMaxLength(4000)
                    .HasColumnName("text");

                entity.Property(e => e.file)
                    .IsRequired() // Добавлено, если поле обязательно
                    .HasMaxLength(4000)
                    .HasColumnName("file");

                entity.Property(e => e.folder)
                    .IsRequired() // Добавлено, если поле обязательно
                    .HasMaxLength(4000)
                    .HasColumnName("folder");
            });

            modelBuilder.Entity<Response>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_Responses_id");

                entity.HasIndex(e => e.CrmId, "fk_responses_crm");
                entity.HasIndex(e => e.JobId, "job_id");

                entity.ToTable("Responses");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.AttemptCount)
                    .HasDefaultValue(0)
                    .HasColumnType("int")
                    .HasColumnName("attempt_count");

                entity.Property(e => e.CrmId)
                    .HasColumnType("int")
                    .HasColumnName("CRM_id");

                entity.Property(e => e.JobId)
                    .IsRequired()
                    .HasColumnType("int")
                    .HasColumnName("job_id");

                entity.Property(e => e.JobLink)
                    .HasMaxLength(255)
                    .HasColumnName("job_link");

                entity.Property(e => e.ResponseText)
                    .IsRequired()
                    .HasColumnType("nvarchar(max)")
                    .HasColumnName("response_text");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status");

                entity.Property(e => e.TgStatus)
                    .IsRequired()
                    .HasColumnName("TG_status");

                entity.Property(e => e.UserId)
                    .HasColumnType("int")
                    .HasColumnName("user_id");

                entity.Property(e => e.WhatsappStatus)
                    .IsRequired()
                    .HasColumnName("Whatsapp_status");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Responses)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Responses_ibfk_1");
            });

            modelBuilder.Entity<StockCred>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_stock_creds_id");

                entity.ToTable("stock_creds");

                entity.HasIndex(e => e.ContactTypesId, "contact_types_id");
                entity.HasIndex(e => e.DocTypesId, "doc_types_id");
                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.ContactAddress)
                    .HasMaxLength(30)
                    .HasColumnName("contact_address");

                entity.Property(e => e.ContactTypesId)
                    .IsRequired()
                    .HasColumnName("contact_types_id");

                entity.Property(e => e.DocTypesId)
                    .HasColumnName("doc_types_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(30)
                    .HasDefaultValue("partsbot@gmail.com")
                    .HasColumnName("email");

                entity.Property(e => e.ExcelColumn)
                    .HasMaxLength(2)
                    .HasColumnName("excel_column");

                entity.Property(e => e.FileName)
                    .HasMaxLength(300)
                    .HasColumnName("file_name");

                entity.Property(e => e.Partmanager)
                    .HasMaxLength(40)
                    .HasColumnName("partmanager");

                entity.Property(e => e.RecordsCount)
                    .HasDefaultValue(0)
                    .HasColumnName("records_count");

                entity.Property(e => e.ReplaceRecords)
                    .HasDefaultValue(0)
                    .HasColumnName("replace_records");

                entity.Property(e => e.StockLink)
                    .HasMaxLength(300)
                    .HasColumnName("stock_link");

                entity.Property(e => e.StockName)
                    .HasMaxLength(30)
                    .HasColumnName("stock_name");

                entity.Property(e => e.SyncFreq)
                    .HasMaxLength(5)
                    .HasDefaultValue("60")
                    .HasColumnName("sync_freq");

                entity.Property(e => e.SyncSwitch)
                    .HasDefaultValue("ON")
                    .HasColumnName("sync_switch");

                entity.Property(e => e.UpdateStatus)
                    .HasColumnName("update_status");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("update_time");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id");

                entity.Property(e => e.MaxRowsPerUpload)
                   .HasColumnName("max_rows_per_upload");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.StockCreds)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("stock_creds_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_users_id");

                entity.ToTable("users");

                entity.HasIndex(e => e.CrmId, "CRM_id");

                // уникальный индекс по TelegramId (анти-дубликат)
                entity.HasIndex(e => e.TelegramId)
                      .IsUnique();

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Login)
                    .HasMaxLength(30)
                    .HasColumnName("login")
                    .HasDefaultValue(null);

                entity.Property(e => e.Password)
                    .HasMaxLength(30)
                    .HasColumnName("password")
                    .HasDefaultValue(null);

                entity.Property(e => e.CrmId)
                    .HasColumnName("CRM_id")
                    .HasDefaultValue(null);

                entity.Property(e => e.CrmLogin)
                    .HasMaxLength(30)
                    .HasColumnName("CRM_login")
                    .HasDefaultValue(null);

                entity.Property(e => e.CrmPassword)
                    .HasMaxLength(30)
                    .HasColumnName("CRM_password")
                    .HasDefaultValue(null);

                entity.Property(e => e.Proxy)
                    .HasMaxLength(60)
                    .HasColumnName("proxy")
                    .HasDefaultValue(null);

                entity.Property(e => e.SyncFreq)
                    .HasMaxLength(5)
                    .HasColumnName("sync_freq")
                    .HasDefaultValue("10");

                entity.Property(e => e.SyncSwitch)
                    .HasMaxLength(3)
                    .HasColumnName("sync_switch")
                    .HasDefaultValue("ON");

                entity.Property(e => e.UpdateStatus)
                    .HasMaxLength(20)
                    .HasColumnName("update_status")
                    .HasDefaultValue(null);

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasDefaultValue(null);

                entity.Property(e => e.ChatId)
                    .HasColumnName("chat_id")
                    .HasDefaultValue(null);

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasDefaultValue(123);

                entity.Property(e => e.PrefersCrm)
                    .HasMaxLength(1)
                    .HasColumnName("prefers_crm")
                    .HasDefaultValue("0");

                entity.Property(e => e.PrefersWhatsapp)
                    .HasMaxLength(1)
                    .HasColumnName("prefers_whatsapp")
                    .HasDefaultValue("0");

                entity.Property(e => e.PrefersTelegram)
                    .HasMaxLength(1)
                    .HasColumnName("prefers_telegram")
                    .HasDefaultValue("0");

                entity.Property(e => e.AllHistory)
                    .HasMaxLength(255)
                    .HasColumnName("all_history")
                    .HasDefaultValue(null);

                entity.Property(e => e.TelegramId)
                    .HasColumnName("telegram_id")
                    .HasDefaultValue(null);

                entity.Property(e => e.TelegramState)
                    .HasMaxLength(20)
                    .HasColumnName("telegram_state")
                    .HasDefaultValue(null);

                entity.Property(e => e.LastUpdatedDateTime)
                    .HasColumnName("lastUpdatedDateTime")
                    .HasColumnType("datetime")
                    .HasDefaultValue(null);

                entity.Property(e => e.DiagramProbability)
                    .HasColumnName("diagram_probability")
                    .HasDefaultValue(50);

                entity.Property(e => e.ManualProbability)
                    .HasColumnName("manual_probability")
                    .HasDefaultValue(25);

                entity.Property(e => e.TgState)
                    .HasColumnName("tg_state");

                entity.Property(e => e.ProUntil)
                    .HasColumnName("pro_until")
                    .HasColumnType("datetime")
                    .HasDefaultValue(null);

                entity.Property(e => e.LastRepairVideoId)
                    .HasColumnName("last_repair_video_id")
                    .HasDefaultValue(null);

                entity.Property(e => e.LastWarehouseVideoId)
                    .HasColumnName("last_warehouse_video_id")
                    .HasDefaultValue(null);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime")
                    .HasDefaultValue(null);

                entity.Property(e => e.LastVideoTipId)
                    .HasColumnName("last_video_tip_id")
                    .HasDefaultValue(null);

                entity.Property(e => e.LastLinkTipId)
                    .HasColumnName("last_link_tip_id")
                    .HasDefaultValue(null);

                entity.Property(e => e.LimitedUntil)
                    .HasColumnName("limited_until")
                    .HasDefaultValue(0);

                entity.Property(e => e.RequestsLimit)
                    .HasColumnName("requests_limit")
                    .HasDefaultValue(100);

                entity.Property(e => e.SupportThreadId)
                    .HasColumnName("support_thread_id")
                    .HasDefaultValue(null);

                entity.Property(e => e.Access)
                    .HasColumnName("access")
                    .HasDefaultValue(0);
            });

            modelBuilder.Entity<User>()
                .ToTable(tb => tb.UseSqlOutputClause(false));


            modelBuilder.Entity<PartsRequest>(entity =>
            {
                // Имя таблицы
                entity.ToTable("Parts_request");

                // Первичный ключ
                entity.HasKey(e => e.Id)
                      .HasName("PK_Parts_request_id"); // имя можешь подобрать под свой стиль

                // Колонки

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id");

                entity.Property(e => e.StockId)
                    .HasColumnName("stock_id");

                entity.Property(e => e.PhotoPath)
                    .HasMaxLength(255)
                    .HasColumnName("photo_path");

                entity.Property(e => e.RecognizedPartNumber)
                    .HasMaxLength(50)
                    .HasColumnName("recognized_part_number");

                entity.Property(e => e.PartsAndReplacesId)
                    .HasColumnName("parts_and_replaces_id");

                entity.Property(e => e.Status)
                    .HasColumnName("status"); // tinyint -> byte? в классе

                entity.Property(e => e.CreatedAt)
                    .IsRequired()
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .IsRequired()
                    .HasColumnName("updated_at");

                entity.Property(e => e.JobId)
                    .HasColumnName("job_id");

                // Если нужны связи, можно добавить навигации и FK
                // entity.HasOne(e => e.User)...
                // entity.HasOne(e => e.Stock)...
                // entity.HasOne(e => e.PartsAndReplace)...

                // Если нужно отключить OUTPUT clause (как в UserStock):
                 entity.ToTable(tb => tb.UseSqlOutputClause(false));
            });


            modelBuilder.Entity<UserStock>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_user_stocks_id");

                entity.ToTable("user_stocks");

                entity.HasIndex(e => e.PartsAndReplacesId, "parts_and_replaces_id");
                entity.HasIndex(e => e.StockId, "stock_id");
                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.PartNumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("part_number");

                entity.Property(e => e.PartsAndReplacesId)
                    .HasColumnName("parts_and_replaces_id");

                entity.Property(e => e.StockId)
                    .IsRequired()
                    .HasColumnName("stock_id");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id");

                entity.HasOne(d => d.PartsAndReplaces)
                    .WithMany(p => p.UserStocks)
                    .HasForeignKey(d => d.PartsAndReplacesId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("user_stocks_ibfk_3");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.UserStocks)
                    .HasForeignKey(d => d.StockId)
                    .HasConstraintName("user_stocks_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserStocks)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_stocks_ibfk_1");

                modelBuilder.Entity<UserStock>()
            .ToTable(tb => tb.UseSqlOutputClause(false));
            });


            modelBuilder.Entity<ErrorLog>(entity =>
            {
                // Задаем имя таблицы
                entity.ToTable("Error_log");  // Учитывайте регистр, если это важно для вашей базы данных

                // Задаем первичный ключ
                entity.HasKey(e => e.id);  // Указываем поле id как первичный ключ

                // Конфигурация свойств (Column Types and Nullability)
                entity.Property(e => e.id)
                    .HasColumnName("id")
                    .UseIdentityColumn();  // Указываем, что столбец является Identity (автоинкрементным)

                entity.Property(e => e.user_id)
                    .HasColumnName("user_id")
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime")
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.error_type)
                    .HasColumnName("error_type")
                    .HasColumnType("nvarchar")
                    .HasMaxLength(50)
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.details)
                    .HasColumnName("details")
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.action_taken)
                    .HasColumnName("action_taken")
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.screen_name)
                    .HasColumnName("screen_name")
                    .HasColumnType("nvarchar")
                    .HasMaxLength(100)
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.request_data)
                    .HasColumnName("request_data")
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.job_id)
                    .HasColumnName("job_id")
                    .HasColumnType("int").IsRequired(false); // Allow NULL

                entity.Property(e => e.status)
                    .HasColumnName("status")
                    .HasColumnType("nvarchar")
                    .HasMaxLength(10)
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.script_name)
                    .HasColumnName("script_name")
                    .HasColumnType("nvarchar")
                    .HasMaxLength(100)
                    .IsRequired(true); // NOT NULL

                entity.Property(e => e.error_message)
                    .HasColumnName("error_message")
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(true); // NOT NULL

                entity.Property(e => e.error_time)
                    .HasColumnName("error_time")
                    .HasColumnType("datetime")
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.stock_id)
                    .HasColumnName("stock_id")
                    .HasColumnType("int")
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.additional_data1)
                    .HasColumnName("additional_data1")
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.additional_data2)
                    .HasColumnName("additional_data2")
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.additional_data3)
                    .HasColumnName("additional_data3")
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.additional_data4)
                    .HasColumnName("additional_data4")
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false); // Allow NULL

                entity.Property(e => e.additional_data5)
                    .HasColumnName("additional_data5")
                    .HasColumnType("nvarchar(max)")
                    .IsRequired(false); // Allow NULL
            });

            modelBuilder.Entity<PricebotTask>(entity =>
            {
                // Table Name (Optional, but good to be explicit)
                entity.ToTable("pricebot_tasks");

                // Primary Key
                entity.HasKey(e => e.Id);

                // Property Configurations (Column names)
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .UseIdentityColumn(); // For auto-incrementing identity columns

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id");

                entity.Property(e => e.PartNumber)
                    .HasColumnName("part_number");

                entity.Property(e => e.ShopId)
                    .HasColumnName("shop_id");

                entity.Property(e => e.Status)
                    .HasColumnName("status");

                entity.Property(e => e.Price)
                    .HasColumnName("price");

                entity.Property(e => e.ErrorMessage)
                    .HasColumnName("error_message"); // nvarchar(max) defaults to string

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at");

                entity.Property(e => e.SitePartNumber)
                    .HasColumnName("Site_part_number");

                entity.Property(e => e.PageLink)
                    .HasColumnName("page_link");

                entity.Property(e => e.FoundParts)
                    .HasColumnName("found_parts");

                entity.Property(e => e.Availability)
                    .HasColumnName("availability");

                entity.Property(e => e.PartName)
                    .HasColumnName("part_name");

                entity.Property(e => e.YourPrice)
                    .HasColumnName("your_price");

                entity.Property(e => e.RegularPrice)
                    .HasColumnName("regular_price");

                entity.Property(e => e.SpecialField1)
                    .HasColumnName("special_field_1");

                entity.Property(e => e.SpecialField2)
                    .HasColumnName("special_field_2");

                entity.Property(e => e.SpecialField3)
                    .HasColumnName("special_field_3");

            });

            modelBuilder.Entity<UniqModel>(entity =>
            {

                entity.ToTable("UniqModel");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Title)
                    .HasColumnName("Title");

                entity.Property(e => e.DocCounter)
                    .HasColumnName("doc_counter");


                modelBuilder.Entity<UniqModel>()
            .ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<DocumentPdfText>(entity =>
            {

                entity.ToTable("DocumentPdfText");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("Id");

                entity.Property(e => e.DocumentId)
                    .HasColumnName("DocumentId");

                entity.Property(e => e.PdfText)
                    .HasColumnName("PdfText");


                modelBuilder.Entity<DocumentPdfText>()
            .ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<SharedStock>(entity =>
            {
                entity.ToTable("shared_stocks");

                entity.HasKey(e => new { e.UserId, e.StockId });
                entity.Property(e => e.UserId)
                    .HasColumnName("user_id");

                entity.Property(e => e.StockId)
                    .HasColumnName("stock_id");

                entity.Property(e => e.GrantedByUserId)
                    .HasColumnName("granted_by_user_id")
                    .IsRequired(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    ;

                modelBuilder.Entity<SharedStock>()
                    .ToTable(tb => tb.UseSqlOutputClause(false));
            });

            modelBuilder.Entity<DocumentQa>(entity =>
            {
                entity.ToTable("document_qa");

                entity.HasIndex(e => e.AnalysisId, "IX_DocumentQA_AnalysisId");

                entity.Property(e => e.Id)
                      .HasColumnName("Id");

                entity.Property(e => e.Question)
                      .IsRequired()
                      .HasMaxLength(256)
                      .HasColumnName("Question");

                entity.Property(e => e.Answer)
                      .HasColumnName("Answer");

                entity.Property(e => e.Status)
                      .HasColumnName("Status");

                entity.Property(e => e.AnalysisId)
                      .HasColumnName("AnalysisId");

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("CreatedAt")
                      .HasColumnType("datetime");

                entity.HasOne(d => d.Analysis)
                      .WithMany(p => p.DocumentQas)
                      .HasForeignKey(d => d.AnalysisId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_DocumentQa_Analysis");

                entity.HasMany(d => d.DocumentGenericQas)
                      .WithOne(p => p.Qa)
                      .HasForeignKey(d => d.Qaid)
                      .HasConstraintName("FK_DocumentGenericQa_Qa");
            });

            // DocumentAnalysis
            modelBuilder.Entity<DocumentAnalysis>(entity =>
            {
                entity.ToTable("document_analysis");

                entity.HasIndex(e => e.DocumentId, "IX_document_analysis_DocumentId");

                entity.Property(e => e.Id)
                      .HasColumnName("Id");

                entity.Property(e => e.DocumentId)
                      .HasColumnName("DocumentId");

                entity.Property(e => e.Status)
                      .HasColumnName("Status");

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("CreatedAt")
                      .HasColumnType("datetime");

                entity.HasOne(d => d.Document);

                entity.HasMany(d => d.DocumentGenericQas)
                      .WithOne(p => p.Analysis)
                      .HasForeignKey(d => d.AnalysisId)
                      .HasConstraintName("FK_DocumentGenericQa_Analysis");

                entity.HasMany(d => d.DocumentQas)
                      .WithOne(p => p.Analysis)
                      .HasForeignKey(d => d.AnalysisId)
                      .HasConstraintName("FK_DocumentQa_Analysis");
            });

            // DocumentGenericQa
            modelBuilder.Entity<DocumentGenericQa>(entity =>
            {
                entity.ToTable("document_generic_qa");

                entity.HasIndex(e => e.AnalysisId, "IX_DocumentGenericQA_AnalysisId");
                entity.HasIndex(e => e.QuestionId, "IX_DocumentGenericQA_QuestionId");
                entity.HasIndex(e => e.Qaid, "IX_document_generic_qa_QAId");

                entity.Property(e => e.Id)
                      .HasColumnName("Id");

                entity.Property(e => e.QuestionId)
                      .HasColumnName("QuestionId");

                entity.Property(e => e.Qaid)
                      .HasColumnName("QAId");

                entity.Property(e => e.AnalysisId)
                      .HasColumnName("AnalysisId");

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("CreatedAt")
                      .HasColumnType("datetime");

                entity.HasOne(d => d.Analysis)
                      .WithMany(p => p.DocumentGenericQas)
                      .HasForeignKey(d => d.AnalysisId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_DocumentGenericQa_Analysis");

                entity.HasOne(d => d.Qa)
                      .WithMany(p => p.DocumentGenericQas)
                      .HasForeignKey(d => d.Qaid)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_DocumentGenericQa_Qa");

                entity.HasOne(d => d.Question)
                      .WithMany(p => p.DocumentGenericQas)
                      .HasForeignKey(d => d.QuestionId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_DocumentGenericQa_GenericQuestion");
            });

            // GenericQuestion
            modelBuilder.Entity<GenericQuestion>(entity =>
            {
                entity.ToTable("generic_questions");

                entity.Property(e => e.Id)
                      .HasColumnName("Id");

                entity.Property(e => e.Question)
                      .IsRequired()
                      .HasColumnType("nvarchar(max)")
                      .HasColumnName("Question");

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("CreatedAt")
                      .HasColumnType("datetime");

                entity.HasMany(d => d.DocumentGenericQas)
                      .WithOne(p => p.Question)
                      .HasForeignKey(d => d.QuestionId)
                      .HasConstraintName("FK_DocumentGenericQa_GenericQuestion");
            });
        }
    }

}