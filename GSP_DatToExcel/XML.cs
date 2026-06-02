using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GSP_DatToExcel
{
    internal static class XML
    {
        public static void ExcelToXml(string fileName)//生成xml文件
        {
            try
            {
                Form1.form1.AddLog("开始转换Xml......", Color.Blue);

                using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (XLWorkbook workbook = new XLWorkbook(fileStream))//FilePath
                {
                    //初始化一个xml实例
                    XmlDocument myXmlDoc = new XmlDocument();
                    //创建Xml的根节点
                    XmlElement rootElement = myXmlDoc.CreateElement("MINI3");

                    //Form1.form1.progressBar1.Maximum = workbook.Worksheets.Count;
                    //Form1.form1.progressBar1.Value = 0;
                    foreach (IXLWorksheet sheet in workbook.Worksheets)
                    {
                        var range = sheet.RangeUsed();

                        if (sheet.Name == "注释信息")
                        {
                            for (int i = 1; i < range.RowCount() + 1; i++)
                            {
                                if (i == 1)
                                    myXmlDoc.AppendChild(myXmlDoc.CreateXmlDeclaration(range.Cell(1, 1).Value.ToString(), range.Cell(2, 1).Value.ToString(), string.Empty));
                                else if (i > 2)
                                    myXmlDoc.AppendChild(myXmlDoc.CreateComment(range.Cell(i, 1).Value.ToString()));
                            }
                            //Form1.form1.progressBar1.Value++;
                            continue;
                        }

                        //创建表
                        XmlElement tableElement = myXmlDoc.CreateElement(sheet.Name);
                        rootElement.AppendChild(tableElement);

                        if (range == null)
                        {
                            //Form1.form1.progressBar1.Value++;
                            continue;
                        }

                        //创建行
                        for (int i = 2; i < range.RowCount() + 1; i++)
                        {
                            XmlElement rowElement = myXmlDoc.CreateElement("VALUE");
                            tableElement.AppendChild(rowElement);
                            //行赋值
                            for (int j = 1; j < range.ColumnCount() + 1; j++)
                            {
                                rowElement.SetAttribute(range.Cell(1, j).Value.ToString(), range.Cell(i, j).Value.ToString());
                            }
                        }
                        //Form1.form1.progressBar1.Value++;
                    }

                    myXmlDoc.AppendChild(rootElement);
                    //将xml文件保存到制定的路径下
                    //myXmlDoc.Save(Path.ChangeExtension(fileName, ".xml"));
                    //using (TextWriter sw = new StreamWriter(Path.ChangeExtension(fileName, ".xml"), false, Encoding.UTF8)) //Set encoding
                    //{
                    //    XmlWriterSettings xdrs = new XmlWriterSettings()
                    //    {
                    //        Encoding = Encoding.UTF8,
                    //    };
                    //    var xdr = XmlWriter.Create(sw, xdrs);
                    //    myXmlDoc.Save(xdr);
                    //}
                    XmlWriterSettings xdrs = new XmlWriterSettings();
                    XmlDeclaration xmlDeclaration = (XmlDeclaration)myXmlDoc.ChildNodes[0];
                    if (xmlDeclaration.Encoding == "utf-16")
                        xdrs.Encoding = Encoding.Unicode;
                    else
                        xdrs.Encoding = Encoding.UTF8;
                    using (XmlWriter xdr = XmlWriter.Create(Path.ChangeExtension(fileName, ".xml"), xdrs)) //Set encoding
                    {
                        myXmlDoc.Save(xdr);
                    }

                    File.WriteAllText(Path.ChangeExtension(fileName, ".xml"), File.ReadAllText(Path.ChangeExtension(fileName, ".xml"), Encoding.UTF8).Replace("&#xA;", "&#xD;&#xA;"), Encoding.UTF8);
                    Form1.form1.AddLog("转换Xml成功 -> " + Path.ChangeExtension(fileName, ".xml"), Color.Blue);

                }
            }
            catch (Exception ex)
            {
                Form1.form1.AddLog("转换Xml失败!!!", Color.Red);
                Form1.form1.AddLog("Error:" + ex.Message, Color.Red);
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public static void XmlToExcel(string fileName)//遍历xml文件的信息
        {
            try
            {
                Form1.form1.AddLog("开始转换Excel......", Color.Blue);

                //初始化一个xml实例
                XmlDocument myXmlDoc = new XmlDocument();

                using (TextReader file = File.OpenText(fileName))
                {
                    XmlReaderSettings xdrs = new XmlReaderSettings()
                    {
                        //IgnoreComments = true,
                        IgnoreWhitespace = true,
                        CloseInput = true
                    };
                    var xdr = XmlReader.Create(file, xdrs);

                    //加载XML文件（参数为xml文件的路径）
                    myXmlDoc.Load(xdr);
                }
                
                if (myXmlDoc.HasChildNodes)
                {
                    //新建工作簿
                    using (var workbook = new XLWorkbook())
                    {
                        //增加个sheet页
                        var worksheet = workbook.Worksheets.Add("注释信息");

                        //第一个sheet页为注释信息
                        for (int i = 0; i < myXmlDoc.ChildNodes.Count; i++)
                        {
                            XmlNode node = myXmlDoc.ChildNodes[i];
                            if (node.NodeType == XmlNodeType.XmlDeclaration)
                            {
                                XmlDeclaration xmlDeclaration = (XmlDeclaration)node;
                                worksheet.Cell(get_item(0, 0)).Style.NumberFormat.Format = "@";//文本格式
                                worksheet.Cell(get_item(0, 0)).Value = xmlDeclaration.Version;

                                worksheet.Cell(get_item(0, 1)).Style.NumberFormat.Format = "@";//文本格式
                                worksheet.Cell(get_item(0, 1)).Value = xmlDeclaration.Encoding;
                            }
                            else if (node.NodeType == XmlNodeType.Comment)
                            {
                                XmlComment xmlComment = (XmlComment)node;
                                worksheet.Cell(get_item(0, i + 1)).Style.NumberFormat.Format = "@";//文本格式
                                worksheet.Cell(get_item(0, i + 1)).Value = xmlComment.Value;
                            }
                        }

                        //获得第一个姓名匹配的节点（SelectSingNode）:此xml文件的根节点
                        XmlNode rootNode = myXmlDoc.SelectSingleNode("MINI3");
                        //获得该节点的子节点（即：该节点的第一层子节点）
                        XmlNodeList firstLevelNodeList = rootNode.ChildNodes;

                        //Form1.form1.progressBar1.Maximum = firstLevelNodeList.Count;
                        //Form1.form1.progressBar1.Value = 0;
                        List<Task> loadingTasks = new List<Task>();
                        foreach (XmlNode node in firstLevelNodeList)
                        {
                            ///*
                            DataTable dt = new DataTable(node.Name);
                            //判断此表是否有数据
                            if (node.HasChildNodes)
                            {
                                loadingTasks.Add(Task.Run(() =>
                                {
                                    XmlAttributeCollection headerAttr = node.FirstChild.Attributes;
                                    for (int i = 0; i < headerAttr.Count; i++)
                                        dt.Columns.Add(headerAttr[i].Name);

                                    DataRow dr = dt.NewRow();
                                    for (int i = 0; i < headerAttr.Count; i++)
                                        dr[headerAttr[i].Name] = headerAttr[i].Name;
                                    dt.Rows.Add(dr);

                                    //循环每行数据
                                    for (int i = 0; i < node.ChildNodes.Count; i++)
                                    {
                                        XmlNode secondLevelNode1 = node.ChildNodes[i];

                                        //获得该表的属性集合 即每列
                                        XmlAttributeCollection attributeCol = secondLevelNode1.Attributes;

                                        dr = dt.NewRow();
                                        //循环每列
                                        for (int j = 0; j < attributeCol.Count; j++)
                                        {
                                            XmlAttribute attri = attributeCol[j];
                                            //获得属性名称与属性值
                                            string name = attri.Name;
                                            string value = attri.Value;
                                            dr[name] = value;
                                        }
                                        dt.Rows.Add(dr);
                                    }

                                    var ws = workbook.Worksheets.Add(dt.TableName);
                                    ws.Cell(1, 1).InsertData(dt.Rows);
                                    ws.Columns().AdjustToContents();
                                }));

                            }
                            //*/

                            /*
                            //增加个sheet页 sheet名即为表名 RES_XXX
                            var xsheet = workbook.Worksheets.Add(node.Name);

                            //判断此表是否有数据
                            if (node.HasChildNodes)
                            {
                                loadingTasks.Add(Task.Run(() =>
                                {
                                    int col = node.ChildNodes[0].Attributes.Count - 1;
                                    int line = node.ChildNodes.Count - 1;
                                    //设置表头粗体
                                    var headRange = xsheet.Range("A1", get_item(col, 0));
                                    headRange.Style.Font.Bold = true;
                                    headRange.Style.NumberFormat.Format = "@";//文本格式

                                    var excelRange = xsheet.Range("A1", get_item(col, line + 1));
                                    excelRange.Style.NumberFormat.Format = "@";//文本格式
                                    excelRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    excelRange.Style.Border.InsideBorder = XLBorderStyleValues.Dotted;
                                    excelRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                                    excelRange.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    excelRange.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    excelRange.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    //循环每行数据
                                    for (int i = 0; i < node.ChildNodes.Count; i++)
                                    {
                                        XmlNode secondLevelNode1 = node.ChildNodes[i];

                                        //获得该表的属性集合 即每列
                                        XmlAttributeCollection attributeCol = secondLevelNode1.Attributes;
                                        //循环每列
                                        for (int j = 0; j < attributeCol.Count; j++)
                                        {
                                            XmlAttribute attri = attributeCol[j];
                                            //获得属性名称与属性值
                                            string name = attri.Name;
                                            string value = attri.Value;
                                            //第一次先生成表头
                                            if (i == 0)
                                                excelRange.Cell(get_item(j, 0)).Value = name;
                                            excelRange.Cell(get_item(j, i + 1)).Value = value;
                                        }
                                    }

                                    //Form1.form1.progressBar1.Value++;
                                }));
                            }
                            */
                            //else
                            //Form1.form1.progressBar1.Value++;
                        };
                        if (loadingTasks.Count > 0)
                            Task.WaitAll(loadingTasks.ToArray());
                        workbook.SaveAs(Path.ChangeExtension(fileName, ".xlsx"));
                        Form1.form1.AddLog("转换Excel成功 -> " + Path.ChangeExtension(fileName, ".xlsx"), Color.Blue);
                    }

                }

            }
            catch (Exception ex)
            {
                Form1.form1.AddLog("转换Excel失败!!!", Color.Red);
                Form1.form1.AddLog("Error:" + ex.Message, Color.Red);
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// 获取单元格
        /// </summary>
        /// <param name="iCol">第几列，以0起始</param>
        /// <param name="iLine">第几行，以0起始</param>
        /// <returns></returns>
        private static string get_item(int iCol, int iLine)
        {
            string strCol = "";
            if (iCol < 26)
                strCol = string.Format("{0}{1}", ((char)((int)'a' + iCol)), iLine + 1);
            else if (iCol < 26 * 26 + 26)
            {
                iCol -= 26;
                strCol = string.Format("{0}{1}{2}", ((char)((int)'a' + iCol / 26)), ((char)((int)'a' + iCol % 26)), iLine + 1);
            }
            return strCol;
        }
    }
}
