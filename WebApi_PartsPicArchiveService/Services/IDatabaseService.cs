using KameraData.Data.Models;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace WebApi_PartsPicArchiveService.Services
{
  
    public interface IDatabaseService:KameraData.SharedMicroservicesLibrary.Services.IDatabaseService
    {
		Task<PartsPicArchive> GetPartsPicArchiveById(int id);

		Task<bool> DeletePartsPicArchive(int id);
		Task<bool> UpdatePartsPicArchive(PartsPicArchive partsPicArchive);

		Task<bool> AddPartsPicArchive(PartsPicArchive partsPicArchive);
	}

}
