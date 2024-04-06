using System;
using catchTheAI;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using YokaiNoMori.Enumeration;
using YokaiNoMori.Interface;

namespace catchTheAI
{
    public class Node
    {
        public int playerid;
        public int[,] piecesPosition;
        public List<IPawn> reserve;
        public Vector4 move;
        public int visits;
        public double wins;
        public List<Node> childNodes;
        public Node parent;

        public Node(int idplayer, Vector4 move, Node parent, int[,] boardArray, List<IPawn> reserve)
        {
            playerid = idplayer;
            this.move = move;
            wins = 0;
            visits = 0;
            childNodes = new List<Node>();
            this.parent = parent;
            this.piecesPosition = (int[,])boardArray.Clone();
            this.reserve = reserve;

        }

        public double ScoreValue()
        {
            const double explorationWeight = 2;
            if (visits == 0)
            {
                return double.MaxValue;
            }
            return (double)wins / visits + explorationWeight + Math.Sqrt(2 * Math.Log(parent.visits) / visits);
        }
    }
    public class _tempMonteCarlo : MonoBehaviour
    {
        [SerializeField] IGameManager boardManager;
        public SOPiece Kodama;
        public SOPiece KodamaSamourai;
        public SOPiece Kitsune;
        public SOPiece Koropokkuru;
        public SOPiece Tanuki;
        int[,] boardTab;
        private Dictionary<int, SOPiece> pieceDataDictionnary = new();

        private void OnEnable()
        {
            PopulateDictionnary();
        }
        private void PopulateDictionnary()
        {
            pieceDataDictionnary[-5] = Koropokkuru;
            pieceDataDictionnary[-4] = Kitsune;
            pieceDataDictionnary[-3] = Tanuki;
            pieceDataDictionnary[-2] = KodamaSamourai;
            pieceDataDictionnary[-1] = Kodama;
            pieceDataDictionnary[0] = null;
            pieceDataDictionnary[1] = Kodama;
            pieceDataDictionnary[2] = KodamaSamourai;
            pieceDataDictionnary[3] = Tanuki;
            pieceDataDictionnary[4] = Kitsune;
            pieceDataDictionnary[5] = Koropokkuru;
        }



        /// <summary>
        /// will turn the number of time we decide to make more precise moves
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="visits"></param>
        /// <returns></returns>
        public Vector4 MonteCarloSearch(Node rootNode, int visits,IGameManager iGameManager)
        {
            PopulateDictionnary();
            boardManager = iGameManager;
            if (boardManager != null)
            {
                boardTab = (int[,])rootNode.piecesPosition.Clone();              
                for (int i = 0; i < visits; i++)
                {
                    Node node = Selection(rootNode);
                    Expansion(node);
                    double result = Simulation(node);
                    BackPropagation(node, result);
                }

                Node bestChild = GetBestChild(rootNode);

                if (bestChild != null)
                {
                    //Debug.Log($"Meilleur coup trouve - Piece : {bestChild.Piece}, Position : {bestChild.move}, Score : {bestChild.wins}");
                    return new Vector4(bestChild.move.x, bestChild.move.y, bestChild.move.z, bestChild.move.w);
                }
                else
                {
                    Debug.Log($"Aucun meilleur coup trouve pourtant il y avait {rootNode.childNodes.Count} enfants ");
                }
            }
            return new Vector4();
        }

        private Node GetBestChild(Node node)
        {
            Node bestChild = null;
            double maxScore = double.MinValue;
            foreach (Node child in node.childNodes)
            {
                //Debug.Log(child.wins);
                // Vous pouvez �galement choisir le meilleur enfant en utilisant la valeur UCB ici
                double ucbValue = child.ScoreValue();
                if (ucbValue > maxScore)
                {
                    //Debug.Log($"coup {child.move} avec comme score {child.wins}");
                    maxScore = ucbValue;
                    bestChild = child;
                }

                //if (child.wins > maxScore)
                //{
                //    maxScore = child.wins;
                //    bestChild = child;
                //}
            }
            //Debug.Log($" move : {bestChild.move} score : {bestChild.wins}");
            return bestChild;
        }


        /// <summary>
        /// choose a node in wich we will start the exploration based on UCB 
        /// </summary>
        /// <param name="id"></param>
        private Node Selection(Node node)
        {
            Node currentNode = node;
            while (currentNode.childNodes.Count > 0)
            {
                double maxScoreValue = double.MinValue;
                Node selectedChild = null;

                foreach (Node child in currentNode.childNodes)
                {

                    double scoreValue = child.ScoreValue();
                    if (scoreValue > maxScoreValue)
                    {
                        maxScoreValue = scoreValue;
                        selectedChild = child;
                    }
                }
                currentNode = selectedChild;
            }
            return currentNode;
        }
        /// <summary>
        /// after the expansion -> it chooses a child node to simulate the game
        /// mais pour l'instant win ne se passe jamais ou draw non plus
        /// et j'ai un soucis avec l'index random
        /// </summary>
        /// <returns></returns>
        private double Simulation(Node node)
        {
            double score = new double();
            int maxIterations = 100;
            int currentIteration = 0;

            int currentPlayer = node.playerid;
            int[,] currentBoard = (int[,])node.piecesPosition.Clone();

            while (currentIteration < maxIterations)
            {
                currentIteration++;
                List<Vector3> playerPieces = GetAllPiecesOfPlayer(node);
                List<Vector4> validMoves = new List<Vector4>();

                foreach (Vector3 piece in playerPieces)
                {
                    List<Vector4> piecesMoves = GetValidMoves(piece, currentPlayer);
                    validMoves.AddRange(piecesMoves);
                }

                if (validMoves.Count == 0)
                {
                    return 0; // La partie est termin�e, aucun joueur n'a gagn�
                }
                Vector4 temp = validMoves[UnityEngine.Random.Range(0, validMoves.Count - 1)];
                Vector3 randomMove = new Vector3(temp.x, temp.y, temp.z);

                currentBoard = ReplaceInTab(currentBoard, randomMove.z, new Vector2(randomMove.x, randomMove.y));

                if (IsWinner(currentBoard, currentPlayer))
                {
                    //Debug.Log("coup finale pour l'ia "+randomMove);
                    score = double.MaxValue / currentIteration; // Le joueur actuel a gagn�, attribuer un score �lev�

                }
                else if (IsWinner(currentBoard, -currentPlayer))
                {
                    //Debug.Log("coup finale pour l'adversaire");
                    score -= 9999999999999; // Le joueur adverse a gagn�, attribuer un score tr�s bas
                }
                else
                {

                    if (korSafe(currentBoard, currentPlayer))
                    {
                        //Debug.Log("le roi est protegé");
                        score += 500000000;
                    }
                    else
                    {
                        if (hasMorePieces(currentBoard, currentPlayer))
                        {
                            //Debug.Log("j'ai plus de piece");
                            score += 50000;
                        }
                        else
                        {
                            //Debug.Log("coup de merde"+ randomMove);
                            score -= 999;
                        }
                    }
                }
            }

            return score;
        }
        /// <summary>
        /// after selection -> if the node haven t all the solutions it add node 
        /// </summary>
        /// <returns></returns>
        private void Expansion(Node node)
        {
            // we need to generate all the nodes based on every possible moves 
            List<Vector3> posidpiece = GetAllPiecesOfPlayer(node);
            List<IPawn> reserve = new();

            reserve = node.reserve; // List<IPawn> reserve (déja la bonne reserve) comment quon fait
            

            if (reserve != null)
            {
                foreach (var pieceid in reserve)
                {
                    posidpiece.Add(new Vector3(-1, -1, TransformIpawnIntoId(pieceid,node.playerid)));
                }

            }

            foreach (Vector3 piece in posidpiece)
            {
                List<Vector4> moves = GetValidMoves(piece, node.playerid);
                //Debug.Log($"id joueur: {node.playerid}");
                foreach (Vector4 move in moves)
                {
                    //Debug.Log($"move : {move}");
                    Node child = new Node(node.playerid, move, node, ReplaceInTab(node.piecesPosition, piece.z, move), node.reserve);
                    node.childNodes.Add(child);
                    child.parent = node;

                }
            }

        }
        /// <summary>
        /// after the simulation -> the result is propaged to the node chosen and all its parents 
        /// it updates the statistiques of each node (number of win and number of simulation)
        /// </summary>
        /// <returns></returns>
        private void BackPropagation(Node node, double result)
        {
            //Debug.Log("backpropagate");
            while (node != null)
            {
                node.visits++;
                node.wins += result;
                if (node.parent != null)
                {
                    node = node.parent;
                   
                }
                else break;
            }
        }

        //----------------- complementary funct ----------------------
        public bool hasMorePieces(int[,] array, int playerid)
        {
            List<int> piecesAi = new List<int>();
            List<int> pieces = new List<int>();

            int width = array.GetLength(0);
            int height = array.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                for (int k = 0; k < height; k++)
                {
                    int value = array[i, k];
                    if (Math.Sign(value) == Math.Sign(playerid))
                    {
                        piecesAi.Add(value);
                    }
                    else
                    {
                        pieces.Add(value);
                    }

                }
            }
            int numbPiecesAi = piecesAi.Count;
            int numbPieces = pieces.Count;
            if (numbPiecesAi > numbPieces)
            {
                return true;
            }
            else if (numbPiecesAi == numbPieces)
            {
                if (sumOfList(piecesAi) > sumOfList(pieces))
                {
                    return true;
                }
            }
            return false;
        }
        private int sumOfList(List<int> list)
        {
            int sum = 0;
            foreach (int integer in list)
            {
                sum += integer;
            }
            return sum;
        }

        public bool korSafe(int[,] array, int playerid)
        {
            Vector2 korPosition = new Vector2();
            int width = array.GetLength(0);
            int height = array.GetLength(1);
            for (int i = 0; i < width; i++)
            {
                for (int k = 0; k < height; k++)
                {
                    if (array[i, k] == 10 * playerid)
                    {
                        korPosition = new Vector2(i, k);
                    }

                }
            }

            bool otherThanKor = false;

            // verificate if the king is in danger 
            for (int i = (int)Math.Max(0, korPosition.x - 1); i <= Math.Min(korPosition.x + 1, width - 1); i++)
            {
                for (int j = (int)Math.Max(0, korPosition.y - 1); j <= Math.Min(korPosition.y + 1, height - 1); j++)
                {
                    if (i != korPosition.x || j != korPosition.y)
                    {
                        if (array[i, j] != 0 && Math.Sign(array[i, j]) != Math.Sign(playerid))
                        {
                            otherThanKor = true;
                            break;
                        }
                    }
                }
                if (otherThanKor) break;
            }

            return !otherThanKor;
        }
        private List<Vector4> GetValidMoves(Vector3 posplusidpiece, int player)
        {
            //valid moves pos x,y , idpiece , isParachuting
            SOPiece so = pieceDataDictionnary[(int)posplusidpiece.z];
            List<Vector4> validMoves = new List<Vector4>();
            if (posplusidpiece.x < 0)
            {
                // c'est un parachutage 
                for (int i = 0; i < boardTab.GetLength(0); i++)
                {
                    for (int k = 0; k < boardTab.GetLength(1); k++)
                    {
                        int newX = i;
                        int newY = k;
                        if (IsInsideBoard(newX, newY) && IsReachableParachuting(new Vector2(newX, newY)))
                        {
                            validMoves.Add(new Vector4(newX, newY, posplusidpiece.z, 1));
                        }
                    }
                }
            }
            else
            {
                int[] deltaX;
                int[] deltaY;

                deltaX = new int[] { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
                deltaY = new int[] { -1, -1, -1, 0, 0, 0, 1, 1, 1 };
                // take into account the player ROTATION

                if (player != 1)
                {
                    for (int i = 0; i < deltaX.Length; i++)
                    {
                        deltaX[i] *= -1;
                        deltaY[i] *= -1;
                    }
                }

                for (int i = 0; i < so.PossibleMoves.Count; i++)
                {
                    if (so.PossibleMoves[i])
                    {
                        int newX = (int)posplusidpiece.x + deltaX[i];
                        int newY = (int)posplusidpiece.y + deltaY[i];
                        if (IsInsideBoard(newX, newY) && IsReachable(new Vector2(newX, newY), player))
                        {
                            validMoves.Add(new Vector4(newX, newY, posplusidpiece.z, 0));
                        }
                    }
                }
            }
            return validMoves;
        }

        private bool IsInsideBoard(int x, int y)
        {
            if (boardTab == null || boardTab.Length == 0)
            {
                Debug.LogError("tempBoardArray is null or empty");
                return false;
            }

            int numRows = boardTab.GetLength(0);
            int numCols = boardTab.GetLength(1);
            return x >= 0 && x < numRows && y >= 0 && y < numCols;
        }
        private bool IsReachableParachuting(Vector2 pos)
        {
            if (boardTab[(int)pos.x, (int)pos.y] != 0)
            {
                return false;
            }
            return true;
        }
        private bool IsReachable(Vector2 pos, int player)
        {
            if (boardTab[(int)pos.x, (int)pos.y] != 0)
            {
                int pieceValue = boardTab[(int)pos.x, (int)pos.y];
                if (Math.Sign(pieceValue) != Math.Sign(player))
                {
                    return true;
                }
                else if (Math.Sign(pieceValue) == Math.Sign(player))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
        private List<Vector3> GetAllPiecesOfPlayer(Node node)
        {
            List<Vector3> positionArrayAndPiecesId = new();
            int playerid = node.playerid;
            List<IPawn> reserve = new();
           
            reserve = node.reserve;
          
            //Debug.Log($"id du joueur : { playerid}");
            for (int x = 0; x < node.piecesPosition.GetLength(0); x++)
            {
                for (int y = 0; y < node.piecesPosition.GetLength(1); y++)
                {
                    int value = node.piecesPosition[x, y];
                    if (value != 0 && Math.Sign(value) == Math.Sign(playerid))
                    {
                        positionArrayAndPiecesId.Add(new Vector3(x, y, value));
                    }
                }
            }
            if (reserve != null)
            {
                for (int i = 0; i < reserve.Count; i++)
                {
                    int pieceV = TransformIpawnIntoId(reserve[i],playerid);
                    positionArrayAndPiecesId.Add(new Vector3(-1, -1, pieceV));
                }
            }
            return positionArrayAndPiecesId;
        }
       
        int[,] ReplaceInTab(int[,] array1, float value, Vector2 newposition)
        {
            int lignes = array1.GetLength(0);
            int colonnes = array1.GetLength(1);

            for (int i = 0; i < lignes; i++)
            {
                for (int j = 0; j < colonnes; j++)
                {
                    if (array1[i, j] == value)
                    {
                        // La pièce avec la valeur spécifiée a été trouvée

                        array1[i, j] = 0;
                        array1[(int)newposition.x, (int)newposition.y] = (int)value;
                        return array1;
                    }

                }

            }
            return array1;
        }

        public bool IsWinner(int[,] array, int playerid)
        {
            bool stillourkor = false;
            bool stillotherkor = false;
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int k = 0; k < array.GetLength(1); k++)
                {
                    //Debug.Log(array[i, k]);
                    if (array[i, k] == 5 * playerid)
                    {
                        stillourkor = true;
                    }
                    else if (array[i, k] == 5 * playerid * -1)
                    {
                        stillotherkor = true;
                    }
                }
            }

            //Debug.Log($"avons nous toujours notre roi {stillourkor} et est ce que l'adversaire a toujours son roi {stillotherkor}");
            if (stillourkor && !stillotherkor)
            {
                return true;
            }
            else
            {

                return false;
            }
        }
        public bool IsDraw(int[,] array)
        {
            return false;
        }
        private bool IsKingEatable(int[,] board, int currentPlayer, Vector3 move)
        {
            int kingValue = 5; // Valeur du roi
            int opponentPlayer = -currentPlayer;

            // simuler une potentiel capture du roi adverse
            int[,] simulatedBoard = (int[,])board.Clone(); // Cloner le tableau pour la simulation
            Vector2 newPosition = new Vector2(move.x, move.y);
            int pieceId = (int)move.z;

            // Mettre à jour le tableau simulé avec le mouvement
            simulatedBoard = ReplaceInTab(simulatedBoard, pieceId, newPosition);

            // cherche la position du roi adverse dans le tableau
            Vector2 kingPosition = FindPiece(simulatedBoard, kingValue * opponentPlayer);

            // Parcourir les cases voisines du roi pour vérifier s'il est capturable
            for (int i = Mathf.Max(0, (int)kingPosition.x - 1); i <= Mathf.Min(simulatedBoard.GetLength(0) - 1, (int)kingPosition.x + 1); i++)
            {
                for (int j = Mathf.Max(0, (int)kingPosition.y - 1); j <= Mathf.Min(simulatedBoard.GetLength(1) - 1, (int)kingPosition.y + 1); j++)
                {
                    if (simulatedBoard[i, j] != 0 && simulatedBoard[i, j] / Mathf.Abs(simulatedBoard[i, j]) == opponentPlayer)
                    {
                        // vérifier si la pièce voisine en question peut se déplacer vers le roi
                        Vector3 piecePos = new Vector3(i, j, simulatedBoard[i, j]);
                        List<Vector4> possibleMoves = GetValidMoves(piecePos, opponentPlayer);
                        foreach (Vector4 possibleMove in possibleMoves)
                        {
                            if (possibleMove.x == kingPosition.x && possibleMove.y == kingPosition.y)
                            {
                                // Une pièce adverse peut capturer le roi
                                return true;
                            }
                        }
                    }
                }
            }

            return false; // Le roi adverse n'est pas capturable dans ce mouvement
        }



        private Vector2 FindPiece(int[,] board, int pieceValue)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == pieceValue)
                    {
                        return new Vector2(i, j); // Retourner la position de la pièce
                    }
                }
            }
            return new Vector2(-1, -1); // La pièce n'a pas été trouvée sur le plateau
        }

        private int TransformIpawnIntoId(IPawn pawn,int idPlayer)
        {
            EPawnType type = pawn.GetPawnType();
            switch (type)
            {
                case EPawnType.Kodama:
                    return 1*idPlayer;
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
    }
}