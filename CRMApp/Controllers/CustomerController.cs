using CRMApp.Common;
using CRMAppEntity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace CRMApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpPost("GetCustomers")]
        public async Task<JsonTemplate<List<Customer>>> GetCustomers([FromBody] Customer customer)
        {
            List<Customer> result;
            using (DBContext ctx = new())
            {

               var exp = from e in ctx.CustomerSet
                         select e;
                customer.Name = customer.Name.Trim();
                customer.Address = customer.Address.Trim();
                if (customer.Name != "" && customer.Name.Length>0)
                   exp = exp.Where(e => e.Name.Contains(customer.Name));
                if (customer.Address != "" && customer.Address.Length > 0)
                   exp = exp.Where(e => e.Address.Contains(customer.Address));

                result = await exp.ToListAsync();
            }

            return new JsonTemplate<List<Customer>> { StatusCode = "200", Msg = "success", Content = result ?? new List<Customer>() };

        }
        [HttpPost("AddCustomer")]
        public async Task<JsonTemplate<string>> AddCustomer([FromBody] Customer customer)
        {
            try
            {
                using (DBContext ctx = new())
                {
                    customer.Guid = Guid.NewGuid().ToString();  
                    await ctx.CustomerSet.AddAsync(customer);
                    await ctx.SaveChangesAsync();
                }
                return new JsonTemplate<string> { StatusCode = "200", Msg = "Add success！", Content = "success" };
            }
            catch (Exception ex)
            {
                return new JsonTemplate<string> { StatusCode = "302", Msg = ex.ToString(), Content = "error" };
            }
        }

        [HttpPut("EditCustomer")]
        public async Task<JsonTemplate<string>> EditCustomer([FromBody] Customer customer)
        {
            try
            {
                using (DBContext ctx = new())
                {

                    Customer? record = await (from e in ctx.CustomerSet
                                   where e.Id==customer.Id
                                   select e).FirstOrDefaultAsync();

                    if(record == null)
                    {
                        return  new JsonTemplate<string> { StatusCode = "303", Msg = "No the data！", Content = "error" };
                    }
                    record.Name = customer.Name;
                    record.Address = customer.Address;
                    await ctx.SaveChangesAsync();
                }
                return new JsonTemplate<string> { StatusCode = "200", Msg = "Update success！", Content = "success" };
            }
            catch (Exception ex)
            {
                return new JsonTemplate<string> { StatusCode = "302", Msg = ex.ToString(), Content = "error" };
            }
        }
        [HttpDelete("DelCustomer/{customerId}")]
        public async Task<JsonTemplate<string>> DelCustomer(string customerId)
        {
            try
            {
                var list = customerId.Split(",");
                using (DBContext ctx = new())
                {

                    List<Customer>? records = await (from e in ctx.CustomerSet
                                              where list.Contains(e.Id.ToString())
                                              select e).ToListAsync<Customer>();

                    ctx.CustomerSet.RemoveRange(records);
                    await ctx.SaveChangesAsync();
                }
                return new JsonTemplate<string> { StatusCode = "200", Msg = "Update success！", Content = "success" };
            }
            catch (Exception ex)
            {
                return new JsonTemplate<string> { StatusCode = "302", Msg = ex.ToString(), Content = "error" };
            }
        }
    }
}
