namespace WordSearch
{
    /// <summary>
    /// Méthodes d'extension du type <see cref="string"/>.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Vérifie si la chaîne contient au moins un caractère interdit.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="forbiddenChars">Liste des caractères interdits.</param>
        /// <returns><see langword="true"/> si <paramref name="value"/> contient au moins un caractère interdit, sinon <see langword="false"/>.</returns>
        public static bool ContainsForbiddenChar(this string value, char[] forbiddenChars)
        {
            char[] chars = value.ToCharArray();
            return chars.Any(x => forbiddenChars.Contains(x));
        }
    }
}
