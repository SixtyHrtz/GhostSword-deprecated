namespace GhostSword.Types.Sequences
{
    public class CharSequence : Sequence<char, string>
    {
        public CharSequence(string sequence) : base(sequence) { }

        protected override string CreateSubSequence(char[] array, int start, int length) =>
            new string(array, start, length);

        protected override char GetDefault() => '\0';
    }
}
