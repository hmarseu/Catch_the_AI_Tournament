using YokaiNoMori.Interface;


namespace YokaiNoMori.General
{
    public class CompetitorsManager
    {
        #region Properties
        public ICompetitor PlayerOne
        {
            get { return m_playerOne; }
            private set { m_playerOne = value; }
        }

        public ICompetitor PlayerTwo
        {
            get { return m_playerTwo; }
            private set { m_playerTwo = value; }
        }


        public bool IsFirstMatch
        {
            get { return m_isFirstMatch; }
            private set { m_isFirstMatch = value; }
        }

        #endregion



        #region Private Methods

        private void SelectPlayers()
        {
            if (m_isFirstMatch)
            {
                PlayerOne = GameManager.Instance.TournamentManager.CompetitorsList[m_currentIndexFirstPlayer].CurrentCompetitor;
                PlayerTwo = GameManager.Instance.TournamentManager.CompetitorsList[m_currentIndexSecondPlayer].CurrentCompetitor;
            }
            else
            {
                PlayerOne = GameManager.Instance.TournamentManager.CompetitorsList[m_currentIndexSecondPlayer].CurrentCompetitor;
                PlayerTwo = GameManager.Instance.TournamentManager.CompetitorsList[m_currentIndexFirstPlayer].CurrentCompetitor;
            }
        }


        #endregion



        #region Public Methods
        public void InitPlayer()
        {
            SelectPlayers();
        }

        public bool NextPlayers()
        {
            bool isContinue = true;
            if (!IsFirstMatch)
            {
                if (m_currentIndexSecondPlayer + 1 >= GameManager.Instance.TournamentManager.CompetitorsList.Count)
                {
                    m_currentIndexFirstPlayer++;
                    m_currentIndexSecondPlayer = m_currentIndexFirstPlayer + 1;
                    isContinue = m_currentIndexSecondPlayer < GameManager.Instance.TournamentManager.CompetitorsList.Count;
                }
                else
                {
                    m_currentIndexSecondPlayer += 1;
                    isContinue = true;
                }
            }
            m_isFirstMatch = !IsFirstMatch;
            return isContinue;
        }

        #endregion



        #region Private members
        // COMPETITORS SELECTIONS
        int m_currentIndexFirstPlayer = 0;
        int m_currentIndexSecondPlayer = 1;
        private bool m_isFirstMatch = true;


        // PLAYERS
        private ICompetitor m_playerOne;
        private ICompetitor m_playerTwo;

        #endregion
    }

}