﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SFA.DAS.RoATPService.Application.Api.Helpers
{
    public class DataTableHelper : IDataTableHelper
    {
        DataTable IDataTableHelper.ToDataTable(IEnumerable<IDictionary<string, object>> list)
        {
            var dataTable = new DataTable();

            if (list != null || list.Any())
            {
                var columnNames = list.SelectMany(dict => dict.Keys).Distinct();
                dataTable.Columns.AddRange(columnNames.Select(col => new DataColumn(col)).ToArray());

                foreach (var item in list)
                {
                    var row = dataTable.NewRow();
                    foreach (var key in item.Keys)
                    {
                        row[key] = item[key];
                    }

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }
    }
}
