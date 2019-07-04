using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoProcessor
{
    public static class ProcessVideoOrchestrators
    {
        [FunctionName("O_ProcessVideo")]
        public static async Task<object> ProcessVideo(
            [OrchestrationTrigger] DurableOrchestrationContext ctx,
            TraceWriter log)
        {

            var videoLoaction = ctx.GetInput<string>();

            if (!ctx.IsReplaying)
                log.Info("About to call transcode video activity");

            var transcodedLocation = await
                                     ctx.CallActivityAsync<string>("A_TranscodeVideo", videoLoaction);

            if (!ctx.IsReplaying)
                log.Info("About to call extract thumbnail");

            var thumbnailLocation = await
                                    ctx.CallActivityAsync<string>("A_ExtractThumbnail", transcodedLocation);

            if (!ctx.IsReplaying)
                log.Info("About to call prependIntro");

            var withIntroLocation = await
                                    ctx.CallActivityAsync<string>("A_PrependIntro", transcodedLocation);

            return new
            {
                Transcoded = transcodedLocation,
                Thumbnail = thumbnailLocation,
                WithIntro = withIntroLocation
            };

        }

    }
}
