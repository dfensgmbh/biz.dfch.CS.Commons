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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Commons.Diagnostics;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Commons.Workflow;

namespace biz.dfch.CS.Commons.Tests.Workflow
{
    [TestClass]
    public class DefaultLoggingTrackingParticipantSettingsTest
    {
        [TestMethod]
        [ExpectContractFailure(MessagePattern = "Precondition.+value")]
        public void SetNullTraceSourceNameThrowsContractExeption()
        {
            // Arrange
            var defaultLoggingTrackingParticipantSettings = new DefaultLoggingTrackingParticipantSettings();

            // Act
            defaultLoggingTrackingParticipantSettings.TraceSourceName = null;

            // Assert
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "Precondition.+value")]
        public void SetEmptyTraceSourceNameThrowsContractExeption()
        {
            // Arrange
            var defaultLoggingTrackingParticipantSettings = new DefaultLoggingTrackingParticipantSettings();

            // Act
            defaultLoggingTrackingParticipantSettings.TraceSourceName = string.Empty;

            // Assert
        }

        [TestMethod]
        public void CreateDefaultLoggingTrackingParticipantSettingsSetsTraceSourceNameToDefaultTraceSourceName()
        {
            // Arrange

            // Act
            var defaultLoggingTrackingParticipantSettings = new DefaultLoggingTrackingParticipantSettings();

            // Assert
            Assert.AreEqual(Logger.DEFAULT_TRACESOURCE_NAME, defaultLoggingTrackingParticipantSettings.TraceSourceName);
            Assert.IsNull(defaultLoggingTrackingParticipantSettings.TrackingProfile);
        }
    }
}
