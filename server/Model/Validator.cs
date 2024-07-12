namespace Server.Model
{
    public class Validator<T>
    {
        public T Value { get; set; }

        public int TotalPage { get; set; }

        public int TotalCount { get; set; }

        public string Error { get; set; }
    }
}
