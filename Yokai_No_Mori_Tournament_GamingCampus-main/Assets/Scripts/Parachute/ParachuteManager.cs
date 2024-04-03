using UnityEngine;
using YokaiNoMori.Interface;


namespace YokaiNoMori.General
{
    public class ParachuteManager
    {
        public void DoParachute(IPawn pawn, Vector2 newPosition)
        {
            (pawn as Pawn).SetCurrentBoardCase(GameManager.Instance.BoardManager.GetBoardCase(newPosition));
            (pawn.GetCurrentBoardCase() as BoardCase).SetCurrentPawnOnIt(pawn);

            GameManager.Instance.GraveyardManager.RemovePawnToGraveyard(pawn);
        }
    }
}
