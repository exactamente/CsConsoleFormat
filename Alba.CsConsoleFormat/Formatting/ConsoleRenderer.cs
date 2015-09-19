﻿using System;
using System.Linq;

namespace Alba.CsConsoleFormat
{
    public static class ConsoleRenderer
    {
        public static Size ConsoleBufferSize => new Size(Console.BufferWidth, Console.BufferHeight);

        public static Size ConsoleLargestWindowSize => new Size(Console.LargestWindowWidth, Console.LargestWindowHeight);

        public static Rect DefaultRenderRect => new Rect(0, 0, Console.BufferWidth, Size.Infinity);

        public static Point ConsoleCursorPosition
        {
            get { return new Point(Console.CursorLeft, Console.CursorTop); }
            set
            {
                Console.CursorLeft = value.X;
                Console.CursorTop = value.Y;
            }
        }

        public static Rect ConsoleWindowRect
        {
            get { return new Rect(Console.WindowLeft, Console.WindowTop, Console.WindowWidth, Console.WindowHeight); }
            set
            {
                Console.SetWindowPosition(value.Left, value.Top);
                Console.SetWindowSize(value.Width, value.Height);
            }
        }

        public static void RenderDocument (Document document, IRenderTarget target = null)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            if (target == null)
                target = new ConsoleRenderTarget();
            Rect renderRect = DefaultRenderRect;
            var buffer = new ConsoleBuffer(renderRect.Size.Width);
            RenderDocumentToBuffer(document, buffer, renderRect);
            target.Render(buffer);
        }

        public static string RenderDocumentToText (Document document, TextRenderTargetBase target)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            Rect renderRect = DefaultRenderRect;
            var buffer = new ConsoleBuffer(renderRect.Size.Width);
            RenderDocumentToBuffer(document, buffer, renderRect);
            target.Render(buffer);
            return target.OutputText;
        }

        public static void RenderDocumentToBuffer (Document document, ConsoleBuffer buffer, Rect renderRect)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            document.GenerateVisualTree();
            document.Measure(renderRect.Size);
            document.Arrange(renderRect);
            RenderElement(document, buffer, new Vector(0, 0), renderRect);
        }

        private static void RenderElement (BlockElement element, ConsoleBuffer buffer, Vector parentOffset, Rect renderRect)
        {
            Vector offset = parentOffset + element.ActualOffset;
            if (element.Visibility == Visibility.Visible && !element.RenderSize.IsEmpty) {
                buffer.Clip = new Rect(element.RenderSize).Intersect(element.LayoutClip).Offset(offset).Intersect(renderRect);
                buffer.Offset = offset;
                element.Render(buffer);
                foreach (BlockElement childElement in element.VisualChildren.OfType<BlockElement>())
                    RenderElement(childElement, buffer, offset, renderRect);
            }
        }
    }
}