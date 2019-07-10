using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoProcessor
{
    public static class ProcessVideoActivity
    {
        [FunctionName("A_GetTranscodeBitRates")]
        public static int[] GetTranscodeBitRates(
           [ActivityTrigger]   object input,
           TraceWriter log
        )
        {
            return ConfigurationManager.AppSettings["TranscodeBitRates"]
                .Split(',')
                .Select(int.Parse)
                .ToArray();
        }

        [FunctionName("A_TranscodeVideo")]
        public static async Task<VideoFileInfo> TranscodeVideo(
            [ActivityTrigger] VideoFileInfo inputVideo,
            TraceWriter log
            )
        {
            log.Info($"Transcoding {inputVideo.Location} to {inputVideo.BitRate}");

            //simulate doing the activity
            await Task.Delay(5000);
            var transcodedLocation = $"{Path.GetFileNameWithoutExtension(inputVideo.Location)}+" +
                $"{inputVideo.BitRate}kbps.mp4";

            return new VideoFileInfo
            {
                Location = transcodedLocation,
                BitRate = inputVideo.BitRate
            };
        }

        [FunctionName("A_ExtractThumbnail")]
        public static async Task<string> ExtractThumbnail(
            [ActivityTrigger] string inputVideo,
            TraceWriter log
            )
        {
            log.Info($"Extracting thumbnail {inputVideo}");

            if (inputVideo.Contains("error"))
            {
                throw new InvalidOperationException("Could not extract thumbnail");
            }

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
        [FunctionName("A_Cleanup")]
        public static async Task<string> cleanup(
            [ActivityTrigger] string [] filesToCleanUp,
            TraceWriter log)       
        {
            foreach(var file in filesToCleanUp.Where(f => f != null))
            {
                log.Info($"Deleting {file}");
                //simulate doing something
                await Task.Delay(1000);
            }
            return "Cleaned up successfully";
        }

    }
}
