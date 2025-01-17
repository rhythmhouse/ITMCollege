﻿using AspNetCoreHero.ToastNotification.Abstractions;
using ITMCollege.Areas.Client.Controllers;
using ITMCollege.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ITMCollege.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoursesController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyf;

        private readonly string uriCourse = "http://localhost:20646/api/courses/";
        private readonly string uriField = "http://localhost:20646/api/fields/";
        private readonly string uriStream = "http://localhost:20646/api/streams/";
        private readonly string uri = "http://localhost:20646/api/fields/GetFieldsByStreamId/";
        private HttpClient httpclient = new HttpClient();

        public CoursesController(ILogger<HomeController> logger, INotyfService notyf)
        {
            _logger = logger;
            _notyf = notyf;
        }
        // GET: CoursesController
        public ActionResult Index( int searchStream, int searchField, int page)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToAction("Login", "Home");
            }
           
            ViewBag.searchStream = searchStream;
            ViewBag.searchField = searchField;
            List<SelectListItem> streamList = new List<SelectListItem>();
            var streams = JsonConvert.DeserializeObject<IEnumerable<ITMCollege.Models.Stream>>(httpclient.GetStringAsync(uriStream).Result);
            streamList.Add(new SelectListItem { Text = "---Choose Stream---", Value = "0" });
            foreach (var item in streams)
            {
                streamList.Add(new SelectListItem { Text = $"{item.StreamName}", Value = $"{item.StreamId}" });
            }
            foreach (var item in streamList)
            {
                item.Selected = item.Value.Equals(searchStream.ToString()) ? true : false;
            }
            ViewBag.StreamList = streamList;
            
            var list = JsonConvert.DeserializeObject<IEnumerable<Course>>(httpclient.GetStringAsync(uriCourse).Result);
            foreach (var item in list)
            {
                item.Field = JsonConvert.DeserializeObject<Field>(httpclient.GetStringAsync(uriField + item.FieldId).Result);
            }
           
            if (searchStream != 0)
            {
                list = list.Where(a => a.StreamId == searchStream);
            }
            if (searchField != 0)
            {
                list = list.Where(a => a.FieldId == searchField);
            }
        

            const int pageSize = 5;
            page = page > 1 ? page : 1;
            int resCount = list.Count();
            var pager = new Pager(resCount, page, pageSize);
            int recSkip = (page - 1) * pageSize;
            var data = list.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            ViewBag.TotalPage = (int)resCount / pageSize + 1;
            return View(data);

        }

        
      
        public ActionResult Details(int id)
        {
            var model = JsonConvert.DeserializeObject<Course>(httpclient.GetStringAsync(uriCourse + id).Result);
            httpclient.Dispose();
            return View(model);
        }

        public JsonResult getfieldsFromDatabaseByStream(int id)
        {
            var model = JsonConvert.DeserializeObject<IEnumerable<Field>>(httpclient.GetStringAsync(uri + id).Result);
            httpclient.Dispose();
            return Json(new SelectList(model,"FieldId","FieldName"));
        }

        // GET: CoursesController/Create
        public ActionResult Create()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var data = JsonConvert.DeserializeObject<IEnumerable<ITMCollege.Models.Stream>>(httpclient.GetStringAsync(uriStream).Result);
            ViewBag.ListStream = new SelectList(data, "StreamId", "StreamName");
            httpclient.Dispose();
            return View();
        }

        // POST: CoursesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,CourseName,Description,FieldId,StreamId,Image")] Course course, IFormFile file)
        {
            try
            {
                if (file != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string file_path = Path.Combine
                        (Directory.GetCurrentDirectory(), @"wwwroot/Images/Course", fileName);
                    using (var stream = new FileStream(file_path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    course.Image = "Images/Course/" + fileName;
                }
                else
                {
                    _notyf.Warning("Image file invalid");
                    return RedirectToAction(nameof(Create));
                }
                var data = httpclient.PostAsJsonAsync<Course>(uriCourse, course).Result;
                if (data.IsSuccessStatusCode)
                {
                    _notyf.Success("Create Succesfully");
                    httpclient.Dispose();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _notyf.Warning("Create fail!! FieldId Invalid");
                    httpclient.Dispose();
                    return RedirectToAction(nameof(Create));
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: CoursesController/Edit/5
        public ActionResult Edit(int id)
        {

            var data = JsonConvert.DeserializeObject<IEnumerable<ITMCollege.Models.Stream>>(httpclient.GetStringAsync(uriStream).Result);
            ViewBag.ListStream = new SelectList(data, "StreamId", "StreamName");
            var model = JsonConvert.DeserializeObject<Course>(httpclient.GetStringAsync(uriCourse + id).Result);
            httpclient.Dispose();
            return View(model);
        }

        // POST: CoursesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,CourseName,Description,FieldId,StreamId,Image")] Course course, IFormFile file)
        {
            try
            {
                if (file != null)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    string file_path = Path.Combine
                        (Directory.GetCurrentDirectory(), @"wwwroot/Images/Course", fileName);
                    using (var stream = new FileStream(file_path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    course.Image = "Images/Course/" + fileName;
                    _notyf.Success("Edit Succesfully");
                    var model = httpclient.PutAsJsonAsync(uriCourse + id, course).Result;
                    httpclient.Dispose();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _notyf.Success("Edit Succesfully");
                    var model = httpclient.PutAsJsonAsync(uriCourse + id, course).Result;
                    httpclient.Dispose();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: CoursesController/Delete/5
        public ActionResult Delete(int id)
        {
            var data = JsonConvert.DeserializeObject<Course>(httpclient.GetStringAsync(uriCourse + id).Result);
            return View(data);

        }

        // POST: CoursesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                _notyf.Success("Delete Succesfully");
                var data = httpclient.DeleteAsync(uriCourse + id).Result;
                httpclient.Dispose();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


      
    }
}
