namespace GhostSword.Types
{
    public class Data<T>
    {
        public T Value { get; private set; }
        public bool IsValid { get; private set; }
        public Error Error { get; private set; }

        public static Data<T> CreateValid(T data) =>
            new Data<T> { Value = data, IsValid = true, Error = null };

        public static Data<T> CreateError(string message) =>
            new Data<T> { Value = default(T), IsValid = false, Error = new Error(message) };

        public static Data<T> CreateError(Error error) =>
            new Data<T> { Value = default(T), IsValid = false, Error = error };
    }
}
