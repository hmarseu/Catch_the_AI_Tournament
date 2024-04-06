using System;
using UnityEngine;
using YokaiNoMori.Enumeration;


namespace YokaiNoMori.General
{
    [Serializable]
    public struct SBoardCase
    {
        public Vector2 Position;
        public EPawnType PawnToSpawn;
        public CommonPawnData PawnData;
        public ECampType Camp;
    }
}
