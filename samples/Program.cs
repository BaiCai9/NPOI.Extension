﻿using System;
using NPOI.Extension;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace samples
{
    class Program
    {
        static void Main(string[] args)
        {
            // global call this
            FluentConfiguration();

            var len = 10;
            var reports = new Report[len];
            for (int i = 0; i < len; i++)
            {
                reports[i] = new Report
                {
                    City = "ningbo",
                    Building = "世茂首府",
                    HandleTime = new DateTime(2015, 11, 23),
                    Broker = "rigofunc 18957139**7",
                    Customer = "rigofunc 18957139**7",
                    Room = "2#1703",
                    Brokerage = 125M,
                    Profits = 25m
                };
            }

            var excelFile = @"/Users/rigofunc/Documents/sample.xlsx";

            // save to excel file
            reports.ToExcel(excelFile);

            // load from excel
            var loadFromExcel = Excel.Load<Report>(excelFile: excelFile, valueConverter: (row, cellIndex, cellValue) =>
            {
                if (cellValue is null) return cellValue;

                object valueTemp = null;
                switch (cellIndex)
                {
                    case 5 when !(cellValue is DateTime) && cellValue is double://假如第6列是日期数据列
                        var isDate1904 = false;
                        if (row.Sheet.Workbook is HSSFWorkbook)
                        {
                            isDate1904 = ((HSSFWorkbook)row.Sheet.Workbook).Workbook.IsUsing1904DateWindowing;
                        }
                        else if (row.Sheet.Workbook is XSSFWorkbook)
                        {
                            isDate1904 = ((XSSFWorkbook)row.Sheet.Workbook).IsDate1904();
                        }

                        valueTemp = DateUtil.GetJavaDate((double)cellValue, isDate1904);
                        break;
                    default:
                        break;
                }

                return valueTemp;
            });
        }

        /// <summary>
        /// Use fluent configuration api. (doesn't poison your POCO)
        /// </summary>
        static void FluentConfiguration()
        {
            var fc = Excel.Setting.For<Report>();

            fc.HasStatistics("合计", "SUM", 6, 7)
              .HasFilter(firstColumn: 0, lastColumn: 2, firstRow: 0)
              .HasFreeze(columnSplit: 2,rowSplit: 1, leftMostColumn: 2, topMostRow: 1);

            fc.Property(r => r.City)
              .HasExcelIndex(0)
              .HasExcelTitle("城市")
              .IsMergeEnabled();

            fc.Property(r => r.Building)
              .HasExcelIndex(1)
              .HasExcelTitle("楼盘")
              .IsMergeEnabled();

            fc.Property(r => r.HandleTime)
              .HasExcelIndex(2)
              .HasExcelTitle("成交时间")
              .HasDataFormatter("yyyy-MM-dd");

            fc.Property(r => r.Broker)
              .HasExcelIndex(3)
              .HasExcelTitle("经纪人");

            fc.Property(r => r.Customer)
              .HasExcelIndex(4)
              .HasExcelTitle("客户");

            fc.Property(r => r.Room)
              .HasExcelIndex(5)
              .HasExcelTitle("房源");

            fc.Property(r => r.Brokerage)
              .HasExcelIndex(6)
              .HasExcelTitle("佣金(元)");

            fc.Property(r => r.Profits)
              .HasExcelIndex(7)
              .HasExcelTitle("收益(元)");
        }
    }
}