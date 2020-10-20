namespace MinecraftTranslatorTool.Util {
    public static class MTUtil {
        public static string ReplaceLast(this string source, string find, string replace) {
            int place = source.LastIndexOf(find);

            if (place == -1) return source;

            string result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }
    }
}
