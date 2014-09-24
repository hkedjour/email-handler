/*
* Copyright (C) 2014 Hichem Kedjour
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program. If not, see <http://www.gnu.org/licenses/>.
* 
* Part of code in this file was taken from an msdn sample.
*/
using System;
using System.Activities;
using System.Activities.DurableInstancing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.DurableInstancing;
using System.Threading;
using System.Xml.Linq;
using EmailHandler.Common.Configuration;
using EmailHandler.Common.Emails;

namespace EmailHandlerWorkflow
{
    public static class EmailHandlerService
    {
        static EmailHandlerService()
        {
            // Create a unique name that is used to associate instances in the instance store hosts that can load them. This is needed to prevent a Workflow host from loading
            // instances that have different implementations. The unique name should change whenever the implementation of the workflow changes to prevent workflow load exceptions.
            WfHostTypeName = XName.Get("Version8BEB976B-7F5D-42E1-BAC4-1CD5628034A6", typeof(HandleEmailActivity).FullName);

        }

        public static void HandleEmail(MessageInfo msg)
        {
            var store = new SqlWorkflowInstanceStore(ConfigurationManager.GetWfStoreConnectionString());

            var args = new Dictionary<string, object>();
            args["Message"] = msg;

            // Create a WorkflowApplication instance to start a new workflow
            var wfApp = CreateWorkflowApplication(new HandleEmailActivity(), store, WfHostTypeName, args);

            // This will create the workflow and execute it until the delay is executed, the workflow goes idle, and is unloaded
            wfApp.Run();
        }

        // summary:
        //      Start a loop to monitor runnable instances of the workflow from the database
        public static void Start()
        {
            new Thread(WakeupReadyInstances).Start();
        }

        private static void WakeupReadyInstances()
        {
            var store = new SqlWorkflowInstanceStore(ConfigurationManager.GetWfStoreConnectionString());

            // Create an InstanceStore owner that is associated with the workflow type
            InstanceHandle ownerHandle = CreateInstanceStoreOwner(store, WfHostTypeName);

            // This loop handles re-loading workflows that have been unloaded due to durable timers
            while (true)
            {
                // Wait for a timer registered by the delay to expire and the workflow instance to become "runnable" again
                WaitForRunnableInstance(store, ownerHandle);

                // Create a new WorkflowApplication instance to host the re-loaded workflow
                var wfApp = CreateWorkflowApplication(new HandleEmailActivity(), store, WfHostTypeName);

                try
                {
                    // Re-load the runnable workflow instance and run it
                    wfApp.LoadRunnableInstance();

                    wfApp.Run();
                    
                    Thread.Sleep(3000); // Give it some time to start before starting another one
                }
                catch (InstanceNotReadyException)
                {
                    Console.WriteLine("Handled expected InstanceNotReadyException, retrying...");
                }
            }
        }

        // A well known property that is needed by WorkflowApplication and the InstanceStore
        private static readonly XName WorkflowHostTypePropertyName = XNamespace.Get("urn:schemas-microsoft-com:System.Activities/4.0/properties").GetName("WorkflowHostType");

        private static readonly XName WfHostTypeName;


        // Configure a Default Owner for the instance store so instances can be re-loaded from WorkflowApplication
        private static InstanceHandle CreateInstanceStoreOwner(InstanceStore store, XName wfHostTypeName)
        {
            var ownerHandle = store.CreateInstanceHandle();

            var ownerCommand = new CreateWorkflowOwnerCommand
            {
                InstanceOwnerMetadata =
                {
                    { WorkflowHostTypePropertyName, new InstanceValue(wfHostTypeName) }
                }
            };

            store.DefaultInstanceOwner = store.Execute(ownerHandle, ownerCommand, TimeSpan.FromSeconds(30)).InstanceOwner;

            return ownerHandle;
        }

        // Creates and configures a new instance of WorkflowApplication
        private static WorkflowApplication CreateWorkflowApplication(Activity rootActivity, InstanceStore store, XName wfHostTypeName, IDictionary<string, object> args = null)
        {
            var wfApp = args != null ? new WorkflowApplication(rootActivity, args) : new WorkflowApplication(rootActivity);
            wfApp.InstanceStore = store;

            var wfScope = new Dictionary<XName, object> 
            {
                 { WorkflowHostTypePropertyName, wfHostTypeName }
            };

            // Add the WorkflowHostType value to workflow application so that it stores this data in the instance store when persisted
            wfApp.AddInitialInstanceValues(wfScope);

            // The workflow is unloaded as soon as it is idle (and able to persist)
            wfApp.PersistableIdle = idleArgs => PersistableIdleAction.Unload;

            // Configure some tracing and synchronization for the other WorkflowApplication events
            wfApp.Completed = obj => Trace.TraceInformation("Workflow completed");

            return wfApp;
        }

        private static void WaitForRunnableInstance(InstanceStore store, InstanceHandle ownerHandle)
        {
            try
            {
                store.WaitForEvents(ownerHandle, TimeSpan.MaxValue);
            }
            catch (Exception)
            {
                
            }
        }

    }
}