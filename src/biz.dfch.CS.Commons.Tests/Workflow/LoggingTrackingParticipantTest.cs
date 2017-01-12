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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Commons.Workflow;

namespace biz.dfch.CS.Commons.Tests.Workflow
{
    [TestClass]
    public class LoggingTrackingParticipantTest
    {
        private const string TRACKING_PROFILE_NAME = "ArbitraryTrackingProfile";
        private const string TRACE_SOURCE_NAME = "ArbitraryTrackingSource";

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "Assertion.+loggingTrackingParticipantSettings")]
        public void CreateLoggingTrackingParticipantWithNullSettingsThrowsContractException()
        {
            // Arrange

            // Act
            new LoggingTrackingParticipant(null);

            // Assert
        }

        [TestMethod]
        public void CreateLoggingTrackingParticipantInstanceWithCustomTrackingProfileSetsTrackingProfile()
        {
            // Arrange
            var trackingProfile = new TrackingProfile()
            {
                Name = TRACKING_PROFILE_NAME
            };

            var loggingTrackingParticipantSettings = new DefaultLoggingTrackingParticipantSettings
            {
                TraceSourceName = TRACE_SOURCE_NAME,
                TrackingProfile = trackingProfile
            };

            // Act
            TrackingParticipant trackingParticipant = new LoggingTrackingParticipant(loggingTrackingParticipantSettings);

            // Assert
            Assert.IsNotNull(trackingParticipant.TrackingProfile);
            Assert.AreEqual(trackingProfile, trackingParticipant.TrackingProfile);
            Assert.AreEqual(TRACKING_PROFILE_NAME, trackingParticipant.TrackingProfile.Name);
        }
    }
}
