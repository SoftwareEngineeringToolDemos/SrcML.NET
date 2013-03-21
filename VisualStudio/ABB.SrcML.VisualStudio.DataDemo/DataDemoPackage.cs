﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using ABB.SrcML;
using ABB.SrcML.Data;
using ABB.SrcML.VisualStudio.SrcMLService;
using EnvDTE;
using EnvDTE80;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace ABB.SrcML.VisualStudio.DataDemo {
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidDataDemoPkgString)]
    //Autoload on UICONTEXT_SolutionExists
    //[ProvideAutoLoad("f1536ef8-92ec-443c-9ed7-fdadf150da82")]
    public sealed class DataDemoPackage : Package {
        private DTE2 dte;
        private ISrcMLGlobalService srcMLService;
        
        private DataArchive dataArchive;

        private Guid outputPaneGuid;
        private IVsOutputWindowPane outputPane;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public DataDemoPackage() {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize() {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if(null != mcs) {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidDataDemoCmdSet, (int)PkgCmdIDList.cmdidSrcMLData);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);
            }

            srcMLService = GetService(typeof(SSrcMLGlobalService)) as ISrcMLGlobalService;
            if(srcMLService == null) {
                PrintOutputLine("Could not get SrcMLGlobalService!");
                return;
            }
            srcMLService.StartupCompleted += srcMLService_StartupCompleted;
            
        }

        #endregion

        void srcMLService_StartupCompleted(object sender, EventArgs e) {
            var service = sender as ISrcMLGlobalService;
            if(service != null) {
                dataArchive = new DataArchive(service.GetSrcMLArchive());
            }
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e) {
            if(dte == null) {
                dte = GetService(typeof(DTE)) as DTE2;
                if(dte == null) {
                    PrintOutputLine("Could not get DTE!");
                    return;
                }
            }
            if(dataArchive == null) {
                PrintOutputLine("SrcML.NET is still processing the solution. Please try again in a few moments.");
                return;
            }

            var doc = dte.ActiveDocument;
            if(doc != null) {
                var sel = doc.Selection;
                var cursor = ((TextSelection)sel).ActivePoint;
                PrintOutputLine(string.Format("Cursor at: {0}:{1},{2}", dte.ActiveDocument.FullName, cursor.Line, cursor.LineCharOffset));

                var scope = dataArchive.FindScope(new SourceLocation(dte.ActiveDocument.FullName, cursor.Line, cursor.LineCharOffset));
                PrintOutputLine(scope.ToString());
            }

            
        }

        
        private void SetupOutputPane() {
            outputPaneGuid = Guid.NewGuid();
            outputPane = GetOutputPane(outputPaneGuid, "SrcML.Data");
            outputPane.Activate();
        }

        private void PrintOutput(string text) {
            if(outputPane == null) {
                SetupOutputPane();
            }
            outputPane.OutputString(text);
        }

        private void PrintOutputLine(string text) {
            if(outputPane == null) {
                SetupOutputPane();
            }
            outputPane.OutputString(text + Environment.NewLine);
        }
    }
}
