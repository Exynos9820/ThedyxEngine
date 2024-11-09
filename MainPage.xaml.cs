using System.Timers;
using ThedyxEngine.Engine;
using Timer = System.Timers.Timer;

namespace ThedyxEngine;

public partial class MainPage : ContentPage
{
    int count = 0;
    private readonly Timer updateTimer;
    private bool _objectsChanged = false;
    private readonly int _windowRefreshRate = 60;
    public MainPage()
    {
        InitializeComponent();
        log4net.Config.XmlConfigurator.Configure();

        // init engine(proccessor of the app with entity of main window)
        Engine.Engine.Init(this);

        _windowRefreshRate = Util.SystemInfo.GetRefreshRate();
        updateTimer = new Timer
        {
            Interval = 1000.0 / _windowRefreshRate // Interval in milliseconds
        };
        updateTimer.Elapsed += UpdateTimer_Elapsed;
        updateTimer.Start();
        ObjectsChanged();

        _engineObjectsList.OnSelectedObjectChanged = SelectedObjectChanged;
        //_controlPanel.DeleteSelected = SelectedObjectChanged;
        //_controlPanel.UpdateUI = UpdateAll;
        _engineObjectsList.OnDeleteObject = UpdateAllAfterChangeProperties;
        _engineObjectsList.OnZoomToObject = ZoomToObject;
        //_controlPanel.EngineModeChanged = EngineModeChanged;*/
    }
    
    private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        // Dispatch to the main thread for UI updates
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Update();
        });
    }

    private void ZoomToObject(EngineObject obj) {
            //_engineCanva.ZoomToObject(obj);
        }

        private void EngineModeChanged() {
            if(Engine.Engine.Mode != Engine.Engine.EngineMode.Stopped) {
                _engineObjectsList.Enable(false);
                //_engineTabProperties.Enable(false);
            } else {
                _engineObjectsList.Enable(true);
                //_engineTabProperties.Enable(true);
            }
        }

        private void SelectedObjectChanged(EngineObject obj) {
            // Implement the logic to handle the selection change
            /*_engineCanva.Update();
            _engineTabProperties.SetObject(obj);
            _engineTabProperties.Update();*/

            if(obj == null) {  _engineObjectsList.Update(Engine.Engine.EngineObjectsManager.GetObjects()); return; }

            obj.PropertyChanged += (sender, args) => {
                _engineObjectsList.Update(Engine.Engine.EngineObjectsManager.GetObjects());
                //_engineTabProperties.Update();
                //_engineCanva.Update();
            };
        }

        private void UpdateAllAfterChangeProperties() {
            //_engineTabProperties.SetObject(null);
            UpdateAll();
        }

        public void UpdateAll() {
            /*_engineObjectsList.Update(Engine.Engine.EngineObjectsManager.GetObjects());
            _engineTabProperties.Update();
            _engineCanva.Update();*/
        }
        

        private void ObjectsChanged() {
            _objectsChanged = true;
        }

        public void Update() {
            if (!Engine.Engine.IsRunning() && !_objectsChanged) return;
            if(_objectsChanged) _objectsChanged = false;

            if(Engine.Engine.Mode != Engine.Engine.EngineMode.Running) 
                _engineObjectsList.Update(Engine.Engine.EngineObjectsManager.GetObjects());
            /*_controlPanel.Update();
            _engineCanva.Update();
            _engineTabProperties.Update();*/

        }
}