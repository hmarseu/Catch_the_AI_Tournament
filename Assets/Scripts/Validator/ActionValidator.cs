using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;


namespace YokaiNoMori.General
{

    public static class ActionValidator
    {
        public static EValidationType CheckMovement(IPawn pawn, Vector2Int newPosition, int maxX, int maxY, int minX = 0, int minY = 0)
        {
            #region OUT_OF_RANGE & ILLEGAL_ACTION
            if (newPosition.x < minX || newPosition.x >= maxX || newPosition.y < minY || newPosition.y >= maxY)
            {
                return EValidationType.OUT_OF_RANGE;
            }

            IBoardCase currentCase = pawn.GetCurrentBoardCase();
            if (currentCase is null) 
                return EValidationType.ILLEGAL_ACTION;

            #endregion


            #region ILLEGAL_ACTION - Validation de la nouvelle position

            List<Vector2Int> validPosition = new List<Vector2Int>();
            Vector2Int currentPosition = currentCase.GetPosition();
            int orientation = GameManager.Instance.GetOrientationFromPawn(pawn);

            foreach (Vector2Int direction in pawn.GetDirections())
            {
                validPosition.Add(currentPosition + (direction * orientation));
            }


            if(!validPosition.Contains(newPosition))
            {
                return EValidationType.ILLEGAL_ACTION;
            }

            #endregion

            #region KOROPPOKURU_CHECKMATE
            // S'il bouge le Koropokurru
            if (pawn.GetPawnType() == EPawnType.Koropokkuru)
            {
                if (CheckKoropokurruIsCheckMate(pawn, newPosition))
                {
                    return EValidationType.KOROPPOKURU_CHECKMATE;
                }
            }
            // S'il bouge une autre pièce, on check l'état du koropokurru
            else
            {
                IPawn koropokurru = GameManager.Instance.GetPawnsOnBoard(pawn.GetCurrentOwner().GetCamp()).First(x => x.GetPawnType() == EPawnType.Koropokkuru);
                if (CheckKoropokurruIsCheckMate(koropokurru, koropokurru.GetCurrentPosition()))
                {
                    return EValidationType.KOROPPOKURU_CHECKMATE;
                }
            }

            #endregion

            // Si tout se passe : Légal !
            return EValidationType.LEGAL_ACTION;
        }

        public static bool CheckKoropokurruIsCheckMate(IPawn pawn, Vector2Int position)
        {
            List<IPawn> pawnsList = GameManager.Instance.BoardManager.GetPawnsNearbyAPosition(position);

            if (pawnsList is not null)
            {
                pawnsList.RemoveAll(x => x.GetCurrentPosition() == position || x.GetCurrentOwner().GetCamp() == pawn.GetCurrentOwner().GetCamp());
                foreach (IPawn pawnToCheck in pawnsList)
                {
                    if (pawnToCheck.GetCurrentOwner() != pawn.GetCurrentOwner())
                    {
                        foreach (Vector2Int direction in pawnToCheck.GetDirections())
                        {
                            if (position == pawnToCheck.GetCurrentPosition() + (direction * GameManager.Instance.GetOrientationFromPawn(pawnToCheck)))
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        public static EValidationType CheckParachutage(IPawn pawn, Vector2 newPosition, int maxX, int maxY, int minX = 0, int minY = 0)
        {
            if (newPosition.x < minX || newPosition.x > maxX || newPosition.y < minY || newPosition.y > maxY) return EValidationType.OUT_OF_RANGE;

            if (pawn.GetCurrentBoardCase() is null)
            {
                if (GameManager.Instance.BoardManager.GetBoardCase(newPosition).GetPawnOnIt() is null)
                {
                    return EValidationType.LEGAL_ACTION;
                }
            }
            return EValidationType.ILLEGAL_ACTION;
        }
    }
}
