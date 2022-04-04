using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Model.Twitch.EventSub;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Functions
{
    public abstract class TwitchEventSubControllerBase<T1, T2>
    {
        protected readonly ILogger _logger;
        private readonly string _expectedMessageType;
        private const string MESSAGE_TYPE_HEADER_NAME = "Twitch-Eventsub-Message-Type";
        private const string CALLBACK_VERIFICATION_TYPE_NAME = "webhook_callback_verification";

        protected TwitchEventSubControllerBase(
            ILogger<TwitchEventSubControllerBase<T1, T2>> logger,
            string expectedMessageType)
        {
            _logger = logger;
            _expectedMessageType = expectedMessageType;
        }

        protected async Task<T2> HandleRequestAsync(HttpRequest req)
        {
            if(!req.Headers.TryGetValue(MESSAGE_TYPE_HEADER_NAME, out var messageType))
            {
                var message = $"{MESSAGE_TYPE_HEADER_NAME} header not present";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }

            var requestContentString = await req.ReadAsStringAsync();

            if (messageType == CALLBACK_VERIFICATION_TYPE_NAME)
            {
                var requestContentObject = JsonSerializer.Deserialize<EventSubChallenge>(requestContentString);
                var challenge = requestContentObject.Challenge;
                return default(T2);
            }
            else if (messageType == _expectedMessageType)
            {
                _logger.LogDebug($"{this._expectedMessageType} request content:\n{requestContentString}");
                var requestContentObject = JsonSerializer.Deserialize<T1>(requestContentString);
                return await HandleMessageAsync(requestContentObject);
            }
            else
            {
                _logger.LogError($"Invalid Request");
                throw new InvalidOperationException();
            }
        }

        protected abstract Task<T2> HandleMessageAsync(T1 message);
    }
}
