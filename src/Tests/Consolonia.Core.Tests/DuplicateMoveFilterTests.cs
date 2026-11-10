using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Consolonia.Core.Helpers.InputProcessing;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
    [TestFixture]
    public class DuplicateMoveFilterTests
    {
        /// <summary>The exact flood from the Gnome Terminal report: many reports, one cell.</summary>
        private static readonly Point Cell = new(27, 22);

        [Test]
        public void ConsecutiveMovesWithTheSamePositionAreDropped()
        {
            var filter = new DuplicateMoveFilter();

            Assert.That(filter.Passes(RawPointerEventType.Move, Cell, RawInputModifiers.None), Is.True);
            Assert.That(filter.Passes(RawPointerEventType.Move, Cell, RawInputModifiers.None), Is.False);
            Assert.That(filter.Passes(RawPointerEventType.Move, Cell, RawInputModifiers.None), Is.False);
        }

        [Test]
        public void MoveToAnotherCellPasses()
        {
            var filter = new DuplicateMoveFilter();
            Point next = new(28, 22);

            Assert.That(filter.Passes(RawPointerEventType.Move, Cell, RawInputModifiers.None), Is.True);
            Assert.That(filter.Passes(RawPointerEventType.Move, next, RawInputModifiers.None), Is.True);
        }

        [Test]
        public void MoveWithDifferentModifiersPasses()
        {
            var filter = new DuplicateMoveFilter();

            Assert.That(filter.Passes(RawPointerEventType.Move, Cell, RawInputModifiers.None), Is.True);
            Assert.That(filter.Passes(RawPointerEventType.Move, Cell, RawInputModifiers.Shift), Is.True);
        }

        [Test]
        public void PressResetsTheFilterSoTheMoveAfterItIsReported()
        {
            var filter = new DuplicateMoveFilter();

            Assert.That(filter.Passes(RawPointerEventType.Move, Cell, RawInputModifiers.None), Is.True);
            Assert.That(filter.Passes(RawPointerEventType.Move, Cell, RawInputModifiers.None), Is.False);
            // button press, then the drag continues from the same cell with the button modifier
            Assert.That(filter.Passes(RawPointerEventType.LeftButtonDown, Cell, RawInputModifiers.LeftMouseButton), Is.True);
            Assert.That(filter.Passes(RawPointerEventType.Move, Cell, RawInputModifiers.LeftMouseButton), Is.True);
        }

        [Test]
        public void WheelAndReleaseResetTheFilter()
        {
            var filter = new DuplicateMoveFilter();

            Assert.That(filter.Passes(RawPointerEventType.Move, Cell, RawInputModifiers.None), Is.True);
            Assert.That(filter.Passes(RawPointerEventType.Wheel, Cell, RawInputModifiers.None), Is.True);
            Assert.That(filter.Passes(RawPointerEventType.Move, Cell, RawInputModifiers.None), Is.True);

            Assert.That(filter.Passes(RawPointerEventType.LeftButtonUp, Cell, RawInputModifiers.None), Is.True);
            Assert.That(filter.Passes(RawPointerEventType.Move, Cell, RawInputModifiers.None), Is.True);
        }
    }
}
