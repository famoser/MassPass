﻿using System;
using System.Collections.ObjectModel;
using Famoser.MassPass.Business.Models.Base;
using Famoser.MassPass.Data.Models;

namespace Famoser.MassPass.Business.Models
{
    public class CollectionModel : BaseModel
    {
        public CollectionModel(Guid id, Guid typeId, EntityServerInformations entityServerInformations, CollectionModel parent = null)
        {
            Id = id;
            TypeId = typeId;
            EntityServerInformations = entityServerInformations;
            Parent = parent;
            Contents = new ObservableCollection<ContentModel>();
        }

        public Guid Id { get; }

        public Guid TypeId { get; }

        public EntityServerInformations EntityServerInformations { get; }

        public CollectionModel Parent { get; }

        public ObservableCollection<ContentModel> Contents { get; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        public bool IsCollection => Id == Guid.Empty;
        
        public ContentModel CreateNewContent(Guid typeId, EntityServerInformations infos)
        {
            return new ContentModel(Guid.NewGuid(), typeId, infos, this);
        }
    }
}
