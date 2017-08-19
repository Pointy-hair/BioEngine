﻿using System;
using System.Threading.Tasks;
using BioEngine.Data.Core;
using BioEngine.Data.News.Commands;
using BioEngine.Social;
using FluentValidation;
using JetBrains.Annotations;
using Social;

namespace BioEngine.Data.News.Handlers
{
    [UsedImplicitly]
    internal class UnPublishNewsHandler : RestCommandHandlerBase<UnPublishNewsCommand, bool>
    {
        public UnPublishNewsHandler(HandlerContext<UnPublishNewsHandler> context,
            IValidator<UnPublishNewsCommand>[] validators) : base(
            context,
            validators)
        {
        }

        protected override async Task<bool> ExecuteCommand(UnPublishNewsCommand command)
        {
            command.Model.LastChangeDate = DateTimeOffset.Now.ToUnixTimeSeconds();
            command.Model.Pub = 0;

            await Mediator.Send(new ManageNewsTweetCommand(command.Model, TwitterOperationEnum.Delete));

            DBContext.Update(command.Model);
            await DBContext.SaveChangesAsync();
            return true;
        }
    }
}