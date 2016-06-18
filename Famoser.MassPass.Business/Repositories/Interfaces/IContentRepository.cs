using System.Collections.Generic;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;

namespace Famoser.MassPass.Business.Repositories.Interfaces
{
    public interface IContentRepository
    {
        Task<bool> FillValues(ContentModel model);
        Task<bool> FillHistory(ContentModel model);
        Task<List<ContentModel>> ReadOutAll();
        Task<bool> GetContentModelForHistory(HistoryModel model);
        Task<bool> SaveAll();
        
        ContentModel GetSampleContent();
        Task<bool> DeleteAll();
    }
}
