using System.Drawing;

namespace MainApplication
{
    //This class will be used to render/draw the chart, indicator and text in the control
    public class ChartItem
    {
        public Slice Slice { get; set; }
        public double Percentage { get; set; } //percentage in the chart of the value
        public Color Color { get; set; } //color of the slice/indicator
        public Size Size { get; set; } //size of the indicator/text
        public Point Location { get; set; } //location of the indicator/text
        public Font Font { get; set; } //font of the indicator/text
        public int ChartStartAngle { get; set; } //calculated angle of the chart slice
        public int ChartEndAngle { get; set; } //calculated angle of the chart slice
    }
}
