namespace MinecraftTranslatorTool.Util {
    public static class MTUtil {
        public static string ReplaceLast(this string source, string find, string replace) {
            int index = source.LastIndexOf(find);

            if (index == -1) return source;

            string result = source.Remove(index, find.Length).Insert(index, replace);
            return result;
        }

        public static string RemoveLast(this string source, string find) {
            int index = source.LastIndexOf(find);

            if (index == -1) return source;

            string result = source.Remove(index, find.Length);
            return result;
        }
    }
}
