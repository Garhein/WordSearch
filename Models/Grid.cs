using System.Text.Json;
using WordSearch.Helpers;

namespace WordSearch.Models
{
    // TODO: si des cellules sont vides à la fin de la génération ?

    [Serializable]
    public class Grid
    {
        public const int MIN_WORD_LENGTH = 4;
        public const int MAX_WORD_LENGTH = 12;
        public const int DEFAULT_GRID_LENGTH = 12;
        public const string CHAR_PATTERN = "?";

        // Longueur de la grille
        private int _gridLength;
        // Hauteur de la grille
        private int _gridHeigth;
        // Taille maximale des mots
        private int _maxWordLength;
        // Cellules de la grille, chaque cellule contenant une lettre
        private string[] _gridCells;
        // Liste des mots à trouver
        private List<Word> _wordList;
        // Numéros des colonnes d'après les index des cellules
        private int[] _columnsNumber;

        /// <summary>
        /// Longueur de la grille.
        /// </summary>
        public int GridLength => this._gridLength;

        /// <summary>
        /// Hauteur de la grille.
        /// </summary>
        public int GridHeigth => this._gridHeigth;

        /// <summary>
        /// Cellules de la grille.
        /// </summary>
        public string[] GridCells => this._gridCells;

        /// <summary>
        /// Liste des mots à trouver.
        /// </summary>
        public List<Word> WordList => this._wordList;

        /// <summary>
        /// Indique si la grille est valide, c'est-à-dire si toutes les cellules contiennent une lettre.
        /// </summary>
        public bool IsValid => !this._gridCells.Any(x => string.IsNullOrWhiteSpace(x));

        /// <summary>
        /// Nombre de cellules de la grille.
        /// </summary>
        private int NumberOfCells => this._gridLength * this._gridHeigth;

        /// <summary>
        /// Liste des caractères interdits.
        /// </summary>
        private char[] ForbiddenChars => new char[12] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ', '-' };

        /// <summary>
        /// Constructeur par défaut.
        /// </summary>
        public Grid() : this(Grid.DEFAULT_GRID_LENGTH) { }
        
        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="gridLength">Longueur de la grille.</param>
        public Grid(int gridLength)
        {
            if (gridLength < Grid.MIN_WORD_LENGTH)
            {
                throw new ArgumentException($"La longueur de la grille doit être supérieure ou égale à {Grid.MIN_WORD_LENGTH} caractères.");
            }

            this._gridLength        = gridLength;
            this._gridHeigth        = gridLength;
            this._wordList          = new List<Word>();
            this._gridCells         = new string[gridLength * gridLength];
            this._columnsNumber     = new int[2 * gridLength * gridLength];
            this._maxWordLength     = gridLength <= Grid.MAX_WORD_LENGTH ? gridLength : Grid.MAX_WORD_LENGTH;

            // Index des colonnes => pour gérer les dépassements en bas
            for (int i = 0; i < (2 * this.NumberOfCells); i++)
            {
                this._columnsNumber[i] = (i % gridLength) + 1;
            }
        }

        /// <summary>
        /// Génération de la grille.
        /// </summary>
        public void Generate()
        {
            // Cellule de départ tirée au hasard
            int wordStartPos = RandomHelper.GenerateRandomValue(0, this.NumberOfCells - 1);

            // Parcours des cellules
            int pos = 0;

            while (pos < this.NumberOfCells)
            {
                this.PlaceWord(wordStartPos);

                pos++; 
                wordStartPos++;

                // Fin de la grille atteinte : on repart du début pour gérer les cellules non traitées
                // du fait de l'utilisation d'une cellule de départ tirée au hasard
                if (wordStartPos == this.NumberOfCells)
                {
                    wordStartPos = 0;
                }
            }
        }

        /// <summary>
        /// Placement d'un mot dans la grille.
        /// </summary>
        /// <param name="wordStartPos">Index de la cellule contenant le premier caractère du mot à placer.</param>
        private void PlaceWord(int wordStartPos)
        {
            int wordLength  = RandomHelper.GenerateRandomValue(Grid.MIN_WORD_LENGTH, this._maxWordLength);
            int increment   = 1;

            Word word  = new Word()
            {
                Orientation = (eWordOrientation)RandomHelper.GenerateRandomValue((int)eWordOrientation.HORIZONTAL, (int)eWordOrientation.VERTICAL),
                StartPos    = wordStartPos,
                IsReversed  = RandomHelper.GenerateRandomValue(0, 1) == 1
            };

            // Calcul des positions de départ et de fin
            switch (word.Orientation)
            {
                case eWordOrientation.HORIZONTAL:
                    {
                        word.EndPos = word.StartPos + wordLength - 1;
                        
                        // Le mot est placé sur 2 lignes => on décale à gauche
                        while (this._columnsNumber[word.EndPos] < this._columnsNumber[word.StartPos])
                        {
                            word.StartPos   = word.StartPos - 1;
                            word.EndPos     = word.StartPos + wordLength - 1;
                        }

                        break;
                    }
                case eWordOrientation.VERTICAL:
                    {
                        increment   = this._gridLength;
                        word.EndPos = word.StartPos + (wordLength * this._gridLength) - this._gridLength;

                        // Le mot dépasse la grille en bas => on décale vers le haut
                        while (word.EndPos > this._gridCells.Length - 1)
                        {
                            word.StartPos   = word.StartPos - this._gridLength;
                            word.EndPos     = word.StartPos + (wordLength * this._gridLength) - this._gridLength;
                        }

                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            // Construction du pattern de recherche
            string searchPattern = string.Empty;

            for (int i = word.StartPos; i <= word.EndPos; i += increment)
            {
                if (string.IsNullOrWhiteSpace(this._gridCells[i]))
                {
                    searchPattern += Grid.CHAR_PATTERN;
                }
                else
                {
                    searchPattern += this._gridCells[i];
                }
            }

            // Si le pattern est un texte entier => pas besoin de placer un nouveau mot
            if (searchPattern.IndexOf(Grid.CHAR_PATTERN) >= 0)
            {
                if (word.IsReversed)
                {
                    searchPattern = new string(searchPattern.Reverse().ToArray());
                }

                word.WordText = this.GetRandomWord(searchPattern).Result;
                this.AddWord(word);
            }
        }

        /// <summary>
        /// Recherche d'un mot aléatoire à placer dans la grille.
        /// </summary>
        /// <param name="searchPattern">Pattern de recherche.</param>
        /// <returns></returns>
        private async Task<string> GetRandomWord(string searchPattern)
        {
            string word         = string.Empty;
            bool isValidWord    = false;
            int nbAttempts      = 0;

            List<DatamuseApiResult> words = await DatamuseApiHelper.GetRandomWord(searchPattern);
            if (words != null && words.Any())
            {
                while (!isValidWord && nbAttempts < words.Count())
                {
                    word = words.ElementAt(RandomHelper.GenerateRandomValue(0, words.Count() - 1)).word.ToUpper();

                    if (!word.ContainsForbiddenChar(this.ForbiddenChars) && !this._wordList.Any(x => x.WordTextBase == word))
                    {
                        isValidWord = true;
                    }

                    nbAttempts++;
                }
            }

            return word;
        }

        /// <summary>
        /// Ajout d'un mot dans la grille.
        /// </summary>
        /// <param name="word">Mot à ajouter.</param>
        private void AddWord(Word word)
        {
            if (word != null && !string.IsNullOrWhiteSpace(word.WordText))
            {
                if (word.IsReversed)
                {
                    word.WordText = new string(word.WordText.Reverse().ToArray());
                }

                this._wordList.Add(word);

                int j = 0;

                switch (word.Orientation)
                {
                    case eWordOrientation.HORIZONTAL:
                        {
                            for (int i = word.StartPos; j < word.WordText.Length; i++)
                            {
                                this._gridCells[i] = word.WordText.Substring(j, 1);
                                j++;
                            }
                            
                            break;
                        }
                    case eWordOrientation.VERTICAL:
                        {
                            for (int i = word.StartPos; j < word.WordText.Length; i += this._gridLength)
                            {
                                this._gridCells[i] = word.WordText.Substring(j, 1);
                                j++;
                            }
                            
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }
    }
}
