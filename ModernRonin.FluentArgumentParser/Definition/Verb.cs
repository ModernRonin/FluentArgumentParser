namespace ModernRonin.FluentArgumentParser.Definition
{
    public class Verb : AVerbContainer
    {
        public Verb() : this(default) { }
        public Verb(string name) => Name = name;
        public string Name { get; set; }
        public string HelpText { get; set; } = string.Empty;
        public AParameter[] Parameters { get; set; } = new AParameter[0];
    }
}