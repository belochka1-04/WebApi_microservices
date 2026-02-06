using KameraData.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KameraData.Data.Configurations
{
    public class JobDocsModelInfoConfiguration : IEntityTypeConfiguration<JobDocsModelInfo>
    {
        public void Configure(EntityTypeBuilder<JobDocsModelInfo> entity)
        {
            entity.HasNoKey();
            entity.ToView("job_docs_model_info");

            entity.Property(e => e.Brand).HasMaxLength(100).HasColumnName("brand");
            entity.Property(e => e.CategoriesId).HasColumnType("int").HasColumnName("categories_id");
            entity.Property(e => e.DateModel).HasColumnName("date_model");
            entity.Property(e => e.FileName).HasMaxLength(100).HasColumnName("file_name");
            entity.Property(e => e.FileTitle).HasMaxLength(100).HasColumnName("file_title");
            entity.Property(e => e.Id).HasColumnType("int").HasColumnName("id");
            entity.Property(e => e.Jdbrand).HasMaxLength(100).HasColumnName("jdbrand");
            entity.Property(e => e.JdcategoriesId).HasColumnType("int").HasColumnName("jdcategories_id");
            entity.Property(e => e.JddateModel).HasColumnName("jddate_model");
            entity.Property(e => e.JdfileName).HasMaxLength(100).HasColumnName("jdfile_name");
            entity.Property(e => e.JdfileTitle).HasMaxLength(100).HasColumnName("jdfile_title");
            entity.Property(e => e.JdlocalPath).HasMaxLength(200).HasColumnName("jdlocal_path");
            entity.Property(e => e.Jdmodel).HasMaxLength(40).HasColumnName("jdmodel");
            entity.Property(e => e.JdmodelConfidence).HasMaxLength(1).HasColumnName("jdmodel_confidence");
            entity.Property(e => e.JdmodelVersion).HasMaxLength(100).HasColumnName("jdmodel_version");
            entity.Property(e => e.JdsourceId).HasColumnType("int").HasColumnName("jdsource_id");
            entity.Property(e => e.JdwebLink).HasMaxLength(200).HasColumnName("jdweb_link");
            entity.Property(e => e.JobDescription).HasColumnType("nvarchar(max)").HasColumnName("job_description");
            entity.Property(e => e.JobDocModelId).HasColumnType("int").HasColumnName("job_doc_model_id");
            entity.Property(e => e.JobId).HasColumnType("int").HasColumnName("job_id");
            entity.Property(e => e.JobNotes).HasColumnType("nvarchar(max)").HasColumnName("job_notes");
            entity.Property(e => e.LocalPath).HasMaxLength(200).HasColumnName("local_path");
            entity.Property(e => e.Model).HasMaxLength(40).HasColumnName("model");
            entity.Property(e => e.ModelConfidence).HasMaxLength(1).HasColumnName("model_confidence");
            entity.Property(e => e.ModelId).HasColumnType("int").HasColumnName("model_id");
            entity.Property(e => e.ModelNumber).HasMaxLength(100).HasColumnName("model_number");
            entity.Property(e => e.ModelVersion).HasMaxLength(100).HasColumnName("model_version");
            entity.Property(e => e.ModelsNumberId).HasColumnType("int").HasColumnName("models_number_id");
            entity.Property(e => e.NotFountModel).HasMaxLength(100).HasColumnName("not_fount_model");
            entity.Property(e => e.ResponseText).HasColumnType("nvarchar(max)").HasColumnName("response_text");
            entity.Property(e => e.SourceId).HasColumnType("int").HasColumnName("source_id");
            entity.Property(e => e.Token).HasColumnType("nvarchar(max)").HasColumnName("token");
            entity.Property(e => e.WebLink).HasMaxLength(200).HasColumnName("web_link");
        }
    }
}