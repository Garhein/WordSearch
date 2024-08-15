namespace WordSearch.Models
{
    [Serializable]
    public class Word
    {
        public eWordOrientation Orientation { get; set; } 
        public int              StartPos { get; set; }
        public int              EndPos { get; set; }
        public bool             IsReversed { get; set; }
        public string           WordText { get; set; }

        /// <summary>
        /// Constructeur par défaut.
        /// </summary>
        public Word() 
        {
            this.Orientation    = eWordOrientation.HORIZONTAL;
            this.StartPos       = -1;
            this.EndPos         = -1;
            this.IsReversed     = false;
            this.WordText       = string.Empty;
        }

        /// <summary>
        /// Récupération du mot d'origine.<br/>
        /// Si le mot a été inversé, il est de nouveau inversé pour obtenir le mot d'origine.
        /// </summary>
        public string WordTextBase
        {
            get
            {
                return this.IsReversed ? new string(this.WordText.Reverse().ToArray()) : this.WordText;
            }
        }
    }

    /// <summary>
    /// Orientation d'un mot.
    /// </summary>
    public enum eWordOrientation : int
    {
        HORIZONTAL              = 1,
        VERTICAL                = 2,
        DIAGONAL_LEFT_TO_RIGHT  = 3,
        DIAGONAL_RIGHT_TO_LEFT  = 4,
    }
}
