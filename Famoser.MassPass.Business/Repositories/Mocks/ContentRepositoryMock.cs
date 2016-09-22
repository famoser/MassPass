using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Famoser.MassPass.Business.Content.Models;
using Famoser.MassPass.Business.Content.Models.Base;
using Famoser.MassPass.Business.Managers;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Repositories.Interfaces;

namespace Famoser.MassPass.Business.Repositories.Mocks
{
    public class ContentRepositoryMock : IContentRepository
    {
        public ObservableCollection<GroupedCollectionModel> GetGroupedCollectionModels()
        {
            CollectionManager.AddCollectionModel(GetCollectionModel());
            CollectionManager.AddCollectionModel(GetCollectionModel());
            CollectionManager.AddCollectionModel(GetCollectionModel());
            return CollectionManager.GroupedCollectionModels;
        }

        private Random _random = new Random();
        private CollectionModel GetCollectionModel()
        {
            return new CollectionModel(Guid.Empty)
            {
                ContentModels =
                {
                    GenerateContentModel(),
                    GenerateContentModel(),
                    GenerateContentModel(),
                    GenerateContentModel(),
                    GenerateContentModel(),
                }
            };
        }

        private BaseContentModel GenerateContentModel()
        {
            switch (_random.Next(0,2))
            {
                case 0:

                    return new NoteModel(Guid.Empty)
                    {
                        Description = "A pretty long description about this note, far too long to fit on one line",
                        Name = "My first Note!"
                    };
                case 1:
                    return new CreditCardModel(Guid.Empty)
                    {
                        Description = "BLKB Credit card",
                        Name = "BLKB Master card"
                    };
                default:
                    return new LoginModel(Guid.Empty)
                    {
                        Description = "facebook",
                        Name = "Login facebook.com"
                    };
            }
        }

        public Task<bool> SyncAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> LoadValues(BaseContentModel model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FillHistory(BaseContentModel model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Save(BaseContentModel model)
        {
            throw new NotImplementedException();
        }
    }
}
