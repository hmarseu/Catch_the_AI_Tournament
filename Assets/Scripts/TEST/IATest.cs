using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;


namespace YokaiNoMori.General
{
    public class IATest : MonoBehaviour, ICompetitor
    {
        public string Name;
        
        IGameManager gameManager;
        ECampType m_currentCamp;


        List<IPawn> m_myGraveyards = new List<IPawn>();
        List<IPawn> m_myPawns = new List<IPawn>();

        Queue<IAMoveData> m_moveToDo;



        public void Init(IGameManager igameManager, float timerForAI, ECampType camp)
        {
            gameManager = igameManager;
            m_currentCamp = camp;

            m_myGraveyards = new List<IPawn>();
            m_myPawns = new List<IPawn>();


            string directoryName = m_currentCamp == ECampType.PLAYER_ONE ? "Player 1" : "Player 2";

            m_moveToDo = new Queue<IAMoveData>(Resources.LoadAll<IAMoveData>($"Scenarios/Scenario_{IAScenarioManager.Instance.ScenarioNumber}/{directoryName}"));

            //Debug.Log($"Number of Move : {m_moveToDo.Count}");
        }

        public string GetName()
        {
            return Name;
        }

        public ECampType GetCamp()
        {
            return m_currentCamp;
        }

        public void StartTurn()
        {
            if (m_moveToDo.Count > 0)
            {
                IAMoveData data = m_moveToDo.Dequeue();

                string move = data.IsNormalMove ? "Move" : "Parachutage";
                Debug.Log($"Joueur : {GetName()} - {data.PawnTargeted} {data.MovementType} {move} {data.PositionTargeted}");

                IPawn pawn;

                if (data.IsNormalMove)
                {
                    pawn = m_myPawns.First(x => x.GetPawnType() == data.PawnTargeted);

                    Vector2Int direction = MovementTypeValue.GetDirection(data.MovementType) * GameManager.Instance.GetOrientationFromPawn(pawn);

                    Debug.Log($"Joueur : {GetName()} - {pawn.GetPawnType()} {data.MovementType} - Current position : {pawn.GetCurrentPosition()} + direction : {direction} /// Destination : {pawn.GetCurrentPosition() + direction}");

                    Vector2Int newPosition = pawn.GetCurrentPosition() + direction;
                    gameManager.DoAction(pawn, newPosition, EActionType.MOVE);
                }
                else
                {
                    pawn = m_myGraveyards.FirstOrDefault(x => x.GetPawnType() == data.PawnTargeted);

                    if (pawn is not null)
                        Debug.Log($"Joueur : {GetName()} - {pawn.GetPawnType()} {data.MovementType} - Destination : {pawn.GetCurrentPosition()} /// Current pawn in the case : {GameManager.Instance.BoardManager.GetBoardCase(data.PositionTargeted)?.GetPawnOnIt()?.GetPawnType()}");

                    gameManager.DoAction(pawn, data.PositionTargeted, EActionType.PARACHUTE);
                }
            }
            else
                Debug.Log("Tous les coups sont finis");            
        }

        public void StopTurn()
        {
            Debug.Log($"{Name} : J'ai fini mon tour :)");
        }

        public void GetDatas()
        {
            m_myGraveyards.Clear();
            m_myPawns.Clear();

            m_myGraveyards.AddRange(gameManager.GetReservePawnsByPlayer(m_currentCamp));
            m_myPawns.AddRange(gameManager.GetPawnsOnBoard(m_currentCamp));
        }
    }
}
