namespace WordSearch.Extensions
{
    /// <summary>
    /// Méthodes d'extension du type <see cref="string"/>.
    /// </summary>
    public static class StringExtension
    {
        private static readonly char[] ALPHABET_CHARS = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        /// <summary>
        /// Vérifie si la chaîne contient uniquement des caractères alphabétique (A-Z).
        /// </summary>
        /// <param name="value"></param>
        /// <returns><see langword="true"/> si <paramref name="value"/> contient uniquement des caractères alphabétique, sinon <see langword="false"/>.</returns>
        public static bool ContainsOnlyAlphabetChars(this string value)
        {
            return !value.ToCharArray().Any(x => !StringExtension.ALPHABET_CHARS.Contains(x));
        }
    }
}
