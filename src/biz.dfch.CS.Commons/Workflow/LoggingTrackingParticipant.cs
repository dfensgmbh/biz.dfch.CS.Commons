/**
 * Copyright 2017 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Commons.Diagnostics;
using Newtonsoft.Json;

namespace biz.dfch.CS.Commons.Workflow
{
    public class LoggingTrackingParticipant : TrackingParticipant
    {
        private readonly ILoggingTrackingParticipantSettings _loggingTrackingParticipantSettings;

        public sealed override TrackingProfile TrackingProfile { get; set; }

        public LoggingTrackingParticipant(ILoggingTrackingParticipantSettings loggingTrackingParticipantSettings)
        {
            Contract.Assert(null != loggingTrackingParticipantSettings);

            _loggingTrackingParticipantSettings = loggingTrackingParticipantSettings;
            TrackingProfile = loggingTrackingParticipantSettings.TrackingProfile;
        }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            Contract.Assert(null != record);
            Contract.Assert(null != timeout);

            if (record is ActivityStateRecord)
                LogActivityRecord((ActivityStateRecord)record);
            else if (record is WorkflowInstanceRecord)
                LogWorkflowRecord((WorkflowInstanceRecord)record);
            else if (record is BookmarkResumptionRecord)
                LogBookmarkRecord((BookmarkResumptionRecord)record);
            else if (record is ActivityScheduledRecord)
                LogActivityScheduledRecord((ActivityScheduledRecord)record);
            else if (record is CancelRequestedRecord)
                LogCancelRequestedRecord((CancelRequestedRecord)record);
            else if (record is FaultPropagationRecord)
                LogFaultPropagationRecord((FaultPropagationRecord)record);
            else
                LogCustomRecord((CustomTrackingRecord)record);
        }

        private void LogActivityRecord(ActivityStateRecord record)
        {
            Contract.Requires(null != record);

            var annotations = JsonConvert.SerializeObject(record.Annotations, Formatting.Indented);
            var arguments = JsonConvert.SerializeObject(record.Arguments, Formatting.Indented);
            var variables = JsonConvert.SerializeObject(record.Variables, Formatting.Indented);
            var activity = JsonConvert.SerializeObject(record.Activity, Formatting.Indented);

            Logger.Get(_loggingTrackingParticipantSettings.TraceSourceName)
                .TraceInformation("{0} {1} {2} - InstanceId = '{3}', Annotations = '{4}', Activity = '{5}', State = '{6}', Arguments = '{7}', Variables = '{8}'",
                record.GetType().Name,
                record.EventTime,
                record.Level,
                record.InstanceId,
                annotations,
                activity,
                record.State,
                arguments,
                variables
                );
        }

        private void LogWorkflowRecord(WorkflowInstanceRecord record)
        {
            Contract.Requires(null != record);

            var annotations = JsonConvert.SerializeObject(record.Annotations, Formatting.Indented);
            var workflowDefinitionIdentity = JsonConvert.SerializeObject(record.WorkflowDefinitionIdentity, Formatting.Indented);

            Logger.Get(_loggingTrackingParticipantSettings.TraceSourceName)
                .TraceInformation("{0} {1} {2} - InstanceId = '{3}', Annotations = '{4}', ActivityDefinitionId = '{5}', State = '{6}', WorkflowDefinitionIdentity = '{7}'",
                record.GetType().Name,
                record.EventTime,
                record.Level,
                record.InstanceId,
                annotations,
                record.ActivityDefinitionId,
                record.State,
                workflowDefinitionIdentity
                );
        }

        private void LogBookmarkRecord(BookmarkResumptionRecord record)
        {
            Contract.Requires(null != record);

            var annotations = JsonConvert.SerializeObject(record.Annotations, Formatting.Indented);
            var payload = JsonConvert.SerializeObject(record.Payload);
            var owner = JsonConvert.SerializeObject(record.Owner);

            Logger.Get(_loggingTrackingParticipantSettings.TraceSourceName)
                .TraceInformation("{0} {1} {2} - InstanceId = '{3}', Annotations = '{4}', BookmarkName = '{5}', BookmarkScope = '{6}', Owner = '{7}', Payload = '{8}'",
                record.GetType().Name,
                record.EventTime,
                record.Level,
                record.InstanceId,
                annotations,
                record.BookmarkName,
                record.BookmarkScope,
                owner,
                payload
                );
        }

        private void LogActivityScheduledRecord(ActivityScheduledRecord record)
        {
            Contract.Requires(null != record);

            var annotations = JsonConvert.SerializeObject(record.Annotations, Formatting.Indented);
            var activity = JsonConvert.SerializeObject(record.Activity, Formatting.Indented);
            var child = JsonConvert.SerializeObject(record.Child, Formatting.Indented);

            Logger.Get(_loggingTrackingParticipantSettings.TraceSourceName)
                .TraceInformation("{0} {1} {2} - InstanceId = '{3}', Annotations = '{4}', Activity = '{5}', Child = '{6}'",
                record.GetType().Name,
                record.EventTime,
                record.Level,
                record.InstanceId,
                annotations,
                activity,
                child
                );
        }

        private void LogCancelRequestedRecord(CancelRequestedRecord record)
        {
            Contract.Requires(null != record);

            var annotations = JsonConvert.SerializeObject(record.Annotations, Formatting.Indented);
            var activity = JsonConvert.SerializeObject(record.Activity, Formatting.Indented);
            var child = JsonConvert.SerializeObject(record.Child, Formatting.Indented);

            Logger.Get(_loggingTrackingParticipantSettings.TraceSourceName)
                .TraceInformation("{0} {1} {2} - InstanceId = '{3}', Annotations = '{4}', Activity = '{5}', Child = '{6}'",
                record.GetType().Name,
                record.EventTime,
                record.Level,
                record.InstanceId,
                annotations,
                activity,
                child
                );
        }

        private void LogFaultPropagationRecord(FaultPropagationRecord record)
        {
            Contract.Requires(null != record);

            var annotations = JsonConvert.SerializeObject(record.Annotations, Formatting.Indented);
            var fault = JsonConvert.SerializeObject(record.Fault, Formatting.Indented);
            var faultHandler = JsonConvert.SerializeObject(record.FaultHandler, Formatting.Indented);
            var faultSource = JsonConvert.SerializeObject(record.FaultSource, Formatting.Indented);

            Logger.Get(_loggingTrackingParticipantSettings.TraceSourceName)
                .TraceInformation("{0} {1} {2} - InstanceId = '{3}', Annotations = '{4}', Fault = '{5}',  FaultHandler = '{6}', FaultSource= '{7}', IsFaultSource = '{8}'",
                record.GetType().Name,
                record.EventTime,
                record.Level,
                record.InstanceId,
                annotations,
                fault,
                faultHandler,
                faultSource,
                record.IsFaultSource
                );
        }

        private void LogCustomRecord(CustomTrackingRecord record)
        {
            Contract.Requires(null != record);

            var annotations = JsonConvert.SerializeObject(record.Annotations, Formatting.Indented);

            Logger.Get(_loggingTrackingParticipantSettings.TraceSourceName).TraceInformation("{0} {1} {2} - InstanceId = '{3}', Annotations = '{4}'",
                record.GetType().Name,
                record.EventTime,
                record.Level,
                record.InstanceId,
                annotations
                );
        }
    }
}
