using GhostSword.Types;
using System.Reflection;

namespace GhostSword
{
    public class ArgumentContext
    {
        private Argument[] arguments;
        protected int currentIndex;

        public int ArgumentCount { get { return arguments.Length; } }

        public ArgumentContext(Argument[] arguments)
        {
            if (arguments == null)
                arguments = new Argument[0];
            this.arguments = arguments;
        }

        public virtual Data<object> TryResolve(ParameterInfo parameterInfo)
        {
            if (arguments.Length == 0)
                return Data<object>.CreateError(Resources.ArgumentsNotFound);
            if (currentIndex < 0 || currentIndex >= arguments.Length)
                return Data<object>.CreateError(Resources.ArgumentIndexOutOfBound);

            var value = arguments[currentIndex++].Value;
            if (!parameterInfo.ParameterType.IsAssignableFrom(value.GetType()))
                return Data<object>.CreateError(Resources.ArgumentInvalidType);

            return Data<object>.CreateValid(value);
        }
    }
}
