using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostSword.Types.Sequences
{
    public abstract class Sequence<TItem, TSequence> where TSequence : IEnumerable<TItem>
    {
        private TItem[] sequence;
        private int index = 0;

        public TItem Current { get { return index >= sequence.Length ? GetDefault() : sequence[index]; } }
        public int Length { get { return sequence.Length; } }
        public bool IsEos { get { return index >= sequence.Length; } }

        protected Sequence(TSequence sequence) => this.sequence = sequence.ToArray();

        protected abstract TItem GetDefault();
        protected abstract TSequence CreateSubSequence(TItem[] array, int start, int length);

        public bool TryPeek(out TItem item)
        {
            if (CheckOutOfRange(1, false))
            {
                item = Peek();
                return true;
            }
            else
            {
                item = GetDefault();
                return false;
            }
        }

        public bool TryPeek(int length, out TSequence sequence)
        {
            if (CheckOutOfRange(length, false))
            {
                sequence = CreateSubSequence(this.sequence, index, length);
                return true;
            }
            else
            {
                sequence = default(TSequence);
                return false;
            }
        }

        private bool CheckOutOfRange(int length, bool throwException)
        {
            if (index + length > sequence.Length)
            {
                if (throwException)
                    throw new ArgumentOutOfRangeException();
                return false;
            }

            return true;
        }

        private TItem Peek() => sequence[index + 1];

        public TItem Pop()
        {
            var value = Current;
            Increment(1);
            return value;
        }

        public TSequence Pop(int length)
        {
            var result = CreateSubSequence(sequence, index, length);
            Increment(length);
            return result;
        }

        public TSequence PopRest() => Pop(sequence.Length - index);

        private void Increment(int length)
        {
            CheckOutOfRange(length, true);
            index += length;
        }

        public override string ToString() => $"{Resources.Current}: {Current}; {Resources.Length}: {Length}; {Resources.EndOfSequence}: {IsEos}";
    }
}
