﻿namespace Alba.CsConsoleFormat
{
    //[TrimSurroundingWhitespace]
    public class Br : InlineElement
    {
        protected override bool CanHaveChildren => false;

        public override string GeneratedText => "\n";

        public override void GenerateSequence (IInlineSequence sequence)
        {
            sequence.AppendText("\n");
        }
    }
}