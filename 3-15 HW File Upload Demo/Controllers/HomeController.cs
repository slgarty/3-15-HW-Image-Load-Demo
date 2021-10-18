using _3_15_HW_File_Upload_Demo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _3_15_HW_File_Upload_Demo.Controllers
{
    public class HomeController : Controller
    {

        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Images;Integrated Security=true;";


        private readonly IWebHostEnvironment _environment;

        public HomeController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        [HttpPost]
        public IActionResult Upload(IFormFile myfile, string password)
        {
            Guid guid = Guid.NewGuid();
            string actualFileName = $"{guid}-{myfile.FileName}";
            string finalFileName = Path.Combine(_environment.WebRootPath, "uploads", actualFileName);
            using var fs = new FileStream(finalFileName, FileMode.CreateNew);
            myfile.CopyTo(fs);
            var db = new ImageDb(_connectionString);
            db.AddImage(actualFileName, password );
            var vm = new ViewImagesViewModel();
            vm.Image = db.GetImages().First(i=> i.ImageName== actualFileName);

            return View(vm);

        }
        public IActionResult Index()
        {
            return View();
        }
        private bool HasPermissionToView(int id)
        {
            var approvedIds = HttpContext.Session.Get<List<int>>("approvedids");
            if (approvedIds == null)
            {
                return false;
            }

            return approvedIds.Contains(id);
        }
        public IActionResult PreView(int id)
        {
            var db = new ImageDb(_connectionString);
            var vm = new ViewImagesViewModel();
            vm.Image = db.GetImageById(id);
            List<int> ids = HttpContext.Session.Get<List<int>>("ApprovedIds");
            if (ids != null && ids.Contains(id))
            {
                return Redirect($"/home/viewImage?id={id}]");
            }
           
            return View(vm);
        }
        [HttpPost]
        public IActionResult ViewImage(int id, string password)
        {
            var db = new ImageDb(_connectionString);
            var vm = new ViewImagesViewModel();
            var image = db.GetImageById(id);
            if (password != image.Password)
            {
                TempData["message"] = "Please reenter your password";
                return Redirect($"/Home/PreView?id={id}");
            }

            db.AddView(id, (image.Views + 1));
            vm.Image = image;
            List<int> ids = HttpContext.Session.Get<List<int>>("ApprovedIds");
            if (ids == null)
            {
                ids = new List<int>();
            }
            if (!ids.Contains(image.Id))
            {
                ids.Add(image.Id);
            }
            HttpContext.Session.Set("ApprovedIds", ids);
           

            return View(vm);

        }
    }
}

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonConvert.SerializeObject(value));
    }

    public static T Get<T>(this ISession session, string key)
    {
        string value = session.GetString(key);
        return value == null ? default(T) :
        JsonConvert.DeserializeObject<T>(value);
    }
}


