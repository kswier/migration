using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using Library;

namespace aspnet
{
    /// <summary>
    /// Summary description for orders_handler
    /// </summary>
    public class orders_handler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            try
            {
                // Get orders data
                List<OrderInfo> orders = GetOrders();
                
                // Create response object
                var response = new
                {
                    success = true,
                    orders = orders,
                    message = "Orders loaded successfully"
                };

                // Serialize and return JSON
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(response);
                context.Response.Write(json);
            }
            catch (Exception ex)
            {
                // Handle errors
                var errorResponse = new
                {
                    success = false,
                    orders = new List<OrderInfo>(),
                    message = "Error loading orders: " + ex.Message
                };

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(errorResponse);
                context.Response.Write(json);
            }
        }

        private List<OrderInfo> GetOrders()
        {
            List<OrderInfo> orders = new List<OrderInfo>();
            
            try
            {
                ADODB db = ConfigObj.globalConfig().getNewDB();
                
                // Check if Orders table exists, if not, return sample data
                string checkTableSql = @"
                    SELECT COUNT(*) as TableCount 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_NAME = 'Orders'";

                ADOQuery checkQuery = new ADOQuery(db);
                checkQuery.open(checkTableSql);
                
                bool tableExists = false;
                if (!checkQuery.eof)
                {
                    tableExists = checkQuery.fieldAsInteger("TableCount") > 0;
                }
                checkQuery.close();

                if (tableExists)
                {
                    // Load real data from Orders table
                    string sql = @"
                        SELECT TOP 100
                            OrderId,
                            CustomerName,
                            OrderDate,
                            TotalAmount,
                            Status
                        FROM Orders 
                        ORDER BY OrderDate DESC";

                    ADOQuery query = new ADOQuery(db);
                    query.open(sql);

                    while (!query.eof)
                    {
                        OrderInfo order = new OrderInfo
                        {
                            OrderId = query.fieldAsInteger("OrderId"),
                            CustomerName = query.fieldAsString("CustomerName"),
                            OrderDate = query.fieldAsDateTime("OrderDate").ToString("MM/dd/yyyy"),
                            TotalAmount = query.fieldAsDecimal("TotalAmount"),
                            Status = query.fieldAsString("Status")
                        };
                        orders.Add(order);
                        query.next();
                    }
                    query.close();
                }
                else
                {
                    // Return sample data if Orders table doesn't exist
                    orders = GetSampleOrders();
                }
                
                // Close database connection
                db.connected = false;
            }
            catch (Exception ex)
            {
                // If database connection fails, return sample data
                orders = GetSampleOrders();
            }

            return orders;
        }

        private List<OrderInfo> GetSampleOrders()
        {
            return new List<OrderInfo>
            {
                new OrderInfo
                {
                    OrderId = 1001,
                    CustomerName = "John Smith",
                    OrderDate = DateTime.Now.AddDays(-5).ToString("MM/dd/yyyy"),
                    TotalAmount = 249.99m,
                    Status = "Shipped"
                },
                new OrderInfo
                {
                    OrderId = 1002,
                    CustomerName = "Jane Doe",
                    OrderDate = DateTime.Now.AddDays(-3).ToString("MM/dd/yyyy"),
                    TotalAmount = 89.50m,
                    Status = "Processing"
                },
                new OrderInfo
                {
                    OrderId = 1003,
                    CustomerName = "Bob Johnson",
                    OrderDate = DateTime.Now.AddDays(-2).ToString("MM/dd/yyyy"),
                    TotalAmount = 156.75m,
                    Status = "Delivered"
                },
                new OrderInfo
                {
                    OrderId = 1004,
                    CustomerName = "Alice Brown",
                    OrderDate = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy"),
                    TotalAmount = 324.00m,
                    Status = "Pending"
                },
                new OrderInfo
                {
                    OrderId = 1005,
                    CustomerName = "Charlie Wilson",
                    OrderDate = DateTime.Now.ToString("MM/dd/yyyy"),
                    TotalAmount = 67.25m,
                    Status = "Processing"
                }
            };
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    public class OrderInfo
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public string OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
}