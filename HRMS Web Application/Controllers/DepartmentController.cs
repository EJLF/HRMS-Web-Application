using HRMS_Web_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HRMS_Web_Application.Controllers
{
    public class DepartmentController : Controller
    {
        public static string baseUrl = "http://localhost:5237/api/Department";
        HttpClient client = new HttpClient();
        public void SetupHttpRequestHeaders()
        {
            string accessToken = HttpContext.Session.GetString("JWToken");
            string apiKey = HttpContext.Session.GetString("ApiKey");
            client.DefaultRequestHeaders.Add("ApiKey", apiKey);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            try
            {
                SetupHttpRequestHeaders();

                var stringContent = new StringContent(JsonConvert.SerializeObject(department), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(baseUrl + "?newDepartmentName=" + department.DeptName, stringContent);
                if (response.IsSuccessStatusCode)
                {
                    TempData["DepartmentAlert"] = "Save Successfull!";
                    return RedirectToAction("List");
                }     
                TempData["DepartmentAlert"] = "Failed";
                return View();
            }
            catch (Exception ex)
            {
                TempData["DepartmentAlert"] = "Error, Please Try Again!" + ex.Message;
                return View();
            }  
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                var dept = await GetDepartments();
                return View(dept);
            }
            catch (Exception ex)
            {
                TempData["HRMSAlert"] = "Error, Please Try Again!" + ex.Message;
                if (ex.Message.Contains("500"))
                {

                    return RedirectToAction("Privacy", "Home");
                }
                return RedirectToAction("Unauthorized", "Home");
            } 
        }
        [HttpGet]
        public async Task<List<Department>> GetDepartments()
        {
            SetupHttpRequestHeaders();
            string jsonStr = await client.GetStringAsync(baseUrl);
            var result = JsonConvert.DeserializeObject<List<Department>>(jsonStr).ToList();
            return result;
        }
        [HttpGet]
        public async Task<IActionResult> Update(int DeptId)
        {
            try
            {
                SetupHttpRequestHeaders();
                var response = client.GetAsync(baseUrl + "/" + DeptId).Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    Department dept = JsonConvert.DeserializeObject<Department>(data);
                    return View(dept);
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["DepartmentAlert"] = "Error, Please Try Again!" + ex.Message;
                return View();
            } 
        }
        [HttpPost]
        public async Task<IActionResult> Update(Department dept, int DeptId)
        {
            try
            {
                SetupHttpRequestHeaders();
                string data = JsonConvert.SerializeObject(dept);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
           
                var url = string.Format(baseUrl + "?id={0}&newDeptName={1}", DeptId, dept.DeptName);
                var response = client.PutAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responsecontent = response.Content.ReadAsStringAsync().Result;
                    Department todo = JsonConvert.DeserializeObject<Department>(responsecontent);
                    TempData["DepartmentAlert"] = "Update Successfull!";
                    return RedirectToAction("List");
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["DepartmentAlert"] = "Error, Please Try Again!" + ex.Message;
                return View();
            }
        }
        public async Task<IActionResult> DeleteAsync(int DeptId)
        {
            try
            {
                SetupHttpRequestHeaders();
                var url = string.Format(baseUrl + "?id={0}", DeptId);
                var response = client.DeleteAsync(url).Result;
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    TempData["DepartmentAlert"] = "Delete Successfull!";
                    return RedirectToAction("List");
                }
                //The DELETE statement conflicted with the REFERENCE constraint
                if(responseContent.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    TempData["DepartmentAlert"] = "It is not possible to delete this department while there is an Employee or designation assigned to it.";
                    return RedirectToAction("List");
                }
                TempData["DepartmentAlert"] = "Error Please Try Agian! "+ response;
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["DepartmentAlert"] = "Error, Please Try Again!"+ex.Message;
                return RedirectToAction("List");
            }
        }
    }
}
