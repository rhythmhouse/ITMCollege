﻿using ITMCollege.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ITMCollege.Areas.Client.Controllers
{
    [Area("Client")]
    public class DepartmentsController : Controller
    {
        private readonly string uri = "http://localhost:20646/api/departments/";
        private readonly string uri2 = "http://localhost:20646/api/faculties/";
        private HttpClient httpclient = new HttpClient();
        // GET: DepartmentsController
        public ActionResult Index(int pg=1)
        {
            var model = JsonConvert.DeserializeObject<IEnumerable<Department>>(httpclient.GetStringAsync(uri).Result);
            httpclient.Dispose();
            const int pageSize = 6;
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

        // GET: DepartmentsController/Details/5
        public ActionResult Details(int id)
        {
            var model = JsonConvert.DeserializeObject<Department>(httpclient.GetStringAsync(uri + id).Result);
            var listfacul = JsonConvert.DeserializeObject<IEnumerable<Faculty>>(httpclient.GetStringAsync(uri2).Result);
            ViewBag.listfacul = listfacul.Where(m => m.DepId == id);
            httpclient.Dispose();
            return View(model);
        }
    }
}
