using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentQueue<ContentModel> NeedSavingModels = new ConcurrentQueue<ContentModel>();
        public static void AddOrReplaceContent(ContentModel content, bool isAlreadySaved = false)
        {
            //if already in collection, remove it
            var existing = FlatContentModelCollection.FirstOrDefault(c => c.Id == content.Id);
            if (existing != null)
            {
                FlatContentModelCollection.Remove(existing);
                if (ContentModelCollection.Contains(existing))
                    ContentModelCollection.Remove(existing);
                if (ParentLessModels.Contains(existing))
                    ParentLessModels.Remove(existing);
            }

            //add to collections
            FlatContentModelCollection.Add(content);
            if (content.ParentId != Guid.Empty)
                ParentLessModels.Add(content);
            else
                ContentModelCollection.Add(content);

            ResolveParentLessModels();

            if (!isAlreadySaved)
                NeedSavingModels.Enqueue(content);
        }

        public static void AddFromCache(CollectionCacheModel cache)
        {
            var models = CacheHelper.ReadCache(cache);
            foreach (var contentModel in models)
            {
                AddOrReplaceContent(contentModel, true);
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

        public static List<ContentModel> UnsavedModels()
        {
            var list = new List<ContentModel>();
            ContentModel res;
            while (NeedSavingModels.TryDequeue(out res))
            {
                list.Add(res);
            }
            return list;
        }

        public static void NeedsSaving(ContentModel model)
        {
            if (!NeedSavingModels.Contains(model))
                NeedSavingModels.Enqueue(model);
        }
    }
}
