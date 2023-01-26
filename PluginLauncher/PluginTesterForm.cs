﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using System.Reflection;
using System.Windows.Threading;
using Serilog;
using ESAPIScript;



namespace PluginTester
{
    public partial class PluginTesterForm : Form
    {
        VMS.TPS.ESAPIApplication app;

        private void RunOnNewStaThread(Action a)
        {
            Thread thread = new Thread(() => a());
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        public PluginTesterForm()
        {
            InitializeComponent(); // This is the entry point to the standalone app that you will use to debug your script (I've called it TestScript in this example)
            app = VMS.TPS.ESAPIApplication.Instance; // instantiate the ESAPI context

            // Can use this to seed the form if you have a standard test patient.
            textBox_PID.Text = "CN_ESAPI201_pro1";
            textBox_SSID.Text = "CT_pro1";
            textBox_planId.Text = "proN_RO2C";
            textBox_CourseID.Text = "Course1";
        }


        private void InitializeAndStartMainWindow(EsapiWorker esapiWorker)
        {
            var viewModel = new ViewModel(esapiWorker);
            try
            {
                var mainWindow = new ScriptWindow(viewModel);
                mainWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                var test = ex.Message;
            }

        }

        private async void Run_Script_Click(object sender, EventArgs e)
        {
            string PID = textBox_PID.Text;  // get the patient ID
            string CourseID = textBox_CourseID.Text;  // get the patient ID
            string PlanID = textBox_planId.Text;
            string SSID = textBox_SSID.Text; // get the plan ID
            Patient p = null;
            Course c = null;
            PlanSetup pl = null;
            StructureSet ss = null;
            try
            {
                p = app.Context.OpenPatientById(PID);
                if (p == null)
                {
                    System.Windows.Forms.MessageBox.Show("Patient not found!");
                    app.Context.ClosePatient();
                    return;
                }
                c = p.Courses.FirstOrDefault(x => x.Id == CourseID);
                if (c != null)
                {
                    pl = c.PlanSetups.FirstOrDefault(x => x.Id == PlanID);
                    
                }
                ss = p.StructureSets.FirstOrDefault(x => string.Equals(x.Id, SSID, StringComparison.OrdinalIgnoreCase));
                if (ss == null)
                {
                    System.Windows.Forms.MessageBox.Show("SS not found!");
                    app.Context.ClosePatient();
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("There was an error opening the patient / plan.");
                System.Windows.Forms.MessageBox.Show(string.Format("{0} \r\n {1} \r\n {2} \r\n", ex.Message, ex.InnerException, ex.StackTrace));
                app.Context.ClosePatient();
                return;
            }
            try
            {
                EsapiWorker ew = null;
                if (pl != null)
                    ew = new EsapiWorker(p, pl);
                else if (ss != null)
                    ew = new EsapiWorker(p, ss); // write enabled for plan
                if (ew != null)
                {
                    Helpers.SeriLog.Initialize(app.Context.CurrentUser.Id);
                    DispatcherFrame frame = new DispatcherFrame();
                    RunOnNewStaThread(() =>
                    {
                        // This method won't return until the window is closed
                        InitializeAndStartMainWindow(ew);

                        // End the queue so that the script can exit
                        frame.Continue = false;
                    });
                    Dispatcher.PushFrame(frame);

                    if (checkBox_save.Checked)
                        app.Context.SaveModifications();

                    app.Context.ClosePatient();
                }
                else
                    System.Windows.Forms.MessageBox.Show("The plan or structure set was not found.");





            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("There was an error running your script.");
                System.Windows.Forms.MessageBox.Show(string.Format("{0} \r\n {1} \r\n {2} \r\n", ex.Message, ex.InnerException, ex.StackTrace));
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            app.Context.Dispose();
        }
              
    }
}
