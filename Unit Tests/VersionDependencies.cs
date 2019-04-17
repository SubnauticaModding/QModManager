using NUnit.Framework;
using System.Collections.Generic;

namespace QModManager.Tests
{
    [TestFixture]
    public class VersionDependencies
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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "Release",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", "Release" }
                    }
                    };

                    Patcher.foundMods = new List<QMod>
                {
                    mod1,
                    mod2,
                };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "Release",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", "=Release" }
                    }
                    };

                    Patcher.foundMods = new List<QMod>
                {
                    mod1,
                    mod2,
                };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "Release",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", " = Release" }
                    }
                    };

                    Patcher.foundMods = new List<QMod>
                {
                    mod1,
                    mod2,
                };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "Release",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", "AnotherVersion" }
                    }
                    };

                    Patcher.foundMods = new List<QMod>
                {
                    mod1,
                    mod2,
                };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "Release",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", "=AnotherVersion" }
                    }
                    };

                    Patcher.foundMods = new List<QMod>
                {
                    mod1,
                    mod2,
                };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "Release",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", " = AnotherVersion" }
                    }
                    };

                    Patcher.foundMods = new List<QMod>
                {
                    mod1,
                    mod2,
                };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.0",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", "1.0" }
                    }
                    };

                    Patcher.foundMods = new List<QMod>
                {
                    mod1,
                    mod2,
                };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.0",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", "=1.0" }
                    }
                    };

                    Patcher.foundMods = new List<QMod>
                {
                    mod1,
                    mod2,
                };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.0",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", " = 1.0" }
                    }
                    };

                    Patcher.foundMods = new List<QMod>
                {
                    mod1,
                    mod2,
                };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.1",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", "1.0" }
                    }
                    };

                    Patcher.foundMods = new List<QMod>
                {
                    mod1,
                    mod2,
                };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.1",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", "=1.0" }
                    }
                    };

                    Patcher.foundMods = new List<QMod>
                {
                    mod1,
                    mod2,
                };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.1",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                    {
                        { "Mod1", " = 1.0" }
                    }
                    };

                    Patcher.foundMods = new List<QMod>
                {
                    mod1,
                    mod2,
                };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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
                [Test] [Ignore("Broken")]
                public void MatchSuccess_1()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", ">1.2" }
                        }
                    };

                    Patcher.foundMods = new List<QMod>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", "<1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QMod>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", ">1.2.2 <1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QMod>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.2.2",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", ">=1.2.3 <1.3.0 || =1.2.2" }
                        }
                    };

                    Patcher.foundMods = new List<QMod>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", ">=1.2 <=1.3" }
                        }
                    };

                    Patcher.foundMods = new List<QMod>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
                [Test] [Ignore("Broken")]
                public void MatchSuccess_1_WithSpace()
                {
                    Patcher.loadedMods.Clear();
                    Patcher.foundMods.Clear();
                    Patcher.sortedMods.Clear();
                    Patcher.erroredMods.Clear();

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " > 1.2" }
                        }
                    };

                    Patcher.foundMods = new List<QMod>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " < 1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QMod>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " > 1.2.2 < 1.4" }
                        }
                    };

                    Patcher.foundMods = new List<QMod>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.2.2",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " >= 1.2.3 < 1.3.0 || = 1.2.2" }
                        }
                    };

                    Patcher.foundMods = new List<QMod>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

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

                    QMod mod1 = new QMod
                    {
                        Id = "Mod1",
                        Version = "1.2.3",
                    };

                    QMod mod2 = new QMod
                    {
                        Id = "Mod2",
                        VersionDependencies = new Dictionary<string, string>()
                        {
                            { "Mod1", " >= 1.2 <= 1.3" }
                        }
                    };

                    Patcher.foundMods = new List<QMod>
                    {
                        mod1,
                        mod2,
                    };

                    Patcher.sortedMods = new List<QMod>(Patcher.foundMods);

                    Patcher.CheckForDependencies();

                    Assert.IsTrue(Patcher.erroredMods.Count == 0);
                }
            }
        }
    }
}
