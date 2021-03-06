﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ContosoUniversity.ViewModels;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;

namespace ContosoUniversity.Controllers
{
    public class DepartmentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        public ActionResult ExportClaim(BelianVM bln)
        {
            var dpmntList = bln.DepmtList;
            var products = new System.Data.DataTable("teste");
            products.Columns.Add("Bil", typeof(string));
            products.Columns.Add("Tarikh", typeof(string));
            products.Columns.Add("No. Siri Resit Rasmi", typeof(string));
            products.Columns.Add("Berat Basah (kg)", typeof(string));
            products.Columns.Add("KGK", typeof(string));
            products.Columns.Add("Berat 100% KGK", typeof(string));
            products.Columns.Add("CATATAN", typeof(string));
            
            //dept.Administrator.FullName
            decimal rm = 0;
            decimal kg = 0;
            if (dpmntList == null)
            {
                dpmntList = db.Departments.Include(d => d.Administrator);
            }
            int x = 1;
            foreach (var dept in dpmntList)
            {
                rm = rm + dept.Budget;
                kg = kg + dept.Kg;
                products.Rows.Add(x++,dept.StartDate.Date, dept.ResitRasmi
                    , dept.Kg,"60%" , dept.Budget, dept.Multiplier);
            }
            products.Rows.Add("", "", "", "", "", "", "");
            products.Rows.Add("", "Total :", "", kg, "", rm, "");

            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Claim" + "_" + bln.NamaPenJual + "_" + bln.Month +".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View(bln);
        }

        public ActionResult Export(BelianVM bln)
        {
            var dpmntList = bln.DepmtList;
            var products = new System.Data.DataTable("teste");
            products.Columns.Add("Tarikh", typeof(DateTime));
            products.Columns.Add("Nama Penjual", typeof(string));
            products.Columns.Add("Nombor Lesen/Pat-G", typeof(string));
            products.Columns.Add("Kebenaran Bertulis", typeof(string));
            products.Columns.Add("Resit Rasmi", typeof(string));
            products.Columns.Add("Skrap", typeof(string));
            products.Columns.Add("Lateks", typeof(string));
            products.Columns.Add("Lain-Lain", typeof(string));
            products.Columns.Add("KG", typeof(string));
            products.Columns.Add("Harga Sekilo", typeof(string));
            products.Columns.Add("Jumlah Dibayar", typeof(string));

            if (dpmntList == null) 
            {
                dpmntList = db.Departments.Include(d => d.Administrator);
            }
            foreach (var dept in dpmntList)
            {
                products.Rows.Add(dept.StartDate.Date, dept.Administrator.FullName, dept.Administrator.NomborLesen,
                    dept.KebenaranBertulis, dept.ResitRasmi, dept.Skrap ? "Ya" : "Tidak", dept.Lateks ? "Ya" : "Tidak", dept.Lain ? "Ya" : "Tidak"
                    , dept.Kg, dept.Multiplier, dept.Budget);
            }
            
            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Belian" + "_" + bln.NamaPenJual + "_" + bln.Month + ".xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View(bln);
        }

        // GET: Department
        public ActionResult Index(DateTime? SelectedDT, int? InstructorID, string submitButton)
        {

            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "FullName");

            var viewModel = initBelianVMData(SelectedDT, InstructorID);
            switch (submitButton)
            {
                case "Export":
                    Export(viewModel);
                    break;
                case "ExportClaim":
                    ExportClaim(viewModel);
                    break;
                case "ClaimAll":
                    List<BelianVM> bvmlist = new List<BelianVM>();
                    IEnumerable<Instructor> list = db.Instructors.ToList();
                    foreach (Instructor item in list)
                    {
                        viewModel = initBelianVMData(SelectedDT, item.ID);
                        bvmlist.Add(viewModel);
                    }
                    initExcelFromTemplate(bvmlist);
                    break;
                default:
                    break;
            }

           
            return View(viewModel);
        }

        private BelianVM initBelianVMData(DateTime? SelectedDT, int? InstructorID)
        {
            var viewModel = new BelianVM();
            if (SelectedDT != null && InstructorID != null)
            {
                int month = SelectedDT.Value.Month;
                viewModel.Month = month;
                Instructor Inst = new Instructor();
                Inst = db.Instructors.Where(i => i.ID == InstructorID).Single();
                viewModel.NamaPenJual = Inst.LastName;
                viewModel.DepmtList = getDepartmentByMonthAndUser(month, InstructorID);

            }
            else
            {
                viewModel.Month = 0;
                viewModel.NamaPenJual = "All";
                viewModel.DepmtList = db.Departments.Include(d => d.Administrator);
            }
            return viewModel;
        }

        public IEnumerable<Department> getDepartmentByMonthAndUser (int month, int ? userId)
        {
            return db.Departments.Include(d => d.Administrator).Where(d => d.StartDate.Month == month && d.Administrator.ID == userId);
        }

        public ActionResult Upload(FormCollection formCollection)
        {
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    var instList = new List<Instructor>();
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;

                        for (int rowIterator = 1; rowIterator <= noOfRow; rowIterator++)
                        {
                            var inst = new Instructor();
                            inst.FirstMidName = workSheet.Cells[rowIterator, 1].Value.ToString();
                            inst.LastName = workSheet.Cells[rowIterator, 2].Value.ToString();
                            inst.NomborLesen = workSheet.Cells[rowIterator, 3].Value.ToString();
                            instList.Add(inst);
                            db.Instructors.Add(inst);
                            db.SaveChanges();
                        }
                    }
                    
                }
            }
            return View();
        }

        public void initExcelFromTemplate(List<BelianVM> bvmlist)
        {
            if (!bvmlist.Any())
            {
                return;
            }
            Application excel = new Application();
            excel.Visible = false;
            string path = HttpContext.Server.MapPath("~/Template/instructor.xlsx");
            string newfilename = "Claim_All_" + bvmlist[0].Month + ".xlsx";
            string newpath = HttpContext.Server.MapPath("~/Template/" + newfilename);
            Workbook wb = excel.Workbooks.Open(path);
            for (int rowIterator = 0; rowIterator < bvmlist.Count(); rowIterator++)
            {
                BelianVM bln = bvmlist[rowIterator];

                if (bln != null && bln.DepmtList.Any())
                {
                    int sheetindex = rowIterator + 1;
                    Worksheet sh = null;
                    if (sheetindex == 1)
                    {
                        sh = wb.Sheets.get_Item(sheetindex); 
                    }
                    else
                    {
                        sh = wb.Sheets.Add();
                    }
                    sh.Name = bln.NamaPenJual;
                    sh.Cells[5, "G"].Value = bln.NamaPenJual;
                    int x = 12;
                    foreach (Department item in bln.DepmtList)
                    {
                        int newx = x++;
                        sh.Cells[newx, "B"].Value = item.StartDate;
                        sh.Cells[newx, "C"].Value = "";
                        sh.Cells[newx, "D"].Value = item.Kg;
                        sh.Cells[newx, "E"].Value = "60%";
                        sh.Cells[newx, "F"].Value = item.Multiplier;
                        sh.Cells[newx, "G"].Value = item.Budget;

                    }
                    
                }
            }
            wb.SaveAs(newpath);
            excel.Quit();
        }

        // GET: Department/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Commenting out original code to show how to use a raw SQL query.
            //Department department = await db.Departments.FindAsync(id);

            // Create and execute raw SQL query.
            string query = "SELECT * FROM Department WHERE DepartmentID = @p0";
            Department department = await db.Departments.SqlQuery(query, id).SingleOrDefaultAsync();

            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "FullName");
            return View();
        }

        // POST: Department/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "DepartmentID,KebenaranBertulis,ResitRasmi,Skrap,Lateks,Lain,StartDate,Multiplier,Kg,InstructorID")] Department department)
        {
            if (ModelState.IsValid)
            {
                department.Budget = department.Kg * department.Multiplier;
                db.Departments.Add(department);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        // GET: Department/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "FullName", department.InstructorID);
            return View(department);
        }

        // POST: Department/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, byte[] rowVersion)
        {
            string[] fieldsToBind = new string[] { "KebenaranBertulis", "ResitRasmi", "StartDate", "Skrap", "Lateks", "Lain", "Kg", "Multiplier", "InstructorID", "RowVersion" };

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var departmentToUpdate = await db.Departments.FindAsync(id);
            if (departmentToUpdate == null)
            {
                Department deletedDepartment = new Department();
                TryUpdateModel(deletedDepartment, fieldsToBind);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");
                ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "FullName", deletedDepartment.InstructorID);
                return View(deletedDepartment);
            }

            if (TryUpdateModel(departmentToUpdate, fieldsToBind))
            {
                try
                {
                    departmentToUpdate.Budget = departmentToUpdate.Kg * departmentToUpdate.Multiplier;
                    departmentToUpdate.Lateks = true;
                    db.Entry(departmentToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    db.Entry(departmentToUpdate).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (Department)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The department was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Department)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                            ModelState.AddModelError("Name", "Current value: "
                                + databaseValues.Name);
                        if (databaseValues.Budget != clientValues.Budget)
                            ModelState.AddModelError("Budget", "Current value: "
                                + String.Format("{0:c}", databaseValues.Budget));
                        if (databaseValues.StartDate != clientValues.StartDate)
                            ModelState.AddModelError("StartDate", "Current value: "
                                + String.Format("{0:d}", databaseValues.StartDate));
                        if (databaseValues.InstructorID != clientValues.InstructorID)
                            ModelState.AddModelError("InstructorID", "Current value: "
                                + db.Instructors.Find(databaseValues.InstructorID).FullName);
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List hyperlink.");
                        departmentToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "FullName", departmentToUpdate.InstructorID);
            return View(departmentToUpdate);
        }

        // GET: Department/Delete/5
        public async Task<ActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = await db.Departments.FindAsync(id);
            if (department == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Department department)
        {
            try
            {
                db.Entry(department).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = department.DepartmentID });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");
                return View(department);
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
