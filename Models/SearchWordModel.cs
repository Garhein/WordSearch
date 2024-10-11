using Microsoft.AspNetCore.Html;

namespace WordSearch.Models
{
    [Serializable]
    public class SearchWordModel
    {
        public Grid Grid { get; set; }

        /// <summary>
        /// Constructeur vide.
        /// </summary>
        public SearchWordModel() { }

        public HtmlString PrintCharacter(int pos)
        {
            string retChar = this.Grid.GridCells[pos];

            if (string.IsNullOrWhiteSpace(retChar))
            {
                retChar = "&nbsp;";
            }

            return new HtmlString(retChar);
        }
    }
}