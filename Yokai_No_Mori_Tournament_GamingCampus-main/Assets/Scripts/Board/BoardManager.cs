using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;

namespace YokaiNoMori.General
{
	public class BoardManager
	{
		public readonly int BOARD_Y;
		public readonly int BOARD_X;
		private BoardData m_boardData;

		public BoardManager(BoardData boardData)
		{
            BOARD_X = boardData.X;
            BOARD_Y = boardData.Y;
            m_boardData = boardData;
        }


		public List<IBoardCase> BoardCases
		{
			get { return m_boardCases; }
			private set { m_boardCases = value; }
		}

		public void InitBoard()
		{
			BoardCases = new List<IBoardCase>();

			for (int x = 0; x < BOARD_X; x++)
			{
				for (int y = 0; y < BOARD_Y; y++)
				{
					if (y == 0 || y == 3)
					{
						BoardCases.Add(new SpecialBoardCase(x, y));
					}
					else
					{
						BoardCases.Add(new BoardCase(x, y));
					}
				}
			}

			PlacePawnOnBoard();
		}

		private void PlacePawnOnBoard()
		{
			foreach (SBoardCase caseData in m_boardData.BoardCases)
			{
				foreach (BoardCase bCase in BoardCases)
				{
					if (caseData.Position == bCase.Position)
					{
						Pawn pawn = new Pawn(bCase, caseData.Camp, caseData.PawnData, GameManager.Instance);
						bCase.SetCurrentPawnOnIt(pawn);
						GameManager.Instance.AddPawnToList(pawn);
					}
				}
			}
		}

		public IBoardCase GetBoardCase(Vector2 position)
		{
			IBoardCase boardCase = BoardCases.FirstOrDefault(x => x.GetPosition() == position);
			return boardCase;
        }

		public List<IPawn> GetPawnsNearbyAPosition(Vector2Int position)
		{
			List<IPawn> pawnsList = new List<IPawn>();
            Vector2Int currentPositionToCheck;

            foreach (EMovementType currentState in Enum.GetValues(typeof(EMovementType)))
			{
                currentPositionToCheck = position;
				currentPositionToCheck += MovementTypeValue.GetDirection(currentState);

				if (currentPositionToCheck.x.IsBetween(0, BOARD_X) && currentPositionToCheck.y.IsBetween(0, BOARD_Y))
				{
                    IPawn currentPawnChecked = GetBoardCase(currentPositionToCheck).GetPawnOnIt();

                    if (currentPawnChecked != null)
                        pawnsList.Add(currentPawnChecked);
                }
            }

            return pawnsList;
		}




		private List<IBoardCase> m_boardCases;
	}
}
