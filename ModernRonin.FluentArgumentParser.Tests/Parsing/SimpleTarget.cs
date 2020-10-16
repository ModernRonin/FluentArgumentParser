namespace ModernRonin.FluentArgumentParser.Tests.Parsing
{
    public class SimpleTarget : TargetAncestor
    {
        public float OptionalReal { get; set; } = 3.141f;
        public string Text { get; set; }
        public int Number { get; set; }
        public float Real { get; set; }
        public Color Color { get; set; }
        public bool IsOn { get; set; }

        /// <summary>
        ///     To prove that get-only properties create no problems
        /// </summary>
        public bool IsNotOn => !IsOn;

        /// <summary>
        ///     To prove that only public properties are considered
        /// </summary>
        internal int InternalProperty { get; set; }
    }
}