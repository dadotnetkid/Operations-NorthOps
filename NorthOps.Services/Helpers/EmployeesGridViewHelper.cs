using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using NorthOps.Models;

namespace NorthOps.Services.Helpers
{
    public class EmployeesGridViewHelper
    {
        private static GridViewSettings _exportGridViewSettings;

        public static GridViewSettings EmployeesGridViewSettings(bool Export = false)
        {
            var settings = new GridViewSettings();
            settings.Name = "EmployeesGridView";
            settings.CallbackRouteValues = new { Controller = "Employee", Action = "EmployeesGridViewPartial" };

            settings.SettingsEditing.AddNewRowRouteValues = new { Controller = "Employee", Action = "EmployeesGridViewPartialAddNew" };
            settings.SettingsEditing.UpdateRowRouteValues = new { Controller = "Employee", Action = "EmployeesGridViewPartialUpdate" };
            settings.SettingsEditing.DeleteRowRouteValues = new { Controller = "Employee", Action = "EmployeesGridViewPartialDelete" };
            settings.SettingsEditing.Mode = GridViewEditingMode.EditFormAndDisplayRow;
            settings.SettingsBehavior.ConfirmDelete = true;

            settings.CommandColumn.Visible = true;
            settings.CommandColumn.ShowNewButtonInHeader = true;
            settings.CommandColumn.ShowDeleteButton = true;
            settings.CommandColumn.ShowEditButton = true;

            settings.KeyFieldName = "Id";

            settings.SettingsPager.Visible = true;
            settings.Settings.ShowGroupPanel = true;
            settings.Settings.ShowFilterRow = true;
            settings.SettingsBehavior.AllowSelectByRowClick = true;

            settings.SettingsAdaptivity.AdaptivityMode = GridViewAdaptivityMode.HideDataCells;
            settings.SettingsAdaptivity.AdaptiveColumnPosition = GridViewAdaptiveColumnPosition.Right;
            settings.SettingsAdaptivity.AdaptiveDetailColumnCount = 1;
            settings.SettingsAdaptivity.AllowOnlyOneAdaptiveDetailExpanded = true;
            settings.SettingsAdaptivity.HideDataCellsAtWindowInnerWidth = 300;
            //settings.SettingsExport.ExportSelectedRowsOnly = true;
            settings.SettingsExport.FileName = "Employees";

            //settings.CommandColumn.ShowSelectCheckbox = true;
            if (Export == false)
            {
                settings.Columns.Add("FullName");
                settings.Columns.Add("Divisions.DivisionName");
                settings.Columns.Add("Branch.BranchName");
            }
            else
            {
                ExportGridColumn(settings);

            }
            return settings;
        }
        public static GridViewSettings ExportGridViewSettings
        {
            get
            {
                // var columnCollection = new MVCxGridViewColumnCollection();
                // var columns = "FullName;Cellular;BirthDate;Divisions.DivisionName;Departments.DepartmentName;Branch.BranchName;CostCenter;TaxStatus;CivilStatus;Gender;TinNo;SSSNo;HDMFNo;BankAccountNo;";
                if (_exportGridViewSettings == null)
                {
                    _exportGridViewSettings = EmployeesGridViewSettings(true);

                }

                return _exportGridViewSettings;
            }
        }

        public static void ExportGridColumn(GridViewSettings settings)
        {
            settings.Columns.Add("Rfid");
            settings.Columns.Add("BiometricId");
            settings.Columns.Add("FirstName");
            settings.Columns.Add("MiddleName");
            settings.Columns.Add("LastName");
            settings.Columns.Add("AddressLine1");
            settings.Columns.Add("AddressLine2");
            settings.Columns.Add("AddressTownCities.Name").Caption = @"Town/City";
            settings.Columns.Add("AddressTownCities.AddressStateProvinces.Name").Caption = @"Province";
            settings.Columns.Add("Cellular");
            settings.Columns.Add("BirthDate");
            settings.Columns.Add("Divisions.DivisionName");
            settings.Columns.Add("Departments.DepartmentName");
            settings.Columns.Add("Branch.BranchName");

            settings.Columns.Add(col =>
            {
                col.FieldName = "Position";
                col.ColumnType = MVCxGridViewColumnType.ComboBox;
                var cbo = col.PropertiesEdit as ComboBoxProperties;
                cbo.ValueField = "Id";
                cbo.TextField = "Name";
                cbo.ValueType = typeof(Position);
                cbo.DataSource = Enum.GetValues(typeof(Position)).Cast<Position>()
                    .Select(x => new { Id = (int)x, Name = x.ToString() });
            });
            settings.Columns.Add("CostCenter");
            settings.Columns.Add("CivilStatus");
            settings.Columns.Add("Gender");
            settings.Columns.Add("TinNo");
            settings.Columns.Add("SSSNo");
            settings.Columns.Add("PhilHealthNo");
            settings.Columns.Add("HDMFNo");
            settings.Columns.Add("BankAccountNo");
        }
    }
}
