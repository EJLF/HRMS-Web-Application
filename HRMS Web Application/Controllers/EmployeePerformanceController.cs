using HRMS_Web_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HRMS_Web_Application.Controllers
{
    public class EmployeePerformanceController : Controller
    {
        public static string baseUrl = "http://localhost:5237/api/EmployeePerformance";
        HttpClient client = new HttpClient();
        public void SetupHttpRequestHeaders()
        {
            string accessToken = HttpContext.Session.GetString("JWToken");
            string apiKey = HttpContext.Session.GetString("ApiKey");
            client.DefaultRequestHeaders.Add("ApiKey", apiKey);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<IActionResult> Create(string userID, string employeeName)
        {
            SetupHttpRequestHeaders();
            string userName =HttpContext.Session.GetString("UserName");
            string employee = await client.GetStringAsync("http://localhost:5237/api/ApplicationUser");
            List<ApplicationUser> employees = JsonConvert.DeserializeObject<List<ApplicationUser>>(employee);
            var reviewer = employees.FirstOrDefault(a => a.UserName == userName);
            
            EmployeePerformance employeePerformance = new EmployeePerformance();
            {
                employeePerformance.UserID = userID;
                employeePerformance.DateReview = DateTime.Now.ToString("MM/dd/yyyy");
                employeePerformance.About = null;
                employeePerformance.PerformanceReview = null;
                employeePerformance.EmployeeName = employeeName;
                employeePerformance.reviewerName = reviewer.FullName;
                employeePerformance.ReviewerID = reviewer.Id;
                return View(employeePerformance);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeePerformance newEmployeePerformance)
        {
            try
            {
                SetupHttpRequestHeaders();

                var stringContent = new StringContent(JsonConvert.SerializeObject(newEmployeePerformance), Encoding.UTF8, "application/json");
                var url = string.Format(baseUrl + "?UserID=" + newEmployeePerformance.UserID +
                                                  "&ReviewBy=" + newEmployeePerformance.ReviewerID +
                                                  "&About=" + newEmployeePerformance.About +
                                                  "&PerformanceReview=" + newEmployeePerformance.PerformanceReview +
                                                  "&DateReview=" + newEmployeePerformance.DateReview +
                                                  "&Status=false" +
                                                  "&DeleteStatus=false");

                var response = client.PostAsync(url, stringContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["EmployeePerformanceAlert"] = "Save Successfull!";
                    return RedirectToAction("List");
                }
                TempData["DepartmentAlert"] = "Failed";
                return View();
            }
            catch (Exception ex)
            {
                TempData["EmployeePerformanceAlert"] = "Error, Please Try Again!" + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                SetupHttpRequestHeaders();
                string jsonStr = await client.GetStringAsync(baseUrl);
                var result = JsonConvert.DeserializeObject<List<EmployeePerformance>>(jsonStr).ToList();
                
                return View(result);
            }
            catch (Exception ex)
            {
                TempData["EmployeePerformanceAlert"] = "Error, Please Try Again! " + ex.Message;
                if (ex.Message.Contains("500") || ex.Message.Contains("No connection"))
                {

                    return RedirectToAction("Privacy", "Home");
                }
                return RedirectToAction("Unauthorized", "Home");
            } 
        }

        [HttpGet]
        public async Task<IActionResult> Update(int No)
        {
            try
            {
                SetupHttpRequestHeaders();
                var response = client.GetAsync(baseUrl + "/" + No).Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    EmployeePerformance departmentPosition = JsonConvert.DeserializeObject<EmployeePerformance>(data);
                    return View(departmentPosition);
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["EmployeePerformanceAlert"] = "Error, Please Try Again!" + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(EmployeePerformance newEmployeePerformance, int No)
        {
            try
            {
                SetupHttpRequestHeaders();
                string data = JsonConvert.SerializeObject(newEmployeePerformance);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                //?No=1&UserID=eb873458-a5cc-4247-81b9-3b2094dd0c56&ReviewBy=eb873458-a5cc-4247-81b9-3b2094dd0c56&About=try&PerformanceReview=try&DateReview=try&Status=false&DeleteStatus=false
                var url = string.Format(baseUrl + "?No=" + newEmployeePerformance.No +
                                                  "&UserID=" + newEmployeePerformance.UserID +
                                                  "&ReviewBy=" + newEmployeePerformance.ReviewerID +
                                                  "&About=" + newEmployeePerformance.About +
                                                  "&PerformanceReview=" + newEmployeePerformance.PerformanceReview +
                                                  "&DateReview=" + newEmployeePerformance.DateReview+
                                                  "&Status=false" + 
                                                  "&DeleteStatus=false");
                var response = client.PutAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responsecontent = response.Content.ReadAsStringAsync().Result;
                    DepartmentPosition dp = JsonConvert.DeserializeObject<DepartmentPosition>(responsecontent);
                    TempData["EmployeePerformanceAlert"] = "Update Successfull!";
                    return RedirectToAction("List");
                }
                TempData["EmployeePerformanceAlert"] = "Error, Please Try Again! " + response.StatusCode;
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["EmployeePerformanceAlert"] = "Error, Please Try Again! " + ex.Message;
                return View();
            }
        }

        public IActionResult Delete(int No)
        {
            try
            {
                SetupHttpRequestHeaders();
                var url = string.Format(baseUrl + "?No={0}", No);
                var response = client.DeleteAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["EmployeePerformanceAlert"] = "Delete Successfull!";
                    return RedirectToAction("List");
                }
                TempData["EmployeePerformanceAlert"] = "Error, Please Try Again! Status Code: " + response.StatusCode;
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["EmployeePerformanceAlert"] = "Error, Please Try Again!" + ex.Message;
                return RedirectToAction("List");
            }
        }
    }
}
