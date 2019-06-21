using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Newtonsoft.Json;
using System.Text;


namespace PillBox
{
    public class PillboxStatus : Controller
    {
        private static readonly JsonParser jsonParser = new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));

        [FunctionName("GetpillboxStatus")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]  HttpRequest req,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function processed a request. Get Pillbox status");
            WebhookRequest request;

            using (var reader = new StreamReader(req.Body))
            {
                request = jsonParser.Parse<WebhookRequest>(reader);
            }

            var pas = request.QueryResult.Parameters;
            var askingName = pas.Fields.ContainsKey("name") && pas.Fields["name"].ToString().Replace('\"', ' ').Trim().Length > 0;
            var askingAddress = pas.Fields.ContainsKey("address") && pas.Fields["address"].ToString().Replace('\"', ' ').Trim().Length > 0;
            var askingBusinessHour = pas.Fields.ContainsKey("business-hours") && pas.Fields["business-hours"].ToString().Replace('\"', ' ').Trim().Length > 0;
            var response = new WebhookResponse();

            string name = "Jeffson Library", address = "1234 Brentwood Lane, Dallas, TX 12345", businessHour = "8:00 am to 8:00 pm";

            StringBuilder sb = new StringBuilder();

            if (askingName)
            {
                sb.Append("The name of library is: " + name + "; ");
            }

            if (askingAddress)
            {
                sb.Append("The Address of library is: " + address + "; ");
            }

            if (askingBusinessHour)
            {
                sb.Append("The Business Hour of library is: " + businessHour + "; ");
            }

            if (sb.Length == 0)
            {
                sb.Append("Greetings from our Webhook API!");
            }

            response.FulfillmentText = sb.ToString();

            

            return response != null
              ? (ActionResult)Json(response)
              : new BadRequestObjectResult("There is no order stats infomration");
            
        }
    }
}
