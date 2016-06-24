namespace Famoser.MassPass.View.Models
{
    public class NoteModel : NameModel
    {
        private string _content;
        public string Content
        {
            get { return _content; }
            set
            {
                if (Set(ref _content, value))
                    RegisterValueChange("Content", _content);
            }
        }
    }
}
