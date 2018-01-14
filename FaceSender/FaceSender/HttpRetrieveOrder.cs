using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace FaceSender
{
    public static class HttpRetrieveOrder
    {
        [FunctionName("HttpRetrieveOrder")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req,
            [Table("Orders", Connection = "OrdersTableConn")]CloudTable ordersTable, TraceWriter log)
        {
            string fileName = req.Query["fileName"];
            CloudTableClient tableClient = ordersTable.ServiceClient;
            CloudTable table = tableClient.GetTableReference(ordersTable.Name);
            TableQuery<PhotoOrder> query = new TableQuery<PhotoOrder>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, fileName));
            TableContinuationToken continuationToken = null;
            TableQuerySegment<PhotoOrder> tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
            var resultList = tableQueryResult.Results;

            if (resultList.Count > 0)
            {                
                return new JsonResult(new {
                    resultList[0].CustomerEmail,
                    resultList[0].FileName,
                    resultList[0].RequiredHeight,
                    resultList[0].RequiredWidth
                });
            }

            return new NotFoundResult();
        }
    }
}
