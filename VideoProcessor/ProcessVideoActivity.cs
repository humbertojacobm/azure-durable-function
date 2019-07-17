using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SendGrid.Helpers.Mail;
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
            await Task.Delay(1000);
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

            await Task.Delay(1000);

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

            await Task.Delay(1000);

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

        [FunctionName("A_SendApprovalRequestEmail")]
        public static void SendApprovalRequestEmail(
             [ActivityTrigger] ApprovalInfo approvalInfo,
             [SendGrid(ApiKey = "SendGridKey")] out Mail message,
             [Table("Approvals", "AzureWebJobsStorage")] out Approval approval,
             TraceWriter log)
        {
            var approvalCode = Guid.NewGuid().ToString("N");
            approval = new Approval
            {
                PartitionKey = "Approval",
                RowKey = approvalCode,
                OrchestrationId = approvalInfo.OrchestrationId
            };
            var approverEmail = new Email(ConfigurationManager.AppSettings["ApproverEmail"]);
            var senderEmail = new Email(ConfigurationManager.AppSettings["SenderEmail"]);
            var subject = "A video is awaiting approval";

            log.Info($"Sending approval request for {approvalInfo.VideoLocation}");
            var host = ConfigurationManager.AppSettings["Host"];

            var functionAddress = $"{host}/api/SubmitVideoApproval/{approvalCode}";
            var approvedLink = functionAddress + "?result=Approved";
            var rejectedLink = functionAddress + "?result=Rejected";
            var body = $"Please review {approvalInfo.VideoLocation}<br>"
                               + $"<a href=\"{approvedLink}\">Approve</a><br>"
                               + $"<a href=\"{rejectedLink}\">Reject</a>";
            var content = new Content("text/html", body);
            message = new Mail(senderEmail, subject, approverEmail, content);

            log.Info(body);
        }

        [FunctionName("A_PublishVideo")]
        public static async Task PublishVideo(
            [ActivityTrigger] string inputVideo,
            TraceWriter log
            )
        {
            log.Info($"Publishing {inputVideo}");
            //simulate doing something
            await Task.Delay(1000);
        }

        [FunctionName("A_RejectVideo")]
        public static async Task RejectVideo(
            [ActivityTrigger] string inputVideo,
            TraceWriter log
            )
        {
            log.Info($"Rejecting {inputVideo}");
            //simulate doing something
            await Task.Delay(1000);
        }

        [FunctionName("A_PeriodicActivity")]
        public static void PeriodicActivity(
            [ActivityTrigger] int timesRun,
            TraceWriter log
        )
        {
            log.Warning($"Running the periodic activity, times run = {timesRun}");

        }

    }
}
