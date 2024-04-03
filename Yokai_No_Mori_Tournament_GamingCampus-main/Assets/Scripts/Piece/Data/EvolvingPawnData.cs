using UnityEngine;


namespace YokaiNoMori.General
{

	[CreateAssetMenu(fileName = "Evolving Pawn Data", menuName = "Pawns/Evolving Pawn Data", order = 2)]
	public class EvolvingPawnData : CommonPawnData
	{
		[SerializeField]
		private EvolvePawnData m_devolvePawnData;
		public EvolvePawnData DevolvePawnData
		{
			get { return m_devolvePawnData; }
			set { m_devolvePawnData = value; }
		}
	}
}