using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using cis237Assignment6.Models;
using System.Data;
using System.Net;
using System.Data.Entity;

namespace cis237Assignment6.Controllers
{
    [Authorize]
    public class BeveragesController : Controller
    {
        private BeverageZHoldermanEntities db = new BeverageZHoldermanEntities();
        // GET: Beverages
        public ActionResult Index()
        {
            DbSet<Beverage> beveragesToFilter = db.Beverages;
            string filterName = "";
            string filterMin = "";
            string filterMax = "";

            int min = 0;
            int max = 200;

            //Check to see if there is a value in the session, and if there is, assign it
            //to the variable that we setup to hold the value.
            if (Session["name"] != null && !String.IsNullOrWhiteSpace((string)Session["name"]))
            {
                filterName = (string)Session["make"];
            }

            if (Session["min"] != null && !String.IsNullOrWhiteSpace((string)Session["min"]))
            {
                filterMin = (string)Session["min"];
                min = Int32.Parse(filterMin);
            }

            if (Session["max"] != null && !String.IsNullOrWhiteSpace((string)Session["max"]))
            {
                filterMax = (string)Session["max"];
                max = Int32.Parse(filterMax);
            }

            //Do the filter on the beverageToFilter Dataset. Use the where that we used before
            //when doing the last inclass, only this time send in more lamda expressions to
            //narrow it down further. Since we setup the default values for each of the filter
            //parameters, min, max, and filtername, we can count on this always running with no errors
            IEnumerable<Beverage> filtered = beveragesToFilter.Where(beverage => beverage.price >= min &&
                                                                  beverage.price <= max &&
                                                                  beverage.name.Contains(filterName));

            //Convert the dataset to a list now that the query work is done on it.
            //The view is expecting a list, so we convert the database set to a list.
            IEnumerable<Beverage> finalFiltered = filtered.ToList();

            //Place the string representation of the values that are in the session into
            //the viewbag so that they can be retrieved and displayed on the view
            ViewBag.filterMake = filterName;
            ViewBag.filterMin = filterMin;
            ViewBag.filterMax = filterMax;

            //Return the view with the filtered selection of cars.
            return View(finalFiltered);

            //This is what was originally here
            //return View(db.Beverages.ToList());
        }

        // GET: Beverages/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }
        public ActionResult Create()
        {
            return View();
        }

        // POST: Beverages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id, name, price, pack, active")] Beverage beverage)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Beverages.Add(beverage);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch(DbUpdateException ex)
            {
                return RedirectToAction("ItemExists");
            }
            
            return View(beverage);
        }

        // GET: Cars/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id, name, price, pack, active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: Beverages/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //Mark the method as POST since it is reached from a form submit
        //Make sure to validate the anitForgeryToken too since we included it in the form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter()
        {
            //Get the form data that we sent out of the request object.
            //The string that is used as a key to get the data matches the
            //name property of the form control
            string name = Request.Form.Get("name");
            string min = Request.Form.Get("min");
            string max = Request.Form.Get("max");

            //Now that we have the data pulled out from the request object
            //let's put it into the session so that other methods can have access to it.
            Session["name"] = name;
            Session["min"] = min;
            Session["max"] = max;


            //Redirect to the index page
            return RedirectToAction("Index");
        }
    }
}