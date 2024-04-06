using UnityEngine;
using YokaiNoMori.Enumeration;



namespace YokaiNoMori.General
{
	[CreateAssetMenu(fileName = "Common Pawn Data", menuName = "Pawns/Common Pawn Data", order = 0)]
	public class CommonPawnData : ScriptableObject
	{
		[SerializeField]
		private EMovementType m_movementType;
		public EMovementType MovementType
		{
			get { return m_movementType; }
			private set { m_movementType = value; }
		}


		[SerializeField]
		private EPawnType m_pawnType;
		public EPawnType PawnType
		{
			get { return m_pawnType; }
			private set { m_pawnType = value; }
		}
	}

}
