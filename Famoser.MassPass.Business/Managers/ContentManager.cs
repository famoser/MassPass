using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Famoser.MassPass.Business.Enums;
using Famoser.MassPass.Business.Helpers;
using Famoser.MassPass.Business.Models;
using Famoser.MassPass.Business.Models.Storage;
using Famoser.MassPass.Data.Enum;

namespace Famoser.MassPass.Business.Managers
{
    public static class ContentManager
    {
        public static readonly ObservableCollection<ContentModel> FlatContentModelCollection = new ObservableCollection<ContentModel>();
        public static readonly ContentModel RootContentModel = new ContentModel()
        {
            HistoryLoadingState = LoadingState.Loaded,
            RuntimeStatus = RuntimeStatus.Idle,
            ContentLoadingState = LoadingState.Loaded,
            LocalStatus = LocalStatus.Immutable,
            Name = "Sammlungen"
        };

        private static readonly List<ContentModel> ParentLessModels = new List<ContentModel>();
        public static void AddOrReplaceContent(ContentModel content)
        {
            //if already in collection, remove it
            var existing = FlatContentModelCollection.FirstOrDefault(c => c.Id == content.Id);
            if (existing != null)
            {
                FlatContentModelCollection.Remove(existing);
                if (RootContentModel.Contents.Contains(existing))
                    RootContentModel.Contents.Remove(existing);
                if (ParentLessModels.Contains(existing))
                    ParentLessModels.Remove(existing);
            }

            //add to collections
            FlatContentModelCollection.Add(content);
            if (content.ParentId != Guid.Empty)
                ParentLessModels.Add(content);
            else
            {
                content.Parent = RootContentModel;
                RootContentModel.Contents.Add(content);
            }

            ResolveParentLessModels();
        }

        public static void AddFromCache(CollectionCacheModel cache)
        {
            var models = CacheHelper.ReadCache(cache);
            foreach (var contentModel in models)
            {
                AddOrReplaceContent(contentModel);
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
                    parentLessModel.Parent = parent;
                    parent.Contents.Add(parentLessModel);

                    ParentLessModels.Remove(parentLessModel);
                    index--;
                }
            }
        }
    }
}
