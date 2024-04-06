using UnityEngine;
using YokaiNoMori.Interface;


namespace YokaiNoMori.General
{
    public class MovementManager
    {
        public void DoMovement(IPawn pawn, Vector2 newPosition)
        {
            //Debug.Log($"Joueur : {GameManager.Instance.CurrentPlayer.GetName()} \nAction : Movement \nPawn : {pawn.GetPawnType()} \nCurrentPosition : {pawn.GetCurrentPosition()} \nnew Position : {newPosition}");


            (pawn.GetCurrentBoardCase() as BoardCase).SetCurrentPawnOnIt(null);
            (pawn as Pawn).SetCurrentBoardCase(GameManager.Instance.BoardManager.GetBoardCase(newPosition));
            (pawn.GetCurrentBoardCase() as BoardCase).SetCurrentPawnOnIt(pawn);
        }
    }
}