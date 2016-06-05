using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Famoser.MassPass.Business.Helpers;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Models.Storage;

namespace Famoser.MassPass.Business.Managers
{
    public static class ContentManager
    {
        public static readonly ObservableCollection<ContentModel> FlatContentModelCollection = new ObservableCollection<ContentModel>();
        public static readonly ObservableCollection<ContentModel> ContentModelCollection = new ObservableCollection<ContentModel>();

        private static readonly List<ContentModel> ParentLessModels = new List<ContentModel>(); 
        public static void AddContent(ContentModel content)
        {
            FlatContentModelCollection.Add(content);
            if (content.ParentId != Guid.Empty)
            {
                ParentLessModels.Add(content);
            }
            ResolveParentLessModels();
        }

        public static void AddFromCache(CollectionCacheModel cache)
        {
            var models = CacheHelper.ReadCache(cache);
            foreach (var contentModel in models)
            {
                AddContent(contentModel);
            }
        }

        public static CollectionCacheModel CreateCacheModel()
        {
            return CacheHelper.CreateCache(FlatContentModelCollection);
        }

        private static void ResolveParentLessModels()
        {
            for (int index = 0; index < ParentLessModels.Count; index++)
            {
                var parentLessModel = ParentLessModels[index];
                var parent = FlatContentModelCollection.FirstOrDefault(p => p.Id == parentLessModel.ParentId);
                if (parent != null)
                {
                    parent.Contents.Add(parentLessModel);
                    ParentLessModels.Remove(parentLessModel);
                    index--;
                }
            }
        }
    }
}
