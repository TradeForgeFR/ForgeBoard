﻿using ForgeBoard.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ForgeBoard.Core.Views
{
    /// <summary>
    /// Logique d'interaction pour SparkChart.xaml
    /// </summary>
    public partial class SparkChart : UserControl
    {
        public List<SparkChartPoint> StockDatas 
        { get; set; }
        public SparkChart()
        {
            InitializeComponent(); 
        }

        private void GenerateRandomStockData()
        {
            StockDatas = new List<SparkChartPoint>();

            // Generate 20 random data points for demonstration
            Random random = new Random();
            DateTime startDate = DateTime.Now.AddDays(-20);

            for (int i = 0; i < 50; i++)
            {
                SparkChartPoint dataPoint = new SparkChartPoint
                {
                    Date = startDate.AddDays(i),
                    Price = random.Next(50, 150) + random.NextDouble()  // Random price between 50 and 150
                };

                StockDatas.Add(dataPoint);
            }
        }

        private void DrawStockChart()
        {
            if (StockDatas == null || StockDatas.Count < 2)
                return;

            double canvasWidth = chartCanvas.ActualWidth;
            double canvasHeight = chartCanvas.ActualHeight;

            // Find the minimum price instead of the maximum
            double minPrice = StockDatas.Min(dataPoint => dataPoint.Price);
            double maxPrice = StockDatas.Max(dataPoint => dataPoint.Price);

            // Ensure that the entire canvas height is used
            double priceScale = canvasHeight / ((maxPrice - minPrice) > 0 ? (maxPrice - minPrice) : 1);

            chartCanvas.Children.Clear();  // Clear existing drawings

            for (int i = 1; i < StockDatas.Count; i++)
            {
                double x1 = (canvasWidth / (StockDatas.Count - 1)) * (i - 1);
                double y1 = canvasHeight - ((StockDatas[i - 1].Price - minPrice) * priceScale);

                double x2 = (canvasWidth / (StockDatas.Count - 1)) * i;
                double y2 = canvasHeight - ((StockDatas[i].Price - minPrice) * priceScale);

                Line line = new Line
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = this.Foreground,
                    StrokeThickness = 0.5,
                    Opacity = 0.5
                };

                chartCanvas.Children.Add(line);
            }
        }

        private void DrawStockAreaChart()
        {
            if (StockDatas == null || StockDatas.Count < 2)
                return;

            double canvasWidth = chartCanvas.ActualWidth;
            double canvasHeight = chartCanvas.ActualHeight;

            double minPrice = StockDatas.Min(dataPoint => dataPoint.Price);
            double maxPrice = StockDatas.Max(dataPoint => dataPoint.Price);

            double priceScale = canvasHeight / ((maxPrice - minPrice) > 0 ? (maxPrice - minPrice) : 1);

            chartCanvas.Children.Clear();  // Clear existing drawings

            Polygon areaPolygon = new Polygon
            {
                Stroke = this.Foreground,
                StrokeThickness = 1,
                Opacity = 0.15,
                Fill = this.Foreground
            };

            for (int i = 0; i < StockDatas.Count; i++)
            {
                double x = (canvasWidth / (StockDatas.Count - 1)) * i;
                double y = canvasHeight - ((StockDatas[i].Price - minPrice) * priceScale);

                areaPolygon.Points.Add(new Point(x, y));
            }

            // Close the polygon to complete the area chart
            areaPolygon.Points.Add(new Point(canvasWidth, canvasHeight));
            areaPolygon.Points.Add(new Point(0, canvasHeight));

            chartCanvas.Children.Add(areaPolygon);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateRandomStockData();
            DrawStockAreaChart();
        }
    }  
}
