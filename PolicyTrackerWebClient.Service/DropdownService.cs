using PolicyTrackerWebClient.Infrastructure;
using PolicyTrackerWebClient.Infrastructure.IService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Service
{
    public class DropdownService : ApiServiceBase, IDropdownService
    {
        private readonly IHttpClientService _httpClientService;

        public DropdownService(IHttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public List<SelectListItem> GetStatusOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Active", Value = "Active" },
                new SelectListItem { Text = "Disabled", Value = "Disabled" }
            };
        }

        public List<SelectListItem> GetPageSizeOptions(int selectedSize = 20)
        {
            var list = new List<SelectListItem>
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "20", Value = "20" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "All", Value = "-1" }
            };

            list.ForEach(item => item.Selected = item.Value == selectedSize.ToString());
            return list;
        }

        public List<SelectListItem> GetBrokerOptions(int? selected = null)
        {
            var items = Get<List<Domain.Broker>>("Broker/GetActive");
            return items.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == selected }).ToList();
        }

        public List<SelectListItem> GetInsuranceCompanyOptions(int? selected = null)
        {
            var items = Get<List<Domain.InsuranceCompany>>("InsuranceCompany/GetActive");
            return items.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == selected }).ToList();
        }

        public List<SelectListItem> GetCategoryOptions(int? selected = null)
        {
            var items = Get<List<Domain.Category>>("Category/GetActive");
            return items.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == selected }).ToList();
        }

        public List<SelectListItem> GetRoleOptions(int? selected = null)
        {
            var items = Get<List<Domain.Role>>("User/Roles");
            return items.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString(), Selected = x.Id == selected }).ToList();
        }

        public List<SelectListItem> GetStatementMonthOptions(int? brokerId, string selected = null)
        {
            var months = Get<List<DateTime>>("Statement/Months?brokerId=" + brokerId);
            return months.OrderByDescending(m => m)
                         .Select(m => new SelectListItem
                         {
                             Text = m.ToString("MMM-yyyy", CultureInfo.InvariantCulture),
                             Value = m.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                             Selected = m.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) == selected
                         }).ToList();
        }

        public List<SelectListItem> GetEnumOptions<TEnum>(string selected = null) where TEnum : struct
        {
            return EnumsHelper.GetTexts<TEnum>()
                              .Select(t => new SelectListItem { Text = t, Value = t, Selected = string.Equals(t, selected, StringComparison.OrdinalIgnoreCase) })
                              .ToList();
        }

        private T Get<T>(string path) where T : new()
        {
            var result = _httpClientService.GetAsync<T>($"{_baseUrl}/{path}", Token).GetAwaiter().GetResult();
            return result == null ? new T() : result;
        }
    }
}
