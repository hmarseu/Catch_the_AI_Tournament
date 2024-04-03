using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;
using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;
using YokaiNoMori.Struct;


namespace YokaiNoMori.General
{

    public class GameManager : MonoBehaviour, IGameManager
    {
        #region Singleton
        private static GameManager m_instance = null;
        public static GameManager Instance => m_instance;
        #endregion

        #region Properties

        #region Managers
        public CompetitorsManager CompetitorsManager
        {
            get { return m_competitorsManager; }
            private set { m_competitorsManager = value; }
        }


        public BoardManager BoardManager
        {
            get { return m_boardManager; }
            private set { m_boardManager = value; }
        }



        public GraveyardManager GraveyardManager
        {
            get { return m_graveyardManage; }
            private set { m_graveyardManage = value; }
        }



        public TournamentManager TournamentManager
        {
            get { return m_tournamentManager; }
            private set { m_tournamentManager = value; }
        }

        #endregion


        public ICompetitor CurrentPlayer
        {
            get { return m_currentPlayerTurn; }
            private set { m_currentPlayerTurn = value; }
        }

        public List<IPawn> PawnsList
        {
            get { return m_pawnsList; }
            private set { m_pawnsList = value; }
        }

        #endregion


        #region Unity Methods

        private void Awake()
        {
            #region Singleton
            if (m_instance != null && m_instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                m_instance = this;
            }
            DontDestroyOnLoad(gameObject);
            #endregion
        }

        private void Start()
        {
            BoardManager = new BoardManager(m_boardData);
            m_movementManager = new MovementManager();
            m_parachuteManager = new ParachuteManager();
            CompetitorsManager = new CompetitorsManager();
            m_fightManager = new FightManager();
            GraveyardManager = new GraveyardManager();
        }


        private void Update()
        {
            switch (m_currentGameState)
            {
                case EGameState.INIT:
                    InitUpdate();
                    break;
                case EGameState.CHOICE_PLAYERS:
                    ChoicePlayerUpdate();
                    break;
                case EGameState.NEW_GAME:
                    NewGameUpdate();
                    break;
                case EGameState.TURN_PLAYER:
                    TurnPlayerUpdate();
                    break;
                case EGameState.VALIDATION_ACTION:
                    ValidationActionUpdate();
                    break;
                case EGameState.CHECK_WIN_CONDITION:
                    CheckWinConditionUpdate();
                    break;
                case EGameState.UNEXPECTED_ERROR:
                    UnexpectedErrorUpdate();
                    break;
                case EGameState.UNEXPECTED_ACTION:
                    UnexpectedActionUpdate();
                    break;
                case EGameState.NEXT_PLAYER:
                    NextPlayerUpdate();
                    break;
                case EGameState.FINISH_GAME:
                    FinishGameUpdate();
                    break;
                case EGameState.NEXT_GAME:
                    NextGameUpdate();
                    break;
                case EGameState.END_GAME:
                    EndTournamentUpdate();
                    break;
                default: throw new Exception("ERROR : This state cannot exist");
            }
        }




        #endregion

        #region STATES

        private void InitUpdate()
        {
            if (m_isFirstTimeInState)
            {
                m_isFirstTimeInState = false;

                //Debug.Log("====== INIT ======");

                PawnsList = new List<IPawn>();
                BoardManager.InitBoard();
                m_isKorropokuruDefeated = false;

                m_pawnToDoAction = null;
                m_askedPawnPosition = new Vector2Int(-99, -99);
                m_actionTypeRequested = EActionType.NONE;
                m_validationType = EValidationType.NONE;


                SetCurrentState(EGameState.CHOICE_PLAYERS);

            }
        }

        private void ChoicePlayerUpdate()
        {
            if (m_isFirstTimeInState)
            {
                m_isFirstTimeInState = false;

                //Debug.Log("====== CHOICE PLAYER ======");

                CompetitorsManager.InitPlayer();

                SetCurrentState(EGameState.NEW_GAME);

            }
        }

        private void NewGameUpdate()
        {
            if (m_isFirstTimeInState)
            {
                m_isFirstTimeInState = false;

                //Debug.Log("====== NEW GAME ======");
                CurrentPlayer = CompetitorsManager.PlayerOne;
                CompetitorsManager.PlayerOne.Init(this, 5f, ECampType.PLAYER_ONE);
                CompetitorsManager.PlayerTwo.Init(this, 5f, ECampType.PLAYER_TWO);


                PawnsList.ForEach(x => (x as Pawn).InitOwner());

                GraveyardManager.InitGraveyard();
            }

            if (m_currentTimer > 5f)
                SetCurrentState(EGameState.TURN_PLAYER);
            else
                m_currentTimer += Time.deltaTime;
        }

        private void TurnPlayerUpdate()
        {
            if (m_isFirstTimeInState)
            {
                m_isFirstTimeInState = false;


                //Debug.Log("====== TURN PLAYER ======");
                try
                {
                    Debug.Log("Current player : " + CurrentPlayer);
                    CurrentPlayer.GetDatas();
                    CurrentPlayer.StartTurn();
                }
                catch(Exception ex)
                {
                    Debug.LogError($"{ex.Message} {ex.StackTrace}");
                    SetCurrentState(EGameState.UNEXPECTED_ERROR);
                }
            }
        }

        private void ValidationActionUpdate()
        {
            if (m_isFirstTimeInState)
            {
                m_isFirstTimeInState = false;

                //Debug.Log("====== VALIDATION ACTION ======");

                m_validationType = m_actionTypeRequested == EActionType.PARACHUTE
                    ? ActionValidator.CheckParachutage(m_pawnToDoAction, m_askedPawnPosition, BoardManager.BOARD_X, BoardManager.BOARD_Y)
                    : ActionValidator.CheckMovement(m_pawnToDoAction, m_askedPawnPosition, BoardManager.BOARD_X, BoardManager.BOARD_Y);

                Debug.Log($"Current Validation Type : {m_validationType}");



                if (m_validationType == EValidationType.LEGAL_ACTION || m_validationType == EValidationType.KOROPPOKURU_CHECKMATE)
                {
                    m_lastAction.SetAction(CurrentPlayer.GetCamp(), m_pawnToDoAction.GetPawnType(), m_actionTypeRequested, m_pawnToDoAction.GetCurrentPosition(), m_askedPawnPosition, BoardManager.GetBoardCase(m_askedPawnPosition).GetPawnOnIt());
                    m_lastAction.ShowActionDebug();

                    if (m_actionTypeRequested == EActionType.MOVE)
                    {
                        DoPawnMove();
                    }
                    else if (m_actionTypeRequested == EActionType.PARACHUTE)
                    {
                        DoPawnParachute();
                    }

                    if (m_validationType == EValidationType.LEGAL_ACTION) SetCurrentState(EGameState.CHECK_WIN_CONDITION);
                    else SetCurrentState(EGameState.UNEXPECTED_ACTION);
                }
                else SetCurrentState(EGameState.UNEXPECTED_ACTION);


            }
        }

        private void CheckWinConditionUpdate()
        {
            if (m_isFirstTimeInState)
            {
                m_isFirstTimeInState = false;


                //Debug.Log("====== CHECK WIN CONDITION ======");

                if (m_isKorropokuruDefeated)
                {
                    m_isKorropokuruDefeated = false;
                    SetCurrentState(EGameState.FINISH_GAME);
                }
                else if (IsKoropokurruIsSafeAndWinCase())
                    SetCurrentState(EGameState.FINISH_GAME);
                else
                    SetCurrentState(EGameState.NEXT_PLAYER);


            }
        }

        private void UnexpectedActionUpdate()
        {
            if (m_isFirstTimeInState)
            {
                m_isFirstTimeInState = false;


                //Debug.Log("====== UNEXPECTED ACTION ======");
                if (m_validationType == EValidationType.KOROPPOKURU_CHECKMATE)
                {
                    if(!m_isKorropokuruDefeated)
                        TournamentManager.AddError(m_currentPlayerTurn, m_validationType);

                    SetCurrentState(EGameState.CHECK_WIN_CONDITION);
                }
                else
                {
                    SetCurrentState(EGameState.FINISH_GAME);
                }

            }
        }

        private void UnexpectedErrorUpdate()
        {
            if(m_isFirstTimeInState)
            {
                m_isFirstTimeInState = false;

                m_validationType = EValidationType.EXCEPTION_CATCHED;
                SetCurrentState(EGameState.FINISH_GAME);
            }
        }

        private void NextPlayerUpdate()
        {
            if (m_isFirstTimeInState)
            {
                m_isFirstTimeInState = false;


                //Debug.Log("====== NEXT PLAYER ======");

                CurrentPlayer = CurrentPlayer is null || CurrentPlayer.GetCamp() == ECampType.PLAYER_TWO ? CompetitorsManager.PlayerOne : CompetitorsManager.PlayerTwo;
                SetCurrentState(EGameState.TURN_PLAYER);

            }

        }

        private void FinishGameUpdate()
        {
            if (m_isFirstTimeInState)
            {
                m_isFirstTimeInState = false;
                //Debug.Log("====== FINISH GAME ======");

                if (m_validationType == EValidationType.LEGAL_ACTION || m_validationType == EValidationType.KOROPPOKURU_CHECKMATE)
                    TournamentManager.AddVictory(CurrentPlayer.GetCamp());
                else
                    TournamentManager.AddIllegalError(CurrentPlayer, m_validationType);



                SetCurrentState(EGameState.NEXT_GAME);

            }
        }

        private void NextGameUpdate()
        {
            if (m_isFirstTimeInState)
            {
                m_isFirstTimeInState = false;


                //Debug.Log("====== NEXT GAME ======");

                foreach (var item in TournamentManager.CompetitorsList)
                {
                    //Debug.LogWarning($"Name : {item.CurrentCompetitor.GetName()} /// WIN : {item.CurrentVictory} /// LOSE : {item.CurrentLose} /// DRAW : {item.CurrentDraw} /// ERROR : {item.CurrentErrorOccured} /// SCORE : {item.GetScore()}");
                }

                if (CompetitorsManager.NextPlayers())
                    SetCurrentState(EGameState.INIT);
                else
                    SetCurrentState(EGameState.END_GAME);


            }
        }

        private void EndTournamentUpdate()
        {
            if (m_isFirstTimeInState)
            {
                m_isFirstTimeInState = false;

                //Debug.Log("====== END TOURNAMENT ======");
                TournamentManager.ShowResult();
            }
        }


        private void SetCurrentState(EGameState nextState)
        {
            //Debug.Log($"====== SET STATE ======");
            //Debug.Log($"OLD STATE : {m_currentGameState} -----> NEW STATE : {nextState}");


            m_currentGameState = nextState;
            m_isFirstTimeInState = true;
            m_currentTimer = 0f;
        }




        #endregion

        #region Private Methods

        private void DoPawnMove()
        {
            if (BoardManager.GetBoardCase(m_askedPawnPosition).GetPawnOnIt() != null)
            {
                m_fightManager.DoFight(m_pawnToDoAction, BoardManager.GetBoardCase(m_askedPawnPosition).GetPawnOnIt());
            }
            m_movementManager.DoMovement(m_pawnToDoAction, m_askedPawnPosition);
        }

        private void DoPawnParachute()
        {
            m_parachuteManager.DoParachute(m_pawnToDoAction, m_askedPawnPosition);
        }

        private bool IsKoropokurruIsSafeAndWinCase()
        {
            if(m_pawnToDoAction.GetPawnType() == EPawnType.Koropokkuru)
            {
                if(m_pawnToDoAction.GetCurrentBoardCase() is SpecialBoardCase)
                {
                    if(CurrentPlayer.GetCamp() == ECampType.PLAYER_ONE && !ActionValidator.CheckKoropokurruIsCheckMate(m_pawnToDoAction, m_pawnToDoAction.GetCurrentPosition()) && m_pawnToDoAction.GetCurrentPosition().y == 3)
                    {
                        return true;
                    }
                    else if(CurrentPlayer.GetCamp() == ECampType.PLAYER_TWO && !ActionValidator.CheckKoropokurruIsCheckMate(m_pawnToDoAction, m_pawnToDoAction.GetCurrentPosition()) && m_pawnToDoAction.GetCurrentPosition().y == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Public Methods
        public void AddPawnToList(IPawn pawn)
        {
            PawnsList.Add(pawn);
        }

        public int GetOrientationFromPawn(IPawn pawn)
        {
            return pawn.GetCurrentOwner().GetCamp() == ECampType.PLAYER_ONE ? 1 : -1;
        }

        public List<IPawn> GetAllPawn()
        {
            return PawnsList;
        }

        public List<IBoardCase> GetAllBoardCase()
        {
            return BoardManager.BoardCases;
        }

        public void DoAction(IPawn pawnTarget, Vector2Int position, EActionType actionType)
        {
            m_pawnToDoAction = pawnTarget;
            m_askedPawnPosition = position;
            m_actionTypeRequested = actionType;

            if (m_pawnToDoAction is null)
            {
                m_validationType = EValidationType.NULL_PAWN;
                SetCurrentState(EGameState.UNEXPECTED_ACTION);
            }
            else if (m_pawnToDoAction.GetCurrentOwner() != m_currentPlayerTurn)
            {
                m_validationType = EValidationType.CALL_ACTION_ON_ENEMY_PAWN;
                SetCurrentState(EGameState.UNEXPECTED_ACTION);
            }
            else if (actionType == EActionType.NONE)
            {
                m_validationType = EValidationType.BAD_ACTION;
                SetCurrentState(EGameState.UNEXPECTED_ACTION);
            }
            else
            {
                m_currentPlayerTurn.StopTurn();
                SetCurrentState(EGameState.VALIDATION_ACTION);
            }
        }

        public List<IPawn> GetReservePawnsByPlayer(ECampType campType)
        {
            if (campType == ECampType.PLAYER_ONE)
                return GraveyardManager.PlayerOneGraveyard.Pawns;
            else
                return GraveyardManager.PlayerTwoGraveyard.Pawns;
        }

        public List<IPawn> GetPawnsOnBoard(ECampType campType)
        {
            return PawnsList.Where(x => x.GetCurrentOwner().GetCamp() == campType && x.GetCurrentBoardCase() is not null).ToList();
        }

        public void KorropokuruIsDefeated()
        {
            m_isKorropokuruDefeated = true;
        }

        SAction IGameManager.GetLastAction()
        {
            return m_lastAction;
        }

        #endregion

        #region Private members
        private BoardManager m_boardManager;
        private CompetitorsManager m_competitorsManager;
        private MovementManager m_movementManager;
        private ParachuteManager m_parachuteManager;
        private GraveyardManager m_graveyardManage;
        private FightManager m_fightManager;

        [SerializeField]
        private TournamentManager m_tournamentManager;

        [SerializeField]
        private BoardData m_boardData;


        private EGameState m_currentGameState;


        private float m_currentTimer;
        private bool m_isFirstTimeInState = true;

        private ICompetitor m_currentPlayerTurn;

        private List<IPawn> m_pawnsList;


        private IPawn m_pawnToDoAction = null;
        private Vector2Int m_askedPawnPosition = new Vector2Int(-99, -99);
        private EActionType m_actionTypeRequested = EActionType.NONE;
        EValidationType m_validationType = EValidationType.NONE;
        private bool m_isKorropokuruDefeated = false;

        private SAction m_lastAction;
        #endregion

    }
}
