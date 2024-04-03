using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YokaiNoMori.General
{
    [CreateAssetMenu(fileName = "Board Data", menuName = "Board/Board Data", order = 0)]
    public class BoardData : ScriptableObject
    {
        public int X;
        public int Y;
        public List<SBoardCase> BoardCases;
    }
}
