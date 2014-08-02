﻿using System;
using GitDiffMargin.Git;
using GitDiffMargin.View;
using GitDiffMargin.ViewModel;
using Microsoft.VisualStudio.Text.Editor;

namespace GitDiffMargin
{
    internal sealed class ScrollDiffMargin : DiffMarginBase
    {
        private readonly IVerticalScrollBar _scrollBar;
        private const double MarginWidth = 10.0;

        public const string MarginNameConst = "ScrollDiffMargin";

        protected override string MarginName
        {
            get { return MarginNameConst; }
        }

        internal ScrollDiffMargin(IWpfTextView textView, IMarginCore marginCore, IWpfTextViewMargin containerMargin)
            : base(textView, marginCore)
        {
            var scrollBarMargin = containerMargin.GetTextViewMargin(PredefinedMarginNames.VerticalScrollBar);
            // ReSharper disable once SuspiciousTypeConversion.Global
            _scrollBar = (IVerticalScrollBar)scrollBarMargin;

            ViewModel = new ScrollDiffMarginViewModel(marginCore, UpdateDiffDimensions);

            UserControl = new ScrollDiffMarginControl {DataContext = ViewModel, Width = MarginWidth};
        }

        private void UpdateDiffDimensions(DiffViewModel diffViewModel, HunkRangeInfo hunkRangeInfo)
        {
            if (TextView.IsClosed)
                return;

            var startLineNumber = hunkRangeInfo.NewHunkRange.StartingLineNumber;
            var endLineNumber = startLineNumber + hunkRangeInfo.NewHunkRange.NumberOfLines - 1;

            var snapshot = TextView.TextBuffer.CurrentSnapshot;

            var startLine = snapshot.GetLineFromLineNumber(startLineNumber);
            var endLine = snapshot.GetLineFromLineNumber(endLineNumber);

            //var mapTop = _scrollBar.Map.GetBufferPositionAtFraction(_hunkRangeInfo.NewHunkRange.StartingLineNumber) - 0.5;
            //var mapBottom = _scrollBar.Map.GetBufferPositionAtFraction(_hunkRangeInfo.NewHunkRange.StartingLineNumber + _hunkRangeInfo.NewHunkRange.NumberOfLines - 1) + 0.5;

            var mapTop = _scrollBar.Map.GetCoordinateAtBufferPosition(startLine.Start) - 0.5;
            var mapBottom = _scrollBar.Map.GetCoordinateAtBufferPosition(endLine.End) + 0.5;

            diffViewModel.Top = Math.Round(_scrollBar.GetYCoordinateOfScrollMapPosition(mapTop)) - 2.0;
            diffViewModel.Height = Math.Round(_scrollBar.GetYCoordinateOfScrollMapPosition(mapBottom)) - diffViewModel.Top + 2.0;
        }
    }
}