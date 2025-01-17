﻿using AspNetCoreHero.ToastNotification.Abstractions;
using ITMCollege.Areas.Client.Controllers;
using ITMCollege.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ITMCollege.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StreamsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyf;

        private readonly string uriStream = "http://localhost:20646/api/streams/";
        private readonly string uri = "http://localhost:20646/api/fields/GetFieldsByStreamId/";
        private HttpClient httpclient = new HttpClient();

        public StreamsController(ILogger<HomeController> logger, INotyfService notyf)
        {
            _logger = logger;
            _notyf = notyf;
        }
        // GET: StreamsController
        public ActionResult Index(int pg = 1)
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var model = JsonConvert.DeserializeObject<IEnumerable<ITMCollege.Models.Stream>>(httpclient.GetStringAsync(uriStream).Result);
            httpclient.Dispose();
            const int pageSize = 10;
            if (pg < 1)
                pg = 1;
            int rescCount = model.Count();
            var pager = new Pager(rescCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = model.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            //return View(model);
            return View(data);
        }

        // GET: StreamsController/Details/5
        public ActionResult Details(int id)
        {
            var model = JsonConvert.DeserializeObject<ITMCollege.Models.Stream>(httpclient.GetStringAsync(uriStream + id).Result);
            httpclient.Dispose();
            return View(model);
        }

        // GET: StreamsController/Create
        public ActionResult Create()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // POST: StreamsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ITMCollege.Models.Stream st)
        {
            try
            {

                var data = httpclient.PostAsJsonAsync<ITMCollege.Models.Stream>(uriStream, st).Result;
                if (data.IsSuccessStatusCode)
                {
                    _notyf.Success("Create Succesfully");
                    httpclient.Dispose();
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }

          
        }

        // GET: StreamsController/Edit/5
        public ActionResult Edit(int id)
        {
            var model = JsonConvert.DeserializeObject<ITMCollege.Models.Stream>(httpclient.GetStringAsync(uriStream + id).Result);
            httpclient.Dispose();
            return View(model);
        }

        // POST: StreamsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StreamId,StreamName")] ITMCollege.Models.Stream stream)
        {
            try
            {
                if (stream != null)
                {
                    _notyf.Success("Edit Succesfully");
                    var model = httpclient.PutAsJsonAsync(uriStream + id, stream).Result;
                    httpclient.Dispose();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _notyf.Success("Edit fail");

                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: StreamsController/Delete/5
        public ActionResult Delete(int id)
        {
            var model = JsonConvert.DeserializeObject<IEnumerable<Field>>(httpclient.GetStringAsync(uri + id).Result);
            if (model.Count() < 1)
            {
                var data = JsonConvert.DeserializeObject<ITMCollege.Models.Stream>(httpclient.GetStringAsync(uriStream + id).Result);
                return View(data);
            }
            else
            {
                _notyf.Warning("Cant delete this record right now");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: StreamsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                
                    _notyf.Success("Delete Succesfully");
                    var data = httpclient.DeleteAsync(uriStream + id).Result;
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
