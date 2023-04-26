using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TTS_2019.Tools.Utils
{
    class ExportExcel
    {
        // 导出DataGrid数据到Excel
        #region 导出DataGrid数据到Excel
        /// <summary>
        /// CSV格式化
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>格式化数据</returns>
        private static string FormatCsvField(string data)
        {
            return String.Format("\"{0}\"", data.Replace("\"", "\"\"\"").Replace("\n", "").Replace("\r", ""));
        }
        /// <summary>
        /// 导出DataGrid数据到Excel
        /// </summary>
        /// <param name="withHeaders">是否需要表头</param>
        /// <param name="grid">DataGrid</param>
        /// <param name="dataBind"></param>
        /// <returns>Excel内容字符串</returns>
        public static string ExportDataGrid(bool withHeaders, System.Windows.Controls.DataGrid grid, DataTable dataTable)
        {
            try
            {
                var strBuilder = new System.Text.StringBuilder();//列头 + 数据
                //var source = (grid.ItemsSource as System.Collections.IList);
                //if (source == null) return "";
                var headers = new List<string>();//列头
                List<string> bt = new List<string>();
                if (withHeaders)
                {
                    //回填列头
                    foreach (var hr in grid.Columns)
                    {
                        // DataGridTextColumn textcol = hr. as DataGridTextColumn;
                        headers.Add(hr.Header.ToString());
                        if (hr is DataGridTextColumn)//列绑定数据
                        {
                            DataGridTextColumn textcol = hr as DataGridTextColumn;

                            if (textcol != null)
                                bt.Add((textcol.Binding as System.Windows.Data.Binding).Path.Path.ToString()); //获取绑定源
                        }
                        else if (hr is DataGridTemplateColumn)
                        {
                            if (hr.Header.Equals("操作"))
                                bt.Add("Id");
                        }
                        else { }

                    }
                }
                strBuilder.Append(String.Join(",", headers.ToArray())).Append("\r\n");
                //列数据
                foreach (DataRow data in dataTable.Rows)//
                {
                    var csvRow = new List<string>();//每一行的数据

                    //方法一 固定值    
                    //string s = data["ID"].ToString();
                    //csvRow.Add(FormatCsvField(s));

                    //方法二 循环增加
                    for (int i = 0; i < headers.Count; i++)
                    {
                        string s = data[i].ToString();
                        csvRow.Add(FormatCsvField(s));
                    }
                    strBuilder.Append(String.Join(",", csvRow.ToArray())).Append("\r\n");
                }
                return strBuilder.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 导出DataGrid数据到Excel为CVS文件
        /// 使用utf8编码 中文是乱码 改用Unicode编码
        /// 
        /// </summary>
        /// <param name="withHeaders">是否带列头</param>
        /// <param name="grid">DataGrid</param>
        public static void ExportDataGridSaveAs(bool withHeaders, System.Windows.Controls.DataGrid grid, DataTable dataTable)
        {
            try
            {
                string data = ExportDataGrid(true, grid, dataTable);
                var sfd = new Microsoft.Win32.SaveFileDialog
                {
                    DefaultExt = "csv",
                    Filter = "CSV Files (*.csv)|*.csv|All files (*.*)|*.*",
                    FilterIndex = 1
                };
                if (sfd.ShowDialog() == true)
                {
                    using (Stream stream = sfd.OpenFile())
                    {
                        using (var writer = new StreamWriter(stream, System.Text.Encoding.Unicode))
                        {
                            data = data.Replace(",", "\t");
                            writer.Write(data);
                            writer.Close();
                        }
                        stream.Close();
                    }
                }
                MessageBox.Show("导出成功", "提 示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            { }
        }

        #endregion 导出DataGrid数据到Excel
    }
}
