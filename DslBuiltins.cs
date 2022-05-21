namespace NET_SandboxExperiments.Builtins
{
    public interface IDslBuiltins
    {
        public int len(string input);
    }

    public static class DslBuiltins
    {
        public static int len(string input)
        {
            return input.Length;
        }
    }
}
