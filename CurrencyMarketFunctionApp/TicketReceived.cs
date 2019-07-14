using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace CurrencyMarketFunctionApp
{
    public static class TicketReceived
    {
        [FunctionName("TicketReceived")] 
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, 
            [Queue("Tickets")] IAsyncCollector<TicketMessage> ticketQueue,TraceWriter log)
        {
            log.Info("Message received.");

            try
            {
                var content = await req.Content.ReadAsByteArrayAsync();
                string requestBody = await new StreamReader(new MemoryStream(content)).ReadToEndAsync();

                var ticket = JsonConvert.DeserializeObject<TicketMessage>(requestBody);
                await ticketQueue.AddAsync(ticket);
                log.Info(string.Format("Ticket receieved.Message Id = {0}", ticket.MessageId));
                return req.CreateResponse(HttpStatusCode.OK, "We received your request. Tracking Id = " + Guid.NewGuid().ToString());
            }
            catch (System.Exception ex)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError, "Request was not process");
            }
        }
    }
}
