using System;
using System.Data;
using System.IO;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Jinher.AMP.BTP.Common
{
    public static class ExcelHelper
    {
        public static byte[] Export(DataTable dtSource)
        {
            int[] numArray = new int[dtSource.Columns.Count];
            foreach (DataColumn column in (InternalDataCollectionBase)dtSource.Columns)
                numArray[column.Ordinal] = Encoding.GetEncoding(936).GetBytes(column.ColumnName.ToString()).Length;
            for (int index1 = 0; index1 < dtSource.Rows.Count; ++index1)
            {
                for (int index2 = 0; index2 < dtSource.Columns.Count; ++index2)
                {
                    int length = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[index1][index2].ToString()).Length;
                    if (length > numArray[index2])
                        numArray[index2] = length <= 254 ? length : 254;
                }
            }
            IWorkbook workbook = (IWorkbook)new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();
            IRow row1 = sheet.CreateRow(0);
            ICellStyle cellStyle1 = workbook.CreateCellStyle();
            cellStyle1.Alignment = HorizontalAlignment.Center;
            IFont font1 = workbook.CreateFont();
            font1.FontHeightInPoints = (short)10;
            font1.Boldweight = (short)700;
            cellStyle1.IsLocked = true;
            cellStyle1.SetFont(font1);
            foreach (DataColumn column in (InternalDataCollectionBase)dtSource.Columns)
            {
                ICell cell = row1.CreateCell(column.Ordinal);
                cell.SetCellValue(column.ColumnName);
                cell.CellStyle = cellStyle1;
                sheet.SetColumnWidth(column.Ordinal, (numArray[column.Ordinal] + 1) * 256);
            }
            sheet.CreateFreezePane(0, 1, 0, dtSource.Columns.Count - 1);
            workbook.CreateCellStyle().DataFormat = workbook.CreateDataFormat().GetFormat("yyyy年m月d日");
            ICellStyle cellStyle2 = workbook.CreateCellStyle();
            IFont font2 = workbook.CreateFont();
            font2.Underline = FontUnderlineType.Single;
            font2.Color = (short)12;
            cellStyle2.SetFont(font2);
            IDrawing drawingPatriarch = sheet.CreateDrawingPatriarch();
            for (int index = 0; index < dtSource.Rows.Count; ++index)
            {
                DataRow row2 = dtSource.Rows[index];
                IRow row3 = sheet.CreateRow(index + 1);
                foreach (DataColumn column in (InternalDataCollectionBase)dtSource.Columns)
                {
                    ICell cell = row3.CreateCell(column.Ordinal);
                    switch (column.DataType.ToString())
                    {
                        case "System.String":
                            cell.SetCellValue(row2[column].ToString());
                            continue;
                        case "System.DateTime":
                            cell.SetCellValue(DateTime.Parse(row2[column].ToString()));
                            continue;
                        case "System.Boolean":
                            cell.SetCellValue(bool.Parse(row2[column].ToString()));
                            continue;
                        case "System.Int16":
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            cell.SetCellValue((double)int.Parse(row2[column].ToString()));
                            continue;
                        case "System.Single":
                        case "System.Decimal":
                        case "System.Double":
                            cell.SetCellValue(double.Parse(row2[column].ToString()));
                            continue;
                        case "System.DBNull":
                            cell.SetCellValue("");
                            continue;
                        default:
                            cell.SetCellValue("");
                            continue;
                    }
                }
            }
            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.Write((Stream)memoryStream);
                return memoryStream.ToArray();
            }
        }

        //public static MemoryStream DataToExcel(DataTable dt)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    using (dt)
        //    {
        //        IWorkbook workbook = new HSSFWorkbook();//Create an excel Workbook
        //        ISheet sheet = workbook.CreateSheet();//Create a work table in the table
        //        IRow headerRow = sheet.CreateRow(0); //To add a row in the table
        //        foreach (DataColumn column in dt.Columns)
        //            headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);
        //        int rowIndex = 1;
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            IRow dataRow = sheet.CreateRow(rowIndex);
        //            foreach (DataColumn column in dt.Columns)
        //            {
        //                dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
        //            }
        //            rowIndex++;
        //        }
        //        workbook.Write(ms);
        //        ms.Flush();
        //        ms.Position = 0;
        //    }
        //    return ms;
        //}
        public static DataTable Import(Stream stream, string type)
        {
            DataTable dt = new DataTable();
            IWorkbook hssfworkbook;
            switch (type)
            {
                case "xlsx":
                    hssfworkbook = new XSSFWorkbook(stream);
                    break;
                default:
                    hssfworkbook = new HSSFWorkbook(stream);
                    break;
            }
            //IWorkbook hssfworkbook = new XSSFWorkbook(stream);
            var sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
            var headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            for (int j = 0; j < cellCount; j++)
            {
                var cell = headerRow.GetCell(j);
                dt.Columns.Add(cell.ToString().Trim());
            }
            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();
                for (int j = row.FirstCellNum; j < cellCount; j++)
                {
                    if (row.GetCell(j) != null)
                        dataRow[j] = row.GetCell(j).ToString();
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }             
    }
}
