using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using PublicConfig;
using System.IO;

namespace IntelligentCarbinet
{
    public class MemoryTable
    {

        public static bool isInitialized = false;
        static DataSet ds = null;
        static string dbFilePath = "db.xml";
        public static DataTable unitTable = null;
        public static DataTable remindTable = null;//统计标签数量，专为防盗功能提醒服务
        public static DataTable typeMapTable = null;//new DataTable("typeMapTable");//对每种产品的通用参数的定义
        public static DataTable staticsTable = null;
        public static bool memoryTableInitilize()
        {
            return initializeTabes();
        }
        static bool initializeTabes()
        {
            if (isInitialized == true)
            {
                return true;
            }
            else
            {
                ds = new DataSet("nsDB");
                ds.Namespace = "";
                if (!File.Exists(dbFilePath))
                {
                    ds.WriteXml(dbFilePath);
                }
                else
                {
                    ds.ReadXml(dbFilePath);

                }
                //初始化各个表的结构，如果读取的文件中没有该表结构的话
                if (ds.Tables.IndexOf("unitTable") == -1)
                {
                    unitTable = new DataTable("unitTable");
                    unitTable.Columns.Add("id", typeof(string));
                    unitTable.Columns.Add("epc", typeof(string));
                    unitTable.Columns.Add("group", typeof(int));
                    unitTable.Columns.Add("floor", typeof(int));
                    //unitTable.Columns.Add("index", typeof(string));
                    unitTable.Columns.Add("status", typeof(string));//当前状态
                    unitTable.Columns.Add("action", typeof(string));//标记是否是新添加的或者删除的
                    unitTable.Columns.Add("type", typeof(string));//标记是否是新添加的或者删除的

                    ds.Tables.Add(unitTable);
                }
                else
                {
                    unitTable = ds.Tables["unitTable"];
                }
                if (ds.Tables.IndexOf("typeMapTable") == -1)
                {
                    typeMapTable = new DataTable("typeMapTable");
                    typeMapTable.Columns.Add("type", typeof(string));
                    typeMapTable.Columns.Add("picname", typeof(string));
                    typeMapTable.Columns.Add("width", typeof(int));
                    typeMapTable.Columns.Add("height", typeof(int));
                    typeMapTable.Columns.Add("count", typeof(int));
                    typeMapTable.Columns.Add("minCount", typeof(int));
                    typeMapTable.Columns.Add("productName", typeof(string));
                    typeMapTable.Columns.Add("red", typeof(int));
                    typeMapTable.Columns.Add("green", typeof(int));
                    typeMapTable.Columns.Add("blue", typeof(int));
                    typeMapTable.Columns.Add("description", typeof(string));

                    ds.Tables.Add(typeMapTable);
                }
                else
                {
                    typeMapTable = ds.Tables["typeMapTable"];
                }

                if (null == remindTable)
                {
                    remindTable = new DataTable("remindTable");
                    remindTable.Columns.Add("epc", typeof(string));
                    remindTable.Columns.Add("action", typeof(string));//当前状态
                    remindTable.Columns.Add("type", typeof(string));//产品类别
                    remindTable.Columns.Add("productName", typeof(string));
                }

                //DataTable dt = getAllProductConfigInfo();
                //if (dt != null && dt.Rows.Count >= 0)
                //{
                //    for (int i = 0; i < dt.Rows.Count; i++)
                //    {
                //        DataRow dr = dt.Rows[i];
                //        string type = (string)dr["VTYPE"];
                //        string picName = (string)dr["PICNAME"];
                //        int width = int.Parse(dr["WIDTH"].ToString());
                //        int height = int.Parse(dr["HEIGHT"].ToString());
                //        int minCount = int.Parse(dr["MINCOUNT"].ToString());
                //        string productName = (string)dr["PRODUCTNAME"];
                //        int red = int.Parse(dr["RED"].ToString());
                //        int green = int.Parse(dr["GREEN"].ToString());
                //        int blue = int.Parse(dr["BLUE"].ToString());

                //        typeMapTable.Rows.Add(new object[] { type, picName, width, height, 0, minCount, productName, red, green, blue });

                //    }
                //}
                //else
                //{
                //typeMapTable.Rows.Add(new object[] { "01", "WP_000019.png", 80, 80, 0, 3, "指导书", 221, 221, 0 });
                //typeMapTable.Rows.Add(new object[] { "02", "pad.png", 80, 80, 0, 3, "pad", 128, 128, 128 });
                //typeMapTable.Rows.Add(new object[] { "03", "文具袋.png", 90, 90, 0, 3, "文具袋", 30, 70, 225 });
                //}

                //上下架统计
                staticsTable = new DataTable("staticsTable");
                staticsTable.Columns.Add("产品编号", typeof(string));
                staticsTable.Columns.Add("事件", typeof(string));//标记是否是上架还是下架 
                staticsTable.Columns.Add("时间", typeof(string));//标记上架或者下架的时间

            }
            isInitialized = true;
            return true;
        }
        public static DataRow getSpecifiedProductConfigInfo(string type)
        {
            if (initializeTabes() == true)
            {
                DataRow[] rows = typeMapTable.Select("type = '" + type + "'");
                if (rows.Length > 0)
                {
                    return rows[0];
                }

                //return typeMapTable;
            }
            return null;
            //DataSet ds = null;
            //try
            //{
            //    //ds = DbOperate.ExecuteDataSet(sqlSelect_SpecifiedConfig, new object[1] { type });
            //    //if (ds != null)
            //    //{
            //    //    if (ds.Tables.Count > 0)
            //    //    {
            //    //        return ds.Tables[0];
            //    //    }
            //    //}
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show("查询数据库时出现错误：" + ex.Message);
            //}
            //return null;
        }
        //public static DataTable getAllProductConfigInfo()
        //{
        //DataSet ds = null;
        //try
        //{
        //    return 
        //    //ds = DbOperate.ExecuteDataSet(sqlSelect_allProductConfig, null);
        //    //if (ds != null)
        //    //{
        //    //    if (ds.Tables.Count > 0)
        //    //    {
        //    //        return ds.Tables[0];
        //    //    }
        //    //}
        //}
        //catch (System.Exception ex)
        //{
        //    MessageBox.Show("查询数据库时出现错误：" + ex.Message);
        //}
        //return null;
        //}
        public static bool AddNewConfig(string type, string picName, int width, int height,
                                int minCount, string productName, int red, int green, int blue, string _description)
        {
            if (initializeTabes() == true)
            {
                try
                {
                    if (!ConfigExists(type))
                    {
                        typeMapTable.Rows.Add(new object[]
                    {
                         type
                        ,picName
                        ,width
                        ,height
                        ,0
                        ,minCount
                        ,productName
                        ,red
                        ,green
                        ,blue
                        ,_description
                    });
                        ds.WriteXml(dbFilePath);
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                    //int result = int.Parse(DbOperate.ExecuteNonQuery(
                    //             sqlInsert_addConfig
                    //             , new object[9]
                    //                                    {
                    //                                        type
                    //                                        ,picName
                    //                                        ,width
                    //                                        ,height
                    //                                        ,minCount
                    //                                        ,productName
                    //                                        ,red
                    //                                        ,green
                    //                                        ,blue
                    //                                    }).ToString());
                    //if (result > 0)
                    //{
                    //    return true;
                    //}
                }
                catch (System.Exception ex)
                {

                    MessageBox.Show("更新数据时出现错误：" + ex.Message);
                }
            }
            return false;
        }
        public static bool deleteConfig(string type)
        {
            if (initializeTabes() == true)
            {
                DataRow[] rows = typeMapTable.Select("type = '" + type + "'");
                if (rows.Length > 0)
                {
                    typeMapTable.Rows.Remove(rows[0]);
                    ds.WriteXml(dbFilePath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
            //string sql = sqlDelete_deleteConfig;
            //try
            //{
            //    //int result = int.Parse(DbOperate.ExecuteNonQuery(
            //    //             sql
            //    //             , new object[1]
            //    //                                    {
            //    //                                        type
            //    //                                    }).ToString());
            //    //if (result > 0)
            //    //{
            //    //    return true;
            //    //}
            //}
            //catch (System.Exception ex)
            //{

            //    MessageBox.Show("删除数据时出现错误：" + ex.Message);
            //}
            //return false;
        }
        public static List<string> getAllConfigTypes()
        {
            List<string> list = new List<string>();
            if (initializeTabes() == true)
            {
                for (int i = 0; i < typeMapTable.Rows.Count; i++)
                {
                    list.Add(typeMapTable.Rows[i]["type"].ToString());
                }
            }

            return list;
        }
        public static bool updateConfig(string type, string picName, int width, int height,
                                int minCount, string productName, int red, int green, int blue, string _description)
        {
            if (initializeTabes() == true)
            {
                DataRow[] rows = typeMapTable.Select("type = '" + type + "'");
                if (rows.Length > 0)
                {
                    DataRow dr = rows[0];
                    dr["picname"] = picName;
                    dr["width"] = width;
                    dr["height"] = height;
                    dr["minCount"] = minCount;
                    dr["productName"] = productName;
                    dr["red"] = red;
                    dr["green"] = green;
                    dr["blue"] = blue;
                    dr["description"] = _description;
                    ds.WriteXml(dbFilePath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //string sql = sqlUpdate_updateConfig;
            //try
            //{
            //    //int result = int.Parse(DbOperate.ExecuteNonQuery(
            //    //             sql
            //    //             , new object[9]
            //    //                                    {
            //    //                                        picName
            //    //                                        ,width
            //    //                                        ,height
            //    //                                        ,minCount
            //    //                                        ,productName
            //    //                                        ,red
            //    //                                        ,green
            //    //                                        ,blue
            //    //                                        ,type
            //    //                                    }).ToString());
            //    //if (result > 0)
            //    //{
            //    //    return true;
            //    //}
            //}
            //catch (System.Exception ex)
            //{

            //    MessageBox.Show("更新数据时出现错误：" + ex.Message);
            //}
            return false;
        }
        public static bool ConfigExists(string type)
        {
            if (initializeTabes() == true)
            {
                DataRow[] rows = typeMapTable.Select("type = '" + type + "'");
                if (rows.Length > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //DataSet ds = null;
            //try
            //{
            //    //ds = DbOperate.ExecuteDataSet(sqlSelect_SpecifiedConfig, new object[1] { type });
            //    //if (ds != null)
            //    //{
            //    //    if (ds.Tables.Count > 0)
            //    //    {
            //    //        if (ds.Tables[0].Rows.Count > 0)
            //    //        {
            //    //            return true;
            //    //        }
            //    //    }
            //    //}
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show("查询数据库时出现错误：" + ex.Message);
            //}
            return false;
        }
        public static string getProductTypeID(string tag)
        {
            if (tag != null && tag.Length >= 22)
            {
                return tag.Substring(20, 2);

            }
            else
            {
                return "ff";
            }
        }
        public static string sqlSelect_SpecifiedConfig =
            @"SELECT  VTYPE,PICNAME,WIDTH,HEIGHT,MINCOUNT,PRODUCTNAME,RED,GREEN,BLUE FROM  T_PRODUCT_CONFIG where VTYPE=@VTYPE;";
        public static string sqlUpdate_updateConfig =
            @" UPDATE T_PRODUCT_CONFIG set PICNAME=@PICNAME,WIDTH=@WIDTH,HEIGHT=@HEIGHT
                ,MINCOUNT=@MINCOUNT,PRODUCTNAME=@PRODUCTNAME,RED=@RED,GREEN=@GREEN,BLUE=@BLUE
                 WHERE VTYPE= @VTYPE;";
        public static string sqlInsert_addConfig =
                 @" INSERT INTO T_PRODUCT_CONFIG(VTYPE,PICNAME,WIDTH,HEIGHT,MINCOUNT,PRODUCTNAME,RED,GREEN,BLUE)
                    VALUES(@VTYPE,@PICNAME,@WIDTH,@HEIGHT,@MINCOUNT,@PRODUCTNAME,@RED,@GREEN,@BLUE);";
        public static string sqlDelete_deleteConfig =
            @"DELETE FROM T_PRODUCT_CONFIG WHERE VTYPE= @VTYPE;";
        public static string sqlSelect_allProductConfig =
                @"SELECT  VTYPE,PICNAME,WIDTH,HEIGHT,MINCOUNT,PRODUCTNAME,RED,GREEN,BLUE FROM  T_PRODUCT_CONFIG;";
    }
}
