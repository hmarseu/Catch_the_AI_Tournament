using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using catchTheAI;
using UnityEngine.UIElements;
using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;
using YokaiNoMori.General;
using System;

namespace catchTheAI
{

    public class Player : MonoBehaviour, ICompetitor
    {
        public string groupName;
        public int idPlayer;
        public IGameManager boardManager;
        private ECampType myCamp;
        private string name;
        _tempMonteCarlo ia;
        public bool isAI;
        int[,] boardId;

        private void Start()
        {
            
            //boardManager = GameObject.FindAnyObjectByType<GameManager>();
        }

        public ECampType GetCamp()
        {
            return myCamp;
        }

        public string GetName()
        {
            return name;
        }
        public void SetName(string name)
        {
            this.name = name;
        }

        public void SetCamp(ECampType camp)
        {
            this.myCamp = camp;
            if (camp == ECampType.PLAYER_ONE)
            {
                idPlayer = 1;
            }
            else
            {
                idPlayer = -1;
            }
            ia = GetComponent<_tempMonteCarlo>();
        }

        public void StartTurn()
        {
           
            // if the player is an IA
           
            // get its cemetery
            List<IPawn> PlayersReserve = boardManager.GetReservePawnsByPlayer(myCamp);

            // get positions of its pieces - already inverted
            List<Vector3Int> yohanPositions = GetInformationArray();

            // checked : its good
            // int[,] yohanArray = ConvertPiecePositionsToYohanArray(positions, 3, 4);


            boardId = UpdateArrayInt(yohanPositions);
            Vector4 bestMove = ia.MonteCarloSearch(new Node(idPlayer, new Vector4(), null, boardId, PlayersReserve), 1000,boardManager);
            //Debug.Log($"pieceid + position : {bestMove} ");

            // get a selectedPiece to play
            Debug.Log($"piece id : {bestMove.z}");
            IPawn pawnTarget = GetPieceById((int)bestMove.z,idPlayer);

            // get a postion where to move
            Vector2Int newPosition = new Vector2Int((int)bestMove.x, (int)bestMove.y);

            // get selected action type
            EActionType actionType;
            if (bestMove.w == 1)
            {
                actionType = EActionType.PARACHUTE;
            }
            else
            {
                actionType = EActionType.MOVE;
            }
            //Debug.Log("Position : (" + newPosition.x + ", " + newPosition.y + "), index : " + bestMove.z);

            // do action

           //test changement dans git
     
            Debug.Log($"actiontype : {actionType}");
   
            Debug.Log($"position : {newPosition} ");
            Debug.Log($"pawn : {pawnTarget?.GetPawnType()}");

            boardManager.DoAction(pawnTarget, newPosition, actionType);

          
        }
        public int[,] ConvertPiecePositionsToYohanArray(List<Vector3Int> piecePositions, int height, int width)
        {
            int[,] yohanArray = new int[height, width];

            // Remplissage du tableau avec les valeurs des pi�ces
            foreach (var piecePos in piecePositions)
            {
                // Convertir la position de la pi�ce en coordonn�es Yohan
                Vector2Int yohanPos = ConvertToYohanArray(new Vector2Int(piecePos.x, piecePos.y));

                // Assigner la valeur de la pi�ce � la position correspondante dans le tableau Yohan
                yohanArray[yohanPos.y, yohanPos.x] = piecePos.z;
            }

            return yohanArray;
        }
        public void StopTurn()
        {
          
        }

        public void GetDatas()
        {
            
        }

        public void Init(IGameManager igameManager, float timerForAI, ECampType currentCamp)
        {
            boardManager = igameManager;
            SetCamp(currentCamp);
        }
        
        public ECampType GetPieceCamp(int idPiece)
        {
            //Debug.Log(idPiece);
            if (idPiece < 0)
            {
                return ECampType.PLAYER_TWO;
            }
            else if (idPiece > 0)
            {
                return ECampType.PLAYER_ONE;
            }
            else
            {
                return ECampType.NONE;
            }
        }
        public IPawn GetPieceById(int id,int idPlayer)
        {
            ECampType pieceCamp = GetPieceCamp(id);
            List<IPawn> pawnsInBoard = boardManager.GetPawnsOnBoard(pieceCamp);

            // is in board?
            foreach (Pawn pawn in pawnsInBoard)
            {
                int idPawn = TransformIpawnIntoId(pawn, idPlayer);
                if (idPawn == id)
                {
                    return pawn;
                }
            }

            // is in cemetery?
            List<IPawn> pawnsInCemetery = boardManager.GetReservePawnsByPlayer(pieceCamp);

            foreach (Pawn pawn in pawnsInBoard)
            {
                int idPawn = TransformIpawnIntoId(pawn, idPlayer);
                if (idPawn == id)
                {
                    return pawn;
                }
            }
            return null;
        }


        private List<Vector3Int> GetInformationArray()
        {
            List<IPawn> pawns = boardManager.GetAllPawn();
            List <Vector3Int> arrayInformations = new List<Vector3Int>();

            foreach (IPawn pawn in pawns)
            {
                Vector2Int pos = pawn.GetCurrentPosition();
                if (pos == new Vector2Int(-1, -1)) break; // break if in cemetery

                ICompetitor competitor = pawn.GetCurrentOwner();
                ECampType camp = competitor.GetCamp();
                int idPlayer = TransformECampIntoId(camp);

                int idPiece = TransformIpawnIntoId(pawn,idPlayer);

                // Debug.LogWarning("opela : " + new Vector3Int(pos.x, pos.y, idPiece));
                arrayInformations.Add(new Vector3Int(pos.x, pos.y, idPiece));
            }
            return arrayInformations;
        }
        private int TransformIpawnIntoId(IPawn pawn,int idPlayer)
        {
            EPawnType type = pawn.GetPawnType();
            switch (type)
            {
                case EPawnType.Kodama:
                    return 1* idPlayer;
                case EPawnType.KodamaSamurai:
                    return 2 * idPlayer;
                case EPawnType.Tanuki:
                    return 3 * idPlayer;
                case EPawnType.Kitsune:
                    return 4 * idPlayer;
                case EPawnType.Koropokkuru:
                    return 5 * idPlayer;
            }
            return 0;
        }
        private int TransformECampIntoId(ECampType type)
        {
            switch (type)
            {
                case ECampType.PLAYER_ONE:
                    return 1;
                case ECampType.PLAYER_TWO:
                    return -1;
            }
            return 0;
        }



        // if got position + id of the pieces -> return the board array int
        public int[,] UpdateArrayInt(List<Vector3Int> arrayInformations)
        {
            int heightArray = 3;
            int widthArray = 4;

            int[,] newArray = CreateIntArray(arrayInformations, heightArray, widthArray);
            return newArray;
        }

        private int[,] CreateIntArray(List<Vector3Int> arrayInformations, int heightArray, int widthArray)
        {
            Debug.LogWarning("Updating the array : ");
            int[,] newArray = new int[heightArray, widthArray];

            // init empty array
            for (int i = 0; i > heightArray; i++)
            {
                for (int j = 0; j < widthArray; j++)
                {
                    newArray[i, j] = 0;
                }
            }

            // fill array
            foreach(Vector3Int pos in arrayInformations)
            {
                int x = pos.x;
                int y = pos.y;
                int value = pos.z;

                // still in the array?
                if (x >= 0 && y < widthArray && y >= 0 && x < heightArray)
                {
                    newArray[x, y] = value;
                    Debug.LogWarning("Position in the array : (" + x + ", " + y + ", " + value + ")");
                }
                else Debug.LogError("Position out of the array : (" + x + ", " + y + ", " + value + ")");
            }
            LogPieceIds(newArray);
            return newArray;
        }

        // Debug -> display the array of position
        public void LogPieceIds(int[,] array)
        {
            if (array == null) return;

            int numRows = array.GetLength(0);
            int numCols = array.GetLength(1);

            for(int i = numRows - 1; i >= 0; i--)
            {
                string rowContent = "";
                for (int j = 0; j < numCols; j++)
                {
                    rowContent += array[i, j].ToString();

                    if(j < numCols - 1)
                    {
                        rowContent += ", ";
                    }
                }
                //Debug.Log(rowContent);
            }
        }

        // utils
        public Vector2Int ConvertToYohanArray(Vector2Int position)
        {
            int x = position.y;
            int y = position.x;
            return new Vector2Int(x, y);
        }
    }

}