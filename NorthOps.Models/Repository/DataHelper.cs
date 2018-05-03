using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Web;
using NorthOps.Models;
using NorthOps.Models.Repository;

namespace NorthOps.Models.Repository
{
    public class DataHelper
    {
        static UnitOfWork unitOfWork = new UnitOfWork();
        const string LargeDatabaseDataContextKey = "DXLargeDatabaseDataContext";
        public static northopsEntities DB
        {
            get
            {
                if (HttpContext.Current.Items[LargeDatabaseDataContextKey] == null)
                    HttpContext.Current.Items[LargeDatabaseDataContextKey] = new northopsEntities();
                return (northopsEntities)HttpContext.Current.Items[LargeDatabaseDataContextKey];
            }
        }

        public static IEnumerable<object> GetLocationById(ListEditItemRequestedByValueEventArgs args)
        {
            if (args.Value == null || !int.TryParse(args.Value.ToString(), out int id))
                return null;
            var res = (from m in DB.AddressTownCities
                where m.TownCityId == id
                select new {Id = m.TownCityId, Name = m.Name}).Take(1);

            return res.ToList();
        }

        public static IEnumerable<object> GetLocationRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {

            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;

            var ret = (from address in DB.AddressTownCities
                       where (address.Name).Contains(args.Filter)
                       orderby address.Name
                       select new { Id = address.TownCityId, Name = address.Name }
                ).Skip(skip).Take(take).ToList();
            return ret;
        }
    }
}