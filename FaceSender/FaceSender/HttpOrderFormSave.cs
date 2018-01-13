
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;

namespace FaceSender
{
    public static class HttpOrderFormSave
    {
        [FunctionName("HttpOrderFormSave")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, 
            [Table("Orders", Connection = "OrdersTableConn")]ICollector<PhotoOrder> ordersTable, TraceWriter log)
        {
            try
            {
                PhotoOrder orderData = null;
                log.Info("PhotoOrder orderData = null");
                try
                {
                    string requestBody = new StreamReader(req.Body).ReadToEnd();
                    orderData = JsonConvert.DeserializeObject<PhotoOrder>(requestBody);
                    log.Info("deser");
                }
                catch (System.Exception)
                {
                    return new BadRequestObjectResult("Received data invalid");
                }

                orderData.PartitionKey = System.DateTime.UtcNow.ToShortDateString();
                orderData.RowKey = orderData.FileName;
                log.Info("assign keys");
                ordersTable.Add(orderData);
                log.Info("add order");
                return (ActionResult)new OkObjectResult($"Order processed");
            }
            catch (System.Exception ex)
            {
                log.Info(ex.Message);
                return new BadRequestObjectResult("Received data invalid");
            }
            
        }
    }

    public class PhotoOrder : TableEntity
    {
        public string CustomerEmail { get; set; }
        public string FileName { get; set; }
        public int RequiredHeight { get; set; }
        public int RequiredWidth { get; set; }
    }
}


