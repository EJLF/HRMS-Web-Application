using HRMS_Web_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HRMS_Web_Application.Controllers
{
    public class DepartmentPositionController : Controller
    {
        public static string baseUrl = "http://localhost:5237/api/DepartmentPosition";
        HttpClient client = new HttpClient();
        public void SetupHttpRequestHeaders()
        {
            string accessToken = HttpContext.Session.GetString("JWToken");
            string apiKey = HttpContext.Session.GetString("ApiKey");
            client.DefaultRequestHeaders.Add("ApiKey", apiKey);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task SetupViewBag()
        {
            SetupHttpRequestHeaders();
            string jsonStrDepartment = await client.GetStringAsync("http://localhost:5237/api/Department");
            var result = JsonConvert.DeserializeObject<List<Department>>(jsonStrDepartment).ToList();
            List<SelectListItem> departmentList = result.Select(d => new SelectListItem
            {
                Value = d.DeptId.ToString(),
                Text = d.DeptName
            }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "Select Department"
            };
            departmentList.Insert(0, defItem);

            ViewBag.DepartmentNames = departmentList;

            string jsonStrPosition = await client.GetStringAsync("http://localhost:5237/api/Position");
            var resultpos = JsonConvert.DeserializeObject<List<Position>>(jsonStrPosition).ToList();
            List<SelectListItem> positionList = resultpos.Select(d => new SelectListItem
            {
                Value = d.PosId.ToString(),
                Text = d.positionName
            }).ToList();

            var defItem1 = new SelectListItem()
            {
                Value = "",
                Text = "Select Position"
            };
            positionList.Insert(0, defItem1);

            ViewBag.DepartmentList = departmentList;
            ViewBag.PositionList = positionList;
        }

        public async Task<IActionResult> Create()
        {
            await SetupViewBag();
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentPosition designation)
        {
            try
            {
                SetupHttpRequestHeaders();

                var stringContent = new StringContent(JsonConvert.SerializeObject(designation), Encoding.UTF8, "application/json");
                
                var url = string.Format(baseUrl + "?DepartmentId={0}&PositionId={1}", designation.DepartmentId, designation.PositionId);
                var response = client.PostAsync(url, stringContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["DepartmentPositionAlert"] = "Save Successfull!";
                    return RedirectToAction("List");
                }
                TempData["DepartmentPositionAlert"] = "Failed to Save!" + response.StatusCode;
                return View();
            }
            catch (Exception ex)
            {
                TempData["DepartmentPositionAlert"] = "Error, Please Try Again!" + ex.Message;
                return View();
            }
        }

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
        public async Task<List<DepartmentPosition>> GetDepartments()
        {
            SetupHttpRequestHeaders();
            string jsonStr = await client.GetStringAsync(baseUrl);
            var result = JsonConvert.DeserializeObject<List<DepartmentPosition>>(jsonStr).ToList();
         
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> Update(int No)
        {   
            try
            {
                await SetupViewBag();
                //SetupHttpRequestHeaders();
                var response = client.GetAsync(baseUrl + "/" + No).Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    DepartmentPosition departmentPosition = JsonConvert.DeserializeObject<DepartmentPosition>(data);
                    return View(departmentPosition);
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["DepartmentPositionAlert"] = "Error, Please Try Again!" + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(DepartmentPosition departmentPosition, int No)
        {
            try
            {
                SetupHttpRequestHeaders();
                string data = JsonConvert.SerializeObject(departmentPosition);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                //http://localhost:5237/api/DepartmentPosition?id=3&NewDeptId=1&NewPosId=1
                var url = string.Format(baseUrl + "?id={0}&NewDeptId={1}&NewPosId={2}", No, departmentPosition.DepartmentId, departmentPosition.PositionId);
                var response = client.PutAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responsecontent = response.Content.ReadAsStringAsync().Result;
                    DepartmentPosition dp = JsonConvert.DeserializeObject<DepartmentPosition>(responsecontent);
                    TempData["DepartmentPositionAlert"] = "Update Successfull!";
                    return RedirectToAction("List");
                }
                TempData["DepartmentPositionAlert"] = "Error, Please Try Again!" + response.StatusCode;
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["DepartmentPositionAlert"] = "Error, Please Try Again!" + ex.Message;
                return View();
            }
        }

        public IActionResult Delete(int No)
        {
            try
            {
                SetupHttpRequestHeaders();
                var url = string.Format(baseUrl + "?id={0}", No);
                var response = client.DeleteAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    TempData["DepartmentPositionAlert"] = "Delete Successfull!";
                    return RedirectToAction("List");
                }
                TempData["DepartmentPositionAlert"] = "Error, Please Try Again! Status Code: " + response.StatusCode;
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["DepartmentPositionAlert"] = "Error, Please Try Again!" + ex.Message;
                return RedirectToAction("List");
            }
        }
    }
}
