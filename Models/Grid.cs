using System.Text.RegularExpressions;

namespace WordSearch.Models
{
    [Serializable]
    public class Grid
    {
        public const int    MIN_WORD_LENGTH         = 4;
        public const int    MAX_WORD_LENGTH         = 12;
        public const int    DEFAULT_GRID_LENGTH     = 12;
        public const string CHAR_PATTERN            = ".";

        // Longueur de la grille
        private int             _gridLength;
        // Taille maximale des mots, adaptée en fonction de la longueur de la grille
        private int             _maxWordLength;
        // Cellules de la grille, chaque cellule contenant une lettre
        private string[]        _gridCells;
        // Liste des mots à trouver
        private List<Word>      _wordList;
        // Numéros des colonnes d'après les index des cellules
        private int[]           _columnsNumber;

        /// <summary>
        /// Nombre de cellules de la grille.
        /// </summary>
        private int NumberOfCells
        {
            get
            {
                return this._gridLength * this._gridLength;
            }
        }
        
        /// <summary>
        /// Indique si la grille est valide, c'est-à-dire si toutes les cellules contiennent une lettre.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !this._gridCells.Any(x => string.IsNullOrWhiteSpace(x));
            }
        }

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
            int wordStartPos = this.GenerateRandomValue(0, this.NumberOfCells - 1);

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

        private void PlaceWord(int wordStartPos)
        {
            int wordLength  = this._gridLength; // random.Next(Grid.MIN_WORD_LENGTH, this._maxWordLength);
            int increment   = 1;

            Word word  = new Word()
            {
                Orientation = (eWordOrientation)this.GenerateRandomValue((int)eWordOrientation.HORIZONTAL, (int)eWordOrientation.VERTICAL),
                StartPos    = wordStartPos,
                IsReversed  = this.GenerateRandomValue(0, 1) == 1
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

                word.WordText = this.GetRandomWord(searchPattern);
                this.AddWord(word);
            }
        }

        /// <summary>
        /// Recherche d'un mot aléatoire à placer dans la grille.
        /// </summary>
        /// <param name="searchPattern">Pattern de recherche.</param>
        /// <returns></returns>
        private string GetRandomWord(string searchPattern)
        {
            string regexPattern         = $"^{searchPattern}$";
            string word                 = string.Empty;
            bool isValidWord            = false;
            IEnumerable<string> words   = File.ReadAllLines(@"C:\Users\Xavier\Documents\Sources\NET\WordSearch\wordDB.txt")[0]
                                              .Split(";")
                                              .Where(x => Regex.IsMatch(x, regexPattern, RegexOptions.IgnoreCase));

            if (words != null && words.Any())
            {
                do
                {
                    word = words.ElementAt(this.GenerateRandomValue(0, words.Count() - 1));

                    if (!this._wordList.Any(x => x.WordTextBase == word))
                    {
                        isValidWord = true;
                    }
                } while (!isValidWord);
            }

            return word;
        }

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

        /// <summary>
        /// Génération d'une valeur entière aléatoire.
        /// </summary>
        /// <param name="minValue">Limite inférieure du nombre aléatoire retourné.</param>
        /// <param name="maxValue">Limite supérieure du nombre aléatoire retourné.</param>
        /// <returns></returns>
        private int GenerateRandomValue(int minValue, int maxValue)
        {
            /// Nécessaire de faire +1 sur la valeur maximale car elle est exclue par la méthode <seealso cref="Random.Next(int, int)"/>.
            return new Random().Next(minValue, maxValue + 1);
        }
    }
}
