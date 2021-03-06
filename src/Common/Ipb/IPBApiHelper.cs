﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BioEngine.Common.Interfaces;
using BioEngine.Common.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BioEngine.Common.Ipb
{
    [UsedImplicitly]
    public class IPBApiHelper
    {
        private readonly IPBApiConfig _ipbApiConfig;
        private readonly IContentHelperInterface _contextHelper;
        private readonly ILogger<IPBApiHelper> _logger;
        private readonly HttpClient _client;

        private static readonly Regex BlockQuoteRegex =
            new Regex("<blockquote.*?>(.+?)<\\/blockquote>", RegexOptions.Singleline);

        public IPBApiHelper(IOptions<IPBApiConfig> ipbApiConfig, IContentHelperInterface contextHelper,
            ILogger<IPBApiHelper> logger)
        {
            _ipbApiConfig = ipbApiConfig.Value;
            _contextHelper = contextHelper;
            _logger = logger;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("Authorization", "Basic " + Base64Encode(_ipbApiConfig.ApiKey) + ":");
        }

        private async Task<HttpResponseMessage> DoApiRequestAsync(string method,
            IEnumerable<KeyValuePair<string, string>> data)
        {
            var url = _ipbApiConfig.ApiUrl + method;
            try
            {
                var response = await _client.PostAsync(url, new FormUrlEncodedContent(data));
                return response;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogCritical($"New IPB Request to url {url}. Data: {JsonConvert.SerializeObject(data)}");
                _logger.LogCritical($"Http client exception ({ex.GetType()}): {ex.Message}");
                _logger.LogCritical(862, ex, ex.Message);
                if (ex.InnerException != null)
                {
                    _logger.LogCritical(
                        $"Http client exception ({ex.InnerException.GetType()}): {ex.InnerException.Message}");
                    _logger.LogCritical(862, ex.InnerException, ex.InnerException.Message);
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"New IPB Request to url {url}. Data: {JsonConvert.SerializeObject(data)}");
                _logger.LogCritical($"IPB Error ({ex.GetType()}): {ex.Message}");
                _logger.LogCritical(862, ex, ex.Message);
                throw;
            }
        }

        private async Task<HttpResponseMessage> DoDeleteApiRequestAsync(string method)
        {
            var url = _ipbApiConfig.ApiUrl + method;
            try
            {
                var response = await _client.DeleteAsync(url);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"IPB Delete Error ({ex.GetType()}): {ex.Message}");
                _logger.LogCritical(862, ex, ex.Message);
                throw;
            }
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public async Task<(int topicId, int postId)> CreateOrUpdateNewsTopicAsync(News news)
        {
            (int topicId, int postId) result = (news.ForumTopicId ?? 0, news.ForumPostId ?? 0);
            if (_ipbApiConfig.DevMode)
            {
                var rnd = new Random();
                return (rnd.Next(1, 1000), rnd.Next(1000, 10000));
            }
            if (news.ForumTopicId == null)
            {
                var topicCreateResponse = await DoApiRequestAsync("/forums/topics",
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("forum", _ipbApiConfig.NewsForumId),
                        new KeyValuePair<string, string>("author", news.AuthorId.ToString()),
                        new KeyValuePair<string, string>("title", news.Title),
                        new KeyValuePair<string, string>("hidden", news.Pub == 1 ? "0" : "1"),
                        new KeyValuePair<string, string>("pinned", news.Sticky == 1 ? "1" : "0"),
                        new KeyValuePair<string, string>("post", await GetPostContentAsync(news))
                    });
                var response = await topicCreateResponse.Content.ReadAsStringAsync();
                if (topicCreateResponse.IsSuccessStatusCode)
                {
                    var topicCreateData =
                        JObject.Parse(response);
                    result.topicId = topicCreateData.Value<int>("id");
                    result.postId = topicCreateData["firstPost"].Value<int>("id");
                }
                else
                {
                    throw new Exception($"Can't create topic: {response}");
                }
            }
            else
            {
                var topicTitleUpdateResponse = await DoApiRequestAsync("/forums/topics/" + news.ForumTopicId,
                    new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("title", news.Title),
                    });
                if (!topicTitleUpdateResponse.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"Can't update topic title: {await topicTitleUpdateResponse.Content.ReadAsStringAsync()}");
                }

                var topicStatusUpdateResponse = await DoApiRequestAsync("/forums/topics/" + news.ForumTopicId,
                    new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("hidden", news.Pub == 1 ? "0" : "1"),
                        new KeyValuePair<string, string>("pinned", news.Sticky == 1 ? "1" : "0")
                    });
                if (topicStatusUpdateResponse.IsSuccessStatusCode)
                {
                    var postUpdateResponse = await DoApiRequestAsync("/forums/posts/" + news.ForumPostId,
                        new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("post", await GetPostContentAsync(news))
                        });
                    if (!postUpdateResponse.IsSuccessStatusCode)
                    {
                        throw new Exception(
                            $"Can't update post content: {await postUpdateResponse.Content.ReadAsStringAsync()}");
                    }
                }
            }
            return result;
        }

        private async Task<string> GetPostContentAsync(News news)
        {
            var postContent = await _contextHelper.ReplacePlaceholdersAsync(news.ShortText);
            if (!string.IsNullOrEmpty(news.AddText))
            {
                var addText = "<div class=\"ipsSpoiler\" data-ipsspoiler=\"\">" +
                              "<div class=\"ipsSpoiler_header\">" +
                              "<span>Скрытый текст</span>" +
                              "</div>" +
                              "<div class=\"ipsSpoiler_contents\">" +
                              $"{await _contextHelper.ReplacePlaceholdersAsync(news.AddText)}" +
                              "</div>" +
                              "</div>";

                postContent += addText;
            }

            postContent = postContent.Replace("<ul>", "<ul class=\"bbc\">");
            postContent = postContent.Replace("<ol>", "<ul class=\"bbcol decimal\">");
            postContent = postContent.Replace("</ol>", "</ul>");
            postContent = BlockQuoteRegex.Replace(postContent,
                "<blockquote class=\"bioQuote\">$1</blockquote>");

            return postContent;
        }

        public async Task<bool> DeleteNewsTopicAsync(News news)
        {
            if (_ipbApiConfig.DevMode || news.ForumTopicId == null)
            {
                return true;
            }
            var topicDeleteResponse = await DoDeleteApiRequestAsync("/forums/topics/" + news.ForumTopicId);
            if (topicDeleteResponse.IsSuccessStatusCode)
            {
                return true;
            }
            throw new Exception($"Can't delete topic: {await topicDeleteResponse.Content.ReadAsStringAsync()}");
        }
    }
}