using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Raw.Streaming.Webhook.Functions
{
    public static class WebhookConfirmController
    {
        [FunctionName("WebHookConfirm")]
        public static IActionResult WebHookConfirm(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "webhook/{provider}/{topic}")] HttpRequest req,
            ILogger log,
            string provider,
            string topic)
        {
            if(req.Query.ContainsKey("hub.challenge"))
            {
                log.LogInformation($"Subscription to {provider} topic {topic} verified");
                var challengeString = req.Query["hub.challenge"];
                return new OkObjectResult(challengeString[0]);
            }
            else if (req.Query.ContainsKey("hub.reason"))
            {
                var reason = req.Query["hub.reason"];
                log.LogError($"Subscription to {provider} topic {topic} denied: {reason}");
                return new BadRequestResult();
            }
            else
            {
                log.LogError($"Invalid Request");
                return new BadRequestResult();
            }
        }
    }
}
