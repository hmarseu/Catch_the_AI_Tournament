using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YokaiNoMori.Enumeration;

namespace catchTheAI
{
    [CreateAssetMenu(fileName = "Piece", menuName = "ScriptableObject/Piece")]
    public class SOPiece : ScriptableObject
    {
        public Sprite Image;
        public EPawnType ePawnType;
        public List<bool> PossibleMoves;
    }

}
