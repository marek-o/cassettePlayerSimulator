using System.IO;

namespace cassettePlayerSimulator
{
    internal class Translations
    {
        private static NGettext.Catalog catalog;

        public static void InitTranslations()
        {
            catalog = new NGettext.Catalog();
        }

        public static void InitTranslations(Stream moFile)
        {
            catalog = new NGettext.Catalog(moFile);
        }

        public static string _(string s)
        {
            if (catalog != null)
            {
                return catalog.GetString(s);
            }

            return s;
        }
    }
}
