using System;
using System.Windows;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Windows.Controls;
// using Microsoft.Office.Interop.Excel;
// using Application = Microsoft.Office.Interop.Excel.Application;
using DataTable = System.Data.DataTable;
using Window = System.Windows.Window;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Win32;


namespace DataBase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string QueryGetMalesWithAge =
            "SELECT *, DateDiff(\"yyyy\", Datarogd, Date()) AS [Возраст]  FROM Students WHERE Students.Pol = 'М' AND Familia LIKE ?";

        private const string BaseQuery = "SELECT * FROM Students WHERE Familia LIKE ?";

        private const string GroupQuery =
            "SELECT Familia + \" \" + Left([Imya], 1) + \". \" + Left([Otchestvo], 1) + \".\" AS [Фамилия и инициалы] FROM Students WHERE Familia LIKE ? AND №gr =  @group";

        private const string YearQuery =
            "SELECT Familia, [№gr]  FROM Students WHERE Year([Datarogd]) = @year AND Familia LIKE ?";

        private const string OutOfTownQuery =
            "SELECT Familia, [№gr] FROM Students WHERE Gorod <> \"\" AND Familia LIKE ?";

        private string _queryNow;

        private const string ConnectionString =
            @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\user\Desktop\labs\04.01\db_lab1\DataBase\db\college.mdb;Persist Security Info=False";

        private int _year;
        private OleDbConnection connection = new(ConnectionString);
        private Role _role;
        private DataTable _dataTable;
        private OleDbDataAdapter _adapter;
        private int _userId;

        public MainWindow()
        {
            InitializeComponent();
            ReloadTablebyCommand(Query: BaseQuery);
        }

        public MainWindow(int userId, int role) : this()
        {
            _role = Constants.RoleByInt[role];
            _userId = userId;
            if (_role == Role.User)
            {
                data.IsReadOnly = true;
                Reform1Btn.IsEnabled = false;
                Reform2Btn.IsEnabled = false;
            }
        }

        private void OpenConnection()
        {
            try
            {
                connection.Open();
            }
            catch (Exception _)
            {
                MessageBox.Show("База данных не найдена или что-то пошло не так.");
                Close();
            }
        }

        private void ReloadTable(OleDbCommand command)
        {
            OpenConnection();
            _dataTable = new DataTable();
            _adapter = new OleDbDataAdapter(command);
            connection.Close();
            _adapter.Fill(_dataTable);
            data.ItemsSource = _dataTable.DefaultView;
        }


        public void ReloadTablebyCommand(string? Query = null, string Familia = "%")
        {
            _queryNow = Query ?? _queryNow;
            var command = new OleDbCommand(Query ?? _queryNow, connection);
            command.Parameters.Add("?", OleDbType.VarChar, 80).Value = $"%{Familia.ToLowerInvariant()}%";
            ReloadTable(command);
        }

        public void ReloadTablebyCommand(string groupName, string? Query = null, string Familia = "%")
        {
            _queryNow = Query ?? _queryNow;
            var command = new OleDbCommand(_queryNow, connection);
            command.Parameters.Add("?", OleDbType.VarChar, 80).Value = $"%{Familia.ToLowerInvariant()}%";
            command.Parameters.Add("@group", OleDbType.VarChar, 6).Value = groupName;
            ReloadTable(command);
        }

        public void ReloadTablebyCommand(int year, string? Query = null, string Familia = "%")
        {
            _queryNow = Query ?? _queryNow;
            var command = new OleDbCommand(_queryNow, connection);
            command.Parameters.AddWithValue("@year", year);
            command.Parameters.Add("?", OleDbType.VarChar, 80).Value = $"%{Familia.ToLowerInvariant()}%";

            ReloadTable(command);
        }


        private void Search(object sender, RoutedEventArgs e)
        {
            if (Group.Visibility == Visibility.Visible)
                ReloadTablebyCommand(groupName: Group.Text, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
            else if (Year.Visibility == Visibility.Visible && int.TryParse(Year.Text, out _year))
                ReloadTablebyCommand(_year, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
            else
                ReloadTablebyCommand(Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            ReloadTablebyCommand(Query: QueryGetMalesWithAge, Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }


        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ReloadTablebyCommand(Query: BaseQuery, Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void SearchByGroup(object sender, RoutedEventArgs e)
        {
            ReloadTablebyCommand(Group.Text, GroupQuery, Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void GroupSearch_OnChecked(object sender, RoutedEventArgs e)
        {
            GroupText.Visibility = Visibility.Visible;
            Group.Visibility = Visibility.Visible;
            GroupButton.Visibility = Visibility.Visible;
            HideCheckBoxes();
            ReloadTablebyCommand(Group.Text, GroupQuery, Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void GroupSearch_OnUnchecked(object sender, RoutedEventArgs e)
        {
            GroupText.Visibility = Visibility.Collapsed;
            Group.Visibility = Visibility.Collapsed;
            GroupButton.Visibility = Visibility.Collapsed;
            ShowCheckBoxes();
            ReloadTablebyCommand(BaseQuery, Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        void HideCheckBoxes()
        {
            YoungMen.IsChecked = false;
            YoungMen.Visibility = Visibility.Collapsed;
            OutOfTown.IsChecked = false;
            OutOfTown.Visibility = Visibility.Collapsed;
        }

        void ShowCheckBoxes()
        {
            YoungMen.Visibility = Visibility.Visible;
            OutOfTown.Visibility = Visibility.Visible;
        }

        private void YearSearch_OnChecked(object sender, RoutedEventArgs e)
        {
            HideCheckBoxes();
            GroupSearch.IsChecked = false;
            GroupSearch.IsEnabled = false;
            YearText.Visibility = Visibility.Visible;
            Year.Visibility = Visibility.Visible;
            YearButton.Visibility = Visibility.Visible;
            ReloadTablebyCommand(0, YearQuery, Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void YearButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(Year.Text, out _year))
                ReloadTablebyCommand(_year, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void YearSearch_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ShowCheckBoxes();
            GroupSearch.IsEnabled = true;
            YearText.Visibility = Visibility.Collapsed;
            Year.Visibility = Visibility.Collapsed;
            YearButton.Visibility = Visibility.Collapsed;
            ReloadTablebyCommand(Query: BaseQuery, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void OutOfTown_OnChecked(object sender, RoutedEventArgs e)
        {
            ReloadTablebyCommand(Query: OutOfTownQuery, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void OutOfTown_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ReloadTablebyCommand(Query: BaseQuery, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void Reform1(object sender, RoutedEventArgs e)
        {
            OpenConnection();
            new OleDbCommand("UPDATE Students SET Gorod = \'г. Колпино\', Budget = 1 WHERE POL = \'М\' AND Budget = 0",
                connection).ExecuteNonQuery();
            connection.Close();
            ReloadTablebyCommand(Query: BaseQuery, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void Reform2(object sender, RoutedEventArgs e)
        {
            OpenConnection();
            new OleDbCommand("DELETE FROM Students WHERE Budget = 0", connection).ExecuteNonQuery();
            connection.Close();
            ReloadTablebyCommand(Query: BaseQuery, Familia: Familia.Text.Trim() == "" ? "%" : Familia.Text);
        }

        private void Data_OnCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
        {
            // MessageBox.Show($"{e.Column.Header} = {(e.EditingElement)}");
            // new OleDbCommand($"UPDATE Students SET {e.Column.Header} = {e.EditingElement} ")
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            new AuthWindow().Show();
            Close();
        }

        /* Функция взята из документации и примеров использования этой библиотеки:
         https://learn.microsoft.com/en-us/office/open-xml/how-to-insert-text-into-a-cell-in-a-spreadsheet#:~:text=it.%20%0A%20%20%20%20private%20static%20Cell-,InsertCellInWorksheet,-(string%20columnName%2C%20uint
         */
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>()!;
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex! == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex! == rowIndex).First();
            }
            else
            {
                row = new Row() {RowIndex = rowIndex};
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference!.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference!.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (cell.CellReference!.Value!.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() {CellReference = cellReference};
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }

        private void Import(object sender, RoutedEventArgs e)
        {
            // Application excelApp = new ();
            // Workbook workbook = excelApp.Workbooks.Add();
            /* Из-за того что умные люди в майкрософт, очень умные и не сделали нормальных способов для COM dependends
             Мы будем использовать адекватный вариант - OpenDocument(это официальная библиотека от microsoft, так 
             что ручки чистые)
             */

            SaveFileDialog fileDialog = new ();

            fileDialog.InitialDirectory = Environment.GetEnvironmentVariable("USERHOME");;
            fileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            fileDialog.FilterIndex = 1;
            string filePath;
            if (fileDialog.ShowDialog() == true)
            {
                filePath = fileDialog.FileName;
            }
            else
            {
                return;
            }

            SpreadsheetDocument spreadsheetDocument =
                SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook);
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());
            Sheets sheets = spreadsheetDocument.WorkbookPart!.Workbook.AppendChild(new Sheets());
            Sheet sheet = new Sheet
                {Id = spreadsheetDocument.WorkbookPart!.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Ваша таблица"};
            sheets.Append(sheet);
            uint rowId = 1;
            uint columnId = 0;
            foreach (DataColumn column in _dataTable.Columns)
            {
                var cell = InsertCellInWorksheet(Constants.ColumnNames[(int) columnId++], rowId, worksheetPart);
                cell.CellValue = new CellValue(column.ToString());
                cell.DataType = new EnumValue<CellValues>(CellValues.String);
            }

            rowId++;
            foreach (DataRow row in _dataTable.Rows)
            {
                columnId = 0;
                foreach (var item in row.ItemArray)
                {
                    var cell = InsertCellInWorksheet(Constants.ColumnNames[(int) columnId++], rowId, worksheetPart);
                    cell.CellValue = new CellValue(item.ToString());
                    // По хорошему надо сделать преобразование всех типов
                    cell.DataType = new EnumValue<CellValues>(item is int ? CellValues.Number : CellValues.String);
                }

                rowId++;
            }


            workbookpart.Workbook.Save();
            spreadsheetDocument.Close();
        }
    }
}