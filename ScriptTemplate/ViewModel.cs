using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration.Assemblies;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using PropertyChanged;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

[assembly: ESAPIScript(IsWriteable = true)]

namespace ESAPIScript
{

    public class SometimesObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotfication = false;
        public bool SuppressNotification
        {
            get { return _suppressNotfication; }
            set
            {
                _suppressNotfication = value;
                if (value == false)
                    base.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
            }
        }
        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotfication)
                base.OnCollectionChanged(e);
        }
    }


    [AddINotifyPropertyChangedInterface]

    public class ViewModel : ObservableObject
    {
        public bool IsErrorAcknowledged { get; set; } = true;
        public string currentStructureSetId { get; set; } = "Design Value";

        public bool working { get; set; } = false;

        private string _dataPath { get; set; }

        public bool CanLoadTemplates { get; set; } = true;
        public bool UseTest { get; set; } = true;

        public string structureVolumeString { get; set; } = "DesignValue";

        private string _selectedStructureId;
        public string selectedStructureId
        {
            get { return _selectedStructureId; }
            set
            {
                _selectedStructureId = value;
                DisplayStructureMeanHU();
            }
        }

        private async void DisplayStructureMeanHU()
        {
            await ew.AsyncRunStructureContext((pat, ss, ui) =>
            {
                var selectedStructure = ss.Structures.FirstOrDefault(x => string.Equals(x.Id, _selectedStructureId, StringComparison.OrdinalIgnoreCase));
                if (selectedStructure != null)
                {
                    if (selectedStructure.IsEmpty)
                    {
                        ui.Invoke(() =>
                        {
                            structureVolumeString = "Structure is empty";
                        });
                    }
                    else
                    {
                        double volume = selectedStructure.Volume;
                        bool isHighResolution = selectedStructure.IsHighResolution;
                        ui.Invoke(() =>
                        {
                            structureVolumeString = string.Format("{0:0.##} and the resolution is {1}", volume, isHighResolution ? "high resolution" : "standard resolution");
                        });
                    }
                }
               
            });
        }

        public EsapiWorker ew = null;

        private ScriptConfig scriptConfig;

        private SolidColorBrush WarningColour = new SolidColorBrush(Colors.Goldenrod);
        public SometimesObservableCollection<string> structureIds { get; set; } = new SometimesObservableCollection<string>() { "DesignStructure1", "DesignStructure2" };

        public SolidColorBrush ScriptCompletionStatusColour { get; set; } = new SolidColorBrush(Colors.PaleGreen);

        public ViewModel()
        {

        }


        public ViewModel(EsapiWorker _ew = null)
        {
            ew = _ew;
            ew.ui = Dispatcher.CurrentDispatcher;
            Initialize();
        }


        private async void Initialize()
        {
            try
            {
                // Read Script Configuration
                _dataPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                GetScriptConfigFromXML(); // note this will fail unless this config file is defined
                // Initialize other GUI settings

                string ew_currentStructureSetId = string.Empty;
                await ew.AsyncRunStructureContext((pat, ss, ui) =>
                {
                    ew_currentStructureSetId = ss.Id;
                    ui.Invoke(() =>
                    {
                        currentStructureSetId = ew_currentStructureSetId;
                    });
                    foreach (string structureId in ss.Structures.Select(x => x.Id))
                    {
                        ui.Invoke(() =>
                        {
                            structureIds.Add(structureId);
                        });
                    }
                });
                //structureIds = ew_structureIds;
            }
            catch (Exception ex)
            {
                Helpers.SeriLog.LogError(string.Format("{0}\r\n{1}\r\n{2}", ex.Message, ex.InnerException, ex.StackTrace));
                MessageBox.Show(string.Format("{0}\r\n{1}\r\n{2}", ex.Message, ex.InnerException, ex.StackTrace));
            }
        }

        public ICommand StartCommand
        {
            get
            {
                return new DelegateCommand(Start);
            }
        }

        public async void Start(object param = null)
        {
            if (working)
            {
                return;
            }
            CanLoadTemplates = false;
            working = true;
            var newstructures = new List<string>();
            bool Done = await Task.Run(() => ew.AsyncRunStructureContext((p, S, ui) =>
            {
                // Do something with the structure set
            }
            ));

            working = false;

            CanLoadTemplates = true;

        }

        public void GetScriptConfigFromXML()
        {
            working = true;
            try
            {
                XmlSerializer Ser = new XmlSerializer(typeof(ScriptConfig));
                var configFile = Path.Combine(_dataPath, @"ScriptConfig.xml");
                using (StreamReader config = new StreamReader(configFile))
                {
                    try
                    {
                        scriptConfig = (ScriptConfig)Ser.Deserialize(config);
                    }
                    catch (Exception ex)
                    {
                        Helpers.SeriLog.LogFatal(string.Format("Unable to deserialize config file: {0}\r\n", configFile), ex);
                        MessageBox.Show(string.Format("Unable to read protocol file {0}\r\n\r\nDetails: {1}", configFile, ex.InnerException));

                    }
                }
            }
            catch (Exception ex)
            {
                Helpers.SeriLog.LogFatal(string.Format("Unable to find/open config file"), ex);
                MessageBox.Show(string.Format("{0}\r\n{1}\r\n{2}", ex.Message, ex.InnerException, ex.StackTrace));
            }
            working = false;
        }

    }


}
