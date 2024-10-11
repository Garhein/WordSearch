using Microsoft.AspNetCore.Html;

namespace WordSearch.Models
{
    [Serializable]
    public class SearchWordModel
    {
        public Grid Grid { get; set; }

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="gridLength">Longueur de la grille.</param>
        public SearchWordModel(int gridLength) 
        {
            this.Grid = new Grid(gridLength);
        }

        public HtmlString PrintCharacter(int pos)
        {
            string retChar = this.Grid.GridCells[pos];

            if (string.IsNullOrWhiteSpace(retChar))
            {
                retChar = "&nbsp;";
            }
            else
            {
                retChar = retChar.ToUpper();
            }

            return new HtmlString(retChar);
        }
    }
}