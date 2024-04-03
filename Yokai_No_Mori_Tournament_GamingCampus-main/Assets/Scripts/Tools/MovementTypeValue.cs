using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YokaiNoMori.Enumeration;


namespace YokaiNoMori.General
{
    public static class MovementTypeValue
    {
        public static List<Vector2Int> GetDirections(CommonPawnData data)
        {
            List<Vector2Int> directions = new List<Vector2Int>();

            #region F/W MID
            // +0 X & +1 Y
            if (data.MovementType.HasFlag(EMovementType.FORWARD_MID))
                directions.Add(GetDirection(EMovementType.FORWARD_MID));

            // +0 X & -1 Y
            if (data.MovementType.HasFlag(EMovementType.BACK_MID))
                directions.Add(GetDirection(EMovementType.BACK_MID));


            #endregion


            #region RIGHT
            // +1 X & +1 Y
            if (data.MovementType.HasFlag(EMovementType.FORWARD_RIGHT))
                directions.Add(GetDirection(EMovementType.FORWARD_RIGHT));

            // +1 X & -1 Y
            if (data.MovementType.HasFlag(EMovementType.BACK_RIGHT))
                directions.Add(GetDirection(EMovementType.BACK_RIGHT));

            // +1 X & 0 Y
            if (data.MovementType.HasFlag(EMovementType.MID_RIGHT))
                directions.Add(GetDirection(EMovementType.MID_RIGHT));

            #endregion


            #region LEFT
            // -1 X & +1 Y
            if (data.MovementType.HasFlag(EMovementType.FORWARD_LEFT))
                directions.Add(GetDirection(EMovementType.FORWARD_LEFT));

            // -1 X & -1 Y
            if (data.MovementType.HasFlag(EMovementType.BACK_LEFT))
                directions.Add(GetDirection(EMovementType.BACK_LEFT));

            // -1 X & 0 Y
            if (data.MovementType.HasFlag(EMovementType.MID_LEFT))
                directions.Add(GetDirection(EMovementType.MID_LEFT));

            #endregion

            return directions;
        }

        public static Vector2Int GetDirection(EMovementType movementType)
        {
            #region F/W MID
            // +0 X & +1 Y
            if (movementType == EMovementType.FORWARD_MID)
                return new Vector2Int(0, 1);

            // +0 X & -1 Y
            else if (movementType == EMovementType.BACK_MID)
                return new Vector2Int(0, -1);

            #endregion


            #region RIGHT



            // +1 X & +1 Y
            else if (movementType == EMovementType.FORWARD_RIGHT)
                return new Vector2Int(1, 1);

            // +1 X & -1 Y
            else if (movementType == EMovementType.BACK_RIGHT)
                return new Vector2Int(1, -1);

            // +1 X & 0 Y
            else if (movementType == EMovementType.MID_RIGHT)
                return new Vector2Int(1, 0);

            #endregion


            #region LEFT
            // -1 X & +1 Y
            else if (movementType == EMovementType.FORWARD_LEFT)
                return new Vector2Int(-1, 1);

            // -1 X & -1 Y
            else if (movementType == EMovementType.BACK_LEFT)
                return new Vector2Int(-1, -1);

            // -1 X & 0 Y
            else if (movementType == EMovementType.MID_LEFT)
                return new Vector2Int(-1, 0);
            #endregion

            else
                throw new System.Exception("ERROR. MovementType Doesn't Exist");
        }
    }
}
