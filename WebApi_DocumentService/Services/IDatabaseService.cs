using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_document.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
        Task<KameraData.Data.Models.Document> GetDocByIdAsync(int ID);
        Task<DocumentPdfText> GetDocPdfByIdAsync(int docID);

    }

}
