using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace ESAPIScript
{
    public class EsapiWorker 
    {
        private readonly StructureSet _ss = null;
        private readonly Patient _p = null;
        private readonly Dispatcher Dispatcher = null;
        public Dispatcher ui;
        
        public EsapiWorker(Patient p, StructureSet ss)
        {
            _p = p;
            _ss = ss;
            Dispatcher = Dispatcher.CurrentDispatcher;
        }
    
        public delegate void D(Patient p, StructureSet s);
        public async Task<bool> AsyncRunStructureContext(Action<Patient, StructureSet, Dispatcher> a)
        {
            await Dispatcher.BeginInvoke(a, _p, _ss, ui);
            return true;
        }
   }
}
