using System.Threading;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;

namespace Consolonia.Core.Helpers.InputProcessing
{
    /// <summary>
    ///     Drops consecutive mouse move events with the same position and modifiers.
    ///     Terminals such as Gnome Terminal (VTE) send a motion report on every pointer movement while
    ///     all-motion tracking (<c>CSI ? 1003 h</c>) is enabled, including movements that do not leave the
    ///     current cell, so one physical move arrives as many reports with identical coordinates (#728).
    /// </summary>
    public sealed class DuplicateMoveFilter
    {
        private readonly object _sync = new();

        private bool _lastWasMove;
        private Point _lastMovePosition;
        private RawInputModifiers _lastMoveModifiers;

        /// <summary>
        ///     Returns <see langword="false" /> when the event repeats the last passed move exactly.
        ///     Any non-move event resets the filter, so a move after a press, release or wheel is always reported.
        /// </summary>
        public bool Passes(RawPointerEventType type, Point position, RawInputModifiers modifiers)
        {
            lock (_sync)
            {
                if (type != RawPointerEventType.Move)
                {
                    _lastWasMove = false;
                    return true;
                }

                if (_lastWasMove && position == _lastMovePosition && modifiers == _lastMoveModifiers)
                    return false;

                _lastWasMove = true;
                _lastMovePosition = position;
                _lastMoveModifiers = modifiers;
                return true;
            }
        }
    }
}
