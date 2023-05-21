using HRMS_Web_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HRMS_Web_Application.Controllers
{
    public class DashboardController : Controller
    {
        HttpClient client = new HttpClient();
        public void SetupHttpRequestHeaders()
        {
            string accessToken = HttpContext.Session.GetString("JWToken");
            string apiKey = HttpContext.Session.GetString("ApiKey");
            client.DefaultRequestHeaders.Add("ApiKey", apiKey);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        public async Task<IActionResult> IndexAsync()
        {
            try
            {
                SetupHttpRequestHeaders();
                string department = await client.GetStringAsync("http://localhost:5237/api/Department");
                List<Department> departments = JsonConvert.DeserializeObject<List<Department>>(department);

                string position = await client.GetStringAsync("http://localhost:5237/api/Position");
                List<Position> positions = JsonConvert.DeserializeObject<List<Position>>(position);

                string employee = await client.GetStringAsync("http://localhost:5237/api/ApplicationUser");
                List<ApplicationUser> employees = JsonConvert.DeserializeObject<List<ApplicationUser>>(employee);

                string employeePerformance = await client.GetStringAsync("http://localhost:5237/api/Department");
                //List<EmployeePerformance> employeePerformances = JsonConvert.DeserializeObject<List<employeePerformance>>(employeePerformance);

                
                ViewBag.Employees = employees.Where(a => a.DeleteStatus == false).Count()-1;
                ViewBag.Departments = departments.Count;
                ViewBag.Positions = positions.Count();
                ViewBag.EmployeeInActive = employees.Where(a => a.ActiveStatus == false).Where(a => a.DeleteStatus == false);//employees;
                ViewBag.EmployeePerformance = ""; //_employeePerformance.ListOfEmployeePerformance(null).Where(e => e.Status == true);
                return View();
            }
            catch (Exception ex)
            {
                TempData["HRMSAlert"] = "Error, Please Try Again! " + ex.Message;
                if (ex.Message.Contains("500") || ex.Message.Contains("No connection"))
                {

                    return RedirectToAction("Privacy", "Home");
                }

                return RedirectToAction("Unauthorized", "Home");
            }
        }
    }
}
