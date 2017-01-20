using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acerva.Tests.Modelo
{
    [TestClass]
    public abstract class EqualsHashCodeContract<T> : EqualsHashCodeContractTest<T> where T : class
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestMethod]
        public override void TestEqualsAgainstNewObject()
        {
            base.TestEqualsAgainstNewObject();
        }

        [TestMethod]
        public override void TestEqualsAgainstNull()
        {
            base.TestEqualsAgainstNull();
        }

        [TestMethod]
        public override void TestEqualsAgainstUnequalObjects()
        {
            base.TestEqualsAgainstUnequalObjects();
        }

        [TestMethod]
        public override void TestEqualsIsReflexive()
        {
            base.TestEqualsIsReflexive();
        }

        [TestMethod]
        public override void TestEqualsIsSymmetricAndTransitive()
        {
            base.TestEqualsIsSymmetricAndTransitive();
        }

        [TestMethod]
        public override void TestEqualsIsConsistentAcrossInvocations()
        {
            base.TestEqualsIsConsistentAcrossInvocations();
        }

        [TestMethod]
        public override void TestHashCodeContract()
        {
            base.TestEqualsIsConsistentAcrossInvocations();
        }

        [TestMethod]
        public override void TestGetClass()
        {
            base.TestEqualsIsConsistentAcrossInvocations();
        }

        [TestMethod]
        public override void TestHashCodeIsConsistentAcrossInvocations()
        {
            base.TestEqualsIsConsistentAcrossInvocations();
        }

        [TestMethod]
        public override void TestEqualsAgainstSameReference()
        {
            base.TestEqualsIsConsistentAcrossInvocations();
        }
    }
}
