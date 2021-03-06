﻿using BioEngine.Common.Base;
using BioEngine.Data.Core;

namespace BioEngine.Data.Search.Commands
{
    public class IndexEntityCommand<T> : CommandBase where T : IBaseModel
    {
        public IndexEntityCommand(T model)
        {
            Model = model;
        }

        public T Model { get; }
    }
}