// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

#region Using Directives



#endregion

namespace Microsoft.AzureCat.Samples.UserEmulator
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Windows.Forms;

    #region Declaration

    /// <summary>
    /// Represents the method that will handle a change in value.
    /// </summary>
    public delegate void ValueChangedHandler(object sender, decimal value);

    public enum BrushStyle
    {
        Solid,
        LinearGradient
    }

    public enum CustomBorderStyle
    {
        /// <summary>
        /// No border.
        /// </summary>
        None,

        /// <summary>
        /// A dashed border.
        /// </summary>
        Dashed, //from ButtonBorderStyle Enumeration

        /// <summary>
        /// A dotted-line border.
        /// </summary>
        Dotted, //from ButtonBorderStyle Enumeration

        /// <summary>
        /// A sunken border.
        /// </summary>
        Inset, //from ButtonBorderStyle Enumeration

        /// <summary>
        /// A raised border.
        /// </summary>
        Outset, //from ButtonBorderStyle Enumeration

        /// <summary>
        /// A solid border.
        /// </summary>
        Solid, //from ButtonBorderStyle Enumeration

        /// <summary>
        /// The border is drawn outside the specified rectangle, preserving the dimensions of the rectangle for drawing.
        /// </summary>
        Adjust, //from Border3DStyle Enumeration

        /// <summary>
        /// The inner and outer edges of the border have a raised appearance.
        /// </summary>
        Bump, //from Border3DStyle Enumeration

        /// <summary>
        /// The inner and outer edges of the border have an etched appearance.
        /// </summary>
        Etched, //from Border3DStyle Enumeration

        /// <summary>
        /// The border has no three-dimensional effects.
        /// </summary>
        Flat, //from Border3DStyle Enumeration

        /// <summary>
        /// The border has raised inner and outer edges.
        /// </summary>
        Raised, //from Border3DStyle Enumeration

        /// <summary>
        /// The border has a raised inner edge and no outer edge.
        /// </summary>
        RaisedInner, //from Border3DStyle Enumeration

        /// <summary>
        /// The border has a raised outer edge and no inner edge.
        /// </summary>
        RaisedOuter, //from Border3DStyle Enumeration

        /// <summary>
        /// The border has sunken inner and outer edges.
        /// </summary>
        Sunken, //from Border3DStyle Enumeration

        /// <summary>
        /// The border has a sunken inner edge and no outer edge.
        /// </summary>
        SunkenInner, //from Border3DStyle Enumeration

        /// <summary>
        /// The border has a sunken outer edge and no inner edge.
        /// </summary>
        SunkenOuter //from Border3DStyle Enumeration
    }

    #endregion

    [Description("CustomTrackBar represents an advanced track bar that is very better than the standard trackbar.")]
    [ToolboxBitmap(typeof(CustomTrackBar), "Editors.CustomTrackBar.CustomTrackBar.bmp")]
    [Designer(typeof(CustomTrackBarDesigner))]
    [DefaultProperty("Maximum")]
    [DefaultEvent("ValueChanged")]
    public class CustomTrackBar : Control
    {
        #region Public Contruction

        /// <summary>
        /// Constructor method of <see cref="CustomTrackBar"/> class
        /// </summary>
        public CustomTrackBar()
        {
            this.MouseDown += this.OnMouseDownSlider;
            this.MouseUp += this.OnMouseUpSlider;
            this.MouseMove += this.OnMouseMoveSlider;

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer |
                ControlStyles.SupportsTransparentBackColor,
                true);

            this.Font = new Font("Verdana", 8.25F, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
            this.ForeColor = Color.FromArgb(123, 125, 123);
            this.BackColor = Color.Transparent;

            this.tickColor = Color.FromArgb(148, 146, 148);
            this.tickHeight = 4;

            this.trackerColor = Color.FromArgb(24, 130, 198);
            this.trackerSize = new Size(16, 16);
            this.indentWidth = 6;
            this.indentHeight = 6;

            this.trackLineColor = Color.FromArgb(90, 93, 90);
            this.brushStyle = BrushStyle.LinearGradient;
            this.trackLineHeight = 3;

            this.borderStyle = CustomBorderStyle.None;
            this.borderColor = SystemColors.ActiveBorder;

            this.autoSize = true;
            this.Height = this.FitSize.Height;
        }

        #endregion

        #region Private Fields

        // Instance fields
        private int value = 0;
        private int minimum = 0;
        private int maximum = 10;

        private int largeChange = 2;
        private int smallChange = 1;

        private Orientation orientation = Orientation.Horizontal;

        private CustomBorderStyle borderStyle = CustomBorderStyle.None;
        private Color borderColor = SystemColors.ActiveBorder;

        private Size trackerSize = new Size(10, 20);
        private int indentWidth = 6;
        private int indentHeight = 6;

        private int tickHeight = 2;
        private int tickFrequency = 1;
        private Color tickColor = Color.Black;
        private TickStyle tickStyle = TickStyle.BottomRight;
        private TickStyle textTickStyle = TickStyle.BottomRight;

        private int trackLineHeight = 3;
        private Color trackLineColor = SystemColors.Control;
        private BrushStyle brushStyle = BrushStyle.LinearGradient;

        private Color trackerColor = SystemColors.Control;
        private RectangleF trackerRect = RectangleF.Empty;

        private bool autoSize = true;

        private bool leftButtonDown;
        private float mouseStartPos = -1;

        /// <summary>
        /// Occurs when the property Value has been changed.
        /// </summary>
        public event ValueChangedHandler ValueChanged;

        /// <summary>
        /// Occurs when either a mouse or keyboard action moves the slider.
        /// </summary>
        public event EventHandler Scroll;

        #endregion

        #region Public Properties

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (!this.autoSize)
            {
                return;
            }
            // Calculate the Position for children controls
            if (this.orientation == Orientation.Horizontal)
            {
                this.Height = this.FitSize.Height;
            }
            else
            {
                this.Width = this.FitSize.Width;
            }
            //=================================================
        }


        /// <summary>
        /// Gets or sets a value indicating whether the height or width of the track bar is being automatically sized.
        /// </summary>
        /// <remarks>You can set the AutoSize property to true to cause the track bar to adjust either its height or width, depending on orientation, to ensure that the control uses only the required amount of space.</remarks>
        /// <value>true if the track bar is being automatically sized; otherwise, false. The default is true.</value>
        [Category("Behavior")]
        [Description("Gets or sets the height of track line.")]
        [DefaultValue(true)]
        public new bool AutoSize
        {
            get { return this.autoSize; }

            set
            {
                if (this.autoSize != value)
                {
                    this.autoSize = value;
                    if (this.autoSize == true)
                    {
                        this.Size = this.FitSize;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the <see cref="Value"/> property when the slider is moved a large distance.
        /// </summary>
        /// <remarks>
        /// When the user presses the PAGE UP or PAGE DOWN key or clicks the track bar on either side of the slider, the <see cref="Value"/> 
        /// property changes according to the value set in the <see cref="LargeChange"/> property. 
        /// You might consider setting the <see cref="LargeChange"/> value to a percentage of the <see cref="Control.Height"/> (for a vertically oriented track bar) or 
        /// <see cref="Control.Width"/> (for a horizontally oriented track bar) values. This keeps the distance your track bar moves proportionate to its size.
        /// </remarks>
        /// <value>A numeric value. The default value is 2.</value>
        [Category("Behavior")]
        [Description("Gets or sets a value to be added to or subtracted from the Value property when the slider is moved a large distance.")]
        [DefaultValue(2)]
        public int LargeChange
        {
            get { return this.largeChange; }

            set
            {
                this.largeChange = value;
                if (this.largeChange < 1)
                {
                    this.largeChange = 1;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the <see cref="Value"/> property when the slider is moved a small distance.
        /// </summary>
        /// <remarks>
        /// When the user presses one of the arrow keys, the <see cref="Value"/> property changes according to the value set in the SmallChange property.
        /// You might consider setting the <see cref="SmallChange"/> value to a percentage of the <see cref="Control.Height"/> (for a vertically oriented track bar) or 
        /// <see cref="Control.Width"/> (for a horizontally oriented track bar) values. This keeps the distance your track bar moves proportionate to its size.
        /// </remarks>
        /// <value>A numeric value. The default value is 1.</value>
        [Category("Behavior")]
        [Description("Gets or sets a value to be added to or subtracted from the Value property when the slider is moved a small distance.")]
        [DefaultValue(1)]
        public int SmallChange
        {
            get { return this.smallChange; }

            set
            {
                this.smallChange = value;
                if (this.smallChange < 1)
                {
                    this.smallChange = 1;
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of track line.
        /// </summary>
        /// <value>The default value is 4.</value>
        [Category("Appearance")]
        [Description("Gets or sets the height of track line.")]
        [DefaultValue(4)]
        public int TrackLineHeight
        {
            get { return this.trackLineHeight; }

            set
            {
                if (this.trackLineHeight != value)
                {
                    this.trackLineHeight = value;
                    if (this.trackLineHeight < 1)
                    {
                        this.trackLineHeight = 1;
                    }

                    if (this.trackLineHeight > this.trackerSize.Height)
                    {
                        this.trackLineHeight = this.trackerSize.Height;
                    }

                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tick's <see cref="Color"/> of the control.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the tick's color of the control.")]
        public Color TickColor
        {
            get { return this.tickColor; }

            set
            {
                if (this.tickColor != value)
                {
                    this.tickColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the delta between ticks drawn on the control.
        /// </summary>
        /// <remarks>
        /// For a <see cref="CustomTrackBar"/> with a large range of values between the <see cref="Minimum"/> and the 
        /// <see cref="Maximum"/>, it might be impractical to draw all the ticks for values on the control. 
        /// For example, if you have a control with a range of 100, passing in a value of 
        /// five here causes the control to draw 20 ticks. In this case, each tick 
        /// represents five units in the range of values.
        /// </remarks>
        /// <value>The numeric value representing the delta between ticks. The default is 1.</value>
        [Category("Appearance")]
        [Description("Gets or sets a value that specifies the delta between ticks drawn on the control.")]
        [DefaultValue(1)]
        public int TickFrequency
        {
            get { return this.tickFrequency; }

            set
            {
                if (this.tickFrequency != value)
                {
                    this.tickFrequency = value;
                    if (this.tickFrequency < 1)
                    {
                        this.tickFrequency = 1;
                    }
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of tick.
        /// </summary>
        /// <value>The height of tick in pixels. The default value is 2.</value>
        [Category("Appearance")]
        [Description("Gets or sets the height of tick.")]
        [DefaultValue(6)]
        public int TickHeight
        {
            get { return this.tickHeight; }

            set
            {
                if (this.tickHeight != value)
                {
                    this.tickHeight = value;

                    if (this.tickHeight < 1)
                    {
                        this.tickHeight = 1;
                    }

                    if (this.autoSize == true)
                    {
                        this.Size = this.FitSize;
                    }

                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of indent (or Padding-Y).
        /// </summary>
        /// <value>The height of indent in pixels. The default value is 6.</value>
        [Category("Appearance")]
        [Description("Gets or sets the height of indent.")]
        [DefaultValue(2)]
        public int IndentHeight
        {
            get { return this.indentHeight; }

            set
            {
                if (this.indentHeight != value)
                {
                    this.indentHeight = value;
                    if (this.indentHeight < 0)
                    {
                        this.indentHeight = 0;
                    }

                    if (this.autoSize == true)
                    {
                        this.Size = this.FitSize;
                    }

                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of indent (or Padding-Y).
        /// </summary>
        /// <value>The width of indent in pixels. The default value is 6.</value>
        [Category("Appearance")]
        [Description("Gets or sets the width of indent.")]
        [DefaultValue(6)]
        public int IndentWidth
        {
            get { return this.indentWidth; }

            set
            {
                if (this.indentWidth != value)
                {
                    this.indentWidth = value;
                    if (this.indentWidth < 0)
                    {
                        this.indentWidth = 0;
                    }

                    if (this.autoSize == true)
                    {
                        this.Size = this.FitSize;
                    }

                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tracker's size. 
        /// The tracker's width must be greater or equal to tracker's height.
        /// </summary>
        /// <value>The <see cref="Size"/> object that represents the height and width of the tracker in pixels.</value>
        [Category("Appearance")]
        [Description("Gets or sets the tracker's size.")]
        public Size TrackerSize
        {
            get { return this.trackerSize; }

            set
            {
                if (this.trackerSize != value)
                {
                    this.trackerSize = value;
                    if (this.trackerSize.Width > this.trackerSize.Height)
                    {
                        this.trackerSize.Height = this.trackerSize.Width;
                    }

                    if (this.autoSize == true)
                    {
                        this.Size = this.FitSize;
                    }

                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the text tick style of the trackbar.
        /// There are 4 styles for selection: None, TopLeft, BottomRight, Both. 
        /// </summary>
        /// <remarks>You can use the <see cref="Control.Font"/>, <see cref="Control.ForeColor"/>
        /// properties to customize the tick text.</remarks>
        /// <value>One of the <see cref="TickStyle"/> values. The default is <b>BottomRight</b>.</value>
        [Category("Appearance")]
        [Description("Gets or sets the text tick style.")]
        [DefaultValue(TickStyle.BottomRight)]
        public TickStyle TextTickStyle
        {
            get { return this.textTickStyle; }

            set
            {
                if (this.textTickStyle != value)
                {
                    this.textTickStyle = value;

                    if (this.autoSize == true)
                    {
                        this.Size = this.FitSize;
                    }

                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the tick style of the trackbar.
        /// There are 4 styles for selection: None, TopLeft, BottomRight, Both. 
        /// </summary>
        /// <remarks>You can use the <see cref="TickColor"/>, <see cref="TickFrequency"/>, 
        /// <see cref="TickHeight"/> properties to customize the trackbar's ticks.</remarks>
        /// <value>One of the <see cref="TickStyle"/> values. The default is <b>BottomRight</b>.</value>
        [Category("Appearance")]
        [Description("Gets or sets the tick style.")]
        [DefaultValue(TickStyle.BottomRight)]
        public TickStyle TickStyle
        {
            get { return this.tickStyle; }

            set
            {
                if (this.tickStyle != value)
                {
                    this.tickStyle = value;

                    if (this.autoSize == true)
                    {
                        this.Size = this.FitSize;
                    }

                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or set tracker's color.
        /// </summary>
        /// <value>
        /// <remarks>You can change size of tracker by <see cref="TrackerSize"/> property.</remarks>
        /// A <see cref="Color"/> that represents the color of the tracker. 
        /// </value>
        [Description("Gets or set tracker's color.")]
        [Category("Appearance")]
        public Color TrackerColor
        {
            get { return this.trackerColor; }
            set
            {
                if (this.trackerColor != value)
                {
                    this.trackerColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets a numeric value that represents the current position of the slider on the track bar.
        /// </summary>
        /// <remarks>The Value property contains the number that represents the current position of the slider on the track bar.</remarks>
        /// <value>A numeric value that is within the <see cref="Minimum"/> and <see cref="Maximum"/> range. 
        /// The default value is 0.</value>
        [Description("The current value for the CustomTrackBar, in the range specified by the Minimum and Maximum properties.")]
        [Category("Behavior")]
        public int Value
        {
            get { return this.value; }
            set
            {
                if (this.value != value)
                {
                    if (value < this.minimum)
                    {
                        value = this.minimum;
                    }
                    else if (value > this.maximum)
                    {
                        value = this.maximum;
                    }
                    else
                    {
                        this.value = value;
                    }

                    this.OnValueChanged(value);

                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the lower limit of the range this <see cref="CustomTrackBar"/> is working with.
        /// </summary>
        /// <remarks>You can use the <see cref="SetRange"/> method to set both the <see cref="Maximum"/> and <see cref="Minimum"/> properties at the same time.</remarks>
        /// <value>The minimum value for the <see cref="CustomTrackBar"/>. The default value is 0.</value>
        [Description("The lower bound of the range this CustomTrackBar is working with.")]
        [Category("Behavior")]
        public int Minimum
        {
            get { return this.minimum; }
            set
            {
                this.minimum = value;

                if (this.minimum > this.maximum)
                {
                    this.maximum = this.minimum;
                }
                if (this.minimum > value)
                {
                    value = this.minimum;
                }

                if (this.autoSize == true)
                {
                    this.Size = this.FitSize;
                }
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the upper limit of the range this <see cref="CustomTrackBar"/> is working with.
        /// </summary>
        /// <remarks>You can use the <see cref="SetRange"/> method to set both the <see cref="Maximum"/> and <see cref="Minimum"/> properties at the same time.</remarks>
        /// <value>The maximum value for the <see cref="CustomTrackBar"/>. The default value is 10.</value>
        [Description("The uppper bound of the range this CustomTrackBar is working with.")]
        [Category("Behavior")]
        public int Maximum
        {
            get { return this.maximum; }
            set
            {
                this.maximum = value;

                if (this.maximum < value)
                {
                    value = this.maximum;
                }
                if (this.maximum < this.minimum)
                {
                    this.minimum = this.maximum;
                }

                if (this.autoSize == true)
                {
                    this.Size = this.FitSize;
                }
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the horizontal or vertical orientation of the track bar.
        /// </summary>
        /// <remarks>
        /// When the <b>Orientation</b> property is set to <b>Orientation.Horizontal</b>, 
        /// the slider moves from left to right as the <see cref="Value"/> increases. 
        /// When the <b>Orientation</b> property is set to <b>Orientation.Vertical</b>, the slider moves 
        /// from bottom to top as the <see cref="Value"/> increases.
        /// </remarks>
        /// <value>One of the <see cref="Orientation"/> values. The default value is <b>Horizontal</b>.</value>
        [Description("Gets or sets a value indicating the horizontal or vertical orientation of the track bar.")]
        [Category("Behavior")]
        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation
        {
            get { return this.orientation; }
            set
            {
                if (value != this.orientation)
                {
                    this.orientation = value;
                    if (this.orientation == Orientation.Horizontal)
                    {
                        if (this.Width < this.Height)
                        {
                            int temp = this.Width;
                            this.Width = this.Height;
                            this.Height = temp;
                        }
                    }
                    else //Vertical 
                    {
                        if (this.Width > this.Height)
                        {
                            int temp = this.Width;
                            this.Width = this.Height;
                            this.Height = temp;
                        }
                    }
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the border type of the trackbar control.
        /// </summary>
        /// <value>A <see cref="CustomBorderStyle"/> that represents the border type of the trackbar control. 
        /// The default is <b>CustomBorderStyle.None</b>.</value>
        [Description("Gets or sets the border type of the trackbar control.")]
        [Category("Appearance"), DefaultValue(typeof(CustomBorderStyle), "None")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public CustomBorderStyle BorderStyle
        {
            get { return this.borderStyle; }
            set
            {
                if (this.borderStyle != value)
                {
                    this.borderStyle = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color of the control.
        /// </summary>
        /// <value>A <see cref="Color"/> object that represents the border color of the control.</value>
        [Category("Appearance")]
        [Description("Gets or sets the border color of the control.")]
        public Color BorderColor
        {
            get { return this.borderColor; }
            set
            {
                if (value != this.borderColor)
                {
                    this.borderColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the track line.
        /// </summary>
        /// <value>A <see cref="Color"/> object that represents the color of the track line.</value>
        [Category("Appearance")]
        [Description("Gets or sets the color of the track line.")]
        public Color TrackLineColor
        {
            get { return this.trackLineColor; }
            set
            {
                if (value != this.trackLineColor)
                {
                    this.trackLineColor = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Gets or sets the brush style of the track line.
        /// </summary>
        /// <value>A <see cref="Color"/> object that represents the color of the track line.</value>
        [Category("Appearance")]
        [Description("Gets or sets the brush style of the track line.")]
        public BrushStyle TrackLineBrushStyle
        {
            get { return this.brushStyle; }
            set
            {
                if (value != this.brushStyle)
                {
                    this.brushStyle = value;
                    this.Invalidate();
                }
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the Size of area need for drawing.
        /// </summary>
        [Description("Gets the Size of area need for drawing.")]
        [Browsable(false)]
        private Size FitSize
        {
            get
            {
                try
                {
                    Size fitSize;
                    float textAreaSize;

                    // Create a Graphics object for the Control.
                    Graphics g = this.CreateGraphics();

                    Rectangle workingRect = Rectangle.Inflate(this.ClientRectangle, -this.indentWidth, -this.indentHeight);
                    float currentUsedPos = 0;

                    if (this.orientation == Orientation.Horizontal)
                    {
                        currentUsedPos = this.indentHeight;
                        //==========================================================================

                        // Get Height of Text Area
                        textAreaSize = g.MeasureString(this.maximum.ToString(), this.Font).Height;

                        if (this.textTickStyle == TickStyle.TopLeft || this.textTickStyle == TickStyle.Both)
                        {
                            currentUsedPos += textAreaSize;
                        }

                        if (this.tickStyle == TickStyle.TopLeft || this.tickStyle == TickStyle.Both)
                        {
                            currentUsedPos += this.tickHeight + 1;
                        }

                        currentUsedPos += this.trackerSize.Height;

                        if (this.tickStyle == TickStyle.BottomRight || this.tickStyle == TickStyle.Both)
                        {
                            currentUsedPos += 1;
                            currentUsedPos += this.tickHeight;
                        }

                        if (this.textTickStyle == TickStyle.BottomRight || this.textTickStyle == TickStyle.Both)
                        {
                            currentUsedPos += textAreaSize;
                        }

                        currentUsedPos += this.indentHeight;

                        fitSize = new Size(this.ClientRectangle.Width, (int) currentUsedPos);
                    }
                    else //_orientation == Orientation.Vertical
                    {
                        currentUsedPos = this.indentWidth;
                        //==========================================================================

                        // Get Width of Text Area
                        textAreaSize = g.MeasureString(this.maximum.ToString(), this.Font).Width;

                        if (this.textTickStyle == TickStyle.TopLeft || this.textTickStyle == TickStyle.Both)
                        {
                            currentUsedPos += textAreaSize;
                        }

                        if (this.tickStyle == TickStyle.TopLeft || this.tickStyle == TickStyle.Both)
                        {
                            currentUsedPos += this.tickHeight + 1;
                        }

                        currentUsedPos += this.trackerSize.Height;

                        if (this.tickStyle == TickStyle.BottomRight || this.tickStyle == TickStyle.Both)
                        {
                            currentUsedPos += 1;
                            currentUsedPos += this.tickHeight;
                        }

                        if (this.textTickStyle == TickStyle.BottomRight || this.textTickStyle == TickStyle.Both)
                        {
                            currentUsedPos += textAreaSize;
                        }

                        currentUsedPos += this.indentWidth;

                        fitSize = new Size((int) currentUsedPos, this.ClientRectangle.Height);
                    }

                    // Clean up the Graphics object.
                    g.Dispose();

                    return fitSize;
                }
                    // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                }
                return new Size(0, 0);
            }
        }


        /// <summary>
        /// Gets the rectangle containing the tracker.
        /// </summary>
        [Description("Gets the rectangle containing the tracker.")]
        private RectangleF TrackerRect
        {
            get
            {
                RectangleF trackerRect;
                float textAreaSize;

                // Create a Graphics object for the Control.
                Graphics g = this.CreateGraphics();

                Rectangle workingRect = Rectangle.Inflate(this.ClientRectangle, -this.indentWidth, -this.indentHeight);
                float currentUsedPos = 0;

                if (this.orientation == Orientation.Horizontal)
                {
                    currentUsedPos = this.indentHeight;
                    //==========================================================================

                    // Get Height of Text Area
                    textAreaSize = g.MeasureString(this.maximum.ToString(), this.Font).Height;

                    if (this.textTickStyle == TickStyle.TopLeft || this.textTickStyle == TickStyle.Both)
                    {
                        currentUsedPos += textAreaSize;
                    }

                    if (this.tickStyle == TickStyle.TopLeft || this.tickStyle == TickStyle.Both)
                    {
                        currentUsedPos += this.tickHeight + 1;
                    }


                    //==========================================================================
                    // Caculate the Tracker's rectangle
                    //==========================================================================
                    float currentTrackerPos;
                    if (this.maximum == this.minimum)
                    {
                        currentTrackerPos = workingRect.Left;
                    }
                    else
                    {
                        currentTrackerPos = (workingRect.Width - this.trackerSize.Width)*(this.value - this.minimum)/(this.maximum - this.minimum) +
                                            workingRect.Left;
                    }
                    trackerRect = new RectangleF(currentTrackerPos, currentUsedPos, this.trackerSize.Width, this.trackerSize.Height);
                        // Remember this for drawing the Tracker later
                    trackerRect.Inflate(0, -1);
                }
                else //_orientation == Orientation.Vertical
                {
                    currentUsedPos = this.indentWidth;
                    //==========================================================================

                    // Get Width of Text Area
                    textAreaSize = g.MeasureString(this.maximum.ToString(), this.Font).Width;

                    if (this.textTickStyle == TickStyle.TopLeft || this.textTickStyle == TickStyle.Both)
                    {
                        currentUsedPos += textAreaSize;
                    }

                    if (this.tickStyle == TickStyle.TopLeft || this.tickStyle == TickStyle.Both)
                    {
                        currentUsedPos += this.tickHeight + 1;
                    }

                    //==========================================================================
                    // Caculate the Tracker's rectangle
                    //==========================================================================
                    float currentTrackerPos;
                    if (this.maximum == this.minimum)
                    {
                        currentTrackerPos = workingRect.Top;
                    }
                    else
                    {
                        currentTrackerPos = (workingRect.Height - this.trackerSize.Width)*(this.value - this.minimum)/(this.maximum - this.minimum);
                    }

                    trackerRect = new RectangleF(
                        currentUsedPos,
                        workingRect.Bottom - currentTrackerPos - this.trackerSize.Width,
                        this.trackerSize.Height,
                        this.trackerSize.Width); // Remember this for drawing the Tracker later
                    trackerRect.Inflate(-1, 0);
                }

                // Clean up the Graphics object.
                g.Dispose();

                return trackerRect;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="value">The new value</param>
        public virtual void OnValueChanged(int value)
        {
            // Any attached event handlers?
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, value);
            }
        }

        /// <summary>
        /// Raises the Scroll event.
        /// </summary>
        public virtual void OnScroll()
        {
            try
            {
                // Any attached event handlers?
                if (this.Scroll != null)
                {
                    this.Scroll(this, new EventArgs());
                }
            }
            catch (Exception Err)
            {
                MessageBox.Show("OnScroll Exception: " + Err.Message);
            }
        }


        /// <summary>
        /// Call the Increment() method to increase the value displayed by an integer you specify 
        /// </summary>
        /// <param name="value"></param>
        public void Increment(int value)
        {
            if (value < this.maximum)
            {
                value += value;
                if (value > this.maximum)
                {
                    value = this.maximum;
                }
            }
            else
            {
                value = this.maximum;
            }

            this.OnValueChanged(value);
            this.Invalidate();
        }

        /// <summary>
        /// Call the Decrement() method to decrease the value displayed by an integer you specify 
        /// </summary>
        /// <param name="value"> The value to decrement</param>
        public void Decrement(int value)
        {
            if (value > this.minimum)
            {
                value -= value;
                if (value < this.minimum)
                {
                    value = this.minimum;
                }
            }
            else
            {
                value = this.minimum;
            }

            this.OnValueChanged(value);
            this.Invalidate();
        }

        /// <summary>
        /// Sets the minimum and maximum values for a TrackBar.
        /// </summary>
        /// <param name="minValue">The lower limit of the range of the track bar.</param>
        /// <param name="maxValue">The upper limit of the range of the track bar.</param>
        public void SetRange(int minValue, int maxValue)
        {
            this.minimum = minValue;

            if (this.minimum > this.value)
            {
                this.value = this.minimum;
            }

            this.maximum = maxValue;

            if (this.maximum < this.value)
            {
                this.value = this.maximum;
            }
            if (this.maximum < this.minimum)
            {
                this.minimum = this.maximum;
            }

            this.Invalidate();
        }

        /// <summary>
        /// Reset the appearance properties.
        /// </summary>
        public void ResetAppearance()
        {
            this.Font = new Font("Verdana", 8.25F, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
            this.ForeColor = Color.FromArgb(123, 125, 123);
            this.BackColor = Color.Transparent;

            this.tickColor = Color.FromArgb(148, 146, 148);
            this.tickHeight = 4;

            this.trackerColor = Color.FromArgb(24, 130, 198);
            this.trackerSize = new Size(16, 16);
            //_trackerRect.Size = _trackerSize;

            this.indentWidth = 6;
            this.indentHeight = 6;

            this.trackLineColor = Color.FromArgb(90, 93, 90);
            this.trackLineHeight = 3;

            this.borderStyle = CustomBorderStyle.None;
            this.borderColor = SystemColors.ActiveBorder;

            //==========================================================================

            if (this.autoSize == true)
            {
                this.Size = this.FitSize;
            }
            this.Invalidate();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// The OnCreateControl method is called when the control is first created.
        /// </summary>
        protected override void OnCreateControl()
        {
        }

        /// <summary>
        /// This member overrides <see cref="Control.OnLostFocus">Control.OnLostFocus</see>.
        /// </summary>
        protected override void OnLostFocus(EventArgs e)
        {
            this.Invalidate();
            base.OnLostFocus(e);
        }

        /// <summary>
        /// This member overrides <see cref="Control.OnGotFocus">Control.OnGotFocus</see>.
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            this.Invalidate();
            base.OnGotFocus(e);
        }

        /// <summary>
        /// This member overrides <see cref="Control.OnClick">Control.OnClick</see>.
        /// </summary>
        protected override void OnClick(EventArgs e)
        {
            this.Focus();
            this.Invalidate();
            base.OnClick(e);
        }

        /// <summary>
        /// This member overrides <see cref="Control.ProcessCmdKey">Control.ProcessCmdKey</see>.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool blResult = true;

            /// <summary>
            /// Specified WM_KEYDOWN enumeration value.
            /// </summary>
            const int WM_KEYDOWN = 0x0100;

            /// <summary>
            /// Specified WM_SYSKEYDOWN enumeration value.
            /// </summary>
            const int WM_SYSKEYDOWN = 0x0104;


            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                switch (keyData)
                {
                    case Keys.Left:
                    case Keys.Down:
                        this.Decrement(this.smallChange);
                        break;
                    case Keys.Right:
                    case Keys.Up:
                        this.Increment(this.smallChange);
                        break;

                    case Keys.PageUp:
                        this.Increment(this.largeChange);
                        break;
                    case Keys.PageDown:
                        this.Decrement(this.largeChange);
                        break;

                    case Keys.Home:
                        this.Value = this.maximum;
                        break;
                    case Keys.End:
                        this.Value = this.minimum;
                        break;

                    default:
                        blResult = base.ProcessCmdKey(ref msg, keyData);
                        break;
                }
            }

            return blResult;
        }

        /// <summary>
        /// Dispose of instance resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #region Painting Methods

        /// <summary>
        /// This member overrides <see cref="Control.OnPaint">Control.OnPaint</see>.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Brush brush;
            RectangleF rectTemp, drawRect;
            float textAreaSize;

            Rectangle workingRect = Rectangle.Inflate(this.ClientRectangle, -this.indentWidth, -this.indentHeight);
            float currentUsedPos = 0;

            //==========================================================================
            // Draw the background of the ProgressBar control.
            //==========================================================================
            brush = new SolidBrush(this.BackColor);
            rectTemp = this.ClientRectangle;
            e.Graphics.FillRectangle(brush, rectTemp);
            brush.Dispose();
            //==========================================================================

            //==========================================================================
            if (this.orientation == Orientation.Horizontal)
            {
                currentUsedPos = this.indentHeight;
                //==========================================================================

                // Get Height of Text Area
                textAreaSize = e.Graphics.MeasureString(this.maximum.ToString(), this.Font).Height;

                if (this.textTickStyle == TickStyle.TopLeft || this.textTickStyle == TickStyle.Both)
                {
                    //==========================================================================
                    // Draw the 1st Text Line.
                    //==========================================================================
                    drawRect = new RectangleF(workingRect.Left, currentUsedPos, workingRect.Width, textAreaSize);
                    drawRect.Inflate(-this.trackerSize.Width/2, 0);
                    currentUsedPos += textAreaSize;

                    this.DrawTickTextLine(e.Graphics, drawRect, this.tickFrequency, this.minimum, this.maximum, this.ForeColor, this.Font, this.orientation);
                    //==========================================================================
                }

                if (this.tickStyle == TickStyle.TopLeft || this.tickStyle == TickStyle.Both)
                {
                    //==========================================================================
                    // Draw the 1st Tick Line.
                    //==========================================================================
                    drawRect = new RectangleF(workingRect.Left, currentUsedPos, workingRect.Width, this.tickHeight);
                    drawRect.Inflate(-this.trackerSize.Width/2, 0);
                    currentUsedPos += this.tickHeight + 1;

                    this.DrawTickLine(e.Graphics, drawRect, this.tickFrequency, this.minimum, this.maximum, this.tickColor, this.orientation);
                    //==========================================================================
                }

                //==========================================================================
                // Caculate the Tracker's rectangle
                //==========================================================================
                float currentTrackerPos;
                if (this.maximum == this.minimum)
                {
                    currentTrackerPos = workingRect.Left;
                }
                else
                {
                    currentTrackerPos = (workingRect.Width - this.trackerSize.Width)*(this.value - this.minimum)/(this.maximum - this.minimum) +
                                        workingRect.Left;
                }
                this.trackerRect = new RectangleF(currentTrackerPos, currentUsedPos, this.trackerSize.Width, this.trackerSize.Height);
                    // Remember this for drawing the Tracker later
                //_trackerRect.Inflate(0,-1);

                //==========================================================================
                // Draw the Track Line
                //==========================================================================
                drawRect = new RectangleF(
                    workingRect.Left,
                    currentUsedPos + this.trackerSize.Height/2 - this.trackLineHeight/2,
                    workingRect.Width,
                    this.trackLineHeight);
                this.DrawTrackLine(e.Graphics, drawRect);
                currentUsedPos += this.trackerSize.Height;


                //==========================================================================

                if (this.tickStyle == TickStyle.BottomRight || this.tickStyle == TickStyle.Both)
                {
                    //==========================================================================
                    // Draw the 2st Tick Line.
                    //==========================================================================
                    currentUsedPos += 1;
                    drawRect = new RectangleF(workingRect.Left, currentUsedPos, workingRect.Width, this.tickHeight);
                    drawRect.Inflate(-this.trackerSize.Width/2, 0);
                    currentUsedPos += this.tickHeight;

                    this.DrawTickLine(e.Graphics, drawRect, this.tickFrequency, this.minimum, this.maximum, this.tickColor, this.orientation);
                    //==========================================================================
                }

                if (this.textTickStyle == TickStyle.BottomRight || this.textTickStyle == TickStyle.Both)
                {
                    //==========================================================================
                    // Draw the 2st Text Line.
                    //==========================================================================
                    // Get Height of Text Area
                    drawRect = new RectangleF(workingRect.Left, currentUsedPos, workingRect.Width, textAreaSize);
                    drawRect.Inflate(-this.trackerSize.Width/2, 0);
                    currentUsedPos += textAreaSize;

                    this.DrawTickTextLine(e.Graphics, drawRect, this.tickFrequency, this.minimum, this.maximum, this.ForeColor, this.Font, this.orientation);
                    //==========================================================================
                }
            }
            else //_orientation == Orientation.Vertical
            {
                currentUsedPos = this.indentWidth;
                //==========================================================================

                // Get Width of Text Area
                textAreaSize = e.Graphics.MeasureString(this.maximum.ToString(), this.Font).Width;

                if (this.textTickStyle == TickStyle.TopLeft || this.textTickStyle == TickStyle.Both)
                {
                    //==========================================================================
                    // Draw the 1st Text Line.
                    //==========================================================================
                    // Get Height of Text Area
                    drawRect = new RectangleF(currentUsedPos, workingRect.Top, textAreaSize, workingRect.Height);
                    drawRect.Inflate(0, -this.trackerSize.Width/2);
                    currentUsedPos += textAreaSize;

                    this.DrawTickTextLine(e.Graphics, drawRect, this.tickFrequency, this.minimum, this.maximum, this.ForeColor, this.Font, this.orientation);
                    //==========================================================================
                }

                if (this.tickStyle == TickStyle.TopLeft || this.tickStyle == TickStyle.Both)
                {
                    //==========================================================================
                    // Draw the 1st Tick Line.
                    //==========================================================================
                    drawRect = new RectangleF(currentUsedPos, workingRect.Top, this.tickHeight, workingRect.Height);
                    drawRect.Inflate(0, -this.trackerSize.Width/2);
                    currentUsedPos += this.tickHeight + 1;

                    this.DrawTickLine(e.Graphics, drawRect, this.tickFrequency, this.minimum, this.maximum, this.tickColor, this.orientation);
                    //==========================================================================
                }

                //==========================================================================
                // Caculate the Tracker's rectangle
                //==========================================================================
                float currentTrackerPos;
                if (this.maximum == this.minimum)
                {
                    currentTrackerPos = workingRect.Top;
                }
                else
                {
                    currentTrackerPos = (workingRect.Height - this.trackerSize.Width)*(this.value - this.minimum)/(this.maximum - this.minimum);
                }

                this.trackerRect = new RectangleF(
                    currentUsedPos,
                    workingRect.Bottom - currentTrackerPos - this.trackerSize.Width,
                    this.trackerSize.Height,
                    this.trackerSize.Width); // Remember this for drawing the Tracker later
                //_trackerRect.Inflate(-1,0);

                rectTemp = this.trackerRect; //Testing

                //==========================================================================
                // Draw the Track Line
                //==========================================================================
                drawRect = new RectangleF(
                    currentUsedPos + this.trackerSize.Height/2 - this.trackLineHeight/2,
                    workingRect.Top,
                    this.trackLineHeight,
                    workingRect.Height);
                this.DrawTrackLine(e.Graphics, drawRect);
                currentUsedPos += this.trackerSize.Height;
                //==========================================================================

                if (this.tickStyle == TickStyle.BottomRight || this.tickStyle == TickStyle.Both)
                {
                    //==========================================================================
                    // Draw the 2st Tick Line.
                    //==========================================================================
                    currentUsedPos += 1;
                    drawRect = new RectangleF(currentUsedPos, workingRect.Top, this.tickHeight, workingRect.Height);
                    drawRect.Inflate(0, -this.trackerSize.Width/2);
                    currentUsedPos += this.tickHeight;

                    this.DrawTickLine(e.Graphics, drawRect, this.tickFrequency, this.minimum, this.maximum, this.tickColor, this.orientation);
                    //==========================================================================
                }

                if (this.textTickStyle == TickStyle.BottomRight || this.textTickStyle == TickStyle.Both)
                {
                    //==========================================================================
                    // Draw the 2st Text Line.
                    //==========================================================================
                    // Get Height of Text Area
                    drawRect = new RectangleF(currentUsedPos, workingRect.Top, textAreaSize, workingRect.Height);
                    drawRect.Inflate(0, -this.trackerSize.Width/2);
                    currentUsedPos += textAreaSize;

                    this.DrawTickTextLine(e.Graphics, drawRect, this.tickFrequency, this.minimum, this.maximum, this.ForeColor, this.Font, this.orientation);
                    //==========================================================================
                }
            }

            //==========================================================================
            // Check for special values of Max, Min & Value
            if (this.maximum == this.minimum)
            {
                // Draw border only and exit;
                this.DrawBorder(e.Graphics);
                return;
            }
            //==========================================================================

            //==========================================================================
            // Draw the Tracker
            //==========================================================================
            this.DrawTracker(e.Graphics, this.trackerRect);
            //==========================================================================

            // Draw border
            this.DrawBorder(e.Graphics);
            //==========================================================================

            // Draws a focus rectangle
            //if(Focused && BackColor != Color.Transparent)
            if (this.Focused)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, Rectangle.Inflate(this.ClientRectangle, -2, -2));
            }
            //==========================================================================
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="drawRect"></param>
        private void DrawTrackLine(Graphics g, RectangleF drawRect)
        {
            CustomStyleHelper.DrawAquaPillSingleLayer(g, drawRect, this.trackLineColor, this.orientation, this.brushStyle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="trackerRect"></param>
        private void DrawTracker(Graphics g, RectangleF trackerRect)
        {
            CustomStyleHelper.DrawAquaPill(g, trackerRect, this.trackerColor, this.orientation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="drawRect"></param>
        /// <param name="tickFrequency"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="foreColor"></param>
        /// <param name="font"></param>
        /// <param name="orientation"></param>
        private void DrawTickTextLine(
            Graphics g, RectangleF drawRect, int tickFrequency, int minimum, int maximum, Color foreColor, Font font, Orientation orientation)
        {
            //Check input value
            if (maximum == minimum)
            {
                return;
            }

            //Caculate tick number
            int tickCount = (int) ((maximum - minimum)/tickFrequency);
            if ((maximum - minimum)%tickFrequency == 0)
            {
                tickCount -= 1;
            }

            //Prepare for drawing Text
            //===============================================================
            StringFormat stringFormat;
            stringFormat = new StringFormat();
            stringFormat.FormatFlags = StringFormatFlags.NoWrap;
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.Trimming = StringTrimming.EllipsisCharacter;
            stringFormat.HotkeyPrefix = HotkeyPrefix.Show;

            Brush brush = new SolidBrush(foreColor);
            string text;
            float tickFrequencySize;
            //===============================================================

            if (orientation == Orientation.Horizontal)
            {
                // Calculate tick's setting
                tickFrequencySize = drawRect.Width*tickFrequency/(maximum - minimum);

                //===============================================================

                // Draw each tick text
                for (int i = 0; i <= tickCount; i++)
                {
                    text = Convert.ToString(minimum + tickFrequency*i, 10);
                    g.DrawString(text, font, brush, drawRect.Left + tickFrequencySize*i, drawRect.Top + drawRect.Height/2, stringFormat);
                }
                // Draw last tick text at Maximum
                text = Convert.ToString(maximum, 10);
                g.DrawString(text, font, brush, drawRect.Right, drawRect.Top + drawRect.Height/2, stringFormat);

                //===============================================================
            }
            else //Orientation.Vertical
            {
                // Calculate tick's setting
                tickFrequencySize = drawRect.Height*tickFrequency/(maximum - minimum);
                //===============================================================

                // Draw each tick text
                for (int i = 0; i <= tickCount; i++)
                {
                    text = Convert.ToString(minimum + tickFrequency*i, 10);
                    g.DrawString(text, font, brush, drawRect.Left + drawRect.Width/2, drawRect.Bottom - tickFrequencySize*i, stringFormat);
                }
                // Draw last tick text at Maximum
                text = Convert.ToString(maximum, 10);
                g.DrawString(text, font, brush, drawRect.Left + drawRect.Width/2, drawRect.Top, stringFormat);
                //===============================================================
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="drawRect"></param>
        /// <param name="tickFrequency"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="tickColor"></param>
        /// <param name="orientation"></param>
        private void DrawTickLine(Graphics g, RectangleF drawRect, int tickFrequency, int minimum, int maximum, Color tickColor, Orientation orientation)
        {
            //Check input value
            if (maximum == minimum)
            {
                return;
            }

            //Create the Pen for drawing Ticks
            Pen pen = new Pen(tickColor, 1);
            float tickFrequencySize;

            //Caculate tick number
            int tickCount = (int) ((maximum - minimum)/tickFrequency);
            if ((maximum - minimum)%tickFrequency == 0)
            {
                tickCount -= 1;
            }

            if (orientation == Orientation.Horizontal)
            {
                // Calculate tick's setting
                tickFrequencySize = drawRect.Width*tickFrequency/(maximum - minimum);

                //===============================================================

                // Draw each tick
                for (int i = 0; i <= tickCount; i++)
                {
                    g.DrawLine(pen, drawRect.Left + tickFrequencySize*i, drawRect.Top, drawRect.Left + tickFrequencySize*i, drawRect.Bottom);
                }
                // Draw last tick at Maximum
                g.DrawLine(pen, drawRect.Right, drawRect.Top, drawRect.Right, drawRect.Bottom);
                //===============================================================
            }
            else //Orientation.Vertical
            {
                // Calculate tick's setting
                tickFrequencySize = drawRect.Height*tickFrequency/(maximum - minimum);
                //===============================================================

                // Draw each tick
                for (int i = 0; i <= tickCount; i++)
                {
                    g.DrawLine(pen, drawRect.Left, drawRect.Bottom - tickFrequencySize*i, drawRect.Right, drawRect.Bottom - tickFrequencySize*i);
                }
                // Draw last tick at Maximum
                g.DrawLine(pen, drawRect.Left, drawRect.Top, drawRect.Right, drawRect.Top);
                //===============================================================
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        private void DrawBorder(Graphics g)
        {
            switch (this.borderStyle)
            {
                case CustomBorderStyle.Dashed: //from ButtonBorderStyle Enumeration
                    ControlPaint.DrawBorder(g, this.ClientRectangle, this.borderColor, ButtonBorderStyle.Dashed);
                    break;
                case CustomBorderStyle.Dotted: //from ButtonBorderStyle Enumeration
                    ControlPaint.DrawBorder(g, this.ClientRectangle, this.borderColor, ButtonBorderStyle.Dotted);
                    break;
                case CustomBorderStyle.Inset: //from ButtonBorderStyle Enumeration
                    ControlPaint.DrawBorder(g, this.ClientRectangle, this.borderColor, ButtonBorderStyle.Inset);
                    break;
                case CustomBorderStyle.Outset: //from ButtonBorderStyle Enumeration
                    ControlPaint.DrawBorder(g, this.ClientRectangle, this.borderColor, ButtonBorderStyle.Outset);
                    break;
                case CustomBorderStyle.Solid: //from ButtonBorderStyle Enumeration
                    ControlPaint.DrawBorder(g, this.ClientRectangle, this.borderColor, ButtonBorderStyle.Solid);
                    break;

                case CustomBorderStyle.Adjust: //from Border3DStyle Enumeration
                    ControlPaint.DrawBorder3D(g, this.ClientRectangle, Border3DStyle.Adjust);
                    break;
                case CustomBorderStyle.Bump: //from Border3DStyle Enumeration
                    ControlPaint.DrawBorder3D(g, this.ClientRectangle, Border3DStyle.Bump);
                    break;
                case CustomBorderStyle.Etched: //from Border3DStyle Enumeration
                    ControlPaint.DrawBorder3D(g, this.ClientRectangle, Border3DStyle.Etched);
                    break;
                case CustomBorderStyle.Flat: //from Border3DStyle Enumeration
                    ControlPaint.DrawBorder3D(g, this.ClientRectangle, Border3DStyle.Flat);
                    break;
                case CustomBorderStyle.Raised: //from Border3DStyle Enumeration
                    ControlPaint.DrawBorder3D(g, this.ClientRectangle, Border3DStyle.Raised);
                    break;
                case CustomBorderStyle.RaisedInner: //from Border3DStyle Enumeration
                    ControlPaint.DrawBorder3D(g, this.ClientRectangle, Border3DStyle.RaisedInner);
                    break;
                case CustomBorderStyle.RaisedOuter: //from Border3DStyle Enumeration
                    ControlPaint.DrawBorder3D(g, this.ClientRectangle, Border3DStyle.RaisedOuter);
                    break;
                case CustomBorderStyle.Sunken: //from Border3DStyle Enumeration
                    ControlPaint.DrawBorder3D(g, this.ClientRectangle, Border3DStyle.Sunken);
                    break;
                case CustomBorderStyle.SunkenInner: //from Border3DStyle Enumeration
                    ControlPaint.DrawBorder3D(g, this.ClientRectangle, Border3DStyle.SunkenInner);
                    break;
                case CustomBorderStyle.SunkenOuter: //from Border3DStyle Enumeration
                    ControlPaint.DrawBorder3D(g, this.ClientRectangle, Border3DStyle.SunkenOuter);
                    break;
                case CustomBorderStyle.None:
                default:
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void OnMouseDownSlider(object sender, MouseEventArgs e)
        {
            int offsetValue = 0;
            int oldValue = 0;
            PointF currentPoint;

            currentPoint = new PointF(e.X, e.Y);
            if (this.trackerRect.Contains(currentPoint))
            {
                if (!this.leftButtonDown)
                {
                    this.leftButtonDown = true;
                    this.Capture = true;
                    switch (this.orientation)
                    {
                        case Orientation.Horizontal:
                            this.mouseStartPos = currentPoint.X - this.trackerRect.X;
                            break;

                        case Orientation.Vertical:
                            this.mouseStartPos = currentPoint.Y - this.trackerRect.Y;
                            break;
                    }
                }
            }
            else
            {
                switch (this.orientation)
                {
                    case Orientation.Horizontal:
                        if (currentPoint.X + this.trackerSize.Width/2 >= this.Width - this.indentWidth)
                        {
                            offsetValue = this.maximum - this.minimum;
                        }
                        else if (currentPoint.X - this.trackerSize.Width/2 <= this.indentWidth)
                        {
                            offsetValue = 0;
                        }
                        else
                        {
                            offsetValue =
                                (int)
                                    (((currentPoint.X - this.indentWidth - this.trackerSize.Width/2)*(this.maximum - this.minimum))/
                                     (this.Width - 2*this.indentWidth - this.trackerSize.Width) + 0.5);
                        }

                        break;

                    case Orientation.Vertical:
                        if (currentPoint.Y + this.trackerSize.Width/2 >= this.Height - this.indentHeight)
                        {
                            offsetValue = 0;
                        }
                        else if (currentPoint.Y - this.trackerSize.Width/2 <= this.indentHeight)
                        {
                            offsetValue = this.maximum - this.minimum;
                        }
                        else
                        {
                            offsetValue =
                                (int)
                                    (((this.Height - currentPoint.Y - this.indentHeight - this.trackerSize.Width/2)*(this.maximum - this.minimum))/
                                     (this.Height - 2*this.indentHeight - this.trackerSize.Width) + 0.5);
                        }

                        break;

                    default:
                        break;
                }

                oldValue = this.value;
                this.value = this.minimum + offsetValue;
                this.Invalidate();

                if (oldValue != this.value)
                {
                    this.OnScroll();
                    this.OnValueChanged(this.value);
                }
            }
        }

        private void OnMouseUpSlider(object sender, MouseEventArgs e)
        {
            this.leftButtonDown = false;
            this.Capture = false;
        }

        private void OnMouseMoveSlider(object sender, MouseEventArgs e)
        {
            int offsetValue = 0;
            int oldValue = 0;
            PointF currentPoint;

            currentPoint = new PointF(e.X, e.Y);

            if (this.leftButtonDown)
            {
                try
                {
                    switch (this.orientation)
                    {
                        case Orientation.Horizontal:
                            if ((currentPoint.X + this.trackerSize.Width - this.mouseStartPos) >= this.Width - this.indentWidth)
                            {
                                offsetValue = this.maximum - this.minimum;
                            }
                            else if (currentPoint.X - this.mouseStartPos <= this.indentWidth)
                            {
                                offsetValue = 0;
                            }
                            else
                            {
                                offsetValue =
                                    (int)
                                        (((currentPoint.X - this.mouseStartPos - this.indentWidth)*(this.maximum - this.minimum))/
                                         (this.Width - 2*this.indentWidth - this.trackerSize.Width) + 0.5);
                            }

                            break;

                        case Orientation.Vertical:
                            if (currentPoint.Y + this.trackerSize.Width/2 >= this.Height - this.indentHeight)
                            {
                                offsetValue = 0;
                            }
                            else if (currentPoint.Y + this.trackerSize.Width/2 <= this.indentHeight)
                            {
                                offsetValue = this.maximum - this.minimum;
                            }
                            else
                            {
                                offsetValue =
                                    (int)
                                        (((this.Height - currentPoint.Y + this.trackerSize.Width/2 - this.mouseStartPos - this.indentHeight)*
                                          (this.maximum - this.minimum))/(this.Height - 2*this.indentHeight) + 0.5);
                            }

                            break;
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    oldValue = this.value;
                    this.Value = this.minimum + offsetValue;
                    this.Invalidate();

                    if (oldValue != this.value)
                    {
                        this.OnScroll();
                        this.OnValueChanged(this.value);
                    }
                }
            }
        }

        #endregion
    }
}