using Avengers.MVC.Models.Contributions;
using PSPRS.Avengers.Core.Contracts;
using PSPRS.Avengers.DomainModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Avengers.MVC.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DefaultDatabase"].ConnectionString; }
        }

        protected IEnumerable<SelectListItem> GetEmployerNames(IEmployerService employer)
        {
            List<EmployerNameDM> empList = employer.GetEmployers();

            var result = from eList in empList
                         select new SelectListItem()
                         {
                             Value = eList.ID.ToString(),
                             Text = eList.Name
                         };

            IEnumerable<SelectListItem> vm = result;

            return vm;
        }

        protected IEnumerable<SelectListItem> GetEmploymentStatuses(IEmploymentStatusService employmentStatusService)
        {
            IList<EmploymentStatusLookupDM> statusList = employmentStatusService.GetStatuses();

            var result = from eList in statusList
                         select new SelectListItem()
                         {
                             Value = eList.EmploymentStatusID.ToString(),
                             Text = eList.Status
                         };

            IEnumerable<SelectListItem> vm = result;

            return vm;
        }

        protected List<FixedPositionVM> CreatePropertyColumns(Type[] types)
        {
            List<FixedPositionVM> columns = new List<FixedPositionVM>();

            for (int i = 0; i < types.Count(); i++)
            {
                AddColumns(types[i], columns);
            }

            return columns;
        }

        protected void AddColumns(Type model, List<FixedPositionVM> columns)
        {
            PropertyInfo[] properties = model.GetProperties();

            foreach (PropertyInfo p in properties)
            {
                columns.Add(new FixedPositionVM()
                {
                    ColumnName = p.Name
                });
            }

        }
    }
}