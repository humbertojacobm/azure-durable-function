using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace VideoProcessor
{
    public static class ProcessVideoStarter
    {
        [FunctionName("ProcessVideoStarter")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
               HttpRequestMessage req,
            [OrchestrationClient] DurableOrchestrationClient starter,
            TraceWriter log)
        {
            // parse query parameter
            string video = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "video", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            video = video ?? data?.video;

            if (video == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest,
                   "Please pass the video location the query string or in the request body");
            }

            log.Info($"About to start orchestration for {video}");

            var orchestrationId = await starter.StartNewAsync("O_ProcessVideo", video);

            return starter.CreateCheckStatusResponse(req, orchestrationId);
        }
    }
}
