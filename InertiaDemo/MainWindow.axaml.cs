using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace InertiaDemo;

public class InertiaScroller
{
    private double velocity;
    private double position;
    private double decayRate;
    private double time;

    public InertiaScroller(double velocity, double decayRate)
    {
        this.velocity = velocity;
        this.decayRate = decayRate;
    }

    public double GetPosition(double elapsedTime)
    {
        time += elapsedTime;
        position += velocity * elapsedTime;
        velocity *= Math.Exp(-decayRate * time);

        return position;
    }

    public bool IsStopped()
    {
        return Math.Abs(velocity) < 0.01f;
    }
}

public class InertiaScroller2
{
    private double velocity;
    private double position;
    private double target;
    private double springConstant;
    private double dampingRatio;
    private double elapsedTime;

    public InertiaScroller2(double velocity, double springConstant, double dampingRatio)
    {
        this.velocity = velocity;
        this.position = 0;
        this.target = 0;
        this.springConstant = springConstant;
        this.dampingRatio = dampingRatio;
        this.elapsedTime = 0;
    }

    public double GetPosition(double elapsedTime)
    {
        if (Math.Abs(velocity) < 0.001f)
        {
            return position;
        }

        this.elapsedTime += elapsedTime;

        double angularFrequency = Math.Sqrt(springConstant);
        double dampingCoefficient = 2 * dampingRatio * angularFrequency;
        double exponentialDecay = Math.Exp(-dampingCoefficient * this.elapsedTime);
        double displacement = velocity / angularFrequency * exponentialDecay;
        position = target + displacement * Math.Sin(angularFrequency * this.elapsedTime);

        return position;
    }

    public bool IsStopped()
    {
        return Math.Abs(velocity) < 0.001f;
    }
}

/*
public class MyScrollViewer : ScrollViewer
{
    private InertiaScroller scroller;
    private Point lastMousePosition;
    private bool isDragging;

    public MyScrollViewer()
    {
        this.PointerPressed += OnPointerPressed;
        this.PointerMoved += OnPointerMoved;
        this.PointerReleased += OnPointerReleased;
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        isDragging = true;
        lastMousePosition = e.GetPosition(this);
        scroller = null;
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (isDragging)
        {
            var currentMousePosition = e.GetPosition(this);
            var delta = currentMousePosition - lastMousePosition;
            lastMousePosition = currentMousePosition;

            this.ScrollToHorizontalOffset(this.HorizontalOffset - delta.X);
            this.ScrollToVerticalOffset(this.VerticalOffset - delta.Y);
        }
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        isDragging = false;
        var velocity = e.GetVelocity(this);
        scroller = new InertiaScroller(velocity.X, 0.1f);

        this.InvalidateVisual();
    }

    protected override void OnRender(DrawingContext context)
    {
        base.OnRender(context);

        if (scroller != null && !scroller.IsStopped())
        {
            var delta = scroller.GetPosition((float)this.Clock.GetElapsedTime().TotalSeconds);
            this.ScrollToHorizontalOffset(this.HorizontalOffset + delta);
            this.InvalidateVisual();
        }
    }
}
*/

/*
public class MyCanvas : Canvas
{
    private InertiaScroller scroller;
    private Point lastMousePosition;
    private bool isDragging;

    public MyCanvas()
    {
        this.PointerPressed += OnPointerPressed;
        this.PointerMoved += OnPointerMoved;
        this.PointerReleased += OnPointerReleased;
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        isDragging = true;
        lastMousePosition = e.GetPosition(this);
        scroller = null;
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (isDragging)
        {
            var currentMousePosition = e.GetPosition(this);
            var delta = currentMousePosition - lastMousePosition;
            lastMousePosition = currentMousePosition;

            var rect = this.GetLayoutBounds();
            rect.X -= delta.X;
            rect.Y -= delta.Y;

            this.SetLayoutBounds(rect);
        }
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        isDragging = false;
        var velocity = e.GetVelocity(this);
        scroller = new InertiaScroller(velocity.X, 0.1f);

        this.InvalidateVisual();
    }

    protected override void OnRender(DrawingContext context)
    {
        base.OnRender(context);

        if (scroller != null && !scroller.IsStopped())
        {
            var delta = scroller.GetPosition((float)this.Clock.GetElapsedTime().TotalSeconds);
            var rect = this.GetLayoutBounds();
            rect.X -= delta;
            this.SetLayoutBounds(rect);

            this.InvalidateVisual();
        }
    }
}
*/

/*
public class InertiaGestureRecognizer : GestureRecognizer
{
    private InertiaScroller scroller;
    private Point lastMousePosition;
    private bool isDragging;
    private float decayRate = 0.1f;

    public InertiaGestureRecognizer()
    {
        this.PointerPressed += OnPointerPressed;
        this.PointerMoved += OnPointerMoved;
        this.PointerReleased += OnPointerReleased;
    }

    public float DecayRate
    {
        get { return decayRate; }
        set { decayRate = value; }
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        isDragging = true;
        lastMousePosition = e.GetPosition(e.Source);
        scroller = null;
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
        if (isDragging)
        {
            var currentMousePosition = e.GetPosition(e.Source);
            var delta = currentMousePosition - lastMousePosition;
            lastMousePosition = currentMousePosition;

            this.Delta = delta;
            this.RaiseDeltaChanged();
        }
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        isDragging = false;
        var velocity = e.GetVelocity(e.Source);
        scroller = new InertiaScroller(velocity.X, decayRate);
        this.RaiseInertiaStarted();
    }

    protected override void Update(IClock clock)
    {
        base.Update(clock);

        if (scroller != null && !scroller.IsStopped())
        {
            var delta = scroller.GetPosition((float)clock.GetElapsedTime().TotalSeconds);
            this.Delta = new Point(delta, 0);
            this.RaiseDeltaChanged();

            if (scroller.IsStopped())
            {
                this.RaiseInertiaStopped();
            }
        }
    }
}
*/

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
