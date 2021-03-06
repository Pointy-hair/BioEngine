﻿using System.Threading.Tasks;
using BioEngine.Data.Core;
using BioEngine.Data.Social.Commands;
using BioEngine.Social.Twitter;
using JetBrains.Annotations;

namespace BioEngine.Data.Social.Handlers
{
    [UsedImplicitly]
    internal class PublishTweetHandler : QueryHandlerBase<PublishTweetCommand, long>
    {
        private readonly TwitterService _twitterService;

        public PublishTweetHandler(HandlerContext<PublishTweetHandler> context,
            TwitterService twitterService) : base(context)
        {
            _twitterService = twitterService;
        }

        protected override async Task<long> RunQueryAsync(PublishTweetCommand command)
        {
            var tweetId = await Task.FromResult(_twitterService.CreateTweet(command.Text));

            return tweetId;
        }
    }
}