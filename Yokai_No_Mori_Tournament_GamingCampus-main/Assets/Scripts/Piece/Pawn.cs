using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;


namespace YokaiNoMori.General
{
    public class Pawn : IPawn
    {
        public IBoardCase CurrentBoardCase
        {
            get { return m_currentBoardCase; }
            private set { m_currentBoardCase = value; }
        }


        public ECampType CurrentPlayerCamp
        {
            get { return m_currentPlayerCamp; }
            private set { m_currentPlayerCamp = value; }
        }


        public ICompetitor CurrentPlayerOwner
        {
            get { return m_currentPlayerOwner; }
            private set { m_currentPlayerOwner = value; }
        }


        public CommonPawnData PawnData
        {
            get { return m_pawnData; }
            private set { m_pawnData = value; }
        }


        public Pawn(IBoardCase newCase, ECampType currentCamp, CommonPawnData data, IGameManager gm)
        {
            CurrentBoardCase = newCase;
            PawnData = data;
            ChangeCamp(currentCamp);
        }

        public void InitPawn()
        {
            CurrentPlayerOwner = CurrentPlayerCamp == ECampType.PLAYER_ONE ? GameManager.Instance.CompetitorsManager.PlayerOne : GameManager.Instance.CompetitorsManager.PlayerTwo;
        }

        private void Evolve()
        {
            if (PawnData is EvolvePawnData)
            {
                //Debug.Log($"{PawnData.PawnType} evolve to {(PawnData as EvolvePawnData).EvolutionPawn.PawnType}");
                PawnData = (PawnData as EvolvePawnData).EvolutionPawn;
            }
        }

        private void Devolve()
        {
            if (PawnData is EvolvingPawnData)
            {
                //Debug.Log($"{PawnData.PawnType} divolve to {(PawnData as EvolvingPawnData).DevolvePawnData.PawnType}");
                PawnData = (PawnData as EvolvingPawnData).DevolvePawnData;
            }
        }

        public void SetCurrentBoardCase(IBoardCase newBoardCase)
        {
            CurrentBoardCase = newBoardCase;
            if ((CurrentBoardCase as BoardCase).IsSpecialBoardCase() && PawnData is EvolvePawnData)
                Evolve();
        }

        public void ChangeCamp(ECampType newCamp)
        {
            CurrentPlayerCamp = newCamp;
        }

        public void GetCapturedBy(ICompetitor competitor)
        {
            CurrentPlayerOwner = competitor;
            CurrentBoardCase = null;
            Devolve();
            ChangeCamp(competitor.GetCamp());
        }

        public List<Vector2Int> GetDirections()
        {
            return MovementTypeValue.GetDirections(PawnData);
        }

        public ICompetitor GetCurrentOwner()
        {
            return m_currentPlayerOwner;
        }

        public IBoardCase GetCurrentBoardCase()
        {
            return m_currentBoardCase;
        }

        public Vector2Int GetCurrentPosition()
        {
            if (CurrentBoardCase is null)
                return new Vector2Int(-1, -1);
            else
                return CurrentBoardCase.GetPosition();
        }

        public EPawnType GetPawnType()
        {
            return m_pawnData.PawnType;
        }

        public void InitOwner()
        {
            m_currentPlayerOwner = m_currentPlayerCamp == ECampType.PLAYER_ONE ? GameManager.Instance.CompetitorsManager.PlayerOne : GameManager.Instance.CompetitorsManager.PlayerTwo;
            //Debug.Log($"Pawn Type : {GetPawnType()} /// Current Owner : {GetCurrentOwner()} /// Current Position : {GetCurrentPosition()}");
        }

        private ECampType m_currentPlayerCamp;
        private IBoardCase m_currentBoardCase;
        private ICompetitor m_currentPlayerOwner;

        private CommonPawnData m_pawnData;
    }
}
