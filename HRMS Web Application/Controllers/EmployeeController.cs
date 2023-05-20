using HRMS_Web_Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HRMS_Web_Application.Controllers
{
    public class EmployeeController : Controller
    {
        public static string baseUrl = "http://localhost:5237/api/ApplicationUser";
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

        public async Task<IActionResult> List()
        {
            try
            {
                var result = await GetApplicationUser();
                var activeAccounts = result.Where(d=> d.ActiveStatus == true).Where(a => a.DeleteStatus == false).ToList();
                return View(activeAccounts);
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
        public async Task<IActionResult> InactiveList()
        {
            try
            {
                var result = await GetApplicationUser();
                var activeAccounts = result.Where(d => d.ActiveStatus == false).Where(a => a.DeleteStatus == false).ToList();
                return View(activeAccounts);
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
        public async Task<List<ApplicationUser>> GetApplicationUser()
        {
            SetupViewBag();
            string jsonStr = await client.GetStringAsync(baseUrl);
            var result = JsonConvert.DeserializeObject<List<ApplicationUser>>(jsonStr).ToList();

            return result;
        }

        public async Task<IActionResult> Create()
        {
            await SetupViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterEmployeeViewModel registerEmployee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await SetupViewBag();

                    var stringContent = new StringContent(JsonConvert.SerializeObject(registerEmployee), Encoding.UTF8, "application/json");
                    var url = string.Format(baseUrl + "?FirstName=" + registerEmployee.FirstName +
                                                      "&MiddleName=" + registerEmployee.MiddleName +
                                                      "&LastName=" + registerEmployee.LastName +
                                                      "&Gender=" + registerEmployee.Gender +
                                                      "&DateOfBirth=" + registerEmployee.DateOfBirth +
                                                      "&Phone=" + registerEmployee.Phone +
                                                      "&Email=" + registerEmployee.Email +
                                                      "&DepartmenttId=" + registerEmployee.DepartmentId +
                                                      "&PosistionId=" + registerEmployee.PositionId +
                                                      "&EmployeeType=" + registerEmployee.EmployeeType +
                                                      "&Street=" + registerEmployee.Street +
                                                      "&Barangay=" + registerEmployee.Barangay +
                                                      "&City=" + registerEmployee.City +
                                                      "&State=" + registerEmployee.State +
                                                      "&PostalCode=" + registerEmployee.PostalCode +
                                                      "&DateHired=" + registerEmployee.DateHired +
                                                      "&activeStatus=true" +
                                                      "&deleteStatus=false" +
                                                      "&Password=" + registerEmployee.Password);

                    var response = client.PostAsync(url, stringContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["ApplicationUserAlert"] = "Save Successfull!";
                        return RedirectToAction("List");
                    }
                    TempData["ApplicationUserAlert"] = "Failed to Save! " + response.StatusCode;
                    return View();
                }
                else
                {
                    await SetupViewBag();
                    return View(); 
                }
            }
            catch (Exception ex)
            {
                TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string accountId)
        {
            try
            {
                await SetupViewBag();
                //SetupHttpRequestHeaders();
                var response = client.GetAsync(baseUrl + "/" + accountId).Result;
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    EditEmployeeViewModel editEmployee = JsonConvert.DeserializeObject<EditEmployeeViewModel>(data);
                    return View(editEmployee);
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + ex.Message;
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Details(EditEmployeeViewModel editEmployee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SetupHttpRequestHeaders();
                    string data = JsonConvert.SerializeObject(editEmployee);
                    StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                    var url = string.Format(baseUrl + "/" + editEmployee.Id +
                                                      "?FirstName=" + editEmployee.FirstName +
                                                      "&MiddleName=" + editEmployee.MiddleName +
                                                      "&LastName=" + editEmployee.LastName +
                                                      "&Gender=" + editEmployee.Gender +
                                                      "&DateOfBirth=04%2F12%2F3043" +
                                                      "&Phone=" + editEmployee.Phone +
                                                      "&DepartmentId=" + editEmployee.DepartmentId +
                                                      "&PositionId=" + editEmployee.PositionId +
                                                      "&EmployeeType=try" +
                                                      "&Street=" + editEmployee.Street +
                                                      "&Barangay=" + editEmployee.Barangay +
                                                      "&City=" + editEmployee.FirstName +
                                                      "&State=" + editEmployee.City +
                                                      "&PostalCode=" + editEmployee.PostalCode +
                                                      "&DateHired=04%2F12%2F3043" +
                                                      "&activeStatus=true" +
                                                      "&deleteStatus=false");

                    var response = client.PutAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var responsecontent = response.Content.ReadAsStringAsync().Result;
                        TempData["ApplicationUserAlert"] = responsecontent;
                        return RedirectToAction("List");
                    }
                    TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + response.StatusCode;
                    return RedirectToAction("List");
                }
                else
                {
                    TempData["ApplicationUserAlert"] = "Error, Please Try Again! ";
                    await SetupViewBag();
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + ex.Message;
                return View();
            }
        }

        public IActionResult DeleteFromActive(string accountId)
        {
            try
            {
                SetupHttpRequestHeaders();
                string data = JsonConvert.SerializeObject(accountId);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                //?accountId=186c2123-3c75-430a-86ee-7dd52f5763fc&deleteStatus=true
                var url = string.Format(baseUrl + "?accountId={0}&activeStatus=false", accountId);
                var response = client.PutAsync(url, content).Result;
                if(response.IsSuccessStatusCode)
                {
                    var responsecontent = response.Content.ReadAsStringAsync().Result;
                    TempData["ApplicationUserAlert"] = responsecontent;
                    return RedirectToAction("List");
                }
                TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + response.StatusCode;
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + ex.Message;
                return View();
            }
        }
        
        public IActionResult DeleteFormInActive(string accountId)
        {
            try
            {
                SetupHttpRequestHeaders();
                string data = JsonConvert.SerializeObject(accountId);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                //?accountId=186c2123-3c75-430a-86ee-7dd52f5763fc&deleteStatus=true
                var url = string.Format(baseUrl + "?accountId={0}&activeStatus=true", accountId);
                var response = client.PutAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responsecontent = response.Content.ReadAsStringAsync().Result;
                    TempData["ApplicationUserAlert"] = responsecontent;
                    return RedirectToAction("List");
                }
                TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + response.StatusCode;
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + ex.Message;
                return View();
            }
        }

        public IActionResult Delete(string accountId)
        {
            try
            {
                SetupHttpRequestHeaders();
                string data = JsonConvert.SerializeObject(accountId);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                //?accountId=186c2123-3c75-430a-86ee-7dd52f5763fc&deleteStatus=true
                var url = string.Format(baseUrl + "/api/DeleteStatus?accountId={0}", accountId);
                var response = client.PutAsync(url, content).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responsecontent = response.Content.ReadAsStringAsync().Result;
                    TempData["ApplicationUserAlert"] = responsecontent;
                    return RedirectToAction("List");
                }
                TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + response.StatusCode;
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                TempData["ApplicationUserAlert"] = "Error, Please Try Again! " + ex.Message;
                return View();
            }
        }
    }
}
