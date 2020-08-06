using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MainApplication
{
    //THIS IS AN ENHANCE VERSION OF WHAT IS SHOWN IN THE VIDEO
    public class DonutChart: UserControl
    {
        private int SizeOffset = 31;
        private int CircleBrushWidth = 30;
        private int EmptySpaceHeight = 10; //height of the gap/empty space
        private int ChartItemDefaultHeight = 20; //height of the indicator/item
        private Size ChartSize; //the size of the chart only, indicator is not included

        private List<Color> Colors = new List<Color>();
        private List<ChartItem> ChartItems = new List<ChartItem>();

        [Browsable(true)] //show it on the "Properties Window"
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] //to save it on the Designer.cs
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                Invalidate();
            }
        }

        //chart data, use ObservableCollection instead of List so that we could monitor if the collection has changed
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<Slice> Slices { get; set; } = new ObservableCollection<Slice>();

        [Browsable(false)]
        public double TotalValue
        {
            get { return Slices.Count == 0 ? 0 : Slices.Sum(x => x.Value); }
        }

        //constructor
        public DonutChart()
        {
            Size = new Size(220, 220);
            MinimumSize = new Size(220, 220);
            ChartSize = Size;

            //remove flickerring
            DoubleBuffered = true;

            //subscibe to the ObservableCollection event
            Slices.CollectionChanged += Slices_CollectionChanged;

            //setup the colors
            Colors.Add(Color.LimeGreen);
            Colors.Add(Color.SteelBlue);
            Colors.Add(Color.Blue);
            Colors.Add(Color.Red);
            Colors.Add(Color.Yellow);
            Colors.Add(Color.Cyan);
            Colors.Add(Color.Violet);
            Colors.Add(Color.YellowGreen);
            Colors.Add(Color.Brown);
            Colors.Add(Color.Orange);

            //ENHANCEMENT =======> we just set the default font on the constructor instead of in the paint event
            Font = new Font("Segoe UI", 18, FontStyle.Bold);
        }

        private void Slices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int startAngle = 0;

            //everytime the collection (Slices) has changed we will recreate the ChartItems
            ChartItems.Clear();
            for (int i = 0; i < Slices.Count; i++)
            {
                //get the current slice
                Slice slice = Slices[i];

                //calculate the end angle of the current slice
                int endAngle = (int)Math.Round((360.0 / TotalValue) * slice.Value);

                //calculate the percentage
                double percentage = slice.Value == 0 ? 0 : Math.Round((slice.Value / TotalValue) * 100, 2);
                //ensure that it will total to 100%
                if (Slices.Count - 1 == i)
                    percentage = 100 - ChartItems.Sum(x => x.Percentage);

                //create a new chartItem and add it on the list
                ChartItem chartItem = new ChartItem();

                //set the properties of the chart/slice
                chartItem.Slice = slice;
                chartItem.ChartStartAngle = startAngle;
                chartItem.ChartEndAngle = endAngle;
                chartItem.Color = Colors[i];

                //set the properties of the indicator
                chartItem.Size = new Size(15, 15);
                chartItem.Location = CalculatePositionOfIndicator(i); //ENHANCEMENT ===> computation was moved to a function named "CalculatePositionOfIndicator"

                //set the properties of the text
                chartItem.Percentage = percentage;
                chartItem.Font = new Font("Segoe UI", 9);

                ChartItems.Add(chartItem);

                //calculate the start angle of the next slice
                startAngle += endAngle;
            }

            //repaint the control once the collection has been changed
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //remove pixelation
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            if (Slices.Count == 0)
            {
                //set the control again to original size, if the user removes the slices
                Size = ChartSize;

                //draw the empty chart
                using (SolidBrush brush = new SolidBrush(Color.Gainsboro))
                {
                    using (Pen pen = new Pen(brush, CircleBrushWidth))
                    {
                        e.Graphics.DrawArc(pen, 15, 15, Width - SizeOffset, Height - SizeOffset, 0, 360);
                    }
                }
            }
            else
            {
                //ENHANCEMENT ====> computation was moved to function named "CalculateTotalSize"
                //resize the control
                Size = CalculateTotalSize();

                //iterate on the ChartItems
                foreach (ChartItem chartItem in ChartItems)
                {
                    //draw the chart
                    using (SolidBrush brush = new SolidBrush(chartItem.Color))
                    {
                        using (Pen pen = new Pen(brush, CircleBrushWidth))
                        {
                            //show an effect on the chart if it's item/indicator is selected
                            if (chartItem.Font.Bold)
                            {
                                pen.Color = Color.FromArgb(70, 70, 70);
                                e.Graphics.DrawArc(pen, 15, 15, ChartSize.Width - SizeOffset, ChartSize.Height - SizeOffset,
                                    chartItem.ChartStartAngle, chartItem.ChartEndAngle);

                                //reset the pen and resize
                                pen.Color = chartItem.Color;
                                pen.Width = CircleBrushWidth - 10;
                            }

                            e.Graphics.DrawArc(pen, 15, 15, ChartSize.Width - SizeOffset, ChartSize.Height - SizeOffset,
                                chartItem.ChartStartAngle, chartItem.ChartEndAngle);
                        }
                    }

                    //draw the indicator
                    using (SolidBrush brush = new SolidBrush(chartItem.Color))
                    {
                        Rectangle rectangle = new Rectangle();
                        rectangle.Size = chartItem.Size;
                        rectangle.Location = chartItem.Location;

                        //ENHANCEMENT ====> added a rectangle border when the item was selected
                        if (chartItem.Font.Bold)
                        {
                            using (Pen pen = new Pen(brush))
                            {
                                pen.Color = Color.FromArgb(70, 70, 70);
                                e.Graphics.DrawRectangle(pen, rectangle);
                            }
                        }

                        e.Graphics.FillRectangle(brush, rectangle);
                    }
                    
                    //draw the text
                    using (SolidBrush brush = new SolidBrush(Color.Black))
                    {
                        //ENHANCEMENT ====> added "%" sign
                        string text = String.Format("{0} ({1}) = {2}%",
                            chartItem.Slice.Text, chartItem.Slice.Value, chartItem.Percentage);

                        e.Graphics.DrawString(text, chartItem.Font, brush, new Point(23, chartItem.Location.Y));
                    }
                }

                //draw the total
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    e.Graphics.DrawString("Total: " + TotalValue.ToString(), new Font("Segoe UI", 9, FontStyle.Bold),
                        brush, new Point(23, Height - ChartItemDefaultHeight));
                }
            }

            //draw the text
            if (!string.IsNullOrWhiteSpace(Text))
            {
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(93, 93, 93)))
                {
                    //calculate the appropriate font size to make sure the text will not elapsed on the control
                    AdjustFontSize(e.Graphics, Text);
                    SizeF textMeasurement = e.Graphics.MeasureString(Text, Font);

                    e.Graphics.DrawString(Text, Font, brush,
                        (ChartSize.Width - textMeasurement.Width) / 2, (ChartSize.Height - textMeasurement.Height) / 2);
                }
            }
        }

        //function to check if the mouse pointer is hovering an object
        private bool IsObjectHovered(Point mouseLocation, Point objLocation, Size objSize)
        {
            return mouseLocation.X > objLocation.X
                && mouseLocation.X < objLocation.X + objSize.Width
                && mouseLocation.Y > objLocation.Y
                && mouseLocation.Y < objLocation.Y + objSize.Height;
        }

        //method to calculate the font size and to make it sure it will not elapsed on the usercontrol
        private void AdjustFontSize(Graphics g, string textToDisplay)
        {
            //if font is too small set the default
            if (Font.Size < 12)
                Font = new Font(Font.FontFamily, 18, Font.Style);

            while (true)
            {
                SizeF size = g.MeasureString(textToDisplay, Font);

                if (size.Width >= Width - (CircleBrushWidth * 2))
                    Font = new Font(Font.FontFamily, Font.Size - 1, Font.Style);
                else
                    break;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //iterate on the charItems list
            foreach (ChartItem chartItem in ChartItems)
            {
                //update the font style if the item was hovered by the mouse
                if (IsObjectHovered(e.Location, chartItem.Location, new Size(Width, chartItem.Size.Height)))
                    chartItem.Font = new Font(chartItem.Font.FontFamily, chartItem.Font.Size, FontStyle.Bold);
                else
                    chartItem.Font = new Font(chartItem.Font.FontFamily, chartItem.Font.Size, FontStyle.Regular);

                //ENHANCEMENT ====> we added a feature where in we can highlight the chart and item by hovering the mouse on the slice
                
                //get the center coordinate of the circle and it's radius
                Point centerCircle = new Point(ChartSize.Width / 2, ChartSize.Height / 2);
                int radiusCircle = ChartSize.Width / 2;

                //explanation of the "if" statement below:
                //IsMouseInsideArc = check if the mouse pointer is inside the arc/slice
                //IsMouseInsideCircle = then we check if the mouse pointer is INSIDE the chart/circle ONLY
                //!IsMouseInsideCircle = lastly we check if the mouse pointer is not in the center part (the "Text" part) of the chart
                if (IsMouseInsideArc(e.Location, centerCircle, radiusCircle, chartItem.ChartStartAngle, chartItem.ChartEndAngle)
                    && IsMouseInsideCircle(centerCircle.X, centerCircle.Y, radiusCircle, e.X, e.Y)
                    && !IsMouseInsideCircle(centerCircle.X, centerCircle.Y, radiusCircle - CircleBrushWidth, e.X, e.Y))
                    chartItem.Font = new Font(chartItem.Font.FontFamily, chartItem.Font.Size, FontStyle.Bold);
            }

            //repaint the control
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            //BUGFIX ====> reset the font style when the mouse is not focusing in the control
            //iterate on the charItems list
            foreach (ChartItem chartItem in ChartItems)
            {
                chartItem.Font = new Font(chartItem.Font.FontFamily, chartItem.Font.Size, FontStyle.Regular);
            }

            //repaint the control
            Invalidate();
        }

        //ENHANCEMENT ======> supported the resizing of the control
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //recompute the new size
            StabilizeSize();

            //repaint the control
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            //recompute the new size
            StabilizeSize();

            //repaint the control
            Invalidate();
        }

        private void StabilizeSize()
        {
            //we will just make sure that width and height is proportionate
            int size = Math.Min(Width, Height);

            ChartSize = new Size(size, size);
            if (Slices.Count == 0)
                Size = new Size(size, size);
            else
            {
                Size = CalculateTotalSize();

                //reposition the indicators
                foreach (ChartItem chartItem in ChartItems)
                {
                    chartItem.Location = CalculatePositionOfIndicator(ChartItems.IndexOf(chartItem));
                }
            }
        }

        //function to calculate the total size of the control (chart, gap, indicators plus the total)
        private Size CalculateTotalSize()
        {
            return new Size(ChartSize.Width,
                ChartSize.Height + //original size
                EmptySpaceHeight + //size of the gap
                (ChartItemDefaultHeight * Slices.Count()) + //total size of the indicator
                ChartItemDefaultHeight); //caption "Total"
        }

        //function to calculate the position of the indicator
        private Point CalculatePositionOfIndicator(int zeroBasedIndex)
        {
            return new Point(5, ChartSize.Height + EmptySpaceHeight + (ChartItemDefaultHeight * zeroBasedIndex));
        }

        //algorithm from https://stackoverflow.com/ to check if the mouse is inside the arc or slice of a circle
        private bool IsMouseInsideArc(Point mouseLocation, Point centerCircle, int radiusCircle, int startingAngle, int endingAngle)
        {
            // A = is the angle of the mouse pointer with reference to the center of the circle
            // S = is the starting angle of the slice
            // E = is the ending angle of the slice

            //get the angle of the mouse pointer, referencing on the center of the circle, this will output in radians
            double A = Math.Atan2(mouseLocation.Y - centerCircle.Y, mouseLocation.X - centerCircle.X);
            //convert radians to degrees
            A = A* (180/Math.PI);
            //convert the +/-180 degrees into 0 to 360 degreees format
            A = A < 0 ? 360 + A : A;

            int S = startingAngle;
            int E = startingAngle + endingAngle; //get the actual ending angle with referencing from 0 degrees,
                                                 //since sweep angle (ending angle) is only referencing on the starting angle

            //since the drawing of slice starts with 0 degrees we can already comment the "else if" scenario
            if (S < E && S < A && A < E)
                return true;
            //else if (S > E)
            //{
            //    if (A > S)
            //        return true;
            //    else if (A < E)
            //        return true;
            //}

            return false;
        }

        //algorithm from https://stackoverflow.com/ to check if the mouse is inside the circle
        private bool IsMouseInsideCircle(int xCircle, int yCircle, int radiusCircle, int xMouse, int yMouse)
        {
            int dx = xCircle - xMouse;
            int dy = yCircle - yMouse;
            return dx * dx + dy * dy <= radiusCircle * radiusCircle;
        }

        //lets test
    }
}
