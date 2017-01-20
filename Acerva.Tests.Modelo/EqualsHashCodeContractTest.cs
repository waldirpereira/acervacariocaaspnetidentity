using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acerva.Tests.Modelo
{
    [TestClass]
    public abstract class EqualsHashCodeContractTest<T> where T : class
    {
        private const int NumIterations = 1;
        private T _eq1;
        private T _eq2;
        private T _eq3;

        //private Object _neq;
        /* instancias diferentes */
        private IEnumerable<T> _notEqualInstances;

        private object _refIgual;

        /**
         * Método que retorna uma instância do objeto a ser testado.
         * 
         * @return T - instancia do objeto a ser testado.
         */
        protected abstract T CreateInstance();

        /**
         * Método que retorna uma lista de objetos diferentes (em termos de equals e hashcode) do primeiro
         * objeto.
         * 
         * @return List<T> - lista de objetos diferentes (em termos de equals e hashcode) do primeiro
         * objeto.
         */
        protected abstract IEnumerable<T> CreateNotEqualInstances();

        [TestInitialize]
        public virtual void TestInitialize()
        {
            _eq1 = CreateInstance();
            _eq2 = CreateInstance();
            _eq3 = CreateInstance();
            _notEqualInstances = CreateNotEqualInstances();


            Assert.IsNotNull(_eq1, "1st CreateInstance() returned null");
            Assert.IsNotNull(_eq2, "2nd CreateInstance() returned null");
            Assert.IsNotNull(_eq3, "3rd CreateInstance() returned null");
            Assert.IsNotNull(_notEqualInstances, "createNotEqualInstance() returned null");

            Assert.AreNotSame(_eq1, _eq2);
            Assert.AreNotSame(_eq1, _eq3);
            Assert.AreNotSame(_eq2, _eq3);

            foreach (var neq in _notEqualInstances)
            {
                Assert.AreNotSame(_eq1, neq);
                Assert.AreNotSame(_eq2, neq);
                Assert.AreNotSame(_eq3, neq);
            }
        }

        [TestMethod]
        public virtual void TestEqualsAgainstNewObject()
        {
            var o = new object();

            Assert.IsFalse(o.Equals(_eq1));
            Assert.IsFalse(o.Equals(_eq2));
            Assert.IsFalse(o.Equals(_eq3));
            foreach (var neq in _notEqualInstances)
            {
                Assert.IsFalse(o.Equals(neq));
            }
        }

        [TestMethod]
        public virtual void TestEqualsAgainstNull()
        {
            object nullReference = null;
            Assert.IsFalse(_eq1.Equals(nullReference));
            Assert.IsFalse(_eq2.Equals(nullReference));
            Assert.IsFalse(_eq3.Equals(nullReference));
            foreach (var neq in _notEqualInstances)
            {
                Assert.IsFalse(neq.Equals(nullReference));
            }
        }

        [TestMethod]
        public virtual void TestEqualsAgainstUnequalObjects()
        {
            foreach (var neq in _notEqualInstances)
            {
                Assert.IsFalse(_eq1.Equals(neq));
                Assert.IsFalse(_eq2.Equals(neq));
                Assert.IsFalse(_eq3.Equals(neq));

                Assert.IsFalse(neq.Equals(_eq1));
                Assert.IsFalse(neq.Equals(_eq2));
                Assert.IsFalse(neq.Equals(_eq3));
            }
        }

        [TestMethod]
        public virtual void TestEqualsIsReflexive()
        {
            Assert.AreEqual(_eq1, _eq1, "1st equal instance");
            Assert.AreEqual(_eq2, _eq2, "2nd equal instance");
            Assert.AreEqual(_eq3, _eq3, "3rd equal instance");
            foreach (var neq in _notEqualInstances)
            {
                Assert.AreEqual(neq, neq, "not-equal instance");
            }
        }

        [TestMethod]
        public virtual void TestEqualsIsSymmetricAndTransitive()
        {
            Assert.AreEqual(_eq1, _eq2, "1st vs. 2nd");
            Assert.AreEqual(_eq2, _eq1, "2nd vs. 1st");

            Assert.AreEqual(_eq1, _eq3, "1st vs. 3rd");
            Assert.AreEqual(_eq3, _eq1, "3rd vs. 1st");

            Assert.AreEqual(_eq2, _eq3, "2nd vs. 3rd");
            Assert.AreEqual(_eq3, _eq2, "3rd vs. 2nd");
        }

        [TestMethod]
        public virtual void TestEqualsIsConsistentAcrossInvocations()
        {
            for (var i = 0; i < NumIterations; ++i)
            {
                TestEqualsAgainstNewObject();
                TestEqualsAgainstNull();
                TestEqualsAgainstUnequalObjects();
                TestEqualsIsReflexive();
                TestEqualsIsSymmetricAndTransitive();
            }
        }

        [TestMethod]
        public virtual void TestHashCodeContract()
        {
            Assert.AreEqual(_eq1.GetHashCode(), _eq2.GetHashCode(), "1st vs. 2nd");
            Assert.AreEqual(_eq1.GetHashCode(), _eq3.GetHashCode(), "1st vs. 3rd");
            Assert.AreEqual(_eq2.GetHashCode(), _eq3.GetHashCode(), "2nd vs. 3rd");
        }

        [TestMethod]
        public virtual void TestGetClass()
        {
            Assert.IsFalse(_eq1.Equals(new EqualsHashCodeEmptyClass()));
            Assert.IsFalse(_eq2.Equals(new EqualsHashCodeEmptyClass()));
            Assert.IsFalse(_eq3.Equals(new EqualsHashCodeEmptyClass()));
            foreach (var neq in _notEqualInstances)
            {
                Assert.IsFalse(neq.Equals(new EqualsHashCodeEmptyClass()));
            }
        }

        [TestMethod]
        public virtual void TestHashCodeIsConsistentAcrossInvocations()
        {
            var eq1Hash = _eq1.GetHashCode();
            var eq2Hash = _eq2.GetHashCode();
            var eq3Hash = _eq3.GetHashCode();

            for (var i = 0; i < NumIterations; ++i)
            {
                Assert.AreEqual(eq1Hash, _eq1.GetHashCode(), "1st equal instance");
                Assert.AreEqual(eq2Hash, _eq2.GetHashCode(), "2nd equal instance");
                Assert.AreEqual(eq3Hash, _eq3.GetHashCode(), "3rd equal instance");
                foreach (var neq in _notEqualInstances)
                {
                    var neqHash = neq.GetHashCode();
                    Assert.AreEqual(neqHash, neq.GetHashCode(), "not-equal instance");
                }
            }
        }

        [TestMethod]
        public virtual void TestEqualsAgainstSameReference()
        {
            _refIgual = _eq3;

            Assert.IsNotNull(_refIgual, "Ref. igual returned null");
            var referenciasIguais = _eq3.Equals(_refIgual);
            Assert.AreEqual(referenciasIguais, true);
        }
    }

    public class EqualsHashCodeEmptyClass
    {
    }
}