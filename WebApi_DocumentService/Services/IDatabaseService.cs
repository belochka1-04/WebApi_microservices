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

        // Для DocumentQasController:
        Task<DocumentQa?> GetDocumentQaByIdAsync(int id);
        Task<DocumentQa> CreateDocumentQaAsync(int analysisId, string question);
        Task<List<DocumentQa>> GetDocumentQasByAnalysisIdAsync(int analysisId);
        Task UpdateDocumentQaStatusAsync(int id, int status);

        // Для DocumentAnalysesController:
        Task<DocumentAnalysis?> GetDocumentAnalysisByIdAsync(int id);
        Task<DocumentAnalysis?> GetDocumentAnalysisWithIncludesAsync(int id); // Include GenericQas + Qas
        Task<DocumentAnalysis?> GetDocumentAnalysisByDocumentIdAsync(int documentId);
        Task<DocumentAnalysis> CreateDocumentAnalysisAsync(int documentId);
        Task<List<DocumentGenericQa>> GetDocumentGenericQasByAnalysisIdAsync(int analysisId);

    }

}
