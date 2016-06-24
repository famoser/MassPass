using System;

namespace Famoser.MassPass.View.Models.Interfaces
{
    public interface ICustomContentModel
    {
        bool CanBeSaved();
        bool ContentChanged();

        EventHandler CanBeSavedChanged { get; set; }
    }
}
