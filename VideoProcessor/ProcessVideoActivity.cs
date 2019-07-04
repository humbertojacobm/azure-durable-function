using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoProcessor
{
    public static class ProcessVideoActivity
    {
        [FunctionName("A_TranscodeVideo")]
        public static async Task<string> TranscodeVideo(
            [ActivityTrigger] string inputVideo,
            TraceWriter log
            )
        {
            log.Info($"Transcoding {inputVideo}");

            //simulate doing the activity

            await Task.Delay(5000);

            return "transcoded.mp4";
        }

        [FunctionName("A_ExtractThumbnail")]
        public static async Task<string> ExtractThumbnail(
            [ActivityTrigger] string inputVideo,
            TraceWriter log
            )
        {
            log.Info($"Extracting thumbnail {inputVideo}");

            //simulate doing the activity

            await Task.Delay(5000);

            return "thumbnail.mp4";
        }

        [FunctionName("A_PrependIntro")]
        public static async Task<string> PrependIntro(
            [ActivityTrigger] string inputVideo,
            TraceWriter log
            )
        {
            log.Info($"Appending intro to video {inputVideo}");
            var introLocation = ConfigurationManager.AppSettings["IntroLocation"];

            //simulate doing the activity

            await Task.Delay(5000);

            return "withIntro.mp4";
        }
    }
}
