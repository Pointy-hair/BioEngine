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
    internal class UpdateNewsHandler : RestCommandHandlerBase<UpdateNewsCommand, bool>
    {
        public UpdateNewsHandler(HandlerContext<UpdateNewsHandler> context, IValidator<UpdateNewsCommand>[] validators)
            : base(context, validators)
        {
        }

        protected override async Task<bool> ExecuteCommand(UpdateNewsCommand command)
        {
            command.LastChangeDate = DateTimeOffset.Now.ToUnixTimeSeconds();

            await Validate(command);

            var needTweetUpd = command.Model.Pub == 1 &&
                               (command.Title != command.Model.Title || command.Url != command.Model.Url);

            Mapper.Map(command, command.Model);
            if (needTweetUpd)
            {
                await Mediator.Send(new ManageNewsTweetCommand(command.Model, TwitterOperationEnum.CreateOrUpdate));
            }

            DBContext.Update(command.Model);
            await DBContext.SaveChangesAsync();
            return true;
        }
    }
}