using NUnit.Framework;
using System.Collections.Generic;

namespace QModManager.Tests.QModManager
{
    public class VersionDependenciesTests
    {
        public class ExactMatches
        {
            [TestFixture]
            public class UnparsableVersion
            {
                [Test]
                public void ExactMatchSuccess()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "Release",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "Release" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void ExactMatchSuccess_WithEqualsOperator()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "Release",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "=Release" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void ExactMatchSuccess_WithEqualsOperator_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "Release",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " = Release" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void ExactMatchFail()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "Release",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "AnotherVersion" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void ExactMatchFail_WithEqualsOperator()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "Release",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "=AnotherVersion" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void ExactMatchFail_WithEqualsOperator_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "Release",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " = AnotherVersion" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
            }

            [TestFixture]
            public class ParsableVersion
            {
                [Test]
                public void ExactMatchSuccess()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void ExactMatchSuccess_WithEqualsOperator()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", "=1.0" }
                    }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void ExactMatchSuccess_WithEqualsOperator_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " = 1.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void ExactMatchFail()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.1",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void ExactMatchFail_WithEqualsOperator()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.1",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "=1.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void ExactMatchFail_WithEqualsOperator_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.1",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " = 1.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
            }
        }

        public class Ranges
        {
            [TestFixture]
            public class Standard
            {
                [Test]
                public void MatchSuccess_1()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", ">1.2.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_2()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "<1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_3()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", ">1.2.2 <1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_4()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.2",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", ">=1.2.3 <1.3.0 || =1.2.2" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_5()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", ">=1.2 <=1.3" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_1_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " > 1.2.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_2_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " < 1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_3_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " > 1.2.2 < 1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_4_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.2",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " >= 1.2.3 < 1.3.0 || = 1.2.2" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_5_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " >= 1.2 <= 1.3" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_1()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.1.9",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", ">1.2.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_2()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "2.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "<1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_3()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.4",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", ">1.2.2 <1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_4()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", ">=1.2.3 <1.3.0 || =1.2.2" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_5()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.1",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", ">=1.2 <=1.3" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_1_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.1.9",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " > 1.2.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_2_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "2.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " < 1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_3_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.4",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " > 1.2.2 < 1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_4_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " >= 1.2.3 < 1.3.0 || = 1.2.2" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_5_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.1",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " >= 1.2 <= 1.3" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
            }

            [TestFixture]
            public class Hyphen
            {
                [Test]
                public void SemVer_Weirdness_1()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2.0-1.4.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void SemVer_Weirdness_2()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2-1.3" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void SemVer_Weirdness_3()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2.2-1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_1_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2.0 - 1.4.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_2_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2 - 1.3" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_3_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2.2 - 1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_1()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.4.1",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2.0-1.4.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_2()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "2.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2-1.3" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_3()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.4.1",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2.2-1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_1_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.4.1",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2.0 - 1.4.0" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_2_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "2.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2 - 1.3" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_3_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.4",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2.2 - 1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
            }

            [TestFixture]
            public class X
            {
                [Test]
                public void MatchSuccess_1()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2.x" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_2()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.x" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_3()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_1_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " 1.2.x " }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_2_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " 1.x " }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchSuccess_3_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " 1 " }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_1()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.1.9",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.2.x" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_2()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "2.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1.x" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_3()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "2.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "1" }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_1_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "1.1.9",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " 1.2.x " }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_2_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "2.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " 1.x " }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
                [Test]
                public void MatchFail_3_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QModFromJson mod1 = new QModFromJson
                    {
                        Id = "Mod1",
                        Version = "2.0",
                    };

                    QModFromJson mod2 = new QModFromJson
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " 1 " }
                        }
                    };

                    Patcher.foundMods = new List<QModFromJson>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QModFromJson>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsFalse(Patcher.erroredMods.Count == 0);
                }
            }
        }
    }
}
