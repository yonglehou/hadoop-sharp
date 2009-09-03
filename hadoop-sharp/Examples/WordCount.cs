using System;
using System.IO;
using System.Text;

using Hadoop;
using Hadoop.IO;
using Hadoop.Pipes;

public class WordCount
{
    private static Encoding encoding = new UTF8Encoding();

    private class WordCountFactory : Factory
    {
        public override Reducer CreateReducer(ReduceContext context)
        {
            return new WordCountReducer();
        }

        public override Mapper CreateMapper(MapContext context)
        {
            return new WordCountMapper();
        }
    }

    private class WordCountMapper : Mapper
    {
        private static readonly char[] splitChars = { ' ', '\r', '\n', '\t', '\x0085' };

        public void Map(MapContext context)
        {
            foreach (string word in encoding.GetString(context.InputValue).Split(splitChars, StringSplitOptions.RemoveEmptyEntries))
                context.Emit(encoding.GetBytes(word), encoding.GetBytes("1"));
        }
    }

    private class WordCountReducer : Reducer
    {
        public void Reduce(ReduceContext context)
        {
            long sum = 0;

            while (context.NextValue())
            {
                sum += Convert.ToInt64(encoding.GetString(context.InputValue));
            }

            context.Emit(context.InputKey, encoding.GetBytes(sum.ToString()));
        }
    }

    public static void Main(string[] args)
    {
        Driver.RunTask(new WordCountFactory());
    }
}