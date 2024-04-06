using UnityEngine;


namespace YokaiNoMori.General
{
	[CreateAssetMenu(fileName = "Evolve Pawn Data", menuName = "Pawns/Evolve Pawn Data", order = 1)]
	public class EvolvePawnData : CommonPawnData
	{
		[SerializeField]
		private EvolvingPawnData m_evolutionPawn;

		public EvolvingPawnData EvolutionPawn
		{
			get { return m_evolutionPawn; }
			private set { m_evolutionPawn = value; }
		}
	}
}