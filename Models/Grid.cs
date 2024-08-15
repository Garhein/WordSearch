namespace WordSearch.Models
{
    [Serializable]
    public class Grid
    {
        public const int MIN_WORD_LENGTH        = 4;
        public const int MAX_WORD_LENGTH        = 12;
        public const int DEFAULT_GRID_LENGTH    = 12;

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
            for (int i = 0; i < (2 * gridLength * gridLength); i++)
            {
                this._columnsNumber[i] = (i % gridLength) + 1;
            }
        }

        /// <summary>
        /// Génération de la grille.
        /// </summary>
        public void Generate()
        {
            // 1ère tentative de génération
            this.GenerateGrid();

            if (!this.IsValid)
            {
                // RàZ des éléments générés
                this._wordList.Clear();

                for (int i = 0; i < this._gridCells.Length; i++)
                {
                    this._gridCells[i] = null;
                }

                // 2ème tentative de génération
                this.GenerateGrid();
                if (!this.IsValid)
                {
                    throw new Exception("Génération de la grille non valide, veuillez relancer le traitement.");
                }
            }   
        }

        /// <summary>
        /// Génération de la grille.
        /// </summary>
        public void GenerateGrid()
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

            bool toto = true;
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
            bool withChars       = false;

            for (int i = word.StartPos; i <= word.EndPos; i += increment)
            {
                if (string.IsNullOrWhiteSpace(this._gridCells[i]))
                {
                    searchPattern += "_";
                }
                else
                {
                    searchPattern += this._gridCells[i];
                    withChars      = true;
                }
            }

            // Pas de chevauchement avec un/des mot(s) => ajout du mot
            if (!withChars)
            {
                word.WordText = this.DrawWord(string.Empty, wordLength);
                this.AddWord(word);
            }
            else
            {
                // Si le pattern est un texte entier => pas besoin de placer un nouveau mot
                if (searchPattern.IndexOf("_") >= 0)
                {
                    if (word.IsReversed)
                    {
                        searchPattern = new string(searchPattern.Reverse().ToArray());
                    }

                    word.WordText = this.DrawWord(searchPattern, wordLength);
                    this.AddWord(word);
                }
            }
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

        private string DrawWord(string searchPattern, int wordLength)
        {
            string[] tmp    = File.ReadAllLines(@"C:\Users\Xavier\Documents\Sources\NET\WordSearch\wordDB.txt");
            string[] dbWord = tmp[0].Split(';');
            string word     = string.Empty;

            if (string.IsNullOrWhiteSpace(searchPattern))
            {
                word = this.DrawWordFromLength(ref dbWord, wordLength);
            }
            else
            {
                word = this.DrawWordFromPattern(ref dbWord, searchPattern);
            }

            return word;
        }

        private string DrawWordFromLength(ref string[] dbWord, int wordLength)
        {
            return dbWord[this.GenerateRandomValue(0, dbWord.Length - 1)];
        }

        private string DrawWordFromPattern(ref string[] dbWord, string searchPattern)
        {
            string word         = string.Empty;
            char[] charsPattern = searchPattern.ToArray();
            char[] charsDbWord  = null;
            bool equalToPattern = true;
            int i = 0;
            int j = 0;

            do
            {
                equalToPattern = true;

                if (dbWord[i].Length == searchPattern.Length)
                {
                    charsDbWord = dbWord[i].ToArray();
                    j = 0;

                    do
                    {
                        equalToPattern = charsPattern[j] == '_' || charsPattern[j] == charsDbWord[j];
                        j++;
                    } while (equalToPattern && j < searchPattern.Length);

                    if (equalToPattern)
                    {
                        word = dbWord[i];

                        // Vérifier si le mot est déjà présent dans la liste
                        // Si déplacée plus haut, possibilité de boucle infinie car on retrouve le mot déjà placé
                        if (!this._wordList.Any(x => x.WordTextBase == word))
                        {
                            i = dbWord.Length;
                        }
                    }
                }

                i++;
            } while (i < dbWord.Length);

            return word;
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
