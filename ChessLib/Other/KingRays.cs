using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ChessLib.FieldModels;
using ChessLib.PlayerModels;
using ChessLib.Figures;
using ChessLib.Enums.Players;

namespace ChessLib.Other
{
    public class KingRays
    {
        public List<List<(int, int)>> AllRays { get; set; }
        public List<(int, int)> FigsThatHitsKing { get; set; }
        public List<(int, int)> FigsThatProtectsKing { get; set; }
        public (int, int) KingCord { get; set; }

        List<(int, int)> crossDirections = new List<(int, int)>() 
        { (-1, 1), (1, 1), (1, -1), (-1, -1) };
        List<(int, int)> forwardDirections = new List<(int, int)>() 
        { (-1, 0), (0, 1), (1, 0), (0, -1) };
        private List<(int, int)> horseDirectinos = new List<(int, int)>()
        {(-2, -1), (-1, -2), (1, -2), (2, -1), (2,1), (1,2), (-1, 2), (-2, 1) };

        public KingRays()
        {
            AllRays = new List<List<(int, int)>>();
            FigsThatHitsKing = new List<(int, int)>(); ;
            FigsThatProtectsKing = new List<(int, int)>();
            KingCord = (-1, -1);
        }

        public void GetKingsRayses(Field field, Player player)
        {
            KingCord = field.GetKingsCord(player);

            if (KingCord.Item1 < 0 || KingCord.Item2 < 0)
            {
                return;
            }

            for (int i = 0; i < crossDirections.Count; i++)
            {
                List<(int, int)> ray = GetRayForKingsRay(field, player, KingCord, crossDirections[i]);

                if (ray.Count > 0)
                {
                    AllRays.Add(ray);
                }
            }

            for (int i = 0; i < forwardDirections.Count; i++)
            {
                List<(int, int)> ray = GetRayForKingsRay(field, player, KingCord, forwardDirections[i]);

                if (ray.Count > 0)
                {
                    AllRays.Add(ray);
                }
            }

            for (int i = 0; i < horseDirectinos.Count; i++)
            {
                (int, int) cordToCheck = (KingCord.Item1 + horseDirectinos[i].Item1,
                    KingCord.Item2 + horseDirectinos[i].Item2);

                if (!field.IfChipIsOutOfRange(cordToCheck) &&
                    field.AllCells[cordToCheck.Item1, cordToCheck.Item2].Figure is Horse &&
                    field.AllCells[cordToCheck.Item1, cordToCheck.Item2].Figure.FigureColor != player.Color)
                {
                    AllRays.Add(new List<(int, int)>() { cordToCheck });
                }
            }

            List<List<(int, int)>> asd = GetRaysForPawn(field, player, KingCord);

            AllRays.AddRange(asd);

            for (int i = 0; i < AllRays.Count; i++)
            {
                for (int j = 0; j < AllRays[i].Count; j++)
                {
                    (int, int) tempCord = AllRays[i][j];

                    if (field.AllCells[tempCord.Item1, tempCord.Item2].Figure != null)
                    {
                        if (field.IfChipIsPlayers(player, tempCord))
                        {
                            FigsThatProtectsKing.Add(tempCord);
                        }
                        else
                        {
                            FigsThatHitsKing.Add(tempCord);
                        }
                        break;
                    }
                }
            }
        }
        public List<List<(int, int)>> GetRaysForPawn(Field field, Player player, (int, int) kingCord)
        {
            List<List<(int, int)>> res = new List<List<(int, int)>>();
            if (player.Side == PlayerSide.Down)
            {
                List<(int, int)> pawnsDirs = new List<(int, int)>() { (-1, -1), (-1, 1) };

                for (int i = 0; i < pawnsDirs.Count; i++)
                {
                    (int, int) cordToCheck = (kingCord.Item1 + pawnsDirs[i].Item1,
                        kingCord.Item2 + pawnsDirs[i].Item2);

                    if (!field.IfChipIsOutOfRange(cordToCheck) &&
                        field.AllCells[cordToCheck.Item1, cordToCheck.Item2].Figure is Pawn &&
                        field.AllCells[cordToCheck.Item1, cordToCheck.Item2].Figure.FigureColor != player.Color)
                    {
                        res.Add(new List<(int, int)>() { cordToCheck });
                    }

                }
            }
            else//upper player
            {
                List<(int, int)> pawnsDirs = new List<(int, int)>() { (1, -1), (1, 1) };

                for (int i = 0; i < pawnsDirs.Count; i++)
                {
                    (int, int) cordToCheck = (kingCord.Item1 + pawnsDirs[i].Item1,
                        kingCord.Item2 + pawnsDirs[i].Item2);

                    if (!field.IfChipIsOutOfRange(cordToCheck) &&
                        field.AllCells[cordToCheck.Item1, cordToCheck.Item2].Figure is Pawn &&
                        field.AllCells[cordToCheck.Item1, cordToCheck.Item2].Figure.FigureColor != player.Color)
                    {
                        res.Add(new List<(int, int)>() { cordToCheck });
                    }

                }
            }
            return res;
        }
        public List<(int, int)> GetRayForKingsRay(Field field, Player player, (int, int) kingCord, (int, int) direction)
        {
            List<(int, int)> res = new List<(int, int)>();

            int playersFig = 0;
            (int, int) tempCord = kingCord;

            do
            {
                tempCord = (tempCord.Item1 + direction.Item1, tempCord.Item2 + direction.Item2);
                res.Add(tempCord);

                if (field.IfChipIsOutOfRange(tempCord))
                {
                    return new List<(int, int)>();
                }
                if (field.IfChipIsPlayers(player, tempCord))
                {
                    playersFig++;

                    if (playersFig == 2)
                    {
                        return new List<(int, int)>();
                    }
                }
                if (field.AllCells[tempCord.Item1, tempCord.Item2].Figure != null)
                {
                    if (field.IfChipIsEnemys(player, tempCord))
                    {
                        if (field.AllCells[tempCord.Item1, tempCord.Item2].Figure is Queen ||
                            (crossDirections.Contains(direction) && (field.AllCells[tempCord.Item1, tempCord.Item2].Figure is Bishop)) ||
                            (forwardDirections.Contains(direction) && (field.AllCells[tempCord.Item1, tempCord.Item2].Figure is Rook)))
                        {
                            return res;
                        }
                        return new List<(int, int)>();
                    }
                }
            } while (true);

        }
    }
}
