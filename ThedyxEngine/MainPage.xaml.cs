using System.Timers;
using ThedyxEngine.Engine;
using ThedyxEngine.UI;
using ThedyxEngine.Util;
using Timer = System.Timers.Timer;

namespace ThedyxEngine;

/**
 * \class MainPage
 * \brief Main page of the app, that manages all ui components
 */
public partial class MainPage {
    private readonly Timer _updateTimer;
    private bool _objectsChanged;
    private readonly Canvas _canvas;

    private string _currentError = "";
    private PointF _lastTouchPoint = new PointF(-1, -1);
    public bool IsDrawing { get; set; }
    public MainPage() {
        InitializeComponent();
        log4net.Config.XmlConfigurator.Configure();

        // init engine(proccessor of the app with entity of main window)
        Engine.Engine.Init(this);

        GlobalVariables.WindowRefreshRate = SystemInfo.GetRefreshRate();
        _updateTimer = new Timer
        {
            Interval = 1000.0 / GlobalVariables.WindowRefreshRate // Interval in milliseconds
        };
        _updateTimer.Elapsed += UpdateTimer_Elapsed;
        _updateTimer.Start();
        ObjectsChanged();

        ObjectsList.OnSelectedObjectChanged = SelectedObjectChanged;
        ControlPanel.DeleteSelected = SelectedObjectChanged;
        ControlPanel.UpdateUI = UpdateAll;
        ControlPanel.MainPage = this;
        ObjectsList.OnDeleteObject = UpdateAllAfterChangeProperties;
        ObjectsList.OnZoomToObject = ZoomToObject;
        ControlPanel.EngineModeChanged = EngineModeChanged;
        ControlPanel.UpdateUI = UpdateAll;
        _canvas = new Canvas(this);
        ControlPanel.ZoomChanged = d => _canvas.Zoom(d);
        EngineGraphicsView.Drawable = _canvas;
        EngineGraphicsView.BackgroundColor = Colors.White;
        TabProperties.OnObjectChange = UpdateAll;
        Engine.Engine.ResetSimulation();
        // show error messages
        Engine.Engine.ShowErrorMessage = (message) => {
            _currentError = message;
        }; 
        
        // Set up pinch gesture for zooming
        var pinchGesture = new PinchGestureRecognizer();
        pinchGesture.PinchUpdated += (_, e) => {
            if (e.Status == GestureStatus.Running && !IsDrawing)
            {
                _canvas.Zoom(e.Scale);
                // if engine is not running, update the view
                if (Engine.Engine.Mode != Engine.Engine.EngineMode.Running)
                    EngineGraphicsView.Invalidate();
            }
        };

        // Set up pan gesture for panning
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += (_, e) => {
            if (e.StatusType != GestureStatus.Running || IsDrawing) return;
            _canvas.Move(e);
            if (Engine.Engine.Mode != Engine.Engine.EngineMode.Running)
                EngineGraphicsView.Invalidate();
        };
        EngineGraphicsView.GestureRecognizers.Add(pinchGesture);
        EngineGraphicsView.GestureRecognizers.Add(panGesture);
        EngineGraphicsView.StartInteraction += OnStartInteraction;
    }
    
    void OnStartInteraction(object? sender, TouchEventArgs evt) {
        // if not drawing, return
        if (!IsDrawing) return;
        // if engine is running, return
        if (Engine.Engine.Mode == Engine.Engine.EngineMode.Running) return;
        
        // check if last touch point is valid
        if (_lastTouchPoint.X == -1 && _lastTouchPoint.Y == -1) {
            _lastTouchPoint = evt.Touches.FirstOrDefault();
            return;
        }
        
        // if it's valid
        var start = _canvas.ConvertToCanvasCoordinates(_lastTouchPoint);
        var end = _canvas.ConvertToCanvasCoordinates(evt.Touches.FirstOrDefault());
        
        int width = Math.Abs((int)(end.X - start.X));
        int height = Math.Abs((int)(end.Y - start.Y));
        
        if (width < 1 || height < 1) {
            _lastTouchPoint = new PointF(-1, -1);
            return;
        }
        
        var rect = new EngineStateRectangle(Engine.Engine.EngineObjectsManager?.GenerateUniqueName(), width, height);
        
        // Check which point is the top left
        if (start.X < end.X && start.Y < end.Y) {
            rect.Position = new Point((int)start.X, (int)start.Y);
        } else if (start.X > end.X && start.Y > end.Y) {
            rect.Position = new Point((int)end.X, (int)end.Y);
        } else if (start.X < end.X && start.Y > end.Y) {
            rect.Position = new Point((int)start.X, (int)end.Y);
        } else {
            rect.Position = new Point((int)end.X, (int)start.Y);
        }
        
        // Position can only be integer
        rect.Position = new Point((int)rect.Position.X, (int)rect.Position.Y);

        rect.CurrentTemperature = 373;
        Engine.Engine.EngineObjectsManager?.AddObject(rect);
        
        _lastTouchPoint = evt.Touches.FirstOrDefault();
        _lastTouchPoint = new PointF(-1, -1);
        IsDrawing = false;
        UpdateAll();
    }


    
    private void UpdateTimer_Elapsed(object? sender, ElapsedEventArgs e) {
        // Dispatch to the main thread for UI updates
        MainThread.BeginInvokeOnMainThread(Update);
    }

    private void ZoomToObject(EngineObject obj) {
        _canvas.ZoomToObject(obj);
    }

    private void EngineModeChanged() {
        if(Engine.Engine.Mode != Engine.Engine.EngineMode.Stopped) {
            ObjectsList.Enable(false);
            TabProperties.Enable(false);
        } else {
            ObjectsList.Enable(true);
            TabProperties.Enable(true);
        }
    }
    

    private void SelectedObjectChanged(EngineObject? obj) {
        // Implement the logic to handle the selection change
        EngineGraphicsView.Invalidate();
        TabProperties.SetObject(obj);

        if(obj == null) {  ObjectsList.Update(Engine.Engine.EngineObjectsManager?.GetObjects()); return; }

        obj.PropertyChanged += (_, _) => {
            ObjectsList.Update(Engine.Engine.EngineObjectsManager?.GetObjects());
            TabProperties.Update();
            EngineGraphicsView.Invalidate();
            Engine.Engine.EngineObjectsManager?.UpdateSmallestAndBiggestTemperature();
        };
    }

    private void UpdateAllAfterChangeProperties() {
        TabProperties.SetObject(null);
        UpdateAll();
    }

    public void UpdateAll() {
        ObjectsList.Update(Engine.Engine.EngineObjectsManager?.GetObjects());
        TabProperties.Update();
        EngineGraphicsView.Invalidate();
        ControlPanel.Update();
    }
    

    private void ObjectsChanged() {
        _objectsChanged = true;
    }

    public void Update() {
        if(_currentError != "") {
            Engine.Engine.Stop();
            ControlPanel.SetStoppedMode();
            DisplayAlert("Error", _currentError, "OK");
            _currentError = "";
        }
        
        if (!Engine.Engine.IsRunning() && !_objectsChanged) return;
        if(_objectsChanged) _objectsChanged = false;

        if(Engine.Engine.Mode != Engine.Engine.EngineMode.Running) 
            ObjectsList.Update(Engine.Engine.EngineObjectsManager?.GetObjects());
        ControlPanel.Update();
        TabProperties.Update();
        EngineGraphicsView.Invalidate();
    }
}