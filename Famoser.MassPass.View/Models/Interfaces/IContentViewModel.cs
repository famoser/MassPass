using System.Threading.Tasks;

namespace Famoser.MassPass.View.Models.Interfaces
{
    public interface IContentViewModel
    {
        ICustomContentModel PrepareCustomContentModel();
        bool SaveToContentModel();
    }
}
