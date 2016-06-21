using System.Threading.Tasks;

namespace Famoser.MassPass.View.Models.Interfaces
{
    public interface IContentViewModel
    {
        ICustomContentModel PrepareModel();
        bool SaveModel();
    }
}
