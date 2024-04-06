using UnityEngine;
using YokaiNoMori.Enumeration;


[CreateAssetMenu(fileName = "MoveData_", menuName = "Scenario/Move Data", order = 1)]
public class IAMoveData : ScriptableObject
{
    public EPawnType PawnTargeted;

    public bool IsNormalMove;
    public EMovementType MovementType;
    public Vector2Int PositionTargeted;
}
