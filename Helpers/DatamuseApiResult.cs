namespace WordSearch.Helpers
{
    [Serializable]
    public class DatamuseApiResult
    {
        public string   word { get; set; }
        public int      score { get; set; }

        /// <summary>
        /// Constructeur par défaut.
        /// </summary>
        public DatamuseApiResult()
        {
            this.word   = string.Empty;
            this.score  = 0;
        }
    }
}
