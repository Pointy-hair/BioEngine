﻿using System.Linq;
using System.Threading.Tasks;
using BioEngine.Common.Models;
using BioEngine.Data.Core;
using BioEngine.Data.Files.Queries;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace BioEngine.Data.Files.Handlers
{
    [UsedImplicitly]
    internal class GetFileByIdHandler : QueryHandlerBase<GetFileByIdQuery, File>
    {
        public GetFileByIdHandler(HandlerContext<GetFileByIdHandler> context) : base(context)
        {
        }

        protected override async Task<File> RunQueryAsync(GetFileByIdQuery message)
        {
            var file = await DBContext.Files
                .Where(x => x.Id == message.Id)
                .Include(x => x.Cat)
                .Include(x => x.Author)
                .Include(x => x.Game)
                .Include(x => x.Developer)
                .FirstOrDefaultAsync();
            if (file != null)
            {
                file.Cat =
                    await Mediator.Send(
                        new FileCategoryProcessQuery(file.Cat, new GetFilesCategoryQuery {Parent = file.Parent}));
            }
            return file;
        }
    }
}