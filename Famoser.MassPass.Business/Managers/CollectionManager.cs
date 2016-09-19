using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Models.Content;
using Famoser.MassPass.Business.Models.Content.Base;

namespace Famoser.MassPass.Business.Managers
{
    public static class CollectionManager
    {
        private static ObservableCollection<CollectionModel> CollectionModels { get; } = new ObservableCollection<CollectionModel>();
        private static ObservableCollection<BaseContentModel> ContentModels { get; } = new ObservableCollection<BaseContentModel>();

        //per type
        private static ObservableCollection<BaseContentModel> NoteModels { get; } = new ObservableCollection<BaseContentModel>();
        private static ObservableCollection<BaseContentModel> LoginModels { get; } = new ObservableCollection<BaseContentModel>();
        private static ObservableCollection<BaseContentModel> CreditCardModels { get; } = new ObservableCollection<BaseContentModel>();

        public static ObservableCollection<GroupedCollectionModel> GroupedCollectionModels { get; } = new ObservableCollection<GroupedCollectionModel>()
        {
            new GroupedCollectionModel("all", ContentModels),
            new GroupedCollectionModel("notes", NoteModels),
            new GroupedCollectionModel("logins", LoginModels),
            new GroupedCollectionModel("credit cards", CreditCardModels)
        };

        public static void AddContentModel(BaseContentModel model)
        {
            ContentModels.Add(model);

            if (model.GetType() == typeof(NoteModel))
                NoteModels.Add((NoteModel)model);
            else if (model.GetType() == typeof(LoginModel))
                LoginModels.Add((LoginModel)model);
            else if (model.GetType() == typeof(CreditCardModel))
                CreditCardModels.Add((CreditCardModel)model);
        }

        public static void AddCollectionModel(CollectionModel model)
        {
            CollectionModels.Add(model);
            foreach (var baseContentModel in model.ContentModels)
            {
                AddContentModel(baseContentModel);
            }
        }
    }
}
