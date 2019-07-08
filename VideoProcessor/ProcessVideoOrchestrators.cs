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

            string transcodedLocation = null;
            string thumbnailLocation = null;
            string withIntroLocation = null;

            var videoLoaction = ctx.GetInput<string>();
            try
            {
                if (!ctx.IsReplaying)
                    log.Info("About to call transcode video activity");

                transcodedLocation = await
                                         ctx.CallActivityAsync<string>("A_TranscodeVideo", videoLoaction);

                if (!ctx.IsReplaying)
                    log.Info("About to call extract thumbnail");

                thumbnailLocation = await
                                        ctx.CallActivityAsync<string>("A_ExtractThumbnail", transcodedLocation);

                if (!ctx.IsReplaying)
                    log.Info("About to call prependIntro");

                withIntroLocation = await
                                        ctx.CallActivityAsync<string>("A_PrependIntro", transcodedLocation);
            }
            catch (Exception e)
            {
                if (!ctx.IsReplaying)
                    log.Info($"Caught an error from an activity: {e.Message}");

                await
                    ctx.CallActivityAsync<string>("A_Cleanup",
                    new [] { transcodedLocation , thumbnailLocation , withIntroLocation }
                    );


                return new
                {
                    Error = "Failed to process upload video",
                    Message = e.Message
                };
            }
            return new
            {
                Transcoded = transcodedLocation,
                Thumbnail = thumbnailLocation,
                WithIntro = withIntroLocation
            };

        }

    }
}
