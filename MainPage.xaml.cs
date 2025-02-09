using System.Timers;
using log4net;
using LukeMauiFilePicker;
using ThedyxEngine.Engine;
using ThedyxEngine.UI;
using ThedyxEngine.Util;
using Timer = System.Timers.Timer;

namespace ThedyxEngine;

public partial class MainPage : ContentPage {
    int count = 0;
    private readonly Timer updateTimer;
    private bool _objectsChanged = false;
    private EngineCanvas _engineCanvas;
    public readonly IFilePickerService picker;
    private static readonly ILog log = LogManager.GetLogger(typeof(MainPage)); // Logger

    public MainPage(IFilePickerService picker) {
        InitializeComponent();
        log4net.Config.XmlConfigurator.Configure();

        // init engine(proccessor of the app with entity of main window)
        Engine.Engine.Init(this);

        GlobalVariables.WindowRefreshRate = Util.SystemInfo.GetRefreshRate();
        updateTimer = new Timer
        {
            Interval = 1000.0 / GlobalVariables.WindowRefreshRate // Interval in milliseconds
        };
        updateTimer.Elapsed += UpdateTimer_Elapsed;
        updateTimer.Start();
        ObjectsChanged();

        ObjectsList.OnSelectedObjectChanged = SelectedObjectChanged;
        ControlPanel.DeleteSelected = SelectedObjectChanged;
        ControlPanel.UpdateUI = UpdateAll;
        ControlPanel.MainPage = this;
        ObjectsList.OnDeleteObject = UpdateAllAfterChangeProperties;
        ObjectsList.OnZoomToObject = ZoomToObject;
        ControlPanel.EngineModeChanged = EngineModeChanged;
        ControlPanel.UpdateUI = UpdateAll;
        _engineCanvas = new EngineCanvas(this);
        EngineGraphicsView.Drawable = _engineCanvas;
        EngineGraphicsView.BackgroundColor = Colors.White;
        TabProperties.OnObjectChange = UpdateAll;
        Engine.Engine.ResetSimulation();
        
#if MACCATALYST
        // Set up pinch gesture for zooming
        var pinchGesture = new PinchGestureRecognizer();
        pinchGesture.PinchUpdated += (s, e) => {
            if (e.Status == GestureStatus.Running)
            {
                _engineCanvas.Zoom(e.Scale);
                // if engine is not running, update the view
                if (Engine.Engine.Mode != Engine.Engine.EngineMode.Running)
                    EngineGraphicsView.Invalidate();
            }
        };

        // Set up pan gesture for panning
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += (s, e) => {
            if (e.StatusType == GestureStatus.Running) {
                _engineCanvas.Move(e);
                if (Engine.Engine.Mode != Engine.Engine.EngineMode.Running)
                    EngineGraphicsView.Invalidate();
            }
        };
#endif
        EngineGraphicsView.GestureRecognizers.Add(pinchGesture);
        EngineGraphicsView.GestureRecognizers.Add(panGesture);
    }
    
    private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e) {
        // Dispatch to the main thread for UI updates
        MainThread.BeginInvokeOnMainThread(Update);
    }

    private void ZoomToObject(EngineObject obj) {
        _engineCanvas.ZoomToObject(obj);
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
    

    private void SelectedObjectChanged(EngineObject obj) {
        // Implement the logic to handle the selection change
        EngineGraphicsView.Invalidate();
        TabProperties.SetObject(obj);

        if(obj == null) {  ObjectsList.Update(Engine.Engine.EngineObjectsManager.GetObjects()); return; }

        obj.PropertyChanged += (sender, args) => {
            ObjectsList.Update(Engine.Engine.EngineObjectsManager.GetObjects());
            TabProperties.Update();
            EngineGraphicsView.Invalidate();
        };
    }

    private void UpdateAllAfterChangeProperties() {
        TabProperties.SetObject(null);
        UpdateAll();
    }

    public void UpdateAll() {
        ObjectsList.Update(Engine.Engine.EngineObjectsManager.GetObjects());
        TabProperties.Update();
        EngineGraphicsView.Invalidate();
    }
    

    private void ObjectsChanged() {
        _objectsChanged = true;
    }

    public void Update() {
        if (!Engine.Engine.IsRunning() && !_objectsChanged) return;
        if(_objectsChanged) _objectsChanged = false;

        if(Engine.Engine.Mode != Engine.Engine.EngineMode.Running) 
            ObjectsList.Update(Engine.Engine.EngineObjectsManager.GetObjects());
        ControlPanel.Update();
        TabProperties.Update();
        EngineGraphicsView.Invalidate();
    }
}