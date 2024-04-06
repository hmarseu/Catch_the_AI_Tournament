using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;

namespace YokaiNoMori.General
{

	/// <summary>
	/// Cette classe contient les cimetières des joueurs. Elle possède aussi les actions d'envoi et de parachutage des pièces
	/// </summary>
	public class GraveyardManager
	{
		public Graveyard PlayerOneGraveyard
		{
			get { return m_playerOneGraveyard; }
			private set { m_playerOneGraveyard = value; }
		}


		public Graveyard PlayerTwoGraveyard
		{
			get { return m_playerTwoGraveyard; }
			private set { m_playerTwoGraveyard = value; }
		}


		public void InitGraveyard()
		{
			PlayerOneGraveyard = new Graveyard();
			PlayerTwoGraveyard = new Graveyard();

			PlayerOneGraveyard.Init();
			PlayerTwoGraveyard.Init();
		}



		public void SendToGraveyard(IPawn pawn, ICompetitor player)
		{
            (pawn as Pawn).GetCapturedBy(player);

            if (player.GetCamp() == ECampType.PLAYER_ONE)
				PlayerOneGraveyard.AddToGraveyard(pawn);
			else if (player.GetCamp() == ECampType.PLAYER_TWO)
				PlayerTwoGraveyard.AddToGraveyard(pawn);
			else
				throw new System.Exception("ERROR : Pawn is send in NONE Camp");

        }

		public void RemovePawnToGraveyard(IPawn pawn)
		{
			if (pawn.GetCurrentOwner().GetCamp() == ECampType.PLAYER_ONE)
				PlayerOneGraveyard.RemoveToGraveyard(pawn);
			else if (pawn.GetCurrentOwner().GetCamp() == ECampType.PLAYER_TWO)
				PlayerTwoGraveyard.RemoveToGraveyard(pawn);
			else
				throw new System.Exception("ERROR : Pawn is send in NONE Camp");
		}



		private Graveyard m_playerOneGraveyard;
		private Graveyard m_playerTwoGraveyard;
	}

}