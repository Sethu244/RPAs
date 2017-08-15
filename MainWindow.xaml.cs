using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Presentation;
using System.Activities.Presentation.Metadata;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System.Activities.XamlIntegration;  

namespace MrRobo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WorkflowDesigner wd;
        private static string strFileName;
        public MainWindow()
        {
            InitializeComponent();

            // Register the metadata  
            RegisterMetadata();

            // Add the WFF Designer  
            AddDesigner();

            AddToolBox();

            AddPropertyInspector();
        }

        private void AddDesigner()
        {
            //Create an instance of WorkflowDesigner class.  
            this.wd = new WorkflowDesigner();
             //Place the designer canvas in the middle column of the grid.  
            Grid.SetColumn(this.wd.View, 1);
            Grid.SetRow(this.wd.View, 1);
            //Load a new Sequence as default.  
            this.wd.Load(new Flowchart());

            //Add the designer canvas to the grid.  
            MainGrid.Children.Add(this.wd.View);
        }

        private void RegisterMetadata()
        {
            DesignerMetadata dm = new DesignerMetadata();
            dm.Register();
        }

        private void AddToolBox()
        {
            ToolboxControl tc = GetToolboxControl();
            Grid.SetColumn(tc, 0);
            Grid.SetRow(tc, 1);
            MainGrid.Children.Add(tc);
        }  
        private ToolboxControl GetToolboxControl()
        {
            // Create the ToolBoxControl.  
            ToolboxControl ctrl = new ToolboxControl();

            // Create a category.  
            ToolboxCategory category = new ToolboxCategory("Standard");

            // Create Toolbox items.  
            ToolboxItemWrapper tool1 =
                new ToolboxItemWrapper("System.Activities.Statements.Assign",
                typeof(Assign).Assembly.FullName, null, "Assign");

            ToolboxItemWrapper tool2 = new ToolboxItemWrapper("System.Activities.Statements.Sequence",
                typeof(Sequence).Assembly.FullName, null, "Sequence");

            ToolboxItemWrapper tool3 = new ToolboxItemWrapper("System.Activities.Statements.WriteLine",
                typeof(WriteLine).Assembly.FullName, null, "WriteLine");

            ToolboxItemWrapper tool4 = new ToolboxItemWrapper("System.Activities.Statements.WriteLine",
             typeof(WriteLine).Assembly.FullName, null, "MessageBox");
           
           

            // Add the Toolbox items to the category.  
            category.Add(tool1);
            category.Add(tool2);
            category.Add(tool3);
            // Add the category to the ToolBox control.  
            ctrl.Categories.Add(category);
            return ctrl;
        }

        private void AddPropertyInspector()
        {
            Grid.SetColumn(wd.PropertyInspectorView, 2);
            Grid.SetRow(wd.PropertyInspectorView, 1);
            MainGrid.Children.Add(wd.PropertyInspectorView);
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (this.wd != null && !string.IsNullOrEmpty(strFileName))
            {
                StringWriter objStrWriter = new StringWriter();
                Activity objActivity = ActivityXamlServices.Load(strFileName);
                WorkflowApplication wfApp = new WorkflowApplication(objActivity);
                wfApp.Extensions.Add(objStrWriter);
                wfApp.Completed = WorkFlowCompleted;
                wfApp.OnUnhandledException = WorkFlowException;
                wfApp.Run();

            }
        }

        private UnhandledExceptionAction WorkFlowException(WorkflowApplicationUnhandledExceptionEventArgs arg)
        {
            MessageBox.Show(arg.UnhandledException.Message, "Workflow Unhandled Exception");
            return UnhandledExceptionAction.Terminate;
        }

        private void WorkFlowCompleted(WorkflowApplicationCompletedEventArgs e)
        {
            StringWriter writer = e.GetInstanceExtensions<StringWriter>().First();
            MessageBox.Show(writer.ToString(), "Workflow Completed");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.wd != null)
            {

                SaveFileDialog objSaveFile = new SaveFileDialog();
                objSaveFile.DefaultExt = "xaml";
                objSaveFile.AddExtension = true;
                
                if(objSaveFile.ShowDialog()==true)
                {
                    strFileName = objSaveFile.FileName;
                   
                }
                if(!string.IsNullOrEmpty(strFileName))
                {
                    this.wd.Save(strFileName);
                }

            }
        }
  

    }
}
