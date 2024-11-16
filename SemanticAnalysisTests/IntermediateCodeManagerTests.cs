using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysisTests
{
    public class IntermediateCodeManagerTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void RegisterForBackpatching_WhenInstructionIsAlreadyRegistered_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Backpatch_WhenLabelIdIsSmallerThanFirstId_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Backpatch_WhenLabelIdIsGreaterOrEqualThanNextId_ShouldThrowException()
        {
            Assert.Fail();
        }

        [Test]
        public void Backpatch_WhenJumpLabelIsNotNull_ShouldThrowException()
        {
            Assert.Fail();
        }
    }
}
