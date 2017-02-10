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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

[assembly: AssemblyTitle("biz.dfch.CS.Commons")]
[assembly: AssemblyProduct("biz.dfch.CS.Commons")]

[assembly: Guid("5004a17d-4592-4001-be2e-8c4278f7b9e1")]

[assembly:InternalsVisibleTo("biz.dfch.CS.Commons.Tests" +
",PublicKey=" +
"0024000004800000940000000602000000240000525341310004000001000100b3ff1f5b1cd339" +
"a46db108a12f4a03c7cef0c469a649a1af9c3853730c054e65e5f95a697f46998c3cd3ba7fae75" +
"5cb5556b570ab64daa3e7f720a27331a3334c39510ed49c4b253dcb3f7716000fd836c8c2dbdb5" +
"153efeff6e70570bf6d814ff18272afac8750fb7f1ccd13616597b2828e5ef297043c5e0bf4bbc" +
"101bdcae" +
""
    )]


[assembly: SecurityRules(SecurityRuleSet.Level2)]
[assembly: AllowPartiallyTrustedCallers] // APTCA
