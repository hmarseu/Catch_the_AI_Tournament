using UnityEngine;
using YokaiNoMori.Interface;


namespace YokaiNoMori.General
{
	public class BoardCase : IBoardCase
	{
		public Vector2Int Position
		{
			get { return m_position; }
			private set { m_position = value; }
		}

		public IPawn CurrentPawnOnIt
		{
			get { return m_currentPawnOnIt; }
			private set { m_currentPawnOnIt = value; }
		}


		public BoardCase(int x, int y)
		{
			Position = new Vector2Int(x, y);
		}

		public bool IsSpecialBoardCase()
		{
			return this is SpecialBoardCase;
		}

		public void SetCurrentPawnOnIt(IPawn pawn)
		{
			CurrentPawnOnIt = pawn;
		}

		public Vector2Int GetPosition()
		{
			return Position;
		}

		public IPawn GetPawnOnIt()
		{
			return CurrentPawnOnIt;
		}

		public bool IsBusy()
		{
			return CurrentPawnOnIt is not null;
		}

		private Vector2Int m_position;
		private IPawn m_currentPawnOnIt;
	}
}
