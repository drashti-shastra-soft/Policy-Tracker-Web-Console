using System.Collections.Generic;
using System.Web.Mvc;

namespace PolicyTrackerWebClient.Infrastructure.IService
{
    public interface IDropdownService
    {
        List<SelectListItem> GetStatusOptions();

        List<SelectListItem> GetPageSizeOptions(int selectedSize = 20);

        List<SelectListItem> GetBrokerOptions(int? selected = null);

        List<SelectListItem> GetInsuranceCompanyOptions(int? selected = null);

        List<SelectListItem> GetCategoryOptions(int? selected = null);

        List<SelectListItem> GetRoleOptions(int? selected = null);

        List<SelectListItem> GetStatementMonthOptions(int? brokerId, string selected = null);

        /// <summary>Options from an enum: Quote_Sent -> "Quote Sent".</summary>
        List<SelectListItem> GetEnumOptions<TEnum>(string selected = null) where TEnum : struct;
    }
}
