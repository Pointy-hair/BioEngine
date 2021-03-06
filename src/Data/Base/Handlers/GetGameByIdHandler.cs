﻿using System.Threading.Tasks;
using BioEngine.Common.Models;
using BioEngine.Data.Base.Queries;
using BioEngine.Data.Core;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace BioEngine.Data.Base.Handlers
{
    [UsedImplicitly]
    internal class GetGameByIdHandler : QueryHandlerBase<GetGameByIdQuery, Game>
    {
        public GetGameByIdHandler(HandlerContext<GetGameByIdHandler> context) : base(context)
        {
        }

        protected override async Task<Game> RunQueryAsync(GetGameByIdQuery message)
        {
            return await DBContext.Games.Include(x => x.Developer).FirstOrDefaultAsync(x => x.Id == message.Id);
        }
    }
}