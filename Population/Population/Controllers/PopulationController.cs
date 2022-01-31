using Microsoft.AspNetCore.Mvc;
using Population.Models;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Population.Controllers
{
    public class PopulationController : Controller
    {

        private readonly IPopulation _iPopulation;
        public PopulationController(IPopulation iPopulation)
        {
            _iPopulation = iPopulation;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoadData()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                // Sort Column Direction ( asc ,desc)
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                // Search Value from (Search box)
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                //Paging Size (10,20,50,100)

                var populationData = _iPopulation.GetSearchData(start, length, sortColumn, sortColumnDirection, searchValue);
                //total number of rows count 
                recordsTotal = populationData.Count();
                //Paging 
                var data = populationData.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data
                return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });

            }
            catch (Exception)
            {
                throw;
            }


        }
      
        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                if (string.IsNullOrEmpty(id.ToString()))
                {
                    return RedirectToAction("Index", "Population");
                }
               
                return View("Edit", _iPopulation.GetPopulationRecord(id));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult Edit(PopulationDto populationDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _iPopulation.SavePopulationRecord(populationDto);
                    return RedirectToAction("Index", "Population");
                }
                return View("Edit", populationDto);
            } 
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                if (id == 0)
                {
                    return RedirectToAction("Index", "Population");
                }

                return Json(data: _iPopulation.DeletePopulationRecord(id));
            }
            catch (Exception)
            {
                throw;
            }
        }
        
    }
}
