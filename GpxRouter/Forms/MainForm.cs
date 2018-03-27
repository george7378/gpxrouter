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

namespace GpxRouter.Forms
{
    public partial class MainForm : Form
    {
        #region Private classes

        private class Utf8StringWriter : StringWriter
        {
            #region Properties

            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }

            #endregion

            #region Constructors

            public Utf8StringWriter(StringBuilder sb) : base(sb)
            {

            }

            #endregion
        }

        #endregion

        #region Constants

        private const string NameColumnName = "NameColumn";
        private const string LatitudeColumnName = "LatitudeColumn";
        private const string LongitudeColumnName = "LongitudeColumn";

        private const string FilesFilter = @"GPX files (*.gpx)|*.gpx|CSV files (*.csv)|*.csv";

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
                Name = NameColumnName
            });

            dataGridViewWaypoints.Columns.Add(new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                HeaderText = Resources.Latitude,
                Name = LatitudeColumnName
            });

            dataGridViewWaypoints.Columns.Add(new DataGridViewTextBoxColumn
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                HeaderText = Resources.Longitude,
                Name = LongitudeColumnName
            });

            #endregion
        }

        private void AddWaypointToGridView(Waypoint waypoint)
        {
            int rowId = dataGridViewWaypoints.Rows.Add();
            DataGridViewRow row = dataGridViewWaypoints.Rows[rowId];

            row.Cells[NameColumnName].Value = waypoint.Name;
            row.Cells[LatitudeColumnName].Value = waypoint.Latitude;
            row.Cells[LongitudeColumnName].Value = waypoint.Longitude;
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
                    float latitudeValue;
                    cellValueValid = cell.Value != null && float.TryParse(cell.Value.ToString(), out latitudeValue) && latitudeValue >= -90 && latitudeValue <= 90;
                }
                // Validate longitude column
                else if (cell.OwningColumn.Name == LongitudeColumnName)
                {
                    float longitudeValue;
                    cellValueValid = cell.Value != null && float.TryParse(cell.Value.ToString(), out longitudeValue) && longitudeValue >= -180 && longitudeValue <= 180;
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
            float latitude = 0;
            if (row.Cells[LatitudeColumnName].Value != null)
            {
                float.TryParse(row.Cells[LatitudeColumnName].Value.ToString(), out latitude);
            }

            // Extract longitude
            float longitude = 0;
            if (row.Cells[LongitudeColumnName].Value != null)
            {
                float.TryParse(row.Cells[LongitudeColumnName].Value.ToString(), out longitude);
            }

            return new Waypoint(name, latitude, longitude);
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

                results.Add(new Waypoint(nameElement.Value, float.Parse(latAttribute.Value), float.Parse(lonAttribute.Value)));
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

                results.Add(new Waypoint(csvLineColumnValues[0], float.Parse(csvLineColumnValues[1]), float.Parse(csvLineColumnValues[2])));
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
                    waypointElement.Add(new XAttribute("lat", rowWaypoint.Latitude));
                    waypointElement.Add(new XAttribute("lon", rowWaypoint.Longitude));
                    waypointElement.Add(new XElement(xmlns + "name", rowWaypoint.Name));
                    routeElement.Add(waypointElement);
                }
            }
            gpxElement.Add(routeElement);

            // Convert result to string
            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = new Utf8StringWriter(builder))
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
                    builder.Append(rowWaypoint.Latitude).Append(",");
                    builder.Append(rowWaypoint.Longitude).Append("\n");
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
            DataGridViewRow row = dataGridViewWaypoints.Rows[e.RowIndex];
            if (!row.IsNewRow)
            {
                ValidateRow(row);
            }
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
                Filter = FilesFilter
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
            // Warn regarding no waypoints
            if (dataGridViewWaypoints.Rows.Cast<DataGridViewRow>().Count(row => !row.IsNewRow) < 2)
            {
                MessageBox.Show(Resources.NotEnoughWaypointsError, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Warn regarding invalid cell values
            if (!AllCellsValid)
            {
                MessageBox.Show(Resources.InvalidCellsError, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Present save dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = FilesFilter
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
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(Resources.UnableToSaveFileError, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
