using ProjectsNow.Data;

using System.Collections.Generic;

namespace ProjectsNow.Controllers
{
    public static class FilterController
    {
        public static string PropertyFilter(Filter filter, Group group, List<List<string>> values)
        {
            string filterCondition = $"Select {filter.PropertyName} From [Reference].[ItemsPropertiesView] Where GroupID = {filter.GroupID}";

            for (int i = 1; i <= 9; i++)
            {
                if (group.GetType().GetProperty($"Property{i}").GetValue(group) != null)
                {
                    for (int ii = 1; ii <= 6; ii++)
                    {
                        if (filter.PropertyName != $"Property{i}{ii}")
                        {
                            if (values[i][ii] != null)
                            {
                                if ((bool)filter.GetType().GetProperty($"Property{i}{ii}").GetValue(filter) != false)
                                {
                                    filterCondition += $" And (Property{i}{ii} Like '%{values[i][ii]}%')";
                                }
                            }
                        }
                    }
                }
            }

            filterCondition += $" And (({filter.PropertyName}) Not Like '%,%') AND (({filter.PropertyName}) IS Not Null) Group By {filter.PropertyName} ";
            filterCondition += $", (Case When Isnumeric({filter.PropertyName}) = 1 then 0 else 1 end) Order By (Case When Isnumeric({filter.PropertyName}) = 1 then 0 else 1 end) , LEN({filter.PropertyName})"; //ORDER BY IsNum, LEN(ID), ID;

            return filterCondition;
        }


        public static string ItemFilter(Filter filter, Group group, List<List<string>> values)
        {
            string filterCondition = $"Select * From [Reference].[ItemsPropertiesView] Where (GroupID = {filter.GroupID}) " +
                                     $"And (Items = '{group.GetType().GetProperty(filter.PropertyName).GetValue(group)}') ";

            for (int i = 1; i <= 9; i++)
            {
                for (int ii = 1; ii <= 6; ii++)
                {
                    if (values[i][ii] != null)
                    {
                        if ((bool)filter.GetType().GetProperty($"Property{i}{ii}").GetValue(filter) != false)
                        {
                            filterCondition += $" And (Property{i}{ii} Like '%{values[i][ii]}%')";
                        }
                    }
                }
            }

            return filterCondition;
        }
    }
}
