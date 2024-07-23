using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using GpxRouter.Utility;
using GpxRouter.Properties;
using System.IO;
using System.Text;
using Microsoft.Office.Interop.Excel;
using Point = System.Drawing.Point;

namespace GpxRouter.Forms
{
    public partial class MainForm : Form
    {
        #region Private classes

        private class UTF8StringWriter : StringWriter
        {
            #region Properties

            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }

            #endregion

            #region Constructors

            public UTF8StringWriter(StringBuilder stringBuilder) : base(stringBuilder)
            {

            }

            #endregion
        }

        #endregion

        #region Constants

        private const string NameColumnName = "NameColumn";
        private const string LatitudeColumnName = "LatitudeColumn";
        private const string LongitudeColumnName = "LongitudeColumn";

        private const string OpenFilesFilter = @"GPX files (*.gpx)|*.gpx|CSV files (*.csv)|*.csv";
        private const string SaveFilesFilter = @"GPX files (*.gpx)|*.gpx|CSV files (*.csv)|*.csv|Excel flight log files (*.xlsm)|*.xlsm";

        private const string IcaoFplDirect = "DCT";
        private const char IcaoFplSpace = ' ';

        #endregion

        #region Properties

        private bool AllCellsValid
        {
            get
            {
                foreach (DataGridViewRow row in dataGridViewWaypoints.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            bool? cellValidityTag = cell.Tag as bool?;
                            if (cellValidityTag.HasValue && cellValidityTag.Value == false)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
        }

        #endregion

        #region Constructors

        public MainForm()
        {
            InitializeComponent();

            // Prepare grids for receiving data
            SetUpGridViewColumns();
        }

        #endregion

        #region Private methods

        private void SetUpGridViewColumns()
        {
            #region Waypoints grid

            dataGridViewWaypoints.Columns.Add(new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                HeaderText = Resources.Name,
                Name = NameColumnName,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            dataGridViewWaypoints.Columns.Add(new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                HeaderText = Resources.Latitude,
                Name = LatitudeColumnName,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            dataGridViewWaypoints.Columns.Add(new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                HeaderText = Resources.Longitude,
                Name = LongitudeColumnName,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            #endregion
        }

        private void AddWaypointToGridView(Waypoint waypoint)
        {
            DataGridViewRow row = dataGridViewWaypoints.Rows[dataGridViewWaypoints.Rows.Add()];

            row.Cells[NameColumnName].Value = waypoint.Name;
            row.Cells[LatitudeColumnName].Value = waypoint.LatitudeDegrees;
            row.Cells[LongitudeColumnName].Value = waypoint.LongitudeDegrees;
        }

        private void ValidateRow(DataGridViewRow row)
        {
            // Validate all cells in the row being edited
            foreach (DataGridViewCell cell in row.Cells)
            {
                bool cellValueValid = true;

                // Validate latitude column
                if (cell.OwningColumn.Name == LatitudeColumnName)
                {
                    double latitudeValue;
                    cellValueValid = cell.Value != null && double.TryParse(cell.Value.ToString(), out latitudeValue) && latitudeValue >= -90 && latitudeValue <= 90;
                }
                // Validate longitude column
                else if (cell.OwningColumn.Name == LongitudeColumnName)
                {
                    double longitudeValue;
                    cellValueValid = cell.Value != null && double.TryParse(cell.Value.ToString(), out longitudeValue) && longitudeValue >= -180 && longitudeValue <= 180;
                }

                cell.Tag = cellValueValid;
                cell.Style.BackColor = cellValueValid ? Color.White : Color.Salmon;
            }
        }

        private Waypoint ExtractWaypointFromRow(DataGridViewRow row)
        {
            // Extract name
            string name = string.Empty;
            if (row.Cells[NameColumnName].Value != null)
            {
                name = row.Cells[NameColumnName].Value.ToString();
            }

            // Extract latitude
            double latitude = 0;
            if (row.Cells[LatitudeColumnName].Value != null)
            {
                double.TryParse(row.Cells[LatitudeColumnName].Value.ToString(), out latitude);
            }

            // Extract longitude
            double longitude = 0;
            if (row.Cells[LongitudeColumnName].Value != null)
            {
                double.TryParse(row.Cells[LongitudeColumnName].Value.ToString(), out longitude);
            }

            return new Waypoint(name, latitude, longitude);
        }

        private string CreateIcaoFplRouteString( IEnumerable< Waypoint > waypoints )
        {
            StringBuilder icaoFplRouteStringBuilder = new StringBuilder();

            icaoFplRouteStringBuilder.Append( IcaoFplDirect );

            foreach ( Waypoint waypoint in waypoints )
            {
                icaoFplRouteStringBuilder.Append( IcaoFplSpace );
                icaoFplRouteStringBuilder.Append( waypoint.ToIcaoFplRoutePointString() );
                icaoFplRouteStringBuilder.Append( IcaoFplSpace );
                icaoFplRouteStringBuilder.Append( IcaoFplDirect );
            }

            return icaoFplRouteStringBuilder.ToString();
        }

        #region Data loading/saving

        private List<Waypoint> ExtractWaypointsFromGpx(string gpx)
        {
            List<Waypoint> results = new List<Waypoint>();
            XNamespace xmlns = XNamespace.Get(@"http://www.topografix.com/GPX/1/1");

            // Check if at least one 'rte' element can be extracted
            XDocument gpxDocument = XDocument.Parse(gpx);
            XElement gpxElement = gpxDocument.Element(xmlns + "gpx");
            if (gpxElement == null)
            {
                throw new ArgumentException();
            }
            XElement rteElement = gpxElement.Element(xmlns + "rte");
            if (rteElement == null)
            {
                throw new ArgumentException();
            }

            // Attempt to extract waypoints
            List<XElement> rteptElements = rteElement.Elements(xmlns + "rtept").ToList();
            if (rteptElements.Count < 2)
            {
                throw new ArgumentException();
            }
            foreach (XElement rteptElement in rteptElements)
            {
                XAttribute latAttribute = rteptElement.Attribute("lat");
                XAttribute lonAttribute = rteptElement.Attribute("lon");
                XElement nameElement = rteptElement.Element(xmlns + "name");
                if (latAttribute == null || lonAttribute == null || nameElement == null)
                {
                    throw new ArgumentException();
                }

                results.Add(new Waypoint(nameElement.Value, double.Parse(latAttribute.Value), double.Parse(lonAttribute.Value)));
            }

            return results;
        }

        private List<Waypoint> ExtractWaypointsFromCsv(string csv)
        {
            List<Waypoint> results = new List<Waypoint>();

            // Attempt to extract waypoints
            string[] csvLines = csv.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (csvLines.Count() < 2)
            {
                throw new ArgumentException();
            }
            foreach (string csvLine in csvLines)
            {
                string[] csvLineColumnValues = csvLine.Split(',');
                if (csvLineColumnValues.Count() != 3)
                {
                    throw new ArgumentException();
                }

                results.Add(new Waypoint(csvLineColumnValues[0], double.Parse(csvLineColumnValues[1]), double.Parse(csvLineColumnValues[2])));
            }

            return results;
        }

        private string ExtractGpxFromGrid(string fileName)
        {
            XDocument gpxDocument = new XDocument(new XDeclaration("1.0", "utf-8", "no"));
            XNamespace xmlns = XNamespace.Get(@"http://www.topografix.com/GPX/1/1");
            XNamespace xsi = XNamespace.Get(@"http://www.w3.org/2001/XMLSchema-instance");
            XNamespace schemaLocation = XNamespace.Get(@"http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd");

            // Add main element
            XElement gpxElement = new XElement(xmlns + "gpx");
            gpxElement.Add(new XAttribute(XNamespace.Xmlns + "xsi", xsi));
            gpxElement.Add(new XAttribute(xsi + "schemaLocation", schemaLocation));
            gpxElement.Add(new XAttribute("version", 1.1));
            gpxElement.Add(new XAttribute("creator", Environment.UserName));
            gpxDocument.Add(gpxElement);

            // Add metadata
            XElement metadataElement = new XElement(xmlns + "metadata");
            metadataElement.Add(new XElement(xmlns + "name", fileName));
            metadataElement.Add(new XElement(xmlns + "author", new XElement(xmlns + "name", Environment.UserName)));
            metadataElement.Add(new XElement(xmlns + "time", DateTime.Now.ToLocalTime().ToString("O")));
            gpxElement.Add(metadataElement);

            // Add route/waypoints
            XElement routeElement = new XElement(xmlns + "rte");
            foreach (DataGridViewRow row in dataGridViewWaypoints.Rows)
            {
                if (!row.IsNewRow)
                {
                    Waypoint rowWaypoint = ExtractWaypointFromRow(row);

                    XElement waypointElement = new XElement(xmlns + "rtept");
                    waypointElement.Add(new XAttribute("lat", rowWaypoint.LatitudeDegrees));
                    waypointElement.Add(new XAttribute("lon", rowWaypoint.LongitudeDegrees));
                    waypointElement.Add(new XElement(xmlns + "name", rowWaypoint.Name));
                    routeElement.Add(waypointElement);
                }
            }
            gpxElement.Add(routeElement);

            // Convert result to string
            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = new UTF8StringWriter(builder))
            {
                gpxDocument.Save(writer);
            }

            return builder.ToString();
        }

        private string ExtractCsvFromGrid()
        {
            StringBuilder builder = new StringBuilder();

            // Add waypoints
            foreach (DataGridViewRow row in dataGridViewWaypoints.Rows)
            {
                if (!row.IsNewRow)
                {
                    Waypoint rowWaypoint = ExtractWaypointFromRow(row);

                    builder.Append(rowWaypoint.Name).Append(",");
                    builder.Append(rowWaypoint.LatitudeDegrees).Append(",");
                    builder.Append(rowWaypoint.LongitudeDegrees).Append("\n");
                }
            }

            return builder.ToString().Trim();
        }

        #endregion

        #endregion

        #region Event handlers

        private void dataGridViewWaypoints_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewWaypoints.BeginEdit(true);
        }

        private void dataGridViewWaypoints_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ValidateRow(dataGridViewWaypoints.Rows[e.RowIndex]);
        }

        private void dataGridViewWaypoints_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (dataGridViewWaypoints.CurrentRow != null && !dataGridViewWaypoints.CurrentRow.IsNewRow)
                {
                    dataGridViewWaypoints.Rows.Remove(dataGridViewWaypoints.CurrentRow);
                }
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            // Present open dialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = OpenFilesFilter
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                switch (openFileDialog.FilterIndex)
                {
                    // Open GPX
                    case 1:
                        try
                        {
                            List<Waypoint> gpxWaypoints = ExtractWaypointsFromGpx(File.ReadAllText(openFileDialog.FileName));
                            dataGridViewWaypoints.Rows.Clear();
                            gpxWaypoints.ForEach(AddWaypointToGridView);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show(Resources.UnableToExtractGpxRouteError, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;

                    // Open CSV
                    case 2:
                        try
                        {
                            List<Waypoint> csvWaypoints = ExtractWaypointsFromCsv(File.ReadAllText(openFileDialog.FileName));
                            dataGridViewWaypoints.Rows.Clear();
                            csvWaypoints.ForEach(AddWaypointToGridView);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show(Resources.UnableToExtractCsvRouteError, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                }
            }
        }

        private void buttonSaveAs_Click(object sender, EventArgs e)
        {
            // Warn regarding invalid cell values
            if (!AllCellsValid)
            {
                MessageBox.Show(Resources.InvalidCellsError, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<DataGridViewRow> waypointEntryRows = dataGridViewWaypoints.Rows.Cast<DataGridViewRow>().Where(row => !row.IsNewRow).ToList();

            // Warn regarding not enough waypoints
            if (waypointEntryRows.Count < 2)
            {
                MessageBox.Show(Resources.NotEnoughWaypointsError, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string defaultFilename = string.Format("{0} - {1}", ExtractWaypointFromRow(waypointEntryRows.First()).Name, ExtractWaypointFromRow(waypointEntryRows.Last()).Name);

            // Present save dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                FileName = defaultFilename,
                Filter = SaveFilesFilter
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    switch (saveFileDialog.FilterIndex)
                    {
                        // Save GPX
                        case 1:
                            File.WriteAllText(saveFileDialog.FileName, ExtractGpxFromGrid(Path.GetFileName(saveFileDialog.FileName)));
                            break;

                        // Save CSV
                        case 2:
                            File.WriteAllText(saveFileDialog.FileName, ExtractCsvFromGrid());
                            break;

                        // Save excel flight log
                        case 3:
                            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                            if (excelApp != null)
                            {
                                try
                                {
                                    File.WriteAllBytes(saveFileDialog.FileName, Resources.FlightLogTemplate);

                                    Workbook flightLogWorkbook = excelApp.Workbooks.Open(saveFileDialog.FileName);
                                    Worksheet flightLogWorksheet = flightLogWorkbook.Worksheets[1];
                                    Range flightLogRowRange = flightLogWorksheet.Range["A5:N6"];

                                    // Loop over waypoint entries and add them to the flight log
                                    for (int i = 0; i < waypointEntryRows.Count - 1; i++)
                                    {
                                        int legRowStartIndex = 5 + i*2;

                                        if (i != 0)
                                        {
                                            flightLogRowRange.Copy(flightLogWorksheet.Range[string.Format("A{0}:N{1}", legRowStartIndex, legRowStartIndex + 1)]);
                                        }

                                        Waypoint legWaypoint1 = ExtractWaypointFromRow(waypointEntryRows.ElementAt(i));
                                        Waypoint legWaypoint2 = ExtractWaypointFromRow(waypointEntryRows.ElementAt(i + 1));

                                        flightLogWorksheet.Range[string.Format("A{0}", legRowStartIndex)].Value = legWaypoint1.Name;
                                        flightLogWorksheet.Range[string.Format("A{0}", legRowStartIndex + 1)].Value = legWaypoint2.Name;
                                        flightLogWorksheet.Range[string.Format("E{0}", legRowStartIndex)].Value = Math.Round(legWaypoint1.BearingToDegrees(legWaypoint2));
                                        flightLogWorksheet.Range[string.Format("J{0}", legRowStartIndex)].Value = Math.Round(legWaypoint1.DistanceToMetres(legWaypoint2)*Utility.Global.MetresToNauticalMiles);
                                    }

                                    flightLogWorksheet.Columns[1].Autofit();

                                    flightLogWorkbook.Close(true);
                                }
                                finally
                                {
                                    excelApp.Quit();
                                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                                }
                            }
                            else
                            {
                                MessageBox.Show(Resources.ExcelError, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            break;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(Resources.UnableToSaveFileError, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonReverse_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> rows = dataGridViewWaypoints.Rows.Cast<DataGridViewRow>().Where(r => !r.IsNewRow).ToList();
            if (rows.Any())
            {
                rows.Reverse();

                dataGridViewWaypoints.Rows.Clear();
                dataGridViewWaypoints.Rows.AddRange(rows.ToArray());
            }
        }

        private void buttonMoveUp_Click(object sender, EventArgs e)
        {
            if (dataGridViewWaypoints.CurrentRow != null && !dataGridViewWaypoints.CurrentRow.IsNewRow)
            {
                int currentRowIndex = dataGridViewWaypoints.CurrentRow.Index;
                if (currentRowIndex > 0)
                {
                    DataGridViewCell selectedCell = dataGridViewWaypoints.CurrentCell;

                    dataGridViewWaypoints.Rows.Remove(dataGridViewWaypoints.CurrentRow);
                    dataGridViewWaypoints.Rows.Insert(currentRowIndex - 1, selectedCell.OwningRow);

                    dataGridViewWaypoints.ClearSelection();
                    selectedCell.Selected = true;
                }
            }
        }

        private void buttonMoveDown_Click(object sender, EventArgs e)
        {
            if (dataGridViewWaypoints.CurrentRow != null && !dataGridViewWaypoints.CurrentRow.IsNewRow)
            {
                DataGridViewRow lastRow = dataGridViewWaypoints.Rows[dataGridViewWaypoints.Rows.Count - 1];
                int lastRowIndex = lastRow.IsNewRow ? lastRow.Index - 1 : lastRow.Index;

                int currentRowIndex = dataGridViewWaypoints.CurrentRow.Index;
                if (currentRowIndex < lastRowIndex)
                {
                    DataGridViewCell selectedCell = dataGridViewWaypoints.CurrentCell;

                    dataGridViewWaypoints.Rows.Remove(dataGridViewWaypoints.CurrentRow);
                    dataGridViewWaypoints.Rows.Insert(currentRowIndex + 1, selectedCell.OwningRow);

                    dataGridViewWaypoints.ClearSelection();
                    selectedCell.Selected = true;
                }
            }
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            contextMenuStripCopy.Show( buttonCopy, new Point( 0, buttonCopy.Height ) );
        }

        private void toolStripMenuItemCopyWaypoint_Click(object sender, EventArgs e)
        {
            if (dataGridViewWaypoints.CurrentRow != null && !dataGridViewWaypoints.CurrentRow.IsNewRow)
            {
                string waypointClipboardText = $"{dataGridViewWaypoints.CurrentRow.Cells[NameColumnName].Value},{dataGridViewWaypoints.CurrentRow.Cells[LatitudeColumnName].Value},{dataGridViewWaypoints.CurrentRow.Cells[LongitudeColumnName].Value}";
                Clipboard.SetText(waypointClipboardText);
            }
        }

        private void toolStripMenuItemCopyIcaoFplAll_Click(object sender, EventArgs e)
        {
            if ( !AllCellsValid )
            {
                MessageBox.Show( Resources.InvalidCellsError, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            var waypointsToUse = new List< Waypoint >();
            foreach ( DataGridViewRow row in dataGridViewWaypoints.Rows )
            {
                if ( row != null && !row.IsNewRow )
                {
                    waypointsToUse.Add( ExtractWaypointFromRow( row ) );
                }
            }

            Clipboard.SetText( CreateIcaoFplRouteString( waypointsToUse ) );
        }

        private void toolStripMenuItemCopyIcaoFplExceptEnds_Click(object sender, EventArgs e)
        {
            if ( !AllCellsValid )
            {
                MessageBox.Show( Resources.InvalidCellsError, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            var waypointsToUse = new List< Waypoint >();
            foreach ( DataGridViewRow row in dataGridViewWaypoints.Rows )
            {
                if ( row != null && !row.IsNewRow )
                {
                    waypointsToUse.Add( ExtractWaypointFromRow( row ) );
                }
            }

            waypointsToUse.RemoveAll(
                w => new List< Waypoint >
                    {
                        waypointsToUse.FirstOrDefault(),
                        waypointsToUse.LastOrDefault()
                    }
                    .Where( x => x != null )
                    .Distinct()
                    .Contains( w ) );

            Clipboard.SetText( CreateIcaoFplRouteString( waypointsToUse ) );
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            if (dataGridViewWaypoints.CurrentRow != null)
            {
                string[] clipboardText = Clipboard.GetText().Split(',').Select(s => s.Trim()).ToArray();
                if (clipboardText.Length == 2)
                {
                    dataGridViewWaypoints.CurrentRow.Cells[LatitudeColumnName].Value = clipboardText.ElementAt(0);
                    dataGridViewWaypoints.CurrentRow.Cells[LongitudeColumnName].Value = clipboardText.ElementAt(1);

                    dataGridViewWaypoints.NotifyCurrentCellDirty(true);
                    dataGridViewWaypoints.NotifyCurrentCellDirty(false);
                }
                else if (clipboardText.Length == 3)
                {
                    dataGridViewWaypoints.CurrentRow.Cells[NameColumnName].Value = clipboardText.ElementAt(0);
                    dataGridViewWaypoints.CurrentRow.Cells[LatitudeColumnName].Value = clipboardText.ElementAt(1);
                    dataGridViewWaypoints.CurrentRow.Cells[LongitudeColumnName].Value = clipboardText.ElementAt(2);

                    dataGridViewWaypoints.NotifyCurrentCellDirty(true);
                    dataGridViewWaypoints.NotifyCurrentCellDirty(false);
                }
                else
                {
                    MessageBox.Show(Resources.PasteFormatError, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewWaypoints.CurrentRow != null && !dataGridViewWaypoints.CurrentRow.IsNewRow)
            {
                dataGridViewWaypoints.Rows.Remove(dataGridViewWaypoints.CurrentRow);
            }
        }

        #endregion
    }
}
